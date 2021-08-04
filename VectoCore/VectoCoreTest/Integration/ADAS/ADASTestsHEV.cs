using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsHEV
	{
		const string Group5NoPCC = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_ENG.vecto";
		const string Group5PCC12 = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_ENG_PCC12.vecto";
		const string Group5PCC123 = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_ENG_PCC123.vecto";
		const string Group5PCC123EcoSS = @"TestData\Integration\ADAS-HEV\Group5PCCEng\Class5_Tractor_ENG_PCC123EcoSS.vecto";

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
				//ActualModalData = true,
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
				//ActualModalData = true,
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
				//ActualModalData = true,
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

		[TestCase(Group5NoPCC, 0, TestName = "G5Eng NoPCC CrestCoast 1"),
		TestCase(Group5NoPCC, 1, TestName = "G5Eng NoPCC CrestCoast 2"),
		TestCase(Group5NoPCC, 2, TestName = "G5Eng NoPCC Case A"), // Case A
		TestCase(Group5NoPCC, 3, TestName = "G5Eng NoPCC Case B"), // Case B
		TestCase(Group5NoPCC, 4, TestName = "G5Eng NoPCC Case C"), // Case C
		TestCase(Group5NoPCC, 5, TestName = "G5Eng NoPCC Case D"), // Case D
		TestCase(Group5NoPCC, 6, TestName = "G5Eng NoPCC Case E"), // Case E
		TestCase(Group5NoPCC, 7, TestName = "G5Eng NoPCC Case F"), // Case F
		TestCase(Group5NoPCC, 8, TestName = "G5Eng NoPCC Case G"), // Case G
		TestCase(Group5NoPCC, 9, TestName = "G5Eng NoPCC Case H"), // Case H
		TestCase(Group5NoPCC, 10, TestName = "G5Eng NoPCC Case I"), // Case I
		TestCase(Group5NoPCC, 11, TestName = "G5Eng NoPCC Case J"), // Case J

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

		TestCase(Group5PCC123, 0, TestName = "G5Eng PCC123 CrestCoast 1"),
		TestCase(Group5PCC123, 1, TestName = "G5Eng PCC123 CrestCoast 2"),
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

		TestCase(Group5PCC123EcoSS, 0, TestName = "G5Eng PCC123-EcoSS CrestCoast 1"),
		TestCase(Group5PCC123EcoSS, 1, TestName = "G5Eng PCC123-EcoSS CrestCoast 2"),
		TestCase(Group5PCC123EcoSS, 2, TestName = "G5Eng PCC123-EcoSS Case A"), // Case A
		TestCase(Group5PCC123EcoSS, 3, TestName = "G5Eng PCC123-EcoSS Case B"), // Case B
		TestCase(Group5PCC123EcoSS, 4, TestName = "G5Eng PCC123-EcoSS Case C"), // Case C
		TestCase(Group5PCC123EcoSS, 5, TestName = "G5Eng PCC123-EcoSS Case D"), // Case D
		TestCase(Group5PCC123EcoSS, 6, TestName = "G5Eng PCC123-EcoSS Case E"), // Case E
		TestCase(Group5PCC123EcoSS, 7, TestName = "G5Eng PCC123-EcoSS Case F"), // Case F
		TestCase(Group5PCC123EcoSS, 8, TestName = "G5Eng PCC123-EcoSS Case G"), // Case G
		TestCase(Group5PCC123EcoSS, 9, TestName = "G5Eng PCC123-EcoSS Case H"), // Case H
		TestCase(Group5PCC123EcoSS, 10, TestName = "G5Eng PCC123-EcoSS Case I"), // Case I
		TestCase(Group5PCC123EcoSS, 11, TestName = "G5Eng PCC123-EcoSS Case J"), // Case J
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
				//ActualModalData = true,
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
	}
}
