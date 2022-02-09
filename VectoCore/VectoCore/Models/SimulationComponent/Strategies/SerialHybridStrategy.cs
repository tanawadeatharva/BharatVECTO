using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
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
	public class SerialHybridStrategy : AbstractSerialHybridStrategy
	{
		public enum StateMachineState
		{
			Undefined,
			Acc_S0, // GEN = 0
			Acc_S1, // P_GEN = P_opt, SoC <= SoC_min && P_demand < P_opt || SoC >= SoC_min && SoC <= SoC_target && P_demand <= P_opt
			Acc_S2, // P_GEN = P_max, SoC <= S
			Acc_S3, // P_GEN = P_max, P_drive = P_GEN

			Break_S0,
			Break_S1,
			Break_S2,
		}

		protected DryRunSolutionState DryRunSolution { get; set; }


		public SerialHybridStrategy(VectoRunData runData, IVehicleContainer container) : base(runData, container) { }


		public override IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			EmPosition = DataBus.PowertrainInfo.ElectricMotorPositions.FirstOrDefault(x =>
				x != PowertrainPosition.Generator);

			var retVal = new HybridStrategyResponse()
				{ MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() };

			foreach (var em in ModelData.ElectricMachinesData) {
				retVal.MechanicalAssistPower[em.Item1] = null;
			}

			GenSetCharacteristics.ContinuousTorque =
				(DataBus.ElectricMotorInfo(PowertrainPosition.Generator) as ElectricMotor).ContinuousTorque;

			PreviousState.AngularVelocity = outAngularVelocity;
			PreviousState.GearboxEngaged = true;
			PreviousState.GearshiftTriggerTstmp = -double.MaxValue.SI<Second>();
			PreviousState.SMState = DataBus.BatteryInfo.StateOfCharge > StrategyParameters.TargetSoC
				? StateMachineState.Acc_S0
				: DataBus.BatteryInfo.StateOfCharge > StrategyParameters.MinSoC
					? StateMachineState.Acc_S1
					: StateMachineState.Acc_S2;
			CurrentState.SMState = PreviousState.SMState;
			CurrentState.GearshiftTriggerTstmp = -double.MaxValue.SI<Second>();
			return retVal;
		}

		public override IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity,
			bool dryRun)
		{

			if (DryRunSolution != null && DryRunSolution.DrivingAction != DataBus.DriverInfo.DrivingAction) {
				DryRunSolution = null;
			}

			//if (!dryRun && DryRunSolution != null && !DryRunSolution.Solution.IgnoreReason.AllOK()) {
			//	DryRunSolution = null;
			//}

			if (dryRun && DryRunSolution != null && DryRunSolution.DrivingAction == DataBus.DriverInfo.DrivingAction) {
				var tmp = new HybridStrategyResponse() {
					SimulationInterval = dt,
					CombustionEngineOn = DryRunSolution.ICEOn,
					MechanicalAssistPower = DryRunSolution.Settings
				};
				return tmp;
			}


			var maxPowerGenset = GetMaxElectricPowerGenerated(absTime, dt);

			var drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, maxPowerGenset);
			var emResponse = drivetrainDemand.Response.ElectricMotor;

			//if (((-emResponse.TorqueRequest).IsSmaller(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(), 1e-3) ||
			//	(-emResponse.TorqueRequest).IsGreater(emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>(), 1e-3))) {
			//	var delta = emResponse.TorqueRequest -
			//				(-emResponse.TorqueRequest).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
			//					emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
			//	return new HybridStrategyLimitedResponse() {
			//		Delta = delta * emResponse.AvgDrivetrainSpeed
			//	};
			//}

			switch (DataBus.DriverInfo.DrivingAction) {
				case DrivingAction.Halt:
				case DrivingAction.Roll:
				case DrivingAction.Coast:
				case DrivingAction.Accelerate:
					CurrentState.SMState = GetStateAccelerate(drivetrainDemand, maxPowerGenset, dt);
					break;
				case DrivingAction.Brake:
					CurrentState.SMState = GetStateBrake(drivetrainDemand);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			GenSetOperatingPoint genSetOperatingPoint;
			var emTorque = GetMechanicalAllsistPower(absTime, dt, emResponse.TorqueRequest, emResponse, emResponse.AngularVelocity /* potentially not correct! */);
			switch (CurrentState.SMState) {
				case StateMachineState.Acc_S0:
					genSetOperatingPoint = GensetOff;
					// update drivetrain demand if genset uses a different operating point - we are above target SoC, battery might get full
					var tmp = GensetOff;
					tmp.ElectricPower = 0.SI<Watt>();
					drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, tmp);
					emResponse = drivetrainDemand.Response.ElectricMotor;
					//emTorque = (-emResponse.TorqueRequest).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					//	emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					emTorque = GetMechanicalAllsistPower(absTime, dt, emResponse.TorqueRequest, emResponse, emResponse.AngularVelocity /* potentially not correct! */);
					break;
				case StateMachineState.Acc_S1:
					var optimalPoint = DataBus.ElectricMotorInfo(PowertrainPosition.Generator).DeRatingActive ?
						GenSetCharacteristics.OptimalPointDeRated
						: GenSetCharacteristics.OptimalPoint;
					genSetOperatingPoint = ApproachGensetOperatingPoint(absTime, dt, optimalPoint);
					// update drivetrain demand if genset uses a different operating point? - probably not needed as SoC needs to increase anyway
					//drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, genSetOperatingPoint);
					//emResponse = drivetrainDemand.Response.ElectricMotor;
					//emTorque = (-emResponse.TorqueRequest).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					//	emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					break;
				case StateMachineState.Acc_S2:
					genSetOperatingPoint = MaxGensetPower(absTime, dt, drivetrainDemand, maxPowerGenset);
					break;
				case StateMachineState.Acc_S3:
					genSetOperatingPoint = MaxGensetPower(absTime, dt, drivetrainDemand, maxPowerGenset);
					emTorque = TestPowertrain.ElectricMotor.GetTorqueForElectricPower(
						DataBus.BatteryInfo.InternalVoltage, drivetrainDemand.Response.ElectricSystem.MaxPowerDrive,
						drivetrainDemand.Response.ElectricMotor.AngularVelocity, dt);
					if (emTorque == null) {
						emTorque = emResponse.MaxDriveTorque;
					}
					emTorque = GetMechanicalAllsistPower(absTime, dt, emTorque, emResponse, emResponse.AngularVelocity /* potentially not correct! */);
					//emTorque = emTorque.LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
						//emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					break;
				case StateMachineState.Break_S0:
				case StateMachineState.Break_S1:
				case StateMachineState.Break_S2:
					genSetOperatingPoint = DataBus.EngineInfo.EngineOn ? ApproachGensetOperatingPoint(absTime, dt, GensetIdle) : GensetOff;
					
					// update drivetrain demand if genset uses a different operating point - we are above target SoC, battery might get full
					var tmpBr = GensetOff;
					tmpBr.ElectricPower = 0.SI<Watt>();
					drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, tmpBr);
					emResponse = drivetrainDemand.Response.ElectricMotor;

					emTorque = GetMechanicalAllsistPower(absTime, dt, emResponse.TorqueRequest, emResponse, emResponse.AngularVelocity);
					//if (emTorque > 0 && emResponse.MaxRecuperationTorque == null) {
					//	// we could recuperate, but max recuperation is null - so battery is full. turn off EM
					//	emTorque = null;
					//} else {
					//	emTorque = (-emResponse.TorqueRequest).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					//		emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					//}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var setting = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{
						EmPosition,
						Tuple.Create(drivetrainDemand.AvgEmDrivetrainSpeed, emTorque)
					}, {
						PowertrainPosition.Generator,
						Tuple.Create(genSetOperatingPoint.ICESpeed, genSetOperatingPoint.ICETorque)
					},
				}
				;

			DryRunSolution = new DryRunSolutionState(DataBus.DriverInfo.DrivingAction, setting, genSetOperatingPoint.ICEOn);

			return new HybridStrategyResponse() {
				SimulationInterval = dt,
				CombustionEngineOn = genSetOperatingPoint.ICEOn,
				MechanicalAssistPower = setting
			};

		}

		private NewtonMeter GetMechanicalAllsistPower(Second absTime, Second dt, NewtonMeter emOutTorque, ElectricMotorResponse emResponse, PerSecond currOutAngularVelocity)
		{
			if (!DataBus.GearboxInfo.GearEngaged(absTime) && DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
				var em = DataBus.ElectricMotorInfo(EmPosition);
				var avgSpeed = (em.ElectricMotorSpeed + currOutAngularVelocity) / 2;
				var inertiaTorqueLoss = avgSpeed.IsEqual(0)
					? 0.SI<NewtonMeter>()
					: Formulas.InertiaPower(currOutAngularVelocity, em.ElectricMotorSpeed, ModelData.ElectricMachinesData.First(x => x.Item1 == EmPosition).Item2.Inertia, dt) / avgSpeed;
				//var dragTorque = ElectricMotorData.DragCurve.Lookup()
				return (-inertiaTorqueLoss); //.LimitTo(maxDriveTorque, maxRecuperationTorque);
			}

			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Coast ||
				DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
				return null;
			}

			if (DataBus.VehicleInfo.VehicleSpeed.IsSmallerOrEqual(Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed) && emOutTorque.IsSmaller(0)) {
				return null;
			}
			return (-emOutTorque).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
				emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
		}

		protected StateMachineState GetStateAccelerate(DrivetrainDemand drivetrainDemand,
			GenSetOperatingPoint maxPowerGenset, Second dt)
		{
			var reqBatteryPower = maxPowerGenset.ElectricPower + drivetrainDemand.ElectricPowerDemand;
			if (DataBus.BatteryInfo.StateOfCharge.IsEqual(StrategyParameters.MinSoC, 0.01) && reqBatteryPower < 0 && drivetrainDemand.ElectricPowerDemand < drivetrainDemand.Response.ElectricSystem.MaxPowerDrive) {
				return StateMachineState.Acc_S3;
			}

			var optimalGensetPoint = DataBus.ElectricMotorInfo(PowertrainPosition.Generator).DeRatingActive
				? GenSetCharacteristics.OptimalPointDeRated
				: GenSetCharacteristics.OptimalPoint;
			switch (PreviousState.SMState) {
				case StateMachineState.Acc_S0:
					if (DataBus.BatteryInfo.StateOfCharge < StrategyParameters.MinSoC) {
						return -drivetrainDemand.ElectricPowerDemand <
								optimalGensetPoint.ElectricPower
							? StateMachineState.Acc_S1
							: StateMachineState.Acc_S2;
					}

					break;
				case StateMachineState.Acc_S1:
					if (/*DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.MinSoC &&*/
						DataBus.BatteryInfo.StateOfCharge < StrategyParameters.TargetSoC
						&& -drivetrainDemand.ElectricPowerDemand >
						optimalGensetPoint.ElectricPower) {
						return StateMachineState.Acc_S2;
					}

					if (DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.TargetSoC) {
						return StateMachineState.Acc_S0;
					}

					break;
				case StateMachineState.Acc_S2:
					if (DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.TargetSoC) {
						return StateMachineState.Acc_S0;
					}

					if (DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.MinSoC &&
						DataBus.BatteryInfo.StateOfCharge < StrategyParameters.TargetSoC
						&& -drivetrainDemand.ElectricPowerDemand <=
						optimalGensetPoint.ElectricPower) {
						return StateMachineState.Acc_S1;
					}
					break;
				case StateMachineState.Break_S0:
					return StateMachineState.Acc_S0;
				case StateMachineState.Break_S1:
					return StateMachineState.Acc_S1;
				case StateMachineState.Break_S2:
					return StateMachineState.Acc_S2;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return PreviousState.SMState;
		}

		protected StateMachineState GetStateBrake(DrivetrainDemand drivetrainDemand)
		{
			switch (PreviousState.SMState) {
				case StateMachineState.Acc_S0:
					return StateMachineState.Break_S0;
				case StateMachineState.Acc_S1:
					return StateMachineState.Break_S1;
				case StateMachineState.Acc_S2:
					return StateMachineState.Break_S2;
				case StateMachineState.Acc_S3:
					return StateMachineState.Break_S2;
				case StateMachineState.Break_S0:
					break;
				case StateMachineState.Break_S1:
					break;
				case StateMachineState.Break_S2:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return PreviousState.SMState;
		}
	}

	public abstract class AbstractSerialHybridStrategy  : LoggingObject, IHybridControlStrategy
	{
		protected VectoRunData ModelData;
		protected IDataBus DataBus;

		protected HybridStrategyParameters StrategyParameters;

		protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> ElectricMotorsOff;

		protected StrategyState CurrentState = new StrategyState();
		protected StrategyState PreviousState = new StrategyState();

		protected TestPowertrain<Gearbox> TestPowertrain;
		protected TestGenset TestGenSet;
		protected GenSetCharacteristics GenSetCharacteristics = new GenSetCharacteristics();
		
		protected PowertrainPosition EmPosition;


		public AbstractSerialHybridStrategy (VectoRunData runData, IVehicleContainer container)
		{
			DataBus = container;
			ModelData = runData;
			if (ModelData.ElectricMachinesData.Select(x => x.Item1).Where(x => x != PowertrainPosition.Generator).Distinct().Count() > 1) {
				throw new VectoException("More than one electric motors are currently not supported");
			}
			StrategyParameters = ModelData.HybridStrategyParameters;
			if (StrategyParameters == null) {
				throw new VectoException("Model parameters for hybrid strategy required!");
			}

			ElectricMotorsOff = ModelData.ElectricMachinesData
				.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
				.ToDictionary(x => x.Key, x => new Tuple<PerSecond, NewtonMeter>(null, x.Value));

			

			// create testcontainer
			var modData = new ModalDataContainer(runData, null, null);
			var builder = new PowertrainBuilder(modData);
			var testContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimpleSerialHybridPowertrain(runData, testContainer);

            TestPowertrain = new TestPowertrain<Gearbox>(testContainer, DataBus);

            var gensetContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimpleGenSet(runData, gensetContainer);
			TestGenSet = new TestGenset(gensetContainer, DataBus);

			
			container.AddPreprocessor(new GensetPreprocessor(GenSetCharacteristics ,TestGenSet, runData.EngineData,
				runData.ElectricMachinesData.FirstOrDefault(x => x.Item1 == PowertrainPosition.Generator)?.Item2));
		}

		#region Implementation of IHybridControlStrategy

		protected GenSetOperatingPoint MaxGensetPower(Second absTime, Second dt,
			DrivetrainDemand drivetrainDemand,
			GenSetOperatingPoint maxPowerGenset)
		{
			var electricPowerDemand = drivetrainDemand.ElectricPowerDemand;

			var gensetLimit = DataBus.ElectricMotorInfo(PowertrainPosition.Generator).DeRatingActive
				? GenSetCharacteristics.MaxPowerDeRated
				: GenSetCharacteristics.MaxPower;
			if (maxPowerGenset.ElectricPower.IsSmaller(gensetLimit.ElectricPower)) {
				gensetLimit = maxPowerGenset;
			}

			//if (electricPowerDemand.IsGreater(gensetLimit.ElectricPower)) {
				return ApproachGensetOperatingPoint(absTime, dt, gensetLimit);
			//}

			//var pair = GenSetCharacteristics.OptimalPoints.Keys.GetSection(x =>
			//	x.IsSmaller(drivetrainDemand.ElectricPowerDemand));

			//return ApproachGensetOperatingPoint(absTime, dt, GenSetCharacteristics.OptimalPoints[pair.Item2].MinBy(x => x.FuelConsumption));
			//GenSetCharacteristics.
		}

		public GenSetOperatingPoint GensetOff => new
			GenSetOperatingPoint
			{
				ICEOn = false,
				ICESpeed = ModelData.EngineData.IdleSpeed,
				ICETorque = null,
				EMTorque =  null
			};

		public GenSetOperatingPoint GensetIdle => new GenSetOperatingPoint() {
			ICEOn =  true,
			ICESpeed = ModelData.EngineData.IdleSpeed,
			ICETorque =  0.SI<NewtonMeter>(),
			EMTorque = null,
		};

		protected DrivetrainDemand GetDrivetrainPowerDemand(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GenSetOperatingPoint maxPowerGenset)
		{
			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(EmPosition) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(EmPosition) as ElectricMotor).DeRatingActive;

			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			if (TestPowertrain.Battery != null) {
				TestPowertrain.Battery.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as Battery).PreviousState.PulseDuration;
				TestPowertrain.Battery.PreviousState.PowerDemand =
					(DataBus.BatteryInfo as Battery).PreviousState.PowerDemand;
			}
			if (TestPowertrain.BatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestPowertrain.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestPowertrain.BatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
				TestPowertrain.BatterySystem.PreviousState.PowerDemand = (DataBus.BatteryInfo as BatterySystem).PreviousState.PowerDemand;
			}

			TestPowertrain.Charger.ChargingPower = maxPowerGenset.ElectricPower;
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque,
				Controller.PreviousState.OutAngularVelocity);
			var testResponse =
				TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, false);

			TestPowertrain.HybridController.ApplyStrategySettings(new HybridStrategyResponse() {
				CombustionEngineOn = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{
						EmPosition,
						Tuple.Create(testResponse.ElectricMotor.AvgDrivetrainSpeed, -testResponse.ElectricMotor.TorqueRequest)
					}
				}
			});
			var testResponse2 =
				TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity,
					false);
			return new DrivetrainDemand(){
				AvgEmDrivetrainSpeed = testResponse2.ElectricMotor.AvgDrivetrainSpeed, 
				EmTorqueDemand = testResponse2.ElectricMotor.TorqueRequest, 
				ElectricPowerDemand = testResponse2.ElectricSystem.ConsumerPower,
				Response = testResponse2
			};
		}

		public class DrivetrainDemand
		{
			public PerSecond AvgEmDrivetrainSpeed { get; set; }
			public NewtonMeter EmTorqueDemand { get; set; }

			public Watt ElectricPowerDemand { get; set; }
			public IResponse Response { get; set; }
		}

		protected GenSetOperatingPoint GetMaxElectricPowerGenerated(Second absTime, Second dt)
		{
			var genDerated = DataBus.ElectricMotorInfo(PowertrainPosition.Generator).DeRatingActive;
			return ApproachGensetOperatingPoint(absTime, dt,
				genDerated ? GenSetCharacteristics.MaxPowerDeRated : GenSetCharacteristics.MaxPower);
		}

		protected GenSetOperatingPoint ApproachGensetOperatingPoint(Second absTime, Second dt, GenSetOperatingPoint op)
		{
			TestGenSet.CombustionEngine.Initialize(
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque,
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed);
			TestGenSet.CombustionEngine.PreviousState.EngineOn = //true;
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
			TestGenSet.CombustionEngine.PreviousState.EnginePower =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
			TestGenSet.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
			TestGenSet.CombustionEngine.PreviousState.EngineSpeed =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
			TestGenSet.CombustionEngine.PreviousState.EngineTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
			TestGenSet.CombustionEngine.PreviousState.EngineTorqueOut =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
			TestGenSet.CombustionEngine.PreviousState.DynamicFullLoadTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;

			switch (TestGenSet.CombustionEngine.EngineAux) {
				case EngineAuxiliary engineAux:
					engineAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as EngineAuxiliary).PreviousState
						.AngularSpeed;
					break;
				case BusAuxiliariesAdapter busAux:
					busAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as BusAuxiliariesAdapter).PreviousState
						.AngularSpeed;
					if (busAux.ElectricStorage is SimpleBattery bat) {
						bat.SOC = ((DataBus.EngineInfo as CombustionEngine).EngineAux as BusAuxiliariesAdapter)
							.ElectricStorage
							.SOC;
					}

					break;
			}
			TestGenSet.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(PowertrainPosition.Generator) as ElectricMotor).ThermalBuffer;
			TestGenSet.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(PowertrainPosition.Generator) as ElectricMotor).DeRatingActive;

			var iceSpeed = op.ICESpeed;
			var emTqDt = op.ICETorque;
			
			TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
			var r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
			if (r1 is ResponseOverload && !TestGenSet.CombustionEngine.PreviousState.EngineSpeed.IsEqual(op.ICESpeed)) {
				var rampUp = GenSetCharacteristics.OptimalPoints.Values.SelectMany(x => x).Where(x =>
						TestGenSet.ElectricMotor.DeRatingActive
							? x.EMTorque.IsSmallerOrEqual(GenSetCharacteristics.ContinuousTorque)
							: true).Select(x => new {
						OperatingPoint = x,
						RemainingTorque = x.ICETorque - Formulas.InertiaPower(x.ICESpeed,
								TestGenSet.CombustionEngine.PreviousState.EngineSpeed, ModelData.EngineData.Inertia,
								dt) /
							((TestGenSet.CombustionEngine.PreviousState.EngineSpeed + x.ICESpeed) / 2)
					});
					var rampUp2 = rampUp.Where(
						x => x.OperatingPoint.ICESpeed.IsGreater(TestGenSet.CombustionEngine.PreviousState.EngineSpeed))
					.ToArray();
				if (rampUp2.Any()) {
					var best = rampUp2.MaxBy(x => x.RemainingTorque);
					emTqDt = best.OperatingPoint.EMTorque;
					iceSpeed = best.OperatingPoint.ICESpeed;
					TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
					r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(),
						best.OperatingPoint.ICESpeed);
				}
			}

			if (r1 is ResponseOverload ovl) {
				emTqDt = SearchAlgorithm.Search(emTqDt, ovl.Delta, emTqDt * 0.1,
					getYValue: r => {
						var dryRun = r as ResponseDryRun;
						return dryRun.DeltaFullLoad;
					},
					evaluateFunction:
					x => {
						TestGenSet.ElectricMotorCtl.EMTorque = x;
						return TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed, true);
					},
					criterion: r => {
						var dryRun = r as ResponseDryRun;
						return dryRun.DeltaFullLoad.Value();
					},
					searcher: this);
				TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
				r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
			}

			if (r1 is ResponseUnderload udl) {
				iceSpeed = SearchAlgorithm.Search(iceSpeed, udl.Delta, iceSpeed * 0.01,
					getYValue: r => {
						var dryRun = r as ResponseDryRun;
						return dryRun.DeltaDragLoad;
					},
					evaluateFunction: x => {
						return TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), x, true);
					},
					criterion: r => {
						var dryRun = r as ResponseDryRun;
						return dryRun.DeltaDragLoad.Value();
					},
					searcher: this);
				r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
			}

			return new GenSetOperatingPoint() {
				ICEOn = true,
				ElectricPower = r1.ElectricSystem.ConsumerPower,
				ICETorque = emTqDt,
				ICESpeed = iceSpeed,
				EMSpeed = r1.ElectricMotor.AngularVelocity,
				EMTorque = r1.ElectricMotor.TorqueRequestEmMap
			};
		}

		public abstract IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);

		public abstract IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun);

		public IResponse AmendResponse(IResponse response, Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			return response;
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			var iceStart = PreviousState.ICEStartTStmp;
			PreviousState = CurrentState;
			if (!DataBus.EngineCtl.CombustionEngineOn) {
				PreviousState.ICEStartTStmp = iceStart;
			} else {
				// ice is on, set start-timestamp if it was off
				if (!PreviousState.ICEOn) {
					PreviousState.ICEStartTStmp = time;
				}
			}
			CurrentState = new StrategyState();
			CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
			CurrentState.GearshiftTriggerTstmp = PreviousState.GearshiftTriggerTstmp;
			CurrentState.ICEOn = DataBus.EngineCtl.CombustionEngineOn;
			AllowEmergencyShift = false;
			//DebugData = new DebugData();
			
			//throw new NotImplementedException();
		}

		public IHybridController Controller { get; set; }
		public PerSecond MinICESpeed { get; }
		public bool AllowEmergencyShift { get; set; }
		public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			//throw new NotImplementedException();
		}

		public void OperatingpointChangedDuringRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, IResponse retVal)
		{
			throw new NotImplementedException();
		}

		public void RepeatDrivingAction(Second absTime)
		{
			
		}

		#endregion

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
			public bool ICEOn { get; set; }

			public SerialHybridStrategy.StateMachineState SMState { get; set; }
		}

		public class DryRunSolutionState
		{
			public DryRunSolutionState(DrivingAction drivingAction, Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> settings, bool iceOn)
			{
				DrivingAction = drivingAction;
				Settings = settings;
				ICEOn = iceOn;
			}


			public DrivingAction DrivingAction { get; }

			public bool ICEOn { get; }

			public Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> Settings { get; }
		}
	}

	public class GenSetCharacteristics
	{
		public Dictionary<Watt, List<GenSetOperatingPoint>> OptimalPoints = new Dictionary<Watt, List<GenSetOperatingPoint>>();

		public GenSetOperatingPoint MaxPower;

		public GenSetOperatingPoint MaxPowerDeRated;
		private GenSetOperatingPoint _optimalPoint;
		private GenSetOperatingPoint _optimalPointDerated;

		public GenSetOperatingPoint OptimalPoint => _optimalPoint ?? (_optimalPoint =
			OptimalPoints.Values.SelectMany(x => x).MinBy(x => x.FuelConsumption / x.ElectricPower));

		public GenSetOperatingPoint OptimalPointDeRated => _optimalPointDerated ?? (_optimalPointDerated =
			OptimalPoints.Values.SelectMany(x => x).Where(x => x.EMTorque.IsSmaller(ContinuousTorque)).MinBy(x => x.FuelConsumption / x.ElectricPower));

		public NewtonMeter ContinuousTorque { get; set; }
	}

	public class GenSetOperatingPoint
	{
		public bool ICEOn;

		public Watt ElectricPower;

		public PerSecond ICESpeed;

		public NewtonMeter ICETorque;

		public KilogramPerSecond FuelConsumption;

		public PerSecond EMSpeed;

		public NewtonMeter EMTorque;
	}
}