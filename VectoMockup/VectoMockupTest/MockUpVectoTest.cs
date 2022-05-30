using System.IO;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.XML.Reports;

namespace VectoMockupTest
{



	[TestFixture]
    public class MockUpVectoTest
	{



		private const string XsdPath = @"../../../../../VectoCore/VectoCore/Resources/XSD";

		private IKernel _vectoKernel;
		private ISimulatorFactoryFactory _simFactoryFactory;
		private ISimulatorFactory _simulatorFactory;
		private IXMLInputDataReader _inputDataReader;

		#region TestFiles
		protected const string ConventionalHeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\Conventional_heavyLorry_AMT.xml";
		protected const string HEV_Px_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV_heavyLorry_AMT_Px_IHPC.xml";
		protected const string HEV_S2_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml";
		protected const string HEV_S3_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_S3.xml";
		protected const string HEV_S3_HeavyLorry_ovc =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_S3_ovc.xml";
		protected const string HEV_S4_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_S4.xml";
		protected const string HEV_IEPC_S_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml";
		protected const string PEV_E2_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\PEV_heavyLorry_AMT_E2.xml";
		protected const string PEV_E3_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\PEV_heavyLorry_E3.xml";
		protected const string PEV_E4_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\PEV_heavyLorry_E4.xml";
		protected const string PEV_IEPC_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\IEPC_heavyLorry.xml";

		protected const string Conventional_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\Conventional_primaryBus_AMT.xml";
		protected const string HEV_Px_IHPC_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV_primaryBus_AMT_Px.xml";
		protected const string HEV_S2_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_AMT_S2.xml";
		protected const string HEV_S3_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_S3.xml";
		protected const string HEV_S4_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_S4.xml";
		protected const string HEV_IEPC_S_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_IEPC-S.xml";
		protected const string PEV_E2_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\PEV_primaryBus_AMT_E2.xml";
		protected const string PEV_E3_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\PEV_primaryBus_E3.xml";
		protected const string PEV_E4_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\PEV_primaryBus_E4.xml";
		protected const string PEV_IEPC_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\IEPC_primaryBus.xml";

		protected const string Conventional_InterimBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_consolidated_multiple_stages.xml";



		protected const string Conventional_CompletedBus = @"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_conventional_final_vif.VIF_Report_1.xml";

		#endregion


		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_vectoKernel = new StandardKernel(
				new VectoNinjectModule()
				);

			_simFactoryFactory = _vectoKernel.Get<ISimulatorFactoryFactory>();
			Assert.NotNull(_simFactoryFactory);
			_inputDataReader = _vectoKernel.Get<IXMLInputDataReader>();
			Assert.NotNull(_inputDataReader);
		}

		[SetUp]
		public void Setup()
		{
			//SimulatorFactory.MockUpRun = true;

		}

		public FileOutputWriter GetOutputFileWriter(string subDirectory, string originalFilePath)
		{
			subDirectory = Path.Combine("MockupReports",subDirectory);
			Directory.CreateDirectory(Path.GetFullPath(subDirectory));
			var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileName(originalFilePath));
			return new FileOutputWriter(path);
		}


	
		

		[TestCase(ConventionalHeavyLorry, TestName="ConventionalHeavyLorry")]
		//[TestCase(ConventionalHeavyLorry, false, TestName = "ConventionalHeavyLorryNoMockup")]
		[TestCase(HEV_S2_HeavyLorry, TestName = "HEV_S2_HeavyLorry")]
		[TestCase(HEV_S3_HeavyLorry, TestName = "HEV_S3_HeavyLorry")]
		[TestCase(HEV_S3_HeavyLorry_ovc, TestName="HEV_S3_HeavyLorry_ovc")]
		[TestCase(HEV_S4_HeavyLorry, TestName = "HEV_S4_HeavyLorry")]
		[TestCase(HEV_Px_HeavyLorry, TestName = "HEV_Px_HeavyLorry")]
		[TestCase(PEV_E2_HeavyLorry, TestName = "PEV_E2_HeavyLorry")]
		//[TestCase(PEV_E2_HeavyLorry, false, TestName = "PEV_E2_HeavyLorryNoMockup")]
		[TestCase(PEV_E3_HeavyLorry, TestName = "PEV_E3_HeavyLorry")]
		[TestCase(PEV_E4_HeavyLorry, TestName = "PEV_E4_HeavyLorry")]
		[TestCase(PEV_IEPC_HeavyLorry, TestName = "PEV_IEPC_HeavyLorry")]
		[TestCase(HEV_IEPC_S_HeavyLorry, TestName = "HEV_IEPC_S_HeavyLorry")]
		//[NonParallelizable]
		public void HeavyLorryMockupTest(string fileName, bool mockup = true)
		{
			
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName),XsdPath), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName),XsdPath), "CIF invalid");
		}

		[TestCase(Conventional_PrimaryBus, TestName = "ConventionalPrimaryBus")]
		public void PrimaryBusMockupTest(string fileName, bool mockup = true)
		{
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif:false, checkVif:true);
		}

		[TestCase(Conventional_InterimBus, TestName = "ConventionalInterimBus")]
		public void InterimBusMockupTest(string fileName)
		{
			//SimulatorFactory.MockUpRun = mockup;
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif: false, checkVif: true);


		}


		[TestCase(Conventional_CompletedBus, TestName = "ConventionalCompletedBus")]
		public void CompletedBusMockupTest(string fileName)
		{
			//SimulatorFactory.MockUpRun = mockup;
			XMLDeclarationVIFInputData input = null;
			var fileWriter = new FileOutputWriter(fileName);
			using (var reader = XmlReader.Create(fileName))
			{
				input = new XMLDeclarationVIFInputData(_inputDataReader.Create(fileName) as IMultistageBusInputDataProvider, null);
				fileWriter = new FileOutputVIFWriter(fileName, input.MultistageJobInputData.JobInputData.ManufacturingStages.Count);
			}
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, input, fileWriter, null, null, true);

			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif: false, checkVif: true);


		}


		private static void CheckFileExists(FileOutputWriter fileWriter, bool checkMrf = true, bool checkCif = true, bool checkVif = false)
		{
			if (checkCif && !File.Exists(fileWriter.XMLCustomerReportName)) {
				TestContext.WriteLine(fileWriter.XMLCustomerReportName);
				Assert.Fail();
			}
			if (checkMrf && !File.Exists(fileWriter.XMLFullReportName))
			{
				TestContext.WriteLine(fileWriter.XMLFullReportName);
				Assert.Fail();
			}

			if (checkVif && !File.Exists(fileWriter.XMLPrimaryVehicleReportName)) {
				TestContext.WriteLine(fileWriter.XMLPrimaryVehicleReportName);
				Assert.Fail();
			}
		}
	}
}
