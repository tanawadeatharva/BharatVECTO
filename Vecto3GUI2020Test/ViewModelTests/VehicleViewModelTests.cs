using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test.ViewModelTests
{
    [TestFixture]
    public class VehicleViewModelTests : ViewModelTestBase
    {

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
			vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel = null;
			Assert.IsFalse(vehicleViewModel.AirdragModifiedMultistage);

			//AirdragComponent is removed when airdragmodified is set to false;
			//Load airdrag file
			airdragLoaded = vehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(GetFullPath(airdragLoadTestFile));
			Assert.IsTrue(airdragLoaded, "Airdrag file was not loaded");

			vehicleViewModel.AirdragModifiedMultistage = false;
			Assert.IsNull(vehicleViewModel.MultistageAirdragViewModel.AirDragViewModel);


		}




		#endregion

		[Test]
		public void restoreValuesWhenEditingAgain()
		{
			var vm = loadFile(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;
			var enteredString = "test";
			var modelParam = vehicleVM.ParameterViewModels[nameof(vehicleVM.Model)];
			//Null after loading
			Assert.IsNull(vehicleVM.Model);


			//Enable Editing
			modelParam.EditingEnabled = true;
			Assert.IsNotNull(vehicleVM.Model);

			//Enter Value 
			modelParam.CurrentContent = enteredString;

			//Stored To VM
			Assert.AreEqual(vehicleVM.Model, enteredString);

			//DisableEditing
			modelParam.EditingEnabled = false;
			//Currentvalue stored in storedcontent
			Assert.AreEqual(enteredString, modelParam.StoredContent);


			//Value is null again
			Assert.IsNull(vehicleVM.Model);

			modelParam.EditingEnabled = true;
			Assert.AreEqual(modelParam.CurrentContent, enteredString);

			//Change value 
			var enteredString2 = "test2";
            modelParam.CurrentContent = enteredString2;
			modelParam.EditingEnabled = false;
			modelParam.EditingEnabled = true;
			Assert.AreEqual(enteredString2, modelParam.CurrentContent);


			//modify through CurrentContentProperty
			modelParam.EditingEnabled = false;
			modelParam.CurrentContent = enteredString;
			Assert.AreEqual(modelParam.CurrentContent, enteredString);
			Assert.IsTrue(modelParam.EditingEnabled);

		}

		[Test]
		public void SIDummyCreation()
		{

			var vm = loadFile(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleVM.CurbMassChassis?.Value());
			var curbMassParameter = vehicleVM.ParameterViewModels[nameof(vehicleVM.CurbMassChassis)];

			Assert.IsNotNull(curbMassParameter.DummyContent);
			Assert.IsTrue(curbMassParameter.DummyContent is Kilogram);

			curbMassParameter.EditingEnabled = true;
			Assert.IsNotNull(vehicleVM.CurbMassChassis);
		}



		[Test]
		public void loadVehicleDataAgainUnset()
		{



		}

		[Test]
		public void NoErrorAfterDataLoading()
		{
			var vm = loadFile(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;
			setMockDialogHelper(stageInputFullSample);
			var vmConc = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			vmConc.LoadVehicleDataCommand.Execute(null);
			Assert.IsFalse(vmConc.ManufacturingStageViewModel.VehicleViewModel.HasErrors);
			
		}

		//[Test]
		//public void groupEditing()
		//{
		//	var vm = loadFile(primary_vehicle_only);
		//	var vehicleVM =
		//		vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
		//			DeclarationInterimStageBusVehicleViewModel_v2_8;

		//	vehicleVM.NumberOfPassengersUpperDeck = 2;

		//	Assert.IsTrue(vehicleVM.NumberOfPassengersEditingEnabled);
		//	Assert.AreEqual(2, vehicleVM.NumberOfPassengersUpperDeck);
		//	vehicleVM.NumberOfPassengersLowerDeck = 3;
		//	Assert.AreEqual(3, vehicleVM.NumberOfPassengersLowerDeck);
		//}


		//[Test]
		//public void automaticallyEnableEditingWhenContentIsSet()
		//{
		//	var vm = loadFile(primary_vehicle_only);
		//	var vehicleVM =
		//		vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
		//			DeclarationInterimStageBusVehicleViewModel_v2_8;

			
		//	vehicleVM.NumberOfPassengersUpperDeck = 2;
		//	Assert.IsTrue(vehicleVM.NumberOfPassengersEditingEnabled);

		//	vehicleVM.NumberOfPassengersUpperDeck = null;

		//	getMockDialogHelper(stageInputFullSample);
		//	var vmConc = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
		//	vmConc.LoadVehicleDataCommand.Execute(null);


		//	Assert.IsTrue(vehicleVM.ParameterViewModels[nameof(vehicleVM.NumberOfPassengersUpperDeck)].EditingEnabled);
		//	Assert.IsTrue(vehicleVM.NumberOfPassengersEditingEnabled);
		//}




		#region ADAS
		[Test]
		public void loadPrimaryAndEdit()
		{
			var vm = loadFile(primary_vehicle_only);
			Assert.NotNull(vm);

			var vehicleViewModel =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					DeclarationInterimStageBusVehicleViewModel_v2_8;

			var vehicleData = vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle;



			vehicleViewModel.EcoRollTypeNullable = EcoRollType.WithEngineStop;
			Assert.NotNull(vehicleData.ADAS);


			vehicleViewModel.EcoRollTypeNullable = null;
			Assert.Null(vehicleData.ADAS);





		}







		#endregion




	}
}
