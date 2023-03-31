using System.IO;
using NUnit.Framework;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.ViewModelTests
{
    [TestFixture]
    public class AirdragTests : ViewModelTestBase
    {
		/*
		 *		Airdrag component loaded -> Airdragmodified true (if mandatory)
				Airdrag component deleted -> Airdragmodified unchanged
				Airdrag modified false -> Airdragcomponent set to null (& temporarily saved)
				Airdrag modified true -> Airdragcomponent unchanged
		 *
		 *
		 */

		#region Airdrag
		[Test]
		public void AirdragModifiedInPreviousStages()
		{
			var vm = LoadFileFromPath(TestData.consolidated_multiple_stages_airdrag);

			var vehicleVm = vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
				InterimStageBusVehicleViewModel;

			Assert.IsTrue(vehicleVm.AirdragModifiedMultistepEditingEnabled);

			//try to change to false

			vehicleVm.AirdragModifiedMultistep = false; //should not change the value
			Assert.IsTrue(vehicleVm.AirdragModifiedMultistepEditingEnabled);
		}

		[Test]
		public void AirdragNotModifiedInPreviousStages()
		{
			var vm = LoadFileFromPath(TestData.consolidated_multiple_stages);

			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel;

			Assert.IsFalse(vehicleVm.AirdragModifiedMultistepEditingEnabled);
		}

		/// <summary>
		///  1st interim stage adds AirDrag component 
		///		=> 'AirdragModifiedMultistage' not present in input, VIF 2, Airdrag Component in VIF
		///		=> 'AirdragModifiedMultistage' required in all consecutive stages
		/// </summary>
		[Test]
		public void AirdragComponentLoadedFirstTime()
		{
			///Load VIF without airdrag 

			var vm = LoadFileFromPath(TestData.primary_vehicle_only);
			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel;

			Assert.IsNull(vehicleVm.AirdragModifiedMultistep);

			var airdragViewModel = vehicleVm.MultistageAirdragViewModel as MultistageAirdragViewModel;
			var airdragPath = Path.GetFullPath(TestData.airdragLoadTestFile);
			AssertHelper.FileExists(airdragPath);
			Assert.IsTrue(airdragViewModel.LoadAirdragFile(airdragPath));

			Assert.IsNull(vehicleVm.AirdragModifiedMultistep);
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistepMandatory);

			vehicleVm.AirdragModifiedMultistepEditingEnabled = true;
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistepEditingEnabled);

			Assert.IsNull(vehicleVm.AirdragModifiedMultistep);

			//Set Mandatory Fields
			vehicleVm.Manufacturer = "testManufacturer";
			vehicleVm.ManufacturerAddress = "Address";
			vehicleVm.VIN = "123456789";

			//Save as new VIF
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var outputName = "AidragLoadedInFirstStage";
			multistageJobViewModel.SaveVif(Path.GetFullPath($"{outputName}.xml"));
			_mockDialogHelper.AssertNoErrorDialogs();
			var resultFile = $"{outputName}.VIF_Report_2.xml";
			AssertHelper.FileExists(resultFile);
			var secondstageVm = LoadFileFromPath(resultFile);
			Assert.IsNotNull(secondstageVm);
			var secondStageVehicleVm =
				(secondstageVm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel) as
				InterimStageBusVehicleViewModel;
			Assert.IsTrue(secondStageVehicleVm.AirdragModifiedMultistepEditingEnabled);
			Assert.IsTrue(secondStageVehicleVm.AirdragModifiedMultistepMandatory);
			Assert.IsNull(secondStageVehicleVm.ConsolidatedVehicleData.AirdragModifiedMultistep);

			//try to disable AirdragModified
			secondStageVehicleVm.AirdragModifiedMultistepEditingEnabled = false;
			Assert.IsTrue(secondStageVehicleVm.AirdragModifiedMultistepEditingEnabled);
		}
		/// <summary>
		///  no airdrag component set in VIF => AirdragModifiedMultistage is disabled
		/// </summary>
		[Test]
		public void AirdragModifiedDisabled()
		{
			var vm = LoadFileFromPath(TestData.primary_vehicle_only);
			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel;

			Assert.IsNull(vehicleVm.AirdragModifiedMultistep);

			Assert.IsNull(vehicleVm.AirdragModifiedMultistep);
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistepMandatory);

			var airdragViewModel = vehicleVm.MultistageAirdragViewModel;
			Assert.IsTrue(airdragViewModel.LoadAirdragFile(Path.GetFullPath(TestData.airdragLoadTestFile)), "Airdrag file not loaded");
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistepMandatory);

			vehicleVm.AirdragModifiedMultistepEditingEnabled = true;
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistepEditingEnabled);

		}
		/// <summary>
		/// airdrag component is in VIF set => AirdragModifiedMultistage is mandatory
		/// </summary>
		[Test]

		public void AirdragModifiedMandatory()
		{
			//var vm = loadFile(primary_vehicle_only);
			//var vehicleVM =
			//	vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
			//		InterimStageBusVehicleViewModel;

			//Assert.IsNull(vehicleVM.AirdragModifiedMultistage);

			//Assert.IsNull(vehicleVM.AirdragModifiedMultistage);
			//Assert.IsFalse(vehicleVM.AirdragModifiedMultistageMandatory);

			//vehicleVM.AirdragModifiedMultistageEditingEnabled = true;
			//Assert.IsFalse(vehicleVM.AirdragModifiedMultistageEditingEnabled);

		}

		[Test]
		public void TemporarySaveAirdragComponent1()
		{
			var vm = LoadFileFromPath(TestData.consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistep);


			//Load airdrag file
			var airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(Path.GetFullPath(TestData.airdragLoadTestFile));
			var loadedAirdragComponent = vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel;
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			//Airdrag modified set to true if a component is loaded and the field is mandatory
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistep);
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistepMandatory);
			Assert.AreEqual(
				AIRDRAGMODIFIED.TRUE,
				vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);

			vehicleViewModel.AirdragModifiedMultistep = false;
			Assert.AreEqual(AIRDRAGMODIFIED.FALSE,vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);
			Assert.IsNull(vehicleViewModel.Components?.AirdragInputData);
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

			vehicleViewModel.AirdragModifiedMultistep = true;
			Assert.AreEqual(loadedAirdragComponent, vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

		}

		[Test]
		public void TemporarySaveAirdragComponent2()
		{
			var vm = LoadFileFromPath(TestData.consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistep);


			//Load input file
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			multistageJobViewModel.ManufacturingStageViewModel.LoadStageInputData(Path.GetFullPath(TestData.stageInputFullSample));


		

			var loadedAirdragComponent = vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel;
			Assert.NotNull(loadedAirdragComponent);

			//Airdrag modified set to true if a component is loaded and the field is mandatory
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistep);
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistepMandatory);
			Assert.AreEqual(
				AIRDRAGMODIFIED.TRUE,
				vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);

			vehicleViewModel.AirdragModifiedMultistep = false;
			Assert.AreEqual(AIRDRAGMODIFIED.FALSE, vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);
			Assert.IsNull(vehicleViewModel.Components?.AirdragInputData);
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

			vehicleViewModel.AirdragModifiedMultistep = true;
			Assert.AreEqual(loadedAirdragComponent, vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

		}

		[Test]
		public void RemoveAirdragComponent()
		{
			var vm = LoadFileFromPath(TestData.consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistep);


			//Load input file
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			multistageJobViewModel.ManufacturingStageViewModel.LoadStageInputData(
				Path.GetFullPath(TestData.stageInputFullSample));


			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistep);
			vehicleViewModel.MultistageAirdragViewModel.RemoveAirdragComponent(); //remove airdrag viewmodel;
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistep);



		}




		[Test]
		public void AirdragModifiedSetToTrueWhenComponentIsLoaded()
		{
			var vm = LoadFileFromPath(TestData.consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistep);


			//Load airdrag file
			var airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(Path.GetFullPath(TestData.airdragLoadTestFile));
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			//Airdrag modified set to true if a component is loaded and the field is mandatory
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistep);
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistepMandatory);
			Assert.AreEqual(
				AIRDRAGMODIFIED.TRUE,
				vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);


			//AirdragComponent is removed when airdragmodified is set to false;
			//Load airdrag file
			airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(Path.GetFullPath(TestData.airdragLoadTestFile));
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			vehicleViewModel.AirdragModifiedMultistep = false;
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);


		}

		#endregion	


	}
}
