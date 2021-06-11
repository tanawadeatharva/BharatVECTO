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

			Assert.AreEqual(2, stage);

			//Set Necessary Fields
			var vehicle =
				multistagevm.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			
			var writer = GetFileOutputVIFWriter(multistagevm);
			
			deleteFile(writer.XMLMultistageReportFileName);
			setMockDialogHelper(null, writer.XMLMultistageReportFileName);
			_kernel.Rebind<IDialogHelper>().ToConstant(setMockDialogHelper(null, writer.XMLMultistageReportFileName).Object);

			MultiStageJobViewModel_v0_1.SaveVif(multistagevm, writer);

			Assert.IsTrue(File.Exists(writer.XMLMultistageReportFileName));

			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistageOutputData));
		}


		[Test]
		public void TestAirdragLoadAndSave()
		{
			

			setMockDialogHelper(consolidated_multiple_stages, null);
			
			var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();
			newMultistageJobViewModel.AddVifFile.Execute(null);
			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);

			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);

			var multiStageViewModel = newMultistageJobViewModel.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);

			setMockDialogHelper(stageInputFullSample, null);

			multiStageViewModel.LoadVehicleDataCommand.Execute(null);
			
			var vehicle =
				multiStageViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			vehicle.AirdragModifiedMultistage = true;
			vehicle.VehicleDeclarationType = VehicleDeclarationType.interim;
			
			var writer = GetFileOutputVIFWriter(multiStageViewModel);
			
			deleteFile(writer.XMLMultistageReportFileName);
			setMockDialogHelper(null, writer.XMLMultistageReportFileName);

			MultiStageJobViewModel_v0_1.SaveVif(multiStageViewModel, writer);
			
			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistageOutputData));
		}


		private FileOutputVIFWriter GetFileOutputVIFWriter(IMultiStageJobViewModel multistageViewModel)
		{
			var outputFileName = primary_vehicle_only.Replace(".xml", "_vif_output_mandatory_fields.xml");
			var outputFilePath = Path.Combine(DirPath, outputFileName);

			var currentStageCount = multistageViewModel.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? 0;
			return  new FileOutputVIFWriter(outputFilePath, currentStageCount);
		}

	}
}
