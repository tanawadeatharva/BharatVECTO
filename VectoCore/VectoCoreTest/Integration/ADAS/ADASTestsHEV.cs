using System.IO;
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.PCCStates;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DrivingAction;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	/// <summary>
	/// The engineering ADAS Test generate their segments in the output console.
	/// You can copy and adapt it from there for easier test creation.
	/// </summary>
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsHEV
	{
		private const string BasePath = @"TestData\Integration\ADAS-HEV\Group5PCCEng\";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

		[OneTimeSetUp]
		public void RunBeforeAnyTests() => Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		private GraphWriter GetGraphWriter()
		{
			var graphWriter = new GraphWriter();
			//#if TRACE
			graphWriter.Enable();
			//#else
			//graphWriter.Disable();
			//#endif
			graphWriter.Xfields = new[] { ModalResultField.dist };

			graphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.P_ice_out, ModalResultField.FCMap
			};
			graphWriter.Series1Label = "ADAS PCC";
			graphWriter.PlotIgnitionState = true;
			return graphWriter;
		}

		[TestCase]
		public void TestVECTO_1484()
		{
			var jobName = @"TestData\Integration\ADAS-HEV\VECTO-1484\P2_Group5_s2c0_rep_Payload.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				Validate = false,
				SumData = sumContainer
			};
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
		}


		[
		TestCase(0, TestName = "EcoRoll DH1.1 const"),
		TestCase(1, TestName = "EcoRoll DH1.1 UH0.1"),
		TestCase(2, TestName = "EcoRoll DH1.4 const"),
		TestCase(3, TestName = "EcoRoll DH0.8 const - too flat"),
		TestCase(4, TestName = "EcoRoll DH1.7 const - too steep"),
		TestCase(5, TestName = "EcoRoll DH1.1 const - Stop"),
		TestCase(6, TestName = "EcoRoll DH1.1 const - TS60"),
		TestCase(7, TestName = "EcoRoll DH1.1 const - TS68"),
		TestCase(8, TestName = "EcoRoll DH1.1 const - TS72"),
		TestCase(9, TestName = "EcoRoll DH1.1 const - TS80"),
		]
		public void TestEcoRoll(int cycleIdx)
		{
			var jobName = @"TestData\Integration\ADAS-HEV\Group5EcoRollEng\Class5_Tractor_ENG.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				Validate = false,
				SumData = sumContainer
			};


			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GetGraphWriter().Write(modFilename);
		}

		[TestCase(0, TestName = "AT EcoRoll Neutral DH1.8 const"),
		TestCase(1, TestName = "AT EcoRoll Neutral DH1.8 UH0.1"),
		TestCase(2, TestName = "AT EcoRoll Neutral DH1.9 const"),
		TestCase(3, TestName = "AT EcoRoll Neutral DH1.2 const - too flat"),
		TestCase(4, TestName = "AT EcoRoll Neutral DH2.5 const - too steep"),
		TestCase(5, TestName = "AT EcoRoll Neutral DH1.9 const - Stop"),
		TestCase(6, TestName = "AT EcoRoll Neutral DH1.9 const - TS60"),
		TestCase(7, TestName = "AT EcoRoll Neutral DH1.9 const - TS68"),
		TestCase(8, TestName = "AT EcoRoll Neutral DH1.9 const - TS72"),
		TestCase(9, TestName = "AT EcoRoll Neutral DH1.9 const - TS80"),
		TestCase(10, TestName = "AT EcoRoll Neutral DH1.2 const"),
		]
		public void TestEcoRollAT_Neutral(int cycleIdx)
		{
			var jobName = @"TestData\Integration\ADAS-HEV\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_Neutral.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				Validate = false,
				SumData = sumContainer
			};


			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GetGraphWriter().Write(modFilename);
		}


		[TestCase(0, TestName = "AT EcoRoll TC DH1.8 const"),
		TestCase(1, TestName = "AT EcoRoll TC DH1.8 UH0.1"),
		TestCase(2, TestName = "AT EcoRoll TC DH1.9 const"),
		TestCase(3, TestName = "AT EcoRoll TC DH1.2 const - too flat"),
		TestCase(4, TestName = "AT EcoRoll TC DH2.5 const - too steep"),
		TestCase(5, TestName = "AT EcoRoll TC DH1.9 const - Stop"),
		TestCase(6, TestName = "AT EcoRoll TC DH1.9 const - TS60"),
		TestCase(7, TestName = "AT EcoRoll TC DH1.9 const - TS68"),
		TestCase(8, TestName = "AT EcoRoll TC DH1.9 const - TS72"),
		TestCase(9, TestName = "AT EcoRoll TC DH1.9 const - TS80"),
		]
		public void TestEcoRollAT_TC(int cycleIdx)
		{
			var jobName = @"TestData\Integration\ADAS-HEV\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_TC.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				Validate = false,
				SumData = sumContainer
			};


			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GetGraphWriter().Write(modFilename);
		}

		[TestCase]
		public void Class5_PCC123_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6872, WithinSegment, Coast),           // len: 411m
			(6872, 7256, WithinSegment, Brake),           // len: 384m
			(7256, 7466, OutsideSegment, Brake),          // len: 210m
			(7466, 8025, OutsideSegment, Coast),          // len: 559m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 6047, WithinSegment, Coast),           // len: 169m
			(6047, 7160, WithinSegment, Brake),           // len: 1113m
			(7160, 7234, OutsideSegment, Brake),          // len: 74m
			(7234, 7658, OutsideSegment, Coast),          // len: 424m
			(7658, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2378, UseCase1, Coast),                // len: 476m
			(2378, 2486, WithinSegment, Coast),           // len: 108m
			(2486, 2629, OutsideSegment, Coast),          // len: 143m
			(2629, 4041, OutsideSegment, Accelerate),     // len: 1412m
			(4041, 4951, WithinSegment, Accelerate),      // len: 910m
			(4951, 5939, UseCase1, Coast),                // len: 988m
			(5939, 6145, WithinSegment, Coast),           // len: 206m
			(6145, 6689, WithinSegment, Brake),           // len: 544m
			(6689, 6763, OutsideSegment, Brake),          // len: 74m
			(6763, 7152, OutsideSegment, Coast),          // len: 389m
			(7152, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2345, UseCase1, Coast),                // len: 268m
			(2345, 2499, WithinSegment, Coast),           // len: 154m
			(2499, 2989, WithinSegment, Accelerate),      // len: 490m
			(2989, 3694, UseCase1, Coast),                // len: 705m
			(3694, 3900, WithinSegment, Coast),           // len: 206m
			(3900, 3974, WithinSegment, Brake),           // len: 74m
			(3974, 4023, OutsideSegment, Brake),          // len: 49m
			(4023, 4435, OutsideSegment, Coast),          // len: 412m
			(4435, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2605, WithinSegment, Coast),           // len: 259m
			(2605, 2652, WithinSegment, Accelerate),      // len: 47m
			(2652, 3093, UseCase1, Coast),                // len: 441m
			(3093, 3272, WithinSegment, Coast),           // len: 179m
			(3272, 3571, OutsideSegment, Coast),          // len: 299m
			(3571, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5922, WithinSegment, Coast),           // len: 194m
			(5922, 6564, WithinSegment, Brake),           // len: 642m
			(6564, 6787, OutsideSegment, Brake),          // len: 223m
			(6787, 7539, OutsideSegment, Coast),          // len: 752m
			(7539, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5988, WithinSegment, Coast),           // len: 327m
			(5988, 6401, OutsideSegment, Coast),          // len: 413m
			(6401, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 913m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4709, WithinSegment, Accelerate),      // len: 127m
			(4709, 4723, WithinSegment, Roll),            // len: 14m
			(4723, 4789, WithinSegment, Accelerate),      // len: 66m
			(4789, 4800, WithinSegment, Roll),            // len: 11m
			(4800, 4866, WithinSegment, Accelerate),      // len: 66m
			(4866, 4875, WithinSegment, Roll),            // len: 9m
			(4875, 4931, WithinSegment, Accelerate),      // len: 56m
			(4931, 4940, WithinSegment, Roll),            // len: 9m
			(4940, 5028, WithinSegment, Accelerate),      // len: 88m
			(5028, 5040, WithinSegment, Roll),            // len: 12m
			(5040, 5079, WithinSegment, Accelerate),      // len: 39m
			(5079, 5093, WithinSegment, Roll),            // len: 14m
			(5093, 5131, WithinSegment, Accelerate),      // len: 38m
			(5131, 5147, WithinSegment, Roll),            // len: 16m
			(5147, 5343, WithinSegment, Accelerate),      // len: 196m
			(5343, 5614, UseCase2, Coast),                // len: 271m
			(5614, 5771, WithinSegment, Coast),           // len: 157m
			(5771, 6117, WithinSegment, Brake),           // len: 346m
			(6117, 6191, OutsideSegment, Brake),          // len: 74m
			(6191, 6591, OutsideSegment, Coast),          // len: 400m
			(6591, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC123_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6604, WithinSegment, Coast),           // len: 143m
			(6604, 7253, WithinSegment, Brake),           // len: 649m
			(7253, 7481, OutsideSegment, Brake),          // len: 228m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 5938, WithinSegment, Coast),           // len: 60m
			(5938, 7151, WithinSegment, Brake),           // len: 1213m
			(7151, 7247, OutsideSegment, Brake),          // len: 96m
			(7247, 7510, OutsideSegment, Coast),          // len: 263m
			(7510, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2378, UseCase1, Coast),                // len: 476m
			(2378, 2437, WithinSegment, Coast),           // len: 59m
			(2437, 2485, WithinSegment, Brake),           // len: 48m
			(2485, 2497, OutsideSegment, Brake),          // len: 12m
			(2497, 2616, OutsideSegment, Coast),          // len: 119m
			(2616, 4040, OutsideSegment, Accelerate),     // len: 1424m
			(4040, 4950, WithinSegment, Accelerate),      // len: 910m
			(4950, 5938, UseCase1, Coast),                // len: 988m
			(5938, 6010, WithinSegment, Coast),           // len: 72m
			(6010, 6695, WithinSegment, Brake),           // len: 685m
			(6695, 6779, OutsideSegment, Brake),          // len: 84m
			(6779, 7029, OutsideSegment, Coast),          // len: 250m
			(7029, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2345, UseCase1, Coast),                // len: 268m
			(2345, 2511, WithinSegment, Coast),           // len: 166m
			(2511, 2989, WithinSegment, Accelerate),      // len: 478m
			(2989, 3694, UseCase1, Coast),                // len: 705m
			(3694, 3741, WithinSegment, Coast),           // len: 47m
			(3741, 3970, WithinSegment, Brake),           // len: 229m
			(3970, 4030, OutsideSegment, Brake),          // len: 60m
			(4030, 4303, OutsideSegment, Coast),          // len: 273m
			(4303, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2605, WithinSegment, Coast),           // len: 259m
			(2605, 2652, WithinSegment, Accelerate),      // len: 47m
			(2652, 3093, UseCase1, Coast),                // len: 441m
			(3093, 3260, WithinSegment, Coast),           // len: 167m
			(3260, 3272, WithinSegment, Brake),           // len: 12m
			(3272, 3332, OutsideSegment, Brake),          // len: 60m
			(3332, 3558, OutsideSegment, Coast),          // len: 226m
			(3558, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5787, WithinSegment, Coast),           // len: 59m
			(5787, 6556, WithinSegment, Brake),           // len: 769m
			(6556, 6821, OutsideSegment, Brake),          // len: 265m
			(6821, 7344, OutsideSegment, Coast),          // len: 523m
			(7344, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5757, WithinSegment, Coast),           // len: 96m
			(5757, 5997, WithinSegment, Brake),           // len: 240m
			(5997, 6069, OutsideSegment, Brake),          // len: 72m
			(6069, 6307, OutsideSegment, Coast),          // len: 238m
			(6307, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 913m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4709, WithinSegment, Accelerate),      // len: 127m
			(4709, 4723, WithinSegment, Roll),            // len: 14m
			(4723, 4789, WithinSegment, Accelerate),      // len: 66m
			(4789, 4800, WithinSegment, Roll),            // len: 11m
			(4800, 4866, WithinSegment, Accelerate),      // len: 66m
			(4866, 4875, WithinSegment, Roll),            // len: 9m
			(4875, 4931, WithinSegment, Accelerate),      // len: 56m
			(4931, 4940, WithinSegment, Roll),            // len: 9m
			(4940, 5028, WithinSegment, Accelerate),      // len: 88m
			(5028, 5040, WithinSegment, Roll),            // len: 12m
			(5040, 5079, WithinSegment, Accelerate),      // len: 39m
			(5079, 5093, WithinSegment, Roll),            // len: 14m
			(5093, 5131, WithinSegment, Accelerate),      // len: 38m
			(5131, 5147, WithinSegment, Roll),            // len: 16m
			(5147, 5343, WithinSegment, Accelerate),      // len: 196m
			(5343, 5614, UseCase2, Coast),                // len: 271m
			(5614, 5661, WithinSegment, Coast),           // len: 47m
			(5661, 6118, WithinSegment, Brake),           // len: 457m
			(6118, 6202, OutsideSegment, Brake),          // len: 84m
			(6202, 6464, OutsideSegment, Coast),          // len: 262m
			(6464, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5531, OutsideSegment, Accelerate),        // len: 5531m
			(5531, 6062, OutsideSegment, Coast),          // len: 531m
			(6062, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5892, OutsideSegment, Accelerate),        // len: 5892m
			(5892, 6387, OutsideSegment, Coast),          // len: 495m
			(6387, 7481, OutsideSegment, Brake),          // len: 1094m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5367, OutsideSegment, Accelerate),        // len: 5367m
			(5367, 5685, OutsideSegment, Coast),          // len: 318m
			(5685, 7247, OutsideSegment, Brake),          // len: 1562m
			(7247, 7509, OutsideSegment, Coast),          // len: 262m
			(7509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2124, OutsideSegment, Accelerate),        // len: 2124m
			(2124, 2265, OutsideSegment, Coast),          // len: 141m
			(2265, 2494, OutsideSegment, Brake),          // len: 229m
			(2494, 2613, OutsideSegment, Coast),          // len: 119m
			(2613, 5424, OutsideSegment, Accelerate),     // len: 2811m
			(5424, 5742, OutsideSegment, Coast),          // len: 318m
			(5742, 6776, OutsideSegment, Brake),          // len: 1034m
			(6776, 7026, OutsideSegment, Coast),          // len: 250m
			(7026, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2525, OutsideSegment, Coast),          // len: 366m
			(2525, 3284, OutsideSegment, Accelerate),     // len: 759m
			(3284, 3508, OutsideSegment, Coast),          // len: 224m
			(3508, 4024, OutsideSegment, Brake),          // len: 516m
			(4024, 4286, OutsideSegment, Coast),          // len: 262m
			(4286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2643, OutsideSegment, Coast),          // len: 484m
			(2643, 2818, OutsideSegment, Accelerate),     // len: 175m
			(2818, 3137, OutsideSegment, Coast),          // len: 319m
			(3137, 3329, OutsideSegment, Brake),          // len: 192m
			(3329, 3567, OutsideSegment, Coast),          // len: 238m
			(3567, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 5414, OutsideSegment, Accelerate),        // len: 5414m
			(5414, 5567, OutsideSegment, Coast),          // len: 153m
			(5567, 6817, OutsideSegment, Brake),          // len: 1250m
			(6817, 7352, OutsideSegment, Coast),          // len: 535m
			(7352, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 5227, OutsideSegment, Accelerate),        // len: 5227m
			(5227, 5522, OutsideSegment, Coast),          // len: 295m
			(5522, 6063, OutsideSegment, Brake),          // len: 541m
			(6063, 6289, OutsideSegment, Coast),          // len: 226m
			(6289, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 4563, OutsideSegment, Accelerate),        // len: 4472m
			(4563, 4582, OutsideSegment, Roll),           // len: 19m
			(4582, 4709, OutsideSegment, Accelerate),     // len: 127m
			(4709, 4723, OutsideSegment, Roll),           // len: 14m
			(4723, 4789, OutsideSegment, Accelerate),     // len: 66m
			(4789, 4800, OutsideSegment, Roll),           // len: 11m
			(4800, 4866, OutsideSegment, Accelerate),     // len: 66m
			(4866, 4875, OutsideSegment, Roll),           // len: 9m
			(4875, 4931, OutsideSegment, Accelerate),     // len: 56m
			(4931, 4940, OutsideSegment, Roll),           // len: 9m
			(4940, 5028, OutsideSegment, Accelerate),     // len: 88m
			(5028, 5040, OutsideSegment, Roll),           // len: 12m
			(5040, 5079, OutsideSegment, Accelerate),     // len: 39m
			(5079, 5093, OutsideSegment, Roll),           // len: 14m
			(5093, 5131, OutsideSegment, Accelerate),     // len: 38m
			(5131, 5147, OutsideSegment, Roll),           // len: 16m
			(5147, 5444, OutsideSegment, Accelerate),     // len: 297m
			(5444, 5539, OutsideSegment, Coast),          // len: 95m
			(5539, 6116, OutsideSegment, Brake),          // len: 577m
			(6116, 6401, OutsideSegment, Coast),          // len: 285m
			(6401, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6872, WithinSegment, Coast),           // len: 411m
			(6872, 7256, WithinSegment, Brake),           // len: 384m
			(7256, 7466, OutsideSegment, Brake),          // len: 210m
			(7466, 8025, OutsideSegment, Coast),          // len: 559m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 6047, WithinSegment, Coast),           // len: 169m
			(6047, 7160, WithinSegment, Brake),           // len: 1113m
			(7160, 7234, OutsideSegment, Brake),          // len: 74m
			(7234, 7647, OutsideSegment, Coast),          // len: 413m
			(7647, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2378, UseCase1, Coast),                // len: 476m
			(2378, 2486, WithinSegment, Coast),           // len: 108m
			(2486, 2629, OutsideSegment, Coast),          // len: 143m
			(2629, 4041, OutsideSegment, Accelerate),     // len: 1412m
			(4041, 4951, WithinSegment, Accelerate),      // len: 910m
			(4951, 5939, UseCase1, Coast),                // len: 988m
			(5939, 6145, WithinSegment, Coast),           // len: 206m
			(6145, 6689, WithinSegment, Brake),           // len: 544m
			(6689, 6763, OutsideSegment, Brake),          // len: 74m
			(6763, 7152, OutsideSegment, Coast),          // len: 389m
			(7152, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2345, UseCase1, Coast),                // len: 268m
			(2345, 2499, WithinSegment, Coast),           // len: 154m
			(2499, 2989, WithinSegment, Accelerate),      // len: 490m
			(2989, 3694, UseCase1, Coast),                // len: 705m
			(3694, 3900, WithinSegment, Coast),           // len: 206m
			(3900, 3974, WithinSegment, Brake),           // len: 74m
			(3974, 4023, OutsideSegment, Brake),          // len: 49m
			(4023, 4435, OutsideSegment, Coast),          // len: 412m
			(4435, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2605, WithinSegment, Coast),           // len: 259m
			(2605, 2640, WithinSegment, Accelerate),      // len: 35m
			(2640, 3104, UseCase1, Coast),                // len: 464m
			(3104, 3271, WithinSegment, Coast),           // len: 167m
			(3271, 3569, OutsideSegment, Coast),          // len: 298m
			(3569, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5922, WithinSegment, Coast),           // len: 194m
			(5922, 6564, WithinSegment, Brake),           // len: 642m
			(6564, 6787, OutsideSegment, Brake),          // len: 223m
			(6787, 7539, OutsideSegment, Coast),          // len: 752m
			(7539, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5988, WithinSegment, Coast),           // len: 327m
			(5988, 6401, OutsideSegment, Coast),          // len: 413m
			(6401, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 913m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4708, WithinSegment, Accelerate),      // len: 126m
			(4708, 4722, WithinSegment, Roll),            // len: 14m
			(4722, 4774, WithinSegment, Accelerate),      // len: 52m
			(4774, 4785, WithinSegment, Roll),            // len: 11m
			(4785, 4826, WithinSegment, Accelerate),      // len: 41m
			(4826, 4834, WithinSegment, Roll),            // len: 8m
			(4834, 4873, WithinSegment, Accelerate),      // len: 39m
			(4873, 4880, WithinSegment, Roll),            // len: 7m
			(4880, 4923, WithinSegment, Accelerate),      // len: 43m
			(4923, 4929, WithinSegment, Roll),            // len: 6m
			(4929, 4945, WithinSegment, Accelerate),      // len: 16m
			(4945, 4951, WithinSegment, Roll),            // len: 6m
			(4951, 4999, WithinSegment, Accelerate),      // len: 48m
			(4999, 5008, WithinSegment, Roll),            // len: 9m
			(5008, 5050, WithinSegment, Accelerate),      // len: 42m
			(5050, 5062, WithinSegment, Roll),            // len: 12m
			(5062, 5094, WithinSegment, Accelerate),      // len: 32m
			(5094, 5107, WithinSegment, Roll),            // len: 13m
			(5107, 5144, WithinSegment, Accelerate),      // len: 37m
			(5144, 5159, WithinSegment, Roll),            // len: 15m
			(5159, 5373, WithinSegment, Accelerate),      // len: 214m
			(5373, 5632, UseCase2, Coast),                // len: 259m
			(5632, 5801, WithinSegment, Coast),           // len: 169m
			(5801, 6123, WithinSegment, Brake),           // len: 322m
			(6123, 6185, OutsideSegment, Brake),          // len: 62m
			(6185, 6596, OutsideSegment, Coast),          // len: 411m
			(6596, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 19, OutsideSegment, Roll),               // len: 4m
			//(19, 37, OutsideSegment, Accelerate),         // len: 18m
			//(37, 44, OutsideSegment, Roll),               // len: 7m
			//(44, 88, OutsideSegment, Accelerate),         // len: 44m
			//(88, 98, OutsideSegment, Roll),               // len: 10m
			//(98, 164, OutsideSegment, Accelerate),        // len: 66m
			//(164, 178, OutsideSegment, Roll),             // len: 14m
			//(178, 260, OutsideSegment, Accelerate),       // len: 82m
			//(260, 276, OutsideSegment, Roll),             // len: 16m
			//(276, 427, OutsideSegment, Accelerate),       // len: 151m
			//(427, 446, OutsideSegment, Roll),             // len: 19m
			(500, 3411, OutsideSegment, Accelerate),      // len: 2965m
			(3411, 3876, OutsideSegment, Coast),          // len: 465m
			(3876, 5205, OutsideSegment, Brake),          // len: 1329m
			(5205, 5515, OutsideSegment, Coast),          // len: 310m
			(5515, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 19, OutsideSegment, Roll),               // len: 4m
			//(19, 37, OutsideSegment, Accelerate),         // len: 18m
			//(37, 44, OutsideSegment, Roll),               // len: 7m
			//(44, 88, OutsideSegment, Accelerate),         // len: 44m
			//(88, 98, OutsideSegment, Roll),               // len: 10m
			//(98, 164, OutsideSegment, Accelerate),        // len: 66m
			//(164, 178, OutsideSegment, Roll),             // len: 14m
			//(178, 260, OutsideSegment, Accelerate),       // len: 82m
			//(260, 276, OutsideSegment, Roll),             // len: 16m
			//(276, 427, OutsideSegment, Accelerate),       // len: 151m
			//(427, 446, OutsideSegment, Roll),             // len: 19m
			(500, 3421, OutsideSegment, Accelerate),      // len: 2975m
			(3421, 3844, OutsideSegment, Coast),          // len: 423m
			(3844, 4508, OutsideSegment, Brake),          // len: 664m
			(4508, 4777, OutsideSegment, Coast),          // len: 269m
			(4777, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5461, WithinSegment, Accelerate),      // len: 1331m
			(5461, 5765, UseCase1, Coast),                // len: 304m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6049, OutsideSegment, Coast),          // len: 237m
			(6049, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5472, WithinSegment, Accelerate),      // len: 817m
			(5472, 6459, UseCase1, Coast),                // len: 987m
			(6459, 6870, WithinSegment, Coast),           // len: 411m
			(6870, 7254, WithinSegment, Brake),           // len: 384m
			(7254, 7476, OutsideSegment, Brake),          // len: 222m
			(7476, 8047, OutsideSegment, Coast),          // len: 571m
			(8047, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5868, UseCase1, Coast),                // len: 932m
			(5868, 6037, WithinSegment, Coast),           // len: 169m
			(6037, 7150, WithinSegment, Brake),           // len: 1113m
			(7150, 7236, OutsideSegment, Brake),          // len: 86m
			(7236, 7649, OutsideSegment, Coast),          // len: 413m
			(7649, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1891, WithinSegment, Accelerate),       // len: 1237m
			(1891, 2376, UseCase1, Coast),                // len: 485m
			(2376, 2484, WithinSegment, Coast),           // len: 108m
			(2484, 2639, OutsideSegment, Coast),          // len: 155m
			(2639, 4039, OutsideSegment, Accelerate),     // len: 1400m
			(4039, 4949, WithinSegment, Accelerate),      // len: 910m
			(4949, 5927, UseCase1, Coast),                // len: 978m
			(5927, 6133, WithinSegment, Coast),           // len: 206m
			(6133, 6689, WithinSegment, Brake),           // len: 556m
			(6689, 6775, OutsideSegment, Brake),          // len: 86m
			(6775, 7163, OutsideSegment, Coast),          // len: 388m
			(7163, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2345, UseCase1, Coast),                // len: 268m
			(2345, 2511, WithinSegment, Coast),           // len: 166m
			(2511, 2989, WithinSegment, Accelerate),      // len: 478m
			(2989, 3683, UseCase1, Coast),                // len: 694m
			(3683, 3889, WithinSegment, Coast),           // len: 206m
			(3889, 3976, WithinSegment, Brake),           // len: 87m
			(3976, 4025, OutsideSegment, Brake),          // len: 49m
			(4025, 4449, OutsideSegment, Coast),          // len: 424m
			(4449, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2357, UseCase1, Coast),                // len: 280m
			(2357, 2581, WithinSegment, Coast),           // len: 224m
			(2581, 2628, WithinSegment, Accelerate),      // len: 47m
			(2628, 3103, UseCase1, Coast),                // len: 475m
			(3103, 3270, WithinSegment, Coast),           // len: 167m
			(3270, 3568, OutsideSegment, Coast),          // len: 298m
			(3568, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5910, WithinSegment, Coast),           // len: 182m
			(5910, 6565, WithinSegment, Brake),           // len: 655m
			(6565, 6800, OutsideSegment, Brake),          // len: 235m
			(6800, 7564, OutsideSegment, Coast),          // len: 764m
			(7564, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4842, WithinSegment, Accelerate),      // len: 1027m
			(4842, 5660, UseCase1, Coast),                // len: 818m
			(5660, 6000, WithinSegment, Coast),           // len: 340m
			(6000, 6413, OutsideSegment, Coast),          // len: 413m
			(6413, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4505, WithinSegment, Accelerate),      // len: 854m
			(4505, 4524, WithinSegment, Roll),            // len: 19m
			(4524, 4641, WithinSegment, Accelerate),      // len: 117m
			(4641, 4656, WithinSegment, Roll),            // len: 15m
			(4656, 4709, WithinSegment, Accelerate),      // len: 53m
			(4709, 4720, WithinSegment, Roll),            // len: 11m
			(4720, 4761, WithinSegment, Accelerate),      // len: 41m
			(4761, 4770, WithinSegment, Roll),            // len: 9m
			(4770, 4801, WithinSegment, Accelerate),      // len: 31m
			(4801, 4808, WithinSegment, Roll),            // len: 7m
			(4808, 4840, WithinSegment, Accelerate),      // len: 32m
			(4840, 4845, WithinSegment, Roll),            // len: 5m
			(4845, 4879, WithinSegment, Accelerate),      // len: 34m
			(4879, 4884, WithinSegment, Roll),            // len: 5m
			(4884, 4911, WithinSegment, Accelerate),      // len: 27m
			(4911, 4914, WithinSegment, Roll),            // len: 3m
			(4914, 4949, WithinSegment, Accelerate),      // len: 35m
			(4949, 4954, WithinSegment, Roll),            // len: 5m
			(4954, 4986, WithinSegment, Accelerate),      // len: 32m
			(4986, 4993, WithinSegment, Roll),            // len: 7m
			(4993, 5028, WithinSegment, Accelerate),      // len: 35m
			(5028, 5038, WithinSegment, Roll),            // len: 10m
			(5038, 5074, WithinSegment, Accelerate),      // len: 36m
			(5074, 5086, WithinSegment, Roll),            // len: 12m
			(5086, 5118, WithinSegment, Accelerate),      // len: 32m
			(5118, 5132, WithinSegment, Roll),            // len: 14m
			(5132, 5169, WithinSegment, Accelerate),      // len: 37m
			(5169, 5184, WithinSegment, Roll),            // len: 15m
			(5184, 5397, WithinSegment, Accelerate),      // len: 213m
			(5397, 5668, UseCase2, Coast),                // len: 271m
			(5668, 5837, WithinSegment, Coast),           // len: 169m
			(5837, 6122, WithinSegment, Brake),           // len: 285m
			(6122, 6196, OutsideSegment, Brake),          // len: 74m
			(6196, 6596, OutsideSegment, Coast),          // len: 400m
			(6596, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 19, OutsideSegment, Roll),               // len: 4m
			//(19, 34, OutsideSegment, Accelerate),         // len: 15m
			//(34, 40, OutsideSegment, Roll),               // len: 6m
			//(40, 59, OutsideSegment, Accelerate),         // len: 19m
			//(59, 68, OutsideSegment, Roll),               // len: 9m
			//(68, 101, OutsideSegment, Accelerate),        // len: 33m
			//(101, 112, OutsideSegment, Roll),             // len: 11m
			//(112, 185, OutsideSegment, Accelerate),       // len: 73m
			//(185, 198, OutsideSegment, Roll),             // len: 13m
			//(198, 346, OutsideSegment, Accelerate),       // len: 148m
			//(346, 363, OutsideSegment, Roll),             // len: 17m
			//(363, 492, OutsideSegment, Accelerate),       // len: 129m
			//(492, 511, OutsideSegment, Roll),             // len: 19m
			(511, 3409, OutsideSegment, Accelerate),      // len: 2898m
			(3409, 3854, OutsideSegment, Coast),          // len: 445m
			(3854, 5213, OutsideSegment, Brake),          // len: 1359m
			(5213, 5513, OutsideSegment, Coast),          // len: 300m
			(5513, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 19, OutsideSegment, Roll),               // len: 4m
			//(19, 34, OutsideSegment, Accelerate),         // len: 15m
			//(34, 40, OutsideSegment, Roll),               // len: 6m
			//(40, 59, OutsideSegment, Accelerate),         // len: 19m
			//(59, 68, OutsideSegment, Roll),               // len: 9m
			//(68, 101, OutsideSegment, Accelerate),        // len: 33m
			//(101, 112, OutsideSegment, Roll),             // len: 11m
			//(112, 185, OutsideSegment, Accelerate),       // len: 73m
			//(185, 198, OutsideSegment, Roll),             // len: 13m
			//(198, 346, OutsideSegment, Accelerate),       // len: 148m
			//(346, 363, OutsideSegment, Roll),             // len: 17m
			//(363, 492, OutsideSegment, Accelerate),       // len: 129m
			//(492, 511, OutsideSegment, Roll),             // len: 19m
			(511, 3418, OutsideSegment, Accelerate),      // len: 2907m
			(3418, 3842, OutsideSegment, Coast),          // len: 424m
			(3842, 4506, OutsideSegment, Brake),          // len: 664m
			(4506, 4785, OutsideSegment, Coast),          // len: 279m
			(4785, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5612, WithinSegment, Accelerate),      // len: 1482m
			(5612, 5811, UseCase1, Coast),                // len: 199m
			(5811, 5976, OutsideSegment, Coast),          // len: 165m
			(5976, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5682, WithinSegment, Accelerate),      // len: 1027m
			(5682, 6551, UseCase1, Coast),                // len: 869m
			(6551, 7253, WithinSegment, Coast),           // len: 702m
			(7253, 7933, OutsideSegment, Coast),          // len: 680m
			(7933, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4994, WithinSegment, Accelerate),      // len: 1015m
			(4994, 5948, UseCase1, Coast),                // len: 954m
			(5948, 6141, WithinSegment, Coast),           // len: 193m
			(6141, 7155, WithinSegment, Brake),           // len: 1014m
			(7155, 7180, OutsideSegment, Brake),          // len: 25m
			(7180, 7592, OutsideSegment, Coast),          // len: 412m
			(7592, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1949, WithinSegment, Accelerate),       // len: 1295m
			(1949, 2381, UseCase1, Coast),                // len: 432m
			(2381, 2489, WithinSegment, Coast),           // len: 108m
			(2489, 2607, OutsideSegment, Coast),          // len: 118m
			(2607, 4042, OutsideSegment, Accelerate),     // len: 1435m
			(4042, 5011, WithinSegment, Accelerate),      // len: 969m
			(5011, 6022, UseCase1, Coast),                // len: 1011m
			(6022, 6276, WithinSegment, Coast),           // len: 254m
			(6276, 6684, WithinSegment, Brake),           // len: 408m
			(6684, 6709, OutsideSegment, Brake),          // len: 25m
			(6709, 7109, OutsideSegment, Coast),          // len: 400m
			(7109, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2112, WithinSegment, Accelerate),       // len: 1423m
			(2112, 2381, UseCase1, Coast),                // len: 269m
			(2381, 2475, WithinSegment, Coast),           // len: 94m
			(2475, 3035, WithinSegment, Accelerate),      // len: 560m
			(3035, 3700, UseCase1, Coast),                // len: 665m
			(3700, 3978, WithinSegment, Coast),           // len: 278m
			(3978, 4352, OutsideSegment, Coast),          // len: 374m
			(4352, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2124, WithinSegment, Accelerate),       // len: 1424m
			(2124, 2393, UseCase1, Coast),                // len: 269m
			(2393, 2522, WithinSegment, Coast),           // len: 129m
			(2522, 2744, WithinSegment, Accelerate),      // len: 222m
			(2744, 3129, UseCase1, Coast),                // len: 385m
			(3129, 3272, WithinSegment, Coast),           // len: 143m
			(3272, 3485, OutsideSegment, Coast),          // len: 213m
			(3485, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5134, WithinSegment, Accelerate),      // len: 1190m
			(5134, 5785, UseCase1, Coast),                // len: 651m
			(5785, 6027, WithinSegment, Coast),           // len: 242m
			(6027, 6559, WithinSegment, Brake),           // len: 532m
			(6559, 6633, OutsideSegment, Brake),          // len: 74m
			(6633, 7384, OutsideSegment, Coast),          // len: 751m
			(7384, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4982, WithinSegment, Accelerate),      // len: 1167m
			(4982, 5682, UseCase1, Coast),                // len: 700m
			(5682, 5995, WithinSegment, Coast),           // len: 313m
			(5995, 6319, OutsideSegment, Coast),          // len: 324m
			(6319, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4519, WithinSegment, Accelerate),      // len: 860m
			(4519, 4538, WithinSegment, Roll),            // len: 19m
			(4538, 4735, WithinSegment, Accelerate),      // len: 197m
			(4735, 4748, WithinSegment, Roll),            // len: 13m
			(4748, 4786, WithinSegment, Accelerate),      // len: 38m
			(4786, 4797, WithinSegment, Roll),            // len: 11m
			(4797, 5000, WithinSegment, Accelerate),      // len: 203m
			(5000, 5012, WithinSegment, Roll),            // len: 12m
			(5012, 5044, WithinSegment, Accelerate),      // len: 32m
			(5044, 5057, WithinSegment, Roll),            // len: 13m
			(5057, 5313, WithinSegment, Accelerate),      // len: 256m
			(5313, 5617, UseCase2, Coast),                // len: 304m
			(5617, 5799, WithinSegment, Coast),           // len: 182m
			(5799, 6120, WithinSegment, Brake),           // len: 321m
			(6120, 6145, OutsideSegment, Brake),          // len: 25m
			(6145, 6533, OutsideSegment, Coast),          // len: 388m
			(6533, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 105, OutsideSegment, Accelerate),        // len: 63m
			//(105, 117, OutsideSegment, Roll),             // len: 12m
			//(117, 149, OutsideSegment, Accelerate),       // len: 32m
			//(149, 162, OutsideSegment, Roll),             // len: 13m
			//(162, 368, OutsideSegment, Accelerate),       // len: 206m
			//(368, 387, OutsideSegment, Roll),             // len: 19m
			(500, 3411, OutsideSegment, Accelerate),      // len: 3024m
			(3411, 4160, OutsideSegment, Coast),          // len: 749m
			(4160, 5016, OutsideSegment, Brake),          // len: 856m
			(5016, 5455, OutsideSegment, Coast),          // len: 439m
			(5455, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 105, OutsideSegment, Accelerate),        // len: 63m
			//(105, 117, OutsideSegment, Roll),             // len: 12m
			//(117, 149, OutsideSegment, Accelerate),       // len: 32m
			//(149, 162, OutsideSegment, Roll),             // len: 13m
			//(162, 368, OutsideSegment, Accelerate),       // len: 206m
			//(368, 387, OutsideSegment, Roll),             // len: 19m
			(500, 3605, OutsideSegment, Accelerate),      // len: 3218m
			(3605, 3990, OutsideSegment, Coast),          // len: 385m
			(3990, 4514, OutsideSegment, Brake),          // len: 524m
			(4514, 4742, OutsideSegment, Coast),          // len: 228m
			(4742, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5531, OutsideSegment, Accelerate),        // len: 5531m
			(5531, 6062, OutsideSegment, Coast),          // len: 531m
			(6062, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5892, OutsideSegment, Accelerate),        // len: 5892m
			(5892, 6387, OutsideSegment, Coast),          // len: 495m
			(6387, 7481, OutsideSegment, Brake),          // len: 1094m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5367, OutsideSegment, Accelerate),        // len: 5367m
			(5367, 5685, OutsideSegment, Coast),          // len: 318m
			(5685, 7247, OutsideSegment, Brake),          // len: 1562m
			(7247, 7509, OutsideSegment, Coast),          // len: 262m
			(7509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2124, OutsideSegment, Accelerate),        // len: 2124m
			(2124, 2265, OutsideSegment, Coast),          // len: 141m
			(2265, 2494, OutsideSegment, Brake),          // len: 229m
			(2494, 2613, OutsideSegment, Coast),          // len: 119m
			(2613, 5424, OutsideSegment, Accelerate),     // len: 2811m
			(5424, 5742, OutsideSegment, Coast),          // len: 318m
			(5742, 6776, OutsideSegment, Brake),          // len: 1034m
			(6776, 7026, OutsideSegment, Coast),          // len: 250m
			(7026, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2525, OutsideSegment, Coast),          // len: 366m
			(2525, 3284, OutsideSegment, Accelerate),     // len: 759m
			(3284, 3508, OutsideSegment, Coast),          // len: 224m
			(3508, 4024, OutsideSegment, Brake),          // len: 516m
			(4024, 4286, OutsideSegment, Coast),          // len: 262m
			(4286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2643, OutsideSegment, Coast),          // len: 484m
			(2643, 2818, OutsideSegment, Accelerate),     // len: 175m
			(2818, 3137, OutsideSegment, Coast),          // len: 319m
			(3137, 3329, OutsideSegment, Brake),          // len: 192m
			(3329, 3567, OutsideSegment, Coast),          // len: 238m
			(3567, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 5414, OutsideSegment, Accelerate),        // len: 5414m
			(5414, 5567, OutsideSegment, Coast),          // len: 153m
			(5567, 6817, OutsideSegment, Brake),          // len: 1250m
			(6817, 7352, OutsideSegment, Coast),          // len: 535m
			(7352, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 5227, OutsideSegment, Accelerate),        // len: 5227m
			(5227, 5522, OutsideSegment, Coast),          // len: 295m
			(5522, 6063, OutsideSegment, Brake),          // len: 541m
			(6063, 6289, OutsideSegment, Coast),          // len: 226m
			(6289, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 4563, OutsideSegment, Accelerate),        // len: 4472m
			(4563, 4582, OutsideSegment, Roll),           // len: 19m
			(4582, 4709, OutsideSegment, Accelerate),     // len: 127m
			(4709, 4723, OutsideSegment, Roll),           // len: 14m
			(4723, 4789, OutsideSegment, Accelerate),     // len: 66m
			(4789, 4800, OutsideSegment, Roll),           // len: 11m
			(4800, 4866, OutsideSegment, Accelerate),     // len: 66m
			(4866, 4875, OutsideSegment, Roll),           // len: 9m
			(4875, 4931, OutsideSegment, Accelerate),     // len: 56m
			(4931, 4940, OutsideSegment, Roll),           // len: 9m
			(4940, 5028, OutsideSegment, Accelerate),     // len: 88m
			(5028, 5040, OutsideSegment, Roll),           // len: 12m
			(5040, 5079, OutsideSegment, Accelerate),     // len: 39m
			(5079, 5093, OutsideSegment, Roll),           // len: 14m
			(5093, 5131, OutsideSegment, Accelerate),     // len: 38m
			(5131, 5147, OutsideSegment, Roll),           // len: 16m
			(5147, 5444, OutsideSegment, Accelerate),     // len: 297m
			(5444, 5539, OutsideSegment, Coast),          // len: 95m
			(5539, 6116, OutsideSegment, Brake),          // len: 577m
			(6116, 6401, OutsideSegment, Coast),          // len: 285m
			(6401, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5531, OutsideSegment, Accelerate),        // len: 5531m
			(5531, 6062, OutsideSegment, Coast),          // len: 531m
			(6062, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5892, OutsideSegment, Accelerate),        // len: 5892m
			(5892, 6387, OutsideSegment, Coast),          // len: 495m
			(6387, 7481, OutsideSegment, Brake),          // len: 1094m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5367, OutsideSegment, Accelerate),        // len: 5367m
			(5367, 5685, OutsideSegment, Coast),          // len: 318m
			(5685, 7247, OutsideSegment, Brake),          // len: 1562m
			(7247, 7509, OutsideSegment, Coast),          // len: 262m
			(7509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2124, OutsideSegment, Accelerate),        // len: 2124m
			(2124, 2265, OutsideSegment, Coast),          // len: 141m
			(2265, 2494, OutsideSegment, Brake),          // len: 229m
			(2494, 2613, OutsideSegment, Coast),          // len: 119m
			(2613, 5424, OutsideSegment, Accelerate),     // len: 2811m
			(5424, 5742, OutsideSegment, Coast),          // len: 318m
			(5742, 6776, OutsideSegment, Brake),          // len: 1034m
			(6776, 7026, OutsideSegment, Coast),          // len: 250m
			(7026, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2525, OutsideSegment, Coast),          // len: 366m
			(2525, 3284, OutsideSegment, Accelerate),     // len: 759m
			(3284, 3508, OutsideSegment, Coast),          // len: 224m
			(3508, 4024, OutsideSegment, Brake),          // len: 516m
			(4024, 4286, OutsideSegment, Coast),          // len: 262m
			(4286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2643, OutsideSegment, Coast),          // len: 484m
			(2643, 2818, OutsideSegment, Accelerate),     // len: 175m
			(2818, 3137, OutsideSegment, Coast),          // len: 319m
			(3137, 3329, OutsideSegment, Brake),          // len: 192m
			(3329, 3567, OutsideSegment, Coast),          // len: 238m
			(3567, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 5414, OutsideSegment, Accelerate),        // len: 5414m
			(5414, 5567, OutsideSegment, Coast),          // len: 153m
			(5567, 6817, OutsideSegment, Brake),          // len: 1250m
			(6817, 7352, OutsideSegment, Coast),          // len: 535m
			(7352, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 5227, OutsideSegment, Accelerate),        // len: 5227m
			(5227, 5522, OutsideSegment, Coast),          // len: 295m
			(5522, 6063, OutsideSegment, Brake),          // len: 541m
			(6063, 6289, OutsideSegment, Coast),          // len: 226m
			(6289, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 4563, OutsideSegment, Accelerate),        // len: 4472m
			(4563, 4582, OutsideSegment, Roll),           // len: 19m
			(4582, 4709, OutsideSegment, Accelerate),     // len: 127m
			(4709, 4723, OutsideSegment, Roll),           // len: 14m
			(4723, 4789, OutsideSegment, Accelerate),     // len: 66m
			(4789, 4800, OutsideSegment, Roll),           // len: 11m
			(4800, 4866, OutsideSegment, Accelerate),     // len: 66m
			(4866, 4875, OutsideSegment, Roll),           // len: 9m
			(4875, 4931, OutsideSegment, Accelerate),     // len: 56m
			(4931, 4940, OutsideSegment, Roll),           // len: 9m
			(4940, 5028, OutsideSegment, Accelerate),     // len: 88m
			(5028, 5040, OutsideSegment, Roll),           // len: 12m
			(5040, 5079, OutsideSegment, Accelerate),     // len: 39m
			(5079, 5093, OutsideSegment, Roll),           // len: 14m
			(5093, 5131, OutsideSegment, Accelerate),     // len: 38m
			(5131, 5147, OutsideSegment, Roll),           // len: 16m
			(5147, 5444, OutsideSegment, Accelerate),     // len: 297m
			(5444, 5539, OutsideSegment, Coast),          // len: 95m
			(5539, 6116, OutsideSegment, Brake),          // len: 577m
			(6116, 6401, OutsideSegment, Coast),          // len: 285m
			(6401, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6872, WithinSegment, Coast),           // len: 411m
			(6872, 7256, WithinSegment, Brake),           // len: 384m
			(7256, 7466, OutsideSegment, Brake),          // len: 210m
			(7466, 8025, OutsideSegment, Coast),          // len: 559m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 6047, WithinSegment, Coast),           // len: 169m
			(6047, 7160, WithinSegment, Brake),           // len: 1113m
			(7160, 7234, OutsideSegment, Brake),          // len: 74m
			(7234, 7658, OutsideSegment, Coast),          // len: 424m
			(7658, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2378, UseCase1, Coast),                // len: 476m
			(2378, 2486, WithinSegment, Coast),           // len: 108m
			(2486, 2629, OutsideSegment, Coast),          // len: 143m
			(2629, 4041, OutsideSegment, Accelerate),     // len: 1412m
			(4041, 4951, WithinSegment, Accelerate),      // len: 910m
			(4951, 5939, UseCase1, Coast),                // len: 988m
			(5939, 6145, WithinSegment, Coast),           // len: 206m
			(6145, 6689, WithinSegment, Brake),           // len: 544m
			(6689, 6763, OutsideSegment, Brake),          // len: 74m
			(6763, 7152, OutsideSegment, Coast),          // len: 389m
			(7152, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2345, UseCase1, Coast),                // len: 268m
			(2345, 2499, WithinSegment, Coast),           // len: 154m
			(2499, 2989, WithinSegment, Accelerate),      // len: 490m
			(2989, 3694, UseCase1, Coast),                // len: 705m
			(3694, 3900, WithinSegment, Coast),           // len: 206m
			(3900, 3974, WithinSegment, Brake),           // len: 74m
			(3974, 4023, OutsideSegment, Brake),          // len: 49m
			(4023, 4435, OutsideSegment, Coast),          // len: 412m
			(4435, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2605, WithinSegment, Coast),           // len: 259m
			(2605, 2652, WithinSegment, Accelerate),      // len: 47m
			(2652, 3093, UseCase1, Coast),                // len: 441m
			(3093, 3272, WithinSegment, Coast),           // len: 179m
			(3272, 3571, OutsideSegment, Coast),          // len: 299m
			(3571, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5922, WithinSegment, Coast),           // len: 194m
			(5922, 6564, WithinSegment, Brake),           // len: 642m
			(6564, 6787, OutsideSegment, Brake),          // len: 223m
			(6787, 7539, OutsideSegment, Coast),          // len: 752m
			(7539, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5988, WithinSegment, Coast),           // len: 327m
			(5988, 6401, OutsideSegment, Coast),          // len: 413m
			(6401, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 913m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4709, WithinSegment, Accelerate),      // len: 127m
			(4709, 4723, WithinSegment, Roll),            // len: 14m
			(4723, 4789, WithinSegment, Accelerate),      // len: 66m
			(4789, 4800, WithinSegment, Roll),            // len: 11m
			(4800, 4866, WithinSegment, Accelerate),      // len: 66m
			(4866, 4875, WithinSegment, Roll),            // len: 9m
			(4875, 4931, WithinSegment, Accelerate),      // len: 56m
			(4931, 4940, WithinSegment, Roll),            // len: 9m
			(4940, 5028, WithinSegment, Accelerate),      // len: 88m
			(5028, 5040, WithinSegment, Roll),            // len: 12m
			(5040, 5079, WithinSegment, Accelerate),      // len: 39m
			(5079, 5093, WithinSegment, Roll),            // len: 14m
			(5093, 5131, WithinSegment, Accelerate),      // len: 38m
			(5131, 5147, WithinSegment, Roll),            // len: 16m
			(5147, 5343, WithinSegment, Accelerate),      // len: 196m
			(5343, 5614, UseCase2, Coast),                // len: 271m
			(5614, 5771, WithinSegment, Coast),           // len: 157m
			(5771, 6117, WithinSegment, Brake),           // len: 346m
			(6117, 6191, OutsideSegment, Brake),          // len: 74m
			(6191, 6591, OutsideSegment, Coast),          // len: 400m
			(6591, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6872, WithinSegment, Coast),           // len: 411m
			(6872, 7256, WithinSegment, Brake),           // len: 384m
			(7256, 7466, OutsideSegment, Brake),          // len: 210m
			(7466, 8025, OutsideSegment, Coast),          // len: 559m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 6047, WithinSegment, Coast),           // len: 169m
			(6047, 7160, WithinSegment, Brake),           // len: 1113m
			(7160, 7234, OutsideSegment, Brake),          // len: 74m
			(7234, 7658, OutsideSegment, Coast),          // len: 424m
			(7658, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2378, UseCase1, Coast),                // len: 476m
			(2378, 2486, WithinSegment, Coast),           // len: 108m
			(2486, 2629, OutsideSegment, Coast),          // len: 143m
			(2629, 4041, OutsideSegment, Accelerate),     // len: 1412m
			(4041, 4951, WithinSegment, Accelerate),      // len: 910m
			(4951, 5939, UseCase1, Coast),                // len: 988m
			(5939, 6145, WithinSegment, Coast),           // len: 206m
			(6145, 6689, WithinSegment, Brake),           // len: 544m
			(6689, 6763, OutsideSegment, Brake),          // len: 74m
			(6763, 7152, OutsideSegment, Coast),          // len: 389m
			(7152, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2345, UseCase1, Coast),                // len: 268m
			(2345, 2499, WithinSegment, Coast),           // len: 154m
			(2499, 2989, WithinSegment, Accelerate),      // len: 490m
			(2989, 3694, UseCase1, Coast),                // len: 705m
			(3694, 3900, WithinSegment, Coast),           // len: 206m
			(3900, 3974, WithinSegment, Brake),           // len: 74m
			(3974, 4023, OutsideSegment, Brake),          // len: 49m
			(4023, 4435, OutsideSegment, Coast),          // len: 412m
			(4435, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2605, WithinSegment, Coast),           // len: 259m
			(2605, 2652, WithinSegment, Accelerate),      // len: 47m
			(2652, 3093, UseCase1, Coast),                // len: 441m
			(3093, 3272, WithinSegment, Coast),           // len: 179m
			(3272, 3571, OutsideSegment, Coast),          // len: 299m
			(3571, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5922, WithinSegment, Coast),           // len: 194m
			(5922, 6564, WithinSegment, Brake),           // len: 642m
			(6564, 6787, OutsideSegment, Brake),          // len: 223m
			(6787, 7539, OutsideSegment, Coast),          // len: 752m
			(7539, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5988, WithinSegment, Coast),           // len: 327m
			(5988, 6401, OutsideSegment, Coast),          // len: 413m
			(6401, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 1004m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4709, WithinSegment, Accelerate),      // len: 127m
			(4709, 4723, WithinSegment, Roll),            // len: 14m
			(4723, 4789, WithinSegment, Accelerate),      // len: 66m
			(4789, 4800, WithinSegment, Roll),            // len: 11m
			(4800, 4866, WithinSegment, Accelerate),      // len: 66m
			(4866, 4875, WithinSegment, Roll),            // len: 9m
			(4875, 4931, WithinSegment, Accelerate),      // len: 56m
			(4931, 4940, WithinSegment, Roll),            // len: 9m
			(4940, 5028, WithinSegment, Accelerate),      // len: 88m
			(5028, 5040, WithinSegment, Roll),            // len: 12m
			(5040, 5079, WithinSegment, Accelerate),      // len: 39m
			(5079, 5093, WithinSegment, Roll),            // len: 14m
			(5093, 5131, WithinSegment, Accelerate),      // len: 38m
			(5131, 5147, WithinSegment, Roll),            // len: 16m
			(5147, 5343, WithinSegment, Accelerate),      // len: 196m
			(5343, 5614, UseCase2, Coast),                // len: 271m
			(5614, 5771, WithinSegment, Coast),           // len: 157m
			(5771, 6117, WithinSegment, Brake),           // len: 346m
			(6117, 6191, OutsideSegment, Brake),          // len: 74m
			(6191, 6591, OutsideSegment, Coast),          // len: 400m
			(6591, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5509, OutsideSegment, Coast),          // len: 299m
			(5509, 1e6, OutsideSegment, Accelerate));




		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 4, OutsideSegment, Accelerate),           // len: 4m
			//(4, 6, OutsideSegment, Roll),                 // len: 2m
			//(6, 15, OutsideSegment, Accelerate),          // len: 9m
			//(15, 20, OutsideSegment, Roll),               // len: 5m
			//(20, 35, OutsideSegment, Accelerate),         // len: 15m
			//(35, 42, OutsideSegment, Roll),               // len: 7m
			//(42, 71, OutsideSegment, Accelerate),         // len: 29m
			//(71, 81, OutsideSegment, Roll),               // len: 10m
			//(81, 139, OutsideSegment, Accelerate),        // len: 58m
			//(139, 153, OutsideSegment, Roll),             // len: 14m
			//(153, 242, OutsideSegment, Accelerate),       // len: 89m
			//(242, 258, OutsideSegment, Roll),             // len: 16m
			//(258, 392, OutsideSegment, Accelerate),       // len: 134m
			//(392, 411, OutsideSegment, Roll),             // len: 19m
			(500, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4781, OutsideSegment, Coast),          // len: 268m
			(4781, 1e6, OutsideSegment, Accelerate));



		private void TestPCC(string testname,
			params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
		{
			var jobName = testname.Split('_').Slice(0, -2).Join("_");
			var cycleName = testname.Split('_').Reverse().Skip(1).First();
			DoTestPCC(jobName, cycleName, data);
		}

		private void DoTestPCC(string jobName, string cycleName,
			params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
		{
			jobName = Path.Combine(BasePath, jobName + ".vecto");

			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));
			var sumContainer = new SummaryDataContainer(writer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) { WriteModalResults = true, Validate = false, SumData = sumContainer };

			var run = factory.SimulationRuns().First(r => r.CycleName == cycleName);
			var mod = (run.GetContainer().ModalData as ModalDataContainer).Data;
			run.Run();
			PrintPCCSections(mod);
			Assert.IsTrue(run.FinishedWithoutErrors);

			var expected = data;

			var segmentWasTested = false;

			var distances = mod.Columns[ModalResultField.dist.GetName()].Values<Meter>();
			var pccStates = mod.Columns["PCCState"].Values<PCCStates>();
			var actions = mod.Columns["DriverAction"].Values<DrivingAction>();
			var vActs = mod.Columns[ModalResultField.v_act.GetName()].Values<MeterPerSecond>();

			using (var exp = expected.AsEnumerable().GetEnumerator()) {
				exp.MoveNext();
				foreach (var (dist, pcc, action, vAct) in distances.Zip(pccStates, actions, vActs)) {
					if (dist > exp.Current.end) {
						Assert.IsTrue(segmentWasTested, $"dist {dist}: Expected Segment was not tested. Maybe distance range to narrow?");
						if (!exp.MoveNext())
							break;
						segmentWasTested = false;
					}

					if (dist.IsBetween(exp.Current.start, exp.Current.end)) {
						// if the segment is very short, at least one of the entries should have the expected values
						if (exp.Current.pcc == pcc && exp.Current.action == action)
							segmentWasTested = true;
					}

					if (dist.IsBetween(exp.Current.start.SI<Meter>() + vAct * tolerance.SI<Second>(), exp.Current.end.SI<Meter>() - vAct * tolerance.SI<Second>())) {
						Assert.AreEqual(exp.Current.pcc, pcc, $"dist {dist}: Wrong PCC state: {pcc} instead of {exp.Current.pcc}.");
						Assert.AreEqual(exp.Current.action, action, $"dist {dist}: Wrong DriverAction: {action} instead of {exp.Current.action}.");
						segmentWasTested = true;
					}
				}
			}

			Assert.IsTrue(segmentWasTested);
		}


		private void PrintPCCSections(ModalResults mod)
		{
			var sCol = mod.Columns[ModalResultField.dist.GetName()];
			var pccCol = mod.Columns["PCCState"];
			var driverActionCol = mod.Columns["DriverAction"];

			var pccStates = pccCol.Values<PCCStates>();
			var driverAction = driverActionCol.Values<DrivingAction>();
			var distances = sCol.Values<Meter>();
			var sections = GetDistancesOfStateChanges(pccStates.Zip(driverAction), distances).ToArray();

			Console.WriteLine("Start-End Segments:");
			if (sections.Any()) {
				var start = 0d;
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {(int)section.Distance.Value()}, {section.Before.Item1}, {section.Before.Item2}),",-45} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({(int)sections.Last().Distance.Value()}, 1e6, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
			} else {
				Console.WriteLine("(0, 1e6, OutsideSegment, Accelerate));");
			}
		}

		IEnumerable<(T2 Distance, T1 Before, T1 After, T2[] SegmentValues)> GetDistancesOfStateChanges<T1, T2>(IEnumerable<T1> states, IEnumerable<T2> distances)
		{
			using (var values = states.GetEnumerator()) {
				using (var distance = distances.GetEnumerator()) {
					distance.MoveNext();
					values.MoveNext();
					var value = values.Current;
					var segmentValues = new List<T2> { distance.Current };
					while (values.MoveNext() | distance.MoveNext()) {
						if (!value.Equals(values.Current)) {
							yield return (distance.Current, value, values.Current, segmentValues.ToArray());
							segmentValues.Clear();
							value = values.Current;
							segmentValues.Add(distance.Current);
						}
					}
				}
			}
		}
	}
}
