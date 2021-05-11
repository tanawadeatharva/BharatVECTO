using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test
{
	[TestFixture]
    public class LoadAndSaveVehicleData : ViewModelTestBase
	{
		[Test]
		public void loadInputFileMultipleStage()
		{
			loadFile(consolidated_multiple_stages);
		}

		[Test]
		public void loadPrimaryAndSaveVehicleData()
		{
			var newMultiStageJob = loadFile(primary_vehicle_only);
			var vehicle = newMultiStageJob.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicle);
			vehicle.Manufacturer = "test1";
			vehicle.ManufacturerAddress = "testAddress2";
			vehicle.VIN = "VIN123456789";
			//Remove

			var manufacturingStage =
				newMultiStageJob.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;

			Assert.NotNull(manufacturingStage);


			var fileName = primary_vehicle_only.Replace(".xml", "") + "_output.xml";
			deleteFile(fileName);
			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(fileToSave:fileName).Object);
			manufacturingStage.SaveInputDataAsCommand.Execute(null);
			Assert.True(checkFileExists(fileName));
		}




		[Ignore("Validation is only performed with gui")]
		[TestCase(primary_vehicle_only)]
		[TestCase(consolidated_multiple_stages)]
		[TestCase(consolidated_one_stage)]
		public void SaveVehicleDataWithMissingFields(string fileName)
		{
			var newMultistageJobViewModel = loadFile(fileName);
			var manstage = newMultistageJobViewModel.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			Assert.NotNull(manstage);

			var saveFileName = fileName.Replace(".xml", "") + "_output.xml";
			deleteFile(saveFileName);
			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(fileToSave: saveFileName).Object);
			manstage.SaveInputDataAsCommand.Execute(null);

			Assert.False(checkFileExists(saveFileName));

		}

		[Test]
		public void reloadInputFile()
		{
			var newMultistageJobViewModel = loadFile(consolidated_multiple_stages_airdrag) as NewMultiStageJobViewModel;

			var vehicle = newMultistageJobViewModel.MultiStageJobViewModel.VehicleInputData as
				DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.NotNull(vehicle);


			Assert.True(vehicle.AirdragModifiedMultistageEditingEnabled);

			var mockDialog = getMockDialogHelper(consolidated_multiple_stages);
			newMultistageJobViewModel.AddVifFile.Execute(null);
			Assert.AreEqual(mockDialog.Object.OpenXMLFileDialog(null), newMultistageJobViewModel.VifPath);
			vehicle = newMultistageJobViewModel.MultiStageJobViewModel.VehicleInputData as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.IsFalse(vehicle.AirdragModifiedMultistageEditingEnabled);



		}


		[Test]
		public void loadInputFileMultipleStageAirdrag()
		{
			loadFile(consolidated_multiple_stages_airdrag);
		}

		[Test]
		public void loadInputFileConsolidatedOneStage()
		{
			loadFile(consolidated_one_stage);
		}

		[Test]
		public void loadInputFilePrimaryOnly()
		{
			var vm = loadFile(primary_vehicle_only);
			Assert.AreEqual(2, vm.MultiStageJobViewModel.ManufacturingStageViewModel.StageCount);

			var primaryVehicle = vm.MultiStageJobViewModel.PrimaryVehicle;
			Assert.NotNull(primaryVehicle);

			var vehicleViewModel =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(vehicleViewModel);
			Assert.Null(vehicleViewModel.Manufacturer);
			Assert.Null(vehicleViewModel.ManufacturerAddress);

			var vehicleViewModel_v2_8 = vehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicleViewModel_v2_8);

			Assert.Null(vehicleViewModel_v2_8.HeightInMm);
			Assert.Null(vehicleViewModel_v2_8.LengthInMm);
			Assert.Null(vehicleViewModel_v2_8.WidthInMm);
			Assert.Null(vehicleViewModel_v2_8.EntranceHeightInMm);

			Assert.Null(vehicleViewModel_v2_8.ConsolidatedHeightInMm);
			Assert.Null(vehicleViewModel_v2_8.ConsolidatedLengthInMm);
			Assert.Null(vehicleViewModel_v2_8.ConsolidatedWidthInMm);
			Assert.Null(vehicleViewModel_v2_8.ConsolidatedEntranceHeightInMm);


			Assert.IsFalse(vehicleViewModel_v2_8.AirdragModifiedMultistageEditingEnabled);

			Assert.IsNull(vehicleViewModel_v2_8.AirdragModifiedMultistage);
			Assert.IsNull(vehicleViewModel_v2_8.ConsolidatedAirdragmodified);
			Assert.IsNull(vehicleViewModel_v2_8.AirdragModifiedEnum);
			


			Assert.AreEqual(vehicleViewModel_v2_8.AirdragModifiedMultistageEditingEnabled, false);
			Assert.Null(vehicleViewModel_v2_8.BusAuxiliaries);


			var vifInputData = vm.MultiStageJobViewModel as IMultistageVIFInputData;

			Assert.Null(vifInputData.VehicleInputData.Components);
		}


	







		[Test]
		public void loadVehicleDataTest()
		{
			string multiplestages = "";
			multiplestages = Path.GetFullPath(DirPath + "vecto_multistage_consolidated_multiple_stages.xml");
			
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

			var multiStageViewModel =
				newMultistageJobViewModel.MultiStageJobViewModel as
					MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);

			var vehicleInputData = Path.GetFullPath(DirPath + "vecto_vehicle-stage_input_full-sample.xml");
			Assert.IsTrue(File.Exists(vehicleInputData));

			var vehicleInputDataFiledialogMock = new Mock<IDialogHelper>();
			vehicleInputDataFiledialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(vehicleInputData);
			vehicleInputDataFiledialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(vehicleInputData);
			_kernel.Rebind<IDialogHelper>().ToConstant(vehicleInputDataFiledialogMock.Object);

			multiStageViewModel.LoadVehicleDataCommand.Execute(null);

			var vehicle = multiStageViewModel.VehicleInputData as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicle);

			Assert.AreEqual("VEH-1234567890", vehicle.Identifier);
			Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Today, vehicle.Date);
			Assert.AreEqual("Sample Bus Model", vehicle.Model);
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(500, vehicle.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(3500, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(false, vehicle.AirdragModifiedMultistage);
			Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);//NgTankSystem
			Assert.AreEqual(RegistrationClass.II_III, vehicle.RegisteredClass);//ClassBus
			Assert.AreEqual(0, vehicle.NumberOfPassengersLowerDeck);
			Assert.AreEqual(10, vehicle.NumberOfPassengersUpperDeck);
			Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
			Assert.AreEqual(false, vehicle.LowEntry);
			Assert.AreEqual(2.5, vehicle.Height.Value());//HeightIntegratedBody
			Assert.AreEqual(9.5, vehicle.Length.Value());
			Assert.AreEqual(2.5, vehicle.Width.Value());
			Assert.AreEqual(2500, vehicle.HeightInMm.Value);
			Assert.AreEqual(9500, vehicle.LengthInMm.Value);
			Assert.AreEqual(2500, vehicle.WidthInMm.Value);

			Assert.AreEqual(2, vehicle.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Electrically, vehicle.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);


			var airdrag = vehicle.MultistageAirdragViewModel;
			Assert.NotNull(airdrag.AirDragViewModel.XMLSource);


			TestADASInput(vehicle);
			TestComponents(vehicle.Components);
			TestAirdragComponent(vehicle.Components.AirdragInputData);
			TestAuxiliariesComponent(vehicle.BusAuxiliaries);




		}

		private void TestADASInput(IVehicleDeclarationInputData vehicle)
		{
			Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(true, vehicle.ADAS.ATEcoRollReleaseLockupClutch);
		}

		private void TestComponents(IVehicleComponentsDeclaration components)
		{
			TestAirdragComponent(components.AirdragInputData);
			TestAuxiliariesComponent(components.BusAuxiliaries);
		}

		private void TestAirdragComponent(IAirdragDeclarationInputData airdrag)
		{
			Assert.AreEqual("Generic Manufacturer", airdrag.Manufacturer);
			Assert.AreEqual("Generic Model", airdrag.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", airdrag.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-03-24T15:00:00Z").ToUniversalTime(), airdrag.Date);
			Assert.AreEqual("Vecto AirDrag x.y", airdrag.AppVersion);
			Assert.AreEqual(6.34, airdrag.AirDragArea.Value());
		}

		private void TestAuxiliariesComponent(IBusAuxiliariesDeclarationData busAux)
		{
			TestLedLightsComponent(busAux.ElectricConsumers);
			TestHVACComponent(busAux.HVACAux);
		}

		private void TestLedLightsComponent(IElectricConsumersDeclarationData electricConsumer)
		{
			Assert.AreEqual(false, electricConsumer.InteriorLightsLED);
			Assert.AreEqual(true, electricConsumer.DayrunninglightsLED);
			Assert.AreEqual(true, electricConsumer.PositionlightsLED);
			Assert.AreEqual(true, electricConsumer.BrakelightsLED);
			Assert.AreEqual(false, electricConsumer.HeadlightsLED);
		}

		private void TestHVACComponent(IHVACBusAuxiliariesDeclarationData hvacAux)
		{
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, hvacAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeDriverCompartment);
			Assert.AreEqual(HeatPumpMode.heating, hvacAux.HeatPumpModeDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpTypePassengerCompartment);
			Assert.AreEqual(HeatPumpMode.cooling, hvacAux.HeatPumpModePassengerCompartment);
			Assert.AreEqual(50, hvacAux.AuxHeaterPower.Value());
			Assert.AreEqual(false, hvacAux.DoubleGlazing);
			Assert.AreEqual(true, hvacAux.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, hvacAux.SeparateAirDistributionDucts);
			Assert.AreEqual(true, hvacAux.WaterElectricHeater);
			Assert.AreEqual(false, hvacAux.AirElectricHeater);
			Assert.AreEqual(false, hvacAux.OtherHeatingTechnology);
		}






		#region Helper

		#endregion
	}

}