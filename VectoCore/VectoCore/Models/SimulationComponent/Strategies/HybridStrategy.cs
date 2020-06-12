using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class HybridStrategy : IHybridControlStrategy
	{
		private VectoRunData ModelData;
		private IDataBus DataBus;

		protected Dictionary<PowertrainPosition, NewtonMeter> ElectricMotorsOff;

		private bool ElectricMotorCanPropellDuringTractionInterruption;

		//private Second lastShiftTime;

		protected HybridStrategyResponse Response { get; set; }

		public HybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer)
		{
			DataBus = vehicleContainer;
			ModelData = runData;
			if (ModelData.ElectricMachinesData.Select(x => x.Item1).Distinct().Count() > 1) {
				throw new VectoException("More than one electric motors are currently not supported");
			}

			ElectricMotorsOff = ModelData.ElectricMachinesData
										.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
										.ToDictionary(x => x.Key, x => x.Value);
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			ElectricMotorCanPropellDuringTractionInterruption =
				emPos == PowertrainPosition.HybridP4 || emPos == PowertrainPosition.HybridP3;
		}

		public virtual IHybridController Controller { protected get; set; }

		public virtual HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Halt) {
				return HandleHaltAction(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}

			Tuple<double, ResponseDryRun, HybridStrategyResponse, double> tmp = null;
			Tuple<bool, uint> gs = null;
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate) {
				if (DataBus.GearboxInfo.GearEngaged(absTime)) {
					tmp = FindSolution(absTime, dt, outTorque, outAngularVelocity);
					gs = HandleGearshift(absTime, tmp.Item2);
				} else {
					tmp = new Tuple<double, ResponseDryRun, HybridStrategyResponse, double>(double.NaN, null, new HybridStrategyResponse() {
						GearboxInNeutral = false,
						CombustionEngineOn = true,
						MechanicalAssistPower = ElectricMotorsOff
					}, double.NaN);
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Roll || DataBus.DriverInfo.DrivingAction == DrivingAction.Coast) {
				tmp = new Tuple<double, ResponseDryRun, HybridStrategyResponse, double>(double.NaN, null, new HybridStrategyResponse() {
					GearboxInNeutral = false,
					CombustionEngineOn = true,
					MechanicalAssistPower = ElectricMotorsOff
				}, double.NaN);
			}


			var retVal = new HybridStrategyResponse() {
				CombustionEngineOn = tmp.Item3.CombustionEngineOn,
				GearboxInNeutral = tmp.Item3.GearboxInNeutral,
				MechanicalAssistPower = tmp.Item3.MechanicalAssistPower,
				ShiftRequired = gs?.Item1 ?? false,
				NextGear = gs?.Item2 ?? 0,
			};
			Response = dryRun ? null : retVal;
			return retVal;
		}

		private Tuple<double, ResponseDryRun, HybridStrategyResponse, double> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var first = new HybridStrategyResponse() {
				CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
				GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
				MechanicalAssistPower = ElectricMotorsOff
			};
			var firstResponse = Controller.RequestDryRun(absTime, dt, outTorque, outAngularVelocity, first);

			var gearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime);
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var emTqReq = (firstResponse.ElectricMotor.PowerRequest + firstResponse.ElectricMotor.InertiaPowerDemand)  / firstResponse.ElectricMotor.AngularVelocity;
			var responses = new List<Tuple<double, ResponseDryRun, HybridStrategyResponse, double>>();
			for (var u = -1.0; u <= 1; u += 2.0 / 20) {
				if (!(emTqReq * u).IsBetween(
					firstResponse.ElectricMotor.MaxRecuperationTorque, firstResponse.ElectricMotor.MaxDriveTorque)) {
					continue;
				}
				var cfg = new HybridStrategyResponse() {
					CombustionEngineOn =  true,
					GearboxInNeutral = false,
					MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>() {
						{emPos,  emTqReq * u}
					}
				};
				var resp = Controller.RequestDryRun(absTime, dt, outTorque, outAngularVelocity, cfg);
				var cost = CalcualteCosts(resp, dt);
				responses.Add(Tuple.Create(u, resp, cfg, cost));
			}

			var best = responses.OrderBy(x => x.Item4).First();
			return best;
		}

		private double CalcualteCosts(ResponseDryRun resp, Second dt)
		{
			var fcCost = ModelData.EngineData.Fuels.Sum(
				x => (x.ConsumptionMap.GetFuelConsumptionValue(resp.Engine.EngineTorqueDemandTotal, resp.Engine.EngineSpeed)
					 * x.FuelData.LowerHeatingValueVecto * dt).Value());
			var eqnFactor = 2.5;
			var batCost = (resp.ElectricSystem.ConsumerPower * dt).Value();
			var socPenalty = 1 - Math.Pow((resp.ElectricSystem.BatteryResponse.StateOfCharge - DataBus.BatteryInfo.StateOfCharge) / (0.5 * (ModelData.BatteryData.MaxSOC - ModelData.BatteryData.MinSOC)), 5);

			var cost = fcCost + eqnFactor * batCost * socPenalty;
			return cost;
		}

		private Tuple<bool, uint> HandleGearshift(Second absTime, ResponseDryRun response)
		{
			var retVal = Tuple.Create(false, response.Gearbox.Gear);

			var gear = DataBus.GearboxInfo.Gear;
			var _nextGear = gear;
			var outSpeed = response.Engine.EngineSpeed / ModelData.GearboxData.Gears[gear].Ratio;
			// emergency shift to not stall the engine ------------------------
			if (gear == 1 && SpeedTooLowForEngine(_nextGear, outSpeed)) {
				retVal = Tuple.Create(true, 0u);
			}
			_nextGear = gear;
			while (_nextGear > 1 && SpeedTooLowForEngine(_nextGear, outSpeed)) {
				_nextGear--;
			}
			while (_nextGear < ModelData.GearboxData.Gears.Count &&
					SpeedTooHighForEngine(_nextGear, outSpeed)) {
				_nextGear++;
			}
			if (_nextGear != gear) {
				return Tuple.Create(true, _nextGear);
			}

			// normal shift when all requirements are fullfilled ------------------

			var minimumShiftTimePassed = (DataBus.GearboxInfo.LastShift + ModelData.GearboxData.ShiftTime).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return retVal;
			}

			if (response.Engine.EngineSpeed != null && response.Engine.EngineSpeed.IsGreaterOrEqual(1600.RPMtoRad())) {
				//lastShiftTime = absTime;
				retVal = Tuple.Create(true, response.Gearbox.Gear + 1);
			}
			return retVal;
		}


		private bool SpeedTooLowForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * ModelData.GearboxData.Gears[gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);
		}

		private bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * ModelData.GearboxData.Gears[gear].Ratio).IsGreaterOrEqual(VectoMath.Min(ModelData.GearboxData.Gears[gear].MaxSpeed,
																								DataBus.EngineInfo.EngineN95hSpeed));
		}

		private HybridStrategyResponse HandleHaltAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var tmp = new HybridStrategyResponse() {
				CombustionEngineOn = false,
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff
			};
			Response = dryRun ? null : tmp;
			return tmp;
		}

		public virtual HybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var retVal = new HybridStrategyResponse()
				{ MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>() };

			foreach (var em in ModelData.ElectricMachinesData) {
				retVal.MechanicalAssistPower[em.Item1] = null;
			}

			return retVal;
		}

		public virtual void CommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

	}

	
}