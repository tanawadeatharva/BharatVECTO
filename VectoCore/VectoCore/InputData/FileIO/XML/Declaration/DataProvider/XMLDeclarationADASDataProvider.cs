using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationADASDataProviderV10 : AbstractCommonComponentType,
		IXMLAdvancedDriverAssistantSystemDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "ADASType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationADASDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public virtual bool EngineStopStart
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EngineStopStart)); }
		}

		public virtual EcoRollType EcoRoll
		{
			get {
				return EcorollTypeHelper.Get(
					XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop)),
					XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart)));
			}
		}

		public virtual PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return PredictiveCruiseControlTypeHelper.Parse(GetString(XMLNames.Vehicle_ADAS_PCC)); }
		}

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationADASDataProviderV21 : XMLDeclarationADASDataProviderV10
	{
		/*
		 * ADAS are added in version 2.1
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "AdvancedDriverAssistantSystemsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationADASDataProviderV21(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
