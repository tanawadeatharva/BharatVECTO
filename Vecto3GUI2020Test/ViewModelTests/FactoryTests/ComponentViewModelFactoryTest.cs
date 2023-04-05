using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Ninject.Factories;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.MockInput;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.ViewModelTests.FactoryTests;

[TestFixture]
public class ComponentViewModelFactoryTest
{
	private IComponentViewModelFactory _componentViewModelFactory;
	private IKernel _kernel;
	private MockDialogHelper _mockDialogHelper;

	[SetUp]
	public void Setup()
	{
		_kernel = TestHelper.GetKernel(out _mockDialogHelper, out _);
		_componentViewModelFactory = _kernel.Get<IComponentViewModelFactory>();
	}


	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10)]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20)]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24)]
	public void CreateAirdragViewModel(string version)
	{
		XNamespace ns = version;
		var airdragData = MockComponent.GetAirdragComponentData(ns);

		Assert.NotNull(airdragData);
		Assert.AreEqual(version, airdragData.DataSource.TypeVersion);

		var componentViewModel = _componentViewModelFactory.CreateComponentViewModel(airdragData);
		_mockDialogHelper.AssertNoErrorDialogs();
		Assert.NotNull(componentViewModel);
		
		var airdragVm = componentViewModel as IAirDragViewModel;
		Assert.NotNull(airdragVm);

		switch (airdragVm) {
			//since the vms are derived from each other order matters
			case AirDragViewModel_v2_4:
				Assert.AreEqual(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, version);
				break;
			case AirDragViewModel_v2_0:
				Assert.AreEqual(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20, version);
				break;
            case AirDragViewModel_v1_0:
				Assert.AreEqual(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10, version);
				break;
			default:
				Assert.Fail($"Unexpected type {airdragVm.GetType()}");
				break;
		}
	}

	[Test]
	public void CreateMultistepAuxiliaryViewModel([Values] CompletedBusArchitecture arch)
	{
		if (arch == CompletedBusArchitecture.Exempted) {
			return;
		}
		var multistepComponentFactory = _kernel.Get<IMultistepComponentViewModelFactory>();

		var auxVm = multistepComponentFactory.CreateNewMultistepBusAuxViewModel(arch);
	}


}