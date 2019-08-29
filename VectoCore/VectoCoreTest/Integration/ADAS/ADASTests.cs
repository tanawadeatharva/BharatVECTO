using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	[TestFixture]
	public class ADASTests
	{

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();

			InitGraphWriter();
		}

		private void InitGraphWriter()
		{
			//#if TRACE
			GraphWriter.Enable();
			//#else
			//GraphWriter.Disable();
//#endif
			GraphWriter.Xfields = new[] { ModalResultField.time, ModalResultField.dist };

			GraphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_eng_avg, ModalResultField.Gear,
				ModalResultField.P_eng_out, ModalResultField.P_eng_drag, ModalResultField.FCMap
			};
			GraphWriter.Series1Label = "EcoRoll";
		}


		[TestCase(@"TestData\Integration\ADAS\Group5_EngineStopStart.xml")]
		public void TestVehicleWithADASEngineStopStart(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 4);
		}

		[TestCase(@"TestData\Integration\ADAS\Group5_EcoRoll.xml")]
		public void TestVehicleWithADASEcoRoll(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 4);
		}

		[TestCase(@"TestData\Integration\ADAS\Group5_EcoRollEngineStop.xml")]
		public void TestVehicleWithADASEcoRollEngineStopStart(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 1);
		}


		[TestCase(0, TestName = "EcoRoll DH1.1 const"),
		TestCase(1, TestName = "EcoRoll DH1.1 UH0.1"),
		TestCase(2, TestName = "EcoRoll DH1.3 const"),
		TestCase(3, TestName = "EcoRoll DH0.8 const - too flat"),
		TestCase(4, TestName = "EcoRoll DH1.5 const - too steep"),
		TestCase(5, TestName = "EcoRoll DH1.1 const - Stop"),
		TestCase(6, TestName = "EcoRoll DH1.1 const - TS60"),
		TestCase(7, TestName = "EcoRoll DH1.1 const - TS68"),
		TestCase(8, TestName = "EcoRoll DH1.1 const - TS72"),
		TestCase(9, TestName = "EcoRoll DH1.1 const - TS80"),
			]
		public void TestEcoRoll(int cycleIdx)
		{
			string jobName = @"TestData\Integration\ADAS\Group5EcoRollEng\Class5_Tractor_ENG.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];
			
			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GraphWriter.Write(modFilename);
		}

		[TestCase(0, TestName = "AT EcoRoll DH1.8 const"),
		TestCase(1, TestName = "AT EcoRoll DH1.8 UH0.1"), 
		TestCase(2, TestName = "AT EcoRoll DH1.9 const"),
		TestCase(3, TestName = "AT EcoRoll DH1.2 const - too flat"),
		TestCase(4, TestName = "AT EcoRoll DH2.5 const - too steep"),
		TestCase(5, TestName = "AT EcoRoll DH1.9 const - Stop"),
		TestCase(6, TestName = "AT EcoRoll DH1.9 const - TS60"),
		TestCase(7, TestName = "AT EcoRoll DH1.9 const - TS68"),
		TestCase(8, TestName = "AT EcoRoll DH1.9 const - TS72"),
		TestCase(9, TestName = "AT EcoRoll DH1.9 const - TS80"),
		]
		public void TestEcoRollAT(int cycleIdx)
		{
			string jobName = @"TestData\Integration\ADAS\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GraphWriter.Write(modFilename);
		}


		[TestCase(@"TestData\Integration\ADAS\Group9_AT_EngineStopStart.xml")]
		public void TestATVehicleWithADASEngineStopStart(string filename)
		{
			//var container = RunAllDeclarationJob(filename);
			var container = RunSingleDeclarationJob(filename, 5);
		}

		[TestCase(@"TestData\Integration\ADAS\Group9_AT_EcoRoll.xml")]
		public void TestATVehicleWithADASEcoRoll(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 1);
		}




		[TestCase(0, TestName = "G5Eng PCC CrestCoast 1"),
		TestCase(1, TestName = "G5Eng PCC CrestCoast 2"),
		TestCase(2), // Case A
		TestCase(3), // Case B
		TestCase(4), // Case C
		TestCase(5), // Case D
		TestCase(6), // Case E
		TestCase(7), // Case F
		TestCase(8), // Case G
		TestCase(9), // Case H
		TestCase(10), // Case I
		TestCase(11), // Case J
		]
		public void TestPCCEngineeringSampleCases(int cycleIdx)
		{
			var jobName = @"TestData\Integration\ADAS\Group5PCCEng\Class5_Tractor_ENG.vecto";
			RunSingleEngineeringCycle(jobName, cycleIdx);
		}

		[TestCase(5, TestName = "PCC Group5 RD RefLoad"),
		TestCase(1, TestName = "PCC Group5 LH RefLoad")]
		public void TestTCCDeclaration(int runIdx)
		{
			var jobName = @"TestData\Integration\ADAS\Group5PCCDecl\Tractor_4x2_vehicle-class-5_5_t_0.xml";
			RunSingleDeclarationJob(jobName, runIdx);
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
				Validate = false
			};

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GraphWriter.Write(modFilename);
		}



		public JobContainer RunAllDeclarationJob(string jobName)
		{
			var relativeJobPath =  jobName;
			
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();

			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));

			return jobContainer;
		}


		public JobContainer RunSingleDeclarationJob(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();

			jobContainer.AddRun(runs[runIdx]);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));

			//var run = jobContainer.Runs[runIdx].Run;
			//run.Run();
			//var runs = factory.SimulationRuns().ToArray();
			//runs[runIdx].Run();

			//Assert.IsTrue(runs.FinishedWithoutErrors);

			return jobContainer;
		}
	}
}
