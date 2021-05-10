using System.IO;
using System.Xml;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;

using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	public class VIFTests : ViewModelTestBase
	{

		[Test]
		public void loadPrimaryVehicleOnlyAndCreateNewVIF()
		{
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel;
			var stage = multistagevm.ManufacturingStageViewModel.StageCount;

			Assert.AreEqual(1, stage);

			//Set Necessary Fields
			var vehicle =
				multistagevm.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";


			var outputFileName = primary_vehicle_only.Replace(".xml", "_vif_output_mandatory_fields.xml");
			var outputFilePath = Path.Combine(DirPath, outputFileName);

			var nextStageNumber = multistagevm.ManufacturingStageViewModel?.StageCount + 2 ?? 2;
			var expectedOutputFilePath = outputFilePath.Replace(".xml", $".{FileOutputVIFWriter.REPORT_ENDING_PREFIX}{nextStageNumber}.xml");
			deleteFile(expectedOutputFilePath);


			getMockDialogHelper(null, outputFilePath);
			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(null, outputFilePath).Object);

			MultiStageJobViewModel_v0_1.SaveVif(multistagevm, outputFilePath);

			Assert.IsTrue(File.Exists(expectedOutputFilePath));

			var validator = new XMLValidator(XmlReader.Create(expectedOutputFilePath));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistageOutputData));
		}


		[Test]
		public void TestAirdragLoad()
		{
			var multiplestages = Path.GetFullPath(DirPath + "vecto_multistage_consolidated_multiple_stages.xml");

			var dialogMockConsolidatedMultipleStage = new Mock<IDialogHelper>();
			dialogMockConsolidatedMultipleStage.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(multiplestages);
			dialogMockConsolidatedMultipleStage.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(multiplestages);
			_kernel.Rebind<IDialogHelper>().ToConstant(dialogMockConsolidatedMultipleStage.Object);
			
			var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();
			newMultistageJobViewModel.AddVifFile.Execute(null);
			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);

			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);

			var multiStageViewModel = newMultistageJobViewModel.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);

			var vehicleInputData = Path.GetFullPath(DirPath + "vecto_vehicle-stage_input_full-sample.xml");
			Assert.IsTrue(File.Exists(vehicleInputData));

			var vehicleInputDataFiledialogMock = new Mock<IDialogHelper>();
			vehicleInputDataFiledialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(vehicleInputData);
			vehicleInputDataFiledialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(vehicleInputData);
			_kernel.Rebind<IDialogHelper>().ToConstant(vehicleInputDataFiledialogMock.Object);

			multiStageViewModel.LoadVehicleDataCommand.Execute(null);

			
			var vehicle =
				multiStageViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			vehicle.AirdragModifiedMultistage = true;
			vehicle.VehicleDeclarationType = VehicleDeclarationType.interim;
			
			var outputFileName = primary_vehicle_only.Replace(".xml", "_vif_output_mandatory_fields.xml");
			var outputFilePath = Path.Combine(DirPath, outputFileName);

			var nextStageNumber = multiStageViewModel.ManufacturingStages?.Count + 2 ?? 2;
			var expectedOutputFilePath = outputFilePath.Replace(".xml", $".{FileOutputVIFWriter.REPORT_ENDING_PREFIX}{nextStageNumber}.xml");
			deleteFile(expectedOutputFilePath);
			
			getMockDialogHelper(null, outputFilePath);
			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(null, outputFilePath).Object);
			
			MultiStageJobViewModel_v0_1.SaveVif(multiStageViewModel, outputFilePath);
			
			var validator = new XMLValidator(XmlReader.Create(expectedOutputFilePath));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistageOutputData));

		}

	}
}
