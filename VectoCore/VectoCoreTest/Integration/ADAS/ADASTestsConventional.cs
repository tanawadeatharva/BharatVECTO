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
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsConventional
	{
		private const string Group5NoPCC = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG.vecto";
		private const string Group5PCC12 = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG_PCC12.vecto";
		private const string Group5PCC123 = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG_PCC123.vecto";
		private const string Group5PCC123EcoNoEngineStop = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG_PCC123Eco.vecto";
		private const string Group5PCC123EcoEngineStop = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG_PCC123EcoEngineStop.vecto";
		private const string Group5WithEngineStop = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG_WithEngineStop.vecto";
		private const string Group5WithOutEngineStop = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\Class5_Tractor_ENG_WithoutEngineStop.vecto";

		private IXMLInputDataReader _xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

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


		[TestCase(@"TestData\Integration\ADAS-Conventional\Group5_EngineStopStart.xml")]
		public void TestVehicleWithADASEngineStopStart(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 4);
		}

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group5_EcoRoll.xml")]
		public void TestVehicleWithADASEcoRoll(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 4);
		}

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group5_EcoRollEngineStop.xml")]
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
			var jobName = @"TestData\Integration\ADAS-Conventional\Group5EcoRollEng\Class5_Tractor_ENG.vecto";
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
			const string jobName = @"TestData\Integration\ADAS-Conventional\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_Neutral.vecto";
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
			const string jobName = @"TestData\Integration\ADAS-Conventional\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_TC.vecto";
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

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group9_AT_EngineStopStart.xml")]
		public void TestATVehicleWithADASEngineStopStart(string filename)
		{
			//var container = RunAllDeclarationJob(filename);
			var container = RunSingleDeclarationJob(filename, 5);
		}

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group9_AT_EcoRoll.xml")]
		public void TestATVehicleWithADASEcoRoll(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 1);
		}


		// FC: Group5NoPCC <= Group5PCC12 <= Group5PCC123 <= Group5PCC123EcoEngineStop
		// FC for Group5PCC123EcoNoEngineStop can sometimes be bigger than all others.
		[TestCase(Group5WithOutEngineStop, 0, TestName = "G5Eng EcoRoll Without Engine Stop CrestCoast 1"),
		TestCase(Group5WithOutEngineStop, 1, TestName = "G5Eng EcoRoll Without Engine Stop CrestCoast 2"),
		TestCase(Group5WithOutEngineStop, 2, TestName = "G5Eng EcoRoll Without Engine Stop Case A"), // Case A
		TestCase(Group5WithOutEngineStop, 3, TestName = "G5Eng EcoRoll Without Engine Stop Case B"), // Case B
		TestCase(Group5WithOutEngineStop, 4, TestName = "G5Eng EcoRoll Without Engine Stop Case C"), // Case C
		TestCase(Group5WithOutEngineStop, 5, TestName = "G5Eng EcoRoll Without Engine Stop Case D"), // Case D
		TestCase(Group5WithOutEngineStop, 6, TestName = "G5Eng EcoRoll Without Engine Stop Case E"), // Case E
		TestCase(Group5WithOutEngineStop, 7, TestName = "G5Eng EcoRoll Without Engine Stop Case F"), // Case F
		TestCase(Group5WithOutEngineStop, 8, TestName = "G5Eng EcoRoll Without Engine Stop Case G"), // Case G
		TestCase(Group5WithOutEngineStop, 9, TestName = "G5Eng EcoRoll Without Engine Stop Case H"), // Case H
		TestCase(Group5WithOutEngineStop, 10, TestName = "G5Eng EcoRoll Without Engine Stop Case I"), // Case I
		TestCase(Group5WithOutEngineStop, 11, TestName = "G5Eng EcoRoll Without Engine Stop Case J"), // Case J

		TestCase(Group5WithEngineStop, 0, TestName = "G5Eng EcoRoll With Engine Stop CrestCoast 1"),
		TestCase(Group5WithEngineStop, 1, TestName = "G5Eng EcoRoll With Engine Stop CrestCoast 2"),
		TestCase(Group5WithEngineStop, 2, TestName = "G5Eng EcoRoll With Engine Stop Case A"), // Case A
		TestCase(Group5WithEngineStop, 3, TestName = "G5Eng EcoRoll With Engine Stop Case B"), // Case B
		TestCase(Group5WithEngineStop, 4, TestName = "G5Eng EcoRoll With Engine Stop Case C"), // Case C
		TestCase(Group5WithEngineStop, 5, TestName = "G5Eng EcoRoll With Engine Stop Case D"), // Case D
		TestCase(Group5WithEngineStop, 6, TestName = "G5Eng EcoRoll With Engine Stop Case E"), // Case E
		TestCase(Group5WithEngineStop, 7, TestName = "G5Eng EcoRoll With Engine Stop Case F"), // Case F
		TestCase(Group5WithEngineStop, 8, TestName = "G5Eng EcoRoll With Engine Stop Case G"), // Case G
		TestCase(Group5WithEngineStop, 9, TestName = "G5Eng EcoRoll With Engine Stop Case H"), // Case H
		TestCase(Group5WithEngineStop, 10, TestName = "G5Eng EcoRoll With Engine Stop Case I"), // Case I
		TestCase(Group5WithEngineStop, 11, TestName = "G5Eng EcoRoll With Engine Stop Case J"), // Case J
		
		TestCase(Group5NoPCC, 0, TestName = "G5Eng NoPCC CrestCoast 1"),
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

		// FuelConsumption should always be <= Group5NoPCC
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

		// FuelConsumption should always be <= Group5PCC12
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

		// FuelConsumption can sometimes be greater than Group5PCC123 (because of engine idling)
		TestCase(Group5PCC123EcoNoEngineStop, 2, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case A"), // Case A
		TestCase(Group5PCC123EcoNoEngineStop, 3, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case B"), // Case B
		TestCase(Group5PCC123EcoNoEngineStop, 4, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case C"), // Case C
		TestCase(Group5PCC123EcoNoEngineStop, 5, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case D"), // Case D
		TestCase(Group5PCC123EcoNoEngineStop, 6, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case E"), // Case E
		TestCase(Group5PCC123EcoNoEngineStop, 7, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case F"), // Case F
		TestCase(Group5PCC123EcoNoEngineStop, 8, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case G"), // Case G
		TestCase(Group5PCC123EcoNoEngineStop, 9, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case H"), // Case H
		TestCase(Group5PCC123EcoNoEngineStop, 10, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case I"), // Case I
		TestCase(Group5PCC123EcoNoEngineStop, 11, TestName = "G5Eng PCC123-Eco WithoutEngineStop Case J"), // Case J

		// FuelConsumption should always be <= Group5PCC123
		TestCase(Group5PCC123EcoEngineStop, 0, TestName = "G5Eng PCC123-Eco-EngineStop CrestCoast 1"),
		TestCase(Group5PCC123EcoEngineStop, 1, TestName = "G5Eng PCC123-Eco-EngineStop CrestCoast 2"),
		TestCase(Group5PCC123EcoEngineStop, 2, TestName = "G5Eng PCC123-Eco-EngineStop Case A"), // Case A
		TestCase(Group5PCC123EcoEngineStop, 3, TestName = "G5Eng PCC123-Eco-EngineStop Case B"), // Case B
		TestCase(Group5PCC123EcoEngineStop, 4, TestName = "G5Eng PCC123-Eco-EngineStop Case C"), // Case C
		TestCase(Group5PCC123EcoEngineStop, 5, TestName = "G5Eng PCC123-Eco-EngineStop Case D"), // Case D
		TestCase(Group5PCC123EcoEngineStop, 6, TestName = "G5Eng PCC123-Eco-EngineStop Case E"), // Case E
		TestCase(Group5PCC123EcoEngineStop, 7, TestName = "G5Eng PCC123-Eco-EngineStop Case F"), // Case F
		TestCase(Group5PCC123EcoEngineStop, 8, TestName = "G5Eng PCC123-Eco-EngineStop Case G"), // Case G
		TestCase(Group5PCC123EcoEngineStop, 9, TestName = "G5Eng PCC123-Eco-EngineStop Case H"), // Case H
		TestCase(Group5PCC123EcoEngineStop, 10, TestName = "G5Eng PCC123-Eco-EngineStop Case I"), // Case I
		TestCase(Group5PCC123EcoEngineStop, 11, TestName = "G5Eng PCC123-Eco-EngineStop  Case J"), // Case J
		]
		public void TestPCCEngineeringSampleCases(string jobName, int cycleIdx)
		{
			RunSingleEngineeringCycle(jobName, cycleIdx);
		}

		[TestCase(5, TestName = "PCC Group5 RD RefLoad"),
		TestCase(1, TestName = "PCC Group5 LH RefLoad")]
		public void TestTCCDeclaration(int runIdx)
		{
			var jobName = @"TestData\Integration\ADAS-Conventional\Group5PCCDecl\Tractor_4x2_vehicle-class-5_5_t_0.xml";
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



		public JobContainer RunAllDeclarationJob(string jobName)
		{
			var relativeJobPath = jobName;

			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? _xmlInputReader.CreateDeclaration(relativeJobPath)
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

			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			return jobContainer;
		}


		public JobContainer RunSingleDeclarationJob(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? _xmlInputReader.CreateDeclaration(relativeJobPath)
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
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			//var run = jobContainer.Runs[runIdx].Run;
			//run.Run();
			//var runs = factory.SimulationRuns().ToArray();
			//runs[runIdx].Run();

			//Assert.IsTrue(runs.FinishedWithoutErrors);

			return jobContainer;
		}
	}
}
