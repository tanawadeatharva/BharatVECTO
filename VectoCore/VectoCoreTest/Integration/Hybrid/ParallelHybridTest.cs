using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModFilter;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.ElectricSystem;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
	[TestFixture]
	public class ParallelHybridTest
	{

		public const string MotorFile = @"TestData\Hybrids\ElectricMotor\GenericEMotor.vem";
		public const string BatFile = @"TestData\Hybrids\Battery\GenericBattery.vbat";

		public const string AccelerationFile = @"TestData\Components\Truck.vacc";
		public const string MotorFile240kW = @"TestData\Hybrid\ElectricMotor\GenericEMotor240kW.vem";

		public const string GearboxIndirectLoss = @"TestData\Components\Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData\Components\Direct Gear.vtlm";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(30, 0.8, 500)]
		public void P2HybridDirveOff_ElectricAndICE(double vmax, double initialSoC, double electricTorque)
		{
			var cycleData = string.Format(
				@"   0,   0, 0,    3
				   700, {0}, 0,    0", vmax);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			RunHybridSimulation(vmax, initialSoC, electricTorque, cycle);
		}

		public void RunHybridSimulation(double vmax, double initialSoC, double electricTorque, DrivingCycleData cycle) 
		{
			const bool largeMotor = true;
			var run = CreateEngineeringRun(
				cycle, string.Format("SimpleParallelHybrid_acc_{0}_{2}-{1}.vmod", vmax, initialSoC, electricTorque), initialSoC,
				new DelegateParallelHybridStrategy(), PowertrainPosition.HybridP2);

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var nextState = new StrategyState();
			var currentState = new StrategyState();
			strategy.RequestFunc = (absTime, b, c, d, dryRun) => {
				var shiftAllowed =  absTime > currentState.lastGearShift + 2.SI<Second>();
				var triggerGearshift = shiftAllowed && ( run.GetContainer().EngineSpeed > 1600.RPMtoRad() ||
										run.GetContainer().EngineSpeed < 625.RPMtoRad());
				//var nextGear = run.GetContainer().Gear;
				if (!dryRun && triggerGearshift) {
					nextState.lastGearShift = absTime;
					nextState.nextGear = (uint)(run.GetContainer().Gear + (run.GetContainer().EngineSpeed > 1600.RPMtoRad()
						? 1
						: (run.GetContainer().EngineSpeed < 625.RPMtoRad() ? -1 : 0)));
				}
				return new HybridStrategyResponse {
					MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>()
						{ { PowertrainPosition.HybridP2, 0.SI<NewtonMeter>() } },
					ShiftRequired = triggerGearshift,
					NextGear = nextState.nextGear
				};
			};
			strategy.InitializeFunc = (a, b) => new HybridStrategyResponse {
				MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>()
					{ { PowertrainPosition.HybridP2, 0.SI<NewtonMeter>() } }
			};
			strategy.CommitFunc = () => {
				currentState = nextState;
				nextState = new StrategyState() {
					lastGearShift = currentState.lastGearShift,
					nextGear = currentState.nextGear,
				};
			};

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
		}

		public class StrategyState
		{
			public Second lastGearShift = -double.MaxValue.SI<Second>();
			public Second requestTstmp = -double.MaxValue.SI<Second>();
			public uint nextGear = 0u;
		}

		// =================================================

		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, string modFileName, double initialSoc, IHybridControlStrategy hybridStrategy, PowertrainPosition pos, bool largeMotor = false, SummaryDataContainer sumData = null, double pAuxEl = 0)
		{
			var container = CreateParallelHybridPowerTrain(
				cycleData, Path.GetFileNameWithoutExtension(modFileName), initialSoc, largeMotor, sumData, hybridStrategy, pAuxEl, pos);
			return new DistanceRun(container);
		}

		public static VehicleContainer CreateParallelHybridPowerTrain(DrivingCycleData cycleData, string modFileName, double initialBatCharge, bool largeMotor, SummaryDataContainer sumData, IHybridControlStrategy hybridStrategy, double pAuxEl, PowertrainPosition pos)
		{
			//var strategySettings = GetHybridStrategyParameters(largeMotor);

			//strategySettings.StrategyName = "SimpleParallelHybridStrategy";

			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] {}; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(
				modFileName, new IFuelProperties[] {FuelData.Diesel}, fileWriter, 
				filters: modDataFilter)
			{
				WriteModalResults = true,
			};

			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var electricMotorData = MockSimulationDataFactory.CreateElectricMotorData(largeMotor ? MotorFile240kW : MotorFile, pos);

			var batteryData = MockSimulationDataFactory.CreateBatteryData(BatFile, initialBatCharge);

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			var runData = new VectoRunData()
			{
				//PowertrainConfiguration = PowertrainConfiguration.ParallelHybrid,
				JobRunId = 0,
				DriverData = driverData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = modFileName,
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				EngineData = engineData,
				BatteryData = batteryData,
				//HybridStrategy = strategySettings
			};
			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); });
			container.RunData = runData;

			var es = new ElectricSystem(container);
			var battery = new Battery(container, batteryData);
			battery.Initialize(initialBatCharge);

			var clutch = new SwitchableClutch(container, runData.EngineData);
			var ctl = new HybridController(container, hybridStrategy, es, clutch);

			es.Connect(battery);

			var gearbox = new Gearbox(container, ctl.ShiftStrategy, runData);
			//var hybridStrategy = new DelegateParallelHybridStrategy();

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			var idleController = engine.IdleController;

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius, runData.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(ctl)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent(gearbox, runData.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, runData);

			return container;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, VehicleContainer container,
			IElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null)
			{
				return null;
			}

			container.ModData.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos);
			var motor = new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			motor.Connect(es);
			return motor;
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };

			return new GearboxData
			{
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData
						{
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									string.Format("Gear {0}", i)),
							Ratio = ratio,
							//ShiftPolygon = ShiftPolygonReader.ReadFromFile(ShiftPolygonFile)
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				ShiftTime = 2.SI<Second>(),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				TorqueReserve = 0.2,
				StartTorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
			};
		}

		private static AxleGearData CreateAxleGearData()
		{
			var ratio = 2.59;
			return new AxleGearData
			{
				AxleGear = new GearData
				{
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.38,
					Inertia = 20.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = false,
					TyreTestLoad = 30436.0.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.62,
					Inertia = 18.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = true,
					TyreTestLoad = 30436.SI<Newton>()
				},
			};
			return new VehicleData
			{
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.465.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false
			};
		}

		private static AirdragData CreateAirdragData()
		{
			return new AirdragData()
			{
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(
						3.2634.SI<SquareMeter>(),
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
			};
		}

		private static DriverData CreateDriverData(string accelerationFile, bool overspeed = false)
		{
			return new DriverData
			{
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData
				{
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),

					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
					LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				OverSpeed = new DriverData.OverSpeedData() {
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),
						OverSpeed = 5.KMPHtoMeterPerSecond()
				},
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay = DeclarationData.Driver.EngineStopStart.ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
					UtilityFactor = DeclarationData.Driver.EngineStopStart.UtilityFactor
				}
			};
		}
	}
}