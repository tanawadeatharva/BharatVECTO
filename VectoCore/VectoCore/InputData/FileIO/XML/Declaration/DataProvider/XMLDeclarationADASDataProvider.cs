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

		public virtual bool EngineStopStart
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EngineStopStart)); }
		}

		public virtual bool EcoRollWitoutEngineStop
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop)); }
		}

		public virtual bool EcoRollWithEngineStop
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart)); }
		}

		public virtual PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return PredictiveCruiseControlTypeHelper.Parse(GetString(XMLNames.Vehicle_ADAS_PCC)); }
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationADASDataProviderV20 : XMLDeclarationADASDataProviderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public XMLDeclarationADASDataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.DefaultValue; }
		}

		public override bool EngineStopStart
		{
			get { return false; }
		}

		public override bool EcoRollWitoutEngineStop
		{
			get { return false; }
		}

		public override bool EcoRollWithEngineStop
		{
			get { return false; }
		}

		public override PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return PredictiveCruiseControlType.None; }
		}
	}
}
