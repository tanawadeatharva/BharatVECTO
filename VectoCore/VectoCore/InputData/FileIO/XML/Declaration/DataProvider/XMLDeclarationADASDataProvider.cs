using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationADASDataProviderV10 : AbstractCommonComponentType,
		IXMLAdvancedDriverAssistantSystemDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public XMLDeclarationADASDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public bool EngineStopStart
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EngineStopStart)); }
		}

		public bool EcoRollWitoutEngineStop
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop)); }
		}

		public bool EcoRollWithEngineStop
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart)); }
		}

		public PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return PredictiveCruiseControlTypeHelper.Parse(GetString(XMLNames.Vehicle_ADAS_PCC)); }
		}

		#endregion
	}
}
