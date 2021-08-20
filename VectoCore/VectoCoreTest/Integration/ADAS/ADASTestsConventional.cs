using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
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
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsConventional
	{
		private const string BasePath = @"TestData\Integration\ADAS-PEV\Group5PCCEng\";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

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

		[TestCase(5, TestName = "PCC Group5 RD RefLoad"),
		TestCase(1, TestName = "PCC Group5 LH RefLoad")]
		public void TestTCCDeclaration(int runIdx)
		{
			var jobName = @"TestData\Integration\ADAS-Conventional\Group5PCCDecl\Tractor_4x2_vehicle-class-5_5_t_0.xml";
			RunSingleDeclarationJob(jobName, runIdx);
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
			var run = runs[runIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);

			return jobContainer;
		}

		#region PCC Engineering Testcases
		[Test]
		public void Class5_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4119, WithinSegment, Accelerate),
			(5426, UseCase1, Coast),
			(5830, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4609, WithinSegment, Accelerate),
			(5414, UseCase1, Coast),
			(7291, OutsideSegment, Coast),
			(7490, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3967, WithinSegment, Accelerate),
			(4912, UseCase1, Coast),
			(6089, WithinSegment, Coast),
			(6283, WithinSegment, Brake),
			(7160, OutsideSegment, Coast),
			(7573, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(654, WithinSegment, Accelerate),
			(1867, UseCase1, Coast),
			(2481, OutsideSegment, Accelerate),
			(4021, WithinSegment, Accelerate),
			(4919, UseCase1, Coast),
			(6217, WithinSegment, Coast),
			(6593, WithinSegment, Brake),
			(6704, OutsideSegment, Coast),
			(7092, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(689, WithinSegment, Accelerate),
			(2066, UseCase1, Coast),
			(2377, WithinSegment, Accelerate),
			(2984, UseCase1, Coast),
			(3871, WithinSegment, Coast),
			(3978, OutsideSegment, Coast),
			(4179, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(701, WithinSegment, Accelerate),
			(2066, UseCase1, Coast),
			(2400, WithinSegment, Accelerate),
			(2587, UseCase1, Coast),
			(3283, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3944, WithinSegment, Accelerate),
			(5076, UseCase1, Coast),
			(5899, WithinSegment, Coast),
			(6275, WithinSegment, Brake),
			(6571, WithinSegment, Coast),
			(6596, OutsideSegment, Coast),
			(7323, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3804, WithinSegment, Accelerate),
			(4772, UseCase1, Coast),
			(6003, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3450, OutsideSegment, Accelerate),
			(3600, 4300, WithinSegment, Accelerate),
			(5400, 5600, UseCase2, Coast),
			(5750, 5900, WithinSegment, Coast),
			(5970, 6080, WithinSegment, Brake),
			(6150, 6480, OutsideSegment, Coast),
			(6550, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(4441, OutsideSegment, Brake),
			(5005, OutsideSegment, Coast),
			(5422, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(4138, OutsideSegment, Brake),
			(4510, OutsideSegment, Coast),
			(4718, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4119, WithinSegment, Accelerate),
			(5426, UseCase1, Coast),
			(5830, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4609, WithinSegment, Accelerate),
			(5414, UseCase1, Coast),
			(7291, OutsideSegment, Coast),
			(7490, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3967, WithinSegment, Accelerate),
			(4912, UseCase1, Coast),
			(6089, WithinSegment, Coast),
			(6161, WithinSegment, Brake),
			(7170, OutsideSegment, Coast),
			(7467, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(654, WithinSegment, Accelerate),
			(1867, UseCase1, Coast),
			(2481, OutsideSegment, Accelerate),
			(4021, WithinSegment, Accelerate),
			(4919, UseCase1, Coast),
			(6217, WithinSegment, Coast),
			(6312, WithinSegment, Brake),
			(6696, WithinSegment, Coast),
			(6708, OutsideSegment, Coast),
			(6982, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(689, WithinSegment, Accelerate),
			(2066, UseCase1, Coast),
			(2377, WithinSegment, Accelerate),
			(2984, UseCase1, Coast),
			(3871, WithinSegment, Coast),
			(3978, OutsideSegment, Coast),
			(4179, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(701, WithinSegment, Accelerate),
			(2066, UseCase1, Coast),
			(2400, WithinSegment, Accelerate),
			(2587, UseCase1, Coast),
			(3283, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3944, WithinSegment, Accelerate),
			(5076, UseCase1, Coast),
			(5899, WithinSegment, Coast),
			(5994, WithinSegment, Brake),
			(6595, OutsideSegment, Coast),
			(7118, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3804, WithinSegment, Accelerate),
			(4772, UseCase1, Coast),
			(6003, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),
			(3559, 4470, WithinSegment, Accelerate),
			(5245, 5364, WithinSegment, Accelerate),
			(5364, 5692, UseCase2, Coast),
			(5692, 5751, WithinSegment, Coast),
			(5751, 6124, WithinSegment, Brake),
			(6124, 6136, WithinSegment, Coast),
			(6136, 6409, OutsideSegment, Coast),
			(6409, 1e06, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC12_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(4441, OutsideSegment, Brake),
			(5005, OutsideSegment, Coast),
			(5422, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC12_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(4138, OutsideSegment, Brake),
			(4510, OutsideSegment, Coast),
			(4718, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5601, OutsideSegment, Coast),
			(5940, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(6079, OutsideSegment, Coast),
			(6704, OutsideSegment, Brake),
			(7293, OutsideSegment, Coast),
			(7757, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5449, OutsideSegment, Coast),
			(5779, OutsideSegment, Brake),
			(7160, OutsideSegment, Coast),
			(7458, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2147, OutsideSegment, Coast),
			(2289, OutsideSegment, Brake),
			(2481, OutsideSegment, Coast),
			(2612, OutsideSegment, Accelerate),
			(5505, OutsideSegment, Coast),
			(5847, OutsideSegment, Brake),
			(6700, OutsideSegment, Coast),
			(6985, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2182, OutsideSegment, Coast),
			(2476, OutsideSegment, Accelerate),
			(3328, OutsideSegment, Coast),
			(3563, OutsideSegment, Brake),
			(3972, OutsideSegment, Coast),
			(4234, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2182, OutsideSegment, Coast),
			(2511, OutsideSegment, Accelerate),
			(2873, OutsideSegment, Coast),
			(3475, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5426, OutsideSegment, Coast),
			(5591, OutsideSegment, Brake),
			(6600, OutsideSegment, Coast),
			(7111, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5286, OutsideSegment, Coast),
			(5640, OutsideSegment, Brake),
			(6000, OutsideSegment, Coast),
			(6250, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4400, OutsideSegment, Accelerate),
			(5300, 5430, OutsideSegment, Accelerate),
			(5600, 6000, OutsideSegment, Brake),
			(6150, 6375, OutsideSegment, Coast),
			(6450, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_NoADAS_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(4441, OutsideSegment, Brake),
			(5005, OutsideSegment, Coast),
			(5422, OutsideSegment, Accelerate));


		[Test]
		public void Class5_NoADAS_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(4138, OutsideSegment, Brake),
			(4510, OutsideSegment, Coast),
			(4718, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5601, OutsideSegment, Coast),
			(5671, OutsideSegment, Roll),
			(6072, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(6079, OutsideSegment, Coast),
			(6149, OutsideSegment, Roll),
			(6433, OutsideSegment, Brake),
			(6445, OutsideSegment, Coast),
			(6469, OutsideSegment, Brake),
			(7286, OutsideSegment, Coast),
			(7750, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5449, OutsideSegment, Coast),
			(5519, OutsideSegment, Roll),
			(5708, OutsideSegment, Brake),
			(7162, OutsideSegment, Coast),
			(7460, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2147, OutsideSegment, Coast),
			(2289, OutsideSegment, Brake),
			(2481, OutsideSegment, Coast),
			(2612, OutsideSegment, Accelerate),
			(5505, OutsideSegment, Coast),
			(5575, OutsideSegment, Roll),
			(5764, OutsideSegment, Brake),
			(6701, OutsideSegment, Coast),
			(6987, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2182, OutsideSegment, Coast),
			(2252, OutsideSegment, Roll),
			(2548, OutsideSegment, Accelerate),
			(3330, OutsideSegment, Coast),
			(3400, OutsideSegment, Roll),
			(3518, OutsideSegment, Brake),
			(3975, OutsideSegment, Coast),
			(4236, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2182, OutsideSegment, Coast),
			(2252, OutsideSegment, Roll),
			(2666, OutsideSegment, Accelerate),
			(2876, OutsideSegment, Coast),
			(2946, OutsideSegment, Roll),
			(3147, OutsideSegment, Brake),
			(3159, OutsideSegment, Coast),
			(3183, OutsideSegment, Brake),
			(3280, OutsideSegment, Coast),
			(3517, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5426, OutsideSegment, Coast),
			(5591, OutsideSegment, Brake),
			(6600, OutsideSegment, Coast),
			(7111, OutsideSegment, Accelerate));


		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5286, OutsideSegment, Coast),
			(5356, OutsideSegment, Roll),
			(5545, OutsideSegment, Brake),
			(5557, OutsideSegment, Coast),
			(5569, OutsideSegment, Brake),
			(6002, OutsideSegment, Coast),
			(6252, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4400, OutsideSegment, Accelerate),
			(5300, 5430, OutsideSegment, Accelerate),
			(5600, 6000, OutsideSegment, Brake),
			(6150, 6375, OutsideSegment, Coast),
			(6450, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(3468, OutsideSegment, Roll),
			(3854, OutsideSegment, Brake),
			(3864, OutsideSegment, Coast),
			(3915, OutsideSegment, Brake),
			(5012, OutsideSegment, Coast),
			(5420, OutsideSegment, Accelerate));


		[Test]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(3672, OutsideSegment, Roll),
			(3890, OutsideSegment, Brake),
			(3900, OutsideSegment, Coast),
			(3910, OutsideSegment, Brake),
			(4505, OutsideSegment, Coast),
			(4723, OutsideSegment, Accelerate));


		[Test]
		public void Class5_EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5601, OutsideSegment, Coast),
			(5671, OutsideSegment, Roll),
			(6072, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(6079, OutsideSegment, Coast),
			(6149, OutsideSegment, Roll),
			(6433, OutsideSegment, Brake),
			(6445, OutsideSegment, Coast),
			(6457, OutsideSegment, Brake),
			(7286, OutsideSegment, Coast),
			(7750, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5449, OutsideSegment, Coast),
			(5519, OutsideSegment, Roll),
			(5708, OutsideSegment, Brake),
			(7162, OutsideSegment, Coast),
			(7460, OutsideSegment, Accelerate));


		[Test]
		public void Class5_EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2147, OutsideSegment, Coast),
			(2289, OutsideSegment, Brake),
			(2481, OutsideSegment, Coast),
			(2612, OutsideSegment, Accelerate),
			(5505, OutsideSegment, Coast),
			(5575, OutsideSegment, Roll),
			(5764, OutsideSegment, Brake),
			(6701, OutsideSegment, Coast),
			(6987, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2182, OutsideSegment, Coast),
			(2252, OutsideSegment, Roll),
			(2548, OutsideSegment, Accelerate),
			(3330, OutsideSegment, Coast),
			(3400, OutsideSegment, Roll),
			(3518, OutsideSegment, Brake),
			(3975, OutsideSegment, Coast),
			(4236, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(2182, OutsideSegment, Coast),
			(2252, OutsideSegment, Roll),
			(2666, OutsideSegment, Accelerate),
			(2876, OutsideSegment, Coast),
			(2946, OutsideSegment, Roll),
			(3147, OutsideSegment, Brake),
			(3159, OutsideSegment, Coast),
			(3171, OutsideSegment, Brake),
			(3280, OutsideSegment, Coast),
			(3517, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5426, OutsideSegment, Coast),
			(5591, OutsideSegment, Brake),
			(6600, OutsideSegment, Coast),
			(7111, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(5286, OutsideSegment, Coast),
			(5356, OutsideSegment, Roll),
			(5545, OutsideSegment, Brake),
			(6002, OutsideSegment, Coast),
			(6252, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4400, OutsideSegment, Accelerate),
			(5300, 5430, OutsideSegment, Accelerate),
			(5600, 6000, OutsideSegment, Brake),
			(6150, 6375, OutsideSegment, Coast),
			(6450, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(3468, OutsideSegment, Roll),
			(3854, OutsideSegment, Brake),
			(3864, OutsideSegment, Coast),
			(3884, OutsideSegment, Brake),
			(5012, OutsideSegment, Coast),
			(5420, OutsideSegment, Accelerate));


		[Test]
		public void Class5_EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(3672, OutsideSegment, Roll),
			(3890, OutsideSegment, Brake),
			(4505, OutsideSegment, Coast),
			(4723, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4025, WithinSegment, Accelerate),
			(5309, UseCase1, Roll),
			(5908, OutsideSegment, Coast),
			(5920, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4352, WithinSegment, Accelerate),
			(5216, UseCase1, Roll),
			(6698, WithinSegment, Coast),
			(6769, WithinSegment, Roll),
			(7169, WithinSegment, Brake),
			(7182, WithinSegment, Coast),
			(7551, OutsideSegment, Coast),
			(7911, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3850, WithinSegment, Accelerate),
			(4807, UseCase1, Roll),
			(5935, WithinSegment, Coast),
			(6141, WithinSegment, Brake),
			(7154, WithinSegment, Coast),
			(7278, OutsideSegment, Coast),
			(7579, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(630, WithinSegment, Accelerate),
			(1786, UseCase1, Roll),
			(2500, OutsideSegment, Coast),
			(2547, OutsideSegment, Accelerate),
			(3912, WithinSegment, Accelerate),
			(4834, UseCase1, Roll),
			(5994, WithinSegment, Coast),
			(6272, WithinSegment, Brake),
			(6693, WithinSegment, Coast),
			(6804, OutsideSegment, Coast),
			(7093, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(654, WithinSegment, Accelerate),
			(2007, UseCase1, Roll),
			(2411, WithinSegment, Coast),
			(2434, WithinSegment, Accelerate),
			(2947, UseCase1, Roll),
			(3722, WithinSegment, Coast),
			(4060, OutsideSegment, Coast),
			(4299, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(665, WithinSegment, Accelerate),
			(2007, UseCase1, Roll),
			(3172, WithinSegment, Coast),
			(3243, WithinSegment, Roll),
			(3350, OutsideSegment, Roll),
			(3539, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3920, WithinSegment, Accelerate),
			(4994, UseCase1, Roll),
			(5774, WithinSegment, Coast),
			(6053, WithinSegment, Brake),
			(6572, WithinSegment, Coast),
			(6904, OutsideSegment, Coast),
			(7323, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3722, WithinSegment, Accelerate),
			(4609, UseCase1, Roll),
			(5819, WithinSegment, Coast),
			(5890, WithinSegment, Roll),
			(6094, OutsideSegment, Roll),
			(6331, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),
			(5364, 5623, UseCase2, Roll),
			(5623, 5841, WithinSegment, Coast),
			(5841, 6113, WithinSegment, Brake),
			(6113, 6224, WithinSegment, Coast),
			(6224, 6514, OutsideSegment, Coast),
			(6514, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(3468, OutsideSegment, Roll),
			(3854, OutsideSegment, Brake),
			(3864, OutsideSegment, Coast),
			(3915, OutsideSegment, Brake),
			(5012, OutsideSegment, Coast),
			(5420, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(3672, OutsideSegment, Roll),
			(3890, OutsideSegment, Brake),
			(3900, OutsideSegment, Coast),
			(3910, OutsideSegment, Brake),
			(4505, OutsideSegment, Coast),
			(4723, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4025, WithinSegment, Accelerate),
			(5309, UseCase1, Roll),
			(5908, OutsideSegment, Coast),
			(5932, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(4352, WithinSegment, Accelerate),
			(5216, UseCase1, Roll),
			(6698, WithinSegment, Coast),
			(6757, WithinSegment, Roll),
			(7158, WithinSegment, Brake),
			(7170, WithinSegment, Coast),
			(7552, OutsideSegment, Coast),
			(7912, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3850, WithinSegment, Accelerate),
			(4807, UseCase1, Roll),
			(5935, WithinSegment, Coast),
			(6141, WithinSegment, Brake),
			(7155, WithinSegment, Coast),
			(7278, OutsideSegment, Coast),
			(7579, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(630, WithinSegment, Accelerate),
			(1786, UseCase1, Roll),
			(2500, OutsideSegment, Coast),
			(2559, OutsideSegment, Accelerate),
			(3912, WithinSegment, Accelerate),
			(4834, UseCase1, Roll),
			(5994, WithinSegment, Coast),
			(6273, WithinSegment, Brake),
			(6693, WithinSegment, Coast),
			(6804, OutsideSegment, Coast),
			(7093, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(654, WithinSegment, Accelerate),
			(2007, UseCase1, Roll),
			(2411, WithinSegment, Coast),
			(2434, WithinSegment, Accelerate),
			(2947, UseCase1, Roll),
			(3722, WithinSegment, Coast),
			(4061, OutsideSegment, Coast),
			(4299, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(665, WithinSegment, Accelerate),
			(2007, UseCase1, Roll),
			(3172, WithinSegment, Coast),
			(3231, WithinSegment, Roll),
			(3350, OutsideSegment, Roll),
			(3539, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3920, WithinSegment, Accelerate),
			(4994, UseCase1, Roll),
			(5774, WithinSegment, Coast),
			(6040, WithinSegment, Brake),
			(6572, WithinSegment, Coast),
			(6904, OutsideSegment, Coast),
			(7323, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate),
			(3722, WithinSegment, Accelerate),
			(4609, UseCase1, Roll),
			(5819, WithinSegment, Coast),
			(5890, WithinSegment, Roll),
			(6094, OutsideSegment, Roll),
			(6332, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),
			(3547, 4430, WithinSegment, Accelerate),
			(5364, 5623, UseCase2, Roll),
			(5623, 5841, WithinSegment, Coast),
			(5841, 6113, WithinSegment, Brake),
			(6113, 6225, WithinSegment, Coast),
			(6225, 6514, OutsideSegment, Coast),
			(6514, 1e6, OutsideSegment, Accelerate));

		[Test]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3410, OutsideSegment, Coast),
			(3468, OutsideSegment, Roll),
			(3854, OutsideSegment, Brake),
			(3864, OutsideSegment, Coast),
			(3884, OutsideSegment, Brake),
			(5012, OutsideSegment, Coast),
			(5420, OutsideSegment, Accelerate));


		[Test]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).JoinString("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(474, OutsideSegment, Accelerate),
			(3614, OutsideSegment, Coast),
			(3672, OutsideSegment, Roll),
			(3890, OutsideSegment, Brake),
			(4505, OutsideSegment, Coast),
			(4723, OutsideSegment, Accelerate));


		private void TestPCC(string jobName, string cycleName, params (double distance, DefaultDriverStrategy.PCCStates pcc, DrivingAction action)[] data)
		{
			var expected = new List<(double start, double end, DefaultDriverStrategy.PCCStates pcc, DrivingAction action)>(
					data.Pairwise().Select(x => (x.Item1.distance, x.Item2.distance, x.Item1.pcc, x.Item1.action)))
				{ (data.Last().distance, 1e6d, data.Last().pcc, data.Last().action) };
			TestPCC(jobName, cycleName, expected.ToArray());
		}

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
			Assert.IsTrue(run.FinishedWithoutErrors);

			PrintPCCSections(mod);

			var expected = data;

			var segmentWasTested = false;

			var dists = mod.Columns[ModalResultField.dist.GetName()].Values<Meter>();
			var pccs = mod.Columns["PCCState"].Values<DefaultDriverStrategy.PCCStates>();
			var actions = mod.Columns["DriverAction"].Values<DrivingAction>();
			var vActs = mod.Columns[ModalResultField.v_act.GetName()].Values<MeterPerSecond>();

			using (var exp = expected.AsEnumerable().GetEnumerator()) {
				exp.MoveNext();
				foreach (var (dist, pcc, action, vAct) in dists.Zip(pccs, actions, vActs)) {
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

			if (sections.Any()) {
				var start = 0d;
				Console.WriteLine("Found Segments:");
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {section.Before.Item1}, {section.Before.Item2}),",-40} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({start}, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
				Console.WriteLine();

				Console.WriteLine("Start-End Segments:");
				start = 0;
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {(int)section.Distance.Value()}, {section.Before.Item1}, {section.Before.Item2}),",-45} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({(int)sections.Last().Distance.Value()}, 1e6, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
			} else {
				Console.WriteLine("Found Segments:");
				Console.WriteLine("(0, OutsideSegment, Accelerate));");
				Console.WriteLine();
				Console.WriteLine("Start-End Segments:");
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