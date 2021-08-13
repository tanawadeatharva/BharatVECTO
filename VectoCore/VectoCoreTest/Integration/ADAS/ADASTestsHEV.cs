using System.IO;
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsHEV
	{
		private const string Group5NoADAS = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_NoADAS.vecto";
		private const string Group5EcoRollWithoutEngineStop = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_EcoRollWithoutEngineStop.vecto";
		private const string Group5EcoRollEngineStop = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_EcoRollEngineStop.vecto";
		private const string Group5PCC12 = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_PCC12.vecto";
		private const string Group5PCC123 = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_PCC123.vecto";
		private const string Group5PCC123EcoRollWithoutEngineStop = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_PCC123EcoRollWithoutEngineStop.vecto";
		private const string Group5PCC123EcoRollEngineStop = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_PCC123EcoRollEngineStop.vecto";
		
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
				ModalResultField.P_ice_out
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

		[TestCase(Group5EcoRollWithoutEngineStop, 0, TestName = "G5Eng EcoRoll Without Engine Stop CrestCoast 1"),
		TestCase(Group5EcoRollWithoutEngineStop, 1, TestName = "G5Eng EcoRoll Without Engine Stop CrestCoast 2"),
		TestCase(Group5EcoRollWithoutEngineStop, 2, TestName = "G5Eng EcoRoll Without Engine Stop Case A"), // Case A
		TestCase(Group5EcoRollWithoutEngineStop, 3, TestName = "G5Eng EcoRoll Without Engine Stop Case B"), // Case B
		TestCase(Group5EcoRollWithoutEngineStop, 4, TestName = "G5Eng EcoRoll Without Engine Stop Case C"), // Case C
		TestCase(Group5EcoRollWithoutEngineStop, 5, TestName = "G5Eng EcoRoll Without Engine Stop Case D"), // Case D
		TestCase(Group5EcoRollWithoutEngineStop, 6, TestName = "G5Eng EcoRoll Without Engine Stop Case E"), // Case E
		TestCase(Group5EcoRollWithoutEngineStop, 7, TestName = "G5Eng EcoRoll Without Engine Stop Case F"), // Case F
		TestCase(Group5EcoRollWithoutEngineStop, 8, TestName = "G5Eng EcoRoll Without Engine Stop Case G"), // Case G
		TestCase(Group5EcoRollWithoutEngineStop, 9, TestName = "G5Eng EcoRoll Without Engine Stop Case H"), // Case H
		TestCase(Group5EcoRollWithoutEngineStop, 10, TestName = "G5Eng EcoRoll Without Engine Stop Case I"), // Case I
		TestCase(Group5EcoRollWithoutEngineStop, 11, TestName = "G5Eng EcoRoll Without Engine Stop Case J"), // Case J

		TestCase(Group5EcoRollEngineStop, 0, TestName = "G5Eng EcoRoll With Engine Stop CrestCoast 1"),
		TestCase(Group5EcoRollEngineStop, 1, TestName = "G5Eng EcoRoll With Engine Stop CrestCoast 2"),
		TestCase(Group5EcoRollEngineStop, 2, TestName = "G5Eng EcoRoll With Engine Stop Case A"), // Case A
		TestCase(Group5EcoRollEngineStop, 3, TestName = "G5Eng EcoRoll With Engine Stop Case B"), // Case B
		TestCase(Group5EcoRollEngineStop, 4, TestName = "G5Eng EcoRoll With Engine Stop Case C"), // Case C
		TestCase(Group5EcoRollEngineStop, 5, TestName = "G5Eng EcoRoll With Engine Stop Case D"), // Case D
		TestCase(Group5EcoRollEngineStop, 6, TestName = "G5Eng EcoRoll With Engine Stop Case E"), // Case E
		TestCase(Group5EcoRollEngineStop, 7, TestName = "G5Eng EcoRoll With Engine Stop Case F"), // Case F
		TestCase(Group5EcoRollEngineStop, 8, TestName = "G5Eng EcoRoll With Engine Stop Case G"), // Case G
		TestCase(Group5EcoRollEngineStop, 9, TestName = "G5Eng EcoRoll With Engine Stop Case H"), // Case H
		TestCase(Group5EcoRollEngineStop, 10, TestName = "G5Eng EcoRoll With Engine Stop Case I"), // Case I
		TestCase(Group5EcoRollEngineStop, 11, TestName = "G5Eng EcoRoll With Engine Stop Case J"), // Case J

		TestCase(Group5NoADAS, 0, TestName = "G5Eng NoPCC CrestCoast 1"),
		TestCase(Group5NoADAS, 1, TestName = "G5Eng NoPCC CrestCoast 2"),
		TestCase(Group5NoADAS, 2, TestName = "G5Eng NoPCC Case A"), // Case A
		TestCase(Group5NoADAS, 3, TestName = "G5Eng NoPCC Case B"), // Case B
		TestCase(Group5NoADAS, 4, TestName = "G5Eng NoPCC Case C"), // Case C
		TestCase(Group5NoADAS, 5, TestName = "G5Eng NoPCC Case D"), // Case D
		TestCase(Group5NoADAS, 6, TestName = "G5Eng NoPCC Case E"), // Case E
		TestCase(Group5NoADAS, 7, TestName = "G5Eng NoPCC Case F"), // Case F
		TestCase(Group5NoADAS, 8, TestName = "G5Eng NoPCC Case G"), // Case G
		TestCase(Group5NoADAS, 9, TestName = "G5Eng NoPCC Case H"), // Case H
		TestCase(Group5NoADAS, 10, TestName = "G5Eng NoPCC Case I"), // Case I
		TestCase(Group5NoADAS, 11, TestName = "G5Eng NoPCC Case J"), // Case J

		TestCase(Group5PCC12, 0, TestName = "G5Eng PCC12 CrestCoast 1"),
		TestCase(Group5PCC12, 1, TestName = "G5Eng PCC12 CrestCoast 2"),
		TestCase(Group5PCC12, 2, TestName = "G5Eng PCC12 Case A"), // Case A
		TestCase(Group5PCC12, 3, TestName = "G5Eng PCC12 Case B"), // Case B
		TestCase(Group5PCC12, 4, TestName = "G5Eng PCC12 Case C"), // Case C
		TestCase(Group5PCC12, 5, TestName = "G5Eng PCC12 Case D"), // Case D
		TestCase(Group5PCC12, 6, TestName = "G5Eng PCC12 Case E"), // Case E
		TestCase(Group5PCC12, 7, TestName = "G5Eng PCC12 Case F"), // Case F
		TestCase(Group5PCC12, 8, TestName = "G5Eng PCC12 Case G"), // Case G
		TestCase(Group5PCC12, 9, TestName = "G5Eng PCC12 Case H"), // Case H
		TestCase(Group5PCC12, 10, TestName = "G5Eng PCC12 Case I"), // Case I
		TestCase(Group5PCC12, 11, TestName = "G5Eng PCC12 Case J"), // Case J

		TestCase(Group5PCC123, 2, TestName = "G5Eng PCC123 Case A"), // Case A
		TestCase(Group5PCC123, 3, TestName = "G5Eng PCC123 Case B"), // Case B
		TestCase(Group5PCC123, 4, TestName = "G5Eng PCC123 Case C"), // Case C
		TestCase(Group5PCC123, 5, TestName = "G5Eng PCC123 Case D"), // Case D
		TestCase(Group5PCC123, 6, TestName = "G5Eng PCC123 Case E"), // Case E
		TestCase(Group5PCC123, 7, TestName = "G5Eng PCC123 Case F"), // Case F
		TestCase(Group5PCC123, 8, TestName = "G5Eng PCC123 Case G"), // Case G
		TestCase(Group5PCC123, 9, TestName = "G5Eng PCC123 Case H"), // Case H
		TestCase(Group5PCC123, 10, TestName = "G5Eng PCC123 Case I"), // Case I
		TestCase(Group5PCC123, 11, TestName = "G5Eng PCC123 Case J"), // Case J

		TestCase(Group5PCC123EcoRollWithoutEngineStop, 2, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case A"), // Case A
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 3, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case B"), // Case B
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 4, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case C"), // Case C
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 5, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case D"), // Case D
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 6, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case E"), // Case E
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 7, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case F"), // Case F
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 8, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case G"), // Case G
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 9, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case H"), // Case H
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 10, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case I"), // Case I
		TestCase(Group5PCC123EcoRollWithoutEngineStop, 11, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case J"), // Case J

		TestCase(Group5PCC123EcoRollEngineStop, 0, TestName = "G5Eng PCC123-Eco-EngineStop CrestCoast 1"),
		TestCase(Group5PCC123EcoRollEngineStop, 1, TestName = "G5Eng PCC123-Eco-EngineStop CrestCoast 2"),
		TestCase(Group5PCC123EcoRollEngineStop, 2, TestName = "G5Eng PCC123-Eco-EngineStop Case A"), // Case A
		TestCase(Group5PCC123EcoRollEngineStop, 3, TestName = "G5Eng PCC123-Eco-EngineStop Case B"), // Case B
		TestCase(Group5PCC123EcoRollEngineStop, 4, TestName = "G5Eng PCC123-Eco-EngineStop Case C"), // Case C
		TestCase(Group5PCC123EcoRollEngineStop, 5, TestName = "G5Eng PCC123-Eco-EngineStop Case D"), // Case D
		TestCase(Group5PCC123EcoRollEngineStop, 6, TestName = "G5Eng PCC123-Eco-EngineStop Case E"), // Case E
		TestCase(Group5PCC123EcoRollEngineStop, 7, TestName = "G5Eng PCC123-Eco-EngineStop Case F"), // Case F
		TestCase(Group5PCC123EcoRollEngineStop, 8, TestName = "G5Eng PCC123-Eco-EngineStop Case G"), // Case G
		TestCase(Group5PCC123EcoRollEngineStop, 9, TestName = "G5Eng PCC123-Eco-EngineStop Case H"), // Case H
		TestCase(Group5PCC123EcoRollEngineStop, 10, TestName = "G5Eng PCC123-Eco-EngineStop Case I"), // Case I
		TestCase(Group5PCC123EcoRollEngineStop, 11, TestName = "G5Eng PCC123-Eco-EngineStop  Case J"), // Case J
		]
		public void TestPCCEngineeringSampleCases(string jobName, int cycleIdx)
		{
			RunSingleEngineeringCycle(jobName, cycleIdx);
		}

		public void RunSingleEngineeringCycle(string jobName, int cycleIdx)
		{
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
		public void CompareADAS_HEV_EngineeringJobs()
		{
			var jobName = Group5NoADAS;
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), "Group5_HEV_Compare"));
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			var jobNames = new[] {
				Group5NoADAS,
				Group5EcoRollWithoutEngineStop,
				Group5EcoRollEngineStop,
				Group5PCC12,
				Group5PCC123,
				Group5PCC123EcoRollWithoutEngineStop,
				Group5PCC123EcoRollEngineStop,
			};

			var modData = new ConcurrentDictionary<(string, string), ModalResults>();

			Parallel.ForEach(jobNames, j => {
				var factory = new SimulatorFactory(ExecutionMode.Engineering,
					JSONInputDataFactory.ReadJsonJob(j), writer, validate: false, writeModalResults: true);
				foreach (var run in jobContainer.AddRuns(factory)) {
					modData.TryAdd((j, run.CycleName), (run.GetContainer().ModalData as ModalDataContainer).Data);
				}
			});

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();

			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			var c = "CrestCoast1.vdri";
			var result = CheckCycle(c, sumContainer);
			Assert.AreEqual(203, result.NoADAS, 5);
			Assert.AreEqual(result.PCC12, result.NoADAS, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.AreEqual(result.PCC123, result.NoADAS, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.AreEqual(result.EcoRollEngineStop, result.PCC123EngineStop, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.AreEqual(result.EcoRollEngineStop, result.PCC123EngineStop, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.GreaterOrEqual(result.EcoRollNoStop, result.NoADAS, $"{c}: Enabling EcoRoll increases fuel consumption.");
			Assert.GreaterOrEqual(result.EcoRollEngineStop, result.NoADAS, $"{c}: Enabling EcoRoll increases fuel consumption.");
			TestPCCSections(modData, c);

			var m = modData[(Group5PCC123, c.Slice(0, -5))];
			var pccStates = m.SelectData(x => Convert.ToInt32(x["PCCState"]));
			foreach (var (p, i) in pccStates.Select()) {
				Assert.AreEqual(0, p, $"PCCStates Index[{i}] should be zero.");
			}

			c = "CrestCoast2.vdri"; result = CheckCycle(c, sumContainer);
			Assert.AreEqual(250, result.NoADAS, 5);
			Assert.AreEqual(result.PCC12, result.NoADAS, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.AreEqual(result.PCC123, result.NoADAS, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.AreEqual(result.EcoRollEngineStop, result.PCC123EngineStop, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.AreEqual(result.EcoRollEngineStop, result.PCC123EngineStop, $"{c}: since there is no pcc event, pcc should consume the same.");
			Assert.GreaterOrEqual(result.EcoRollNoStop, result.NoADAS, $"{c}: Enabling EcoRoll increases fuel consumption.");
			Assert.GreaterOrEqual(result.EcoRollEngineStop, result.NoADAS, $"{c}: Enabling EcoRoll increases fuel consumption.");
			TestPCCSections(modData, c);

			c = "Group5Eng_CaseA.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(264, result.NoADAS, 5);
			Assert.Less(result.PCC12, result.NoADAS, $"{c}: pcc should be less.");
			Assert.Less(result.PCC123, result.NoADAS, $"{c}: pcc should be less.");
			Assert.AreEqual(result.PCC12, result.PCC123, $"{c}: pcc 12 and 123 should be equal.");
			Assert.Less(result.EcoRollNoStop, result.NoADAS);
			Assert.Less(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC123EngineStop, result.EcoRollEngineStop);
			Assert.Less(result.PCC123NoEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC123NoEngineStop, result.EcoRollEngineStop);
			TestPCCSections(modData, c, (4119d, 0, 1), (5426d, 1, 2), (5830d, 2, 0));

			c = "Group5Eng_CaseB.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(224, result.NoADAS, 5);
			Assert.AreEqual(result.PCC12, result.PCC123);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.Less(result.NoADAS, result.EcoRollEngineStop);
			Assert.Less(result.NoADAS, result.EcoRollNoStop);
			Assert.Less(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (4609, 0, 1), (5414, 1, 2), (7291, 2, 0));

			c = "Group5Eng_CaseC.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(197, result.NoADAS, 5);
			Assert.Greater(result.EcoRollNoStop, result.NoADAS);
			Assert.Less(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.Less(result.PCC123, result.PCC12);
			Assert.Greater(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (3967, 0, 1), (4912, 1, 2), (6089, 2, 1), (7173, 1, 0));

			c = "Group5Eng_CaseD.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(248, result.NoADAS, 5);
			Assert.Greater(result.EcoRollNoStop, result.NoADAS);
			Assert.Less(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.Less(result.PCC123, result.PCC12);
			Assert.Less(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (654, 0, 1), (1867, 1, 2), (2481, 2, 0), (4021, 0, 1), (4919, 1, 2), (6217, 2, 1), (6704, 1, 0));

			c = "Group5Eng_CaseE.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(230, result.NoADAS, 5);
			Assert.Less(result.EcoRollNoStop, result.NoADAS);
			Assert.Less(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.AreEqual(result.PCC123, result.PCC12);
			Assert.Less(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (689, 0, 1), (2066, 1, 2), (2377, 2, 1), (2984, 1, 2), (3871, 2, 1), (3978, 1, 0));

			c = "Group5Eng_CaseF.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(220, result.NoADAS, 5);
			Assert.Less(result.EcoRollNoStop, result.NoADAS);
			Assert.Less(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.AreEqual(result.PCC123, result.PCC12);
			Assert.Less(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (701, 0, 1), (2066, 1, 2), (2400, 2, 1), (2587, 1, 2), (3283, 2, 0));

			c = "Group5Eng_CaseG.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(236, result.NoADAS, 5);
			Assert.AreEqual(result.EcoRollNoStop, result.NoADAS);
			Assert.AreEqual(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.Less(result.PCC123, result.PCC12);
			Assert.Greater(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123);
			TestPCCSections(modData, c, (3944, 0, 1), (5076, 1, 2), (5899, 2, 1), (6596, 1, 0));

			c = "Group5Eng_CaseH.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(204, result.NoADAS, 5);
			Assert.Greater(result.EcoRollNoStop, result.NoADAS);
			Assert.Less(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.AreEqual(result.PCC123, result.PCC12);
			Assert.Less(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (3804, 0, 1), (4772, 1, 2), (6003, 2, 0));

			c = "Group5Eng_CaseI.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(214, result.NoADAS, 5);
			Assert.AreEqual(result.EcoRollNoStop, result.NoADAS);
			Assert.AreEqual(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.AreEqual(result.PCC12, result.NoADAS);
			Assert.AreEqual(result.PCC123, result.PCC12);
			Assert.AreEqual(result.PCC123NoEngineStop, result.PCC123);
			Assert.AreEqual(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c);

			c = "Group5Eng_CaseJ.vdri";
			result = CheckCycle(c, sumContainer);
			Assert.AreEqual(303, result.NoADAS, 5);
			Assert.AreEqual(result.EcoRollNoStop, result.NoADAS);
			Assert.AreEqual(result.EcoRollEngineStop, result.EcoRollNoStop);
			Assert.Less(result.PCC12, result.NoADAS);
			Assert.Less(result.PCC123, result.PCC12);
			Assert.Greater(result.PCC123NoEngineStop, result.PCC123);
			Assert.Less(result.PCC123EngineStop, result.PCC123NoEngineStop);
			TestPCCSections(modData, c, (3559, 0, 1), (5364, 1, 3), (5692, 3, 1), (6132, 1, 0));
		}

		private (ConvertedSI NoADAS, ConvertedSI EcoRollNoStop, ConvertedSI EcoRollEngineStop,
			ConvertedSI PCC12, ConvertedSI PCC123, ConvertedSI PCC123NoEngineStop, ConvertedSI PCC123EngineStop)
			CheckCycle(string s, SummaryDataContainer summaryDataContainer)
		{

			var sumResults = summaryDataContainer.Table.Select($@"[Cycle [-\]] = '{s}'", "Input File [-]");
			Assert.AreEqual(7, sumResults.Length, $"{s}: Not enough result rows in sum file");
			var values = sumResults.Select(row => row.Field<ConvertedSI>("FC-Final [g/km]")).ToArray();
			var (EcoRollEngineStop, EcoRollNoStop, NoADAS, PCC12, PCC123, PCC123EngineStop, PCC123NoEngineStop) = values;

			Assert.LessOrEqual(PCC12, NoADAS, $"{s}: Enabling ADAS should always reduce fuel consumption.");
			Assert.LessOrEqual(PCC123, NoADAS, $"{s}: Enabling ADAS should always reduce fuel consumption.");

			Assert.LessOrEqual(PCC123EngineStop, PCC123NoEngineStop, $"{s}: with engine stop should always consume less then without engine stop");
			Assert.LessOrEqual(EcoRollEngineStop, EcoRollNoStop, $"{s}: with engine stop should always consume less then without engine stop");

			Assert.LessOrEqual(PCC123NoEngineStop, EcoRollNoStop, $"{s}: PCC EcoRoll should always be lower as EcoRoll.");
			Assert.LessOrEqual(PCC123EngineStop, EcoRollEngineStop, $"{s}: PCC EcoRoll should always be lower as EcoRoll.");

			Assert.LessOrEqual(PCC123, PCC12, $"{s}: better pcc options should consume less");

			return (NoADAS, EcoRollNoStop, EcoRollEngineStop, PCC12, PCC123, PCC123NoEngineStop, PCC123EngineStop);
		}

		private void TestPCCSections(ConcurrentDictionary<(string, string), ModalResults> modData, string c,
			params (double Distance, int Before, int After)[] expectedSections)
		{
			var m = modData[(Group5PCC123, c.Slice(0, -5))];
			var pccStates = m.SelectData(x => Convert.ToInt32(x["PCCState"]));
			var distances = m.SelectData(x => x.Field<Meter>(ModalResultField.dist.GetName()).Value());
			var sections = GetDistancesOfStateChanges(pccStates, distances);
			if (expectedSections.Length == 0) {
				Assert.IsFalse(sections.Any());
			} else {
				foreach (var (exp, actual) in expectedSections.Zip(sections)) {
					Assert.AreEqual(exp.Before, actual.Before, $"Cycle {c}: Expected change from {exp.Before} --> {exp.After} at distance {exp.Distance}");
					Assert.AreEqual(exp.After, actual.After, $"Cycle {c}: Expected change from {exp.Before} --> {exp.After} at distance {exp.Distance}");
					Assert.AreEqual(exp.Distance, actual.Distance, 10, $"Cycle {c}: Expected change from {exp.Before} --> {exp.After} at distance {exp.Distance}");
				}
			}
		}

		IEnumerable<(T2 Distance, T1 Before, T1 After)> GetDistancesOfStateChanges<T1, T2>(IEnumerable<T1> states, IEnumerable<T2> locations)
		{
			using (var values = states.GetEnumerator()) {
				using (var locs = locations.GetEnumerator()) {
					locs.MoveNext();
					values.MoveNext();
					var value = values.Current;
					while (values.MoveNext()) {
						locs.MoveNext();
						if (!value.Equals(values.Current)) {
							yield return (locs.Current, value, values.Current);
							value = values.Current;
						}
					}
				}
			}
		}



	}
}
