using System.IO;
using System.Runtime.Intrinsics;
using Ninject;
using Ninject.Injection;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.XML.XMLInput;

[TestFixture]
public class AirdragTest
{
	private IKernel _kernel;
	private IXMLInputDataReader _inputDataReader;
	private IXMLComponentInputReader _componentReader;
	private IDeclarationInjectFactory _injectFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = TestHelper.GetKernel();
		_inputDataReader = _kernel.Get<IXMLInputDataReader>();
		_componentReader = _kernel.Get<IXMLComponentInputReader>();
		_injectFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

	[TestCase(TestData.airdragLoadTestFile, "1.0")]
	[TestCase(TestData.airdragLoadTestFilev2, "2.0")]
	public void LoadAirdragFile(string fileName, string expVersion)
	{
		var filePath = Path.GetFullPath(fileName);
		AssertHelper.FileExists(filePath);


		IAirdragDeclarationInputData airdragData = _componentReader.CreateAirdrag(fileName);
		Assert.NotNull(airdragData);
		Assert.NotNull(airdragData.DataSource);
		Assert.AreEqual("AirDragDataDeclarationType",airdragData.DataSource.Type);
		Assert.NotNull(airdragData.DigestValue.Reference);
		Assert.AreEqual(expVersion, airdragData.DataSource.SourceVersion);
	}
}