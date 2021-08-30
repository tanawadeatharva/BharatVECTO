using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLElectricStorageSystemDeclarationInputData : AbstractCommonComponentType,
		IXMLElectricStorageSystemDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricStorageDeclarationInputData> _electricStorageElements;
		
		#region Implementation of IElectricStorageSystemDeclarationInputData

		public IList<IElectricStorageDeclarationInputData> ElectricStorageElements =>
			_electricStorageElements ?? (_electricStorageElements = GetElectricStorages());

		#endregion

		public XMLElectricStorageSystemDeclarationInputData(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{

		}

		#region Implementation of IXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion

		private IList<IElectricStorageDeclarationInputData> GetElectricStorages()
		{
			var electricStorages = new List<IElectricStorageDeclarationInputData>();

			var batteries = GetNodes("Battery");
			if (!(batteries?.Count > 0))
				return null;
			foreach (XmlNode battery in batteries) {
				var electricStorage = new XMLElectricStorageDeclaration {
					REESSPack = StorageTypeReader.CreateREESSInputData(battery, REESSType.Battery),
					StringId = XmlConvert.ToInt32(GetString("StringID", battery))
				};
				electricStorages.Add(electricStorage);
			}
			
			return electricStorages;
		}

		#region Implementation of IXMLElectricStorageSystemDeclarationInputData

		public IXMLREESSReader StorageTypeReader { get; set; }

		#endregion
	}

	public class XMLElectricStorageDeclaration : IElectricStorageDeclarationInputData
	{
		#region Implementation of IElectricStorageDeclarationInputData

		public IREESSPackInputData REESSPack { get; set; }
		public int Count { get; set; }
		public int StringId { get; set; }

		#endregion
	}
	
	public class XMLBatteryPackDeclarationDeclarationInputData : AbstractCommonComponentType,
		IXMLBatteryPackDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public const string XSD_TYPE = "REESSBatteryType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationDeclarationInputData( XmlNode componentNode, string sourceFile) 
			: base(componentNode, sourceFile)
		{
		}
		
		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.Battery;

		#endregion

		#region Implementation of IBatteryPackDeclarationInputData

		public double MinSOC => GetDouble("SOCmin", 0);
		public double MaxSOC => GetDouble("SOCmax", 0);
		public BatteryType BatteryType => BatteryTypeHelper.Parse(GetString("BatteryType"));
		public AmpereSecond Capacity => GetDouble("RatedCapacity").SI<AmpereSecond>() * 3600;
		public bool ConnectorsSubsystemsIncluded => GetBool("ConnectorsSubsystemsIncluded");
		public bool JunctionboxIncluded => GetBool("JunctionboxIncluded");
		public Kelvin TestingTemperature => GetDouble("TestingTemperature", 0).DegCelsiusToKelvin();

		public TableData InternalResistanceCurve => ReadTableData("InternalResistance", "Entry", new Dictionary<string, string> {
				{"SoC", "SoC"},
				{"R_2", "R_2"},
				{"R_10", "R_10"},
				{"R_20", "R_20"}
		});

		public TableData VoltageCurve => ReadTableData("OCV", "Entry", new Dictionary<string, string> {
			{"SoC", "SoC"},
			{"OCV", "OCV"}
		});

		public TableData MaxCurrentMap => ReadTableData("CurrentLimits", "Entry", new Dictionary<string, string> {
			{"SoC", "SoC"},
			{"maxChagingCurrent", "maxChagingCurrent"},
			{"maxDischargingCurrent", "maxDischargingCurrent"}
		});

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}
}
