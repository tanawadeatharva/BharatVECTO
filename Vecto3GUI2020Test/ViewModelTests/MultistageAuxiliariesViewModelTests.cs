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

	

		[Test]
		public void TestAllowedValuesHeatPumpModePassenger()
		{

			//var auxVm = new MultistageAuxiliariesViewModel(null);
			//auxVm.HeatPumpTypePassengerCompartment = HeatPumpType.none;
			//Assert.IsTrue(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
			//Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.cooling));
			//Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.heating));
			//Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.heating_and_cooling));


			//auxVm.HeatPumpTypePassengerCompartment = HeatPumpType.R_744;
			//Assert.IsFalse(auxVm.HeatPumpModePassengerCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
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
            Assert.IsFalse(auxVm.HeatPumpModeDriverCompartmentAllowedValues.Contains(HeatPumpMode.N_A));
        }


		[Test]
		public void TestEnumParameters()
		{
			var auxVm = new MultistageAuxiliariesViewModel(null);
			auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.none;
			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);
			auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.R_744;
			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

			auxVm.HeatPumpGroupEditingEnabled = false;
			auxVm.ParameterViewModels[nameof(auxVm.HeatPumpModeDriverCompartment)].CurrentContent =
				HeatPumpType.R_744;
			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

			auxVm.ParameterViewModels[nameof(auxVm.HeatPumpModeDriverCompartment)].CurrentContent =
				HeatPumpType.none;


			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

		}

		[Test]
		public void TestEnum()
		{
			var auxVm = new MultistageAuxiliariesViewModel(null);
			auxVm.HeatPumpTypeDriverCompartment = HeatPumpType.none;
			auxVm.HeatPumpModeDriverCompartment = HeatPumpMode.N_A;
			auxVm.HeatPumpGroupEditingEnabled = false;
			auxVm.HeatPumpGroupEditingEnabled = true;
			Assert.AreEqual(HeatPumpMode.N_A, auxVm.HeatPumpModeDriverCompartment);
			Assert.AreEqual(HeatPumpMode.N_A,
				auxVm.ParameterViewModels[nameof(auxVm.HeatPumpModeDriverCompartment)].CurrentContent);

		}






	}
}
