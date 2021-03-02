using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class HybridStrategy : AbstractHybridStrategy<Gearbox>
	{
		public HybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,
			vehicleContainer)
		{
			// register pre-processors
			vehicleContainer.AddPreprocessor(GetGearshiftPreprocessor(runData));
		}

		protected ISimulationPreprocessor GetGearshiftPreprocessor(VectoRunData runData)
		{
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			var modData = new ModalDataContainer(runData, null, null);
			var builder = new PowertrainBuilder(modData);
			var testContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimpleHybridPowertrain(runData, testContainer);

			return new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption,
				testContainer, -grad, grad, 2);
		}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.Gearbox.Gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;
			TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
			TestPowertrain.Gearbox.DisengageGearbox = !nextGear.Engaged;
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			TestPowertrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			TestPowertrain.SuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);

			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;

			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;

			if (nextGear.Engaged && !nextGear.Equals(currentGear)) {
				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}

				var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				if (!AllowEmergencyShift && !estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
					return null;
				}


				var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
				var vehicleSpeedPostShift = estimatedVelocityPostShift; // DataBus.VehicleInfo.VehicleSpeed - vDrop * ModelData.GearshiftParameters.VelocityDropFactor;
				TestPowertrain.Gearbox.Gear = nextGear;
				var init = TestPowertrain.Container.VehiclePort.Initialize(
					vehicleSpeedPostShift, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				if (!AllowEmergencyShift && init.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)) {
					return null;
				}
			}

			//if (!nextGear.Engaged) {
				TestPowertrain.Gearbox._nextGear = Controller.ShiftStrategy.NextGear;
				TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
			//}

			//if (!PreviousState.GearboxEngaged) {
			TestPowertrain.CombustionEngine.Initialize(
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque,
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed);
			TestPowertrain.CombustionEngine.PreviousState.EngineOn = //true;
					(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
			TestPowertrain.CombustionEngine.PreviousState.EnginePower =
					(DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
			TestPowertrain.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
			TestPowertrain.CombustionEngine.PreviousState.EngineSpeed =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
			TestPowertrain.CombustionEngine.PreviousState.EngineTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
			TestPowertrain.CombustionEngine.PreviousState.EngineTorqueOut =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
			TestPowertrain.CombustionEngine.PreviousState.DynamicFullLoadTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;
		
			switch (TestPowertrain.CombustionEngine.EngineAux) {
				case EngineAuxiliary engineAux:
					engineAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as EngineAuxiliary).PreviousState.AngularSpeed;
					break;
				case BusAuxiliariesAdapter busAux:
					busAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as BusAuxiliariesAdapter).PreviousState.AngularSpeed;
					break;
			}

			TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
				(DataBus.GearboxInfo as Gearbox).PreviousState.InAngularVelocity;

			TestPowertrain.Clutch.PreviousState.InAngularVelocity =
				(DataBus.ClutchInfo as SwitchableClutch).PreviousState.InAngularVelocity;
			TestPowertrain.Clutch.PreviousState.OutAngularVelocity =
				(DataBus.ClutchInfo as SwitchableClutch).PreviousState.OutAngularVelocity;

			//}

			var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).DeRatingActive;

			foreach (var emPos in TestPowertrain.ElectricMotorsUpstreamTransmission.Keys) {
				TestPowertrain.ElectricMotorsUpstreamTransmission[pos].PreviousState.EMSpeed =
					DataBus.ElectricMotorInfo(emPos).ElectricMotorSpeed;
			}

			var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, true);
			retVal.HybridController.StrategySettings = cfg;
			return retVal;
		}

	}

	// =====================================================


	public class HybridStrategyAT : AbstractHybridStrategy<ATGearbox>
	{

		

		public HybridStrategyAT(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,
			vehicleContainer)
		{

		}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.Gearbox.Gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;
			TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
			TestPowertrain.Gearbox.DisengageGearbox = !nextGear.Engaged;
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			//	TestPowertrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			
			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			TestPowertrain.SuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);

			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;

			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;

			if (nextGear.Engaged && !nextGear.Equals(currentGear)) {
				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}

				var vDrop = DataBus.DriverInfo.DriverAcceleration * ModelData.GearshiftParameters.ATLookAheadTime;
				var vehicleSpeedPostShift = (DataBus.VehicleInfo.VehicleSpeed + vDrop * ModelData.GearshiftParameters.VelocityDropFactor).LimitTo(
					0.KMPHtoMeterPerSecond(), DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);

				//var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				//if (!AllowEmergencyShift && !estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				//	return null;
				//}
				var inAngularVelocity = ModelData.GearboxData.Gears[nextGear.Gear].Ratio * outAngularVelocity;

				if (inAngularVelocity.IsEqual(0)) {
					return null;
				}
				var totalTransmissionRatio = inAngularVelocity / (DataBus.VehicleInfo.VehicleSpeed + DataBus.DriverInfo.DriverAcceleration * dt);

				var estimatedEngineSpeed = (vehicleSpeedPostShift * totalTransmissionRatio).Cast<PerSecond>();
				if (estimatedEngineSpeed.IsSmaller(ModelData.GearshiftParameters.MinEngineSpeedPostUpshift)) {
					return null;
				}

				//var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
				//var vehicleSpeedPostShift = estimatedVelocityPostShift; // DataBus.VehicleInfo.VehicleSpeed - vDrop * ModelData.GearshiftParameters.VelocityDropFactor;
				TestPowertrain.Gearbox.Gear = nextGear;
				TestPowertrain.Gearbox.RequestAfterGearshift = true;
				var init = TestPowertrain.Container.VehiclePort.Initialize(
					vehicleSpeedPostShift, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				if (!AllowEmergencyShift && init.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)) {
					return null;
				}
			} else {
				TestPowertrain.Gearbox.RequestAfterGearshift = (DataBus.GearboxInfo as ATGearbox).RequestAfterGearshift;
			}
			//TestPowertrain.Gearbox.ShiftToLocked = (DataBus.GearboxInfo as ATGearbox).ShiftToLocked;

			if (!nextGear.Engaged) {
				//TestPowertrain.Gearbox._nextGear = Controller.ShiftStrategy.NextGear;
				TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
			}

			//if (!PreviousState.GearboxEngaged) {
			TestPowertrain.CombustionEngine.Initialize(
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque,
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed);
			TestPowertrain.CombustionEngine.PreviousState.EngineOn = //true;
					(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
			TestPowertrain.CombustionEngine.PreviousState.EnginePower =
					(DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
			TestPowertrain.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
			TestPowertrain.CombustionEngine.PreviousState.EngineSpeed =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
			TestPowertrain.CombustionEngine.PreviousState.EngineTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
			TestPowertrain.CombustionEngine.PreviousState.EngineTorqueOut =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
			TestPowertrain.CombustionEngine.PreviousState.DynamicFullLoadTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;
			
			switch (TestPowertrain.CombustionEngine.EngineAux) {
				case EngineAuxiliary engineAux:
					engineAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as EngineAuxiliary).PreviousState.AngularSpeed;
					break;
				case BusAuxiliariesAdapter busAux:
					busAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as BusAuxiliariesAdapter).PreviousState.AngularSpeed;
					break;
			}

			TestPowertrain.Gearbox.PreviousState.OutAngularVelocity =
				(DataBus.GearboxInfo as ATGearbox).PreviousState.OutAngularVelocity;
			TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
				(DataBus.GearboxInfo as ATGearbox).PreviousState.InAngularVelocity;
			TestPowertrain.Gearbox._powershiftLossEnergy =
				(DataBus.GearboxInfo as ATGearbox)._powershiftLossEnergy;
			TestPowertrain.Gearbox.PreviousState.PowershiftLossEnergy =
				(DataBus.GearboxInfo as ATGearbox).PreviousState.PowershiftLossEnergy;
			TestPowertrain.Gearbox.LastShift =
				(DataBus.GearboxInfo as ATGearbox).LastShift;
			TestPowertrain.Gearbox.PreviousState.Gear =
				(DataBus.GearboxInfo as ATGearbox).PreviousState.Gear;

			if (nextGear.TorqueConverterLocked.HasValue && !nextGear.TorqueConverterLocked.Value) {
				TestPowertrain.TorqueConverter.PreviousState.InAngularVelocity =
					(DataBus.TorqueConverterInfo as TorqueConverter).PreviousState.InAngularVelocity;
				TestPowertrain.TorqueConverter.PreviousState.InTorque =
					(DataBus.TorqueConverterInfo as TorqueConverter).PreviousState.InTorque;
				TestPowertrain.TorqueConverter.PreviousState.OutAngularVelocity =
					(DataBus.TorqueConverterInfo as TorqueConverter).PreviousState.OutAngularVelocity;
				TestPowertrain.TorqueConverter.PreviousState.IgnitionOn =
					(DataBus.TorqueConverterInfo as TorqueConverter).PreviousState.IgnitionOn;
			}

			//TestPowertrain.Clutch.PreviousState.InAngularVelocity =
			//	(DataBus.ClutchInfo as SwitchableClutch).PreviousState.InAngularVelocity;

			//}

			var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).DeRatingActive;

			foreach (var emPos in TestPowertrain.ElectricMotorsUpstreamTransmission.Keys) {
				TestPowertrain.ElectricMotorsUpstreamTransmission[pos].PreviousState.EMSpeed =
					DataBus.ElectricMotorInfo(emPos).ElectricMotorSpeed;
			}

			try {
				var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque,
					outAngularVelocity, false);

				if (retVal.Source is TorqueConverter) {
					return null;
				}

				retVal.HybridController.StrategySettings = cfg;
				return retVal;
			} catch (Exception e) {
				Log.Debug(e);
				return null;
			}
		}
	}

	// =====================================================

	public abstract class AbstractHybridStrategy<T> : LoggingObject, IHybridControlStrategy where T: class, IHybridControlledGearbox, IGearbox 
	{

		public class StrategyState
		{
			public PerSecond AngularVelocity { get; set; }
			public HybridStrategyResponse Response { get; set; }
			public List<HybridResultEntry> Evaluations;
			public HybridResultEntry Solution { get; set; }

			public bool GearboxEngaged { get; set; }

			public Second ICEStartTStmp { get; set; }

			public Second GearshiftTriggerTstmp { get; set; }
			public NewtonMeter MaxGbxTq { get; set; }
		}

		public class DryRunSolutionState
		{
			public DryRunSolutionState(DrivingAction drivingAction, HybridResultEntry setting,
				List<HybridResultEntry> hybridResultEntries)
			{
				DrivingAction = drivingAction;
				Solution = setting;
				EvaluatedConfigs = hybridResultEntries;
			}


			public DrivingAction DrivingAction { get; }

			public HybridResultEntry Solution { get;  }

			public List<HybridResultEntry> EvaluatedConfigs { get; }
		}

		protected VectoRunData ModelData;
		protected IDataBus DataBus;

		protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> ElectricMotorsOff;

		protected bool ElectricMotorCanPropellDuringTractionInterruption;

		//private Second lastShiftTime;

		protected TestPowertrain<T> TestPowertrain;

		protected readonly VelocityRollingLookup VelocityDropData = new VelocityRollingLookup();


		protected StrategyState CurrentState = new StrategyState();
		protected StrategyState PreviousState = new StrategyState();
		protected double IceRampUpCosts;
		protected double IceIdlingCosts;

		protected HybridStrategyParameters StrategyParameters;

		protected DebugData DebugData = new DebugData();
		protected WattSecond BatteryDischargeEnergyThreshold;

		protected DryRunSolutionState DryRunSolution { get; set; }

		protected readonly GearList GearList;
		protected bool LimitedGbxTorque;

		public AbstractHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer)
		{
			DataBus = vehicleContainer;
			ModelData = runData;
			if (ModelData.ElectricMachinesData.Select(x => x.Item1).Distinct().Count() > 1) {
				throw new VectoException("More than one electric motors are currently not supported");
			}
			StrategyParameters = ModelData.HybridStrategyParameters;
			if (StrategyParameters == null) {
				throw new VectoException("Model parameters for hybrid strategy required!");
			}

			GearList = runData.GearboxData.GearList;

			ElectricMotorsOff = ModelData.ElectricMachinesData
										.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
										.ToDictionary(x => x.Key, x => new Tuple<PerSecond, NewtonMeter>(null,  x.Value));
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			ElectricMotorCanPropellDuringTractionInterruption =
				emPos == PowertrainPosition.HybridP4 || emPos == PowertrainPosition.HybridP3;

			var engineRampUpEnergy = Formulas.InertiaPower(ModelData.EngineData.IdleSpeed, 0.RPMtoRad(), ModelData.EngineData.Inertia, ModelData.EngineData.EngineStartTime) * ModelData.EngineData.EngineStartTime;
			var engineDragEnergy = VectoMath.Abs(ModelData.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(ModelData.EngineData.IdleSpeed)) *
									ModelData.EngineData.IdleSpeed / 2.0 * ModelData.EngineData.EngineStartTime;

			IceRampUpCosts = (engineRampUpEnergy + engineDragEnergy).Value() / DeclarationData.AlternaterEfficiency / DeclarationData.AlternaterEfficiency;

			IceIdlingCosts = ModelData.EngineData.Fuels.Sum(
				x => (x.ConsumptionMap.GetFuelConsumptionValue(0.SI<NewtonMeter>(), ModelData.EngineData.IdleSpeed)
					* x.FuelData.LowerHeatingValueVecto * StrategyParameters.MinICEOnTime).Value());

			// create testcontainer
			var modData = new ModalDataContainer(runData, null, null);
			var builder = new PowertrainBuilder(modData);
			var testContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimpleHybridPowertrain(runData, testContainer);

			TestPowertrain = new TestPowertrain<T>(testContainer, DataBus);
			
			

			var shiftStrategyParameters = runData.GearshiftParameters;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}
			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}

			var auxEnergyReserve = ModelData.ElectricAuxDemand * StrategyParameters.AuxReserveTime;
			BatteryDischargeEnergyThreshold = 0.SI<WattSecond>();
			if (auxEnergyReserve > 0) {
				var minSoc = Math.Max(ModelData.BatteryData?.MinSOC ?? ModelData.SuperCapData.MinVoltage / ModelData.SuperCapData.MaxVoltage,
					StrategyParameters.MinSoC);
				BatteryDischargeEnergyThreshold =
					ModelData.BatteryData.Capacity * minSoc * ModelData.BatteryData.SOCMap.Lookup(minSoc) +
					auxEnergyReserve;
			}
			AllowEmergencyShift = false;
		}

		public virtual IHybridController Controller { protected get; set; }

		public PerSecond MinICESpeed
		{
			get
			{
				return ModelData.EngineData.IdleSpeed;
			}
		}

		public bool AllowEmergencyShift { protected get; set; }


		public virtual IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			//if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
			//    (outTorque * outAngularVelocity).IsGreater(StrategyParameters.MaxDrivetrainPower,
			//        Constants.SimulationSettings.LineSearchTolerance)) {
			//    return HandleRequestExceedsMaxPower(absTime, dt, outTorque, outAngularVelocity, dryRun);
			//}

			IResponse testRequest = null;
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && StrategyParameters.MaxPropulsionTorque != null) {
				var nextGear = !DataBus.GearboxInfo.GearEngaged(absTime)
					? new GearshiftPosition(0)
					: (PreviousState.GearboxEngaged
						? DataBus.GearboxInfo.Gear
						: Controller.ShiftStrategy.NextGear);
				var emOff = new HybridStrategyResponse() {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = ElectricMotorsOff
				};
				testRequest = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOff);
				var tqRequest = testRequest.Gearbox.InputTorque;
				var maxTorque =
					StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(testRequest.Gearbox.InputSpeed);
				if (!dryRun) {
					CurrentState.MaxGbxTq = maxTorque;
				}
				if (((tqRequest - maxTorque) * testRequest.Gearbox.InputSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
					LimitedGbxTorque = true;
					return HandleRequestExceedsMaxPower(absTime, dt, outTorque, outAngularVelocity, dryRun,
						testRequest);
				}
			}



			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;

			if (DryRunSolution != null && DryRunSolution.DrivingAction != DataBus.DriverInfo.DrivingAction) {
				DryRunSolution = null;
				LimitedGbxTorque = false;
			}

			var oldDryRunSolution = DryRunSolution;
			if (!dryRun && DryRunSolution != null && !DryRunSolution.Solution.IgnoreReason.AllOK()) {
				DryRunSolution = null;
				LimitedGbxTorque = false;
			}

			if (dryRun && DryRunSolution != null && DryRunSolution.DrivingAction == DataBus.DriverInfo.DrivingAction) {
				var tmp = CreateResponse(DryRunSolution.Solution, currentGear);
				return tmp;
			}


			var eval = new List<HybridResultEntry>();

			switch (DataBus.DriverInfo.DrivingAction) {
				case DrivingAction.Accelerate:
					HandleAccelerateAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Coast:
					HandleCoastAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Roll:
					HandleRollAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Brake:
					HandleBrakeAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Halt:
					HandleHaltAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				default: throw new ArgumentOutOfRangeException();
			}


			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && (eval.Count  == 0 )) {
				eval.Add(MaxRecuperationSetting(absTime, dt, outTorque, outAngularVelocity, dryRun));
			}

			var best = SelectBestOption(eval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);

			if (best == null && oldDryRunSolution != null) {
				best = oldDryRunSolution.Solution;
			}

			if (oldDryRunSolution != null && best != null && best.IgnoreReason.Evaluated() && !best.IgnoreReason.AllOK()) {
				//throw new NotImplementedException("hmmm");
				Log.Info("found better solution...");
				best = oldDryRunSolution.Solution;
			}

			if (best == null) {
				best = ResponseEmOff;
				best.ICEOff = false;
			}
			
			var retVal = CreateResponse(best, currentGear);
			
			retVal.GearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime);
			if (!DataBus.EngineInfo.EngineOn && !best.ICEOff && retVal.ShiftRequired) {
				CurrentState.ICEStartTStmp = absTime + dt;
			} else {
				CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
			}
			CurrentState.Response = dryRun ? null : retVal;
			if (!dryRun) {
				CurrentState.Solution = best;
				CurrentState.AngularVelocity = outAngularVelocity;
				CurrentState.Evaluations = eval;
				CurrentState.GearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime) || DataBus.GearboxInfo.GearboxType.AutomaticTransmission();
				if (!DataBus.EngineCtl.CombustionEngineOn && !best.ICEOff && !retVal.ShiftRequired) {
					CurrentState.ICEStartTStmp = absTime;
				} else {
					CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
				}
			}

			DryRunSolution = new DryRunSolutionState(DataBus.DriverInfo.DrivingAction, best, eval);

			if (retVal.ShiftRequired) {
				DryRunSolution = null;
				CurrentState.GearshiftTriggerTstmp = absTime;
			}

			DebugData.Add(new { DrivingAction = DataBus.DriverInfo.DrivingAction, Evaluations = eval, Best = best, RetVal = retVal, DryRun = dryRun });
			return retVal;
		}

		public IResponse AmendResponse(IResponse response, Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			if (response is ResponseSuccess) {
				if (!DataBus.EngineInfo.EngineOn && !CurrentState.Solution.ICEOff) {
					//CurrentState.ICEStartTStmp = absTime;
				}
			}
			//if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
			//	(outTorque * outAngularVelocity).IsEqual(StrategyParameters.MaxDrivetrainPower,
			//		Constants.SimulationSettings.LineSearchTolerance.SI<Watt>())) {
			//	if (dryRun && response is ResponseDryRun responseDryRun) {
			//		if (responseDryRun.DeltaFullLoad.IsSmaller(0)) {
			//			return new ResponseDryRun(this, responseDryRun) { DeltaFullLoad = 0.SI<Watt>() };
			//		}
			//	}
			//}

			if (dryRun && DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && StrategyParameters.MaxPropulsionTorque != null) {
				var dryRunResponse = response as ResponseDryRun;
				if (response.Engine.EngineOn && dryRunResponse.DeltaFullLoad.IsSmallerOrEqual(0) &&
					dryRunResponse.DeltaDragLoad.IsGreaterOrEqual(0) && LimitedGbxTorque) {
					// during this request the torque at gbx-in was limited - engine is ok and a seaerch operation is going on
					// overwrite delta value...
					var maxTorque =
						StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(response.Gearbox.InputSpeed);
					dryRunResponse.DeltaFullLoad = (dryRunResponse.Gearbox.InputTorque - maxTorque) *
													dryRunResponse.Gearbox.InputSpeed;
					dryRunResponse.DeltaFullLoadTorque = (dryRunResponse.Gearbox.InputTorque - maxTorque);
					dryRunResponse.DeltaDragLoad = (dryRunResponse.Gearbox.InputTorque - maxTorque) *
													dryRunResponse.Gearbox.InputSpeed;
					dryRunResponse.DeltaDragLoadTorque = (dryRunResponse.Gearbox.InputTorque - maxTorque);

				}
			}

			return response;
		}

		public void OperatingpointChangedDuringRequest(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity,
			bool dryRun, IResponse retVal)
		{
			DryRunSolution = null;
			LimitedGbxTorque = false;
		}

		public void RepeatDrivingAction(Second absTime)
		{
			DryRunSolution = null;
			LimitedGbxTorque = false;
		}

		protected abstract IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg);

		private IHybridStrategyResponse HandleRequestExceedsMaxPower(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun, IResponse emOffResponse)
		{
			// issue dry-run to get max available power from EM and ICE,
			// Search PWheel with max available EM power with ICE operating point on MaxTorque
			// return overload with Delta as P_out - PWheelMax

			//var responses = new List<HybridResultEntry>();

			var gearRange = GetGearRange(absTime, dryRun);

			var gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear; // DataBus.GearboxInfo.Gear;

			var firstGear = GearList.Predecessor(gear, Math.Min(1, gearRange.Item1));
			var lastGear = gear; // GearList.Successor(gear, (uint)gearRange.Item2);

			var candidates = new Dictionary<GearshiftPosition, Tuple<Watt, IResponse>>();
			var maxTorqueGbxIn =
				StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(emOffResponse.Gearbox.InputSpeed);
			candidates[emOffResponse.Gearbox.Gear] = Tuple.Create(maxTorqueGbxIn * emOffResponse.Gearbox.InputSpeed, emOffResponse);
			foreach (var nextGear in GearList.IterateGears(firstGear, lastGear)) {
				if (candidates.ContainsKey(nextGear)) {
					continue;
				}
				var emOff = new HybridStrategyResponse() {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = ElectricMotorsOff
				};
				var testRequest = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOff);
				if (testRequest != null) {
					var maxGbxTorque = StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(testRequest.Gearbox.InputSpeed);
					candidates[nextGear] = Tuple.Create(maxGbxTorque * testRequest.Gearbox.InputSpeed, testRequest);
				}
			}

			var maxPwr = candidates.MaxBy(x => x.Value.Item1);
			if (!emOffResponse.Gearbox.Gear.Equals(maxPwr.Key)) {
				return new HybridStrategyResponse() {
					ShiftRequired = true,
					NextGear = maxPwr.Key,
					//CombustionEngineOn = 
					EvaluatedSolution = new HybridResultEntry() {
						Gear = maxPwr.Key,
						Response = maxPwr.Value.Item2
					},
					MechanicalAssistPower = maxPwr.Value.Item2.HybridController.StrategySettings.MechanicalAssistPower
				};
			}

			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;

			var maxEmDriveSetting = new HybridStrategyResponse() {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{emPos , Tuple.Create(emOffResponse.ElectricMotor.AngularVelocity, emOffResponse.ElectricMotor.MaxDriveTorque)}
				},
			};
			var maxEmDriveResponse =
				RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, maxEmDriveSetting);
			var deltaFullLoadTq = (maxEmDriveResponse.Engine.TotalTorqueDemand -
								maxEmDriveResponse.Engine.DynamicFullLoadTorque);
			var maxEngineSpeed =
				maxEmDriveResponse.Gearbox.Gear.Gear == 0 || !DataBus.ClutchInfo.ClutchClosed(absTime) ||
				!DataBus.GearboxInfo.TCLocked
					? ModelData.EngineData.FullLoadCurves[0].N95hSpeed :
					VectoMath.Min(DataBus.GearboxInfo.GetGearData(DataBus.GearboxInfo.Gear.Gear).MaxSpeed, ModelData.EngineData.FullLoadCurves[0].N95hSpeed);

			if (deltaFullLoadTq.IsSmallerOrEqual(0)) {
				// the engine is not overloaded if EM boosts, limit to max gearbox torque
				return new HybridStrategyLimitedResponse() {
					//Delta = outTorque * outAngularVelocity - StrategyParameters.MaxDrivetrainPower,
					Delta = (emOffResponse.Gearbox.InputTorque - maxTorqueGbxIn) * emOffResponse.Gearbox.InputSpeed,
					DeltaEngineSpeed = maxEmDriveResponse.Engine.EngineSpeed - maxEngineSpeed, // .DeltaEngineSpeed
				};
			}

			var deltaMaxTorque = emOffResponse.Gearbox.InputTorque - maxTorqueGbxIn;
			if (deltaMaxTorque.IsGreater(deltaFullLoadTq)) {
				// gearbox in torque is much higher above limit than ice operating point above full-load curve (with em boosting)
				// search to max torque curve
				// i.e. when limiting to max torque the ICE operating point is below full-load curve
				return new HybridStrategyLimitedResponse() {
					Delta = (emOffResponse.Gearbox.InputTorque - maxTorqueGbxIn) * emOffResponse.Gearbox.InputSpeed,
					DeltaEngineSpeed = maxEmDriveResponse.Engine.EngineSpeed - maxEngineSpeed, // .DeltaEngineSpeed
				};
			}

			// ICE operating point (with EM boosting) is higher above full-load curve than gearbox in-torque above max torque limit
			// (i.e. going to max torque point would still overload the ICE with max EM boosting)

			var avgEngineSpeed = (maxEmDriveResponse.Engine.EngineSpeed + DataBus.EngineInfo.EngineSpeed) / 2;
			var maxTorque = SearchAlgorithm.Search(outTorque, deltaFullLoadTq, -outTorque * 0.1,
				getYValue: resp => {
					var r = resp as IResponse;
					var deltaMaxTq = (r.Engine.TotalTorqueDemand -
											r.Engine.DynamicFullLoadTorque);
					return deltaMaxTq * avgEngineSpeed;
				},
				evaluateFunction: x => {
					return RequestDryRun(absTime, dt, x, outAngularVelocity, currentGear, maxEmDriveSetting);
				},
				criterion: resp => {
					var r = resp as IResponse;
					var deltaMaxTq = (r.Engine.TotalTorqueDemand -
									r.Engine.DynamicFullLoadTorque);
					return (deltaMaxTq * avgEngineSpeed).Value();
				});
			var rqMaxTorque = RequestDryRun(absTime, dt, maxTorque, outAngularVelocity, currentGear, maxEmDriveSetting);
			// limiting to ICE FLD with max propulsion - delta gearbox torque
			var delta1 = ( rqMaxTorque.Gearbox.InputTorque - maxTorqueGbxIn) * emOffResponse.Gearbox.InputSpeed;
			var delta2 = (outTorque - maxTorque) * outAngularVelocity;
			var delta = VectoMath.Max(delta1, delta2);
			return new HybridStrategyLimitedResponse() {
				Delta = delta,
				DeltaEngineSpeed = maxEmDriveResponse.Engine.EngineSpeed - maxEngineSpeed
			};
		}

		protected HybridResultEntry ResponseEmOff
		{
			get {
				return new HybridResultEntry {
					U = double.NaN,
					Response = null,
					Setting = new HybridStrategyResponse() {
						GearboxInNeutral = false,
						CombustionEngineOn = DataBus.EngineInfo.EngineOn,
						MechanicalAssistPower = ElectricMotorsOff
					},
					FuelCosts = double.NaN,
					ICEOff = !DataBus.EngineInfo.EngineOn,
					Gear = new GearshiftPosition(0),
				};
			}
		}

		protected virtual bool AllowICEOff(Second absTime)
		{
			if (!ModelData.VehicleData.ADAS.EngineStopStart) {
				return false;
			}
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			if (ModelData.VehicleData.ADAS.EngineStopStart && emPos == PowertrainPosition.HybridP1) {
				return false;
			}
			return PreviousState.ICEStartTStmp == null ||
					(PreviousState.ICEStartTStmp + StrategyParameters.MinICEOnTime).IsSmaller(absTime);
		}

		protected virtual void HandleBrakeAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() &&
				DataBus.GearboxInfo.Gear.Equals(GearList.First())) {
				eval.Add(ResponseEmOff);
				return;
			}
			if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {

				var emPos = ModelData.ElectricMachinesData.First().Item1;
				var disengageSpeedThreshold = DataBus.GearboxInfo.GearboxType.AutomaticTransmission()
					? Constants.SimulationSettings.ATGearboxDisengageWhenHaltingSpeed
					: Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed;
				var vehiclespeedBelowThreshold = DataBus.VehicleInfo.VehicleSpeed.IsSmaller(disengageSpeedThreshold);
				if ((vehiclespeedBelowThreshold) && emPos == PowertrainPosition.HybridP2) {
					eval.Add(ResponseEmOff);
					return;
				}
				
				var nextGear = !DataBus.GearboxInfo.GearEngaged(absTime)
					? new GearshiftPosition(0)
					: (PreviousState.GearboxEngaged
						? DataBus.GearboxInfo.Gear
						: Controller.ShiftStrategy.NextGear);
				var disengaged = nextGear.Gear == 0;
				var currentGear = nextGear;
				var tmp = new HybridStrategyResponse() {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = ElectricMotorsOff
				};
				var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear,  tmp);

				var engineSpeedTooLow = !DataBus.GearboxInfo.GearboxType.AutomaticTransmission()
					? firstResponse.Clutch.OutputSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)
					: firstResponse.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed);

				if (GearList.HasPredecessor(nextGear) && engineSpeedTooLow && !vehiclespeedBelowThreshold) {
					// engine speed would fall below idling speed - consider downshift
					var estimatedVelocityPostShift = VelocityDropData.Valid
						? VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed,
							DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>())
						: DataBus.VehicleInfo.VehicleSpeed;
					var postShiftBelowThreshold = estimatedVelocityPostShift.IsSmaller(disengageSpeedThreshold);
					if (postShiftBelowThreshold) {
						var downshift = ResponseEmOff;
						downshift.Gear = GearList.Predecessor(nextGear);
						eval.Add(downshift);
						return;
					}
					do {
						nextGear = GearList.Predecessor(nextGear);
						firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, tmp);
					} while (GearList.HasPredecessor(nextGear) && firstResponse == null);
				}

				if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() && firstResponse == null && nextGear.Equals(GearList.First())) {
					var downshift = ResponseEmOff;
					downshift.Gear = nextGear;
					eval.Add(downshift);
					return;
				}

				if (tmp.CombustionEngineOn) {
					var firstEntry = new HybridResultEntry();
					CalcualteCosts(firstResponse, dt, firstEntry, AllowICEOff(absTime), dryRun);
					var minimumShiftTimePassed = (DataBus.GearboxInfo.LastShift + ModelData.GearshiftParameters.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
					if (DataBus.GearboxInfo.GearEngaged(absTime) && !vehiclespeedBelowThreshold) {
						if (firstEntry.IgnoreReason.EngineSpeedBelowDownshift() ||
							firstEntry.IgnoreReason.EngineSpeedTooLow()) {
							// downshift required!
							var downshift = ResponseEmOff;
							downshift.Gear = GearList.Predecessor(nextGear);
							eval.Add(downshift);
							return;
						}
					}
				}

				var deltaDragTqFirst = disengaged ? 
					(firstResponse as ResponseDryRun).DeltaDragLoadTorque
					: firstResponse.Engine.TotalTorqueDemand - firstResponse.Engine.DragTorque;

				if (deltaDragTqFirst.IsGreater(0)) {
					// braking requested but engine operating point is not below drag curve.
					if (ElectricMotorCanPropellDuringTractionInterruption) {
						if (DataBus.GearboxInfo.GearEngaged(absTime)) {
							eval.AddRange(FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun));
						} else {
							EvaluateConfigsForGear(
								absTime, dt, outTorque, outAngularVelocity, nextGear, AllowICEOff(absTime), eval, emPos, dryRun);
						}
					}else if (DataBus.GearboxInfo.GearEngaged(absTime)) {
						eval.AddRange(FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun));
					} else {
						eval.Add(ResponseEmOff);
					}
					return;
				}

				if (!firstResponse.Gearbox.Gear.Engaged && !ElectricMotorCanPropellDuringTractionInterruption) {
					// we are disengaged and EM cannot recuperate - switch EM off
					eval.Add(ResponseEmOff);
					return;
				}

				if (firstResponse.ElectricMotor.MaxRecuperationTorque == null) {
					eval.Add(ResponseEmOff);
					return;
				}

				var maxRecuperation = new HybridStrategyResponse() {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
						{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, firstResponse.ElectricMotor.MaxRecuperationTorque) }
					}
				};
				var maxRecuperationResponse = RequestDryRun(
					absTime, dt, outTorque, outAngularVelocity, nextGear, maxRecuperation);

				var deltaDragTqMaxRecuperation = disengaged
					? (maxRecuperationResponse as ResponseDryRun).DeltaDragLoadTorque
					: maxRecuperationResponse.Engine.TotalTorqueDemand - maxRecuperationResponse.Engine.DragTorque;

				if (deltaDragTqMaxRecuperation.IsEqual(0)) {
					// with max recuperation we are already at the drag curve (e.g. because search braking power was invoked before
					eval.Add(
						new HybridResultEntry() {
							ICEOff = !DataBus.EngineInfo.EngineOn,
							Gear = nextGear,
							Setting = new HybridStrategyResponse() {
								CombustionEngineOn = DataBus.EngineInfo.EngineOn,
								GearboxInNeutral = false,
								NextGear = nextGear,
								MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
									{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, firstResponse.ElectricMotor.MaxRecuperationTorque) }
								}
							}
						});
					return;
				}

				if (deltaDragTqMaxRecuperation.IsSmaller(0) && 
					maxRecuperationResponse.ElectricSystem.RESSPowerDemand.IsBetween(maxRecuperationResponse.ElectricSystem.MaxPowerDrag, maxRecuperationResponse.ElectricSystem.MaxPowerDrive)) {
					// even with full recuperation (and no braking) the operating point is below the drag curve (and the battery can handle it) - use full recuperation
					eval.Add(
						new HybridResultEntry() {
							ICEOff = !DataBus.EngineInfo.EngineOn,
							Gear = nextGear,
							Setting = new HybridStrategyResponse() {
								CombustionEngineOn = DataBus.EngineInfo.EngineOn,
								GearboxInNeutral = false,
								NextGear = nextGear,
								MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
									{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, firstResponse.ElectricMotor.MaxRecuperationTorque) }
								}
							}
						});
					return;
				}

				// full recuperation is not possible - ICE would need to propel - search max possible EM torque
				var emRecuperationTq = SearchAlgorithm.Search(
					maxRecuperationResponse.ElectricMotor.ElectricMotorPowerMech /
					maxRecuperationResponse.ElectricMotor.AngularVelocity,
					maxRecuperationResponse.Engine.TorqueOutDemand, maxRecuperationResponse.ElectricMotor.MaxRecuperationTorque * 0.1,
					getYValue: r => {
						var response = r as IResponse;
						var deltaDragLoad = disengaged
							? (response as ResponseDryRun).DeltaDragLoadTorque 
							: response.Engine.TotalTorqueDemand - response.Engine.DragTorque;
						return deltaDragLoad;
					},
					evaluateFunction: emTq => {
						var cfg = new HybridStrategyResponse() {
							CombustionEngineOn = DataBus.EngineInfo.EngineOn,
							GearboxInNeutral = false,
							NextGear = nextGear,
							MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
								{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
							}
						};
						return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, DataBus.GearboxInfo.GearEngaged(absTime) ? nextGear : new GearshiftPosition(0), cfg);
					},
					criterion: r => {
						var response = r as IResponse;
						var deltaDragLoad = disengaged
							? (response as ResponseDryRun).DeltaDragLoadTorque 
							: response.Engine.TotalTorqueDemand - response.Engine.DragTorque;
						return deltaDragLoad.Value();
					}
				);
				if (emRecuperationTq.IsBetween(
					firstResponse.ElectricMotor.MaxDriveTorque, firstResponse.ElectricMotor.MaxRecuperationTorque)) {
					var entry = new HybridResultEntry() {
						ICEOff = !DataBus.EngineInfo.EngineOn,
						Gear = nextGear,
						Setting = new HybridStrategyResponse() {
							CombustionEngineOn = DataBus.EngineInfo.EngineOn,
							GearboxInNeutral = false,
							NextGear = nextGear,
							MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
								{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emRecuperationTq) }
							}
						}
					};
					entry.Response = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, entry.Setting);
					eval.Add(entry);
				} else {
					if (emRecuperationTq.IsGreater(0)) {
						eval.Add(
							new HybridResultEntry() {
								ICEOff = !DataBus.EngineInfo.EngineOn,
								Gear = nextGear,
								Setting = new HybridStrategyResponse() {
									CombustionEngineOn = DataBus.EngineInfo.EngineOn,
									GearboxInNeutral = false,
									NextGear = nextGear,
									MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
										{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, firstResponse.ElectricMotor.MaxRecuperationTorque) }
									}
								}
							});
					} else {
						eval.Add(ResponseEmOff);
					}
				}
			} else {
				eval.Add(ResponseEmOff);
			}
		}

		protected virtual void HandleCoastAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			var nextGear = !DataBus.GearboxInfo.GearEngaged(absTime)
				? new GearshiftPosition(0)
				: (PreviousState.GearboxEngaged
					? DataBus.GearboxInfo.Gear
					: Controller.ShiftStrategy.NextGear);
			var tmp = new HybridStrategyResponse()
			{
				CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
				GearboxInNeutral = false,
				NextGear = nextGear,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var resp = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, tmp);
			if (!dryRun && resp != null) {
				// resp.Engine.EngineSpeed != null && resp.Gearbox.Gear > 1 && ModelData.GearboxData
				//.Gears[resp.Gearbox.Gear].ShiftPolygon
				//.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				var engineSpeed = resp.Gearbox.InputSpeed;
				if (GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData
					.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon
					.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, engineSpeed)) {
					// consider downshift
					var downshift = ResponseEmOff;
					downshift.Gear = GearList.Predecessor(nextGear);
					eval.Add(downshift);
					return;
				}
				if (!resp.Gearbox.Gear.Equals(new GearshiftPosition(0)) && GearList.HasSuccessor(resp.Gearbox.Gear) && ModelData.GearboxData
					.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon
					.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, engineSpeed)) {
					// consider downshift
					var upshift = ResponseEmOff;
					upshift.Gear = GearList.Successor(nextGear);
					eval.Add(upshift);
					return;
				}
			}

			eval.Add(ResponseEmOff);
		}

		protected virtual void HandleRollAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			eval.Add(ResponseEmOff);

			// in case of P3 or P4 the EM could propell/recuperate during roll acttion. but as we have no information
			// what the real vehicle does lets skip this

			//if (ElectricMotorCanPropellDuringTractionInterruption) {
			//	//eval = FindSolutionDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
			//	eval.Add(responseEmOff);
			//} else {
			//	eval.Add(responseEmOff);
			//}
		}

		protected virtual void HandleHaltAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			var tmp = ResponseEmOff;
			tmp.Setting.GearboxInNeutral = false;
			tmp.Setting.CombustionEngineOn = false;
			tmp.ICEOff = true;

			eval.Add(tmp);
		}

		protected virtual void HandleAccelerateAction(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime) ||
				DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
				eval.AddRange(FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun));
			} else {
				eval.Add(ResponseEmOff);
			}
		}

		private HybridResultEntry SelectBestOption(
			List<HybridResultEntry> eval, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, GearshiftPosition currentGear)
		{
			var best = DoSelectBestOption(eval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);
			if (best == null) {
				return null;
			}
			if (!best.IgnoreReason.InvalidEngineSpeed() || best.ICEOff ||
				eval.Select(x => x.Gear).Distinct().Count() <= 1) {
				best.SimulationInterval = dt;
				return best;
			}

			// selected solution has invalid engine speed and engine is on and evaluation contains only one gear - allow emergency shift
			if (best.IgnoreReason.EngineSpeedAboveUpshift()) {
				//try upshift
				var newEval = new List<HybridResultEntry>();
				EvaluateConfigsForGear(
					absTime, dt, outTorque, outAngularVelocity, GearList.Successor(best.Gear), AllowICEOff(absTime), newEval,
					best.Setting.MechanicalAssistPower.First().Key, dryRun);
				if (newEval.Count > 0) {
					best = DoSelectBestOption(newEval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);
				}
			}
			if (best.IgnoreReason.EngineSpeedBelowDownshift()) {
				//try downshift
				var newEval = new List<HybridResultEntry>();
				EvaluateConfigsForGear(
					absTime, dt, outTorque, outAngularVelocity, GearList.Predecessor(best.Gear), AllowICEOff(absTime), newEval,
					best.Setting.MechanicalAssistPower.First().Key, dryRun);
				if (newEval.Count > 0) {
					best = DoSelectBestOption(newEval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);
				}
			}

			best.SimulationInterval = dt;
			return best;
		}

		private HybridResultEntry  DoSelectBestOption(
			List<HybridResultEntry> eval, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, GearshiftPosition currentGear)
		{
			if (eval.Count == 0) {
				return null;
			}
			HybridResultEntry best = null;

			if (DataBus.VehicleInfo.VehicleSpeed.IsSmallerOrEqual(ModelData.GearshiftParameters.StartSpeed)) {
				best = eval.Where(x => !double.IsNaN(x.Score)).Where(x => !x.IgnoreReason.EngineSpeedTooHigh())
							.OrderBy(x => x.Score).FirstOrDefault();
			} else {
				best = eval.Where(x => !double.IsNaN(x.Score)).Where(x => x.IgnoreReason.AllOK()).OrderBy(x => x.Score)
								.FirstOrDefault();
			}
			if (best != null) {
				return best;
			}

			best = eval.Where(x => !double.IsNaN(x.Score) && !x.IgnoreReason.InvalidEngineSpeed()).OrderBy(x => x.Score)
						.FirstOrDefault();
			if (best != null) {
				return best;
			}

			best = eval.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).FirstOrDefault();
			if (best != null) {
				return best;
			}

			var validResponses = eval.Where(x => x.Response != null).ToArray();
			var allOverload = validResponses.Where(x => !(x.IgnoreReason.BatteryDemandExceeded() || x.IgnoreReason.BatterySoCTooLow()))
								.All(x => x.IgnoreReason.EngineTorqueDemandTooHigh());
			var allUnderload = validResponses.All(x => x.IgnoreReason.EngineTorqueDemandTooLow());
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && allOverload) {
				if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
					// overload, EM can support - use solution with max EM power
					var filtered = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded() && !x.IgnoreReason.BatterySoCTooLow())
										.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
					if (filtered.Length > 0) {
						best = filtered.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear)))
							.ThenBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()))
										.FirstOrDefault();
							//.MinBy(
							//..x => x.Setting.MechanicalAssistPower.Sum(e => e.Value ?? 0.SI<NewtonMeter>()));
						return best;
					}
					best = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded())
								.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear)))
								.ThenBy(x => -x.Response.ElectricSystem.RESSPowerDemand.Value()).First();
					return best;
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && allUnderload) {
				if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
					var filtered = eval.Where(x => !x.IgnoreReason.InvalidEngineSpeed())
										.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
					if (!filtered.Any()) {
						filtered = eval.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
					}
					best = filtered.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
					if (best != null) {
						return best;
					}
				}
			}

			var emEngaged = (!ElectricMotorCanPropellDuringTractionInterruption ||
							(DataBus.GearboxInfo.GearEngaged(absTime) && (eval.First().Response?.Gearbox.Gear.Engaged  ?? true)));
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && emEngaged) {
				//var filtered = eval.Where(x => !x.IgnoreReason.InvalidEngineSpeed()).ToArray();
				var filtered = eval
					.Where(x => !x.IgnoreReason.EngineSpeedTooLow() && !x.IgnoreReason.EngineSpeedTooHigh()).ToArray();
				if (filtered.Length == 0) {
					filtered = eval
						.Where(x => !x.IgnoreReason.EngineSpeedTooLow() && !x.IgnoreReason.EngineSpeedTooHigh()).ToArray();
				}
				if (filtered.Length == 0) {
					filtered = eval.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				}
				var filtered2 = filtered.Where(x => !x.IgnoreReason.EngineTorqueDemandTooLow()).ToArray();
				if (filtered2.Length == 0) {
					filtered2 = filtered.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				}

				var filtered3 = filtered2
					.Where(x => !((x.IgnoreReason & HybridConfigurationIgnoreReason.BatteryBelowMinSoC) != 0))
					.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				if (filtered3.Length == 0) {
					filtered3 = filtered2;
				}

				var filteredCurrentGear = filtered3.Where(x => x.Gear.Equals(currentGear)).ToArray();
				if (filteredCurrentGear.Length > 0) {
					best = filteredCurrentGear.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
					return best;
				}
				best = filtered3.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				if (best != null) {
					return best;
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && emEngaged) {
				best = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				if (best != null) {
					return best;
				}
			}
			return eval.FirstOrDefault(); 
		}


		private HybridStrategyResponse CreateResponse(HybridResultEntry best, GearshiftPosition currentGear)
		{
			var retVal = new HybridStrategyResponse() {
				CombustionEngineOn = !best.ICEOff,
				GearboxInNeutral = best.Setting.GearboxInNeutral,
				MechanicalAssistPower = best.Setting.MechanicalAssistPower,
				ShiftRequired = best.Gear.Engaged && !best.Gear.Equals(currentGear), //  gs?.Item1 ?? false,
				NextGear = best.Gear, // gs?.Item2 ?? 0,
				EvaluatedSolution = best,
				SimulationInterval = best.SimulationInterval
			};
			//var pos = retVal.MechanicalAssistPower.Keys.First();
			//if (retVal.MechanicalAssistPower[pos].Item1 == null) {
			//	retVal.MechanicalAssistPower[pos] = Tuple.Create(best.Response.ElectricMotor.AngularVelocity, retVal.MechanicalAssistPower[pos].Item2);
			//}
			if (best.IgnoreReason.EngineSpeedTooHigh() && !DataBus.EngineInfo.EngineOn) {
				// ICE is off, selected solution has a too low or too high engine speed - keep ICE off
				retVal.CombustionEngineOn = false;
			}
			if (best.IgnoreReason.EngineSpeedTooLow() && !DataBus.EngineInfo.EngineOn && DataBus.VehicleInfo.VehicleSpeed.IsGreater(ModelData.GearshiftParameters.StartSpeed)) {
				// ICE is off, selected solution has a too low or too high engine speed - keep ICE off
				retVal.CombustionEngineOn = false;
			}
			return retVal;
		}


		private HybridResultEntry MaxRecuperationSetting(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			var first = new HybridStrategyResponse() {
				CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;
			var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, first);

			var emPos = ModelData.ElectricMachinesData.First().Item1;
			
			var emTorque = !ElectricMotorCanPropellDuringTractionInterruption && (!firstResponse.Gearbox.Gear.Engaged || !DataBus.GearboxInfo.GearEngaged(absTime)) ? null : firstResponse.ElectricMotor.MaxRecuperationTorque;
			return TryConfiguration(absTime, dt, outTorque, outAngularVelocity, currentGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorque), double.NaN, AllowICEOff(absTime), dryRun);
		}

		private List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption).IsGreaterOrEqual(absTime);
			var allowICEOff = AllowICEOff(absTime) && !duringTractionInterruption;

			var emPos = ModelData.ElectricMachinesData.First().Item1;

			var gearRange = GetGearRange(absTime, dryRun);

			var gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear; // DataBus.GearboxInfo.Gear;

			var firstGear = GearList.Predecessor(gear, (uint)gearRange.Item1);
			var lastGear = GearList.Successor(gear, (uint)gearRange.Item2);

			var responses = new List<HybridResultEntry>();

			var allowEmergencyUpshift = false;
			var allowEmergencyDownshift = false;
			foreach (var nextGear in GearList.IterateGears(firstGear, lastGear)) {

				var emOffEntry = EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);

				if (emOffEntry == null) {
					continue;
				}
				if (nextGear.Equals(gear) && gearRange.Item2 == 0 && (emOffEntry.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooHigh) != 0) {
					allowEmergencyUpshift = true;
				}
				if (nextGear.Equals(gear) && gearRange.Item1 == 0 && (emOffEntry.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooLow) != 0) {
					allowEmergencyDownshift = true;
				}
			}

			var tmpBest = responses.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).FirstOrDefault(); 
			if (allowEmergencyUpshift && tmpBest != null && !tmpBest.ICEOff) {
				var nextGear = GearList.Successor(gear);
				var emOffEntry = EvaluateConfigsForGear(
					absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);
			}
			if (allowEmergencyDownshift && tmpBest != null && !tmpBest.ICEOff) {
				var nextGear = GearList.Predecessor(gear);
				var emOffEntry = EvaluateConfigsForGear(
					absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);
			}

			return responses;
		}

		private Tuple<uint, uint> GetGearRange(Second absTime, bool dryRun)
		{
			var minimumShiftTimePassed =
				(DataBus.GearboxInfo.LastShift + ModelData.GearshiftParameters.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			var gearRangeUpshift = ModelData.GearshiftParameters.AllowedGearRangeUp;
			var gearRangeDownshift = ModelData.GearshiftParameters.AllowedGearRangeDown;
			if (!AllowEmergencyShift) {
				if (dryRun || !minimumShiftTimePassed ||
					(absTime - DataBus.GearboxInfo.LastUpshift).IsSmaller(
						ModelData.GearshiftParameters.DownshiftAfterUpshiftDelay /*, 0.1*/)
					|| (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
						DataBus.VehicleInfo.VehicleSpeed.IsSmaller(5.KMPHtoMeterPerSecond()))) {
					gearRangeDownshift = 0;
				}

				if (dryRun || !minimumShiftTimePassed ||
					(absTime - DataBus.GearboxInfo.LastDownshift).IsSmaller(
						ModelData.GearshiftParameters.UpshiftAfterDownshiftDelay /*,0.1*/)
					|| (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
						DataBus.VehicleInfo.VehicleSpeed.IsSmaller(5.KMPHtoMeterPerSecond()))) {
					gearRangeUpshift = 0;
				}
			}

			return Tuple.Create((uint)gearRangeDownshift, (uint)gearRangeUpshift);
		}

		private HybridResultEntry EvaluateConfigsForGear(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, bool allowICEOff,
			List<HybridResultEntry> responses, PowertrainPosition emPos, bool dryRun)
		{
			var emOffEntry = GetEmOffResultEntry(absTime, dt, outTorque, outAngularVelocity, nextGear);
			if (emOffEntry == null) {
				return null;
			}

			if (StrategyParameters.MaxPropulsionTorque != null) {
				var maxTqNextGear =
					StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(emOffEntry.Response.Gearbox.InputSpeed);
				if ((( emOffEntry.Response.Gearbox.InputTorque - maxTqNextGear) * emOffEntry.Response.Gearbox.InputSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
					return null;
				}
			}

			CalcualteCosts(emOffEntry.Response, dt, emOffEntry, allowICEOff, dryRun);

			responses.Add(emOffEntry);

			var emTqReq = emOffEntry.Response.ElectricMotor.TorqueRequest +
						emOffEntry.Response.ElectricMotor.InertiaTorque;

			IterateEMTorque(
				absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, emOffEntry.Response, emTqReq, emPos, responses, dryRun);
			return emOffEntry;
		}

		private HybridResultEntry GetEmOffResultEntry(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGear)
		{
			var emOffSetting = new HybridStrategyResponse() {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var emOffResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOffSetting);

			if (emOffResponse == null) {
				return null;
			}



			var entry = new HybridResultEntry() {
				U = double.NaN,
				Response = emOffResponse,
				Setting = emOffSetting,
				Gear = nextGear
			};
			return entry;
		}

		private void IterateEMTorque(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, bool allowIceOff, IResponse firstResponse, NewtonMeter emTqReq, PowertrainPosition emPos,
			List<HybridResultEntry> responses, bool dryRun)
		{
			const double stepSize = 0.1;

			// iterate over 'EM provides torque'. allow EM to provide more torque in order to overcome ICE inertia
			var maxEmTorque = firstResponse.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>();
			var maxU = allowIceOff
				? -1.0
				: Math.Min((maxEmTorque) / emTqReq, -1.0);
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
				maxU = 0;
			}
			if (firstResponse.ElectricMotor.MaxDriveTorque != null && (ElectricMotorCanPropellDuringTractionInterruption || firstResponse.Gearbox.Gear.Engaged)) {
				for (var u = -stepSize; u >= maxU; u -= stepSize * (u < -10 ? 100 :(u < -4 ? 10 : (u < -2 ? 5 : 1)))) {
					var emTorque = emTqReq.Abs() * u;
					if (!emTorque.IsBetween(
						0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorque), u, allowIceOff, dryRun);
					responses.Add(tmp);
				}

				// make sure the max drive point is also covered.
				var batEnergyAvailable = (DataBus.BatteryInfo.StoredEnergy - BatteryDischargeEnergyThreshold) / dt;
				var emDrivePower = -(batEnergyAvailable - ModelData.ElectricAuxDemand);
				var emTorqueM = emTqReq * maxU;
				if (!responses.Any(x => x.Gear == nextGear && x.U.IsEqual(maxU)) && emTorqueM.IsBetween(
					0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueM), maxU, allowIceOff, dryRun);
					responses.Add(tmp);
				}
				if (maxEmTorque.IsSmaller(0) && emTqReq.IsGreater(-maxEmTorque)) { 
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, maxEmTorque), maxEmTorque / emTqReq, allowIceOff, dryRun);
					if (!tmp.Response.ElectricSystem.ConsumerPower.IsSmaller(emDrivePower)) {
						responses.Add(tmp);
					}
				}
				// if battery is getting empty try to set EM-torque to discharge battery to lower SoC boundary
				
				if (maxEmTorque.IsSmaller(0) && (-emDrivePower).IsGreaterOrEqual(maxEmTorque * firstResponse.ElectricMotor.AngularVelocity)) {
					// maxEmTorque < 0  ==> EM can still propel
					// (-emDrivePower).IsGreaterOrEqual(maxEmTorque * firstResponse.ElectricMotor.AngularVelocity) ==> power available from battery for driving does not exceed max EM power (otherwise torque lookup may fail) 
					//var emDriveTorque = ModelData.ElectricMachinesData.Where(x => x.Item1 == emPos).First().Item2.EfficiencyMap
					//							.LookupTorque(emDrivePower, firstResponse.ElectricMotor.AngularVelocity, maxEmTorque);

					var emDriveTorque = DataBus.ElectricMotorInfo(emPos).GetTorqueForElectricPower(emDrivePower, firstResponse.ElectricMotor.AngularVelocity, dt);
					var emDragTorque = ModelData.ElectricMachinesData.Where(x => x.Item1 == emPos).First().Item2
												.DragCurve.Lookup(firstResponse.ElectricMotor.AngularVelocity);
					if (emDriveTorque != null &&
						emDriveTorque.IsBetween(
							firstResponse.ElectricMotor.MaxRecuperationTorque, firstResponse.ElectricMotor.MaxDriveTorque) &&
						!emDriveTorque.IsEqual(emDragTorque, 1.SI<NewtonMeter>())) {
						var tmp = TryConfiguration(
							absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emDriveTorque), emDriveTorque / emTqReq,
							allowIceOff, dryRun);
						responses.Add(tmp);
					}
				}

				if (ElectricMotorCanPropellDuringTractionInterruption && (allowIceOff || !DataBus.GearboxInfo.GearEngaged(absTime)) /*&& (DataBus.DriverInfo.DrivingAction != DrivingAction.Brake || !DataBus.EngineInfo.EngineOn)*/) {
					// this means that the EM is between wheels and transmission
					// search EM Torque that results in 0 torque at ICE out
					try {
						var emTorqueICEOff = SearchAlgorithm.Search(
							firstResponse.ElectricMotor.ElectricMotorPowerMech / firstResponse.ElectricMotor.AngularVelocity,
							firstResponse.Engine.TorqueOutDemand, firstResponse.ElectricMotor.MaxDriveTorque * 0.1,
							getYValue: r => {
								var response = r as IResponse;
								return response.Engine.TorqueOutDemand;
							},
							evaluateFunction: emTq => {
								var cfg = new HybridStrategyResponse() {
									CombustionEngineOn = true,
									GearboxInNeutral = false,
									MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
										{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
									}
								};
								return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
							},
							criterion: r => {
								var response = r as IResponse;
								return response.Engine.TorqueOutDemand.Value();
							}
						);
						if (emTorqueICEOff.IsBetween(
							firstResponse.ElectricMotor.MaxDriveTorque, firstResponse.ElectricMotor.MaxRecuperationTorque)) {
							// only consider when within allowed EM torque range
							var tmp = TryConfiguration(
								absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueICEOff), emTorqueICEOff / emTqReq,
								allowIceOff, dryRun);
							responses.Add(tmp);
						}
					} catch (Exception ) {
						Log.Debug("Failed to find EM torque to compensate drag losses of next components.");
					}
				}
			}
			
			// iterate over 'EM recuperates' up to max available recuperation potential
			if (firstResponse.ElectricMotor.MaxRecuperationTorque != null && (ElectricMotorCanPropellDuringTractionInterruption || firstResponse.Gearbox.Gear.Engaged)) {
				for (var u = stepSize; u <= 1.0; u += stepSize) {
					var emTorque = firstResponse.ElectricMotor.MaxRecuperationTorque * u;
					if (!(emTorque).IsBetween(
						firstResponse.ElectricMotor.MaxRecuperationTorque, 0.SI<NewtonMeter>())) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorque), u, allowIceOff, dryRun);
					responses.Add(tmp);
				}
				var maxEmTorqueRecuperate = firstResponse.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>();
				
				if (maxEmTorqueRecuperate.IsGreater(0) && allowIceOff && DataBus.DriverInfo.DrivingAction != DrivingAction.Brake) {
					if (ElectricMotorCanPropellDuringTractionInterruption) {
						// this means that the EM is between wheels and transmission
						// search EM Torque that results in 0 torque at ICE out
						try {
							var emTorqueICEOff = SearchAlgorithm.Search(
								firstResponse.ElectricMotor.ElectricMotorPowerMech / firstResponse.ElectricMotor.AngularVelocity,
								firstResponse.Engine.TorqueOutDemand, firstResponse.ElectricMotor.MaxRecuperationTorque * 0.1,
								getYValue: r => {
									var response = r as IResponse;
									return response.Engine.TorqueOutDemand;
								},
								evaluateFunction: emTq => {
									var cfg = new HybridStrategyResponse() {
										CombustionEngineOn = true,
										GearboxInNeutral = false,
										MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
											{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
										}
									};
									return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
								},
								criterion: r => {
									var response = r as IResponse;
									return response.Engine.TorqueOutDemand.Value();
								}
							);
							if (emTorqueICEOff.IsBetween(maxEmTorqueRecuperate, 0.SI<NewtonMeter>())) {
								// only consider where EM is recuperating
								var tmp = TryConfiguration(
									absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueICEOff),
									emTorqueICEOff / maxEmTorqueRecuperate,
									allowIceOff, dryRun);
								responses.Add(tmp);
							}
						} catch (Exception) {
							Log.Debug("Failed to find EM torque to compensate drag losses of next components.");
						}
					} else {
						if (maxEmTorqueRecuperate.IsGreater(0) && (-emTqReq).IsBetween(maxEmTorqueRecuperate, 0.SI<NewtonMeter>())) {
							var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos,
								Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, -emTqReq), -emTqReq / maxEmTorqueRecuperate, allowIceOff, dryRun);
							responses.Add(tmp);
						}
					}
				}
			}
		}

		private HybridResultEntry TryConfiguration(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, PowertrainPosition emPos, Tuple<PerSecond, NewtonMeter> emTorque, double u,
			bool allowIceOff, bool dryRun)
		{
			var cfg = new HybridStrategyResponse() {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{ emPos,  emTorque }
				}
			};
			var resp = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
			
			var tmp = new HybridResultEntry {
				U = u,
				Setting = cfg,
				Response = resp,
				Gear = nextGear,
			};
			
			CalcualteCosts(resp, dt, tmp, allowIceOff, dryRun);
			return tmp;
		}

		private void CalcualteCosts(IResponse resp, Second dt, HybridResultEntry tmp, bool allowIceOff, bool dryRun)
		{
			tmp.IgnoreReason = 0;
			if (resp == null) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NoResponseAvailable;
				return;
			}

			var iceOff = allowIceOff && resp.Engine.TorqueOutDemand.IsEqual(0, 1e-3);
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && dryRun) {
				// this means we search for an acceleration the vehicle is capable to do - so treat ICE overload regularly
				iceOff = false;
			}

			if (!iceOff && !resp.Engine.TotalTorqueDemand.IsBetween(
				resp.Engine.DragTorque, resp.Engine.DynamicFullLoadTorque)) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= resp.Engine.TotalTorqueDemand.IsGreater(resp.Engine.DynamicFullLoadTorque)
					? HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
					: HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
			}

			if (resp.Gearbox.Gear.Engaged && resp.Engine.EngineSpeed.IsGreaterOrEqual(
					VectoMath.Min(
						ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear].MaxSpeed,
						DataBus.EngineInfo.EngineN95hSpeed)) ) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
			}
			if (resp.Engine.EngineSpeed.IsSmallerOrEqual(ModelData.EngineData.IdleSpeed)) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooLow;
			}

			if (resp.Engine.EngineSpeed != null && resp.Gearbox.Gear.Engaged && GearList.HasSuccessor(resp.Gearbox.Gear) && ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				tmp.FuelCosts = double.NaN; // Tuple.Create(true, response.Gearbox.Gear + 1);
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
			}
			if (resp.Engine.EngineSpeed != null && GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				tmp.FuelCosts = double.NaN; // = Tuple.Create(true, response.Gearbox.Gear - 1);
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift;
			}

			SetBatteryCosts(resp, dt, tmp);
			var absTime = DataBus.AbsTime; // todo!
			if (DataBus.GearboxInfo.GearEngaged(absTime)) {

				if (iceOff) {
					// no torque from ICE requested, ICE could be turned off
					tmp.FuelCosts = 0;
					tmp.ICEOff = true;
				} else {
					if (!double.IsNaN(tmp.FuelCosts)) {
						//if (!allowIceOff || !resp.Engine.TorqueOutDemand.IsEqual(0)) {
						tmp.FuelCosts = ModelData.EngineData.Fuels.Sum(
							x => (x.ConsumptionMap.GetFuelConsumptionValue(resp.Engine.TotalTorqueDemand, resp.Engine.EngineSpeed)
								* x.FuelData.LowerHeatingValueVecto * dt).Value());

						//}
					}
				}
			} else {
				if (!resp.Engine.TorqueOutDemand.IsEqual(0, 1e-3)) {
					tmp.FuelCosts = double.NaN;
					tmp.IgnoreReason |= resp.Engine.TorqueOutDemand.IsGreater(0)
						? HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
						: HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
				}
				if (iceOff) {
					// no torque from ICE requested, ICE could be turned off
					tmp.FuelCosts = 0;
					tmp.ICEOff = true;
				} else {
					if (!double.IsNaN(tmp.FuelCosts)) {
						tmp.FuelCosts = ModelData.EngineData.Fuels.Sum(
							x => (x.ConsumptionMap.GetFuelConsumptionValue(0.SI<NewtonMeter>(), resp.Engine.EngineSpeed)
								* x.FuelData.LowerHeatingValueVecto * dt).Value());
					}
				}
			}

			var maxSoC = Math.Min(DataBus.BatteryInfo.MaxSoC, StrategyParameters.MaxSoC);
			var minSoC = Math.Max(DataBus.BatteryInfo.MinSoC , StrategyParameters.MinSoC);
			tmp.SoCPenalty = 1 - Math.Pow((DataBus.BatteryInfo.StateOfCharge - StrategyParameters.TargetSoC) / (0.5 * (maxSoC - minSoC)), 5);

			var socthreshold = StrategyParameters.MinSoC + (StrategyParameters.MaxSoC - StrategyParameters.MinSoC) * 0.1;
			var minSoCPenalty = 10.0;
			if (DataBus.BatteryInfo.StateOfCharge.IsSmaller(socthreshold)) {
				var k = minSoCPenalty / (minSoC - socthreshold);
				var d = minSoCPenalty - k * minSoC;
				var extraSoCPenalty = k * DataBus.BatteryInfo.StateOfCharge + d;
				tmp.SoCPenalty += extraSoCPenalty;
			}

			tmp.EquivalenceFactor = resp.ElectricSystem.RESSPowerDemand.IsSmaller(0)
				? StrategyParameters.EquivalenceFactorDischarge
				: StrategyParameters.EquivalenceFactorCharge;
			tmp.GearshiftPenalty = resp.Gearbox.Gear.Engaged && !resp.Gearbox.Gear.Equals(DataBus.GearboxInfo.Gear)
				? ModelData.GearshiftParameters.RatingFactorCurrentGear
				: 1;

			if (!DataBus.EngineCtl.CombustionEngineOn && !tmp.ICEOff && DataBus.BatteryInfo.StateOfCharge.IsGreater(socthreshold)) {
				tmp.ICEStartPenalty1 = IceRampUpCosts / 10;
				tmp.ICEStartPenalty2 = IceIdlingCosts * 0;
			} else {
				tmp.ICEStartPenalty1 = 0;
				tmp.ICEStartPenalty2 = 0;
			}
			if (!double.IsNaN(tmp.FuelCosts) && tmp.IgnoreReason == 0) {
				tmp.IgnoreReason = HybridConfigurationIgnoreReason.Evaluated;
			}
		}

		private void SetBatteryCosts(IResponse resp, Second dt, HybridResultEntry tmp)
		{
			var batEnergyStored = DataBus.BatteryInfo.StoredEnergy;
			var batEnergy = resp.ElectricSystem.RESSPowerDemand * dt;
			var batPower = resp.ElectricSystem.RESSResponse.PowerDemand;

			if (batPower.IsSmaller(resp.ElectricSystem.RESSResponse.MaxDischargePower) || batPower.IsGreater(resp.ElectricSystem.RESSResponse.MaxChargePower)) {
				// battery power demand too high - would discharge below min SoC / charge above max SoC
				tmp.BatCosts = double.NaN;
				tmp.IgnoreReason |= batPower.IsSmaller(
						resp.ElectricSystem.RESSResponse.MaxDischargePower)
						? HybridConfigurationIgnoreReason.BatteryBelowMinSoC
						: HybridConfigurationIgnoreReason.BatteryAboveMaxSoc;
			}
			if ((batEnergyStored + batEnergy).IsSmaller(BatteryDischargeEnergyThreshold)) {
				// battery level would go below buffer for auxiliary power - do not alow at 
				tmp.BatCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.BatterySoCTooLow;
			}
			if (batEnergyStored.IsSmaller(BatteryDischargeEnergyThreshold)) {
				var missingBatCharge = BatteryDischargeEnergyThreshold - batEnergyStored;
				var minChargePower = missingBatCharge / StrategyParameters.AuxReserveChargeTime;
				if (batPower.IsSmaller(minChargePower)) {
					tmp.BatCosts = double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.BatterySoCTooLow;
				} else {
					tmp.BatCosts = 0;
					tmp.IgnoreReason &= ~HybridConfigurationIgnoreReason.BatterySoCTooLow;
				}
			}
			if (!double.IsNaN(tmp.BatCosts)) { 
				tmp.BatCosts = -(batEnergy).Value();
			}
		}


		public virtual IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var retVal = new HybridStrategyResponse()
				{ MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() };

			foreach (var em in ModelData.ElectricMachinesData) {
				retVal.MechanicalAssistPower[em.Item1] = null;
			}

			PreviousState.AngularVelocity = outAngularVelocity;
			PreviousState.GearboxEngaged = true;
			PreviousState.GearshiftTriggerTstmp = -double.MaxValue.SI<Second>();
			CurrentState.GearshiftTriggerTstmp = -double.MaxValue.SI<Second>();
			return retVal;
		}

		public virtual void CommitSimulationStep(Second time, Second simulationInterval)
		{
			PreviousState = CurrentState;
			CurrentState = new StrategyState();
			CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
			CurrentState.GearshiftTriggerTstmp = PreviousState.GearshiftTriggerTstmp;
			AllowEmergencyShift = false;
			DebugData = new DebugData();
			DryRunSolution = null;
			LimitedGbxTorque = false;
		}

		public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.HybridStrategyScore] = (CurrentState.Solution?.Score ?? 0)/1e3;
			container[ModalResultField.HybridStrategySolution] = CurrentState.Solution?.U ?? -100;

			container[ModalResultField.MaxPropulsionTorqe] = CurrentState.MaxGbxTq ?? 0.SI<NewtonMeter>();

			//if (CurrentState.Evaluations != null) {
			//    container.SetDataValue(
			//        "HybridStrategyEvaluation",
			//        string.Join(
			//            " | ", CurrentState.Evaluations.Select(
			//                x => {
			//                    var foo = string.Join(" ", x.Setting.MechanicalAssistPower.Select(e => $"{e.Key.GetName()} - {e.Value}"));
			//                    var ice = "====";
			//                    if (x.Response != null) {
			//                        ice =
			//                            $"{x.Response.Engine.TorqueOutDemand}, {x.Response.Engine.TotalTorqueDemand}, {x.Response.Engine.DynamicFullLoadTorque}";
			//                    }
			//                    return
			//                        $"{x.U:F2}: {x.Score:F2}; G{x.Gear}; ({x.FuelCosts:F2} + {x.EquivalenceFactor:F2} * ({x.BatCosts:F2} + {x.ICEStartPenalty1:F2}) * {x.SoCPenalty:F2} + {x.ICEStartPenalty2:F2}) / {x.GearshiftPenalty:F2} = {x.Score:F2} ({foo} ICE: {ice}); {x.IgnoreReason.HumanReadable()}";
			//                })
			//            )
			//        );
			//}
		}

		
	}

}