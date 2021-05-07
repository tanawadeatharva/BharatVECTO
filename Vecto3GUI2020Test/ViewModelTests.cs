using System;
using System.IO;
using System.Runtime.CompilerServices;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test
{
    [TestFixture]
    public class ViewModelTests
    {
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		const string DirPath = @"Testdata\";

		private const string consolidated_multiple_stages = "vecto_multistage_consolidated_multiple_stages.xml";

		private const string consolidated_multiple_stages_airdrag =
			"vecto_multistage_consolidated_multiple_stages_airdrag.xml";

		private const string consolidated_one_stage = "vecto_multistage_consolidated_one_stage.xml";
		private const string primary_vehicle_only = "vecto_multistage_primary_vehicle_only.xml";

		[SetUp]
        public void OneTimeSetUp()
        {
			_kernel = new StandardKernel(
				new VectoNinjectModule(),
				new JobEditModule(),
				new ComponentModule(),
				new DocumentModule(),
				new XMLWriterFactoryModule(),
				new FactoryModule(),
				new MultistageModule()
			);
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			
		}

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
			vehicle.Manufacturer = "test1";
			vehicle.ManufacturerAddress = "testAddress2";
			vehicle.VIN = "VIN123456789";
			//Remove

			var manufacturingStage =
				newMultiStageJob.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;

			var fileName = primary_vehicle_only.Replace(".xml", "") + "_output.xml";

			_kernel.Rebind<IDialogHelper>().ToConstant(getMockDialogHelper(fileToSave:fileName).Object);
			
			manufacturingStage.SaveInputDataAsCommand.Execute(null);


			checkFileExists(fileName);



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
			Assert.AreEqual(1, vm.MultiStageJobViewModel.ManufacturingStageViewModel.StageCount);

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

			Assert.AreEqual(vehicleViewModel_v2_8.AirdragModifiedMultistageEditingEnabled, false);


			Assert.Null(vehicleViewModel_v2_8.BusAuxiliaries);

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



		}

		[TearDown]
		public void TearDown()
		{
			_kernel.Dispose();
			_kernel = null;
		}




		#region Helper
		public bool checkFileExists(string fileName)
		{
			var filePath = Path.GetFullPath(DirPath + fileName);
			var exists = File.Exists(filePath);
			if (!exists)
			{
				Console.WriteLine(filePath + " not existing");
			}

			Assert.IsTrue(exists);
			return exists;
		}

		public NewMultiStageJobViewModel loadFile(string fileName)
		{
			string filePath = "";
			filePath = Path.GetFullPath(DirPath + fileName);

			var dialogMock = new Mock<IDialogHelper>();
			dialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(filePath);
			dialogMock.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(filePath);

			var newMultistageJobViewModel = new NewMultiStageJobViewModel(dialogMock.Object, xmlInputReader,
				_kernel.Get<IMultiStageViewModelFactory>());
			newMultistageJobViewModel.AddVifFile.Execute(null);

			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as DeclarationInterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(manstageVehicleViewModel);

			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);




			var airdragViewModel = (manstageVehicleViewModel as DeclarationInterimStageBusVehicleViewModel_v2_8)?.MultistageAirdragViewModel;
			Assert.NotNull(airdragViewModel);

			Assert.AreEqual(filePath, newMultistageJobViewModel.VifPath);



			return newMultistageJobViewModel;
		}

		private Mock<IDialogHelper> getMockDialogHelper(string fileToLoad = null, string fileToSave = null)
		{
			Mock<IDialogHelper> mockDialogHelper = null;
			if (fileToLoad != null) {
				var filePath = Path.GetFullPath(DirPath + fileToLoad);

				Assert.NotNull(filePath);
				mockDialogHelper = new Mock<IDialogHelper>();
				mockDialogHelper.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(filePath);
				mockDialogHelper.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(filePath);
			}

			if (fileToSave != null) {
				var filePath = Path.GetFullPath(DirPath + fileToSave);
				mockDialogHelper = mockDialogHelper ?? new Mock<IDialogHelper>();
				mockDialogHelper.Setup(dialogHelper =>
					dialogHelper.SaveToXMLDialog(It.IsAny<string>())).Returns(filePath);
				mockDialogHelper.Setup(dialogHelper =>
					dialogHelper.SaveToXMLDialog(null)).Returns(filePath);
			}


			return mockDialogHelper;
		}
#endregion
	}

}