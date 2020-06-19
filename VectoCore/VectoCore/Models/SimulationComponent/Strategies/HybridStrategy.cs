using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class HybridStrategy : LoggingObject, IHybridControlStrategy
	{
		private VectoRunData ModelData;
		private IDataBus DataBus;

		protected Dictionary<PowertrainPosition, NewtonMeter> ElectricMotorsOff;

		private bool ElectricMotorCanPropellDuringTractionInterruption;

		//private Second lastShiftTime;
		private Gearbox TestContainerGbx;
		private SimplePowertrainContainer TestContainer;
		protected readonly VelocityRollingLookup VelocityDropData;
		private SimpleHybridController TestContainerHybCtl;
		public Battery TestContainerBattery;
		private Clutch TestContainerClutch;


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

			var modData = new ModalDataContainer(runData, null, new[] { FuelData.Diesel }, null, false);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimpleHybridPowertrain(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			TestContainerHybCtl = TestContainer.HybridController as SimpleHybridController;
			TestContainerBattery = TestContainer.BatteryInfo as Battery;
			TestContainerClutch = TestContainer.ClutchInfo as Clutch;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype in TestContainer: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			if (TestContainerHybCtl == null) {
				throw new VectoException("Unknown HybridController in TestContainer: {0}", TestContainer.HybridController.GetType().FullName);
			}

			// register pre-processors
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			VelocityDropData = new VelocityRollingLookup();
			vehicleContainer.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption, TestContainer, -grad, grad, 2));

			var shiftStrategyParameters = runData.GearshiftParameters;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}
			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		
		public virtual IHybridController Controller { protected get; set; }
		

		public virtual HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Halt) {
				return HandleHaltAction(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}


			HybridResultEntry tmp = null;
			Tuple<bool, uint> gs = null;
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate || DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
				if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
					tmp = FindSolution(absTime, dt, outTorque, outAngularVelocity);
					if (tmp != null) {
						gs = HandleGearshift(absTime, tmp.Response);
					}
				} else {
					tmp = new HybridResultEntry {
						U = double.NaN,
						Response = null,
						Setting = new HybridStrategyResponse() {
							GearboxInNeutral = false,
							CombustionEngineOn = true,
							MechanicalAssistPower = ElectricMotorsOff
						},
						FuelCosts = double.NaN
					};
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Roll || DataBus.DriverInfo.DrivingAction == DrivingAction.Coast) {
				tmp = new HybridResultEntry {
					U = double.NaN,
					Response = null,
					Setting = new HybridStrategyResponse() {
						GearboxInNeutral = false,
						CombustionEngineOn = true,
						MechanicalAssistPower = ElectricMotorsOff
					},
					FuelCosts = double.NaN
				};
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && tmp == null) {
				tmp = MaxRecuperationSetting(absTime, dt, outTorque, outAngularVelocity);
				gs = HandleGearshift(absTime, tmp.Response);
			}


			var retVal = new HybridStrategyResponse() {
				CombustionEngineOn = tmp.Setting.CombustionEngineOn,
				GearboxInNeutral = tmp.Setting.GearboxInNeutral,
				MechanicalAssistPower = tmp.Setting.MechanicalAssistPower,
				ShiftRequired = gs?.Item1 ?? false,
				NextGear = gs?.Item2 ?? 0,
			};
			Response = dryRun ? null : retVal;
			if (!dryRun) {
				Solution = tmp;
			}
			return retVal;
		}

		public HybridResultEntry Solution { get; set; }

		private HybridResultEntry MaxRecuperationSetting(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var first = new HybridStrategyResponse() {
				CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
				GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
				MechanicalAssistPower = ElectricMotorsOff
			};
			var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, first);

		
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var emTorque = firstResponse.ElectricMotor.MaxRecuperationTorque;
			return TryConfiguration(absTime, dt, outTorque, outAngularVelocity, emPos, emTorque, double.NaN);
		}

		private HybridResultEntry FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var first = new HybridStrategyResponse() {
				CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
				GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
				MechanicalAssistPower = ElectricMotorsOff
			};
			var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, first);

			//var gearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime);
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var emTqReq = (firstResponse.ElectricMotor.PowerRequest + firstResponse.ElectricMotor.InertiaPowerDemand)  / firstResponse.ElectricMotor.AngularVelocity;
			var responses = new List<HybridResultEntry>();

			var entry = new HybridResultEntry() {
				U = double.NaN,
				Response = firstResponse,
				Setting = first,

				//Score = CalcualteCosts(firstResponse, dt)
			};
			CalcualteCosts(firstResponse, dt, entry);
			responses.Add(entry);

			if (firstResponse.Gearbox.Gear == 0 && !ElectricMotorCanPropellDuringTractionInterruption) {
				return responses.First();
			}

			var minimumShiftTimePassed = (DataBus.GearboxInfo.LastShift + ModelData.GearboxData.ShiftTime).IsSmallerOrEqual(absTime);
			var gearRangeUpshift = ModelData.GearshiftParameters.AllowedGearRangeUp;
			var gearRangeDownshift = ModelData.GearshiftParameters.AllowedGearRangeDown;
			if (!minimumShiftTimePassed || (absTime - DataBus.GearboxInfo.LastUpshift).IsSmaller(ModelData.GearboxData.DownshiftAfterUpshiftDelay)) {
				gearRangeDownshift = 0;
			}
			if (!minimumShiftTimePassed || (absTime - DataBus.GearboxInfo.LastDownshift).IsSmaller(ModelData.GearboxData.UpshiftAfterDownshiftDelay)) {
				gearRangeUpshift = 0;
			}

			var gear = DataBus.GearboxInfo.Gear;
			var numGears = ModelData.GearboxData.Gears.Count;
			for (var nextGear = Math.Max(1, gear - gearRangeDownshift);
				nextGear <= Math.Min(numGears, gear + gearRangeUpshift);
				nextGear++) 
				{
					if (!ElectricMotorCanPropellDuringTractionInterruption) {
						// em is at gearbox input side - angular speed is different so get new 
					}
				IterateEMTorque(absTime, dt, outTorque, outAngularVelocity, firstResponse, emTqReq, emPos, responses);
			}

			if (responses.All(x => double.IsNaN(x.Score))) {
				return null;
			}

			var best = responses.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).First();
			return best;
		}

		private void IterateEMTorque(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, ResponseDryRun firstResponse,
			NewtonMeter emTqReq, PowertrainPosition emPos, List<HybridResultEntry> responses)
		{
			const double stepSize = 0.1;

			// iterate over 'EM provides torque'. allow EM to provide more torque in order to overcome ICE inertia
			var maxU = Math.Min((firstResponse.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>()) / emTqReq, -1.0);
			if (firstResponse.ElectricMotor.MaxDriveTorque != null) {
				for (var u = 0.0; u >= maxU; u -= stepSize * (u < -4 ? 10 : (u < -2 ? 5 : 1))) {
					var emTorque = emTqReq.Abs() * u;
					if (!emTorque.IsBetween(
						0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, emPos, emTorque, u);
					responses.Add(tmp);
				}

				// make sure the max drive point is also covered.
				var emTorqueM = emTqReq * maxU;
				if (emTorqueM.IsBetween(
					0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, emPos, emTorqueM, maxU);
					responses.Add(tmp);
				}
			}

			// iterate over 'EM recuperates' up to max available recuperation potential
			if (firstResponse.ElectricMotor.MaxRecuperationTorque != null) {
				for (var u = stepSize; u <= 1.0; u += stepSize) {
					var emTorque = firstResponse.ElectricMotor.MaxRecuperationTorque * u;
					if (!(emTorque).IsBetween(
						firstResponse.ElectricMotor.MaxRecuperationTorque, 0.SI<NewtonMeter>())) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, emPos, emTorque, u);
					responses.Add(tmp);
				}
			}
		}

		private HybridResultEntry TryConfiguration(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, PowertrainPosition emPos,
			NewtonMeter emTorque, double u)
		{
			var cfg = new HybridStrategyResponse() {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>() {
					{ emPos, emTorque }
				}
			};
			var resp = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, cfg);
			
			var tmp = new HybridResultEntry {
				U = u,
				Setting = cfg,
				Response = resp,
				//Score = cost
			};
			CalcualteCosts(resp, dt, tmp);
			return tmp;
		}

		private ResponseDryRun RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, HybridStrategyResponse cfg)
		{
			TestContainerHybCtl.ApplyStrategySettings(cfg);
			TestContainerGbx.Gear = DataBus.GearboxInfo.Gear;
			TestContainerHybCtl.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			TestContainerClutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			TestContainerBattery.Initialize(DataBus.BatteryInfo.StateOfCharge);
			var retVal = TestContainerHybCtl.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, true);
			var retVal2 = Controller.RequestDryRun(absTime, dt, outTorque, outAngularVelocity, cfg);
			return retVal as ResponseDryRun;
		}

		private void CalcualteCosts(ResponseDryRun resp, Second dt, HybridResultEntry tmp)
		{
			if (!resp.Engine.TotalTorqueDemand.IsBetween(
				resp.Engine.DragTorque, resp.Engine.DynamicFullLoadTorque)) {
				tmp.FuelCosts = double.NaN;
			}

			tmp.FuelCosts = ModelData.EngineData.Fuels.Sum(
				x => (x.ConsumptionMap.GetFuelConsumptionValue(resp.Engine.TotalTorqueDemand, resp.Engine.EngineSpeed)
					 * x.FuelData.LowerHeatingValueVecto * dt).Value());
			tmp.BatCosts = -(resp.ElectricSystem.ConsumerPower * dt).Value();
			tmp.SoCPenalty = 1 - Math.Pow((DataBus.BatteryInfo.StateOfCharge - ModelData.BatteryData.TargetSoC) / (0.5 * (ModelData.BatteryData.MaxSOC - ModelData.BatteryData.MinSOC)), 5);

			tmp.EqualityFactor = 2.5;
		}

		private Tuple<bool, uint> HandleGearshift(Second absTime, ResponseDryRun response)
		{
			var retVal = Tuple.Create(false, response.Gearbox.Gear);

			var gear = DataBus.GearboxInfo.Gear;
			var _nextGear = gear;
			var outSpeed = (response.Gearbox.GearboxInputSpeed ?? response.Engine.EngineSpeed ) / ModelData.GearboxData.Gears[gear].Ratio;
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

			if (response.Engine.EngineSpeed != null && gear < ModelData.GearboxData.Gears.Count  && ModelData.GearboxData.Gears[gear].ShiftPolygon.IsAboveUpshiftCurve(response.Engine.TorqueOutDemand, response.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				retVal = Tuple.Create(true, response.Gearbox.Gear + 1);
			}
			if (response.Engine.EngineSpeed != null && gear > 0  && ModelData.GearboxData.Gears[gear].ShiftPolygon.IsBelowDownshiftCurve(response.Engine.TorqueOutDemand, response.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				retVal = Tuple.Create(true, response.Gearbox.Gear - 1);
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

		public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.HybridStrategyScore] = Solution?.Score ?? 0;
			container[ModalResultField.HybridStrategySolution] = Solution?.U ?? -100;
		}

		[DebuggerDisplay("{U}: {Score} {Setting.MechanicalAssistPower}")]
		public class HybridResultEntry
		{
			public double U { get; set; }

			public HybridStrategyResponse Setting { get; set; }

			public ResponseDryRun Response { get; set; }

			public double Score { get { return FuelCosts + EqualityFactor * BatCosts * SoCPenalty; } }

			public double FuelCosts { get; set; }

			public double BatCosts { get; set; }

			public double SoCPenalty { get; set; }

			public double EqualityFactor { get; set; }
		}

	}

	
}