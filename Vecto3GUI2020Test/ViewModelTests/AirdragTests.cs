using NUnit.Framework;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

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
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleVm = vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
				InterimStageBusVehicleViewModel_v2_8;

			Assert.IsTrue(vehicleVm.AirdragModifiedMultistageEditingEnabled);

			//try to change to false

			vehicleVm.AirdragModifiedMultistage = false; //should not change the value
			Assert.IsTrue(vehicleVm.AirdragModifiedMultistageEditingEnabled);
		}

		[Test]
		public void AirdragNotModifiedInPreviousStages()
		{
			var vm = loadFile(consolidated_multiple_stages);

			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;

			Assert.IsFalse(vehicleVm.AirdragModifiedMultistageEditingEnabled);
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

			var vm = loadFile(primary_vehicle_only);
			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleVm.AirdragModifiedMultistage);

			var airdragViewModel = vehicleVm.MultistageAirdragViewModel as MultistageAirdragViewModel;
			Assert.IsTrue(airdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile)));

			Assert.IsNull(vehicleVm.AirdragModifiedMultistage);
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistageMandatory);

			vehicleVm.AirdragModifiedMultistageEditingEnabled = true;
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistageEditingEnabled);

			Assert.IsNull(vehicleVm.AirdragModifiedMultistage);

			//Set Mandatory Fields
			vehicleVm.Manufacturer = "testManufacturer";
			vehicleVm.ManufacturerAddress = "Address";
			vehicleVm.VIN = "123456789";

			//Save as new VIF
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var outputName = "AidragLoadedInFirstStage";
			multistageJobViewModel.SaveVif(GetFullPath($"{outputName}.xml"));

			var resultFile = $"{outputName}.VIF_Report_2.xml";
			Assert.IsTrue(checkFileNameExists(resultFile));
			var secondstageVm = loadFile(resultFile);
			Assert.IsNotNull(secondstageVm);
			var secondStageVehicleVm =
				(secondstageVm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel) as
				InterimStageBusVehicleViewModel_v2_8;
			Assert.IsTrue(secondStageVehicleVm.AirdragModifiedMultistageEditingEnabled);
			Assert.IsTrue(secondStageVehicleVm.AirdragModifiedMultistageMandatory);
			Assert.IsNull(secondStageVehicleVm.ConsolidatedVehicleData.AirdragModifiedMultistage);

			//try to disable AirdragModified
			secondStageVehicleVm.AirdragModifiedMultistageEditingEnabled = false;
			Assert.IsTrue(secondStageVehicleVm.AirdragModifiedMultistageEditingEnabled);
		}
		/// <summary>
		///  no airdrag component set in VIF => AirdragModifiedMultistage is disabled
		/// </summary>
		[Test]
		public void AirdragModifiedDisabled()
		{
			var vm = loadFile(primary_vehicle_only);
			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleVm.AirdragModifiedMultistage);

			Assert.IsNull(vehicleVm.AirdragModifiedMultistage);
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistageMandatory);

			var airdragViewModel = vehicleVm.MultistageAirdragViewModel;
			Assert.IsTrue(airdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile)), "Airdrag file not loaded");
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistageMandatory);

			vehicleVm.AirdragModifiedMultistageEditingEnabled = true;
			Assert.IsFalse(vehicleVm.AirdragModifiedMultistageEditingEnabled);

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
			//		InterimStageBusVehicleViewModel_v2_8;

			//Assert.IsNull(vehicleVM.AirdragModifiedMultistage);

			//Assert.IsNull(vehicleVM.AirdragModifiedMultistage);
			//Assert.IsFalse(vehicleVM.AirdragModifiedMultistageMandatory);

			//vehicleVM.AirdragModifiedMultistageEditingEnabled = true;
			//Assert.IsFalse(vehicleVM.AirdragModifiedMultistageEditingEnabled);

		}

		[Test]
		public void TemporarySaveAirdragComponent1()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistage);


			//Load airdrag file
			var airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile));
			var loadedAirdragComponent = vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel;
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			//Airdrag modified set to true if a component is loaded and the field is mandatory
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistageMandatory);
			Assert.AreEqual(
				AIRDRAGMODIFIED.TRUE,
				vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);

			vehicleViewModel.AirdragModifiedMultistage = false;
			Assert.AreEqual(AIRDRAGMODIFIED.FALSE,vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);
			Assert.IsNull(vehicleViewModel.Components?.AirdragInputData);
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

			vehicleViewModel.AirdragModifiedMultistage = true;
			Assert.AreEqual(loadedAirdragComponent, vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

		}

		[Test]
		public void TemporarySaveAirdragComponent2()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistage);


			//Load input file
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var mockDialogHelper = SetMockDialogHelper(stageInputFullSample, null);
			multistageJobViewModel.LoadVehicleDataCommand.Execute(null);


		

			var loadedAirdragComponent = vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel;
			Assert.NotNull(loadedAirdragComponent);

			//Airdrag modified set to true if a component is loaded and the field is mandatory
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistageMandatory);
			Assert.AreEqual(
				AIRDRAGMODIFIED.TRUE,
				vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);

			vehicleViewModel.AirdragModifiedMultistage = false;
			Assert.AreEqual(AIRDRAGMODIFIED.FALSE, vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);
			Assert.IsNull(vehicleViewModel.Components?.AirdragInputData);
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

			vehicleViewModel.AirdragModifiedMultistage = true;
			Assert.AreEqual(loadedAirdragComponent, vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);

		}

		[Test]
		public void RemoveAirdragComponent()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistage);


			//Load input file
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var mockDialogHelper = SetMockDialogHelper(stageInputFullSample, null);
			multistageJobViewModel.LoadVehicleDataCommand.Execute(null);


			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);
			vehicleViewModel.MultistageAirdragViewModel.RemoveAirdragComponent(); //remove airdrag viewmodel;
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);



		}




		[Test]
		public void AirdragModifiedSetToTrueWhenComponentIsLoaded()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistage);


			//Load airdrag file
			var airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile));
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			//Airdrag modified set to true if a component is loaded and the field is mandatory
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistageMandatory);
			Assert.AreEqual(
				AIRDRAGMODIFIED.TRUE,
				vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);


			//AirdragComponent is removed when airdragmodified is set to false;
			//Load airdrag file
			airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile));
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			vehicleViewModel.AirdragModifiedMultistage = false;
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);


		}

		#endregion	


	}
}
