using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using Castle.Core.Internal;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
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
			//Load Primary Vehicle VIF
			var newMultiStageJob = loadFile(primary_vehicle_only);
			var vehicle = newMultiStageJob.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicle);
			vehicle.Manufacturer = "test1";
			vehicle.ManufacturerAddress = "testAddress2";
			vehicle.VIN = "VIN123456789";



			var manufacturingStage =
				newMultiStageJob.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;

			Assert.NotNull(manufacturingStage);


			var fileName = primary_vehicle_only.Replace(".xml", "") + "_output.xml";
			deleteFile(fileName);
			setMockDialogHelper(null, fileToSave: fileName);


			manufacturingStage.SaveInputDataAsCommand.Execute(null);
			Assert.True(checkFileNameExists(fileName));
		}

		[Test]
		public void loadPrimaryAndSave()
		{
			//load file
			var newMultiStageJob = loadFile(primary_vehicle_only);
			var vehicle = newMultiStageJob.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;



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
			_kernel.Rebind<IDialogHelper>().ToConstant(setMockDialogHelper(fileToSave: saveFileName).Object);
			manstage.SaveInputDataAsCommand.Execute(null);

			Assert.False(checkFileNameExists(saveFileName));

		}

		[Test]
		public void reloadInputFile()
		{
			var newMultistageJobViewModel = loadFile(consolidated_multiple_stages_airdrag) as NewMultiStageJobViewModel;

			var vehicle = newMultistageJobViewModel.MultiStageJobViewModel.VehicleInputData as
				DeclarationInterimStageBusVehicleViewModel_v2_8;

			Assert.NotNull(vehicle);


			Assert.True(vehicle.AirdragModifiedMultistageEditingEnabled);

			var mockDialog = setMockDialogHelper(consolidated_multiple_stages_hev);
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
		public void loadAndSaveFullInputDataSample()
		{
			var vm = loadFile(primary_vehicle_only);
			var multiStageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;


			var fileToSave = "loadAndSaveFullInputDataTest.xml";
			var mockDialogHelper = setMockDialogHelper(stageInputFullSample, fileToSave);
			multiStageJobViewModel.LoadVehicleDataCommand.Execute(null);

			var manufacturingStageViewModel =
				multiStageJobViewModel.ManufacturingStageViewModel as ManufacturingStageViewModel_v0_1;

			var vehicleViewModel =
				manufacturingStageViewModel.VehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;

			vehicleViewModel.AirdragModifiedMultistage = true;
			Assert.AreEqual(ConsumerTechnology.Electrically, vehicleViewModel.DoorDriveTechnology);
			Assert.AreEqual(ConsumerTechnology.Electrically, vm.MultiStageJobViewModel.VehicleInputData.DoorDriveTechnology);

			Assert.IsNotNull(multiStageJobViewModel.VehicleInputData.ManufacturerAddress);

			var fileExists = checkFilePathExists(mockDialogHelper.Object.OpenXMLFileDialog());
			Assert.IsTrue(fileExists);
			File.Delete(mockDialogHelper.Object.SaveToXMLDialog());


		}

		[Test]
		public void loadVehicleInputDataOnly()
		{
			string inputPath = Path.Combine(DirPath, stageInputFullSample);
			var inputDataReader = _kernel.Get<IXMLInputDataReader>();
			var inputData = (IDeclarationInputDataProvider)inputDataReader.Create(inputPath);
			var vehicleInputData = inputData.JobInputData.Vehicle;
			//_manufacturingStageViewModel.SetInputData(vehicleInputData);


			Assert.AreEqual(ConsumerTechnology.Electrically, vehicleInputData.DoorDriveTechnology);
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
			Assert.IsTrue(vehicleViewModel.Manufacturer.IsNullOrEmpty());
			Assert.IsTrue(vehicleViewModel.ManufacturerAddress.IsNullOrEmpty());
			Assert.IsTrue(vehicleViewModel.VIN.IsNullOrEmpty());
			Assert.IsNull(vehicleViewModel.Model);

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
			Assert.IsNull(vehicleViewModel_v2_8.ConsolidatedAirdragModifiedEnum);
			Assert.IsTrue(vehicleViewModel_v2_8.AirdragModifiedEnum == AIRDRAGMODIFIED.UNKNOWN || vehicleViewModel_v2_8.AirdragModifiedEnum == null);
			


			Assert.AreEqual(vehicleViewModel_v2_8.AirdragModifiedMultistageEditingEnabled, false);

			Assert.Null(vehicleViewModel_v2_8.BusAuxiliaries);


			var vifInputData = vm.MultiStageJobViewModel as IMultistageVIFInputData;

			Assert.Null(vifInputData.VehicleInputData.Components);
		}


	







		[Test]
		public void loadVehicleDataTest()
		{
			
			
			TestContext.WriteLine($"Loading {consolidated_multiple_stages}");

			var newMultistageJobViewModel = loadFile(consolidated_multiple_stages);

			//var dialogMockConsolidatedMultipleStage = new Mock<IDialogHelper>();
			//dialogMockConsolidatedMultipleStage.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(multiplestages);
			//dialogMockConsolidatedMultipleStage.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(multiplestages);
			//_kernel.Rebind<IDialogHelper>().ToConstant(dialogMockConsolidatedMultipleStage.Object);


			//var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();
			//newMultistageJobViewModel.AddVifFile.Execute(null);
			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);

			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);

			var multiStageViewModel =
				newMultistageJobViewModel.MultiStageJobViewModel as
					MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);

			var manStageViewModel = multiStageViewModel.ManufacturingStageViewModel as ManufacturingStageViewModel_v0_1;

			var vehicleInputData = "vecto_vehicle-stage_input_full-sample.xml";
			var mockDialogHelper = setMockDialogHelper(fileToLoad: vehicleInputData, fileToSave: null);
			var vehicleInputDataFilePath = mockDialogHelper.Object.OpenXMLFileDialog();
			TestContext.WriteLine($"Loading {vehicleInputDataFilePath}");
			Assert.IsTrue(File.Exists(vehicleInputDataFilePath));

			

			//var vehicleInputDataFiledialogMock = new Mock<IDialogHelper>();
			//vehicleInputDataFiledialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(vehicleInputData);
			//vehicleInputDataFiledialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(vehicleInputData);
			//_kernel.Rebind<IDialogHelper>().ToConstant(vehicleInputDataFiledialogMock.Object);

			multiStageViewModel.LoadVehicleDataCommand.Execute(null);

			var vehicleViewModel = manStageViewModel.VehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicleViewModel);
			//Assert.IsFalse(getMockDialogHelper().Verify);

			Assert.AreEqual("VEH-1234567890", vehicleViewModel.Identifier);
			Assert.AreEqual("Some Manufacturer", vehicleViewModel.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address", vehicleViewModel.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", vehicleViewModel.VIN);
			Assert.AreEqual(DateTime.Today, vehicleViewModel.Date);
			Assert.AreEqual("Sample Bus Model", vehicleViewModel.Model);
			Assert.AreEqual(LegislativeClass.M3, vehicleViewModel.LegislativeClass);
			Assert.AreEqual(500, vehicleViewModel.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(3500, vehicleViewModel.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(false, vehicleViewModel.AirdragModifiedMultistage);
			Assert.AreEqual(AIRDRAGMODIFIED.FALSE, vehicleViewModel.AirdragModifiedEnum);
			Assert.AreEqual(AIRDRAGMODIFIED.FALSE, vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);
			Assert.AreEqual(TankSystem.Compressed, vehicleViewModel.TankSystem);//NgTankSystem
			Assert.AreEqual(RegistrationClass.II_III, vehicleViewModel.RegisteredClass);//ClassBus
			Assert.AreEqual(1, vehicleViewModel.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(10, vehicleViewModel.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(11, vehicleViewModel.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(2, vehicleViewModel.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CB, vehicleViewModel.VehicleCode);
			Assert.AreEqual(false, vehicleViewModel.LowEntry);
			Assert.AreEqual(2.5, vehicleViewModel.Height.Value());//HeightIntegratedBody
			Assert.AreEqual(9.5, vehicleViewModel.Length.Value());
			Assert.AreEqual(2.5, vehicleViewModel.Width.Value());

			Assert.AreEqual(2500, (vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.HeightInMm)].CurrentContent as ConvertedSI).Value);
			Assert.AreEqual(9500, (vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.LengthInMm)].CurrentContent as ConvertedSI).Value);
			Assert.AreEqual(2500, (vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.WidthInMm)].CurrentContent as ConvertedSI).Value);
			

			Assert.AreEqual(2, vehicleViewModel.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Electrically, vehicleViewModel.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicleViewModel.VehicleDeclarationType);


			Assert.AreEqual(newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle.DoorDriveTechnology, vehicleViewModel.DoorDriveTechnology);


			var airdrag = vehicleViewModel.MultistageAirdragViewModel;
			Assert.NotNull(airdrag.AirDragViewModel.XMLSource);


			TestADASInput(vehicleViewModel);
			TestComponents(vehicleViewModel.Components);
			TestAirdragComponent(vehicleViewModel.Components.AirdragInputData);
			TestAuxiliariesComponent(vehicleViewModel.BusAuxiliaries);




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

			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpPassengerCompartments[0].Item1);
			Assert.AreEqual(HeatPumpMode.cooling, hvacAux.HeatPumpPassengerCompartments[0].Item2);

			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvacAux.HeatPumpPassengerCompartments[1].Item1);
			Assert.AreEqual(HeatPumpMode.heating, hvacAux.HeatPumpPassengerCompartments[1].Item2);

			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpPassengerCompartments[2].Item1);
			Assert.AreEqual(HeatPumpMode.cooling, hvacAux.HeatPumpPassengerCompartments[2].Item2);

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