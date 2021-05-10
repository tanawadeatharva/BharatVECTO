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
			getMockDialogHelper(null, fileToSave: primary_vehicle_only.Replace(".xml", "_vif_output.xml"));
			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(null, outputFile).Object);

			MultiStageJobViewModel_v0_1.SaveVif(multistagevm, outputFile);
			
			Assert.IsTrue(checkFileExists(outputFile));

		}



    }
}
