using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
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

			var outputFile = primary_vehicle_only.Replace(".xml", "_vif_output_mandatory_fields.xml");

			deleteFile(outputFile);
			getMockDialogHelper(null, fileToSave: outputFile);
			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(null, outputFile).Object);

			var multistageConcrete = multistagevm as MultiStageJobViewModel_v0_1;

			multistageConcrete.SaveVIFCommand.Execute(null);

			Assert.IsTrue(checkFileExists(outputFile));
		}


		[Test]
		public void LoadPrimaryVehicleAndStageInputThenCreateVif()
		{
			var multistagevm = loadFile(primary_vehicle_only).MultiStageJobViewModel;

			loadVehicleData(multistagevm as MultiStageJobViewModel_v0_1, stageInputFullSample);

			var outputFile = "/output/loadPrimaryAndAddStageInput.xml";

			//deleteFile(outputFile);
			var mockDialogHelper = getMockDialogHelper(null, fileToSave: outputFile);
			Debug.WriteLine($"Write to {mockDialogHelper.Object.SaveToXMLDialog()}");
			var multistageVMConc = multistagevm as MultiStageJobViewModel_v0_1;

			MultiStageJobViewModel_v0_1.SaveVif(multistageVMConc, mockDialogHelper.Object.SaveToXMLDialog());

			//multistageVMConc.SaveVIFCommand.Execute(null);


			//Assert.IsTrue(checkFileExists(outputFile));


		}



    }
}
