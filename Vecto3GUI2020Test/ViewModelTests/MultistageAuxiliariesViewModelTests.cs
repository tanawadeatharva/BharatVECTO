using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
    [TestFixture]
    public class MultistageAuxiliariesViewModelTests : ViewModelTestBase
    {

		//[Test]
		//public void TestAllowedValuesHeatPumpMode()
		//{
		//	var vm = loadFile(consolidated_multiple_stages_airdrag);


		//	var vehicle = vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
		//		DeclarationInterimStageBusVehicleViewModel_v2_8;

		//	var auxVm = vehicle.MultistageAuxiliariesViewModel as MultistageAuxiliariesViewModel;


		//	auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.none;
		//	Assert.IsTrue(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
		//	Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.cooling));
		//	Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.heating));
		//	Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.heating_and_cooling));


		//	auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.R_744;
		//	Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.N_A));



		//}

		[Test]
		public void TestAllowedValuesHeatPumpModePassenger()
		{

			var auxVm = new MultistageAuxiliariesViewModel(null);
			auxVm.HeatPumpTypePassengerCompartment = HeatPumpType.none;
			Assert.IsTrue(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
			Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.cooling));
			Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.heating));
			Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.heating_and_cooling));


			auxVm.HeatPumpTypePassengerCompartment = HeatPumpType.R_744;
			Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
		}

		[Test]
		public void TestAllowedValuesHeatPumpModeDriver()
        {
			var auxVm = new MultistageAuxiliariesViewModel(null);
			auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.none;
            Assert.IsTrue(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
            Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.cooling));
            Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.heating));
            Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.heating_and_cooling));


            auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.R_744;
            Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
        }






	}
}
