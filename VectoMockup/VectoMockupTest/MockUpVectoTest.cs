using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.XML.Reports;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VectoMockupTest
{



	[TestFixture]
    public class MockUpVectoTest
	{
		private const string v27LorryPath = @"TestDataMockup/SchemaVersion2.7/Lorries/";
		private const string v27PrimaryBusPath = @"TestDataMockup/SchemaVersion2.7/PrimaryBuses/";
		private const string v27CompleteBusPath = @"TestDataMockup/SchemaVersion2.7/CompletedBuses/";

		private const string BasePath = @"TestDataMockup\SchemaVersion2.4\Distributed\";

		private const string BasePathMockup = @"TestDataMockup\SchemaVersion2.4\MockupBusTest\";

		private IKernel _vectoKernel;
		private ISimulatorFactoryFactory _simFactoryFactory;
		private IXMLInputDataReader _inputDataReader;

		#region Heavy Lorry Testfiles
		protected const string ConventionalHeavyLorry = BasePath + @"HeavyLorry\Conventional_heavyLorry_AMT.xml";
		protected const string ConventionalHeavyLorry_DifferentTyres = BasePathMockup + @"HeavyLorry\Conventional_heavyLorry_AMT_DifferentTyres.xml";
		protected const string ConventionalHeavyLorry_AT_Angledrive = BasePathMockup + @"HeavyLorry\Conventional_heavyLorry_AT_Angledrive.xml";
		protected const string ConventionalHeavyLorry_NoRetarder = BasePathMockup + @"HeavyLorry\Conventional_heavyLorry_AMT_NoRetarder.xml";
		protected const string ConventionalHeavyLorry_NoAirdrag = BasePathMockup + @"HeavyLorry\Conventional_heavyLorry_AMT_NoAirdrag.xml";

		protected const string ConventionalHeavyLorry_Vocational =
			BasePath + @"HeavyLorry\Conventional_heavyLorry_AMT_Vocational.xml";

		protected const string HEV_Px_HeavyLorry = BasePath + @"HeavyLorry\HEV_heavyLorry_AMT_Px.xml";
		protected const string HEV_Px_HeavyLorry_BatteryStd = BasePathMockup + @"HeavyLorry\HEV_heavyLorry_Px_ADC_BatteryStd.xml";
		protected const string HEV_S2_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml";
		protected const string HEV_S2_HeavyLorry_NoRetarder = BasePathMockup + @"HeavyLorry\HEV-S_heavyLorry_AMT_S2_NoRetarder.xml";
		protected const string HEV_S3_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_S3.xml";
		protected const string HEV_S3_HeavyLorry_ovc = BasePath + @"HeavyLorry\HEV-S_heavyLorry_S3_ovc.xml";
		protected const string HEV_S4_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_S4.xml";
		protected const string HEV_IEPC_S_HeavyLorry = BasePath + @"HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml";
		protected const string PEV_E2_HeavyLorry = BasePath + @"HeavyLorry\PEV_heavyLorry_AMT_E2.xml";
		protected const string PEV_E2_HeavyLorry_NoRetarder = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_AMT_E2_NoRetarder.xml";
		protected const string PEV_E2_HeavyLorry_NoAirdrag = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_AMT_E2_NoAirdrag.xml";
		protected const string PEV_E2_HeavyLorry_Vocational = BasePath + @"HeavyLorry\PEV_heavyLorry_AMT_E2_Vocational.xml";
		protected const string PEV_E2_HeavyLorry_BatteryStd = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_AMT_E2_BatteryStd.xml";
		protected const string PEV_E3_HeavyLorry = BasePath + @"HeavyLorry\PEV_heavyLorry_E3.xml";
		protected const string PEV_E4_HeavyLorry = BasePath + @"HeavyLorry\PEV_heavyLorry_E4.xml";
		protected const string PEV_IEPC_HeavyLorry = BasePath + @"HeavyLorry\IEPC_heavyLorry.xml";

		protected const string PEV_IEPC_HeavyLorry_Gbx1 = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx1.xml";
		protected const string PEV_IEPC_HeavyLorry_Gbx1Axl = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx1Axl.xml";
		protected const string PEV_IEPC_HeavyLorry_Gbx1Whl = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx1Axl.xml";

		protected const string PEV_IEPC_HeavyLorry_Gbx2 = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx3.xml";
		protected const string PEV_IEPC_HeavyLorry_Gbx2_drag = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx3_drag.xml";
		protected const string PEV_IEPC_HeavyLorry_Gbx2Axl = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx3Axl.xml";
		protected const string PEV_IEPC_HeavyLorry_Gbx2Axl_drag = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx3Axl_drag.xml";
		protected const string PEV_IEPC_HeavyLorry_Gbx2Whl = BasePathMockup + @"HeavyLorry\PEV_heavyLorry_IEPC_Gbx3Axl.xml";

		protected const string HEV_IHPC_HeavyLorry = BasePathMockup + @"HeavyLorry\HEV_heavyLorry_IHPC.xml";
		protected const string HEV_Px_HeavyLorry_NoRetarder = BasePathMockup + @"HeavyLorry\HEV_heavyLorry_AMT_Px_NoRetarder.xml";
		protected const string HEV_Px_HeavyLorry_NoAirDrag = BasePathMockup + @"HeavyLorry\HEV_heavyLorry_AMT_Px_NoAirdrag.xml";
		protected const string HEV_Px_HeavyLorry_ADC = BasePathMockup + @"HeavyLorry\HEV_heavyLorry_Px_ADC.xml";
		protected const string HEV_S3_HeavyLorry_ADC = BasePathMockup + @"HeavyLorry\HEV-S_heavyLorry_S3_ADC_GenSetADC.xml";
		protected const string HEV_Px_HeavyLorry_SuperCap = BasePathMockup + @"HeavyLorry\HEV_heavyLorry_Px_SuperCap.xml";

		#endregion

		#region Medium Lorry Testfiles

		protected const string Conventional_mediumLorry_AMT = BasePath + @"MediumLorry/Conventional_mediumLorry_AMT.xml";
		protected const string HEV_S_mediumLorry_AMT_S2 = BasePath + @"MediumLorry/HEV-S_mediumLorry_AMT_S2.xml";
		protected const string HEV_S_mediumLorry_IEPC_S = BasePath + @"MediumLorry/HEV-S_mediumLorry_IEPC-S.xml";
		protected const string HEV_S_mediumLorry_S3 = BasePath + @"MediumLorry/HEV-S_mediumLorry_S3.xml";
		protected const string HEV_S_mediumLorry_S3_BatteryStd = BasePathMockup + @"MediumLorry/HEV-S_mediumLorry_S3_BatteryStd.xml";
		protected const string HEV_S_mediumLorry_S4 = BasePath + @"MediumLorry/HEV-S_mediumLorry_S4.xml";
		protected const string HEV_mediumLorry_AMT_Px = BasePath + @"MediumLorry/HEV_mediumLorry_AMT_Px.xml";
		protected const string IEPC_mediumLorry = BasePath + @"MediumLorry/IEPC_mediumLorry.xml";
		protected const string PEV_mediumLorry_AMT_E2 = BasePath + @"MediumLorry/PEV_mediumLorry_AMT_E2.xml";
		protected const string PEV_mediumLorry_AMT_E2_EM_Std = BasePath + @"MediumLorry/PEV_mediumLorry_AMT_E2_EM_Std.xml";
		protected const string PEV_mediumLorry_APT_N_E2 = BasePath + @"MediumLorry/PEV_mediumLorry_APT-N_E2.xml";
		protected const string PEV_mediumLorry_E3 = BasePath + @"MediumLorry/PEV_mediumLorry_E3.xml";
		protected const string PEV_mediumLorry_E4 = BasePath + @"MediumLorry/PEV_mediumLorry_E4.xml";
		protected const string PEV_mediumLorry_E3_BatteryStd = BasePathMockup + @"MediumLorry/PEV_mediumLorry_E3_BatteryStd.xml";

		protected const string HEV_mediumLorry_IHPC = BasePathMockup + @"MediumLorry/HEV_mediumLorry_IHPC.xml";
		protected const string HEV_mediumLorry_Px_SuperCap = BasePathMockup + @"MediumLorry/HEV_mediumLorry_Px_SuperCap.xml";
		protected const string PEV_mediumLorry_AMT_E2_BatStd = BasePathMockup + @"MediumLorry/PEV_mediumLorry_AMT_E2_BatteryStd.xml";
		//protected const string HEV_mediumLorry_Px_ADC = BasePathMockup + @"MediumLorry/HEV_heavyLorry_IHPC.xml";
		//protected const string HEV_mediumLorry_S3_ADC_GenSetADC = BasePathMockup + @"MediumLorry/HEV_heavyLorry_IHPC.xml";
		#endregion

		#region PrimaryBus

		protected const string Conventional_PrimaryBus = BasePath + @"PrimaryBus\Conventional_primaryBus_AMT.xml";
		protected const string Conventional_PrimaryBus_AT_Angledrive = BasePathMockup + @"PrimaryBus\Conventional_primaryBus_AT_Angledrive.xml";
		protected const string Conventional_PrimaryBus_NoRetarder = BasePathMockup + @"PrimaryBus\Conventional_primaryBus_AT_NoRetarder.xml";
		protected const string Conventional_PrimaryBus_RetarderMeasured = BasePathMockup + @"PrimaryBus\Conventional_primaryBus_AMT_RetarderMeasured.xml";
		protected const string Conventional_PrimaryBus_Tyres = BasePath + @"PrimaryBus\Conventional_primaryBus_AMT_DifferentTyres.xml";
		protected const string HEV_Px_PrimaryBus = BasePath + @"PrimaryBus\HEV_primaryBus_AMT_Px.xml";
		protected const string HEV_Px_PrimaryBus_BatteryStd = BasePathMockup + @"PrimaryBus\HEV_primaryBus_AMT_Px_BatteryStd.xml";
		protected const string HEV_IHPC_PrimaryBus = BasePathMockup + @"PrimaryBus\HEV_primaryBus_AMT_IHPC.xml";
		protected const string HEV_IHPC_PrimaryBus_NoRetarder = BasePathMockup + @"PrimaryBus\HEV_primaryBus_AMT_IHPC_NoRetarder.xml";
		protected const string HEV_Px_PrimaryBus_SuperCap = BasePathMockup + @"PrimaryBus\HEV_primaryBus_AMT_Px_SuperCap.xml";
		protected const string HEV_S2_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_AMT_S2.xml";
		protected const string HEV_S2_PrimaryBus_GenSetADC = BasePathMockup + @"PrimaryBus\HEV-S_primaryBus_AMT_S2_GenSetADC.xml";
		protected const string HEV_S2_PrimaryBus_ADC = BasePathMockup + @"PrimaryBus\HEV-S_primaryBus_AMT_S2_ADC.xml";
		protected const string HEV_S3_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_S3.xml";
		protected const string HEV_S4_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_S4.xml";
		protected const string HEV_IEPC_S_PrimaryBus = BasePath + @"PrimaryBus\HEV-S_primaryBus_IEPC-S.xml";
		protected const string HEV_IEPC_S_PrimaryBus_BatteryStd = BasePathMockup + @"PrimaryBus\HEV-S_primaryBus_IEPC-S_BatteryStd.xml";
		protected const string PEV_E2_PrimaryBus = BasePath + @"PrimaryBus\PEV_primaryBus_AMT_E2.xml";
		protected const string PEV_E3_PrimaryBus = BasePath + @"PrimaryBus\PEV_primaryBus_E3.xml";
		protected const string PEV_E4_PrimaryBus = BasePath + @"PrimaryBus\PEV_primaryBus_E4.xml";
		protected const string PEV_IEPC_PrimaryBus = BasePath + @"PrimaryBus\IEPC_primaryBus.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx1 = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx1.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx1Axl = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx1Axl.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx1Whl = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx1Whl.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx2 = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx2.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx2_drag = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx2_drag.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx2Axl = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx2Axl.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx2Axl_drag = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx2Axl_drag.xml";
		protected const string PEV_IEPC_PrimaryBus_Gbx2Whl = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_Gbx2Whl.xml";

		protected const string PEV_IEPC_std_PrimaryBus = BasePathMockup + @"PrimaryBus\IEPC_primaryBus_StdValues.xml";

		protected const string PEV_E2_PrimaryBus_StdEM = BasePathMockup + @"PrimaryBus\PEV_primaryBus_AMT_E2_EMStd.xml";
		protected const string PEV_E2_PrimaryBus_StdBat = BasePathMockup + @"PrimaryBus\PEV_primaryBus_AMT_E2_BatteryStd.xml";
		protected const string Conventional_PrimaryBus_DF = BasePathMockup + @"PrimaryBus\Conventional_primaryBus_AMT_DF.xml";

		#endregion

		#region Complete(d) Bus Input

		protected const string Conventional_CompletedBusInput = BasePath + @"CompletedBus\Conventional_completedBus_2.xml";
		protected const string Conventional_CompletedBusInputNoAirdrag = BasePathMockup + @"CompletedBus\Conventional_completedBus_NoAirdrag.xml";
		protected const string Conventional_CompletedBusInput_TypeApproval = BasePath + @"CompletedBus\Conventional_completedBus_2_TypeApprovalNumber.xml";
		protected const string Conventional_CompletedBusInput_AirdragV10 = BasePathMockup + @"CompletedBus\Conventional_completedBus_AirdragV10.xml";
		protected const string Conventional_CompletedBusInput_AirdragV20 = BasePathMockup + @"CompletedBus\Conventional_completedBus_AirdragV20.xml";
        protected const string HEV_CompletedBusInput = BasePath + @"CompletedBus\HEV_completedBus_2.xml";
		protected const string PEV_CompletedBusInput = BasePath + @"CompletedBus\PEV_completedBus_2.xml";
		protected const string PEV_IEPC_CompletedBusInput = BasePath + @"CompletedBus\IEPC_completedBus_2.xml";


		#endregion

		#region Interim Bus Input

		protected const string Conventional_InterimBusInput = BasePathMockup + @"CompletedBus\Conventional_InterimBus_Min.xml";
		protected const string Conventional_InterimBusInput_AirdragV10 = BasePathMockup + @"CompletedBus\Conventional_interimBus_AirdragV10.xml";
		protected const string Conventional_InterimBusInput_AirdragV20 = BasePathMockup + @"CompletedBus\Conventional_interimBus_AirdragV20.xml";
		protected const string HEV_InterimBusInput = BasePathMockup + @"CompletedBus\HEV_InterimBus_Min.xml";
		protected const string PEV_InterimBusInput = BasePathMockup + @"CompletedBus\PEV_InterimBus_Min.xml";
		protected const string PEV_IEPC_InterimBusInput = BasePathMockup + @"CompletedBus\IEPC_InterimBus_Min.xml";


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

		private const string CompleteDiesel = TestDataDir + "Integration\\Multistage\\newVifCompletedConventional.vecto";
		private const string CompleteExempted = TestDataDir + "Integration\\Multistage\\newVifExempted.vecto";
		private const string CompleteExemptedWithoutTPMLM = TestDataDir + "Integration\\Multistage\\newVifExempted-noTPMLM.vecto";
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
				fileWriter.XMLCustomerReportName,
				fileWriter.XMLMonitoringReportName
			};
			foreach (var fileName in filesToBeCleared) {
				if (File.Exists(fileName)) {
					File.Delete(fileName);
				}
			}
		}

		private string[] CopyInputFile(params string[] fileNames)
		{
			var subDirectory = Path.Combine( "MockupReports", TestContext.CurrentContext.Test.Name, "Input");
			Directory.CreateDirectory(Path.GetFullPath(subDirectory));
			var retVal = new List<string>();
			foreach (var file in fileNames) {
				var output = Path.Combine(subDirectory, Path.GetFileName(file));
				File.Copy(file, output, true);
				retVal.Add(output);
			}

			return retVal.ToArray();
		}


		public FileOutputWriter GetOutputFileWriter(string subDirectory, string originalFilePath)
		{
			subDirectory = Path.Combine("MockupReports",subDirectory);
			Directory.CreateDirectory(Path.GetFullPath(subDirectory));
			var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileName(originalFilePath));
			return new FileOutputWriter(path);
		}





		[TestCase(ConventionalHeavyLorry, TestName = "ConventionalHeavyLorry")]
		[TestCase(ConventionalHeavyLorry_NoRetarder, TestName = "ConventionalHeavyLorry_NoRetarder")]
		[TestCase(ConventionalHeavyLorry_NoAirdrag, TestName = "ConventionalHeavyLorry_NoAirdrag")]
		[TestCase(ConventionalHeavyLorry_DifferentTyres, TestName = "ConventionalHeavyLorry_DifferentTyres")]
		[TestCase(ConventionalHeavyLorry_AT_Angledrive, TestName = "ConventionalHeavyLorry_AT_Angledrive")]
		[TestCase(ConventionalHeavyLorry_Vocational, TestName = "ConventionalHeavyLorry_Vocational")]
		//[TestCase(ConventionalHeavyLorry, false, TestName = "ConventionalHeavyLorryNoMockup")]
		[TestCase(HEV_S2_HeavyLorry, TestName = "HEV_S2_HeavyLorry")]
		[TestCase(HEV_S2_HeavyLorry_NoRetarder, TestName = "HEV_S2_HeavyLorry_NoRetarder")]
		[TestCase(HEV_S3_HeavyLorry, TestName = "HEV_S3_HeavyLorry")]
		[TestCase(HEV_S3_HeavyLorry_ovc, TestName = "HEV_S3_HeavyLorry_ovc")]
		[TestCase(HEV_S4_HeavyLorry, TestName = "HEV_S4_HeavyLorry")]
		[TestCase(HEV_Px_HeavyLorry, TestName = "HEV_Px_HeavyLorry")]
		[TestCase(HEV_Px_HeavyLorry_BatteryStd, TestName = "HEV_Px_HeavyLorry_BatteryStd")]
		[TestCase(PEV_E2_HeavyLorry, TestName = "PEV_E2_HeavyLorry")]
		[TestCase(PEV_E2_HeavyLorry_BatteryStd, TestName = "PEV_E2_HeavyLorry_BatteryStd")]
		[TestCase(PEV_E2_HeavyLorry_NoRetarder, TestName = "PEV_E2_HeavyLorry_NoRetarder")]
		[TestCase(PEV_E2_HeavyLorry_NoAirdrag, TestName = "PEV_E2_HeavyLorry_NoAirdrag")]
		[TestCase(PEV_E2_HeavyLorry_Vocational, TestName = "PEV_E2_HeavyLorry_Vocational")]
		//[TestCase(PEV_E2_HeavyLorry, false, TestName = "PEV_E2_HeavyLorryNoMockup")]
		[TestCase(PEV_E3_HeavyLorry, TestName = "PEV_E3_HeavyLorry")]
		[TestCase(PEV_E4_HeavyLorry, TestName = "PEV_E4_HeavyLorry")]
		[TestCase(PEV_IEPC_HeavyLorry, TestName = "PEV_IEPC_HeavyLorry")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx1, TestName = "PEV_IEPC_HeavyLorry_Gbx1")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx1Axl, TestName = "PEV_IEPC_HeavyLorry_Gbx1Axl")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx1Whl, TestName = "PEV_IEPC_HeavyLorry_Gbx1Whl")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx2, TestName = "PEV_IEPC_HeavyLorry_Gbx2")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx2_drag, TestName = "PEV_IEPC_HeavyLorry_Gbx2_drag")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx2Axl, TestName = "PEV_IEPC_HeavyLorry_Gbx2Axl")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx2Axl_drag, TestName = "PEV_IEPC_HeavyLorry_Gbx2Axl_drag")]
		[TestCase(PEV_IEPC_HeavyLorry_Gbx2Whl, TestName = "PEV_IEPC_HeavyLorry_Gbx2Whl")]

		[TestCase(HEV_IEPC_S_HeavyLorry, TestName = "HEV_IEPC_S_HeavyLorry")]
        [TestCase(HEV_IHPC_HeavyLorry, TestName = "HEV_IHPC_HeavyLorry")]
		[TestCase(HEV_Px_HeavyLorry_ADC, TestName = "HEV_Px_HeavyLorry_ADC")]
		[TestCase(HEV_Px_HeavyLorry_NoRetarder, TestName = "HEV_Px_HeavyLorry_NoRetarder")]
		[TestCase(HEV_Px_HeavyLorry_NoAirDrag, TestName = "HEV_Px_HeavyLorry_NoAirDrag")]
		[TestCase(HEV_S3_HeavyLorry_ADC, TestName = "HEV_S3_HeavyLorry_ADC")]
		[TestCase(HEV_Px_HeavyLorry_SuperCap, TestName = "HEV_Px_HeavyLorry_SuperCap")]

		//[NonParallelizable]
		[TestCase(Conventional_mediumLorry_AMT, TestName = "Conventional_Medium_Lorry")]
		[TestCase(HEV_S_mediumLorry_AMT_S2, TestName = "HEV_S2_Medium_Lorry")]
		[TestCase(HEV_S_mediumLorry_IEPC_S, TestName = "HEV_IEPC_Medium_Lorry")]
		[TestCase(HEV_S_mediumLorry_S3, TestName = "HEV_S3_Medium_Lorry")]
		[TestCase(HEV_S_mediumLorry_S3_BatteryStd, TestName = "HEV_S3_Medium_Lorry_BatteryStd")]
		[TestCase(HEV_S_mediumLorry_S4, TestName = "HEV_S4_Medium_Lorry")]
		[TestCase(HEV_mediumLorry_AMT_Px, TestName = "HEV_Px_Medium_Lorry")]
		[TestCase(IEPC_mediumLorry, TestName = "PEV_IEPC_Medium_Lorry")]
		[TestCase(PEV_mediumLorry_AMT_E2, TestName = "PEV_E2_Medium_Lorry")]
		[TestCase(PEV_mediumLorry_AMT_E2_EM_Std, TestName = "PEV_E2_std_Medium_Lorry")]
		[TestCase(PEV_mediumLorry_APT_N_E2, TestName = "PEV_E2_Medium_Lorry_2")]
		[TestCase(PEV_mediumLorry_E3, TestName = "PEV_E3_Medium_Lorry")]
		[TestCase(PEV_mediumLorry_E3_BatteryStd, TestName = "PEV_E3_Medium_Lorry_BatteryStd")]
		[TestCase(PEV_mediumLorry_E4, TestName = "PEV_E4_Medium_Lorry")]
		[TestCase(HEV_mediumLorry_IHPC, TestName = "HEV_IHPC_MediumLorry")]
		[TestCase(HEV_mediumLorry_Px_SuperCap, TestName = "HEV_Px_Medium_Lorry_SuperCap")]
		[TestCase(PEV_mediumLorry_AMT_E2_BatStd, TestName = "PEV_E2_Medium_Lorry_BatteryStd")]

		[TestCase(v27LorryPath + "Conventional_HeavyLorry.xml", TestName = "v27_Conventional_HeavyLorry")]
        [TestCase(v27LorryPath + "Conventional_HeavyLorry_requiredOnly.xml", TestName = "v27_Conventional_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Conventional_MediumLorry.xml", TestName = "v27_Conventional_MediumLorry")]
        [TestCase(v27LorryPath + "Conventional_MediumLorry_requiredOnly.xml", TestName = "v27_Conventional_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_F2_HeavyLorry.xml", TestName = "v27_FCHV_F2_HeavyLorry")]
        [TestCase(v27LorryPath + "FCHV_F2_HeavyLorry_requiredOnly.xml", TestName = "v27_FCHV_F2_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_F2_MediumLorry.xml", TestName = "v27_FCHV_F2_MediumLorry")]
        [TestCase(v27LorryPath + "FCHV_F2_MediumLorry_requiredOnly.xml", TestName = "v27_FCHV_F2_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_F3_HeavyLorry.xml", TestName = "v27_FCHV_F3_HeavyLorry")]
        [TestCase(v27LorryPath + "FCHV_F3_HeavyLorry_requiredOnly.xml", TestName = "v27_FCHV_F3_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_F3_MediumLorry.xml", TestName = "v27_FCHV_F3_MediumLorry")]
        [TestCase(v27LorryPath + "FCHV_F3_MediumLorry_requiredOnly.xml", TestName = "v27_FCHV_F3_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_F4_HeavyLorry.xml", TestName = "v27_FCHV_F4_HeavyLorry")]
        [TestCase(v27LorryPath + "FCHV_F4_HeavyLorry_requiredOnly.xml", TestName = "v27_FCHV_F4_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_F4_MediumLorry.xml", TestName = "v27_FCHV_F4_MediumLorry")]
        [TestCase(v27LorryPath + "FCHV_F4_MediumLorry_requiredOnly.xml", TestName = "v27_FCHV_F4_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_IEPC_2xFC_HeavyLorry.xml", TestName = "v27_FCHV_IEPC_2xFC_HeavyLorry")]
        [TestCase(v27LorryPath + "FCHV_IEPC_HeavyLorry.xml", TestName = "v27_FCHV_IEPC_HeavyLorry")]
        [TestCase(v27LorryPath + "FCHV_IEPC_HeavyLorry_requiredOnly.xml", TestName = "v27_FCHV_IEPC_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "FCHV_IEPC_MediumLorry.xml", TestName = "v27_FCHV_IEPC_MediumLorry")]
        [TestCase(v27LorryPath + "FCHV_IEPC_MediumLorry_requiredOnly.xml", TestName = "v27_FCHV_IEPC_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "H2_ICE_HeavyLorry.xml", TestName = "v27_H2_ICE_HeavyLorry")]
        [TestCase(v27LorryPath + "H2_ICE_MediumLorry.xml", TestName = "v27_H2_ICE_MediumLorry")]
        [TestCase(v27LorryPath + "HEV_IHPC_HeavyLorry.xml", TestName = "v27_HEV_IHPC_HeavyLorry")]
        [TestCase(v27LorryPath + "HEV_IHPC_MediumLorry.xml", TestName = "v27_HEV_IHPC_MediumLorry")]
        [TestCase(v27LorryPath + "HEV_P2_HeavyLorry.xml", TestName = "v27_HEV_P2_HeavyLorry")]
        [TestCase(v27LorryPath + "HEV_P2_HeavyLorry_requiredOnly.xml", TestName = "v27_HEV_P2_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "HEV_P2_MediumLorry.xml", TestName = "v27_HEV_P2_MediumLorry")]
        [TestCase(v27LorryPath + "HEV_P2_MediumLorry_requiredOnly.xml", TestName = "v27_HEV_P2_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "HEV_P2_supercap_HeavyLorry.xml", TestName = "v27_HEV_P2_supercap_HeavyLorry")]
        [TestCase(v27LorryPath + "PEV_E2_HeavyLorry.xml", TestName = "v27_PEV_E2_HeavyLorry")]
        [TestCase(v27LorryPath + "PEV_E2_HeavyLorry_requiredOnly.xml", TestName = "v27_PEV_E2_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_E2_MediumLorry.xml", TestName = "v27_PEV_E2_MediumLorry")]
        [TestCase(v27LorryPath + "PEV_E2_MediumLorry_requiredOnly.xml", TestName = "v27_PEV_E2_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_E3_HeavyLorry.xml", TestName = "v27_PEV_E3_HeavyLorry")]
        [TestCase(v27LorryPath + "PEV_E3_HeavyLorry_requiredOnly.xml", TestName = "v27_PEV_E3_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_E3_MediumLorry.xml", TestName = "v27_PEV_E3_MediumLorry")]
        [TestCase(v27LorryPath + "PEV_E3_MediumLorry_requiredOnly.xml", TestName = "v27_PEV_E3_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_E4_HeavyLorry.xml", TestName = "v27_PEV_E4_HeavyLorry")]
        [TestCase(v27LorryPath + "PEV_E4_HeavyLorry_requiredOnly.xml", TestName = "v27_PEV_E4_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_E4_MediumLorry.xml", TestName = "v27_PEV_E4_MediumLorry")]
        [TestCase(v27LorryPath + "PEV_E4_MediumLorry_requiredOnly.xml", TestName = "v27_PEV_E4_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_IEPC_HeavyLorry.xml", TestName = "v27_PEV_IEPC_HeavyLorry")]
        [TestCase(v27LorryPath + "PEV_IEPC_HeavyLorry_requiredOnly.xml", TestName = "v27_PEV_IEPC_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_IEPC_MediumLorry.xml", TestName = "v27_PEV_IEPC_MediumLorry")]
        [TestCase(v27LorryPath + "PEV_IEPC_MediumLorry_requiredOnly.xml", TestName = "v27_PEV_IEPC_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "PEV_IEPC_multiCurve_HeavyLorry.xml", TestName = "v27_PEV_IEPC_multiCurve_HeavyLorry")]
        [TestCase(v27LorryPath + "PEV_IEPC_stdValues_HeavyLorry.xml", TestName = "v27_PEV_IEPC_stdValues_HeavyLorry")]
        [TestCase(v27LorryPath + "SHEV_IEPC_HeavyLorry.xml", TestName = "v27_SHEV_IEPC_HeavyLorry")]
        [TestCase(v27LorryPath + "SHEV_IEPC_HeavyLorry_requiredOnly.xml", TestName = "v27_SHEV_IEPC_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_IEPC_MediumLorry.xml", TestName = "v27_SHEV_IEPC_MediumLorry")]
        [TestCase(v27LorryPath + "SHEV_IEPC_MediumLorry_requiredOnly.xml", TestName = "v27_SHEV_IEPC_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_S2_HeavyLorry.xml", TestName = "v27_SHEV_S2_HeavyLorry")]
        [TestCase(v27LorryPath + "SHEV_S2_HeavyLorry_requiredOnly.xml", TestName = "v27_SHEV_S2_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_S2_MediumLorry.xml", TestName = "v27_SHEV_S2_MediumLorry")]
        [TestCase(v27LorryPath + "SHEV_S2_MediumLorry_requiredOnly.xml", TestName = "v27_SHEV_S2_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_S3_HeavyLorry.xml", TestName = "v27_SHEV_S3_HeavyLorry")]
        [TestCase(v27LorryPath + "SHEV_S3_HeavyLorry_requiredOnly.xml", TestName = "v27_SHEV_S3_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_S3_MediumLorry.xml", TestName = "v27_SHEV_S3_MediumLorry")]
        [TestCase(v27LorryPath + "SHEV_S3_MediumLorry_requiredOnly.xml", TestName = "v27_SHEV_S3_MediumLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_S4_HeavyLorry.xml", TestName = "v27_SHEV_S4_HeavyLorry")]
        [TestCase(v27LorryPath + "SHEV_S4_HeavyLorry_requiredOnly.xml", TestName = "v27_SHEV_S4_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "SHEV_S4_MediumLorry.xml", TestName = "v27_SHEV_S4_MediumLorry")]
        [TestCase(v27LorryPath + "SHEV_S4_MediumLorry_requiredOnly.xml", TestName = "v27_SHEV_S4_MediumLorry_requiredOnly")]
		[TestCase(v27LorryPath + "Multiple_FCHV_F2_IEPC_HeavyLorry.xml", TestName = "v27_Multiple_FCHV_F2_IEPC_HeavyLorry")]
        [TestCase(v27LorryPath + "Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly.xml", TestName = "v27_Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Multiple_FCHV_F3_F4_HeavyLorry.xml", TestName = "v27_Multiple_FCHV_F3_F4_HeavyLorry")]
        [TestCase(v27LorryPath + "Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly.xml", TestName = "v27_Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Multiple_PEV_E2_IEPC_HeavyLorry.xml", TestName = "v27_Multiple_PEV_E2_IEPC_HeavyLorry")]
        [TestCase(v27LorryPath + "Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly.xml", TestName = "v27_Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Multiple_PEV_E3_E4_HeavyLorry.xml", TestName = "v27_Multiple_PEV_E3_E4_HeavyLorry")]
        [TestCase(v27LorryPath + "Multiple_PEV_E3_E4_HeavyLorry_requiredOnly.xml", TestName = "v27_Multiple_PEV_E3_E4_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Multiple_SHEV_S2_IEPC_HeavyLorry.xml", TestName = "v27_Multiple_SHEV_S2_IEPC_HeavyLorry")]
        [TestCase(v27LorryPath + "Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly.xml", TestName = "v27_Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Multiple_SHEV_S3_S4_HeavyLorry.xml", TestName = "v27_Multiple_SHEV_S3_S4_HeavyLorry")]
        [TestCase(v27LorryPath + "Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly.xml", TestName = "v27_Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly")]
        public void LorryMockupTest(string fileName, bool mockup = true)
		{
			CopyInputFile(fileName);
			var inputProvider = _inputDataReader.CreateDeclaration(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumWriter);
			
			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLMonitoringReportName), XmlDocumentType.MonitoringReport), "Monitoring Report invalid");

            if (inputProvider.JobInputData.Vehicle.VocationalVehicle) {
				Assert.IsFalse(CheckElementExists(XMLNames.Report_Results_Summary, fileWriter.XMLCustomerReportName));
			} else {
				Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Summary, fileWriter.XMLCustomerReportName));
			}

		}

		public bool CheckElementExists(string name, string fileName)
		{
			var xDoc = XDocument.Load(fileName);
			return xDoc.XPathSelectElements($"//*[local-name()='{name}']").FirstOrDefault() != null;
		}



		[TestCase(Conventional_PrimaryBus, TestName = "ConventionalPrimaryBus")]
		[TestCase(Conventional_PrimaryBus_NoRetarder, TestName = "ConventionalPrimaryBus_NoRetarder")]
		[TestCase(Conventional_PrimaryBus_RetarderMeasured, TestName = "ConventionalPrimaryBus_RetarderMeasured")]
		[TestCase(Conventional_PrimaryBus_AT_Angledrive, TestName = "ConventionalPrimaryBus_AT_Angledrive")]
		[TestCase(Conventional_PrimaryBus_Tyres, TestName = "ConventionalPrimaryBus Tyres")]
        [TestCase(HEV_IEPC_S_PrimaryBus, TestName="HEV_IEPC_S_PrimaryBus")]
		[TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, TestName = "HEV_IEPC_S_PrimaryBus_BatteryStd")]
        [TestCase(HEV_Px_PrimaryBus, TestName="HEV_Px_PrimaryBus")]
		[TestCase(HEV_Px_PrimaryBus_BatteryStd, TestName = "HEV_Px_PrimaryBus_BatteryStd")]
		[TestCase(HEV_IHPC_PrimaryBus, TestName = "HEV_IHPC_PrimaryBus")]
		[TestCase(HEV_IHPC_PrimaryBus_NoRetarder, TestName = "HEV_IHPC_PrimaryBus_NoRetarder")]
		[TestCase(HEV_Px_PrimaryBus_SuperCap, TestName = "HEV_Px_PrimaryBus_SuperCap")]
        [TestCase(HEV_S2_PrimaryBus, TestName="HEV_S2_PrimaryBus")]
		[TestCase(HEV_S2_PrimaryBus_GenSetADC, TestName = "HEV_S2_PrimaryBus_GenSetADC")]
		[TestCase(HEV_S2_PrimaryBus_ADC, TestName = "HEV_S2_PrimaryBus_ADC")]
		[TestCase(HEV_S3_PrimaryBus, TestName = "HEV_S3_PrimaryBus")]
		[TestCase(HEV_S4_PrimaryBus, TestName = "HEV_S4_PrimaryBus")]
        [TestCase(PEV_E2_PrimaryBus, TestName="PEV_E2_PrimaryBus")]
		[TestCase(PEV_E3_PrimaryBus, TestName = "PEV_E3_PrimaryBus")]
		[TestCase(PEV_E4_PrimaryBus, TestName = "PEV_E4_PrimaryBus")]
        [TestCase(PEV_IEPC_PrimaryBus, TestName="PEV_IEPC_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1, TestName = "PEV_IEPC_PrimaryBus_Gbx1")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, TestName = "PEV_IEPC_PrimaryBus_Gbx1Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, TestName = "PEV_IEPC_PrimaryBus_Gbx1Whl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2, TestName = "PEV_IEPC_PrimaryBus_Gbx2")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, TestName = "PEV_IEPC_PrimaryBus_Gbx2_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, TestName = "PEV_IEPC_PrimaryBus_Gbx2Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, TestName = "PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, TestName = "PEV_IEPC_PrimaryBus_Gbx2Whl")]
		[TestCase(PEV_IEPC_std_PrimaryBus, TestName = "PEV_IEPC-std_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus_StdEM, TestName = "PEV_E2_PrimaryBus_EM-Std")]
		[TestCase(PEV_E2_PrimaryBus_StdBat, TestName = "PEV_E2_PrimaryBus_BatteryStd")]
		[TestCase(Conventional_PrimaryBus_DF, TestName = "ConventionalPrimaryBus_DualFuel")]

		[TestCase(v27PrimaryBusPath + "Conventional_PrimaryBus.xml", TestName = "v27_Conventional_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Conventional_PrimaryBus_requiredOnly.xml", TestName = "v27_Conventional_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Exempted_PrimaryBus.xml", TestName = "v27_Exempted_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Exempted_PrimaryBus_requiredOnly.xml", TestName = "v27_Exempted_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "FCHV_F2_PrimaryBus.xml", TestName = "v27_FCHV_F2_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "FCHV_F2_PrimaryBus_requiredOnly.xml", TestName = "v27_FCHV_F2_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "FCHV_F3_PrimaryBus.xml", TestName = "v27_FCHV_F3_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "FCHV_F3_PrimaryBus_requiredOnly.xml", TestName = "v27_FCHV_F3_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "FCHV_F4_PrimaryBus.xml", TestName = "v27_FCHV_F4_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "FCHV_F4_PrimaryBus_requiredOnly.xml", TestName = "v27_FCHV_F4_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "FCHV_IEPC_PrimaryBus.xml", TestName = "v27_FCHV_IEPC_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "FCHV_IEPC_PrimaryBus_requiredOnly.xml", TestName = "v27_FCHV_IEPC_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "H2_ICE_PrimaryBus.xml", TestName = "v27_H2_ICE_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "HEV_IHPC_PrimaryBus.xml", TestName = "v27_HEV_IHPC_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "HEV_P2_PrimaryBus.xml", TestName = "v27_HEV_P2_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "HEV_P2_PrimaryBus_requiredOnly.xml", TestName = "v27_HEV_P2_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F2_IEPC_PrimaryBus.xml", TestName = "v27_Multiple_FCHV_F2_IEPC_PrimaryBus")]
		[TestCase(v27PrimaryBusPath + "Multiple_FCHV_F2_IEPC_PrimaryBus_requiredOnly.xml", TestName = "v27_Multiple_FCHV_F2_IEPC_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F3_F4_PrimaryBus.xml", TestName = "v27_Multiple_FCHV_F3_F4_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F3_F4_PrimaryBus_requiredOnly.xml", TestName = "v27_Multiple_FCHV_F3_F4_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E2_IEPC_PrimaryBus.xml", TestName = "v27_Multiple_PEV_E2_IEPC_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E2_IEPC_PrimaryBus_requiredOnly.xml", TestName = "v27_Multiple_PEV_E2_IEPC_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E3_E4_PrimaryBus.xml", TestName = "v27_Multiple_PEV_E3_E4_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E3_E4_PrimaryBus_requiredOnly.xml", TestName = "v27_Multiple_PEV_E3_E4_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S2_IEPC_PrimaryBus.xml", TestName = "v27_Multiple_SHEV_S2_IEPC_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S2_IEPC_PrimaryBus_requiredOnly.xml", TestName = "v27_Multiple_SHEV_S2_IEPC_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S3_S4_PrimaryBus.xml", TestName = "v27_Multiple_SHEV_S3_S4_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S3_S4_PrimaryBus_requiredOnly.xml", TestName = "v27_Multiple_SHEV_S3_S4_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "PEV_E2_PrimaryBus.xml", TestName = "v27_PEV_E2_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "PEV_E2_PrimaryBus_requiredOnly.xml", TestName = "v27_PEV_E2_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "PEV_E3_PrimaryBus.xml", TestName = "v27_PEV_E3_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "PEV_E3_PrimaryBus_requiredOnly.xml", TestName = "v27_PEV_E3_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "PEV_E4_PrimaryBus.xml", TestName = "v27_PEV_E4_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "PEV_E4_PrimaryBus_requiredOnly.xml", TestName = "v27_PEV_E4_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "PEV_IEPC_PrimaryBus.xml", TestName = "v27_PEV_IEPC_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "PEV_IEPC_PrimaryBus_requiredOnly.xml", TestName = "v27_PEV_IEPC_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "SHEV_IEPC_PrimaryBus.xml", TestName = "v27_SHEV_IEPC_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "SHEV_IEPC_PrimaryBus_requiredOnly.xml", TestName = "v27_SHEV_IEPC_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "SHEV_S2_PrimaryBus.xml", TestName = "v27_SHEV_S2_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "SHEV_S2_PrimaryBus_requiredOnly.xml", TestName = "v27_SHEV_S2_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "SHEV_S3_PrimaryBus.xml", TestName = "v27_SHEV_S3_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "SHEV_S3_PrimaryBus_requiredOnly.xml", TestName = "v27_SHEV_S3_PrimaryBus_requiredOnly")]
        [TestCase(v27PrimaryBusPath + "SHEV_S4_PrimaryBus.xml", TestName = "v27_SHEV_S4_PrimaryBus")]
        [TestCase(v27PrimaryBusPath + "SHEV_S4_PrimaryBus_requiredOnly.xml", TestName = "v27_SHEV_S4_PrimaryBus_requiredOnly")]
        public void PrimaryBusMockupTest(string fileName, bool mockup = true)
		{
			CopyInputFile(fileName);
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumWriter);

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			Clearfiles(fileWriter); //remove files from previous test runs
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			CheckFileExists(fileWriter, CifShouldExist:false, PrimaryReportShouldExist:true);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLPrimaryVehicleReportName), XmlDocumentType.MultistepOutputData), "VIF invalid" );
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
            Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLMonitoringReportName), XmlDocumentType.MonitoringReport), "Monitoring Report invalid");
        }

		

		[TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, "Conventional", TestName = "Interim_Conventional_Bus_DifferentTyres")]
		[TestCase(Conventional_PrimaryBus_NoRetarder, Conventional_InterimBusInput, "Conventional", TestName = "Interim_ConventionalPrimaryBus_NoRetarder")]
		[TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_InterimBusInput, "Conventional", TestName = "Interim_Conventional_Bus_AT_Angledrive")]
		[TestCase(Conventional_PrimaryBus, Conventional_InterimBusInput_AirdragV10, "Conventional", TestName = "InterimConventionalBusAirdrag_v1_0")]
		[TestCase(Conventional_PrimaryBus, Conventional_InterimBusInput_AirdragV20, "Conventional", TestName = "InterimConventionalBusAirdrag_v2_0")]
		[TestCase(Conventional_PrimaryBus_DF, Conventional_InterimBusInput, "Conventional", TestName = "Interim_ConventionalPrimaryBus_DualFuel")]
		[TestCase(HEV_IEPC_S_PrimaryBus, HEV_InterimBusInput, "IEPC-S", "HEV", TestName = "Interim HEV_IEPC_S_Bus")]
		[TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_InterimBusInput, "IEPC-S", "HEV", TestName = "Interim HEV_IEPC_S_Bus_BatteryStd")]
		[TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "Interim HEV_Px_Bus")]
		[TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_InterimBusInput, "Px", "HEV", TestName = "Interim HEV_Px_Bus_BatteryStd")]
		[TestCase(HEV_IHPC_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "Interim HEV_IHPC_Bus")]
		[TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_InterimBusInput, "Px", "HEV", TestName = "Interim HEV_IHPC_Bus_NoRetarder")]
		[TestCase(HEV_S2_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "Interim HEV_S2_Bus")]
		[TestCase(HEV_S3_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "Interim HEV_S3_Bus")]
		[TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "Interim HEV_S4_Bus")]
		[TestCase(PEV_E2_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "Interim PEV_E2_Bus")]
		[TestCase(PEV_E3_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "Interim PEV_E3_Bus")]
		[TestCase(PEV_E4_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "Interim PEV_E4_Bus")]
		[TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_Bus")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx1")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx1Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx1Whl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx2")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx2_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx2Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC_PrimaryBus_Gbx2Whl")]
		[TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "Interim PEV_IEPC-std_Bus")]
		[TestCase(PEV_E2_PrimaryBus_StdEM, PEV_InterimBusInput, "Ex", "PEV", TestName = "Interim PEV_E2_Bus_EM-Std")]

        [TestCase(v27PrimaryBusPath + "Conventional_PrimaryBus.xml", v27CompleteBusPath + "Conventional_CompletedBus_requiredOnly.xml", "Conventional", TestName = "v27_Conventional_BusInterim")]
        [TestCase(v27PrimaryBusPath + "FCHV_F2_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus_requiredOnly.xml", "Fx", "FCHV", TestName = "v27_FCHV_F2_BusInterim")]
        [TestCase(v27PrimaryBusPath + "FCHV_F3_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus_requiredOnly.xml", "Fx", "FCHV", TestName = "v27_FCHV_F3_BusInterim")]
        [TestCase(v27PrimaryBusPath + "FCHV_F4_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus_requiredOnly.xml", "Fx", "FCHV", TestName = "v27_FCHV_F4_BusInterim")]
        [TestCase(v27PrimaryBusPath + "FCHV_IEPC_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus_requiredOnly.xml", "Fx", "FCHV", TestName = "v27_FCHV_IEPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "H2_ICE_PrimaryBus.xml", v27CompleteBusPath + "Conventional_CompletedBus_requiredOnly.xml", "Conventional", TestName = "v27_H2_ICE_BusInterim")]
        [TestCase(v27PrimaryBusPath + "HEV_IHPC_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "Px", "HEV", TestName = "v27_HEV_IHPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "HEV_P2_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "Px", "HEV", TestName = "v27_HEV_P2_BusInterim")]
        [TestCase(v27PrimaryBusPath + "PEV_E2_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus_requiredOnly.xml", "Ex", "PEV", TestName = "v27_PEV_E2_BusInterim")]
        [TestCase(v27PrimaryBusPath + "PEV_E3_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus_requiredOnly.xml", "Ex", "PEV", TestName = "v27_PEV_E3_BusInterim")]
        [TestCase(v27PrimaryBusPath + "PEV_E4_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus_requiredOnly.xml", "Ex", "PEV", TestName = "v27_PEV_E4_BusInterim")]
        [TestCase(v27PrimaryBusPath + "PEV_IEPC_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus_requiredOnly.xml", "IEPC", "PEV", TestName = "v27_PEV_IEPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "SHEV_S2_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "Sx", "HEV", TestName = "v27_SHEV_S2_BusInterim")]
        [TestCase(v27PrimaryBusPath + "SHEV_S3_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "Sx", "HEV", TestName = "v27_SHEV_S3_BusInterim")]
        [TestCase(v27PrimaryBusPath + "SHEV_S4_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "Sx", "HEV", TestName = "v27_SHEV_S4_BusInterim")]
        [TestCase(v27PrimaryBusPath + "SHEV_IEPC_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "IEPC-S", "HEV", TestName = "v27_SHEV_IEPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F2_IEPC_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus_requiredOnly.xml", "FCHV", TestName = "v27_Multiple_FCHV_F2_IEPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F3_F4_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus_requiredOnly.xml", "FCHV", TestName = "v27_Multiple_FCHV_F3_F4_BusInterim")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E2_IEPC_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus_requiredOnly.xml", "PEV", TestName = "v27_Multiple_PEV_E2_IEPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E3_E4_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus_requiredOnly.xml", "PEV", TestName = "v27_Multiple_PEV_E3_E4_BusInterim")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S2_IEPC_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "HEV", TestName = "v27_Multiple_SHEV_S2_IEPC_BusInterim")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S3_S4_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus_requiredOnly.xml", "HEV", TestName = "v27_Multiple_SHEV_S3_S4_BusInterim")]
        public void InterimTest(string primaryBusInput, string interimBusInput, params string[] expectedType)
		{
			var interimCopy = CopyInputFile(interimBusInput)[0];
			// VIF + interim input =>  VIF
			// (approach: first simulate primary on its own to have an up-to-date VIF
			// (no need to maintain this in the testfiles)

			// setting up testcase 
			// run primary simulation
			var inputProvider = _inputDataReader.Create(primaryBusInput);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
			var sumWriter = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumWriter);

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			Clearfiles(fileWriter); //remove files from previous test runs
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();

			CheckFileExists(fileWriter, CifShouldExist: false, PrimaryReportShouldExist: true, MrfShouldExist: true);
			File.Delete(fileWriter.XMLFullReportName);
			var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName)[0];
			// done preparing testcase...

			// this is the actual test: run completed simulation

			var interimJob = GenerateJsonJobCompletedBus(primaryVif, interimCopy, TestContext.CurrentContext.Test.Name);
			var interimInputData = JSONInputDataFactory.ReadJsonJob(interimJob);
			var interimFileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, interimBusInput);
			
			var interimSumWriter = new SummaryDataContainer(null);
			var interimJobContainer = new JobContainer(interimSumWriter);

			var completedSimulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, interimInputData, interimFileWriter, null, null, true);

			Clearfiles(interimFileWriter); //remove files from previous test runs
			interimJobContainer.AddRuns(completedSimulatorFactory);
			interimJobContainer.Execute(false);
			interimJobContainer.WaitFinished();

			// assertions
			File.Delete(fileWriter.XMLPrimaryVehicleReportName);

			CheckFileExists(interimFileWriter, VifShouldExist: true, MrfShouldExist: false, CifShouldExist: false, MonitoringReportShouldExist: false);

			CheckElementTypeNameContains(interimFileWriter.XMLMultistageReportFileName, "Vehicle", expectedType);
		}

		[TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim Conventional Bus Different Tyres")]
		[TestCase(Conventional_PrimaryBus_NoRetarder, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim_ConventionalPrimaryBus_NoRetarder")]
		[TestCase(Conventional_PrimaryBus_DF, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim ConventionalPrimaryBus_DualFuel")]
		[TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim ConventionalPrimaryBus_AT_Angledrive")]
		[TestCase(HEV_IEPC_S_PrimaryBus, HEV_InterimBusInput, "IEPC-S", "HEV", TestName = "PrimaryAndInterim HEV_IEPC_S_PrimaryBus")]
		[TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_InterimBusInput, "IEPC-S", "HEV", TestName = "PrimaryAndInterim HEV_IEPC_S_PrimaryBus_BatteryStd")]
		[TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_Px_PrimaryBus")]
		[TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_Px_PrimaryBus_BatteryStd")]
		[TestCase(HEV_IHPC_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_IHPC_PrimaryBus")]
		[TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_IHPC_PrimaryBus_NoRetarder")]
		[TestCase(HEV_S2_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S2_PrimaryBus")]
		[TestCase(HEV_S3_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S3_PrimaryBus")]
		[TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S4_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E2_PrimaryBus")]
		[TestCase(PEV_E3_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E3_PrimaryBus")]
		[TestCase(PEV_E4_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E4_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterim PEV_IEPC_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx1")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx1Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx1Whl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2Whl")]
		[TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_InterimBusInput, "PEV", "IEPC", TestName = "PrimaryAndInterim PEV_IEPC-std_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus_StdEM, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E2_PrimaryBus_EM-Std")]
        public void PrimaryWithInterimTest(string primaryBusInput, string interimInput, params string[] expectedType)
		{
			var copied = CopyInputFile(primaryBusInput, interimInput);
			// complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
			// (approach: first simulate primary on its own to have an up-to-date VIF
			// (no need to maintain this in the testfiles)


			var job = GenerateJsonJobCompleteBus(copied[0], copied[1], TestContext.CurrentContext.Test.Name);
			var input = JSONInputDataFactory.ReadJsonJob(job);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, interimInput);
			var sumWriter = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumWriter);

			var completedSimulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, input, fileWriter, null, null, true);

			Clearfiles(fileWriter); //remove files from previous test runs
			jobContainer.AddRuns(completedSimulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();

			// assertions

			CheckFileExists(fileWriter, PrimaryMrfShouldExist: true, VifShouldExist: true, CifShouldExist: false, MrfShouldExist: false, MonitoringReportShouldExist: false);

			CheckElementTypeNameContains(fileWriter.XMLMultistageReportFileName, "Vehicle", expectedType);
		}

		

		[TestCase(Conventional_PrimaryBus_Tyres, Conventional_CompletedBusInput, "Conventional", TestName = "Complete Conventional Bus Different Tyres")]
		[TestCase(Conventional_PrimaryBus_Tyres, Conventional_CompletedBusInputNoAirdrag, "Conventional", TestName = "Complete Conventional Bus NoAirdrag")]
		[TestCase(Conventional_PrimaryBus_NoRetarder, Conventional_CompletedBusInput, "Conventional", TestName = "Complete_ConventionalPrimaryBus_NoRetarder")]
		[TestCase(Conventional_PrimaryBus_DF, Conventional_CompletedBusInput, "Conventional", TestName = "Complete ConventionalPrimaryBus_DualFuel")]
		[TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_CompletedBusInput, "Conventional", TestName = "Complete ConventionalPrimaryBus_AT_Angledrive")]
		[TestCase(Conventional_PrimaryBus, Conventional_CompletedBusInput_TypeApproval, "Conventional", TestName = "Complete Conventional Bus Type Approval")]
        [TestCase(HEV_IEPC_S_PrimaryBus, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Complete HEV_IEPC_S_PrimaryBus")]
		[TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Complete HEV_IEPC_S_PrimaryBus_BatteryStd")]
		[TestCase(HEV_Px_PrimaryBus, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus")]
		[TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus_BatteryStd")]
		[TestCase(HEV_IHPC_PrimaryBus, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_IHPC_PrimaryBus")]
		[TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_IHPC_PrimaryBus_NoRetarder")]
		[TestCase(HEV_S2_PrimaryBus, HEV_CompletedBusInput, "S2", "HEV", TestName = "Complete HEV_S2_PrimaryBus")]
		[TestCase(HEV_S3_PrimaryBus, HEV_CompletedBusInput, "S3", "HEV", TestName = "Complete HEV_S3_PrimaryBus")]
		[TestCase(HEV_S4_PrimaryBus, HEV_CompletedBusInput, "S4", "HEV", TestName = "Complete HEV_S4_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus, PEV_CompletedBusInput, "E2", "PEV", TestName = "Complete PEV_E2_PrimaryBus")]
		[TestCase(PEV_E3_PrimaryBus, PEV_CompletedBusInput, "E3", "PEV", TestName = "Complete PEV_E3_PrimaryBus")]
		[TestCase(PEV_E4_PrimaryBus, PEV_CompletedBusInput, "E4", "PEV", TestName = "Complete PEV_E4_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx1")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx1Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx1Whl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2Whl")]
		[TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Complete PEV_IEPC-std_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus_StdEM, PEV_CompletedBusInput, "E2", "PEV", TestName = "Complete PEV_E2_PrimaryBus_EM-Std")]

        [TestCase(v27PrimaryBusPath + "Conventional_PrimaryBus.xml", v27CompleteBusPath + "Conventional_CompletedBus.xml", "Conventional", TestName = "v27_Conventional_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "FCHV_F2_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus.xml", "Fx", "FCHV", TestName = "v27_FCHV_F2_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "FCHV_F3_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus.xml", "Fx", "FCHV", TestName = "v27_FCHV_F3_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "FCHV_F4_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus.xml", "Fx", "FCHV", TestName = "v27_FCHV_F4_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "FCHV_IEPC_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus.xml", "Fx", "FCHV", TestName = "v27_FCHV_IEPC_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "H2_ICE_PrimaryBus.xml", v27CompleteBusPath + "Conventional_CompletedBus.xml", "Conventional", TestName = "v27_H2_ICE_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "HEV_IHPC_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Px", "HEV", TestName = "v27_HEV_IHPC_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "HEV_P2_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Px", "HEV", TestName = "v27_HEV_P2_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "PEV_E2_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus.xml", "Ex", "PEV", TestName = "v27_PEV_E2_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "PEV_E3_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus.xml", "Ex", "PEV", TestName = "v27_PEV_E3_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "PEV_E4_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus.xml", "Ex", "PEV", TestName = "v27_PEV_E4_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "PEV_IEPC_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus.xml", "IEPC", "PEV", TestName = "v27_PEV_IEPC_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "SHEV_S2_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Sx", "HEV", TestName = "v27_SHEV_S2_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "SHEV_S3_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Sx", "HEV", TestName = "v27_SHEV_S3_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "SHEV_S4_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Sx", "HEV", TestName = "v27_SHEV_S4_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "SHEV_IEPC_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "IEPC", "HEV", TestName = "v27_SHEV_IEPC_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F2_IEPC_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus.xml", "Fx", "FCHV", TestName = "v27_Multiple_FCHV_F2_IEPC_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "Multiple_FCHV_F3_F4_PrimaryBus.xml", v27CompleteBusPath + "FCHV_CompletedBus.xml", "Fx", "FCHV", TestName = "v27_Multiple_FCHV_F3_F4_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E2_IEPC_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus.xml", "Ex", "PEV", TestName = "v27_Multiple_PEV_E2_IEPC_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "Multiple_PEV_E3_E4_PrimaryBus.xml", v27CompleteBusPath + "PEV_CompletedBus.xml", "Ex", "PEV", TestName = "v27_Multiple_PEV_E3_E4_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S2_IEPC_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Sx", "HEV", TestName = "v27_Multiple_SHEV_S2_IEPC_PrimaryBus_BusInterimFullComplete")]
        [TestCase(v27PrimaryBusPath + "Multiple_SHEV_S3_S4_PrimaryBus.xml", v27CompleteBusPath + "HEV_CompletedBus.xml", "Sx", "HEV", TestName = "v27_Multiple_SHEV_S3_S4_PrimaryBus_BusInterimFullComplete")]

        public void CompleteTest(string primaryBusInput, string completeBusInput, params string[] expectedType)
		{
			var copied = CopyInputFile(primaryBusInput, completeBusInput);
			// complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
			// (approach: first simulate primary on its own to have an up-to-date VIF
			// (no need to maintain this in the testfiles)

			
			var completeJob = GenerateJsonJobCompleteBus(copied[0], copied[1], TestContext.CurrentContext.Test.Name);
			var completeBusinput = JSONInputDataFactory.ReadJsonJob(completeJob);
			var completeFileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, completeBusInput);
			var completeSumWriter = new SummaryDataContainer(null);
			var completeJobContainer = new JobContainer(completeSumWriter);

			var completedSimulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, completeBusinput, completeFileWriter, null, null, true);

			Clearfiles(completeFileWriter); //remove files from previous test runs
			completeJobContainer.AddRuns(completedSimulatorFactory);
			completeJobContainer.Execute(false);
			completeJobContainer.WaitFinished();

			// assertions

			CheckFileExists(completeFileWriter, PrimaryMrfShouldExist: true, VifShouldExist: true, CifShouldExist: true, MrfShouldExist: true);

			CheckElementTypeNameContains(completeFileWriter.XMLMultistageReportFileName, "Vehicle", expectedType);

			//var xmlComparer = new XMLElementComparer();
			//xmlComparer.AddDocument(primaryBusInput, XmlDocumentType.DeclarationJobData);
			//xmlComparer.AddDocument(completeFileWriter.XMLFullReportName, XmlDocumentType.ManufacturerReport);
			//Assert.IsTrue(xmlComparer.AreEqual(primaryBusInput, "/tns:VectoInputDeclaration/v2.0:Vehicle/ZeroEmissionVehicle", 
			//	completeFileWriter.XMLFullReportName, "/mrf:VectoOutput/mrf:Data/Vehicle/ZeroEmissionHDV"));


		}



		[TestCase(Conventional_PrimaryBus_Tyres, Conventional_CompletedBusInput, "Conventional", TestName = "Completed Conventional Bus Different Tyres")]
		[TestCase(Conventional_PrimaryBus_DF, Conventional_CompletedBusInput, "Conventional", TestName = "Completed ConventionalPrimaryBus_DualFuel")]
		[TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_CompletedBusInput, "Conventional", TestName = "Completed Conventional Bus_AT_Angledrive")]
		[TestCase(HEV_IEPC_S_PrimaryBus, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Completed HEV_IEPC_S_PrimaryBus")]
		[TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Completed HEV_IEPC_S_PrimaryBus_BatteryStd")]
		[TestCase(HEV_Px_PrimaryBus, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus")]
		[TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus_BatteryStd")]
		[TestCase(HEV_IHPC_PrimaryBus, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_IHPC_PrimaryBus")]
		[TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_IHPC_PrimaryBus_NoRetarder")]
		[TestCase(HEV_S2_PrimaryBus, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S2_PrimaryBus")]
		[TestCase(HEV_S3_PrimaryBus, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S3_PrimaryBus")]
		[TestCase(HEV_S4_PrimaryBus, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S4_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E2_PrimaryBus")]
		[TestCase(PEV_E3_PrimaryBus, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E3_PrimaryBus")]
		[TestCase(PEV_E4_PrimaryBus, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E4_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx1")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx1Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx1Whl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2Axl")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
		[TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2Whl")]
		[TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_CompletedBusInput, "PEV", "IEPC", TestName = "Completed PEV_IEPC-std_PrimaryBus")]
		[TestCase(PEV_E2_PrimaryBus_StdEM, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E2_PrimaryBus_EM-Std")]
        public void CompletedTest(string primaryBusInput, string completeBusInput, params string[] expectedType)
		{
			var completeCopy = CopyInputFile(completeBusInput)[0];
			CopyInputFile(primaryBusInput);
			// completed: VIF + complete input (full) =>  VIF , MRF Completed, CIF Completed
			// (approach: first simulate primary on its own to have an up-to-date VIF
			// (no need to maintain this in the testfiles)

			// setting up testcase 
			// run primary simulation
			var inputProvider = _inputDataReader.Create(primaryBusInput);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
			var sumWriter = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumWriter);

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

			Clearfiles(fileWriter); //remove files from previous test runs
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();

			CheckFileExists(fileWriter, CifShouldExist: false, PrimaryReportShouldExist: true, MrfShouldExist: true);
			//File.Delete(fileWriter.XMLFullReportName);
			var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName)[0];
			// done preparing testcase...

			// this is the actual test: run completed simulation

			var completedJob = GenerateJsonJobCompletedBus(primaryVif, completeCopy, TestContext.CurrentContext.Test.Name);
			var completedInputData = JSONInputDataFactory.ReadJsonJob(completedJob);
			var completedFileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, completeBusInput);
			var completedSumWriter = new SummaryDataContainer(null);
			var completedJobContainer = new JobContainer(completedSumWriter);

			var completedSimulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData, completedFileWriter, null, null, true);

			Clearfiles(completedFileWriter); //remove files from previous test runs
			completedJobContainer.AddRuns(completedSimulatorFactory);
			completedJobContainer.Execute(false);
			completedJobContainer.WaitFinished();

			// assertions
			//File.Delete(fileWriter.XMLPrimaryVehicleReportName);

			CheckFileExists(completedFileWriter, CifShouldExist: true, MrfShouldExist: true, VifShouldExist:true);

			CheckElementTypeNameContains(completedFileWriter.XMLMultistageReportFileName, "Vehicle", expectedType);
		}


		private static void CheckElementTypeNameContains(string fileName, string elementName, params string[] expectedType)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(XmlReader.Create(fileName));
			var validator = new XMLValidator(xmlDoc);
			validator.ValidateXML(XmlDocumentType.MultistepOutputData);

			var vehicleNodes = xmlDoc.SelectNodes($"//*[local-name()='{elementName}']");
			foreach (XmlNode vehicleNode in vehicleNodes) {
				var typeName = vehicleNode?.SchemaInfo?.SchemaType?.Name ?? "";
				var contains = expectedType.Select(x => typeName.Contains(x, StringComparison.InvariantCultureIgnoreCase));
				Assert.IsTrue(contains.Any(x => x), $"{typeName} -- {expectedType.Join()}");
			}
		}

		private string GenerateJsonJobCompletedBus(string vif, string completeBusInput, string subDirectory)
		{
			subDirectory = Path.Combine("MockupReports", subDirectory, "Input");

			var header = new Dictionary<string, object>() {
				{ "FileVersion", 7 }
			};
			var body = new Dictionary<string, object>() {
				{ "PrimaryVehicleResults", Path.GetRelativePath(subDirectory, Path.GetFullPath(vif)) },
				{ "CompletedVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(completeBusInput)) },
				{ "RunSimulation", true}
			};
			var json = new Dictionary<string, object>() {
				{"Header", header},
				{"Body", body}
			};

			Directory.CreateDirectory(Path.GetFullPath(subDirectory));
			var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), "completedJob.vecto");
			var str = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(path, str);
			return path;
		}
		private string GenerateJsonJobCompleteBus(string primaryBusInput, string completeBusInput, string subDirectory)
		{
			subDirectory = Path.Combine("MockupReports", subDirectory, "Input");

			var header = new Dictionary<string, object>() {
				{ "FileVersion", 10 }
			};
			var body = new Dictionary<string, object>() {
				{ "PrimaryVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(primaryBusInput)) },
				{ "InterimStep", Path.GetRelativePath(subDirectory, Path.GetFullPath(completeBusInput)) },
				{ "RunSimulation", true}
			};
			var json = new Dictionary<string, object>() {
				{"Header", header},
				{"Body", body}
			};

			Directory.CreateDirectory(Path.GetFullPath(subDirectory));
			var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), "completeJob.vecto");
			var str = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
			File.WriteAllText(path, str);
			return path;
		}

		private static void CheckFileExists(FileOutputWriter fileWriter, 
			bool MrfShouldExist = true,
			bool CifShouldExist = true, 
			bool VifShouldExist = false, 
			bool PrimaryMrfShouldExist = false,
			bool PrimaryReportShouldExist = false,
			bool MonitoringReportShouldExist = true)
		{
			var fail = false;
			
			if (MrfShouldExist) {
				if (File.Exists(fileWriter.XMLFullReportName)) {
					MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLFullReportName),
						XmlDocumentType.ManufacturerReport);
				} else {
					TestContext.WriteLine(fileWriter.XMLFullReportName + " Missing\n");
					fail = true;
				}
			} else {
				var fileName = fileWriter.XMLFullReportName;
				if (File.Exists(fileName))
				{
					fail = true;
					TestContext.WriteLine($"{fileName} should not exist");
				}
            }

            if (CifShouldExist)
            {
                if (File.Exists(fileWriter.XMLCustomerReportName))
                {
                    MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLCustomerReportName),
                        XmlDocumentType.CustomerReport);
                }
                else
                {
                    TestContext.WriteLine(fileWriter.XMLCustomerReportName + " Missing\n");
                    fail = true;
                }
            }
            else
            {
                var fileName = fileWriter.XMLCustomerReportName;
                if (File.Exists(fileName))
                {
                    fail = true;
                    TestContext.WriteLine($"{fileName} should not exist");
                }
            }

            if (MonitoringReportShouldExist)
            {
                if (File.Exists(fileWriter.XMLMonitoringReportName))
                {
                    MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLMonitoringReportName),
                        XmlDocumentType.MonitoringReport);
                }
                else
                {
                    TestContext.WriteLine(fileWriter.XMLMonitoringReportName + " Missing\n");
                    fail = true;
                }
            }
            else
            {
                var fileName = fileWriter.XMLMonitoringReportName;
                if (File.Exists(fileName))
                {
                    fail = true;
                    TestContext.WriteLine($"{fileName} should not exist");
                }
            }

            var primaryMrfPath = fileWriter.XMLFullReportName.Replace("RSLT_MANUFACTURER", "RSLT_MANUFACTURER_PRIMARY");
			if (PrimaryMrfShouldExist) {
				if (File.Exists(primaryMrfPath)) {
					MRF_CIF_WriterTestBase.Validate(XDocument.Load(primaryMrfPath), XmlDocumentType.ManufacturerReport);
				} else {
					TestContext.WriteLine(primaryMrfPath + " Missing\n");
					fail = true;
				}
			} else {
				var fileName = primaryMrfPath;
				if (File.Exists(fileName))
				{
					fail = true;
					TestContext.WriteLine($"{fileName} should not exist");
				}
            }


			if (PrimaryReportShouldExist) {
				if (File.Exists(fileWriter.XMLPrimaryVehicleReportName)) {
					MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLPrimaryVehicleReportName),
						XmlDocumentType.MultistepOutputData);
				} else {
					TestContext.WriteLine(fileWriter.XMLPrimaryVehicleReportName + " Missing\n");
					fail = true;
				}
			} else {
				var fileName = fileWriter.XMLPrimaryVehicleReportName;
				if (File.Exists(fileName))
				{
					fail = true;
					TestContext.WriteLine($"{fileName} should not exist");
				}
            }

            if (VifShouldExist) {
				if (File.Exists(fileWriter.XMLMultistageReportFileName)) {
					MRF_CIF_WriterTestBase.Validate(XDocument.Load(fileWriter.XMLMultistageReportFileName),
						XmlDocumentType.MultistepOutputData);
				} else {
					TestContext.WriteLine(fileWriter.XMLMultistageReportFileName + " Missing\n");
					fail = true;
				}
			} else {
				var fileName = fileWriter.XMLMultistageReportFileName;
				if (File.Exists(fileName))
				{
					fail = true;
					TestContext.WriteLine($"{fileName} should not exist");
				}
            }

            if (fail) {
				Assert.Fail();
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

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		[Ignore("1.0 Schema ignored")]
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

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		[Ignore("Schema 2_0 not supported in MockupVecto")]
		[TestCase(@"TestData/XML/XMLReaderDeclaration/SchemaVersion2.0/Tractor_4x2_vehicle-class-5_5_t_0.xml", TestName="Schema20Test1")]
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

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();
			
			CheckFileExists(fileWriter);
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		[Ignore("Json not supported in Mockup vecto")]
		[TestCase("TestData/Generic Vehicles/Declaration Mode/40t Long Haul Truck/40t_Long_Haul_Truck.vecto", TestName="JSON_40TLonghaul")]
		[TestCase("TestData/Generic Vehicles/Declaration Mode/Class9_RigidTruck_6x2/Class9_RigidTruck_DECL.vecto", TestName="JSON_RigidTruckClass9")]
        [NonParallelizable]
		public void JSONTest(string fileName, bool mockup = true)
		{

			var inputProvider = JSONInputDataFactory.ReadJsonJob(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			ISimulatorFactory _simulatorFactory = null!;
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
			true,
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
        [TestCase(v27LorryPath + "Exempted_HeavyLorry.xml", false, true, true, false, false, TestName = "v27_Exempted_HeavyLorry")]
        [TestCase(v27LorryPath + "Exempted_HeavyLorry_requiredOnly.xml", false, true, true, false, false, TestName = "v27_Exempted_HeavyLorry_requiredOnly")]
        [TestCase(v27LorryPath + "Exempted_MediumLorry.xml", false, true, true, false, false, TestName = "v27_Exempted_MediumLorry")]
        [TestCase(v27LorryPath + "Exempted_MediumLorry_requiredOnly.xml", false, true, true, false, false, TestName = "v27_Exempted_MediumLorry_requiredOnly")]
        public void ExemptedTest(string fileName, bool checkVif, bool checkCif, bool checkMrf, bool checkPrimaryMrf,
			bool checkPrimaryReport)
		{
			CopyInputFile(fileName);
            var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var _simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			Clearfiles(fileWriter);
			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(false);
			jobContainer.WaitFinished();

			CheckFileExists(fileWriter, 
				VifShouldExist:checkVif, 
				CifShouldExist:checkCif, 
				MrfShouldExist:checkMrf, 
				PrimaryMrfShouldExist:checkPrimaryMrf, 
				PrimaryReportShouldExist:checkPrimaryReport);
			if (checkMrf) Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLFullReportName), XmlDocumentType.ManufacturerReport), "MRF invalid");
			if (checkPrimaryReport) Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLPrimaryVehicleReportName), XmlDocumentType.MultistepOutputData), "VIF invalid");
			if (checkCif) Assert.IsTrue(MRF_CIF_WriterTestBase.ValidateAndPrint(XDocument.Load(fileWriter.XMLCustomerReportName), XmlDocumentType.CustomerReport), "CIF invalid");
		}

		[TestCase(BasePathExempted + "exempted_primaryBus.xml", 
			BasePathExempted + "exempted_completedBus_input_full.xml", 
			"Exempted",
			TestName="ExemptedCompleteBus")]
		public void ExemptedCompleteBusTest(string primaryInput, string completeInput, params string[] expectedType)
		{
			var copied = CopyInputFile(primaryInput, completeInput);
			var completeCopy = copied[1];


			
			// complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
			// (approach: first simulate primary on its own to have an up-to-date VIF
			// (no need to maintain this in the testfiles)


			var completeJob = GenerateJsonJobCompleteBus(primaryInput, completeCopy, TestContext.CurrentContext.Test.Name);
			var completeBusinput = JSONInputDataFactory.ReadJsonJob(completeJob);
			var completeFileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, completeInput);
			var completeSumWriter = new SummaryDataContainer(null);
			var completeJobContainer = new JobContainer(completeSumWriter);

			var completedSimulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, completeBusinput, completeFileWriter, null, null, true);

			Clearfiles(completeFileWriter); //remove files from previous test runs
			completeJobContainer.AddRuns(completedSimulatorFactory);
			completeJobContainer.Execute(false);
			completeJobContainer.WaitFinished();

			// assertions

			CheckFileExists(completeFileWriter, PrimaryMrfShouldExist: true, VifShouldExist: true, CifShouldExist: true, MrfShouldExist: true);

			//CheckElementTypeNameContains(completeFileWriter.XMLMultistageReportFileName, "Vehicle", expectedType);

		}


		[TestCase(BasePathExempted + "exempted_primaryBus.xml",
			BasePathExempted + "exempted_completedBus_input_full.xml",
			"Exempted",
			TestName = "ExemptedCompletedBus")]
        public void ExemptedCompletedTest(string primaryBusInput, string completeBusInput, params string[] expectedType)
        {
            var copied = CopyInputFile(primaryBusInput, completeBusInput);
			var completeCopy = copied[1];
			// completed: VIF + complete input (full) =>  VIF , MRF Completed, CIF Completed
            // (approach: first simulate primary on its own to have an up-to-date VIF
            // (no need to maintain this in the testfiles)

            // setting up testcase 
            // run primary simulation
            var inputProvider = _inputDataReader.Create(primaryBusInput);
            var fileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
            var sumWriter = new SummaryDataContainer(null);
            var jobContainer = new JobContainer(sumWriter);

            var _simulatorFactory =
                _simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);

            Clearfiles(fileWriter); //remove files from previous test runs
            jobContainer.AddRuns(_simulatorFactory);
            jobContainer.Execute(false);
            jobContainer.WaitFinished();

            CheckFileExists(fileWriter, CifShouldExist: false, PrimaryReportShouldExist: true, MrfShouldExist: true);
            //File.Delete(fileWriter.XMLFullReportName);
            var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName)[0];
            // done preparing testcase...

            // this is the actual test: run completed simulation

            var completedJob = GenerateJsonJobCompletedBus(primaryVif, completeCopy, TestContext.CurrentContext.Test.Name);
            var completedInputData = JSONInputDataFactory.ReadJsonJob(completedJob);
            var completedFileWriter = GetOutputFileWriter(TestContext.CurrentContext.Test.Name, completeBusInput);
            var completedSumWriter = new SummaryDataContainer(null);
            var completedJobContainer = new JobContainer(completedSumWriter);

            var completedSimulatorFactory =
                _simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData, completedFileWriter, null, null, true);

            Clearfiles(completedFileWriter); //remove files from previous test runs
            completedJobContainer.AddRuns(completedSimulatorFactory);
            completedJobContainer.Execute(false);
            completedJobContainer.WaitFinished();

            // assertions
            //File.Delete(fileWriter.XMLPrimaryVehicleReportName);

            CheckFileExists(completedFileWriter, CifShouldExist: true, MrfShouldExist: true, VifShouldExist: true);

            CheckElementTypeNameContains(completedFileWriter.XMLMultistageReportFileName, "Vehicle", expectedType);
        }



    }
}
