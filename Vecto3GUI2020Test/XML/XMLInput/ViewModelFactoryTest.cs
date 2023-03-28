using System.Xml.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Ninject;
using NUnit.Framework;
using OpenQA.Selenium.Appium.PageObjects;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using Vecto3GUI2020Test.MockInput;

namespace Vecto3GUI2020Test.XML.XMLInput;

[TestFixture]
public class ViewModelFactoryTest
{
	private StandardKernel _kernel;
	private IMultiStageViewModelFactory _vmFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new Vecto3GUI2020Module());
		_vmFactory = _kernel.Get<IMultiStageViewModelFactory>();
    }


	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20)]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10)]
	public void CreateJobViewModelFromMock(string ns)
	{
		XNamespace nameSpace = ns;
		TestContext.WriteLine(nameSpace.NamespaceName + nameSpace.GetVersionFromNamespaceUri());

		var input = MockDocument.GetDeclarationInputDataProvider(out var mock, VectoSimulationJobType.ConventionalVehicle, nameSpace);

		var vm = _vmFactory.CreateDocumentViewModel(input);

        Assert.NotNull(vm);
		

	}


	

}