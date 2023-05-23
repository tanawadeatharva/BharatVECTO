using System.IO;
using System.Runtime.InteropServices;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class StepViewModelTests
	{
		private IKernel _kernel;
		private MockWindowHelper _windowHelper;
		private MockDialogHelper _dialogHelper;
		private IMultiStageViewModelFactory _viewModelFactory;

		[SetUp]
		public void Setup()
		{
			_kernel = TestHelper.GetKernel(out _dialogHelper, out _windowHelper);
			_viewModelFactory = _kernel.Get<IMultiStageViewModelFactory>();
		}

		[TestCase(true, TestName="Exempted")]
		[TestCase(false, TestName="NotExempted")]
		public void updateFilePathsWhenSaved(bool exempted)
		{
			IMultiStageViewModelFactory vmFactory = _kernel.Get<IMultiStageViewModelFactory>();

			var StageInput = vmFactory.GetCreateNewStepInputViewModel(exempted) as StageInputViewModel;
			var vehicleVm = StageInput.VehicleViewModel as InterimStageBusVehicleViewModel;
			vehicleVm.Manufacturer = "adsf";
			vehicleVm.ManufacturerAddress = "asdf 123";
			vehicleVm.VIN = "1234567890";

			var fileName = TestHelper.GetMethodName() + ".xml";
			var fullPath = Path.GetFullPath(fileName);
			StageInput.SaveInputDataExecute(fullPath);
			AssertHelper.FileExists(fullPath);
			Assert.AreEqual(fullPath, StageInput.VehicleInputDataFilePath);

			//Check if title is updated
			StringAssert.Contains(fileName, StageInput.Title);

			//Check datasource
			Assert.NotNull(StageInput.DataSource);

			File.Delete(fullPath);
		}

		[Test]
		public void SaveFullStageInput()
		{
			IMultiStageViewModelFactory vmFactory = _kernel.Get<IMultiStageViewModelFactory>();

			var StageInput = vmFactory.GetCreateNewStepInputViewModel(false) as StageInputViewModel;
			var vehicleVm = StageInput.VehicleViewModel as InterimStageBusVehicleViewModel;
			vehicleVm.Manufacturer = "adsf";
			vehicleVm.ManufacturerAddress = "asdf 123";
			vehicleVm.VIN = "1234567890";
			vehicleVm.Model = "Model";
			vehicleVm.LegislativeClass = LegislativeClass.M3;
			vehicleVm.CurbMassChassis = Kilogram.Create(20000);
			vehicleVm.GrossVehicleMassRating = Kilogram.Create(20000);
			vehicleVm.RegisteredClass = RegistrationClass.I_II;
			vehicleVm.VehicleCode = VehicleCode.CC;
			vehicleVm.LowEntry = true;
			vehicleVm.Height = Meter.Create(2.6);
			vehicleVm.NumberPassengerSeatsUpperDeck = 2;
			vehicleVm.NumberPassengersStandingLowerDeck = 13;
			vehicleVm.NumberPassengerSeatsLowerDeck = 10;
			vehicleVm.NumberPassengersStandingUpperDeck = 12;

			//SETADAS
			vehicleVm.EngineStopStartNullable = true;
			vehicleVm.EcoRollTypeNullable = EcoRollType.WithEngineStop;
			vehicleVm.PredictiveCruiseControlNullable = PredictiveCruiseControlType.Option_1_2_3;
			vehicleVm.ATEcoRollReleaseLockupClutch = false;



			//SETAUxiliaries
			var auxVm = vehicleVm.MultistageAuxiliariesViewModel as MultistageAuxiliariesViewModel;

			
			auxVm.InteriorLightsLED = true;
			auxVm.DayrunninglightsLED = false;
			auxVm.PositionlightsLED = false;
			auxVm.BrakelightsLED = true;
			auxVm.HeadlightsLED = false;
			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration2;
			auxVm.HeatPumpTypeCoolingDriverCompartment = HeatPumpType.non_R_744_3_stage;
			auxVm.HeatPumpTypeCoolingPassengerCompartment = HeatPumpType.non_R_744_4_stage;
			auxVm.HeatPumpTypeHeatingDriverCompartment = HeatPumpType.non_R_744_2_stage;
			auxVm.HeatPumpTypeHeatingPassengerCompartment = HeatPumpType.non_R_744_continuous;
			auxVm.AuxHeaterPower = SIBase<Watt>.Create(50);
			auxVm.DoubleGlazing = true;
			auxVm.AdjustableAuxiliaryHeater = false;
			auxVm.SeparateAirDistributionDucts = false;


			var fileName = TestHelper.GetMethodName() + ".xml";
			var fullPath = Path.GetFullPath(fileName);
			StageInput.SaveInputDataExecute(fullPath);
			
            AssertHelper.FileExists(fileName);
			Assert.AreEqual(fullPath, StageInput.VehicleInputDataFilePath);

			//Check if title is updated
			StringAssert.Contains(fileName, StageInput.Title);

			//Check datasource
			Assert.NotNull(StageInput.DataSource);

			File.Delete(fullPath);

			_dialogHelper.AssertNoErrorDialogs();
			
		}



		[Test]
		public void CreateStepInput()
		{
			var stepInput = _viewModelFactory.GetCreateNewStepInputViewModel(false) as StageInputViewModel;
			Assert.NotNull(stepInput.VehicleViewModel);
			Assert.NotNull(stepInput.VehicleViewModel.MultistageAirdragViewModel);
			Assert.NotNull(stepInput.VehicleViewModel.MultistageAuxiliariesViewModel);
			Assert.NotNull(stepInput.Architecture == CompletedBusArchitecture.Conventional);
		}

		[Test]
		public void CreateStepInputForAllArchitectures([Values] CompletedBusArchitecture arch)
		{

			var stepInput = _viewModelFactory.GetCreateNewStepInputViewModel(false) as StageInputViewModel;
			Assert.NotNull(stepInput.VehicleViewModel);

			var oldVm = stepInput.VehicleViewModel;
			if (arch == stepInput.Architecture) {
				Assert.Pass("Nothing to see here ...");
            }
			stepInput.Architecture = arch;
			Assert.NotNull(stepInput.VehicleViewModel);
			Assert.AreNotSame(oldVm, stepInput.VehicleViewModel);
			Assert.That(stepInput.Title.Contains(arch.ToString()));
		}

		[Test]
		public void SwitchArchitectures([Values] CompletedBusArchitecture from,
			[Values] CompletedBusArchitecture to)
		{

			if (from == to) {
				Assert.Pass("Nothing to see here ...");
			}
			var stepInput = _viewModelFactory.GetCreateNewStepInputViewModel(false) as StageInputViewModel;
			Assert.NotNull(stepInput.VehicleViewModel);

			
			stepInput.Architecture = from;
			var oldVm = stepInput.VehicleViewModel;
			Assert.NotNull(stepInput.VehicleViewModel);

			stepInput.Architecture = to;
			var newVm = stepInput.VehicleViewModel;
			Assert.NotNull(newVm);
			Assert.AreNotSame(oldVm, newVm);
			Assert.That(stepInput.Title.Contains(to.ToString()));
        }


		[Test]
		public void CreateAndSaveMinimalInput([Values] CompletedBusArchitecture arch, [Values] bool addAuxInput, [Values] bool loadAirdrag, [Values(TestData.airdragLoadTestFile, TestData.airdragLoadTestFilev2)] string airdragFile)
		{
			var stepInput = _viewModelFactory.GetCreateNewStepInputViewModel(false) as StageInputViewModel;
			Assert.NotNull(stepInput.VehicleViewModel);

			var oldVm = stepInput.VehicleViewModel;
			stepInput.Architecture = arch;
			Assert.NotNull(stepInput.VehicleViewModel);
			Assert.That(stepInput.Title.Contains(arch.ToString()));


			var vehicleVm = stepInput.VehicleViewModel as InterimStageBusVehicleViewModel;
			vehicleVm.Manufacturer	= $"A {arch}_manufacturer";
			vehicleVm.VIN = "1234567890";
			vehicleVm.Manufacturer = $"{arch}-avenue 42";


			if (addAuxInput && arch != CompletedBusArchitecture.Exempted) {
				var auxVm = vehicleVm.MultistageAuxiliariesViewModel as MultistageAuxiliariesViewModel;
				auxVm.BrakelightsLED = true;
			}

			if (loadAirdrag && arch != CompletedBusArchitecture.Exempted) {
				var airdragVm = vehicleVm.MultistageAirdragViewModel as MultistageAirdragViewModel;
				airdragVm.LoadAirdragFile(Path.GetFullPath(airdragFile));

			}
			
			stepInput.SaveInputDataExecute($"{arch}_step_input.xml");
			_dialogHelper.AssertNoErrorDialogs();
		}


	}
}