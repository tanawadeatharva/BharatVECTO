using System.IO;
using System.Xml;
using Moq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
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
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
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
			SetMockDialogHelper(null, writer.XMLMultistageReportFileName);
			_kernel.Rebind<IDialogHelper>().ToConstant(SetMockDialogHelper(null, writer.XMLMultistageReportFileName).Object);

			multistagevm.SaveVif(multistagevm, writer);

			Assert.IsTrue(File.Exists(writer.XMLMultistageReportFileName));

			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistageOutputData));
		}

		[Test, Combinatorial]
		public void CreateVifAllParameters(
			[Values("manufacturer")] string manufacturer,
			[Values(LegislativeClass.M3)] LegislativeClass legCategory)
		{
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var stage = multistagevm.ManufacturingStageViewModel.StageCount;

			Assert.AreEqual(2, stage);

			//Set Necessary Fields
			var vehicle =
				multistagevm.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			

			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			vehicle.Model = "Model";


		}



		[Test]
		public void CreateVifWrongDecimal()
		{
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var stage = multistagevm.ManufacturingStageViewModel.StageCount;

			Assert.AreEqual(2, stage);

		//Set Necessary Fields
			var vehicle =
			multistagevm.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			

			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			vehicle.Model = "Model";

			//vehicle.MultistageAuxiliariesViewModel.
		}


		[Test]
		public void TestAirdragLoadAndSave()
		{
			SetMockDialogHelper(consolidated_multiple_stages, null);
			
			var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();
			newMultistageJobViewModel.AddVifFile.Execute(null);
			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);

			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);

			var multiStageViewModel = newMultistageJobViewModel.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);

			SetMockDialogHelper(stageInputFullSample, null);

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
			SetMockDialogHelper(null, writer.XMLMultistageReportFileName);

			multiStageViewModel.SaveVif(multiStageViewModel, writer);
			
			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistageOutputData));
		}


		private FileOutputVIFWriter GetFileOutputVIFWriter(IMultiStageJobViewModel multistageViewModel)
		{
			var outputFileName = primary_vehicle_only.Replace(".xml", "_vif_output_mandatory_fields.xml");
			var outputFilePath = Path.Combine(TestDataDirPath, outputFileName);

			var currentStageCount = multistageViewModel.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? 0;
			return  new FileOutputVIFWriter(outputFilePath, currentStageCount);
		}

	}
}
