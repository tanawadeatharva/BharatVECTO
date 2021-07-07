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
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	public class VIFTests : ViewModelTestBase
	{

		public const string _finalVifReport4 = "final.VIF_Report_4.xml";

		[Test]
		public void loadPrimaryVehicleOnlyAndCreateNewVIF()
		{
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var stage = multistagevm.ManufacturingStageViewModel.StageCount;

			Assert.AreEqual(2, stage);

			//Set Mandatory Fields
			var vehicle =
				multistagevm.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
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
				multistagevm.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
			

			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			vehicle.Model = "Model";


		}

		[TestCase(true, 1, TestName="With Airdrag Component v1")]
		[TestCase(true, 2, TestName="With Airdrag Component v2")]
		[TestCase(false, 0, TestName="Without Airdrag Component")]
		public void CreateCompletedFinalVIFWithAirdrag(bool loadAirdrag, int airdragVersion)
		{
			var multistagevm = loadFile(_finalVifReport4);

			var VehicleViewModel = multistagevm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			VehicleViewModel.Manufacturer = "Manufacturer";
            VehicleViewModel.ManufacturerAddress = "Manufacturer Address";
			VehicleViewModel.VIN = "1234567";
			VehicleViewModel.Model = "asdf";
			VehicleViewModel.AirdragModifiedEnum = loadAirdrag ? AIRDRAGMODIFIED.TRUE: AIRDRAGMODIFIED.FALSE;
			VehicleViewModel.VehicleDeclarationType = VehicleDeclarationType.final;

			//SETADAS
			VehicleViewModel.EngineStopStartNullable = true;
			VehicleViewModel.EcoRollTypeNullable = EcoRollType.WithEngineStop;
			VehicleViewModel.PredictiveCruiseControlNullable = PredictiveCruiseControlType.Option_1_2_3;
			VehicleViewModel.ATEcoRollReleaseLockupClutch = false;


			if (loadAirdrag) {
				var airdragTestFile = airdragVersion == 2 ? airdragLoadTestFilev2 : airdragLoadTestFile;
				Assert.IsTrue(VehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(GetFullPath(airdragTestFile)));
			}
		
			var resultFile = multistagevm.MultiStageJobViewModel.SaveVif(GetFullPath(
				"completed_final" + ".xml"));

			
			var jobListVm = _kernel.Get<IJobListViewModel>();
			Assert.That(() => jobListVm.Jobs.Count, Is.EqualTo(2));

			Assert.IsTrue(jobListVm.Jobs[1].CanBeSimulated);
		}



        [Test]
		public void CreateVifWrongDecimal()
		{
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var stage = multistagevm.ManufacturingStageViewModel.StageCount;

			Assert.AreEqual(2, stage);

		//Set Necessary Fields
			var vehicle =
			multistagevm.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
			

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
				multiStageViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
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
