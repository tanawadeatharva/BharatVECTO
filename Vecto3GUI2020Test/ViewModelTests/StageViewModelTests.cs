using System.Runtime.InteropServices;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore.Configuration;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class StageViewModelTests : ViewModelTestBase
	{


		[TestCase(true, TestName="Exempted")]
		[TestCase(false, TestName="NotExempted")]
		public void updateFilePathsWhenSavedAs_non_exempted(bool exempted)
		{
			IMultiStageViewModelFactory vmFactory = _kernel.Get<IMultiStageViewModelFactory>();

			var StageInput = vmFactory.GetStageInputViewModel(exempted) as StageInputViewModel;
			var vehicleVm = StageInput.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;
			vehicleVm.Manufacturer = "adsf";
			vehicleVm.ManufacturerAddress = "asdf 123";
			vehicleVm.VIN = "1234567890";

			var fileName = TestHelper.GetMethodName() + ".xml";
			StageInput.SaveInputDataExecute(GetFullPath(fileName));
			Assert.True(checkFileNameExists(fileName));

			Assert.AreEqual(GetFullPath(fileName), StageInput.InputDataFilePath);
			
			//Check if title is updated
			StringAssert.Contains(fileName, StageInput.Title);

			//Check datasource
			Assert.NotNull(StageInput.DataSource);


		}
	}
}