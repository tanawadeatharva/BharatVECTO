using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
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
	[Parallelizable(ParallelScope.All)]
	public class ParallelHybridTest
	{
		//private ModalResultField[] Yfields;
		public const string MotorFile = @"TestData\Hybrids\ElectricMotor\GenericEMotor.vem";
		public const string BatFile = @"TestData\Hybrids\Battery\GenericBattery.vbat";

		public const string AccelerationFile = @"TestData\Components\Truck.vacc";
		public const string MotorFile240kW = @"TestData\Hybrids\ElectricMotor\GenericEMotor240kW.vem";

		public const string P1HybridMotor = @"Testdata\Hybrids\GenericVehicle_P1-APT\GenericEMotor20kW.vem";
		public const string P1BatteryFile = @"Testdata\Hybrids\GenericVehicle_P1-APT\GenericBattery.vbat";

		public const string GearboxIndirectLoss = @"TestData\Components\Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData\Components\Direct Gear.vtlm";


		public const string TorqueConverterSerial = @"TestData\Hybrids\GenericVehicle_P1-APT\TorqueConverter.vtcc";
		public const string TorqueConverterPowerSplit = @"TestData\Hybrids\GenericVehicle_P1-APT\TorqueConverterPowerSplit.vtcc";

		public const bool PlotGraphs = true;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);


			//InitGraphWriter();
		}


		private GraphWriter GetGraphWriter(ModalResultField[] emYFields)
		{
			var Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.P_ice_out, ModalResultField.REESSStateOfCharge, ModalResultField.FCMap
			}.Concat(emYFields).ToArray();

			var graphWriter = new GraphWriter();
			graphWriter.Xfields = new[] { ModalResultField.dist };
			graphWriter.Yfields = Yfields;
			graphWriter.Series1Label = "Hybrid";
			graphWriter.PlotIgnitionState = true;

			if (PlotGraphs) {
				graphWriter.Enable();
			} else {
				graphWriter.Disable();
			}

			return graphWriter;
		}

		// --------------------------------------

		[
		TestCase(30, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(70, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.7, level"),

		TestCase(30, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.25, level"),
		TestCase(50, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.25, level"),
		TestCase(70, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.25, level"),

		TestCase(30, 0.5, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(70, 0.5, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(70, 0.5, -5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.5, DH 5%"),

		//------

		TestCase(30, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(70, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.7, level"),

		TestCase(30, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.25, level"),
		TestCase(50, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.25, level"),
		TestCase(70, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.25, level"),

		TestCase(30, 0.5, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(70, 0.5, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(70, 0.5, -5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.5, DH 5%"),
		]
		public void P1HybridConstantSpeed(double vmax, double initialSoC, double slope, GearboxType gbxType)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P1_constant_{0}-{1}_{2}_{3}.vmod", vmax, initialSoC, slope, gbxType.ToXMLFormat());
			const PowertrainPosition pos = PowertrainPosition.HybridP1;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: false, gearboxType: gbxType);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P1 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 80km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 80km/h SoC: 0.7, UH 5"),
			TestCase(30, 0.22, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 30km/h SoC: 0.22, level"),

			TestCase(30, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 80km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 80km/h SoC: 0.7, UH 5"),
			TestCase(30, 0.22, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 30km/h SoC: 0.22, level"),

		]
		public void P1HybridDriveOff(double vmax, double initialSoC, double slope, GearboxType gbxType)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P1 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P1_acc_{0}-{1}_{2}_{3}.vmod", vmax, initialSoC, slope, gbxType.ToXMLFormat());
			const PowertrainPosition pos = PowertrainPosition.HybridP1;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: false, gearboxType: gbxType);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
			TestCase(50, 0.79, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S Brake Standstill 50km/h SoC: 0.79, level"),
			TestCase(50, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S Brake Standstill 50km/h SoC: 0.25, level"),
			TestCase(50, 0.65, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S Brake Standstill 50km/h SoC: 0.65, level"),

			TestCase(50, 0.79, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P Brake Standstill 50km/h SoC: 0.79, level"),
			TestCase(50, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P Brake Standstill 50km/h SoC: 0.25, level"),
			TestCase(50, 0.65, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P1HybridBrakeStandstill(double vmax, double initialSoC, double slope, GearboxType gbxType)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P1_stop_{0}-{1}_{2}_{3}.vmod", vmax, initialSoC, slope, gbxType.ToXMLFormat());
			const PowertrainPosition pos = PowertrainPosition.HybridP1;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: false, gearboxType: gbxType);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P1 });
			graphWriter.Write(modFilename);
		}

		const string TestJobP1_APTS = @"TestData\Hybrids\GenericVehicle_P1-APT\CityBus_AT_Ser.vecto";
		const string TestJobP1_APTP = @"TestData\Hybrids\GenericVehicle_P1-APT\CityBus_AT_PS.vecto";

		private const string TestJobCityBusP1_APTP = @"TestData\Hybrids\Citybus_P1-APT-P-220kW-7.7l\CityBus_AT-P.vecto";

		[
			TestCase(TestJobP1_APTS, 0, TestName = "P1 Hybrid APT-S, DriveCycle LongHaul"),
			TestCase(TestJobP1_APTS, 1, TestName = "P1 Hybrid APT-S, DriveCycle Coach"),
			TestCase(TestJobP1_APTS, 2, TestName = "P1 Hybrid APT-S, DriveCycle Construction"),
			TestCase(TestJobP1_APTS, 3, TestName = "P1 Hybrid APT-S, DriveCycle HeavyUrban"),
			TestCase(TestJobP1_APTS, 4, TestName = "P1 Hybrid APT-S, DriveCycle Interurban"),
			TestCase(TestJobP1_APTS, 5, TestName = "P1 Hybrid APT-S, DriveCycle MunicipalUtility"),
			TestCase(TestJobP1_APTS, 6, TestName = "P1 Hybrid APT-S, DriveCycle RegionalDelivery"),
			TestCase(TestJobP1_APTS, 7, TestName = "P1 Hybrid APT-S, DriveCycle Suburban"),
			TestCase(TestJobP1_APTS, 8, TestName = "P1 Hybrid APT-S, DriveCycle Urban"),
			TestCase(TestJobP1_APTS, 9, TestName = "P1 Hybrid APT-S, DriveCycle UrbanDelivery"),

			TestCase(TestJobP1_APTP, 0, TestName = "P1 Hybrid APT-P, DriveCycle LongHaul"),
			TestCase(TestJobP1_APTP, 1, TestName = "P1 Hybrid APT-P, DriveCycle Coach"),
			TestCase(TestJobP1_APTP, 2, TestName = "P1 Hybrid APT-P, DriveCycle Construction"),
			TestCase(TestJobP1_APTP, 3, TestName = "P1 Hybrid APT-P, DriveCycle HeavyUrban"),
			TestCase(TestJobP1_APTP, 4, TestName = "P1 Hybrid APT-P, DriveCycle Interurban"),
			TestCase(TestJobP1_APTP, 5, TestName = "P1 Hybrid APT-P, DriveCycle MunicipalUtility"),
			TestCase(TestJobP1_APTP, 6, TestName = "P1 Hybrid APT-P, DriveCycle RegionalDelivery"),
			TestCase(TestJobP1_APTP, 7, TestName = "P1 Hybrid APT-P, DriveCycle Suburban"),
			TestCase(TestJobP1_APTP, 8, TestName = "P1 Hybrid APT-P, DriveCycle Urban"),
			TestCase(TestJobP1_APTP, 9, TestName = "P1 Hybrid APT-P, DriveCycle UrbanDelivery"),

            TestCase(TestJobCityBusP1_APTP, 0, TestName = "P1 CityBus Hybrid APT-P, DriveCycle LongHaul"),
            TestCase(TestJobCityBusP1_APTP, 1, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Coach"),
            TestCase(TestJobCityBusP1_APTP, 2, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Construction"),
            TestCase(TestJobCityBusP1_APTP, 3, TestName = "P1 CityBus Hybrid APT-P, DriveCycle HeavyUrban"),
            TestCase(TestJobCityBusP1_APTP, 4, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Interurban"),
            TestCase(TestJobCityBusP1_APTP, 5, TestName = "P1 CityBus Hybrid APT-P, DriveCycle MunicipalUtility"),
            TestCase(TestJobCityBusP1_APTP, 6, TestName = "P1 CityBus Hybrid APT-P, DriveCycle RegionalDelivery"),
            TestCase(TestJobCityBusP1_APTP, 7, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Suburban"),
            TestCase(TestJobCityBusP1_APTP, 8, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Urban"),
			TestCase(TestJobCityBusP1_APTP, 9, TestName = "P1 CityBus Hybrid APT-P, DriveCycle UrbanDelivery"),
        ]
		public void P1APTHybridDriveCycle(string jobFile, int cycleIdx)
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, writer) {
				Validate = false,
				WriteModalResults = true,
			};

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[cycleIdx];

			Assert.NotNull(run);

			var pt = run.GetContainer();

			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			//jobContainer.AddRuns(factory);
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			//Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}

		// =======================================================================================

		[
			TestCase(30, 0.7, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 5, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, UH 5"),
		TestCase(30, 0.22, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.22, level"),
		]
		public void P2HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P2_acc_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}


		[
			TestCase(80, 0.7, 5, 320, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, UH 5% MaxPWR: 320kW"),
			
		]
		public void P2HybridDriveOffLimitPwr(double vmax, double initialSoC, double slope, double maxPwrkW)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P2_acc_{0}-{1}_{2}_maxPwr-{3}.vmod", vmax, initialSoC, slope, maxPwrkW);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, maxDriveTrainPower: (maxPwrkW * 1000).SI<Watt>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
		TestCase(30, 0.7, 0, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(80, 0.7, 0, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

		TestCase(30, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
		TestCase(50, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
		TestCase(80, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

		TestCase(30, 0.5, 5, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(80, 0.5, 5, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(80, 0.5, -5, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

		TestCase(30, 0.25, 0, 1000, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		]
		public void P2HybridConstantSpeed(double vmax, double initialSoC, double slope, double  pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P2_constant_{0}-{1}_{2}_{3}.vmod", vmax, initialSoC, slope, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase("LongHaul", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Construction", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Urban", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Suburban", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Interurban", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Coach", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
		]
		public void P2HybriDriveCycleOffLimitPwr(string declarationMission, double payload, double initialSoC, double pAuxEl, double maxPwrkW)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P2_cycle_{0}-{1}_{2}_{3}_maxPwr-{3}.vmod", declarationMission, initialSoC, payload, pAuxEl, maxPwrkW);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),

			TestCase("LongHaul", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Construction", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Urban", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Suburban", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Interurban", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Coach", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
		]
		public void P2HybriDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P2_cycle_{0}-{1}_{2}_{3}.vmod", declarationMission, initialSoC, payload, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}


		public const string Group5TestJob = @"TestData\Hybrids\GenericVehicle_Group5_P2\P2 Group 5.vecto";

		public const string Group5TestJob325kW = @"TestData\Hybrids\GenericVehicle_Group5_P2\P2 Group 5_325kW.vecto";

		public const string Group5TestJob_noESS = @"TestData\Hybrids\GenericVehicle_Group5_P2\P2 Group 5.vecto";

		public const string Group5TestJob325kW_noESS = @"TestData\Hybrids\GenericVehicle_Group5_P2\P2 Group 5_325kW.vecto";

		public const string Group2TestJob = @"TestData\Hybrids\Hyb_P2_Group2\Class2_RigidTruck_ParHyb_ENG.vecto";

		public const string Group2TestJob180kW = @"TestData\Hybrids\Hyb_P2_Group2\Class2_RigidTruck_ParHyb_ENG.vecto";

		public const string Group5_80kWh_TestJob = @"TestData\Hybrids\Hyb_P2_Group5\Hyb_P2_Group5_80kWh.vecto";

		public const string Group5TestJob2 = @"TestData\Hybrids\Hyb_P2_Group5\Hyb_P2_Group5.vecto";

		public const string Group2TestJobSuperCapOvl = @"TestData\Hybrids\Hyb_P2_Group2SuperCapOvl\Class2_RigidTruck_ParHyb_SuperCap_Ovl_ENG.vecto";

		public const string CityBus6x2 = @"TestData\Hybrids\Input CityBus 6x2_HEV_P2\CityBus_6x2_HEV_P2.vecto";

		public const string Group5_EMTorqueLimit_TestJob = @"TestData\Hybrids\GenericVehicle_Group5_P2\P2 Group 5_LimitEMTorqueDrive.vecto";

		public const string Group5_LimitPropTq_TestJob = @"TestData\Hybrids\GenericVehicle_Group5_P2\P2 Group 5_LimitVehiclePropTq.vecto";

		[
		TestCase(Group5TestJob, 0, TestName = "P2 Hybrid Group 5 DriveCycle LongHaul"),
		TestCase(Group5TestJob, 1, TestName = "P2 Hybrid Group 5 DriveCycle Coach"),  // error
		TestCase(Group5TestJob, 2, TestName = "P2 Hybrid Group 5 DriveCycle Construction"),  // error
		TestCase(Group5TestJob, 3, TestName = "P2 Hybrid Group 5 DriveCycle HeavyUrban"),
		TestCase(Group5TestJob, 4, TestName = "P2 Hybrid Group 5 DriveCycle Interurban"),  // error
		TestCase(Group5TestJob, 5, TestName = "P2 Hybrid Group 5 DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob, 6, TestName = "P2 Hybrid Group 5 DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob, 7, TestName = "P2 Hybrid Group 5 DriveCycle Suburban"),  // error
		TestCase(Group5TestJob, 8, TestName = "P2 Hybrid Group 5 DriveCycle Urban"), 
		TestCase(Group5TestJob, 9, TestName = "P2 Hybrid Group 5 DriveCycle UrbanDelivery"), // error

		TestCase(Group5TestJob325kW, 0, TestName = "P2 Hybrid Group 5 325kW DriveCycle LongHaul"),
		TestCase(Group5TestJob325kW, 1, TestName = "P2 Hybrid Group 5 325kW DriveCycle Coach"),  // error
		TestCase(Group5TestJob325kW, 2, TestName = "P2 Hybrid Group 5 325kW DriveCycle Construction"),  
		TestCase(Group5TestJob325kW, 3, TestName = "P2 Hybrid Group 5 325kW DriveCycle HeavyUrban"), 
		TestCase(Group5TestJob325kW, 4, TestName = "P2 Hybrid Group 5 325kW DriveCycle Interurban"),  // error
		TestCase(Group5TestJob325kW, 5, TestName = "P2 Hybrid Group 5 325kW DriveCycle MunicipalUtility"), 
		TestCase(Group5TestJob325kW, 6, TestName = "P2 Hybrid Group 5 325kW DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob325kW, 7, TestName = "P2 Hybrid Group 5 325kW DriveCycle Suburban"), 
		TestCase(Group5TestJob325kW, 8, TestName = "P2 Hybrid Group 5 325kW DriveCycle Urban"), 
		TestCase(Group5TestJob325kW, 9, TestName = "P2 Hybrid Group 5 325kW DriveCycle UrbanDelivery"), 

		TestCase(Group2TestJob, 0, TestName = "P2 Hybrid Group 2 DriveCycle LongHaul"),
		TestCase(Group2TestJob, 1, TestName = "P2 Hybrid Group 2 DriveCycle RegionalDelivery"),
		TestCase(Group2TestJob, 2, TestName = "P2 Hybrid Group 2 DriveCycle UrbanDelivery"),  // error
		TestCase(Group2TestJob, 3, TestName = "P2 Hybrid Group 2 DriveCycle Coach"), // 
		TestCase(Group2TestJob, 4, TestName = "P2 Hybrid Group 2 DriveCycle Construction"), // error
		TestCase(Group2TestJob, 5, TestName = "P2 Hybrid Group 2 DriveCycle HeavyUrban"),
		TestCase(Group2TestJob, 6, TestName = "P2 Hybrid Group 2 DriveCycle Interurban"), // error
		TestCase(Group2TestJob, 7, TestName = "P2 Hybrid Group 2 DriveCycle MunicipalUtility"),
		TestCase(Group2TestJob, 8, TestName = "P2 Hybrid Group 2 DriveCycle Suburban"),
		TestCase(Group2TestJob, 9, TestName = "P2 Hybrid Group 2 DriveCycle Urban"),

		TestCase(Group5_80kWh_TestJob, 0, TestName = "P2 Hybrid Group 5 80kWh DriveCycle LongHaul"),
		TestCase(Group5_80kWh_TestJob, 1, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Coach"), 
		TestCase(Group5_80kWh_TestJob, 2, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Construction"), 
		TestCase(Group5_80kWh_TestJob, 3, TestName = "P2 Hybrid Group 5 80kWh DriveCycle HeavyUrban"), 
		TestCase(Group5_80kWh_TestJob, 4, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Interurban"), 
		TestCase(Group5_80kWh_TestJob, 5, TestName = "P2 Hybrid Group 5 80kWh DriveCycle MunicipalUtility"),
		TestCase(Group5_80kWh_TestJob, 6, TestName = "P2 Hybrid Group 5 80kWh DriveCycle RegionalDelivery"),
		TestCase(Group5_80kWh_TestJob, 7, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Suburban"),
		TestCase(Group5_80kWh_TestJob, 8, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Urban"),  
		TestCase(Group5_80kWh_TestJob, 9, TestName = "P2 Hybrid Group 5 80kWh DriveCycle UrbanDelivery"), 

		TestCase(Group5TestJob2, 0, TestName = "P2 Hybrid Group 5 B DriveCycle LongHaul"),
		TestCase(Group5TestJob2, 1, TestName = "P2 Hybrid Group 5 B DriveCycle Coach"), // error
		TestCase(Group5TestJob2, 2, TestName = "P2 Hybrid Group 5 B DriveCycle Construction"), // error
		TestCase(Group5TestJob2, 3, TestName = "P2 Hybrid Group 5 B DriveCycle HeavyUrban"), //error 
		TestCase(Group5TestJob2, 4, TestName = "P2 Hybrid Group 5 B DriveCycle Interurban"),  // error
		TestCase(Group5TestJob2, 5, TestName = "P2 Hybrid Group 5 B DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob2, 6, TestName = "P2 Hybrid Group 5 B DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob2, 7, TestName = "P2 Hybrid Group 5 B DriveCycle Suburban"), // error
		TestCase(Group5TestJob2, 8, TestName = "P2 Hybrid Group 5 B DriveCycle Urban"),  // error
		TestCase(Group5TestJob2, 9, TestName = "P2 Hybrid Group 5 B DriveCycle UrbanDelivery"),  // error

		//// P2 without ESS is not really relevant
		TestCase(Group5TestJob_noESS, 0, TestName = "P2 Hybrid Group 5 NoESS DriveCycle LongHaul"),
		TestCase(Group5TestJob_noESS, 1, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Coach"),  // error
		TestCase(Group5TestJob_noESS, 2, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Construction"),  // 
		TestCase(Group5TestJob_noESS, 3, TestName = "P2 Hybrid Group 5 NoESS DriveCycle HeavyUrban"),
		TestCase(Group5TestJob_noESS, 4, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Interurban"),  // error
		TestCase(Group5TestJob_noESS, 5, TestName = "P2 Hybrid Group 5 NoESS DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob_noESS, 6, TestName = "P2 Hybrid Group 5 NoESS DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob_noESS, 7, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Suburban"),  // 
		TestCase(Group5TestJob_noESS, 8, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Urban"), 
		TestCase(Group5TestJob_noESS, 9, TestName = "P2 Hybrid Group 5 NoESS DriveCycle UrbanDelivery"), // 

		//// P2 without ESS is not really relevant
		TestCase(Group5TestJob325kW_noESS, 0, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle LongHaul"),
		TestCase(Group5TestJob325kW_noESS, 1, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Coach"),  // error
		 TestCase(Group5TestJob325kW_noESS, 2, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Construction"),  // 
		 TestCase(Group5TestJob325kW_noESS, 3, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle HeavyUrban"), // 
		 TestCase(Group5TestJob325kW_noESS, 4, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Interurban"),  // error
		TestCase(Group5TestJob325kW_noESS, 5, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle MunicipalUtility"), 
		TestCase(Group5TestJob325kW_noESS, 6, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob325kW_noESS, 7, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Suburban"),  // error
		TestCase(Group5TestJob325kW_noESS, 8, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Urban"), // error
		TestCase(Group5TestJob325kW_noESS, 9, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle UrbanDelivery"), // error 

		//TestCase(CityBus6x2, 0, TestName = "P2 Hybrid CityBus DriveCycle Coach"),  // error
		//TestCase(CityBus6x2, 1, TestName = "P2 Hybrid CityBus DriveCycle HeavyUrban"),  // error
		//TestCase(CityBus6x2, 2, TestName = "P2 Hybrid CityBus DriveCycle Interurban"),  // error
		//TestCase(CityBus6x2, 3, TestName = "P2 Hybrid CityBus DriveCycle LongHaul"),  // error
		//TestCase(CityBus6x2, 4, TestName = "P2 Hybrid CityBus DriveCycle RegionalDelivery"),  // error
		//TestCase(CityBus6x2, 5, TestName = "P2 Hybrid CityBus DriveCycle Suburban"),  // error
		//TestCase(CityBus6x2, 6, TestName = "P2 Hybrid CityBus DriveCycle Urban"),  // error
		//TestCase(CityBus6x2, 7, TestName = "P2 Hybrid CityBus DriveCycle UrbanDelivery"),  // error

		TestCase(Group2TestJobSuperCapOvl, 0, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle LongHaul"),
		TestCase(Group2TestJobSuperCapOvl, 1, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle RegionalDelivery"), 
		TestCase(Group2TestJobSuperCapOvl, 2, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle UrbanDelivery"), 
		TestCase(Group2TestJobSuperCapOvl, 3, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Coach"), 
		TestCase(Group2TestJobSuperCapOvl, 4, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Construction"),  
		TestCase(Group2TestJobSuperCapOvl, 5, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle HeavyUrban"),
		TestCase(Group2TestJobSuperCapOvl, 6, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Interurban"),
		TestCase(Group2TestJobSuperCapOvl, 7, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle MunicipalUtility"), 
		TestCase(Group2TestJobSuperCapOvl, 8, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Suburban"),
		TestCase(Group2TestJobSuperCapOvl, 9, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Urban"),

		TestCase(Group5_EMTorqueLimit_TestJob, 0, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle LongHaul"),
		TestCase(Group5_EMTorqueLimit_TestJob, 1, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Coach "),
		TestCase(Group5_EMTorqueLimit_TestJob, 2, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Construction"),
		TestCase(Group5_EMTorqueLimit_TestJob, 3, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle HeavyUrban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 4, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Interurban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 5, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle MunicipalUtility"),
		TestCase(Group5_EMTorqueLimit_TestJob, 6, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle RegionalDelivery"),
		TestCase(Group5_EMTorqueLimit_TestJob, 7, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Suburban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 8, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Urban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 9, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle UrbanDelivery"),

		TestCase(Group5_LimitPropTq_TestJob, 0, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle LongHaul"),
		TestCase(Group5_LimitPropTq_TestJob, 1, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Coach "),
		TestCase(Group5_LimitPropTq_TestJob, 2, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Construction"),
		TestCase(Group5_LimitPropTq_TestJob, 3, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle HeavyUrban"),
		TestCase(Group5_LimitPropTq_TestJob, 4, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Interurban"),
		TestCase(Group5_LimitPropTq_TestJob, 5, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle MunicipalUtility"),
		TestCase(Group5_LimitPropTq_TestJob, 6, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle RegionalDelivery"),
		TestCase(Group5_LimitPropTq_TestJob, 7, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Suburban"),
		TestCase(Group5_LimitPropTq_TestJob, 8, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Urban"),
		TestCase(Group5_LimitPropTq_TestJob, 9, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle UrbanDelivery"),

			]
		public void P2HybridGroup5DriveCycle(string jobFile, int cycleIdx)
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			
			var writer = new FileOutputWriter(jobFile);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, writer) {
				Validate = false,
				WriteModalResults = true,
			};

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[cycleIdx];

			Assert.NotNull(run);

			var pt = run.GetContainer();

			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			//jobContainer.AddRuns(factory);
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			//Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
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
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P2_stop_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - 


		[
			TestCase(30, 0.7, 0, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
			TestCase(80, 0.25, 0, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			TestCase(80, 0.5, 5, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			TestCase(80, 0.5, -5, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		]
		public void P3HybridConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P3_constant_{0}-{1}_{2}_{3}.vmod", vmax, initialSoC, slope, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, TestName = "P3 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P3 Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.22, 0, TestName = "P3 Hybrid DriveOff 30km/h SoC: 0.22, level")
		]
		public void P3HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P3_acc_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}

		[TestCase(50, 0.79, 0, TestName = "P3 Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "P3 Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "P3 Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P3HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			//var dst =
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P3_stop_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}


		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
		]
		public void P3HybriDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P3_cycle_{0}-{1}_{2}_{3}.vmod", declarationMission, initialSoC, payload, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - 

		[
			TestCase(30, 0.7, 0, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
			TestCase(80, 0.25, 0, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			TestCase(80, 0.5, 5, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			TestCase(80, 0.5, -5, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		]
		public void P4HybridConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P4_constant_{0}-{1}_{2}_{3}.vmod", vmax, initialSoC, slope, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, TestName = "P4 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P4 Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.22, 0, TestName = "P4 Hybrid DriveOff 30km/h SoC: 0.22, level")
		]
		public void P4HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P4_acc_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		[TestCase(50, 0.79, 0, TestName = "P4 Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "P4 Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "P4 Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P4HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P4_stop_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
		]
		public void P4HybriDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-P4_cycle_{0}-{1}_{2}_{3}.vmod", declarationMission, initialSoC, payload, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		// =================================================

		public static JobContainer CreateEngineeringRun(DrivingCycleData cycleData, string modFileName,
			double initialSoc, PowertrainPosition pos, double ratio, bool largeMotor = false, double pAuxEl = 0,
			Kilogram payload = null, Watt maxDriveTrainPower = null, GearboxType gearboxType = GearboxType.NoGeabox)
		{
			var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(modFileName));
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var container = CreateParallelHybridPowerTrain(
				cycleData, modFileName, initialSoc, largeMotor, sumData, pAuxEl, pos, ratio, payload,
				maxDriveTrainPower, gearboxType);
			var run = new DistanceRun(container);
			jobContainer.AddRun(run);
			return jobContainer;
		}

		public static VectoRun CreateConventionalEngineeringRun(DrivingCycleData cycleData, string modFileName, 
			SummaryDataContainer sumData = null, double pAuxEl = 0)
		{
			var container = CreateConventionalPowerTrain(
				cycleData, Path.GetFileNameWithoutExtension(modFileName), sumData, pAuxEl);
			return new DistanceRun(container);
		}

		public static VehicleContainer CreateParallelHybridPowerTrain(DrivingCycleData cycleData, string modFileName,
			double initialBatCharge, bool largeMotor, SummaryDataContainer sumData, double pAuxEl,
			PowertrainPosition pos, double ratio, Kilogram payload = null, Watt maxDriveTrainPower = null, GearboxType gearboxType = GearboxType.NoGeabox)
		{ 
			var gearboxData = CreateGearboxData(gearboxType);
			var axleGearData = CreateAxleGearData(gearboxType);

			var vehicleData = CreateVehicleData(payload ??3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var emFile = pos == PowertrainPosition.HybridP1 ? P1HybridMotor : (largeMotor ? MotorFile240kW : MotorFile);
			var electricMotorData = MockSimulationDataFactory.CreateElectricMotorData(emFile, 1, pos, ratio, 1);

			var batFile = pos == PowertrainPosition.HybridP1 ? P1BatteryFile : BatFile;
			var batteryData = MockSimulationDataFactory.CreateBatteryData(batFile, initialBatCharge);
			//batteryData.TargetSoC = 0.5;

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
				JobType = VectoSimulationJobType.ParallelHybridVehicle,
				DriverData = driverData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = Path.GetFileNameWithoutExtension(modFileName),
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				EngineData = engineData,
				BatteryData = batteryData,
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed),
				HybridStrategyParameters = CreateHybridStrategyData(maxDriveTrainPower),
				ElectricAuxDemand = pAuxEl.SI<Watt>()
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter)
			{
				WriteModalResults = true,
			};
			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); });
			container.RunData = runData;

			var strategy = gearboxType.AutomaticTransmission()
				? (IHybridControlStrategy) new HybridStrategyAT(runData, container)
				: new HybridStrategy(runData, container);
			var es = new ElectricSystem(container);
			var battery = new Battery(container, batteryData);
			battery.Initialize(initialBatCharge);

			var clutch = gearboxType.AutomaticTransmission() ? null :  new SwitchableClutch(container, runData.EngineData);
			var ctl = new HybridController(container, strategy, es);

			es.Connect(battery);

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			var gearbox = gearboxType.AutomaticTransmission()
				? (IHybridControlledGearbox)new ATGearbox(container, ctl.ShiftStrategy)
				: new Gearbox(container, ctl.ShiftStrategy);
			//var hybridStrategy = new DelegateParallelHybridStrategy();
			ctl.Gearbox = gearbox;

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
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent((IGearbox)gearbox, runData.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, runData);

			if (runData.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				if (gearbox is ATGearbox atGbx) {
					atGbx.IdleController = idleController;
					new ATClutchInfo(container);
				}
			}

			return container;
		}

		private static HybridStrategyParameters CreateHybridStrategyData(Watt maxDriveTrainPower)
		{
			return new HybridStrategyParameters() {
				EquivalenceFactorDischarge = 2.5,
				EquivalenceFactorCharge = 2.5,
				MinSoC = 0.22,
				MaxSoC = 0.8,
				TargetSoC = 0.5,
				AuxReserveTime = 5.SI<Second>(),
				AuxReserveChargeTime = 2.SI<Second>(),
				MinICEOnTime = 3.SI<Second>(), 
				//MaxDrivetrainPower = maxDriveTrainPower ?? 1e12.SI<Watt>(),
			};
		}


		public static VehicleContainer CreateConventionalPowerTrain(DrivingCycleData cycleData, string modFileName,
			SummaryDataContainer sumData, double pAuxEl)
		{
			//var strategySettings = GetHybridStrategyParameters(largeMotor);

			//strategySettings.StrategyName = "SimpleParallelHybridStrategy";

			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData(GearboxType.AMT);

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
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				EngineData = engineData,
				//BatteryData = batteryData,
				//HybridStrategy = strategySettings
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed)
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter)
			{
				WriteModalResults = true,
			};

			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); }) { RunData = runData };


			var clutch = new SwitchableClutch(container, runData.EngineData);

			var gbxStrategy = new AMTShiftStrategyOptimized(container);
			
			var gearbox = new Gearbox(container, gbxStrategy);

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
				TimeBetweenGearshifts = 2.SI<Second>(),
				TorqueReserve = 0.2,
				StartTorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),

				StartVelocity = DeclarationData.GearboxTCU.StartSpeed,
				//StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
				GearResidenceTime = DeclarationData.GearboxTCU.GearResidenceTime,
				DnT99L_highMin1 = DeclarationData.GearboxTCU.DnT99L_highMin1,
				DnT99L_highMin2 = DeclarationData.GearboxTCU.DnT99L_highMin2,
				AllowedGearRangeUp = gbx.Type.AutomaticTransmission() ? 1 : DeclarationData.GearboxTCU.AllowedGearRangeUp,
				AllowedGearRangeDown = gbx.Type.AutomaticTransmission() ? 1: DeclarationData.GearboxTCU.AllowedGearRangeDown,
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

		private static GearboxData CreateGearboxData(GearboxType gearboxType = GearboxType.NoGeabox)
		{
			switch (gearboxType) {
				
				case GearboxType.NoGeabox:
				case GearboxType.AMT:
					return CreateAMTGearbox();
				case GearboxType.ATSerial:
					return CreateATSerial();
				case GearboxType.ATPowerSplit:
					return CreateATPowerSplit();
				default:
					throw new ArgumentOutOfRangeException(nameof(gearboxType), gearboxType, null);
			}
		}

		private static GearboxData CreateAMTGearbox()
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
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
			};
		}

		private static GearboxData CreateATSerial()
		{
			//var shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(
			//		gearbox.Type, (int)i, engine.FullLoadCurves[i + 1],
			//		gearsInput, engine,
			//		axlegearRatio, dynamicTyreRadius, null);

			var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
			return new GearboxData {
				Type = GearboxType.ATSerial,
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									string.Format("Gear {0}", i)),
							Ratio = ratio,
							//ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile),
							TorqueConverterRatio = i == 0 ? ratio : double.NaN,
							TorqueConverterGearLossMap = i == 0
								? TransmissionLossMapReader.Create( 0.98, ratio, string.Format("Gear {0}", i))
								: null,
							//TorqueConverterShiftPolygon = i == 0 ? ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile) : null
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				TorqueConverterData = TorqueConverterDataReader.ReadFromFile(TorqueConverterSerial, 1000.RPMtoRad(),
					5000.RPMtoRad(), ExecutionMode.Engineering, 1.0, DeclarationData.Gearbox.UpshiftMinAcceleration,
					DeclarationData.Gearbox.UpshiftMinAcceleration), 
				PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime
		};
		}

		private static GearboxData CreateATPowerSplit()
		{
			var ratios = new[] { 1.35, 1.0, 0.73 };
			return new GearboxData {
				Type = GearboxType.ATPowerSplit,
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									string.Format("Gear {0}", i)),
							Ratio = ratio,
							//ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile),
							TorqueConverterRatio = i == 0 ? 1.0 : double.NaN,
							TorqueConverterGearLossMap = i == 0
								? TransmissionLossMapReader.Create(1.0, ratio, string.Format("Gear {0}", i))
								: null,
							//TorqueConverterShiftPolygon = i == 0 ? ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile) : null
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				TorqueConverterData = TorqueConverterDataReader.ReadFromFile(TorqueConverterSerial, 1000.RPMtoRad(),
					5000.RPMtoRad(), ExecutionMode.Engineering, 1.0 / ratios[0], DeclarationData.Gearbox.UpshiftMinAcceleration,
					DeclarationData.Gearbox.UpshiftMinAcceleration),
				PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime
			};
		}

		private static AxleGearData CreateAxleGearData(GearboxType gearboxType)
		{
			var ratio = 2.59;
			switch (gearboxType) {
				case GearboxType.ATSerial:
					ratio = 6.2;
					break;
				case GearboxType.ATPowerSplit:
					ratio = 5.8;
					break;
			}
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
				SavedInDeclarationMode = false,
				ADAS = new VehicleData.ADASData() {
					EngineStopStart = true
				}
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
					UtilityFactorStandstill = DeclarationData.Driver.EngineStopStart.UtilityFactor
				}
			};
		}
	}
}