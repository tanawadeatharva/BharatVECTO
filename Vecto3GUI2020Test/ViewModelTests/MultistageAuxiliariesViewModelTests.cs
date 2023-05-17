using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class MultistageAuxiliariesViewModelTests : ViewModelTestBase
	{
		private IMultiStageViewModelFactory _viewModelFactory;

		[SetUp]
		public void Setup()
		{
			_kernel = TestHelper.GetKernel();
			_viewModelFactory = _kernel.Get<IMultiStageViewModelFactory>();
		}
		[TestCase(CompletedBusArchitecture.Conventional)]
		[TestCase(CompletedBusArchitecture.HEV)]
        public void TestAllowedValuesHeatPumpTypeDriver(CompletedBusArchitecture arch)
		{
			var auxVm = _viewModelFactory.GetAuxiliariesViewModel(
				arch) as MultistageAuxiliariesViewModel; 

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration6;
			Assert.AreEqual(auxVm.HeatPumpTypeDriverAllowedValues.Count, 1);
			Assert.Contains(HeatPumpType.not_applicable, auxVm.HeatPumpTypeDriverAllowedValues);

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration3;
			Assert.IsFalse(auxVm.HeatPumpTypeDriverAllowedValues.Contains(HeatPumpType.not_applicable));

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration10;
			Assert.AreEqual(auxVm.HeatPumpTypeDriverAllowedValues.Count, 1);
			Assert.Contains(HeatPumpType.not_applicable, auxVm.HeatPumpTypeDriverAllowedValues);

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration2;
			Assert.IsFalse(auxVm.HeatPumpTypeDriverAllowedValues.Contains(HeatPumpType.not_applicable));
		}


		[TestCase(CompletedBusArchitecture.Conventional)]
		[TestCase(CompletedBusArchitecture.HEV)]
        public void TestEnumParameters(CompletedBusArchitecture arch)
        {
			var auxVm = _viewModelFactory.GetAuxiliariesViewModel(
				arch) as MultistageAuxiliariesViewModel;
			auxVm.HeatPumpTypeCoolingDriverCompartment = HeatPumpType.none;
			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);
			auxVm.HeatPumpTypeCoolingDriverCompartment = HeatPumpType.R_744;
			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

			auxVm.HeatPumpGroupEditingEnabled = false;
			auxVm.ParameterViewModels[nameof(auxVm.HeatPumpTypeCoolingDriverCompartment)].CurrentContent =
				HeatPumpType.R_744;
			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

			auxVm.ParameterViewModels[nameof(auxVm.HeatPumpTypeCoolingDriverCompartment)].CurrentContent =
				HeatPumpType.none;


			Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);
		}
	}
}
