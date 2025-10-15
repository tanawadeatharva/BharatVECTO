using System.Xml.Linq;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.MockInput;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.ViewModelTests.FactoryTests
{
    [TestFixture]
    internal class DocumentViewModelFactoryTest
    {
		private MockDialogHelper _mockDialogHelper;
		private IKernel _kernel;
		private IDocumentViewModelFactory _documentViewModelFactory;

		[SetUp]
		public void Setup()
		{
			_kernel = TestHelper.GetKernel(out _mockDialogHelper, out _);
			_documentViewModelFactory = _kernel.Get<IDocumentViewModelFactory>();
		}

		[TestCase( XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase( XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase( XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase( XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase( XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE)]
        public void StepInputDocument(string version, string type)
		{
			XNamespace ns = version;
			var vehicleInput = MockDocument.GetDeclarationJob().AddStepInput(
				type,
				version);
			var document = _documentViewModelFactory.CreateDocumentViewModel(vehicleInput);
			var stepInputViewModel = document as StageInputViewModel;

			var arch = stepInputViewModel.Architecture;
			if (type == XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE) {
				Assert.AreEqual(arch, CompletedBusArchitecture.Conventional);
			}

			if (type == XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE) {
				Assert.AreEqual(arch, CompletedBusArchitecture.PEV);
			}

			if (type == XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE)
			{
				Assert.AreEqual(arch, CompletedBusArchitecture.HEV);
			}

			if (type == XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE)
			{
				Assert.AreEqual(arch, CompletedBusArchitecture.IEPC);
			}

			if (type == XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE)
			{
				Assert.AreEqual(arch, CompletedBusArchitecture.Exempted);
			}

            Assert.NotNull(stepInputViewModel);
		}

		[TestCase]
		public void NewStepInput()
		{
			var exempted = false;
			var document = _documentViewModelFactory.GetCreateNewStepInputViewModel(exempted);

		}


		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE, VectoSimulationJobType.ConventionalVehicle)]
		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE, VectoSimulationJobType.SerialHybridVehicle)]
		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE, VectoSimulationJobType.ParallelHybridVehicle)]
        [TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE, VectoSimulationJobType.BatteryElectricVehicle)]
		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE, VectoSimulationJobType.IEPC_E)]
		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE, VectoSimulationJobType.EngineOnlySimulation)]
        public void MultistepInput(string version, string type, VectoSimulationJobType jobType)
		{
			var ns = version;
			IMultistepBusInputDataProvider multistepInput = MockDocument.GetMultistepInput(ns, type, 3);
			var mock = Mock.Get(multistepInput);
			mock.SetupGet(c => c.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleType).Returns(jobType);
			mock.SetupGet(c => c.JobInputData.ConsolidateManufacturingStage.Vehicle.ExemptedVehicle)
				.Returns(type == XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE);
			var doc = _documentViewModelFactory.CreateDocumentViewModel(multistepInput);
			Assert.NotNull(doc);
			var multistepViewModel = doc as MultiStageJobViewModel_v0_1;
			Assert.NotNull(multistepViewModel);
		}

		
		[TestCase(XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1, "VectoOutputMultistepType", XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase(XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1, "VectoOutputMultistepType", XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase(XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1, "VectoOutputMultistepType", XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase(XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1, "VectoOutputMultistepType",XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE)]
		public void MultistepVIFInputData(string vifVersion, string vifType, string stepVersion, string stepType)
		{
			XNamespace vifNs = vifVersion;
			XNamespace stepNs = stepVersion;

            int stages = 3;

			IMultistageVIFInputData vifInputData = MockDocument.GetMultistepVIFInputData(vifNs, vifType, stages, stepType, stepNs);


			var doc = _documentViewModelFactory.CreateDocumentViewModel(vifInputData);
			Assert.NotNull(doc);
		}

		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24.XSD_TYPE, XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationPevPrimaryBusDataProviderV24.XSD_TYPE, XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE)]
		[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevPxPrimaryBusDataProviderV24.XSD_TYPE, XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE)]
		public void MultistepPrimaryAndStageInputDataProvider(string primaryVersion, string primaryType, string stepVersion, string stepVehicleType)
		{
			XNamespace primaryNs = primaryVersion;
			XNamespace stepNs = stepVersion;
			IMultistagePrimaryAndStageInputDataProvider primaryAndStep = MockDocument.GetPrimaryAndStepInput(
				primaryBusVersion:primaryNs,
				primaryType:primaryType,
				stepVehicleVersion:stepVersion,
				stepVehicleType:stepVehicleType
			);
			var doc = _documentViewModelFactory.CreateDocumentViewModel(primaryAndStep);
			Assert.NotNull(doc);
		}


    }
}
