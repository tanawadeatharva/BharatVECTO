using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{

    public class TestGenset : ITestGenset
	{

		public ISimpleVehicleContainer Container;
		public ICombustionEngine CombustionEngine { get; }
		public IElectricMotor ElectricMotor => _em;
		public IGensetMotorController ElectricMotorCtl { get; }

		public Joule EM_ThermalBuffer
		{
			set { _em.ThermalBuffer = value; }
		}

		public bool EM_DeRatingActive
		{
			set { _em.DeRatingActive = value; }
		}

		public PerSecond EM_DrivetrainSpeed
		{
			set { _em.PreviousState.DrivetrainSpeed = value; }
		}

		public PerSecond EM_Speed
		{
			set { _em.PreviousState.EMSpeed = value; }
		}

		public IAuxPort EngineAux { get; }

		public IElectricEnergyStorage Battery { get; }
		public IElectricEnergyStorage BatterySystem { get; }
		public IElectricEnergyStorage SuperCap { get; }
		private readonly ElectricMotor _em;

		public TestGenset(ISimpleVehicleContainer container, IDataBus realContainer)
		{
			Container = container;
			CombustionEngine = Container.EngineInfo as ICombustionEngine;
			EngineAux = (CombustionEngine as StopStartCombustionEngine)?.EngineAux;
            _em = container.ElectricMotors.FirstOrDefault(x => x.Key == PowertrainPosition.GEN).Value as ElectricMotor;
			ElectricMotorCtl = _em.Control as IGensetMotorController;

			Battery = Container.BatteryInfo as Battery;
			BatterySystem = container.BatteryInfo as BatterySystem;

			SuperCap = Container.BatteryInfo as SuperCap;
		}

		public NewtonMeter ConvertEmTorqueToDrivetrain(PerSecond emSpeed, NewtonMeter tq, bool dryRun)
		{
			return _em.ConvertEmTorqueToDrivetrain(emSpeed, tq, dryRun);
		}

		public PerSecond ConvertEmSpeedToDrivetrain(PerSecond emSpeed)
		{
			return _em.ConvertEmSpeedToDrivetrain(emSpeed);
		}
	}


	public class TestPowertrain<T> : ITestPowertrain<T> where T : class, IHybridControlledGearbox, IGearbox
	{
		public ISimpleVehicleContainer Container { get; }
		public IDataBus RealContainer;

		public T Gearbox { get; }
		
		public ISimpleHybridController HybridController { get; }
		public Battery Battery;
		public IElectricEnergyStorage BatterySystem { get; }
		public SuperCap SuperCap;
		public IClutch Clutch { get; }
		public IBrakes Brakes { get; }

		public ICombustionEngine CombustionEngine { get; }
		public IAuxPort EngineAux { get; }
		public IElectricMotor ElectricMotor { get; }
		public SimpleElectricMotorControl ElectricMotorControl { get;  }
		public IElectricChargerPort Charger { get; }
		public Dictionary<PowertrainPosition, ElectricMotor> ElectricMotorsUpstreamTransmission { get; } = new Dictionary<PowertrainPosition, ElectricMotor>();
		public ITorqueConverter TorqueConverter { get; }
		public IDCDCConverter DCDCConverter { get; }
		public WHRCharger WHRCharger;

		public TestPowertrain(ISimpleVehicleContainer container, IDataBus realContainer)
		{
			Container = container;
			RealContainer = realContainer;

			Gearbox = Container.GearboxCtl as T;
			
			HybridController = Container.HybridController as ISimpleHybridController;
			Battery = Container.BatteryInfo as Battery;
			BatterySystem = container.BatteryInfo as BatterySystem;
			
			SuperCap = Container.BatteryInfo as SuperCap;
			Clutch = Container.ClutchInfo as Clutch;
			CombustionEngine = Container.EngineInfo as StopStartCombustionEngine;
			EngineAux = (CombustionEngine as StopStartCombustionEngine)?.EngineAux;
			ElectricMotor = container.ElectricMotors.FirstOrDefault().Value as ElectricMotor;
			ElectricMotorControl = (ElectricMotor as ElectricMotor)?.Control as SimpleElectricMotorControl;
			Charger = (((ElectricMotor as ElectricMotor)?.ElectricPower as ElectricSystem)?.Charger.FirstOrDefault(x => x is GensetChargerAdapter)) as GensetChargerAdapter;
			foreach (var pos in container.ElectricMotorPositions) {
				if (pos == PowertrainPosition.HybridP1 || pos == PowertrainPosition.HybridP2 || pos == PowertrainPosition.IHPC ||
					pos == PowertrainPosition.HybridP2_5 || pos == PowertrainPosition.HybridP3) {
					ElectricMotorsUpstreamTransmission[pos] = container.ElectricMotors[pos] as ElectricMotor;
				}
			}
			
			if (Gearbox != null && Gearbox.GearboxType.AutomaticTransmission() && Gearbox.GearboxType != GearboxType.APTN && Gearbox.GearboxType != GearboxType.IHPC) {
				TorqueConverter = Container.TorqueConverterInfo as TorqueConverter;
				if (TorqueConverter == null) {
					throw new VectoException("Torque converter missing for automatic transmission: {0}", Container.TorqueConverterInfo?.GetType().FullName);
				}
			}

			//if (HybridController == null) {
			//	throw new VectoException("Unknown HybridController in TestContainer: {0}", Container.HybridController?.GetType().FullName);
			//}

			var busAux = container.RunData.BusAuxiliaries;
			if (busAux != null && busAux.ElectricalUserInputsConfig.ConnectESToREESS) {
				DCDCConverter = container.DCDCConverter as DCDCConverter;
			}

			var whrCharger = container.SimulationComponents().FirstOrDefault(x => x is WHRCharger);
			if (whrCharger != null) {
				WHRCharger = whrCharger as WHRCharger;
			}
			var driver = new MockDriver(container, realContainer);
			var cycle = new MockDrivingCycle(container, realContainer);

			Brakes = container.Brakes;
			if (Brakes == null) {
				throw new VectoException("Unknown or missing brakes in TestContainer: {0}", Container.Brakes?.GetType().FullName);
			}
			//Brakes = new MockBrakes(container);
		}

		
		public void UpdateComponents() => Container.UpdateComponents(RealContainer);
	}

	public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo
	{
		private IDataBus realContainer;

		public MockDrivingCycle(ISimpleVehicleContainer container, IDataBus rcontainer) : base(container)
		{
			realContainer = rcontainer;
		}

		#region Implementation of IDrivingCycleInfo

		public CycleData CycleData => realContainer.DrivingCycleInfo.CycleData;

		public bool PTOActive => realContainer.DrivingCycleInfo.PTOActive;

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return realContainer.DrivingCycleInfo.CycleLookAhead(distance);
		}

		public Meter Altitude => realContainer.DrivingCycleInfo.Altitude;

		public Radian RoadGradient => realContainer.DrivingCycleInfo.RoadGradient;

		public MeterPerSecond TargetSpeed => realContainer.DrivingCycleInfo.TargetSpeed;

		public Second StopTime => realContainer.DrivingCycleInfo.StopTime;

		public Meter CycleStartDistance => realContainer?.DrivingCycleInfo?.CycleStartDistance ?? 0.SI<Meter>();

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return realContainer.DrivingCycleInfo.LookAhead(lookaheadDistance);
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return realContainer.DrivingCycleInfo.LookAhead(time);
		}

		public SpeedChangeEntry LastTargetspeedChange => realContainer.DrivingCycleInfo.LastTargetspeedChange;

		public void FinishSimulation()
		{
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		protected override bool DoUpdateFrom(object other) => false;

		#endregion
	}

	public class MockDriver : VectoSimulationComponent, IDriverInfo
	{
		private IDataBus realContainer;

		public MockDriver(ISimpleVehicleContainer container, IDataBus rcontainer) : base(container)
		{
			realContainer = rcontainer;
		}

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => realContainer?.DriverInfo?.DriverBehavior ?? DrivingBehavior.Accelerating;

		public DrivingAction DrivingAction => realContainer?.DriverInfo?.DrivingAction ?? DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => realContainer?.DriverInfo.DriverAcceleration;
		public PCCStates PCCState => PCCStates.OutsideSegment;

		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		protected override bool DoUpdateFrom(object other) => false;

		#endregion
	}
}