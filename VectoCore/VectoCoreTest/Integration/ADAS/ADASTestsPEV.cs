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
	public class ADASTestsPEV
	{
		const string Group5NoPCC = @"TestData\Integration\ADAS-PEV\Group5PCCEng\Class5_Tractor_ENG.vecto";
		const string Group5PCC12 = @"TestData\Integration\ADAS-PEV\Group5PCCEng\Class5_Tractor_ENG_PCC12.vecto";
		const string Group5PCC123 = @"TestData\Integration\ADAS-PEV\Group5PCCEng\Class5_Tractor_ENG_PCC123.vecto";
		const string Group5PCC123EcoSS = @"TestData\Integration\ADAS-PEV\Group5PCCEng\Class5_Tractor_ENG_PCC123EcoSS.vecto";

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
				ModalResultField.EM_E2_Off_,ModalResultField.P_E2_mech_
			};
			graphWriter.Series1Label = "ADAS PCC";
			graphWriter.PlotIgnitionState = true;
			return graphWriter;
		}

		[
		TestCase(Group5NoPCC, 0, TestName = "G5Eng NoPCC CrestCoast 1"),
		TestCase(Group5NoPCC, 1, TestName = "G5Eng NoPCC CrestCoast 2"),
		TestCase(Group5NoPCC, 2, TestName = "G5Eng NoPCC Case A"), // Case A: Tests Use-Case 1: Creast Coasting (allowing slower speed before a crest)
		TestCase(Group5NoPCC, 3, TestName = "G5Eng NoPCC Case B"), // Case B: 
		TestCase(Group5NoPCC, 4, TestName = "G5Eng NoPCC Case C"), // Case C
		TestCase(Group5NoPCC, 5, TestName = "G5Eng NoPCC Case D"), // Case D: Test two crests after each other
		TestCase(Group5NoPCC, 6, TestName = "G5Eng NoPCC Case E"), // Case E
		TestCase(Group5NoPCC, 7, TestName = "G5Eng NoPCC Case F"), // Case F
		TestCase(Group5NoPCC, 8, TestName = "G5Eng NoPCC Case G"), // Case G: PCC Use Case 1 even if there is a small dip inbetween (at the crest)
		TestCase(Group5NoPCC, 9, TestName = "G5Eng NoPCC Case H"), // Case H
		TestCase(Group5NoPCC, 10, TestName = "G5Eng NoPCC Case I"), // Case I
		TestCase(Group5NoPCC, 11, TestName = "G5Eng NoPCC Case J"), // Case J

		TestCase(Group5PCC12, 0, TestName = "G5Eng PCC12 CrestCoast 1"),
		TestCase(Group5PCC12, 1, TestName = "G5Eng PCC12 CrestCoast 2"),
		TestCase(Group5PCC12, 2, TestName = "G5Eng PCC12 Case A"), // Case A: Tests Use-Case 1: Creast Coasting (allowing slower speed before a crest)
		TestCase(Group5PCC12, 3, TestName = "G5Eng PCC12 Case B"), // Case B: 
		TestCase(Group5PCC12, 4, TestName = "G5Eng PCC12 Case C"), // Case C
		TestCase(Group5PCC12, 5, TestName = "G5Eng PCC12 Case D"), // Case D: Test two crests after each other
		TestCase(Group5PCC12, 6, TestName = "G5Eng PCC12 Case E"), // Case E
		TestCase(Group5PCC12, 7, TestName = "G5Eng PCC12 Case F"), // Case F
		TestCase(Group5PCC12, 8, TestName = "G5Eng PCC12 Case G"), // Case G: PCC Use Case 1 even if there is a small dip inbetween (at the crest)
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

		TestCase(Group5PCC123EcoSS, 2, TestName = "G5Eng PCC123EcoSS Case A"), // Case A should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 3, TestName = "G5Eng PCC123EcoSS Case B"), // Case B should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 4, TestName = "G5Eng PCC123EcoSS Case C"), // Case C should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 5, TestName = "G5Eng PCC123EcoSS Case D"), // Case D should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 6, TestName = "G5Eng PCC123EcoSS Case E"), // Case E should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 7, TestName = "G5Eng PCC123EcoSS Case F"), // Case F should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 8, TestName = "G5Eng PCC123EcoSS Case G"), // Case G should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 9, TestName = "G5Eng PCC123EcoSS Case H"), // Case H should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 10, TestName = "G5Eng PCC123EcoSS Case I"), // Case I should behave same as PCC123 (eco roll always off in PEV)
		TestCase(Group5PCC123EcoSS, 11, TestName = "G5Eng PCC123EcoSS Case J"), // Case J should behave same as PCC123 (eco roll always off in PEV)
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
