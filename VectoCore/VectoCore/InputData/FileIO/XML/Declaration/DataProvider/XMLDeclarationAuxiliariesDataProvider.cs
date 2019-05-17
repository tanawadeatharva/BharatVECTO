using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAuxiliariesDataProviderV10 : AbstractXMLType, IXMLAuxiliariesDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AuxiliariesDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IList<IAuxiliaryDeclarationInputData> _auxiliaries;


		public XMLDeclarationAuxiliariesDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IAuxiliariesDeclarationInputData

		public virtual bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual IList<IAuxiliaryDeclarationInputData> Auxiliaries
		{
			get {
				if (_auxiliaries != null) {
					return _auxiliaries;
				}

				_auxiliaries = new List<IAuxiliaryDeclarationInputData>();

				//var auxNodes = GetNodes(XMLNames.Auxiliaries_Auxiliary);
				var auxNodes = BaseNode.SelectNodes(XMLHelper.QueryLocalName(XMLNames.Auxiliaries_Auxiliary_Technology) + "/..");
				if (auxNodes == null) {
					return _auxiliaries;
				}

				foreach (XmlNode auxNode in auxNodes) {
					_auxiliaries.Add(Reader.CreateAuxiliary(auxNode));
				}

				return _auxiliaries;
			}
		}

		#endregion

		#region Implementation of IXMLAuxiliariesDeclarationInputData

		public virtual IXMLAuxiliaryReader Reader { protected get; set; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAuxiliariesDataProviderV20 : XMLDeclarationAuxiliariesDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "AuxiliariesComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		static XMLDeclarationAuxiliariesDataProviderV20()
		{
			NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;
		}

		public XMLDeclarationAuxiliariesDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }
	}
}
