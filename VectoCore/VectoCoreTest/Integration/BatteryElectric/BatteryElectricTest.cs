using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.ElectricSystem;


namespace TUGraz.VectoCore.Tests.Integration.BatteryElectric
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class BatteryElectricTest
	{



		//private ModalResultField[] Yfields;
		public const string MotorFile = @"TestData\BatteryElectric\GenericVehicleB4\GenericEMotor_125kW_485Nm.vem";
		public const string BatFile = @"TestData\BatteryElectric\GenericVehicleB4\GenericBattery_243kWh_750V.vbat";

		public const string AccelerationFile = @"TestData\Components\Truck.vacc";
		//public const string MotorFile240kW = @"TestData\Hybrids\ElectricMotor\GenericEMotor240kW.vem";

		public const string GearboxIndirectLoss = @"TestData\Components\Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData\Components\Direct Gear.vtlm";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		private GraphWriter GetGraphWriter(ModalResultField[] yFields)
		{
			var graphWriter = new GraphWriter();
			//#if TRACE
			graphWriter.Enable();

			//#else
			//GraphWriter.Disable();
			//#endif
			

			var Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.BatterySOC,
			}.Concat(yFields).ToArray();
			graphWriter.Xfields = new[] { ModalResultField.dist };
			graphWriter.Yfields = yFields;
			graphWriter.Series1Label = "BEV";
			graphWriter.PlotIgnitionState = true;

			return graphWriter;
		}


		[
			TestCase(30, 0.7, 0, 0, TestName = "B4 BEV ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "B4 BEV ConstantSpeed 50km/h SoC: 0.7, level"),
			//TestCase(80, 0.7, 0, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "B4 BEV ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "B4 BEV ConstantSpeed 50km/h SoC: 0.25, level"),
			//TestCase(80, 0.25, 0, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "B4 BEV ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "B4 BEV ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			//TestCase(80, 0.5, 5, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "B4 BEV ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "B4 BEV ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			//TestCase(80, 0.5, -5, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "B4 BEV ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "B4 BEV ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		] // the vehicle can drive max. 56km/h!! 80km/h testcase makes no sense
		public void B4BEVConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleBatteryElectric-B4_constant_{0}-{1}_{2}_{3}", vmax, initialSoC, slope, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricB4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}


		[
			TestCase(30, 0.7, 0, TestName = "B4 BEV DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "B4 BEV DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.25, 0, TestName = "B4 BEV DriveOff 30km/h SoC: 0.25, level")
		]
		public void B4BEVDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleBatteryElectric-B4_acc_{0}-{1}_{2}", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricB4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase("LongHaul", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle LongHaul, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle RegionalDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle UrbanDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle Construction, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle Urban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle SubUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle InterUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.8, 0, TestName = "B4 BEV DriveCycle Coach, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
		]
		public void B4BEVDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-B4_cycle_{0}-{1}_{2}_{3}", declarationMission, initialSoC, payload, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricB4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}

		// - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - -  - - - 

		[
			TestCase(30, 0.7, 0, 0, TestName = "B3 BEV ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "B3 BEV ConstantSpeed 50km/h SoC: 0.7, level"),
			//TestCase(80, 0.7, 0, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "B3 BEV ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "B3 BEV ConstantSpeed 50km/h SoC: 0.25, level"),
			//TestCase(80, 0.25, 0, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "B3 BEV ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "B3 BEV ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			//TestCase(80, 0.5, 5, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "B3 BEV ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "B3 BEV ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			//TestCase(80, 0.5, -5, 0, TestName = "B4 BEV ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "B3 BEV ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "B3 BEV ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		] // the vehicle can drive max. 56km/h!! 80km/h testcase makes no sense
		public void B3BEVConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleBatteryElectric-B3_constant_{0}-{1}_{2}_{3}", vmax, initialSoC, slope, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricB3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}


		[
			TestCase(30, 0.7, 0, TestName = "B3 BEV DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "B3 BEV DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.25, 0, TestName = "B3 BEV DriveOff 30km/h SoC: 0.25, level")
		]
		public void B3BEVDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleBatteryElectric-B3_acc_{0}-{1}_{2}", vmax, initialSoC, slope);
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricB3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase("LongHaul", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle LongHaul, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle RegionalDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle UrbanDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle Construction, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle Urban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle SubUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle InterUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.8, 0, TestName = "B3 BEV DriveCycle Coach, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
		]
		public void B3BEVDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = string.Format("SimpleParallelHybrid-B3_cycle_{0}-{1}_{2}_{3}", declarationMission, initialSoC, payload, pAuxEl);
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricB3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);

			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}


		// =================================================


		public static JobContainer CreateEngineeringRun(
			DrivingCycleData cycleData, string modFileName, double initialSoc, PowertrainPosition pos, int count, double ratio, bool largeMotor = false, double pAuxEl = 0, Kilogram payload = null)
		{
			var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(modFileName));
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var container = CreateBatteryElectricPowerTrain(
				cycleData, modFileName, fileWriter, sumData, initialSoc, count, ratio, largeMotor,  pAuxEl, pos, payload);
			
			var run = new DistanceRun(container);
			jobContainer.AddRun(run);
			return jobContainer;
		}

		public static VehicleContainer CreateBatteryElectricPowerTrain(DrivingCycleData cycleData,
			string modFileName, FileOutputWriter fileWriter, SummaryDataContainer sumData,
			double initialBatCharge, int count, double ratio, bool largeMotor, double pAuxEl, PowertrainPosition pos,
			Kilogram payload = null)
		{
			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(payload ?? 3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var electricMotorData =
				MockSimulationDataFactory.CreateElectricMotorData(MotorFile, count, pos, ratio / (pos == PowertrainPosition.BatteryElectricB3 ? 2.59 : 1.0), 0.97);

			var batteryData = MockSimulationDataFactory.CreateBatteryData(BatFile, initialBatCharge);
			
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1).Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var runData = new VectoRunData() {
				JobRunId = 0,
				JobType = VectoSimulationJobType.BatteryElectricVehicle,
				DriverData = driverData,
				//AxleGearData = axleGearData,
				//GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = modFileName,
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				//EngineData = engineData,
				BatteryData = batteryData,
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed),
				ElectricAuxDemand = pAuxEl.SI<Watt>()
			};

			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter)
			{
				WriteModalResults = true,
			};
			if (pos == PowertrainPosition.BatteryElectricB3) {
				runData.AxleGearData = axleGearData;
			}

			if (pos == PowertrainPosition.BatteryElectricB2) {
				runData.AxleGearData = axleGearData;
				runData.GearboxData = gearboxData;
			}
			
			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); });
			
			container.RunData = runData;
			
			var es = new ElectricSystem(container);
			var battery = new Battery(container, batteryData);
			battery.Initialize(initialBatCharge);

			var ctl = new BatteryElectricMotorController(container, es);

			es.Connect(battery);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			var powertrain = cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new VectoCore.Models.SimulationComponent.Impl.Wheels(container, runData.VehicleData.DynamicTyreRadius,
					runData.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			switch (pos) {
				case PowertrainPosition.HybridPositionNotSet:
					throw new VectoException("invalid powertrain position");
				case PowertrainPosition.HybridP0: 
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2: 
				case PowertrainPosition.HybridP3: 
				case PowertrainPosition.HybridP4: 
					throw new VectoException("testcase does not support parallel powertrain configurations");
				case PowertrainPosition.BatteryElectricB4:
					powertrain.AddComponent(
						GetElectricMachine(PowertrainPosition.BatteryElectricB4, runData.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container);
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricB3:
					powertrain.AddComponent(new AxleGear(container, runData.AxleGearData))
							.AddComponent(
								GetElectricMachine(PowertrainPosition.BatteryElectricB3, runData.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container);
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricB2:
					throw new VectoException("Battery Electric configuration B2 currently not supported");
				default: throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
			}

			return container;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, VehicleContainer container,
			IElectricSystem es, IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			container.ModData.AddElectricMotor(pos);
			//ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl, pos);
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
			var wheels = DeclarationData.Wheels.Lookup("275/70 R22.5");
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = wheels.DynamicTyreRadius,
				AxleData = axles,
				SavedInDeclarationMode = false,
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
				},
			};
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
	}

	public class MockEngineInfo : VectoSimulationComponent, IEngineInfo
	{
		public MockEngineInfo(VehicleContainer container) : base(container)
		{
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{ }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{ }

		public PerSecond EngineSpeed
		{
			get;
			set;
		}
		public NewtonMeter EngineTorque
		{
			get { return null; }
		}
		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			throw new NotImplementedException();
		}

		public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			throw new NotImplementedException();
		}

		public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public PerSecond EngineIdleSpeed { get; }
		public PerSecond EngineRatedSpeed { get; }
		public PerSecond EngineN95hSpeed { get; }
		public PerSecond EngineN80hSpeed { get; }
		public bool EngineOn
		{
			get { return true; }
		}
	}

	
}
