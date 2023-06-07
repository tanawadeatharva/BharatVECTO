using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLElectricStorageSystemDeclarationInputDataV24 : AbstractCommonComponentType,
		IXMLElectricStorageSystemDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricStorageDeclarationInputData> _electricStorageElements;
		private IXMLDeclarationVehicleData _vehicle;
		
		public XMLElectricStorageSystemDeclarationInputDataV24(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			_vehicle = vehicle;
			//SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IElectricStorageSystemDeclarationInputData

		public virtual IList<IElectricStorageDeclarationInputData> ElectricStorageElements =>
			_electricStorageElements ?? (_electricStorageElements = GetElectricStorages());

		#endregion
		
		private IList<IElectricStorageDeclarationInputData> GetElectricStorages()
		{
			var electricStorages = new List<IElectricStorageDeclarationInputData>();
			if (ElementExists(XMLNames.ElectricEnergyStorage_Capacitor)) {
				var capacitor = GetNode(XMLNames.ElectricEnergyStorage_Capacitor);
				electricStorages.Add(new XMLElectricStorageDeclaration {
							REESSPack = StorageTypeReader.CreateREESSInputData(capacitor, REESSType.SuperCap),
							Count = 1,
							StringId = 1,
				});
			}else if (ElementExists(XMLNames.ElectricEnergyStorage_Battery)) {
				var batteries = GetNodes(XMLNames.ElectricEnergyStorage_Battery);
				foreach (XmlNode battery in batteries)
				{
					
					electricStorages.Add(new XMLElectricStorageDeclaration {
							REESSPack = StorageTypeReader.CreateREESSInputData(battery, REESSType.Battery),
							StringId = XmlConvert.ToInt32(GetString(XMLNames.Battery_StringID, battery))
					});
				}
			}
		
			return electricStorages.Any() ? electricStorages : null;
		}

		#region Implementation of IXMLElectricStorageSystemDeclarationInputData

		public virtual IXMLREESSReader StorageTypeReader { protected get; set; }

		#endregion
		
		#region Implementation of IXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType => DataSourceType.XMLEmbedded;

		#endregion
		
		public class XMLElectricStorageDeclaration : IElectricStorageDeclarationInputData
		{
			#region Implementation of IElectricStorageDeclarationInputData
			public IREESSPackInputData REESSPack { get; set; }
			public int Count { get; set; }
			public int StringId { get; set; }

			#endregion
		}
	}

	// ---------------------------------------------------------------------------------------

	public abstract class AbstractBatteryPackDeclarationInputDataProvider : AbstractCommonComponentType, IXMLBatteryPackDeclarationInputData
	{
		protected AbstractBatteryPackDeclarationInputDataProvider(XmlNode node, string source) : base(node, source) { }
		protected abstract XNamespace NamespaceURI { get; }

        #region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.Battery;



		#endregion


		protected TableData _correctedInternalResistanceCurve = null;

		#region Implementation of IBatteryPackDeclarationInputData

        public virtual double? MinSOC
		{
			get
			{
				// ancestor-or-self::*[local-name()='Battery']/*[local-name()='SOCmin']
				var node = BaseNode.SelectSingleNode($"ancestor-or-self::*[local-name()='{XMLNames.ElectricEnergyStorage_Battery}']/*[local-name()='{XMLNames.Battery_SOCmin}']");
				return node != null
					? node.InnerText.ToDouble() / 100
					: (double?)null;
			}
		}

		public virtual double? MaxSOC
		{
			get
			{
				// ancestor-or-self::*[local-name()='Battery']/*[local-name()='SOCmin']
				var node = BaseNode.SelectSingleNode($"ancestor-or-self::*[local-name()='{XMLNames.ElectricEnergyStorage_Battery}']/*[local-name()='{XMLNames.Battery_SOCmax}']");
				return node != null
					? node.InnerText.ToDouble() / 100
					: (double?)null;
            }
		}

		public virtual BatteryType BatteryType => GetString(XMLNames.REESS_BatteryType).ParseEnum<BatteryType>();
		public virtual AmpereSecond Capacity => GetDouble(XMLNames.REESS_RatedCapacity).SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>();

		public virtual bool? ConnectorsSubsystemsIncluded => CertificationMethod == CertificationMethod.StandardValues
			? (bool?)null
			: GetBool(XMLNames.REESS_ConnectorsSubsystemsIncluded);

		public virtual bool? JunctionboxIncluded => CertificationMethod == CertificationMethod.StandardValues
			? (bool?)null
			: GetBool(XMLNames.REESS_JunctionboxIncluded);

		public virtual Kelvin TestingTemperature => CertificationMethod != CertificationMethod.StandardValues
			? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin()
			: null;

		public virtual TableData InternalResistanceCurve => _correctedInternalResistanceCurve ??
															(_correctedInternalResistanceCurve =
																GetInternalResistanceCurve());



		public virtual TableData VoltageCurve => ReadTableData(XMLNames.REESS_OCV, XMLNames.REESS_MapEntry,
			AttributeMappings.VoltageMap);

		public virtual TableData MaxCurrentMap => ReadTableData(XMLNames.REESS_CurrentLimits, XMLNames.REESS_MapEntry,
			AttributeMappings.MaxCurrentMap);

        #endregion
		protected virtual TableData GetInternalResistanceCurve()
		{
			return ReadTableData(XMLNames.REESS_InternalResistanceCurve, XMLNames.REESS_MapEntry,
				AttributeMappings.InternalResistanceMap);
		}




        #region Overrides of AbstractXMLResource

        protected override XNamespace SchemaNamespace => NamespaceURI;
		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		#endregion
	}


    public class XMLBatteryPackDeclarationInputDataMeasuredV23 : AbstractBatteryPackDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "BatterySystemDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataMeasuredV23(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

        #endregion
    }

	public class XMLBatteryPackDeclarationInputDataStandardV23 : AbstractBatteryPackDeclarationInputDataProvider
    {
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "BatterySystemStandardValuesDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataStandardV23(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

		protected override TableData GetInternalResistanceCurve()
		{
			var corrected = base.GetInternalResistanceCurve();
			var socMap = BatterySOCReader.Create(VoltageCurve);
			var vNom = socMap.Lookup(0.5);

			Volt u_cell;
			switch (BatteryType)
			{
				case BatteryType.HPBS:
					u_cell = 3.3.SI<Volt>();
					break;
				case BatteryType.HEBS:
					u_cell = 3.7.SI<Volt>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var dcir_corr = vNom / u_cell;


			var nrCols = corrected.Columns.Count;
			//mOhm
			foreach (DataRow row in corrected.Rows) {
				for (var i = 1; i < nrCols; i++) {
					var uncorr = row.ParseDouble(i);
					row[i] = uncorr * dcir_corr.Value();
				}
			}

			return corrected;
		}

		#endregion
	}



	public class XMLBatteryPackDeclarationDeclarationInputDataV24 : AbstractCommonComponentType,
		IXMLBatteryPackDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "REESSBatteryType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Obsolete]
        public XMLBatteryPackDeclarationDeclarationInputDataV24(XmlNode componentNode, string sourceFile) 
			: base(componentNode, sourceFile)
		{
			throw new NotImplementedException("Replaced with v2.3 dataprovider");
			//SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.Battery;

		#endregion

		#region Implementation of IBatteryPackDeclarationInputData

		public virtual double? MinSOC  => 
			ElementExists(XMLNames.Battery_SOCmin) ? GetDouble(XMLNames.Battery_SOCmin) / 100 : (double?)null;

		public virtual double? MaxSOC => 
			ElementExists(XMLNames.Battery_SOCmax) ? GetDouble(XMLNames.Battery_SOCmax) / 100 : (double?)null;

		public virtual BatteryType BatteryType => GetString(XMLNames.REESS_BatteryType).ParseEnum<BatteryType>();
		public virtual AmpereSecond Capacity => GetDouble(XMLNames.REESS_RatedCapacity).SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>();

		public virtual bool? ConnectorsSubsystemsIncluded => CertificationMethod == CertificationMethod.StandardValues
			? (bool?)null
			: GetBool(XMLNames.REESS_ConnectorsSubsystemsIncluded);

		public virtual bool? JunctionboxIncluded => CertificationMethod == CertificationMethod.StandardValues
			? (bool?)null
			: GetBool(XMLNames.REESS_JunctionboxIncluded);

		public virtual Kelvin TestingTemperature => CertificationMethod != CertificationMethod.StandardValues
			? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin()
			: null;

		public virtual TableData InternalResistanceCurve => ReadTableData(XMLNames.REESS_InternalResistanceCurve, XMLNames.REESS_MapEntry,
			AttributeMappings.InternalResistanceMap);
		
		public virtual TableData VoltageCurve => ReadTableData(XMLNames.REESS_OCV, XMLNames.REESS_MapEntry, 
			AttributeMappings.VoltageMap);
		
		public virtual TableData MaxCurrentMap => ReadTableData(XMLNames.REESS_CurrentLimits, XMLNames.REESS_MapEntry, 
			AttributeMappings.MaxCurrentMap);

        #endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType => DataSourceType.XMLFile;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLBatteryPackDeclarationDeclarationInputDataV01 : XMLBatteryPackDeclarationDeclarationInputDataV24
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "REESSBatteryType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationDeclarationInputDataV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

	}

	public class XMLBatteryPackDeclarationInputDataMeasuredV01 : AbstractBatteryPackDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "BatterySystemDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataMeasuredV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

		#endregion
	}

	public class XMLBatteryPackDeclarationInputDataStandardV01 : AbstractBatteryPackDeclarationInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "BatterySystemStandardValuesDataType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLBatteryPackDeclarationInputDataStandardV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of AbstractBatteryPackDeclarationInputDataProvider

		protected override XNamespace NamespaceURI => NAMESPACE_URI;

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLSuperCapDeclarationInputDataV24 : AbstractCommonComponentType, IXMLSuperCapDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public const string XSD_TYPE = "REESSCapacitorType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLSuperCapDeclarationInputDataV24(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			//SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.SuperCap;

		#endregion

		#region Implementation of ISuperCapDeclarationInputData

		public virtual Farad Capacity => GetDouble(XMLNames.Capacitor_Capacitance).SI<Farad>();
		public virtual Ohm InternalResistance => GetDouble(XMLNames.Capacitor_InternalResistance).SI(Unit.SI.Milli.Ohm).Cast<Ohm>();
		public virtual Volt MinVoltage => GetDouble(XMLNames.Capacitor_MinVoltage).SI<Volt>();
		public virtual Volt MaxVoltage => GetDouble(XMLNames.Capacitor_MaxVoltage).SI<Volt>();
		public virtual Ampere MaxCurrentCharge => GetDouble(XMLNames.Capacitor_MaxChargingCurrent).SI<Ampere>();
		public virtual Ampere MaxCurrentDischarge => GetDouble(XMLNames.Capacitor_MaxDischargingCurrent).SI<Ampere>();

		public virtual Kelvin TestingTemperature => 
			ElementExists(XMLNames.REESS_TestingTemperature)
				? GetDouble(XMLNames.REESS_TestingTemperature).DegCelsiusToKelvin() : null;
		
		#endregion
		
		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType => DataSourceType.XMLEmbedded;

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLSuperCapDeclarationInputDataV01 : XMLSuperCapDeclarationInputDataV24
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "REESSCapacitorType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLSuperCapDeclarationInputDataV01(XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile) { }

		#region Overrides of XMLSuperCapDeclarationInputDataV24

		public override Ohm InternalResistance => null;

		#endregion
	}

	//===================

		public class XMLElectricStorageSystemDeclarationInputDataV01 : XMLElectricStorageSystemDeclarationInputDataV24
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "ElectricEnergyStorageType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLElectricStorageSystemDeclarationInputDataV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }
	}
}
