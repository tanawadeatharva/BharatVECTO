using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{

    public class SimplePowertrainBuilder : PowertrainBuilderBase, ISimplePowertrainBuilder
	{

		private IVehicleContainerFactory _vehicleContainerFactory;

		public SimplePowertrainBuilder(IVehicleContainerFactory vehicleContainerFactory)
        {
            _vehicleContainerFactory = vehicleContainerFactory;
        }


		public ITestPowertrain<T> CreateTestPowertrain<T>(ISimpleVehicleContainer testContainer, IDataBus dataBus) where T : class, IHybridControlledGearbox, IGearbox
		{
			return new TestPowertrain<T>(testContainer, dataBus);
		}

		public ITestGenset CreateTestGenset(ISimpleVehicleContainer testContainer, IDataBus realContainer)
		{
			return new TestGenset(testContainer, realContainer);
		}

        /// <summary>
        /// Builds a simple conventional powertrain.
        /// <code>
        ///(MeasuredSpeedDrivingCycle)
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     ├(Angledrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └ATGearbox or Gearbox
        ///      ├(TransmissionInputRetarder)
        ///      ├(Clutch)
        ///      └CombustionEngine
        ///       └(Aux)
        /// </code>
        /// </summary>
        public ISimpleVehicleContainer BuildSimplePowertrain(VectoRunData data)
		{
			var container = GetVehicleContainer(data);
			IVehicle vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);
			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.MeasuredSpeed:
					new MeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = GetSimpleGearbox(container, data);
			var idleController = GetIdleController(data.PTO, engine, container);

			vehicle.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius,
					data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
                .AddComponent(new WheelEnd(container, data.WheelEndData))
                .AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(
					data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			if (gearbox is ATGearbox atGbx) {
				atGbx.IdleController = idleController;
			}
			return container;
        }

		public ISimpleVehicleContainer BuildSimpleHybridPowertrainGear(VectoRunData data)
		{
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			var container = GetVehicleContainer(data);
            var es = ConnectREESS(data, container);

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = new MeasuredSpeedHybridsCycleGearbox(container, data);

			var ctl = new SimpleHybridController(container, es) { Gearbox = gearbox, Engine = engine };

			var position = data.ElectricMachinesData[0].Item1;

			TimeRunHybridComponents components = new TimeRunHybridComponents() {
				Cycle = new MeasuredSpeedDrivingCycle(container, data.Cycle),
				Engine = engine,
				Gearbox = gearbox,
				Clutch = AddClutch(data, container),
				IdleController = GetIdleController(data.PTO, engine, container),
				ElectricMotor =
					GetElectricMachine<MeasuredSpeedGearHybridsElectricMotor>(position, data.ElectricMachinesData,
						container, es, ctl),
				HybridController = ctl
			};

			_timerunGearHybridBuilders[position].Invoke(data, container, components);

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddHighVoltageAuxiliaries(data, container, es, dcdc);
			AddHybridBusAuxiliaries(data, container, es, dcdc);
			return container;
        }

		/// <summary>
		/// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
		/// <code>
		/// Vehicle
		/// └Wheels
		///  └SimpleHybridController
		///   └Brakes
		///    │ └Engine E4
		///    └AxleGear
		///     │ ├(AxlegearInputRetarder)
		///     │ └Engine E3
		///     ├(AngleDrive)
		///     ├(TransmissionOutputRetarder)
		///     └Gearbox or APTNGearbox
		///      ├(TransmissionInputRetarder)
		///      └Engine E2
		/// </code>
		/// </summary>
		public ISimpleVehicleContainer BuildSimpleSerialHybridPowertrain(VectoRunData data)
		{
			var container = GetVehicleContainer(data);
			var es = ConnectREESS(data, container);
			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddElectricAuxiliaries(data, container, es, null, dcdc);
			es.Connect(new GensetChargerAdapter(null));

			var ctl = new SimpleHybridController(container, es);

			//Vehicle-->Wheels-->SimpleHybridController-->Brakes
			var powertrain = new Vehicle(container, data.VehicleData, data.AirdragData)
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
                .AddComponent(new WheelEnd(container, data.WheelEndData));

            var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);

					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4,
						data.ElectricMachinesData, container, es, ctl));
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);

					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3,
							data.ElectricMachinesData, container, es, ctl));
					break;

				case PowertrainPosition.BatteryElectricE2:
					//-->AxleGear-->(AngleDrive)-->(TransmissionOutputRetarder)-->APTNGearbox or Gearbox-->(TransmissionInputRetarder)-->Engine E2
					Gearbox gearbox;
					if (data.GearboxData.Type.AutomaticTransmission()) {
						gearbox = new APTNGearbox(container, ctl.ShiftStrategy);
					} else {
						gearbox = new Gearbox(container, ctl.ShiftStrategy);
					}

					ctl.Gearbox = gearbox;
					new DummyEngineInfo(container);

					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null
							? new Angledrive(container, data.AngledriveData)
							: null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2,
							data.ElectricMachinesData, container, es, ctl));
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos,
						"Invalid engine powertrain position for simple serial hybrid vehicles.");
			}
			return container;
        }

		/// <summary>
		/// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
		/// <code>
		/// Vehicle
		/// └Wheels
		///  └SimpleHybridController
		///   └Brakes
		///    │ └Engine E4
		///    └AxleGear
		///     │ ├(AxlegearInputRetarder)
		///     │ └Engine E3
		///     ├(AngleDrive)
		///     ├(TransmissionOutputRetarder)
		///     └Gearbox or APTNGearbox
		///      ├(TransmissionInputRetarder)
		///      └Engine E2
		/// </code>
		/// </summary>
		public ISimpleVehicleContainer BuildSimpleIEPCHybridPowertrain(VectoRunData data)
		{
			var container = GetVehicleContainer(data);
			var es = ConnectREESS(data, container);
			es.Connect(new GensetChargerAdapter(null));

			var ctl = new SimpleHybridController(container, es);

			//Vehicle-->Wheels-->SimpleHybridController-->Brakes
			var powertrain = new Vehicle(container, data.VehicleData, data.AirdragData)
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
                .AddComponent(new WheelEnd(container, data.WheelEndData));

            var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new APTNGearbox(container, ctl.ShiftStrategy)
				: new SingleSpeedGearbox(container, data.GearboxData);
			var em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);
			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddElectricAuxiliaries(data, container, es, null, dcdc);
			return container;
        }

		/// <summary>
		/// Builds a simple genset
		/// <code>
		/// Engine Gen
		///  └CombustionEngine
		/// </code>
		/// </summary>
		public ISimpleVehicleContainer BuildSimpleGenSet(VectoRunData data)
		{
			var container = GetVehicleContainer(data);
			var es = ConnectREESS(data, container);
			var ctl = new GensetMotorController(container, es);

			var ice = new StopStartCombustionEngine(container, data.EngineData);
			AddAuxiliariesSerialHybrid(ice, container, data);

			GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es, ctl)
				.AddComponent(ice);

			new ATClutchInfo(container);
			new DummyGearboxInfo(container, new GearshiftPosition(0));
			new DummyVehicleInfo(container);

			if (data.BusAuxiliaries != null) {
				if (!(container.BusAux is BusAuxiliariesAdapter busAux)) {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}

				var auxCfg = data.BusAuxiliaries;
				var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
					? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
						auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
					: (ISimpleBattery)new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
				if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
					busAux.DCDCConverter = dcdc;
					es.Connect(dcdc);
				}
			}
			return container;
        }

		/// <summary>
		/// Builds a simple hybrid powertrain.
		///<code>
		/// (MeasuredSpeedDrivingCycle)
		///  └Vehicle
		///   └Wheels
		///    └SimpleHybridController
		///     └Brakes
		///      ├(Engine P4)
		///      └AxleGear
		///       ├(Engine P3)
		///       ├(Angledrive)
		///       ├(TransmissionOutputRetarder)
		///       └Gearbox, ATGearbox, or APTNGearbox
		///        ├(TransmissionInputRetarder)
		///        ├(Engine P2.5)
		///        ├(Engine P2)
		///        ├(SwitchableClutch)
		///        ├(Engine P1)
		///        └StopStartCombustionEngine
		///         └(Aux)
		/// </code>
		/// </summary>
		public ISimpleVehicleContainer BuildSimpleHybridPowertrain(VectoRunData data)
		{
			var container = GetVehicleContainer(data);
			var es = ConnectREESS(data, container);
			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddHighVoltageAuxiliaries(data, container, es, dcdc);

			//IMPORTANT HINT: add engine BEFORE gearbox to container that gearbox can obtain if an ICE is available
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = GetSimpleGearbox(container, data);
			if (!(gearbox is IHybridControlledGearbox gbx)) {
				throw new VectoException("Gearbox can not be used for parallel hybrid");
			}

			var ctl = new SimpleHybridController(container, es) { Gearbox = gbx, Engine = engine };
			var idleController = GetIdleController(data.PTO, engine, container);
			var clutch = (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC)
				? new SwitchableClutch(container, data.EngineData)
				: null;


			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					new MeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.EngineOnly: break;
				default: throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = new WHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;
			}

			vehicle.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius,
					data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
                .AddComponent(new WheelEnd(container, data.WheelEndData))
                .AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container,
					es,
					ctl))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				// this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
				if (gearbox is ATGearbox atGbx) {
					atGbx.IdleController = idleController;
					new ATClutchInfo(container);
				} else {
					clutch.IdleController = idleController;
				}
			}

			if (data.BusAuxiliaries != null) {
				if (!(container.BusAux is BusAuxiliariesAdapter busAux)) {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}

				var auxCfg = data.BusAuxiliaries;
				var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
					? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
						auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
					: (ISimpleBattery)new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
				if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					busAux.DCDCConverter = dcdc;
					es.Connect(dcdc);
				}
			}
			return container;
		}

        /// <summary>
        /// Builds a simple battery electric powertrain for PEVs.
        /// <code>
        /// (Dummy MeasureSpeedDrivingCycle)
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     └ATGearbox or Gearbox
        ///      └Electric Motor
        /// </code>
        /// </summary>
        public ISimpleVehicleContainer BuildSimplePowertrainElectric(VectoRunData data)
		{
			var container = GetVehicleContainer(data);
			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					new MeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			var es = ConnectREESS(data, container);
			es.Connect(new SimpleCharger());

			vehicle.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius,
					data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
                .AddComponent(new WheelEnd(container, data.WheelEndData))
                .AddComponent(data.AxleGearData is null ? null : new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(data.GearboxData is null ? null : GetSimpleGearbox(container, data))
				.AddComponent(GetElectricMachine(
					data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1,
					data.ElectricMachinesData, container, es, new SimpleElectricMotorControl()));
			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);

			AddElectricAuxiliaries(data, container, es, null, dcdc);
			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container); // necessary for certain IEPC configurations
			}
			return container;
		}

		protected ISimpleVehicleContainer GetVehicleContainer(VectoRunData runData)
		{
			var container = _vehicleContainerFactory.CreateSimpleVehicleContainer(runData);
			return container;
		}
    }
}