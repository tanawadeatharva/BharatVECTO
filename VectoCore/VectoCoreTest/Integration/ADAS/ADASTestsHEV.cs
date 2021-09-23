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
			(4655, 5461, WithinSegment, Accelerate),      // len: 806m
			(5461, 7251, UseCase1, Coast),                // len: 1790m
			(7251, 7263, OutsideSegment, Accelerate),     // len: 12m
			(7263, 7650, OutsideSegment, Coast),          // len: 387m
			(7650, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4924, WithinSegment, Accelerate),      // len: 945m
			(4924, 6065, UseCase1, Coast),                // len: 1141m
			(6065, 6076, UseCase1, Roll),                 // len: 11m
			(6076, 6088, UseCase1, Brake),                // len: 12m
			(6088, 6112, WithinSegment, Roll),            // len: 24m
			(6112, 6258, WithinSegment, Coast),           // len: 146m
			(6258, 7136, WithinSegment, Brake),           // len: 878m
			(7136, 7160, WithinSegment, Coast),           // len: 24m
			(7160, 7549, OutsideSegment, Coast),          // len: 389m
			(7549, 1e6, OutsideSegment, Accelerate));

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
			(2557, 2627, WithinSegment, Accelerate),      // len: 70m
			(2627, 3269, UseCase1, Coast),                // len: 642m
			(3269, 3292, OutsideSegment, Accelerate),     // len: 23m
			(3292, 3339, OutsideSegment, Coast),          // len: 47m
			(3339, 1e6, OutsideSegment, Accelerate));

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
			(3815, 4830, WithinSegment, Accelerate),      // len: 1015m
			(4830, 5992, UseCase1, Coast),                // len: 1162m
			(5992, 6015, OutsideSegment, Accelerate),     // len: 23m
			(6015, 6074, OutsideSegment, Coast),          // len: 59m
			(6074, 1e6, OutsideSegment, Accelerate));

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
			(4655, 5461, WithinSegment, Accelerate),      // len: 806m
			(5461, 7251, UseCase1, Coast),                // len: 1790m
			(7251, 7263, OutsideSegment, Accelerate),     // len: 12m
			(7263, 7650, OutsideSegment, Coast),          // len: 387m
			(7650, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4924, WithinSegment, Accelerate),      // len: 945m
			(4924, 6065, UseCase1, Coast),                // len: 1141m
			(6065, 6076, UseCase1, Roll),                 // len: 11m
			(6076, 6088, UseCase1, Brake),                // len: 12m
			(6088, 6112, WithinSegment, Roll),            // len: 24m
			(6112, 6136, WithinSegment, Coast),           // len: 24m
			(6136, 7145, WithinSegment, Brake),           // len: 1009m
			(7145, 7157, WithinSegment, Coast),           // len: 12m
			(7157, 7431, OutsideSegment, Coast),          // len: 274m
			(7431, 1e6, OutsideSegment, Accelerate));

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
			(2557, 2627, WithinSegment, Accelerate),      // len: 70m
			(2627, 3269, UseCase1, Coast),                // len: 642m
			(3269, 3292, OutsideSegment, Accelerate),     // len: 23m
			(3292, 3339, OutsideSegment, Coast),          // len: 47m
			(3339, 1e6, OutsideSegment, Accelerate));


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
			(3815, 4830, WithinSegment, Accelerate),      // len: 1015m
			(4830, 5992, UseCase1, Coast),                // len: 1162m
			(5992, 6015, OutsideSegment, Accelerate),     // len: 23m
			(6015, 6074, OutsideSegment, Coast),          // len: 59m
			(6074, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4472, WithinSegment, Accelerate),      // len: 913m
			(5152, 5333, WithinSegment, Accelerate),      // len: 181m
			(5333, 5591, UseCase2, Coast),                // len: 258m
			(5591, 5603, UseCase2, Roll),                 // len: 12m
			(5603, 5615, UseCase2, Brake),                // len: 12m
			(5615, 5639, WithinSegment, Roll),            // len: 24m
			(5639, 5651, WithinSegment, Coast),           // len: 12m
			(5651, 6119, WithinSegment, Brake),           // len: 468m
			(6119, 6381, OutsideSegment, Coast),          // len: 262m
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
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6872, WithinSegment, Coast),           // len: 411m
			(6872, 7256, WithinSegment, Brake),           // len: 384m
			(7256, 7466, OutsideSegment, Brake),          // len: 210m
			(7466, 8025, OutsideSegment, Coast),          // len: 559m
			(8025, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 6047, WithinSegment, Coast),           // len: 169m
			(6047, 7160, WithinSegment, Brake),           // len: 1113m
			(7160, 7234, OutsideSegment, Brake),          // len: 74m
			(7234, 7647, OutsideSegment, Coast),          // len: 413m
			(7647, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P3_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P3_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2570, WithinSegment, Coast),           // len: 224m
			(2570, 2652, WithinSegment, Accelerate),      // len: 82m
			(2652, 3093, UseCase1, Coast),                // len: 441m
			(3093, 3272, WithinSegment, Coast),           // len: 179m
			(3272, 3559, OutsideSegment, Coast),          // len: 287m
			(3559, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5922, WithinSegment, Coast),           // len: 194m
			(5922, 6564, WithinSegment, Brake),           // len: 642m
			(6564, 6787, OutsideSegment, Brake),          // len: 223m
			(6787, 7539, OutsideSegment, Coast),          // len: 752m
			(7539, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5988, WithinSegment, Coast),           // len: 327m
			(5988, 6401, OutsideSegment, Coast),          // len: 413m
			(6401, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4472, WithinSegment, Accelerate),      // len: 913m
			//(4472, 4493, WithinSegment, Roll),            // len: 21m
			//(4493, 4686, WithinSegment, Accelerate),      // len: 193m
			//(4686, 4701, WithinSegment, Roll),            // len: 15m
			//(4701, 4758, WithinSegment, Accelerate),      // len: 57m
			//(4758, 4770, WithinSegment, Roll),            // len: 12m
			//(4770, 4821, WithinSegment, Accelerate),      // len: 51m
			//(4821, 4831, WithinSegment, Roll),            // len: 10m
			//(4831, 4998, WithinSegment, Accelerate),      // len: 167m
			//(4998, 5008, WithinSegment, Roll),            // len: 10m
			//(5008, 5048, WithinSegment, Accelerate),      // len: 40m
			//(5048, 5061, WithinSegment, Roll),            // len: 13m
			//(5061, 5095, WithinSegment, Accelerate),      // len: 34m
			//(5095, 5110, WithinSegment, Roll),            // len: 15m
			//(5110, 5148, WithinSegment, Accelerate),      // len: 38m
			//(5148, 5164, WithinSegment, Roll),            // len: 16m
			(5164, 5353, WithinSegment, Accelerate),      // len: 189m
			(5353, 5624, UseCase2, Coast),                // len: 271m
			(5624, 5781, WithinSegment, Coast),           // len: 157m
			(5781, 6115, WithinSegment, Brake),           // len: 334m
			(6115, 6189, OutsideSegment, Brake),          // len: 74m
			(6189, 6577, OutsideSegment, Coast),          // len: 388m
			(6577, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(446, 3411, OutsideSegment, Accelerate),      // len: 2965m
			(3411, 3876, OutsideSegment, Coast),          // len: 465m
			(3876, 5205, OutsideSegment, Brake),          // len: 1329m
			(5205, 5485, OutsideSegment, Coast),          // len: 280m
			(5485, 5505, OutsideSegment, Roll),           // len: 20m
			(5505, 5622, OutsideSegment, Accelerate),     // len: 117m
			(5622, 5641, OutsideSegment, Roll),           // len: 19m
			(5641, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(446, 3421, OutsideSegment, Accelerate),      // len: 2975m
			(3421, 3844, OutsideSegment, Coast),          // len: 423m
			(3844, 4508, OutsideSegment, Brake),          // len: 664m
			(4508, 4748, OutsideSegment, Coast),          // len: 240m
			(4748, 4767, OutsideSegment, Roll),           // len: 19m
			(4767, 4884, OutsideSegment, Accelerate),     // len: 117m
			(4884, 4903, OutsideSegment, Roll),           // len: 19m
			(4903, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5461, WithinSegment, Accelerate),      // len: 1331m
			(5461, 5765, UseCase1, Coast),                // len: 304m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6049, OutsideSegment, Coast),          // len: 237m
			(6049, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5472, WithinSegment, Accelerate),      // len: 817m
			(5472, 6459, UseCase1, Coast),                // len: 987m
			(6459, 6870, WithinSegment, Coast),           // len: 411m
			(6870, 7254, WithinSegment, Brake),           // len: 384m
			(7254, 7476, OutsideSegment, Brake),          // len: 222m
			(7476, 8047, OutsideSegment, Coast),          // len: 571m
			(8047, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5868, UseCase1, Coast),                // len: 932m
			(5868, 6037, WithinSegment, Coast),           // len: 169m
			(6037, 7150, WithinSegment, Brake),           // len: 1113m
			(7150, 7236, OutsideSegment, Brake),          // len: 86m
			(7236, 7649, OutsideSegment, Coast),          // len: 413m
			(7649, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P4_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P4_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P4_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5910, WithinSegment, Coast),           // len: 182m
			(5910, 6565, WithinSegment, Brake),           // len: 655m
			(6565, 6800, OutsideSegment, Brake),          // len: 235m
			(6800, 7564, OutsideSegment, Coast),          // len: 764m
			(7564, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4842, WithinSegment, Accelerate),      // len: 1027m
			(4842, 5660, UseCase1, Coast),                // len: 818m
			(5660, 6000, WithinSegment, Coast),           // len: 340m
			(6000, 6413, OutsideSegment, Coast),          // len: 413m
			(6413, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4413, WithinSegment, Accelerate),      // len: 854m
			//(4413, 4434, WithinSegment, Roll),            // len: 21m
			//(4434, 4627, WithinSegment, Accelerate),      // len: 193m
			//(4627, 4642, WithinSegment, Roll),            // len: 15m
			//(4642, 4692, WithinSegment, Accelerate),      // len: 50m
			//(4692, 4705, WithinSegment, Roll),            // len: 13m
			//(4705, 4761, WithinSegment, Accelerate),      // len: 56m
			//(4761, 4771, WithinSegment, Roll),            // len: 10m
			//(4771, 4810, WithinSegment, Accelerate),      // len: 39m
			//(4810, 4817, WithinSegment, Roll),            // len: 7m
			//(4817, 4950, WithinSegment, Accelerate),      // len: 133m
			//(4950, 4958, WithinSegment, Roll),            // len: 8m
			//(4958, 5023, WithinSegment, Accelerate),      // len: 65m
			//(5023, 5033, WithinSegment, Roll),            // len: 10m
			//(5033, 5062, WithinSegment, Accelerate),      // len: 29m
			//(5062, 5074, WithinSegment, Roll),            // len: 12m
			//(5074, 5107, WithinSegment, Accelerate),      // len: 33m
			//(5107, 5122, WithinSegment, Roll),            // len: 15m
			//(5122, 5167, WithinSegment, Accelerate),      // len: 45m
			//(5167, 5183, WithinSegment, Roll),            // len: 16m
			(5183, 5371, WithinSegment, Accelerate),      // len: 188m
			(5371, 5652, UseCase2, Coast),                // len: 281m
			(5652, 5810, WithinSegment, Coast),           // len: 158m
			(5810, 6119, WithinSegment, Brake),           // len: 309m
			(6119, 6193, OutsideSegment, Brake),          // len: 74m
			(6193, 6593, OutsideSegment, Coast),          // len: 400m
			(6593, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(511, 3409, OutsideSegment, Accelerate),      // len: 2898m
			(3409, 3854, OutsideSegment, Coast),          // len: 445m
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
			(511, 3418, OutsideSegment, Accelerate),      // len: 2907m
			(3418, 3842, OutsideSegment, Coast),          // len: 424m
			(3842, 4506, OutsideSegment, Brake),          // len: 664m
			(4506, 4526, OutsideSegment, Roll),           // len: 20m
			(4526, 4646, OutsideSegment, Coast),          // len: 120m
			(4646, 4666, OutsideSegment, Roll),           // len: 20m
			(4666, 4775, OutsideSegment, Coast),          // len: 109m
			(4775, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5612, WithinSegment, Accelerate),      // len: 1482m
			(5612, 5811, UseCase1, Coast),                // len: 199m
			(5811, 5976, OutsideSegment, Coast),          // len: 165m
			(5976, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5682, WithinSegment, Accelerate),      // len: 1027m
			(5682, 6551, UseCase1, Coast),                // len: 869m
			(6551, 7253, WithinSegment, Coast),           // len: 702m
			(7253, 7933, OutsideSegment, Coast),          // len: 680m
			(7933, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4994, WithinSegment, Accelerate),      // len: 1015m
			(4994, 5948, UseCase1, Coast),                // len: 954m
			(5948, 6141, WithinSegment, Coast),           // len: 193m
			(6141, 7155, WithinSegment, Brake),           // len: 1014m
			(7155, 7180, OutsideSegment, Brake),          // len: 25m
			(7180, 7592, OutsideSegment, Coast),          // len: 412m
			(7592, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P25_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P25_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_P25_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5134, WithinSegment, Accelerate),      // len: 1190m
			(5134, 5785, UseCase1, Coast),                // len: 651m
			(5785, 6027, WithinSegment, Coast),           // len: 242m
			(6027, 6559, WithinSegment, Brake),           // len: 532m
			(6559, 6633, OutsideSegment, Brake),          // len: 74m
			(6633, 7384, OutsideSegment, Coast),          // len: 751m
			(7384, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4982, WithinSegment, Accelerate),      // len: 1167m
			(4982, 5682, UseCase1, Coast),                // len: 700m
			(5682, 5995, WithinSegment, Coast),           // len: 313m
			(5995, 6319, OutsideSegment, Coast),          // len: 324m
			(6319, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4419, WithinSegment, Accelerate),      // len: 860m
			//(4419, 4439, WithinSegment, Roll),            // len: 20m
			//(4439, 4744, WithinSegment, Accelerate),      // len: 305m
			//(4744, 4757, WithinSegment, Roll),            // len: 13m
			//(4757, 4782, WithinSegment, Accelerate),      // len: 25m
			//(4782, 4794, WithinSegment, Roll),            // len: 12m
			//(4794, 5003, WithinSegment, Accelerate),      // len: 209m
			//(5003, 5015, WithinSegment, Roll),            // len: 12m
			//(5015, 5048, WithinSegment, Accelerate),      // len: 33m
			//(5048, 5062, WithinSegment, Roll),            // len: 14m
			(5062, 5311, WithinSegment, Accelerate),      // len: 249m
			(5311, 5603, UseCase2, Coast),                // len: 292m
			(5603, 5785, WithinSegment, Coast),           // len: 182m
			(5785, 6119, WithinSegment, Brake),           // len: 334m
			(6119, 6143, OutsideSegment, Brake),          // len: 24m
			(6143, 6532, OutsideSegment, Coast),          // len: 389m
			(6532, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(387, 3411, OutsideSegment, Accelerate),      // len: 3024m
			(3411, 4160, OutsideSegment, Coast),          // len: 749m
			(4160, 5046, OutsideSegment, Brake),          // len: 886m
			(5046, 5426, OutsideSegment, Coast),          // len: 380m
			(5426, 5435, OutsideSegment, Accelerate),     // len: 9m
			(5435, 5455, OutsideSegment, Roll),           // len: 20m
			(5455, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(387, 3605, OutsideSegment, Accelerate),      // len: 3218m
			(3605, 3990, OutsideSegment, Coast),          // len: 385m
			(3990, 4514, OutsideSegment, Brake),          // len: 524m
			(4514, 4534, OutsideSegment, Roll),           // len: 20m
			(4534, 4723, OutsideSegment, Coast),          // len: 189m
			(4723, 4733, OutsideSegment, Accelerate),     // len: 10m
			(4733, 4752, OutsideSegment, Roll),           // len: 19m
			(4752, 1e6, OutsideSegment, Accelerate));

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
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5461, WithinSegment, Accelerate),      // len: 806m
			(5461, 7251, UseCase1, Coast),                // len: 1790m
			(7251, 7263, OutsideSegment, Accelerate),     // len: 12m
			(7263, 7650, OutsideSegment, Coast),          // len: 387m
			(7650, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4924, WithinSegment, Accelerate),      // len: 945m
			(4924, 6065, UseCase1, Coast),                // len: 1141m
			(6065, 6076, UseCase1, Roll),                 // len: 11m
			(6076, 6088, UseCase1, Brake),                // len: 12m
			(6088, 6112, WithinSegment, Roll),            // len: 24m
			(6112, 6258, WithinSegment, Coast),           // len: 146m
			(6258, 7136, WithinSegment, Brake),           // len: 878m
			(7136, 7160, WithinSegment, Coast),           // len: 24m
			(7160, 7549, OutsideSegment, Coast),          // len: 389m
			(7549, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2274, UseCase1, Coast),                // len: 197m
			(2274, 2286, UseCase1, Roll),                 // len: 12m
			(2286, 2298, UseCase1, Coast),                // len: 12m
			(2298, 2310, UseCase1, Roll),                 // len: 12m
			(2310, 2369, UseCase1, Coast),                // len: 59m
			(2369, 2557, WithinSegment, Coast),           // len: 188m
			(2557, 2627, WithinSegment, Accelerate),      // len: 70m
			(2627, 3269, UseCase1, Coast),                // len: 642m
			(3269, 3292, OutsideSegment, Accelerate),     // len: 23m
			(3292, 3339, OutsideSegment, Coast),          // len: 47m
			(3339, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4830, WithinSegment, Accelerate),      // len: 1015m
			(4830, 5992, UseCase1, Coast),                // len: 1162m
			(5992, 6015, OutsideSegment, Accelerate),     // len: 23m
			(6015, 6074, OutsideSegment, Coast),          // len: 59m
			(6074, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5765, UseCase1, Coast),                // len: 293m
			(5765, 5812, WithinSegment, Coast),           // len: 47m
			(5812, 6037, OutsideSegment, Coast),          // len: 225m
			(6037, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 6461, UseCase1, Coast),                // len: 965m
			(6461, 6872, WithinSegment, Coast),           // len: 411m
			(6872, 7256, WithinSegment, Brake),           // len: 384m
			(7256, 7466, OutsideSegment, Brake),          // len: 210m
			(7466, 8025, OutsideSegment, Coast),          // len: 559m
			(8025, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 5878, UseCase1, Coast),                // len: 942m
			(5878, 6047, WithinSegment, Coast),           // len: 169m
			(6047, 7160, WithinSegment, Brake),           // len: 1113m
			(7160, 7234, OutsideSegment, Brake),          // len: 74m
			(7234, 7635, OutsideSegment, Coast),          // len: 401m
			(7635, 7681, OutsideSegment, Roll),           // len: 46m
			(7681, 7693, OutsideSegment, Accelerate),     // len: 12m
			(7693, 7716, OutsideSegment, Roll),           // len: 23m
			(7716, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2346, UseCase1, Coast),                // len: 257m
			(2346, 2570, WithinSegment, Coast),           // len: 224m
			(2570, 2652, WithinSegment, Accelerate),      // len: 82m
			(2652, 3093, UseCase1, Coast),                // len: 441m
			(3093, 3272, WithinSegment, Coast),           // len: 179m
			(3272, 3559, OutsideSegment, Coast),          // len: 287m
			(3559, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5728, UseCase1, Coast),                // len: 629m
			(5728, 5922, WithinSegment, Coast),           // len: 194m
			(5922, 6564, WithinSegment, Brake),           // len: 642m
			(6564, 6787, OutsideSegment, Brake),          // len: 223m
			(6787, 7539, OutsideSegment, Coast),          // len: 752m
			(7539, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5661, UseCase1, Coast),                // len: 807m
			(5661, 5988, WithinSegment, Coast),           // len: 327m
			(5988, 6401, OutsideSegment, Coast),          // len: 413m
			(6401, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
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
		public void Class5_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 4, OutsideSegment, Accelerate),           // len: 4m
			(411, 3415, OutsideSegment, Accelerate),      // len: 3004m
			(3415, 3848, OutsideSegment, Coast),          // len: 433m
			(3848, 4513, OutsideSegment, Brake),          // len: 665m
			(4513, 4533, OutsideSegment, Roll),           // len: 20m
			(4533, 4752, OutsideSegment, Coast),          // len: 219m
			(4752, 4771, OutsideSegment, Roll),           // len: 19m
			(4771, 1e6, OutsideSegment, Accelerate));


		private void TestPCC(string jobName, string cycleName, params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
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
