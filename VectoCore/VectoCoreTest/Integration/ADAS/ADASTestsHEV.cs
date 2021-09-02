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
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DefaultDriverStrategy.PCCStates;
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

		#region PCC Engineering Testcases
		[TestCase]
		public void Class5_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5461, WithinSegment, Accelerate),      // len: 1331m
			(5461, 5624, UseCase1, Coast),                // len: 163m
			(5624, 5635, UseCase1, Roll),                 // len: 11m
			(5635, 5647, UseCase1, Coast),                // len: 12m
			(5647, 5659, UseCase1, Roll),                 // len: 12m
			(5659, 5764, UseCase1, Coast),                // len: 105m
			(5764, 5812, WithinSegment, Coast),           // len: 48m
			(5812, 6025, OutsideSegment, Coast),          // len: 213m
			(6025, 6071, OutsideSegment, Roll),           // len: 46m
			(6071, 6083, OutsideSegment, Accelerate),     // len: 12m
			(6083, 6106, OutsideSegment, Roll),           // len: 23m
			(6106, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5472, WithinSegment, Accelerate),      // len: 817m
			(5472, 7150, UseCase1, Coast),                // len: 1678m
			(7150, 7185, UseCase1, Roll),                 // len: 35m
			(7185, 7256, UseCase1, Coast),                // len: 71m
			(7256, 7385, OutsideSegment, Coast),          // len: 129m
			(7385, 7431, OutsideSegment, Roll),           // len: 46m
			(7431, 7478, OutsideSegment, Coast),          // len: 47m
			(7478, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4936, WithinSegment, Accelerate),      // len: 981m
			(4936, 6045, UseCase1, Coast),                // len: 1109m
			(6045, 6057, UseCase1, Roll),                 // len: 12m
			(6057, 6069, UseCase1, Brake),                // len: 12m
			(6069, 6093, WithinSegment, Roll),            // len: 24m
			(6093, 6251, WithinSegment, Coast),           // len: 158m
			(6251, 7141, WithinSegment, Brake),           // len: 890m
			(7141, 7190, WithinSegment, Coast),           // len: 49m
			(7190, 7554, OutsideSegment, Coast),          // len: 364m
			(7554, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1891, WithinSegment, Accelerate),       // len: 1237m
			(1891, 2485, UseCase1, Coast),                // len: 594m
			(2485, 4036, OutsideSegment, Accelerate),     // len: 1551m
			(4036, 4946, WithinSegment, Accelerate),      // len: 910m
			(4946, 6155, UseCase1, Coast),                // len: 1209m
			(6155, 6167, UseCase1, Roll),                 // len: 12m
			(6167, 6179, UseCase1, Brake),                // len: 12m
			(6179, 6203, WithinSegment, Roll),            // len: 24m
			(6203, 6494, WithinSegment, Coast),           // len: 291m
			(6494, 6680, WithinSegment, Brake),           // len: 186m
			(6680, 6692, WithinSegment, Coast),           // len: 12m
			(6692, 7069, OutsideSegment, Coast),          // len: 377m
			(7069, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2251, UseCase1, Coast),                // len: 174m
			(2251, 2263, UseCase1, Roll),                 // len: 12m
			(2263, 2275, UseCase1, Coast),                // len: 12m
			(2275, 2286, UseCase1, Roll),                 // len: 11m
			(2286, 2345, UseCase1, Coast),                // len: 59m
			(2345, 2499, WithinSegment, Coast),           // len: 154m
			(2499, 2989, WithinSegment, Accelerate),      // len: 490m
			(2989, 3817, UseCase1, Coast),                // len: 828m
			(3817, 3828, UseCase1, Roll),                 // len: 11m
			(3828, 3840, UseCase1, Brake),                // len: 12m
			(3840, 3864, WithinSegment, Roll),            // len: 24m
			(3864, 3995, WithinSegment, Coast),           // len: 131m
			(3995, 4197, OutsideSegment, Coast),          // len: 202m
			(4197, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2274, UseCase1, Coast),                // len: 197m
			(2274, 2286, UseCase1, Roll),                 // len: 12m
			(2286, 2298, UseCase1, Coast),                // len: 12m
			(2298, 2310, UseCase1, Roll),                 // len: 12m
			(2310, 2369, UseCase1, Coast),                // len: 59m
			(2369, 2557, WithinSegment, Coast),           // len: 188m
			(2557, 2639, WithinSegment, Accelerate),      // len: 82m
			(2639, 3270, UseCase1, Coast),                // len: 631m
			(3270, 3294, OutsideSegment, Accelerate),     // len: 24m
			(3294, 3340, OutsideSegment, Coast),          // len: 46m
			(3340, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5087, WithinSegment, Accelerate),      // len: 1143m
			(5087, 5852, UseCase1, Coast),                // len: 765m
			(5852, 5863, UseCase1, Roll),                 // len: 11m
			(5863, 5875, UseCase1, Brake),                // len: 12m
			(5875, 5899, WithinSegment, Roll),            // len: 24m
			(5899, 6203, WithinSegment, Coast),           // len: 304m
			(6203, 6537, WithinSegment, Brake),           // len: 334m
			(6537, 6562, WithinSegment, Coast),           // len: 25m
			(6562, 7265, OutsideSegment, Coast),          // len: 703m
			(7265, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4842, WithinSegment, Accelerate),      // len: 1027m
			(4842, 5996, UseCase1, Coast),                // len: 1154m
			(5996, 6019, OutsideSegment, Accelerate),     // len: 23m
			(6019, 6066, OutsideSegment, Coast),          // len: 47m
			(6066, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4472, WithinSegment, Accelerate),      // len: 913m
			(4472, 4492, WithinSegment, Roll),            // len: 20m
			(4492, 4679, WithinSegment, Accelerate),      // len: 187m
			(4679, 4695, WithinSegment, Roll),            // len: 16m
			(4695, 4782, WithinSegment, Accelerate),      // len: 87m
			(4782, 4794, WithinSegment, Roll),            // len: 12m
			(4794, 4888, WithinSegment, Accelerate),      // len: 94m
			(4888, 4898, WithinSegment, Roll),            // len: 10m
			(4898, 4963, WithinSegment, Accelerate),      // len: 65m
			(4963, 4974, WithinSegment, Roll),            // len: 11m
			(4974, 5026, WithinSegment, Accelerate),      // len: 52m
			(5026, 5039, WithinSegment, Roll),            // len: 13m
			(5039, 5074, WithinSegment, Accelerate),      // len: 35m
			(5074, 5088, WithinSegment, Roll),            // len: 14m
			(5088, 5135, WithinSegment, Accelerate),      // len: 47m
			(5135, 5152, WithinSegment, Roll),            // len: 17m
			(5152, 5333, WithinSegment, Accelerate),      // len: 181m
			(5333, 5591, UseCase2, Coast),                // len: 258m
			(5591, 5603, UseCase2, Roll),                 // len: 12m
			(5603, 5615, UseCase2, Brake),                // len: 12m
			(5615, 5639, WithinSegment, Roll),            // len: 24m
			(5639, 5797, WithinSegment, Coast),           // len: 158m
			(5797, 6106, WithinSegment, Brake),           // len: 309m
			(6106, 6118, WithinSegment, Coast),           // len: 12m
			(6118, 6494, OutsideSegment, Coast),          // len: 376m
			(6494, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5479, OutsideSegment, Coast),          // len: 249m
			(5479, 5489, OutsideSegment, Accelerate),     // len: 10m
			(5489, 5509, OutsideSegment, Roll),           // len: 20m
			(5509, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5461, WithinSegment, Accelerate),      // len: 1331m
			(5461, 5624, UseCase1, Coast),                // len: 163m
			(5624, 5635, UseCase1, Roll),                 // len: 11m
			(5635, 5647, UseCase1, Coast),                // len: 12m
			(5647, 5659, UseCase1, Roll),                 // len: 12m
			(5659, 5764, UseCase1, Coast),                // len: 105m
			(5764, 5812, WithinSegment, Coast),           // len: 48m
			(5812, 6025, OutsideSegment, Coast),          // len: 213m
			(6025, 6071, OutsideSegment, Roll),           // len: 46m
			(6071, 6083, OutsideSegment, Accelerate),     // len: 12m
			(6083, 6106, OutsideSegment, Roll),           // len: 23m
			(6106, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5472, WithinSegment, Accelerate),      // len: 817m
			(5472, 7150, UseCase1, Coast),                // len: 1678m
			(7150, 7185, UseCase1, Roll),                 // len: 35m
			(7185, 7256, UseCase1, Coast),                // len: 71m
			(7256, 7385, OutsideSegment, Coast),          // len: 129m
			(7385, 7431, OutsideSegment, Roll),           // len: 46m
			(7431, 7478, OutsideSegment, Coast),          // len: 47m
			(7478, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6045, UseCase1, Coast),                // len: 1109m
			(6045, 6057, UseCase1, Roll),                 // len: 12m
			(6057, 6069, UseCase1, Brake),                // len: 12m
			(6069, 6093, WithinSegment, Roll),            // len: 24m
			(6093, 6117, WithinSegment, Coast),           // len: 24m
			(6117, 7150, WithinSegment, Brake),           // len: 1033m
			(7150, 7436, OutsideSegment, Coast),          // len: 286m
			(7436, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1891, WithinSegment, Accelerate),       // len: 1237m
			(1891, 2485, UseCase1, Coast),                // len: 594m
			(2485, 4036, OutsideSegment, Accelerate),     // len: 1551m
			(4036, 4946, WithinSegment, Accelerate),      // len: 910m
			(4946, 6155, UseCase1, Coast),                // len: 1209m
			(6155, 6167, UseCase1, Roll),                 // len: 12m
			(6167, 6179, UseCase1, Brake),                // len: 12m
			(6179, 6203, WithinSegment, Roll),            // len: 24m
			(6203, 6250, WithinSegment, Coast),           // len: 47m
			(6250, 6683, WithinSegment, Brake),           // len: 433m
			(6683, 6695, WithinSegment, Coast),           // len: 12m
			(6695, 6957, OutsideSegment, Coast),          // len: 262m
			(6957, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2251, UseCase1, Coast),                // len: 174m
			(2251, 2263, UseCase1, Roll),                 // len: 12m
			(2263, 2275, UseCase1, Coast),                // len: 12m
			(2275, 2286, UseCase1, Roll),                 // len: 11m
			(2286, 2345, UseCase1, Coast),                // len: 59m
			(2345, 2499, WithinSegment, Coast),           // len: 154m
			(2499, 2989, WithinSegment, Accelerate),      // len: 490m
			(2989, 3817, UseCase1, Coast),                // len: 828m
			(3817, 3828, UseCase1, Roll),                 // len: 11m
			(3828, 3840, UseCase1, Brake),                // len: 12m
			(3840, 3864, WithinSegment, Roll),            // len: 24m
			(3864, 3995, WithinSegment, Coast),           // len: 131m
			(3995, 4197, OutsideSegment, Coast),          // len: 202m
			(4197, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2274, UseCase1, Coast),                // len: 197m
			(2274, 2286, UseCase1, Roll),                 // len: 12m
			(2286, 2298, UseCase1, Coast),                // len: 12m
			(2298, 2310, UseCase1, Roll),                 // len: 12m
			(2310, 2369, UseCase1, Coast),                // len: 59m
			(2369, 2557, WithinSegment, Coast),           // len: 188m
			(2557, 2639, WithinSegment, Accelerate),      // len: 82m
			(2639, 3270, UseCase1, Coast),                // len: 631m
			(3270, 3294, OutsideSegment, Accelerate),     // len: 24m
			(3294, 3340, OutsideSegment, Coast),          // len: 46m
			(3340, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5087, WithinSegment, Accelerate),      // len: 1143m
			(5087, 5852, UseCase1, Coast),                // len: 765m
			(5852, 5863, UseCase1, Roll),                 // len: 11m
			(5863, 5875, UseCase1, Brake),                // len: 12m
			(5875, 5899, WithinSegment, Roll),            // len: 24m
			(5899, 5947, WithinSegment, Coast),           // len: 48m
			(5947, 6560, WithinSegment, Brake),           // len: 613m
			(6560, 7048, OutsideSegment, Coast),          // len: 488m
			(7048, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4842, WithinSegment, Accelerate),      // len: 1027m
			(4842, 5996, UseCase1, Coast),                // len: 1154m
			(5996, 6019, OutsideSegment, Accelerate),     // len: 23m
			(6019, 6066, OutsideSegment, Coast),          // len: 47m
			(6066, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4472, WithinSegment, Accelerate),      // len: 925m
			(5152, 5333, WithinSegment, Accelerate),      // len: 181m
			(5333, 5591, UseCase2, Coast),                // len: 258m
			(5591, 5603, UseCase2, Roll),                 // len: 12m
			(5603, 5615, UseCase2, Brake),                // len: 12m
			(5615, 5639, WithinSegment, Roll),            // len: 24m
			(5639, 5651, WithinSegment, Coast),           // len: 12m
			(5651, 6119, WithinSegment, Brake),           // len: 468m
			(6119, 6143, WithinSegment, Coast),           // len: 24m
			(6143, 6381, OutsideSegment, Coast),          // len: 238m
			(6381, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 20, OutsideSegment, Roll),               // len: 5m
			(20, 35, OutsideSegment, Accelerate),         // len: 15m
			(35, 42, OutsideSegment, Roll),               // len: 7m
			(42, 71, OutsideSegment, Accelerate),         // len: 29m
			(71, 81, OutsideSegment, Roll),               // len: 10m
			(81, 139, OutsideSegment, Accelerate),        // len: 58m
			(139, 153, OutsideSegment, Roll),             // len: 14m
			(153, 242, OutsideSegment, Accelerate),       // len: 89m
			(242, 258, OutsideSegment, Roll),             // len: 16m
			(258, 392, OutsideSegment, Accelerate),       // len: 134m
			(392, 411, OutsideSegment, Roll),             // len: 19m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5479, OutsideSegment, Coast),          // len: 249m
			(5479, 5489, OutsideSegment, Accelerate),     // len: 10m
			(5489, 5509, OutsideSegment, Roll),           // len: 20m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 20, OutsideSegment, Roll),               // len: 5m
			(20, 35, OutsideSegment, Accelerate),         // len: 15m
			(35, 42, OutsideSegment, Roll),               // len: 7m
			(42, 71, OutsideSegment, Accelerate),         // len: 29m
			(71, 81, OutsideSegment, Roll),               // len: 10m
			(81, 139, OutsideSegment, Accelerate),        // len: 58m
			(139, 153, OutsideSegment, Roll),             // len: 14m
			(153, 242, OutsideSegment, Accelerate),       // len: 89m
			(242, 258, OutsideSegment, Roll),             // len: 16m
			(258, 392, OutsideSegment, Accelerate),       // len: 134m
			(392, 411, OutsideSegment, Roll),             // len: 19m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5531, OutsideSegment, Accelerate),        // len: 5531m
			(5531, 6038, OutsideSegment, Coast),          // len: 507m
			(6038, 6085, OutsideSegment, Roll),           // len: 47m
			(6085, 6096, OutsideSegment, Accelerate),     // len: 11m
			(6096, 6120, OutsideSegment, Roll),           // len: 24m
			(6120, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5892, OutsideSegment, Accelerate),        // len: 5892m
			(5892, 6387, OutsideSegment, Coast),          // len: 495m
			(6387, 7481, OutsideSegment, Brake),          // len: 1094m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5367, OutsideSegment, Accelerate),        // len: 5367m
			(5367, 5685, OutsideSegment, Coast),          // len: 318m
			(5685, 7247, OutsideSegment, Brake),          // len: 1562m
			(7247, 7509, OutsideSegment, Coast),          // len: 262m
			(7509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_NoADAS_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2525, OutsideSegment, Coast),          // len: 366m
			(2525, 3284, OutsideSegment, Accelerate),     // len: 759m
			(3284, 3508, OutsideSegment, Coast),          // len: 224m
			(3508, 4024, OutsideSegment, Brake),          // len: 516m
			(4024, 4286, OutsideSegment, Coast),          // len: 262m
			(4286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2608, OutsideSegment, Coast),          // len: 449m
			(2608, 2655, OutsideSegment, Roll),           // len: 47m
			(2655, 2667, OutsideSegment, Accelerate),     // len: 12m
			(2667, 2690, OutsideSegment, Roll),           // len: 23m
			(2690, 2888, OutsideSegment, Accelerate),     // len: 198m
			(2888, 3442, OutsideSegment, Coast),          // len: 554m
			(3442, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5414, OutsideSegment, Accelerate),        // len: 5414m
			(5414, 5567, OutsideSegment, Coast),          // len: 153m
			(5567, 6817, OutsideSegment, Brake),          // len: 1250m
			(6817, 7305, OutsideSegment, Coast),          // len: 488m
			(7305, 7352, OutsideSegment, Roll),           // len: 47m
			(7352, 7363, OutsideSegment, Accelerate),     // len: 11m
			(7363, 7387, OutsideSegment, Roll),           // len: 24m
			(7387, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5227, OutsideSegment, Accelerate),        // len: 5227m
			(5227, 5522, OutsideSegment, Coast),          // len: 295m
			(5522, 6063, OutsideSegment, Brake),          // len: 541m
			(6063, 6289, OutsideSegment, Coast),          // len: 226m
			(6289, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4472, OutsideSegment, Accelerate),        // len: 4472m
			(5152, 5423, OutsideSegment, Accelerate),     // len: 271m
			(5506, 6119, OutsideSegment, Brake),          // len: 613m
			(6119, 6381, OutsideSegment, Coast),          // len: 262m
			(6381, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5479, OutsideSegment, Coast),          // len: 249m
			(5479, 5489, OutsideSegment, Accelerate),     // len: 10m
			(5489, 5509, OutsideSegment, Roll),           // len: 20m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_NoADAS_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4037, OutsideSegment, Accelerate),        // len: 4037m
			(4037, 5332, WithinSegment, Accelerate),      // len: 1295m
			(5332, 5897, UseCase1, Roll),                 // len: 565m
			(5897, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4387, OutsideSegment, Accelerate),        // len: 4387m
			(4387, 5262, WithinSegment, Accelerate),      // len: 875m
			(5262, 6760, UseCase1, Roll),                 // len: 1498m
			(6760, 6819, WithinSegment, Coast),           // len: 59m
			(6819, 7282, WithinSegment, Roll),            // len: 463m
			(7282, 7467, WithinSegment, Brake),           // len: 185m
			(7467, 7529, WithinSegment, Roll),            // len: 62m
			(7529, 8061, OutsideSegment, Roll),           // len: 532m
			(8061, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3862, OutsideSegment, Accelerate),        // len: 3862m
			(3862, 4842, WithinSegment, Accelerate),      // len: 980m
			(4842, 5949, UseCase1, Roll),                 // len: 1107m
			(5949, 6166, WithinSegment, Coast),           // len: 217m
			(6166, 7143, WithinSegment, Brake),           // len: 977m
			(7143, 7266, WithinSegment, Coast),           // len: 123m
			(7266, 7556, OutsideSegment, Coast),          // len: 290m
			(7556, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1809, WithinSegment, Accelerate),       // len: 1179m
			(1809, 2502, UseCase1, Roll),                 // len: 693m
			(2502, 2538, OutsideSegment, Coast),          // len: 36m
			(2538, 3926, OutsideSegment, Accelerate),     // len: 1388m
			(3926, 4859, WithinSegment, Accelerate),      // len: 933m
			(4859, 6020, UseCase1, Roll),                 // len: 1161m
			(6020, 6310, WithinSegment, Coast),           // len: 290m
			(6310, 6669, WithinSegment, Brake),           // len: 359m
			(6669, 6792, WithinSegment, Coast),           // len: 123m
			(6792, 7070, OutsideSegment, Coast),          // len: 278m
			(7070, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2019, WithinSegment, Accelerate),       // len: 1365m
			(2019, 2411, UseCase1, Roll),                 // len: 392m
			(2411, 2959, WithinSegment, Accelerate),      // len: 548m
			(2959, 3735, UseCase1, Roll),                 // len: 776m
			(3735, 4048, WithinSegment, Coast),           // len: 313m
			(4048, 4251, OutsideSegment, Coast),          // len: 203m
			(4251, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2019, WithinSegment, Accelerate),       // len: 1354m
			(2019, 3273, UseCase1, Roll),                 // len: 1254m
			(3273, 3332, WithinSegment, Coast),           // len: 59m
			(3332, 3344, WithinSegment, Roll),            // len: 12m
			(3344, 3461, OutsideSegment, Roll),           // len: 117m
			(3461, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 5017, WithinSegment, Accelerate),      // len: 1097m
			(5017, 5786, UseCase1, Roll),                 // len: 769m
			(5786, 6089, WithinSegment, Coast),           // len: 303m
			(6089, 6534, WithinSegment, Brake),           // len: 445m
			(6534, 6866, WithinSegment, Coast),           // len: 332m
			(6866, 7262, OutsideSegment, Coast),          // len: 396m
			(7262, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3734, OutsideSegment, Accelerate),        // len: 3734m
			(3734, 4644, WithinSegment, Accelerate),      // len: 910m
			(4644, 5866, UseCase1, Roll),                 // len: 1222m
			(5866, 5937, WithinSegment, Coast),           // len: 71m
			(5937, 6079, WithinSegment, Roll),            // len: 142m
			(6079, 6280, OutsideSegment, Roll),           // len: 201m
			(6280, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4472, WithinSegment, Accelerate),      // len: 925m
			(4472, 4493, WithinSegment, Roll),            // len: 21m
			(4493, 4686, WithinSegment, Accelerate),      // len: 193m
			(4686, 4701, WithinSegment, Roll),            // len: 15m
			(4701, 4758, WithinSegment, Accelerate),      // len: 57m
			(4758, 4770, WithinSegment, Roll),            // len: 12m
			(4770, 4821, WithinSegment, Accelerate),      // len: 51m
			(4821, 4831, WithinSegment, Roll),            // len: 10m
			(4831, 4998, WithinSegment, Accelerate),      // len: 167m
			(4998, 5008, WithinSegment, Roll),            // len: 10m
			(5008, 5048, WithinSegment, Accelerate),      // len: 40m
			(5048, 5061, WithinSegment, Roll),            // len: 13m
			(5061, 5095, WithinSegment, Accelerate),      // len: 34m
			(5095, 5110, WithinSegment, Roll),            // len: 15m
			(5110, 5148, WithinSegment, Accelerate),      // len: 38m
			(5148, 5164, WithinSegment, Roll),            // len: 16m
			(5164, 5353, WithinSegment, Accelerate),      // len: 189m
			(5353, 5624, UseCase2, Roll),                 // len: 271m
			(5624, 5842, WithinSegment, Coast),           // len: 218m
			(5842, 6101, WithinSegment, Brake),           // len: 259m
			(6101, 6212, WithinSegment, Coast),           // len: 111m
			(6212, 6502, OutsideSegment, Coast),          // len: 290m
			(6502, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 19, OutsideSegment, Roll),               // len: 4m
			(19, 37, OutsideSegment, Accelerate),         // len: 18m
			(37, 44, OutsideSegment, Roll),               // len: 7m
			(44, 88, OutsideSegment, Accelerate),         // len: 44m
			(88, 98, OutsideSegment, Roll),               // len: 10m
			(98, 164, OutsideSegment, Accelerate),        // len: 66m
			(164, 178, OutsideSegment, Roll),             // len: 14m
			(178, 260, OutsideSegment, Accelerate),       // len: 82m
			(260, 276, OutsideSegment, Roll),             // len: 16m
			(276, 427, OutsideSegment, Accelerate),       // len: 151m
			(427, 446, OutsideSegment, Roll),             // len: 19m
			(446, 3411, OutsideSegment, Accelerate),      // len: 2965m
			(3411, 3450, OutsideSegment, Coast),          // len: 39m
			(3450, 3876, OutsideSegment, Roll),           // len: 426m
			(3876, 5205, OutsideSegment, Brake),          // len: 1329m
			(5205, 5524, OutsideSegment, Roll),           // len: 319m
			(5524, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 19, OutsideSegment, Roll),               // len: 4m
			(19, 37, OutsideSegment, Accelerate),         // len: 18m
			(37, 44, OutsideSegment, Roll),               // len: 7m
			(44, 88, OutsideSegment, Accelerate),         // len: 44m
			(88, 98, OutsideSegment, Roll),               // len: 10m
			(98, 164, OutsideSegment, Accelerate),        // len: 66m
			(164, 178, OutsideSegment, Roll),             // len: 14m
			(178, 260, OutsideSegment, Accelerate),       // len: 82m
			(260, 276, OutsideSegment, Roll),             // len: 16m
			(276, 427, OutsideSegment, Accelerate),       // len: 151m
			(427, 446, OutsideSegment, Roll),             // len: 19m
			(446, 3421, OutsideSegment, Accelerate),      // len: 2975m
			(3421, 3470, OutsideSegment, Coast),          // len: 49m
			(3470, 3844, OutsideSegment, Roll),           // len: 374m
			(3844, 4508, OutsideSegment, Brake),          // len: 664m
			(4508, 4787, OutsideSegment, Roll),           // len: 279m
			(4787, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4037, OutsideSegment, Accelerate),        // len: 4037m
			(4037, 5344, WithinSegment, Accelerate),      // len: 1307m
			(5344, 5899, UseCase1, Roll),                 // len: 555m
			(5899, 5922, OutsideSegment, Coast),          // len: 23m
			(5922, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4387, OutsideSegment, Accelerate),        // len: 4387m
			(4387, 5274, WithinSegment, Accelerate),      // len: 887m
			(5274, 6708, UseCase1, Roll),                 // len: 1434m
			(6708, 6779, WithinSegment, Coast),           // len: 71m
			(6779, 7240, WithinSegment, Roll),            // len: 461m
			(7240, 7475, WithinSegment, Brake),           // len: 235m
			(7475, 7524, WithinSegment, Roll),            // len: 49m
			(7524, 8069, OutsideSegment, Roll),           // len: 545m
			(8069, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3862, OutsideSegment, Accelerate),        // len: 3862m
			(3862, 4842, WithinSegment, Accelerate),      // len: 980m
			(4842, 5939, UseCase1, Roll),                 // len: 1097m
			(5939, 6156, WithinSegment, Coast),           // len: 217m
			(6156, 7145, WithinSegment, Brake),           // len: 989m
			(7145, 7269, WithinSegment, Coast),           // len: 124m
			(7269, 7570, OutsideSegment, Coast),          // len: 301m
			(7570, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1797, WithinSegment, Accelerate),       // len: 1167m
			(1797, 2500, UseCase1, Roll),                 // len: 703m
			(2500, 2536, OutsideSegment, Coast),          // len: 36m
			(2536, 3924, OutsideSegment, Accelerate),     // len: 1388m
			(3924, 4869, WithinSegment, Accelerate),      // len: 945m
			(4869, 6000, UseCase1, Roll),                 // len: 1131m
			(6000, 6290, WithinSegment, Coast),           // len: 290m
			(6290, 6674, WithinSegment, Brake),           // len: 384m
			(6674, 6797, WithinSegment, Coast),           // len: 123m
			(6797, 7086, OutsideSegment, Coast),          // len: 289m
			(7086, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2019, WithinSegment, Accelerate),       // len: 1365m
			(2019, 2411, UseCase1, Roll),                 // len: 392m
			(2411, 2960, WithinSegment, Accelerate),      // len: 549m
			(2960, 3725, UseCase1, Roll),                 // len: 765m
			(3725, 4050, WithinSegment, Coast),           // len: 325m
			(4050, 4277, OutsideSegment, Coast),          // len: 227m
			(4277, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2019, WithinSegment, Accelerate),       // len: 1354m
			(2019, 3228, UseCase1, Roll),                 // len: 1209m
			(3228, 3299, WithinSegment, Coast),           // len: 71m
			(3299, 3346, WithinSegment, Roll),            // len: 47m
			(3346, 3487, OutsideSegment, Roll),           // len: 141m
			(3487, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 5017, WithinSegment, Accelerate),      // len: 1097m
			(5017, 5775, UseCase1, Roll),                 // len: 758m
			(5775, 6078, WithinSegment, Coast),           // len: 303m
			(6078, 6548, WithinSegment, Brake),           // len: 470m
			(6548, 6868, WithinSegment, Coast),           // len: 320m
			(6868, 7287, OutsideSegment, Coast),          // len: 419m
			(7287, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3734, OutsideSegment, Accelerate),        // len: 3734m
			(3734, 4644, WithinSegment, Accelerate),      // len: 910m
			(4644, 5844, UseCase1, Roll),                 // len: 1200m
			(5844, 5915, WithinSegment, Coast),           // len: 71m
			(5915, 6082, WithinSegment, Roll),            // len: 167m
			(6082, 6296, OutsideSegment, Roll),           // len: 214m
			(6296, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4413, WithinSegment, Accelerate),      // len: 866m
			(4413, 4434, WithinSegment, Roll),            // len: 21m
			(4434, 4627, WithinSegment, Accelerate),      // len: 193m
			(4627, 4642, WithinSegment, Roll),            // len: 15m
			(4642, 4692, WithinSegment, Accelerate),      // len: 50m
			(4692, 4705, WithinSegment, Roll),            // len: 13m
			(4705, 4761, WithinSegment, Accelerate),      // len: 56m
			(4761, 4771, WithinSegment, Roll),            // len: 10m
			(4771, 4810, WithinSegment, Accelerate),      // len: 39m
			(4810, 4817, WithinSegment, Roll),            // len: 7m
			(4817, 4950, WithinSegment, Accelerate),      // len: 133m
			(4950, 4958, WithinSegment, Roll),            // len: 8m
			(4958, 5023, WithinSegment, Accelerate),      // len: 65m
			(5023, 5033, WithinSegment, Roll),            // len: 10m
			(5033, 5062, WithinSegment, Accelerate),      // len: 29m
			(5062, 5074, WithinSegment, Roll),            // len: 12m
			(5074, 5107, WithinSegment, Accelerate),      // len: 33m
			(5107, 5122, WithinSegment, Roll),            // len: 15m
			(5122, 5167, WithinSegment, Accelerate),      // len: 45m
			(5167, 5183, WithinSegment, Roll),            // len: 16m
			(5183, 5371, WithinSegment, Accelerate),      // len: 188m
			(5371, 5652, UseCase2, Roll),                 // len: 281m
			(5652, 5895, WithinSegment, Coast),           // len: 243m
			(5895, 6105, WithinSegment, Brake),           // len: 210m
			(6105, 6216, WithinSegment, Coast),           // len: 111m
			(6216, 6506, OutsideSegment, Coast),          // len: 290m
			(6506, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 19, OutsideSegment, Roll),               // len: 4m
			(19, 34, OutsideSegment, Accelerate),         // len: 15m
			(34, 40, OutsideSegment, Roll),               // len: 6m
			(40, 59, OutsideSegment, Accelerate),         // len: 19m
			(59, 68, OutsideSegment, Roll),               // len: 9m
			(68, 101, OutsideSegment, Accelerate),        // len: 33m
			(101, 112, OutsideSegment, Roll),             // len: 11m
			(112, 185, OutsideSegment, Accelerate),       // len: 73m
			(185, 198, OutsideSegment, Roll),             // len: 13m
			(198, 346, OutsideSegment, Accelerate),       // len: 148m
			(346, 363, OutsideSegment, Roll),             // len: 17m
			(363, 492, OutsideSegment, Accelerate),       // len: 129m
			(492, 511, OutsideSegment, Roll),             // len: 19m
			(511, 3409, OutsideSegment, Accelerate),      // len: 2898m
			(3409, 3457, OutsideSegment, Coast),          // len: 48m
			(3457, 3854, OutsideSegment, Roll),           // len: 397m
			(3854, 5213, OutsideSegment, Brake),          // len: 1359m
			(5213, 5233, OutsideSegment, Roll),           // len: 20m
			(5233, 5354, OutsideSegment, Coast),          // len: 121m
			(5354, 5374, OutsideSegment, Roll),           // len: 20m
			(5374, 5493, OutsideSegment, Coast),          // len: 119m
			(5493, 5513, OutsideSegment, Roll),           // len: 20m
			(5513, 5629, OutsideSegment, Accelerate),     // len: 116m
			(5629, 5649, OutsideSegment, Roll),           // len: 20m
			(5649, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 19, OutsideSegment, Roll),               // len: 4m
			(19, 34, OutsideSegment, Accelerate),         // len: 15m
			(34, 40, OutsideSegment, Roll),               // len: 6m
			(40, 59, OutsideSegment, Accelerate),         // len: 19m
			(59, 68, OutsideSegment, Roll),               // len: 9m
			(68, 101, OutsideSegment, Accelerate),        // len: 33m
			(101, 112, OutsideSegment, Roll),             // len: 11m
			(112, 185, OutsideSegment, Accelerate),       // len: 73m
			(185, 198, OutsideSegment, Roll),             // len: 13m
			(198, 346, OutsideSegment, Accelerate),       // len: 148m
			(346, 363, OutsideSegment, Roll),             // len: 17m
			(363, 492, OutsideSegment, Accelerate),       // len: 129m
			(492, 511, OutsideSegment, Roll),             // len: 19m
			(511, 3418, OutsideSegment, Accelerate),      // len: 2907m
			(3418, 3467, OutsideSegment, Coast),          // len: 49m
			(3467, 3842, OutsideSegment, Roll),           // len: 375m
			(3842, 4506, OutsideSegment, Brake),          // len: 664m
			(4506, 4526, OutsideSegment, Roll),           // len: 20m
			(4526, 4646, OutsideSegment, Coast),          // len: 120m
			(4646, 4666, OutsideSegment, Roll),           // len: 20m
			(4666, 4775, OutsideSegment, Coast),          // len: 109m
			(4775, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4037, OutsideSegment, Accelerate),        // len: 4037m
			(4037, 5344, WithinSegment, Accelerate),      // len: 1307m
			(5344, 5899, UseCase1, Roll),                 // len: 555m
			(5899, 5922, OutsideSegment, Coast),          // len: 23m
			(5922, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4387, OutsideSegment, Accelerate),        // len: 4387m
			(4387, 5274, WithinSegment, Accelerate),      // len: 887m
			(5274, 6709, UseCase1, Roll),                 // len: 1435m
			(6709, 6780, WithinSegment, Coast),           // len: 71m
			(6780, 7265, WithinSegment, Roll),            // len: 485m
			(7265, 7277, WithinSegment, Brake),           // len: 12m
			(7277, 7522, WithinSegment, Coast),           // len: 245m
			(7522, 7797, OutsideSegment, Coast),          // len: 275m
			(7797, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3862, OutsideSegment, Accelerate),        // len: 3862m
			(3862, 4842, WithinSegment, Accelerate),      // len: 980m
			(4842, 5939, UseCase1, Roll),                 // len: 1097m
			(5939, 6181, WithinSegment, Coast),           // len: 242m
			(6181, 7083, WithinSegment, Brake),           // len: 902m
			(7083, 7268, WithinSegment, Coast),           // len: 185m
			(7268, 7520, OutsideSegment, Coast),          // len: 252m
			(7520, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1809, WithinSegment, Accelerate),       // len: 1179m
			(1809, 2480, UseCase1, Roll),                 // len: 671m
			(2480, 2503, WithinSegment, Coast),           // len: 23m
			(2503, 2538, OutsideSegment, Coast),          // len: 35m
			(2538, 3927, OutsideSegment, Accelerate),     // len: 1389m
			(3927, 4860, WithinSegment, Accelerate),      // len: 933m
			(4860, 6012, UseCase1, Roll),                 // len: 1152m
			(6012, 6376, WithinSegment, Coast),           // len: 364m
			(6376, 6623, WithinSegment, Brake),           // len: 247m
			(6623, 6795, WithinSegment, Coast),           // len: 172m
			(6795, 7036, OutsideSegment, Coast),          // len: 241m
			(7036, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2019, WithinSegment, Accelerate),       // len: 1365m
			(2019, 2411, UseCase1, Roll),                 // len: 392m
			(2411, 2423, WithinSegment, Coast),           // len: 12m
			(2423, 2959, WithinSegment, Accelerate),      // len: 536m
			(2959, 3724, UseCase1, Roll),                 // len: 765m
			(3724, 3940, WithinSegment, Coast),           // len: 216m
			(3940, 4049, WithinSegment, Roll),            // len: 109m
			(4049, 4371, OutsideSegment, Roll),           // len: 322m
			(4371, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2019, WithinSegment, Accelerate),       // len: 1354m
			(2019, 3229, UseCase1, Roll),                 // len: 1210m
			(3229, 3288, WithinSegment, Coast),           // len: 59m
			(3288, 3347, WithinSegment, Roll),            // len: 59m
			(3347, 3488, OutsideSegment, Roll),           // len: 141m
			(3488, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 5017, WithinSegment, Accelerate),      // len: 1097m
			(5017, 5775, UseCase1, Roll),                 // len: 758m
			(5775, 6225, WithinSegment, Coast),           // len: 450m
			(6225, 6398, WithinSegment, Brake),           // len: 173m
			(6398, 6864, WithinSegment, Coast),           // len: 466m
			(6864, 7137, OutsideSegment, Coast),          // len: 273m
			(7137, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3734, OutsideSegment, Accelerate),        // len: 3734m
			(3734, 4644, WithinSegment, Accelerate),      // len: 910m
			(4644, 5845, UseCase1, Roll),                 // len: 1201m
			(5845, 5916, WithinSegment, Coast),           // len: 71m
			(5916, 6083, WithinSegment, Roll),            // len: 167m
			(6083, 6296, OutsideSegment, Roll),           // len: 213m
			(6296, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4419, WithinSegment, Accelerate),      // len: 872m
			(4419, 4439, WithinSegment, Roll),            // len: 20m
			(4439, 4744, WithinSegment, Accelerate),      // len: 305m
			(4744, 4757, WithinSegment, Roll),            // len: 13m
			(4757, 4782, WithinSegment, Accelerate),      // len: 25m
			(4782, 4794, WithinSegment, Roll),            // len: 12m
			(4794, 5003, WithinSegment, Accelerate),      // len: 209m
			(5003, 5015, WithinSegment, Roll),            // len: 12m
			(5015, 5048, WithinSegment, Accelerate),      // len: 33m
			(5048, 5062, WithinSegment, Roll),            // len: 14m
			(5062, 5311, WithinSegment, Accelerate),      // len: 249m
			(5311, 5569, UseCase2, Roll),                 // len: 258m
			(5569, 5812, WithinSegment, Coast),           // len: 243m
			(5812, 6059, WithinSegment, Brake),           // len: 247m
			(6059, 6219, WithinSegment, Coast),           // len: 160m
			(6219, 6459, OutsideSegment, Coast),          // len: 240m
			(6459, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 20, OutsideSegment, Roll),               // len: 5m
			(20, 35, OutsideSegment, Accelerate),         // len: 15m
			(35, 42, OutsideSegment, Roll),               // len: 7m
			(42, 105, OutsideSegment, Accelerate),        // len: 63m
			(105, 117, OutsideSegment, Roll),             // len: 12m
			(117, 149, OutsideSegment, Accelerate),       // len: 32m
			(149, 162, OutsideSegment, Roll),             // len: 13m
			(162, 368, OutsideSegment, Accelerate),       // len: 206m
			(368, 387, OutsideSegment, Roll),             // len: 19m
			(387, 3411, OutsideSegment, Accelerate),      // len: 3024m
			(3411, 3450, OutsideSegment, Coast),          // len: 39m
			(3450, 3865, OutsideSegment, Roll),           // len: 415m
			(3865, 5044, OutsideSegment, Brake),          // len: 1179m
			(5044, 5432, OutsideSegment, Coast),          // len: 388m
			(5432, 5452, OutsideSegment, Roll),           // len: 20m
			(5452, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(4, 6, OutsideSegment, Roll),                 // len: 2m
			(6, 15, OutsideSegment, Accelerate),          // len: 9m
			(15, 20, OutsideSegment, Roll),               // len: 5m
			(20, 35, OutsideSegment, Accelerate),         // len: 15m
			(35, 42, OutsideSegment, Roll),               // len: 7m
			(42, 105, OutsideSegment, Accelerate),        // len: 63m
			(105, 117, OutsideSegment, Roll),             // len: 12m
			(117, 149, OutsideSegment, Accelerate),       // len: 32m
			(149, 162, OutsideSegment, Roll),             // len: 13m
			(162, 368, OutsideSegment, Accelerate),       // len: 206m
			(368, 387, OutsideSegment, Roll),             // len: 19m
			(387, 3469, OutsideSegment, Accelerate),      // len: 3082m
			(3469, 3843, OutsideSegment, Roll),           // len: 374m
			(3843, 4508, OutsideSegment, Brake),          // len: 665m
			(4508, 4528, OutsideSegment, Roll),           // len: 20m
			(4528, 4727, OutsideSegment, Coast),          // len: 199m
			(4727, 4746, OutsideSegment, Roll),           // len: 19m
			(4746, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5531, OutsideSegment, Accelerate),        // len: 5531m
			(5531, 6038, OutsideSegment, Coast),          // len: 507m
			(6038, 6085, OutsideSegment, Roll),           // len: 47m
			(6085, 6096, OutsideSegment, Accelerate),     // len: 11m
			(6096, 6120, OutsideSegment, Roll),           // len: 24m
			(6120, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5892, OutsideSegment, Accelerate),        // len: 5892m
			(5892, 6387, OutsideSegment, Coast),          // len: 495m
			(6387, 7481, OutsideSegment, Brake),          // len: 1094m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5367, OutsideSegment, Accelerate),        // len: 5367m
			(5367, 5685, OutsideSegment, Coast),          // len: 318m
			(5685, 7247, OutsideSegment, Brake),          // len: 1562m
			(7247, 7509, OutsideSegment, Coast),          // len: 262m
			(7509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_EcoRollWithoutEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2525, OutsideSegment, Coast),          // len: 366m
			(2525, 3284, OutsideSegment, Accelerate),     // len: 759m
			(3284, 3508, OutsideSegment, Coast),          // len: 224m
			(3508, 4024, OutsideSegment, Brake),          // len: 516m
			(4024, 4286, OutsideSegment, Coast),          // len: 262m
			(4286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2608, OutsideSegment, Coast),          // len: 449m
			(2608, 2655, OutsideSegment, Roll),           // len: 47m
			(2655, 2667, OutsideSegment, Accelerate),     // len: 12m
			(2667, 2690, OutsideSegment, Roll),           // len: 23m
			(2690, 2888, OutsideSegment, Accelerate),     // len: 198m
			(2888, 3442, OutsideSegment, Coast),          // len: 554m
			(3442, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5414, OutsideSegment, Accelerate),        // len: 5414m
			(5414, 5567, OutsideSegment, Coast),          // len: 153m
			(5567, 6817, OutsideSegment, Brake),          // len: 1250m
			(6817, 7305, OutsideSegment, Coast),          // len: 488m
			(7305, 7352, OutsideSegment, Roll),           // len: 47m
			(7352, 7363, OutsideSegment, Accelerate),     // len: 11m
			(7363, 7387, OutsideSegment, Roll),           // len: 24m
			(7387, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5227, OutsideSegment, Accelerate),        // len: 5227m
			(5227, 5522, OutsideSegment, Coast),          // len: 295m
			(5522, 6063, OutsideSegment, Brake),          // len: 541m
			(6063, 6289, OutsideSegment, Coast),          // len: 226m
			(6289, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4472, OutsideSegment, Accelerate),        // len: 4472m
			(5152, 5423, OutsideSegment, Accelerate),     // len: 271m
			(5506, 6119, OutsideSegment, Brake),          // len: 613m
			(6119, 6381, OutsideSegment, Coast),          // len: 262m
			(6381, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5479, OutsideSegment, Coast),          // len: 249m
			(5479, 5489, OutsideSegment, Accelerate),     // len: 10m
			(5489, 5509, OutsideSegment, Roll),           // len: 20m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5531, OutsideSegment, Accelerate),        // len: 5531m
			(5531, 6038, OutsideSegment, Coast),          // len: 507m
			(6038, 6085, OutsideSegment, Roll),           // len: 47m
			(6085, 6096, OutsideSegment, Accelerate),     // len: 11m
			(6096, 6120, OutsideSegment, Roll),           // len: 24m
			(6120, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5892, OutsideSegment, Accelerate),        // len: 5892m
			(5892, 6387, OutsideSegment, Coast),          // len: 495m
			(6387, 7481, OutsideSegment, Brake),          // len: 1094m
			(7481, 7874, OutsideSegment, Coast),          // len: 393m
			(7874, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5367, OutsideSegment, Accelerate),        // len: 5367m
			(5367, 5685, OutsideSegment, Coast),          // len: 318m
			(5685, 7247, OutsideSegment, Brake),          // len: 1562m
			(7247, 7509, OutsideSegment, Coast),          // len: 262m
			(7509, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2525, OutsideSegment, Coast),          // len: 366m
			(2525, 3284, OutsideSegment, Accelerate),     // len: 759m
			(3284, 3508, OutsideSegment, Coast),          // len: 224m
			(3508, 4024, OutsideSegment, Brake),          // len: 516m
			(4024, 4286, OutsideSegment, Coast),          // len: 262m
			(4286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2159, OutsideSegment, Accelerate),        // len: 2159m
			(2159, 2608, OutsideSegment, Coast),          // len: 449m
			(2608, 2655, OutsideSegment, Roll),           // len: 47m
			(2655, 2667, OutsideSegment, Accelerate),     // len: 12m
			(2667, 2690, OutsideSegment, Roll),           // len: 23m
			(2690, 2888, OutsideSegment, Accelerate),     // len: 198m
			(2888, 3442, OutsideSegment, Coast),          // len: 554m
			(3442, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5414, OutsideSegment, Accelerate),        // len: 5414m
			(5414, 5567, OutsideSegment, Coast),          // len: 153m
			(5567, 6817, OutsideSegment, Brake),          // len: 1250m
			(6817, 7305, OutsideSegment, Coast),          // len: 488m
			(7305, 7352, OutsideSegment, Roll),           // len: 47m
			(7352, 7363, OutsideSegment, Accelerate),     // len: 11m
			(7363, 7387, OutsideSegment, Roll),           // len: 24m
			(7387, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5227, OutsideSegment, Accelerate),        // len: 5227m
			(5227, 5522, OutsideSegment, Coast),          // len: 295m
			(5522, 6063, OutsideSegment, Brake),          // len: 541m
			(6063, 6289, OutsideSegment, Coast),          // len: 226m
			(6289, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4472, OutsideSegment, Accelerate),        // len: 4472m
			(5152, 5423, OutsideSegment, Accelerate),     // len: 271m
			(5506, 6119, OutsideSegment, Brake),          // len: 613m
			(6119, 6381, OutsideSegment, Coast),          // len: 262m
			(6381, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3870, OutsideSegment, Coast),          // len: 464m
			(3870, 5210, OutsideSegment, Brake),          // len: 1340m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5479, OutsideSegment, Coast),          // len: 249m
			(5479, 5489, OutsideSegment, Accelerate),     // len: 10m
			(5489, 5509, OutsideSegment, Roll),           // len: 20m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4037, OutsideSegment, Accelerate),        // len: 4037m
			(4037, 5344, WithinSegment, Accelerate),      // len: 1307m
			(5344, 5899, UseCase1, Roll),                 // len: 555m
			(5899, 5923, OutsideSegment, Coast),          // len: 24m
			(5923, 5993, OutsideSegment, Roll),           // len: 70m
			(5993, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4387, OutsideSegment, Accelerate),        // len: 4387m
			(4387, 5274, WithinSegment, Accelerate),      // len: 887m
			(5274, 6709, UseCase1, Roll),                 // len: 1435m
			(6709, 6768, WithinSegment, Coast),           // len: 59m
			(6768, 7157, WithinSegment, Roll),            // len: 389m
			(7157, 7466, WithinSegment, Brake),           // len: 309m
			(7466, 7528, WithinSegment, Coast),           // len: 62m
			(7528, 8025, OutsideSegment, Coast),          // len: 497m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3862, OutsideSegment, Accelerate),        // len: 3862m
			(3862, 4842, WithinSegment, Accelerate),      // len: 980m
			(4842, 5939, UseCase1, Roll),                 // len: 1097m
			(5939, 6157, WithinSegment, Coast),           // len: 218m
			(6157, 7134, WithinSegment, Brake),           // len: 977m
			(7134, 7269, WithinSegment, Coast),           // len: 135m
			(7269, 7559, OutsideSegment, Coast),          // len: 290m
			(7559, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1809, WithinSegment, Accelerate),       // len: 1179m
			(1809, 2480, UseCase1, Roll),                 // len: 671m
			(2480, 2503, WithinSegment, Coast),           // len: 23m
			(2503, 2538, OutsideSegment, Coast),          // len: 35m
			(2538, 3927, OutsideSegment, Accelerate),     // len: 1389m
			(3927, 4860, WithinSegment, Accelerate),      // len: 933m
			(4860, 6012, UseCase1, Roll),                 // len: 1152m
			(6012, 6291, WithinSegment, Coast),           // len: 279m
			(6291, 6674, WithinSegment, Brake),           // len: 383m
			(6674, 6797, WithinSegment, Coast),           // len: 123m
			(6797, 7075, OutsideSegment, Coast),          // len: 278m
			(7075, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2019, WithinSegment, Accelerate),       // len: 1365m
			(2019, 2411, UseCase1, Roll),                 // len: 392m
			(2411, 2960, WithinSegment, Accelerate),      // len: 549m
			(2960, 3725, UseCase1, Roll),                 // len: 765m
			(3725, 4051, WithinSegment, Coast),           // len: 326m
			(4051, 4265, OutsideSegment, Coast),          // len: 214m
			(4265, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2019, WithinSegment, Accelerate),       // len: 1354m
			(2019, 3229, UseCase1, Roll),                 // len: 1210m
			(3229, 3300, WithinSegment, Coast),           // len: 71m
			(3300, 3347, WithinSegment, Roll),            // len: 47m
			(3347, 3488, OutsideSegment, Roll),           // len: 141m
			(3488, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 5017, WithinSegment, Accelerate),      // len: 1097m
			(5017, 5775, UseCase1, Roll),                 // len: 758m
			(5775, 6078, WithinSegment, Coast),           // len: 303m
			(6078, 6536, WithinSegment, Brake),           // len: 458m
			(6536, 6868, WithinSegment, Coast),           // len: 332m
			(6868, 7263, OutsideSegment, Coast),          // len: 395m
			(7263, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3734, OutsideSegment, Accelerate),        // len: 3734m
			(3734, 4644, WithinSegment, Accelerate),      // len: 910m
			(4644, 5845, UseCase1, Roll),                 // len: 1201m
			(5845, 5916, WithinSegment, Coast),           // len: 71m
			(5916, 6083, WithinSegment, Roll),            // len: 167m
			(6083, 6308, OutsideSegment, Roll),           // len: 225m
			(6308, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4472, WithinSegment, Accelerate),      // len: 925m
			(5152, 5333, WithinSegment, Accelerate),      // len: 181m
			(5333, 5582, UseCase2, Roll),                 // len: 249m
			(5582, 5788, WithinSegment, Coast),           // len: 206m
			(5788, 6109, WithinSegment, Brake),           // len: 321m
			(6109, 6220, WithinSegment, Coast),           // len: 111m
			(6220, 6497, OutsideSegment, Coast),          // len: 277m
			(6497, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3454, OutsideSegment, Coast),          // len: 48m
			(3454, 3851, OutsideSegment, Roll),           // len: 397m
			(3851, 5210, OutsideSegment, Brake),          // len: 1359m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5480, OutsideSegment, Coast),          // len: 250m
			(5480, 5490, OutsideSegment, Accelerate),     // len: 10m
			(5490, 5509, OutsideSegment, Roll),           // len: 19m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3464, OutsideSegment, Coast),          // len: 49m
			(3464, 3838, OutsideSegment, Roll),           // len: 374m
			(3838, 4513, OutsideSegment, Brake),          // len: 675m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4037, OutsideSegment, Accelerate),        // len: 4037m
			(4037, 5344, WithinSegment, Accelerate),      // len: 1307m
			(5344, 5899, UseCase1, Roll),                 // len: 555m
			(5899, 5923, OutsideSegment, Coast),          // len: 24m
			(5923, 5993, OutsideSegment, Roll),           // len: 70m
			(5993, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4387, OutsideSegment, Accelerate),        // len: 4387m
			(4387, 5274, WithinSegment, Accelerate),      // len: 887m
			(5274, 6709, UseCase1, Roll),                 // len: 1435m
			(6709, 6768, WithinSegment, Coast),           // len: 59m
			(6768, 7157, WithinSegment, Roll),            // len: 389m
			(7157, 7466, WithinSegment, Brake),           // len: 309m
			(7466, 7528, WithinSegment, Coast),           // len: 62m
			(7528, 8025, OutsideSegment, Coast),          // len: 497m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3862, OutsideSegment, Accelerate),        // len: 3862m
			(3862, 4842, WithinSegment, Accelerate),      // len: 980m
			(4842, 5939, UseCase1, Roll),                 // len: 1097m
			(5939, 6157, WithinSegment, Coast),           // len: 218m
			(6157, 7134, WithinSegment, Brake),           // len: 977m
			(7134, 7269, WithinSegment, Coast),           // len: 135m
			(7269, 7559, OutsideSegment, Coast),          // len: 290m
			(7559, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1809, WithinSegment, Accelerate),       // len: 1179m
			(1809, 2480, UseCase1, Roll),                 // len: 671m
			(2480, 2503, WithinSegment, Coast),           // len: 23m
			(2503, 2538, OutsideSegment, Coast),          // len: 35m
			(2538, 3927, OutsideSegment, Accelerate),     // len: 1389m
			(3927, 4860, WithinSegment, Accelerate),      // len: 933m
			(4860, 6012, UseCase1, Roll),                 // len: 1152m
			(6012, 6291, WithinSegment, Coast),           // len: 279m
			(6291, 6674, WithinSegment, Brake),           // len: 383m
			(6674, 6797, WithinSegment, Coast),           // len: 123m
			(6797, 7075, OutsideSegment, Coast),          // len: 278m
			(7075, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2019, WithinSegment, Accelerate),       // len: 1365m
			(2019, 2411, UseCase1, Roll),                 // len: 392m
			(2411, 2960, WithinSegment, Accelerate),      // len: 549m
			(2960, 3725, UseCase1, Roll),                 // len: 765m
			(3725, 4051, WithinSegment, Coast),           // len: 326m
			(4051, 4265, OutsideSegment, Coast),          // len: 214m
			(4265, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2019, WithinSegment, Accelerate),       // len: 1354m
			(2019, 3229, UseCase1, Roll),                 // len: 1210m
			(3229, 3300, WithinSegment, Coast),           // len: 71m
			(3300, 3347, WithinSegment, Roll),            // len: 47m
			(3347, 3488, OutsideSegment, Roll),           // len: 141m
			(3488, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 5017, WithinSegment, Accelerate),      // len: 1097m
			(5017, 5775, UseCase1, Roll),                 // len: 758m
			(5775, 6078, WithinSegment, Coast),           // len: 303m
			(6078, 6536, WithinSegment, Brake),           // len: 458m
			(6536, 6868, WithinSegment, Coast),           // len: 332m
			(6868, 7263, OutsideSegment, Coast),          // len: 395m
			(7263, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3734, OutsideSegment, Accelerate),        // len: 3734m
			(3734, 4644, WithinSegment, Accelerate),      // len: 910m
			(4644, 5845, UseCase1, Roll),                 // len: 1201m
			(5845, 5916, WithinSegment, Coast),           // len: 71m
			(5916, 6083, WithinSegment, Roll),            // len: 167m
			(6083, 6308, OutsideSegment, Roll),           // len: 225m
			(6308, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4472, WithinSegment, Accelerate),      // len: 925m
			(5152, 5333, WithinSegment, Accelerate),      // len: 181m
			(5333, 5582, UseCase2, Roll),                 // len: 249m
			(5582, 5788, WithinSegment, Coast),           // len: 206m
			(5788, 6109, WithinSegment, Brake),           // len: 321m
			(6109, 6220, WithinSegment, Coast),           // len: 111m
			(6220, 6497, OutsideSegment, Coast),          // len: 277m
			(6497, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3406, OutsideSegment, Accelerate),      // len: 2995m
			(3406, 3454, OutsideSegment, Coast),          // len: 48m
			(3454, 3851, OutsideSegment, Roll),           // len: 397m
			(3851, 5210, OutsideSegment, Brake),          // len: 1359m
			(5210, 5230, OutsideSegment, Roll),           // len: 20m
			(5230, 5480, OutsideSegment, Coast),          // len: 250m
			(5480, 5490, OutsideSegment, Accelerate),     // len: 10m
			(5490, 5509, OutsideSegment, Roll),           // len: 19m
			(5509, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3464, OutsideSegment, Coast),          // len: 49m
			(3464, 3838, OutsideSegment, Roll),           // len: 374m
			(3838, 4513, OutsideSegment, Brake),          // len: 675m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));


		private void TestPCC(string jobName, string cycleName, params (double start, double end, DefaultDriverStrategy.PCCStates pcc, DrivingAction action)[] data)
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
			var pccStates = mod.Columns["PCCState"].Values<DefaultDriverStrategy.PCCStates>();
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

			var pccStates = pccCol.Values<DefaultDriverStrategy.PCCStates>();
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
		#endregion

	}
}
