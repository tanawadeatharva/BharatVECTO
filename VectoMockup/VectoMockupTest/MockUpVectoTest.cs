using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.XML.Reports;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VectoMockupTest
{



	[TestFixture]
    public class MockUpVectoTest
	{

		private const string BasePath = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\";

		private const string XsdPath = @".. /../../../../VectoCore/VectoCore/Resources/XSD";

		private IKernel _vectoKernel;
		private ISimulatorFactoryFactory _simFactoryFactory;
		private ISimulatorFactory _simulatorFactory;
		private IXMLInputDataReader _inputDataReader;

		#region Lorry Testfiles
		protected const string ConventionalHeavyLorry = BasePath + @"HeavyLorry\Conventional_heavyLorry_AMT.xml";
		protected const string HEV_Px_HeavyLorry = BasePath + @"HeavyLorry\HEV_heavyLorry_AMT_Px_IHPC.xml";
		protected const string HEV_S2_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml";
		protected const string HEV_S3_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_S3.xml";
		protected const string HEV_S3_HeavyLorry_ovc = BasePath + @"HeavyLorry\HEV-S_heavyLorry_S3_ovc.xml";
		protected const string HEV_S4_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_S4.xml";
		protected const string HEV_IEPC_S_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml";
		protected const string PEV_E2_HeavyLorry = BasePath + @"HeavyLorry\PEV_heavyLorry_AMT_E2.xml";
		protected const string PEV_E3_HeavyLorry = BasePath + @"HeavyLorry\PEV_heavyLorry_E3.xml";
		protected const string PEV_E4_HeavyLorry = BasePath + @"HeavyLorry\PEV_heavyLorry_E4.xml";
		protected const string PEV_IEPC_HeavyLorry = BasePath + @"HeavyLorry\IEPC_heavyLorry.xml";

		#endregion

		#region PrimaryBus

		protected const string Conventional_PrimaryBus = BasePath + @"PrimaryBus\Conventional_primaryBus_AMT.xml";
		protected const string Conventional_PrimaryBus_Tyres = BasePath + @"PrimaryBus\Conventional_primaryBus_AMT_DifferentTyres.xml";
		protected const string HEV_Px_IHPC_PrimaryBus = BasePath + @"PrimaryBus\HEV_primaryBus_AMT_Px.xml";
		protected const string HEV_S2_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_AMT_S2.xml";
		protected const string HEV_S3_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_S3.xml";
		protected const string HEV_S4_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_S4.xml";
		protected const string HEV_IEPC_S_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_IEPC-S.xml";
		protected const string PEV_E2_PrimaryBus = BasePath + @"PrimaryBus\PEV_primaryBus_AMT_E2.xml";
		protected const string PEV_E3_PrimaryBus = BasePath + @"PrimaryBus\PEV_primaryBus_E3.xml";
		protected const string PEV_E4_PrimaryBus = BasePath + @"PrimaryBus\PEV_primaryBus_E4.xml";
		protected const string PEV_IEPC_PrimaryBus = BasePath + @"PrimaryBus\IEPC_primaryBus.xml";

		#endregion

		#region interim bus

		protected const string Conventional_InterimBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_consolidated_multiple_stages.xml";

		protected const string Conventional_StageInput =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\vecto_vehicle-stage_input_full-sample.xml";
		#endregion

		#region completed bus

		protected const string Conventional_CompletedBus = @"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_conventional_final_vif.VIF_Report_1.xml";
		#endregion

		#region special cases multistage

		private const string TestDataDir = "TestData\\";

		private const string CompletedDiesel = TestDataDir + "Integration\\Multistage\\newVifCompletedConventional.vecto";
		private const string CompletedExempted = TestDataDir + "Integration\\Multistage\\newVifExempted.vecto";
		private const string CompletedExemptedWithoutTPMLM = TestDataDir + "Integration\\Multistage\\newVifExempted-noTPMLM.vecto";
		private string CompletedWithoutADAS = TestDataDir + "Integration\\Multistage\\newVifCompletedConventional-noADAS.vecto";




		private const string InterimExempted = TestDataDir + "Integration\\Multistage\\newVifExemptedIncomplete.vecto";
		private const string InterimDiesel = TestDataDir + "Integration\\Multistage\\newVifInterimDiesel.vecto";


		#endregion

		#region GroupTest

		private const string GroupTestDir = @"TestData\XML\XMLReaderDeclaration\GroupTest\";
		


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

		private void Clearfiles(FileOutputWriter fileWriter)
		{
			IList<string> filesToBeCleared = new List<string>() {
				fileWriter.XMLPrimaryVehicleReportName,
				fileWriter.XMLFullReportName,
				fileWriter.XMLCustomerReportName
			};
			foreach (var fileName in filesToBeCleared) {
				if (File.Exists(fileName)) {
					File.Delete(fileName);
				}
			}
		}
		public FileOutputWriter GetOutputFileWriter(string subDirectory, string originalFilePath)
		{
			subDirectory = Path.Combine("MockupReports",subDirectory);
			Directory.CreateDirectory(Path.GetFullPath(subDirectory));
			var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileName(originalFilePath));
			return new FileOutputWriter(path);
		}





		[TestCase(ConventionalHeavyLorry, TestName = "ConventionalHeavyLorry")]
		//[TestCase(ConventionalHeavyLorry, false, TestName = "ConventionalHeavyLorryNoMockup")]
		[TestCase(HEV_S2_HeavyLorry, TestName = "HEV_S2_HeavyLorry")]
		[TestCase(HEV_S3_HeavyLorry, TestName = "HEV_S3_HeavyLorry")]
		[TestCase(HEV_S3_HeavyLorry_ovc, TestName = "HEV_S3_HeavyLorry_ovc")]
		[TestCase(HEV_S4_HeavyLorry, TestName = "HEV_S4_HeavyLorry")]
		[TestCase(HEV_Px_HeavyLorry, TestName = "HEV_Px_HeavyLorry")]
		[TestCase(PEV_E2_HeavyLorry, TestName = "PEV_E2_HeavyLorry")]
		//[TestCase(PEV_E2_HeavyLorry, false, TestName = "PEV_E2_HeavyLorryNoMockup")]
		[TestCase(PEV_E3_HeavyLorry, TestName = "PEV_E3_HeavyLorry")]
		[TestCase(PEV_E4_HeavyLorry, TestName = "PEV_E4_HeavyLorry")]
		[TestCase(PEV_IEPC_HeavyLorry, TestName = "PEV_IEPC_HeavyLorry")]
		[TestCase(HEV_IEPC_S_HeavyLorry, TestName = "HEV_IEPC_S_HeavyLorry")]
		//[NonParallelizable]
		[TestCase(BasePath + @"MediumLorry/Conventional_mediumLorry_AMT.xml", TestName="Conventional_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/HEV-S_mediumLorry_AMT_S2.xml", TestName="HEV_S2_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/HEV-S_mediumLorry_IEPC-S.xml", TestName="HEV_IEPC_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/HEV-S_mediumLorry_S3.xml", TestName="HEV_S3_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/HEV-S_mediumLorry_S4.xml", TestName="HEV_S4_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/HEV_mediumLorry_AMT_Px.xml", TestName="HEV_Px_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/IEPC_mediumLorry.xml", TestName="PEV_IEPC_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/PEV_mediumLorry_AMT_E2.xml", TestName="PEV_E2_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/PEV_mediumLorry_AMT_E2_EM_Std.xml", TestName="PEV_E2_std_Medium_Lorry", Ignore="Segment not found")]
		[TestCase(BasePath + @"MediumLorry/PEV_mediumLorry_APT-N_E2.xml", TestName="PEV_E2_Medium_Lorry_2")]
		[TestCase(BasePath + @"MediumLorry/PEV_mediumLorry_E3.xml", TestName="PEV_E3_Medium_Lorry")]
		[TestCase(BasePath + @"MediumLorry/PEV_mediumLorry_E4.xml", TestName="PEV_E4_Medium_Lorry")]
		public void LorryMockupTest(string fileName, bool mockup = true)
		{
			
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		[TestCase(Conventional_PrimaryBus, TestName = "ConventionalPrimaryBus")]
		[TestCase(Conventional_PrimaryBus_Tyres, TestName = "ConventionalPrimaryBus Tyres")]
        [TestCase(HEV_IEPC_S_PrimaryBus, TestName="HEV_IEPC_S_PrimaryBus")]
        [TestCase(HEV_Px_IHPC_PrimaryBus, TestName="HEV_Px_PrimaryBus")]
        [TestCase(HEV_S2_PrimaryBus, TestName="HEV_S2_PrimaryBus")]
		[TestCase(HEV_S3_PrimaryBus, TestName = "HEV_S3_PrimaryBus")]
		[TestCase(HEV_S4_PrimaryBus, TestName = "HEV_S4_PrimaryBus")]
        [TestCase(PEV_E2_PrimaryBus, TestName="PEV_E2_PrimaryBus")]
		[TestCase(PEV_E3_PrimaryBus, TestName = "PEV_E3_PrimaryBus")]
		[TestCase(PEV_E4_PrimaryBus, TestName = "PEV_E4_PrimaryBus")]
        [TestCase(PEV_IEPC_PrimaryBus, TestName="PEV_IEPC_PrimaryBus")]
		public void PrimaryBusMockupTest(string fileName, bool mockup = true)
		{
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			Clearfiles(fileWriter); //remove files from previous test runs
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif:false, checkPrimaryReport:true);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLPrimaryVehicleReportName), XmlDocumentType.MultistepOutputData), "VIF invalid" );
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
		}

		
		[TestCase(Conventional_InterimBus, Conventional_StageInput, TestName = "ConventionalInterimBus")]
		public void InterimBusMockupTest(string vifInput, string stageInputFile)
		{
			//SimulatorFactory.MockUpRun = mockup;
			var multistageBusInput = _inputDataReader.Create(vifInput) as IMultistageBusInputDataProvider;
			Assert.NotNull(multistageBusInput);

			var stageInput = _inputDataReader.CreateDeclaration(stageInputFile);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, vifInput);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = new XMLDeclarationVIFInputData(multistageBusInput, stageInput.JobInputData.Vehicle);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputData, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkMrf:false, checkCif: false, checkVif: true);


		}


		[TestCase(Conventional_CompletedBus, TestName = "ConventionalCompletedBus")]
		public void CompletedBusMockupTest(string fileName)
		{
			//SimulatorFactory.MockUpRun = mockup;
			XMLDeclarationVIFInputData input = null!;
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			using (var reader = XmlReader.Create(fileName))
			{
				input = new XMLDeclarationVIFInputData(_inputDataReader.Create(fileName) as IMultistageBusInputDataProvider, null);
				fileWriter = new FileOutputVIFWriter(fileName, input.MultistageJobInputData.JobInputData.ManufacturingStages.Count);
			}
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, input, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif: true, checkVif: false);
		}

		[TestCase(CompletedDiesel, TestName="CompletedDiesel")]
        [TestCase(CompletedExempted, TestName = "CompleteExempted Bus")]
        [TestCase(CompletedExemptedWithoutTPMLM, TestName = "CompletedExempted No TPMLM")]
		public void PrimaryAndCompletedTest(string fileName)
		{
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var input = JSONInputDataFactory.ReadJsonJob(fileName);
			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, input, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif: true, checkVif: true, checkMrf: true, checkPrimaryMrf: true);
		}

		[TestCase(InterimDiesel, TestName = "PrimaryAndInterim")]
		public void PrimaryAndInterim(string fileName)
		{

			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var input = JSONInputDataFactory.ReadJsonJob(fileName);
			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, input, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, checkCif: false, checkVif: true, checkMrf:false, checkPrimaryMrf:true);



		}






		private static void CheckFileExists(FileOutputWriter fileWriter, 
			bool checkMrf = true,
			bool checkCif = true, 
			bool checkVif = false, 
			bool checkPrimaryMrf = false,
			bool checkPrimaryReport = false)
		{
			if (checkCif) {
				if (!File.Exists(fileWriter.XMLCustomerReportName)) {
					TestContext.WriteLine(fileWriter.XMLCustomerReportName + " Missing\n");
					Assert.Fail();
				}

				MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLCustomerReportName),
					XmlDocumentType.CustomerReport);
			}
			if (checkMrf) {
				if (!File.Exists(fileWriter.XMLFullReportName)) {
					TestContext.WriteLine(fileWriter.XMLFullReportName + " Missing\n");
					Assert.Fail();
				}

				MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLFullReportName),
					XmlDocumentType.ManufacturerReport);
			}

			var primaryMrfPath = fileWriter.XMLFullReportName.Replace("RSLT_MANUFACTURER", "RSLT_MANUFACTURER_PRIMARY");
			if (checkPrimaryMrf) {
				if (!File.Exists(primaryMrfPath)) {
					TestContext.WriteLine(primaryMrfPath + " Missing\n");
					Assert.Fail();
				}

				MRF_CIF_WriterTestBase.Validate(XDocument.Load(primaryMrfPath), XmlDocumentType.ManufacturerReport);
			}


			if (checkPrimaryReport) {
				if (!File.Exists(fileWriter.XMLPrimaryVehicleReportName)) {
					TestContext.WriteLine(fileWriter.XMLPrimaryVehicleReportName + " Missing\n");
					Assert.Fail();
				}

				MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLPrimaryVehicleReportName),
					XmlDocumentType.MultistepOutputData);
			}


			if (checkVif) {
				if (!File.Exists(fileWriter.XMLMultistageReportFileName)) {
					TestContext.WriteLine(fileWriter.XMLMultistageReportFileName + " Missing\n");
					Assert.Fail();
				}

				MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLMultistageReportFileName),
					XmlDocumentType.MultistepOutputData);
			}

			
		}
		
		[TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml", TestName="GroupClass1")]
		[TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml",TestName="GroupClass9")]
		[TestCase(@"TestData\XML\XMLReaderDeclaration\GroupTest\Tractor_4x2_vehicle-class-5_EURO6_2018.xml", TestName="GroupClass5")]
		[TestCase(@"TestData/XML/XMLReaderDeclaration/GroupTest/Rigid Truck_8x4_vehicle-class-16_EURO6_2018.xml", TestName="GroupClass16")]
		public void GroupTestFail(string fileName, bool mockup = true)
		{
			

			IInputDataProvider inputProvider = null;
			Assert.Throws(typeof(VectoException), () => _inputDataReader.Create(fileName));
			if (inputProvider == null) {
				Assert.Pass("Test cancelled! Inputprovider == null, this is expected on unsupported Schema versions");
			}
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}
		
		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion1.0/Tractor_4x2_vehicle-class-5_5_t_0.xml", TestName="Schema10Test1")]
        
		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion1.0/vecto_vehicle-new_parameters-sample.xml",TestName="Schema10_new_parameters", Ignore = "Invalid combination for ecoroll")]
		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion1.0/vecto_vehicle-sample_LNG.xml", TestName="Schema10_vehicle_sample_lng")]
		public void Schema1_0_Test(string fileName, bool mockup = true)
		{
			
			IInputDataProvider inputProvider = null!;
			Assert.Throws(typeof(VectoException), () => _inputDataReader.Create(fileName));
			if (inputProvider == null)
			{
				Assert.Pass("Test cancelled! Inputprovider == null, this is expected on unsupported Schema versions");
			}
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}
		
		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.0/Tractor_4x2_vehicle-class-5_5_t_0.xml", TestName="Schema20Test1")]
		// [TestCase(@"", TestName="")]
		// [TestCase(@"", TestName="")]
		public void Schema2_0_Test(string fileName, bool mockup = true)
		{
			
			IInputDataProvider inputProvider = null!;
			Assert.Throws(typeof(VectoException), () => _inputDataReader.Create(fileName));
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (inputProvider == null)
			{
				Assert.Pass("Test cancelled! Inputprovider == null, this is expected on unsupported Schema versions");
			}
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}




        [TestCase("TestData/Generic Vehicles/Declaration Mode/40t Long Haul Truck/40t_Long_Haul_Truck.vecto", TestName="JSON_40TLonghaul")]
		[TestCase("TestData/Generic Vehicles/Declaration Mode/Class9_RigidTruck_6x2/Class9_RigidTruck_DECL.vecto", TestName="JSON_RigidTruckClass9")]
        [NonParallelizable]
		public void JSONTest(string fileName, bool mockup = true)
		{

			var inputProvider = JSONInputDataFactory.ReadJsonJob(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory = null!;
			Assert.Throws(typeof(VectoException), () => {
				_simulatorFactory =
					_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			});
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (_simulatorFactory == null) {
				Assert.Pass("Test cancelled! SimulatorFactory could not be created, this is expected on JSON jobs");
			}
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();

			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		private const string BasePathExempted = "TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/Distributed/ExemptedVehicles/";

   //     [TestCase(BasePathExempted + "exempted_completedBus_input_full.xml",
   //         true,
   //         true,
   //         false,
   //         false,
   //         true,
   //         TestName = "ExemptedCompletedBus1")]
   //     [TestCase(BasePathExempted + "exempted_completedBus_input_only_mandatory_entries.xml", 
			//true, 
			//true,
			//false,
			//false, 
			//false, 
			//TestName="ExemptedCompletedBus2")]
		[TestCase(BasePathExempted + "exempted_heavyLorry.xml", 
			false, 
			false,
			true,
			false, 
			false, 
			TestName="ExemptedHeavyLorry")]
		[TestCase(BasePathExempted + "exempted_mediumLorry.xml",
			false, 
			true,
			true,
			false, 
			false, 
			TestName="ExemptedMediumLorry")]
		[TestCase(BasePathExempted + "exempted_primaryBus.xml", 
			false, 
			false,
			true,
			false, 
			true, 
			TestName="ExemptedPrimaryBus")]
		public void ExemptedTest(string fileName, bool checkVif, bool checkCif, bool checkMrf, bool checkPrimaryMrf,
			bool checkPrimaryReport)
		{
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();

			CheckFileExists(fileWriter, 
				checkVif:checkVif, 
				checkCif:checkCif, 
				checkMrf:checkMrf, 
				checkPrimaryMrf:checkPrimaryMrf, 
				checkPrimaryReport:checkPrimaryReport);
			if (checkMrf) Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			if (checkPrimaryReport) Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLPrimaryVehicleReportName), XmlDocumentType.MultistepOutputData), "VIF invalid");
			if (checkCif) Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		public void ExemptedCompleteBusTest()
		{
			var primaryInput = BasePathExempted + "exempted_primaryBus.xml";
			var completeInput = BasePathExempted + "exempted_completedBus_input_full.xml";


		}
	}
}
