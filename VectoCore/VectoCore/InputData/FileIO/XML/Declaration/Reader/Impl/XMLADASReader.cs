using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLADASReaderV10 : AbstractComponentReader, IXMLADASReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "ADASType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IXMLDeclarationVehicleData Vehicle;
		protected IAdvancedDriverAssistantSystemDeclarationInputData _adas;


		public XMLADASReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode)
		{
			Vehicle = vehicle;
		}

		#region Implementation of IXMLADASReader

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADASInputData
		{
			get { return _adas ?? (_adas = CreateComponent(XMLNames.Vehicle_ADAS, ADASCreator)); }
		}

		#endregion

		protected virtual IAdvancedDriverAssistantSystemDeclarationInputData ADASCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				version = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace);
			}
			return Factory.CreateADASData(version, Vehicle, componentNode, sourceFile);
		}

		public virtual XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV20 : XMLADASReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "ADASType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLADASReaderV20(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode) { }

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADASInputData
		{
			get { return _adas ?? (_adas = CreateComponent(XMLNames.Vehicle_ADAS, ADASCreator, true)); }
		}

		public override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV21 : XMLADASReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "AdvancedDriverAssistantSystemsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLADASReaderV21(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode) { }

		public override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV23 : XMLADASReaderV21
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;

		public new const string XSD_TYPE = "AdvancedDriverAssistantSystemsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLADASReaderV23(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode)
		{ }

		public override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
