using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
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
		public const string MotorFile240kW = @"TestData\Hybrids\ElectricMotor\GenericEMotor240kW.vem";

		public const string GearboxIndirectLoss = @"TestData\Components\Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData\Components\Direct Gear.vtlm";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		//[TestCase(30, 0.8, 200),
		//TestCase(30, 0.3, 200),
		//TestCase(30, 0.8, -200),
		//TestCase(30, 0.8, 0),
		//]
		//public void P2HybridDirveOff_ElectricAndICE(double vmax, double initialSoC, double electricTorque)
		//{
		//	var cycleData = string.Format(
		//		@"   0,   0, 0,    3
		//		   700, {0}, 0,    0", vmax);
		//	var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

		//	const bool largeMotor = true;
		//	var electricTq = electricTorque.SI<NewtonMeter>();
		//	var run = CreateEngineeringRun(
		//		cycle, string.Format("SimpleParallelHybrid_acc_{0}_{2}-{1}.vmod", vmax, initialSoC, electricTq.Value()),
		//		initialSoC, PowertrainPosition.HybridP2);

		//	var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
		//	Assert.NotNull(hybridController);
		//	//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
		//	//Assert.NotNull(strategy);

		//	var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		//	var nextState = new StrategyState();
		//	var currentState = new StrategyState();

			

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	Assert.IsTrue(modData.Rows.Count > 0);
		//}

		[
			TestCase(30, 0.7, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, level"),
		TestCase(30, 0.22, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.22, level")
			]
		public void P2HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var run = CreateEngineeringRun(
				cycle, string.Format("SimpleParallelHybrid_acc_{0}-{1}_{2}.vmod", vmax, initialSoC, slope), initialSoC, pos, largeMotor: true);

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var nextState = new StrategyState();
			var currentState = new StrategyState();

			

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
		}

		[
		TestCase(30, 0.7, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(80, 0.7, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

		TestCase(30, 0.2, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.2, level"),
		TestCase(50, 0.2, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.2, level"),
		TestCase(80, 0.2, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.2, level"),

		TestCase(30, 0.5, 5, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(80, 0.5, 5, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(80, 0.5, -5, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),
		]
		public void P2HybridConstantSpeed(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var run = CreateEngineeringRun(
				cycle, string.Format("SimpleParallelHybrid_constant_{0}-{1}_{2}.vmod", vmax, initialSoC, slope), initialSoC, pos, largeMotor: true);

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var nextState = new StrategyState();
			var currentState = new StrategyState();



			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
		}

		[
			TestCase(80, 0, TestName = "Conventional DriveOff 80km/h  level"),
		]
		public void ConventionalVehicleDriveOff(double vmax, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			//const bool largeMotor = true;

			//const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var run = CreateConventionalEngineeringRun(
				cycle, string.Format("ConventionalVehicle_acc_{0}_{1}.vmod", vmax, slope));

			//var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			//Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var nextState = new StrategyState();
			var currentState = new StrategyState();



			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
		}


		[TestCase(50, 0.79, 0, TestName = "P2 Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "P2 Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "P2 Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P2HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			//var dst =
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var run = CreateEngineeringRun(
				cycle, string.Format("SimpleParallelHybrid_stop_{0}-{1}_{2}.vmod", vmax, initialSoC, slope), initialSoC, pos, largeMotor: true);

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var nextState = new StrategyState();
			var currentState = new StrategyState();



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

		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, string modFileName, double initialSoc, PowertrainPosition pos, bool largeMotor = false,
			SummaryDataContainer sumData = null, double pAuxEl = 0)
		{
			var container = CreateParallelHybridPowerTrain(
				cycleData, Path.GetFileNameWithoutExtension(modFileName), initialSoc, largeMotor, sumData, pAuxEl, pos);
			return new DistanceRun(container);
		}

		public static VectoRun CreateConventionalEngineeringRun(DrivingCycleData cycleData, string modFileName, 
			SummaryDataContainer sumData = null, double pAuxEl = 0)
		{
			var container = CreateConventionalPowerTrain(
				cycleData, Path.GetFileNameWithoutExtension(modFileName), sumData, pAuxEl);
			return new DistanceRun(container);
		}

		public static VehicleContainer CreateParallelHybridPowerTrain(DrivingCycleData cycleData, string modFileName,
			double initialBatCharge, bool largeMotor, SummaryDataContainer sumData, double pAuxEl, PowertrainPosition pos)
		{
			//var strategySettings = GetHybridStrategyParameters(largeMotor);

			//strategySettings.StrategyName = "SimpleParallelHybridStrategy";

			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(
				modFileName, new IFuelProperties[] { FuelData.Diesel }, fileWriter,
				filters: modDataFilter) {
				WriteModalResults = true,
			};

			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var electricMotorData =
				MockSimulationDataFactory.CreateElectricMotorData(largeMotor ? MotorFile240kW : MotorFile, pos);

			var batteryData = MockSimulationDataFactory.CreateBatteryData(BatFile, initialBatCharge);
			batteryData.TargetSoC = 0.5;

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1).Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var runData = new VectoRunData() {
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
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed)
			};
			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); });
			container.RunData = runData;

			var strategy = new HybridStrategy(runData, container);
			var es = new ElectricSystem(container);
			var battery = new Battery(container, batteryData);
			battery.Initialize(initialBatCharge);

			var clutch = new SwitchableClutch(container, runData.EngineData);
			var ctl = new HybridController(container, strategy, es, clutch);

			es.Connect(battery);

			var gearbox = new Gearbox(container, ctl.ShiftStrategy, runData);
			//var hybridStrategy = new DelegateParallelHybridStrategy();
			ctl.Gearbox = gearbox;

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			var idleController = engine.IdleController;
			ctl.Engine = engine;

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius,
					runData.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(ctl)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent(gearbox, runData.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, runData);

			return container;
		}


		public static VehicleContainer CreateConventionalPowerTrain(DrivingCycleData cycleData, string modFileName,
			SummaryDataContainer sumData, double pAuxEl)
		{
			//var strategySettings = GetHybridStrategyParameters(largeMotor);

			//strategySettings.StrategyName = "SimpleParallelHybridStrategy";

			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(
				modFileName, new IFuelProperties[] { FuelData.Diesel }, fileWriter,
				filters: modDataFilter) {
				WriteModalResults = true,
			};

			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1).Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var runData = new VectoRunData() {
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
				//ElectricMachinesData = electricMotorData,
				EngineData = engineData,
				//BatteryData = batteryData,
				//HybridStrategy = strategySettings
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed)
			};
			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); });
			container.RunData = runData;


			var clutch = new SwitchableClutch(container, runData.EngineData);

			var gbxStrategy = new AMTShiftStrategyOptimized(runData,container);
			
			var gearbox = new Gearbox(container, gbxStrategy, runData);

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			var idleController = engine.IdleController;
			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius,
					runData.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				//.AddComponent(ctl)
				//.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, runData.ElectricMachinesData, container,
				//	es, ctl))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				//.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, runData.ElectricMachinesData, container,
				//	es, ctl))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent(gearbox, runData.Retarder, container)
				//.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, runData.ElectricMachinesData, container,
				//	es, ctl))
				.AddComponent(clutch)
				//.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, runData.ElectricMachinesData, container,
				//	es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, runData);

			return container;
		}

		public static ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			var retVal = new ShiftStrategyParameters {
				StartVelocity = DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
				GearResidenceTime = DeclarationData.GearboxTCU.GearResidenceTime,
				DnT99L_highMin1 = DeclarationData.GearboxTCU.DnT99L_highMin1,
				DnT99L_highMin2 = DeclarationData.GearboxTCU.DnT99L_highMin2,
				AllowedGearRangeUp = DeclarationData.GearboxTCU.AllowedGearRangeUp,
				AllowedGearRangeDown = DeclarationData.GearboxTCU.AllowedGearRangeDown,
				LookBackInterval = DeclarationData.GearboxTCU.LookBackInterval,
				DriverAccelerationLookBackInterval = DeclarationData.GearboxTCU.DriverAccelerationLookBackInterval,
				DriverAccelerationThresholdLow = DeclarationData.GearboxTCU.DriverAccelerationThresholdLow,
				AverageCardanPowerThresholdPropulsion = DeclarationData.GearboxTCU.AverageCardanPowerThresholdPropulsion,
				CurrentCardanPowerThresholdPropulsion = DeclarationData.GearboxTCU.CurrentCardanPowerThresholdPropulsion,
				TargetSpeedDeviationFactor = DeclarationData.GearboxTCU.TargetSpeedDeviationFactor,
				EngineSpeedHighDriveOffFactor = DeclarationData.GearboxTCU.EngineSpeedHighDriveOffFactor,
				RatingFactorCurrentGear = gbx.Type.AutomaticTransmission()
					? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
					: DeclarationData.GearboxTCU.RatingFactorCurrentGear,
				//AccelerationReserveLookup = AccelerationReserveLookupReader.ReadFromStream(
				//	RessourceHelper.ReadStream(
				//		DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.AccelerationReserveLookup.csv")),
				//ShareTorque99L = ShareTorque99lLookupReader.ReadFromStream(
				//	RessourceHelper.ReadStream(
				//		DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareTq99L.csv")
				//),
				//PredictionDurationLookup = PredictionDurationLookupReader.ReadFromStream(
				//	RessourceHelper.ReadStream(
				//		DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.PredictionTimeLookup.csv")
				//),
				//ShareIdleLow = ShareIdleLowReader.ReadFromStream(
				//	RessourceHelper.ReadStream(
				//		DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareIdleLow.csv")
				//),
				//ShareEngineHigh = EngineSpeedHighLookupReader.ReadFromStream(
				//	RessourceHelper.ReadStream(
				//		DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareEngineSpeedHigh.csv")
				//),

				//--------------------
				RatioEarlyUpshiftFC = DeclarationData.GearboxTCU.RatioEarlyUpshiftFC / axleRatio,
				RatioEarlyDownshiftFC = DeclarationData.GearboxTCU.RatioEarlyDownshiftFC / axleRatio,
				AllowedGearRangeFC = gbx.Type.AutomaticTransmission()
					? (gbx.Gears.Count > DeclarationData.GearboxTCU.ATSkipGearsThreshold
						? DeclarationData.GearboxTCU.AllowedGearRangeFCATSkipGear
						: DeclarationData.GearboxTCU.AllowedGearRangeFCAT)
					: DeclarationData.GearboxTCU.AllowedGearRangeFCAMT,
				VelocityDropFactor = DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
				ATLookAheadTime = DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
														.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
			};

			return retVal;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, VehicleContainer container,
			IElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			container.ModData.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			motor.Connect(es);
			return motor;
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };

			return new GearboxData {
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									string.Format("Gear {0}", i)),
							Ratio = ratio,
							//ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(i,)
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
			return new AxleGearData {
				AxleGear = new GearData {
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
			return new VehicleData {
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
			return new AirdragData() {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(
						3.2634.SI<SquareMeter>(),
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
			};
		}

		private static DriverData CreateDriverData(string accelerationFile, bool overspeed = false)
		{
			return new DriverData {
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData {
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