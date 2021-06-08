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
		public void airdragModifiedInPreviousStages()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleVM = vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
				DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsTrue(vehicleVM.AirdragModifiedMultistageEditingEnabled);

			//try to change to false

			vehicleVM.AirdragModifiedMultistage = false; //should not change the value
			Assert.IsTrue(vehicleVM.AirdragModifiedMultistageEditingEnabled);
		}

		[Test]
		public void airdragNotModifiedInPreviousStages()
		{
			var vm = loadFile(consolidated_multiple_stages);

			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsFalse(vehicleVM.AirdragModifiedMultistageEditingEnabled);
		}

		/// <summary>
		///  1st interim stage adds AirDrag component 
		///		=> 'AirdragModifiedMultistage' not present in input, VIF 2, Airdrag Component in VIF
		///		=> 'AirdragModifiedMultistage' required in all consecutive stages
		/// </summary>
		[Test]
		public void airdragComponentLoadedFirstTime()
		{
			///Load VIF without airdrag 

			var vm = loadFile(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleVM.AirdragModifiedMultistage);

			var airdragViewModel = vehicleVM.MultistageAirdragViewModel as MultistageAirdragViewModel;
			Assert.IsTrue(airdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile)));

			Assert.IsNull(vehicleVM.AirdragModifiedMultistage);
			Assert.IsFalse(vehicleVM.AirdragModifiedMultistageMandatory);

			vehicleVM.AirdragModifiedMultistageEditingEnabled = true;
			Assert.IsFalse(vehicleVM.AirdragModifiedMultistageEditingEnabled);

			Assert.IsNull(vehicleVM.AirdragModifiedMultistage);

			//Set Mandatory Fields
			vehicleVM.Manufacturer = "testManufacturer";
			vehicleVM.ManufacturerAddress = "Address";
			vehicleVM.VIN = "123456789";

			//Save as new VIF
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var outputName = "AidragLoadedInFirstStage";
			multistageJobViewModel.SaveVif(GetFullPath($"{outputName}.xml"));

			var resultFile = $"{outputName}.VIF_Report_2.xml";
			Assert.IsTrue(checkFileNameExists(resultFile));
			var secondstageVM = loadFile(resultFile);
			Assert.IsNotNull(secondstageVM);
			var secondStageVehicleVM =
				(secondstageVM.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel) as
				DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.IsTrue(secondStageVehicleVM.AirdragModifiedMultistageEditingEnabled);
			Assert.IsTrue(secondStageVehicleVM.AirdragModifiedMultistageMandatory);
			Assert.IsNull(secondStageVehicleVM.ConsolidatedVehicleData.AirdragModifiedMultistage);

			//try to disable AirdragModified
			secondStageVehicleVM.AirdragModifiedMultistageEditingEnabled = false;
			Assert.IsTrue(secondStageVehicleVM.AirdragModifiedMultistageEditingEnabled);
		}
		/// <summary>
		///  no airdrag component set in VIF => AirdragModifiedMultistage is disabled
		/// </summary>
		[Test]
		public void airdragModifiedDisabled()
		{
			var vm = loadFile(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleVM.AirdragModifiedMultistage);

			Assert.IsNull(vehicleVM.AirdragModifiedMultistage);
			Assert.IsFalse(vehicleVM.AirdragModifiedMultistageMandatory);

			var airdragViewModel = vehicleVM.MultistageAirdragViewModel;
			Assert.IsTrue(airdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile)), "Airdrag file not loaded");
			Assert.IsFalse(vehicleVM.AirdragModifiedMultistageMandatory);

			vehicleVM.AirdragModifiedMultistageEditingEnabled = true;
			Assert.IsFalse(vehicleVM.AirdragModifiedMultistageEditingEnabled);

		}
		/// <summary>
		/// airdrag component is in VIF set => AirdragModifiedMultistage is mandatory
		/// </summary>
		[Test]

		public void airdragModifiedMandatory()
		{
			//var vm = loadFile(primary_vehicle_only);
			//var vehicleVM =
			//	vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
			//		DeclarationInterimStageBusVehicleViewModel_v2_8;

			//Assert.IsNull(vehicleVM.AirdragModifiedMultistage);

			//Assert.IsNull(vehicleVM.AirdragModifiedMultistage);
			//Assert.IsFalse(vehicleVM.AirdragModifiedMultistageMandatory);

			//vehicleVM.AirdragModifiedMultistageEditingEnabled = true;
			//Assert.IsFalse(vehicleVM.AirdragModifiedMultistageEditingEnabled);

		}

		[Test]
		public void temporarySaveAirdragComponent1()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;

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
		public void temporarySaveAirdragComponent2()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistage);


			//Load input file
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var mockDialogHelper = setMockDialogHelper(stageInputFullSample, null);
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
		public void removeAirdragComponent()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleViewModel.AirdragModifiedMultistage);


			//Load input file
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var mockDialogHelper = setMockDialogHelper(stageInputFullSample, null);
			multistageJobViewModel.LoadVehicleDataCommand.Execute(null);


			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);
			vehicleViewModel.MultistageAirdragViewModel.RemoveAirdragComponent(); //remove airdrag viewmodel;
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);



		}




		[Test]
		public void airdragModifiedSetToTrueWhenComponentIsLoaded()
		{
			var vm = loadFile(consolidated_multiple_stages_airdrag);

			var vehicleViewModel = vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;

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

			//Airdrag modified set to false if the component is removed
			vehicleViewModel.MultistageAirdragViewModel.RemoveAirdragComponent();
			Assert.IsTrue(vehicleViewModel.AirdragModifiedMultistage);

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
