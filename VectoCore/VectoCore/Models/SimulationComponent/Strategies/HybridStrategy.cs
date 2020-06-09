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
			
		}

		public virtual IHybridController Controller { protected get; set; }

		public virtual HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Halt) {
				return HandleHaltAction(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}
			var first = new HybridStrategyResponse() {
				CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
				GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
				MechanicalAssistPower = ElectricMotorsOff
			};
			var firstResponse = Controller.RequestDryRun(absTime, dt, outTorque, outAngularVelocity, first);

			var gs = HandleGearshift(absTime, firstResponse);

			var tmp  = new HybridStrategyResponse() {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff,
				ShiftRequired = gs.Item1,
				NextGear = gs.Item2,
			};
			Response = dryRun ? null : tmp;
			return tmp;
		}

		private Tuple<bool, uint> HandleGearshift(Second absTime, ResponseDryRun response)
		{
			var retVal = Tuple.Create(false, response.Gearbox.Gear);

			var minimumShiftTimePassed = (DataBus.GearboxInfo.LastShift + ModelData.GearboxData.ShiftTime).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return retVal;
			}

			if (response.Engine.EngineSpeed != null && response.Engine.EngineSpeed.IsGreaterOrEqual(1700.RPMtoRad())) {
				//lastShiftTime = absTime;
				retVal = Tuple.Create(true, response.Gearbox.Gear + 1);
			}
			return retVal;
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