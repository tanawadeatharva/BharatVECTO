using System;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Resources.XML;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.Util.XML.Components;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.XML.XMLWritingTest;

[TestFixture]
public class XMLComponentsWriterFactoryTest
{
	private MockDialogHelper _mockDialogHelper;
	private IKernel _kernel;
	private IXMLWriterFactory _xmlWriterFactory;


	[SetUp]
	public void Setup()
	{
		_kernel = TestHelper.GetKernel(out _mockDialogHelper, out _);
		_xmlWriterFactory = _kernel.Get<IXMLWriterFactory>();
	}


	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLTypes.Components_Conventional_CompletedBusType, typeof(XMLCompletedBusComponentWriter_Conventional))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLTypes.Components_xEV_CompletedBusType, typeof(XMLCompletedBusComponentsWriter_xEV))]
    public void CreateComponentsWriter(string version, string type, Type expectedType)
	{
		XNamespace ns = version;


		var writer = _xmlWriterFactory.CreateComponentsWriterWithVersion(ns, type);
		Assert.AreEqual(expectedType, writer.GetType());


	}

	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20, XMLTypes.AirDragDataDeclarationType, typeof(XMLAirDragWriter_v2_0))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10, XMLTypes.AirDragDataDeclarationType, typeof(XMLAirDragWriter_v1_0))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLTypes.AirDragModifiedUseStandardValueType, typeof(XMLAirDragWriter_v2_4))]
    public void CreateAirdragComponentWriter(string version, string type, Type expectedType)
	{

		XNamespace ns = version;
		var writer = _xmlWriterFactory.CreateComponentWriterWithVersion<IXMLComponentWriter>(ns, type);

		Assert.AreEqual(expectedType, writer.GetType());
	}


	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLTypes.AUX_Conventional_CompletedBusType, typeof(XMLCompletedBusAuxiliariesWriterConventional))]
	[TestCase(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLTypes.AUX_xEV_CompletedBusType, typeof(XMLCompletedBusAuxiliariesWriter_xEV))]
    public void CreateBusAuxiliariesWriter(string version, string type, Type expectedType)
	{

		XNamespace ns = version;
		var writer = _xmlWriterFactory.CreateComponentWriterWithVersion<IXMLBusAuxiliariesWriter>(ns, type);

		Assert.AreEqual(expectedType, writer.GetType());
	}

}