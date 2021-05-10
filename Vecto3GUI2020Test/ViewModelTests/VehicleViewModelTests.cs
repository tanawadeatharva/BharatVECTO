using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

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

		#endregion

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
