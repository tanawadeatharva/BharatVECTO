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
	public class XMLPTOReaderV10 : AbstractComponentReader, IXMLPTOReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "PTOType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IXMLDeclarationVehicleData Vehicle;
		protected IPTOTransmissionInputData _ptoInputData;


		public XMLPTOReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode) : base(
			vehicle, componentNode)
		{
			Vehicle = vehicle;
		}

		#region Implementation of IXMLPTOReader

		public virtual IPTOTransmissionInputData PTOInputData
		{
			get { return _ptoInputData ?? (_ptoInputData = CreateComponent(XMLNames.Vehicle_PTO, PTOCreator)); }
		}

		#endregion

		protected virtual IPTOTransmissionInputData PTOCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreatePTOData(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLPTOReaderV20 : XMLPTOReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "PTOType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLPTOReaderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode) : base(
			vehicle, componentNode) { }
	}
}
