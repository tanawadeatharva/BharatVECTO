using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public abstract class XMLCommonElectricMotorDeclarationInputData : AbstractCommonComponentType, IComponentInputData,
		IPowerRatingInputData, IElectricMotorVoltageLevel
	{

		protected XMLCommonElectricMotorDeclarationInputData(XmlNode node, string source) : base(node, source)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IPowerRatingInputData

		public virtual ElectricMachineType ElectricMachineType =>
			GetString(XMLNames.ElectricMachine_ElectricMachineType).ParseEnum<ElectricMachineType>();

		public virtual Watt R85RatedPower =>
			GetDouble(XMLNames.ElectricMachine_R85RatedPower).SI<Watt>();

		public virtual KilogramSquareMeter Inertia =>
			GetDouble(XMLNames.ElectricMachine_RotationalInertia).SI<KilogramSquareMeter>();

		public virtual NewtonMeter ContinuousTorque =>
			GetDouble(XMLNames.ElectricMachine_ContinuousTorque).SI<NewtonMeter>();

		public virtual PerSecond ContinuousTorqueSpeed =>
			GetDouble(XMLNames.ElectricMachine_TestSpeedContinuousTorque).SI<PerSecond>();

		public virtual NewtonMeter OverloadTorque =>
			GetDouble(XMLNames.ElectricMachine_OverloadTorque).SI<NewtonMeter>();

		public virtual PerSecond OverloadTestSpeed =>
			GetDouble(XMLNames.ElectricMachine_TestSpeedOverloadTorque).SI<PerSecond>();

		public virtual Second OverloadTime =>
			GetDouble(XMLNames.ElectricMachine_OverloadDuration).SI<Second>();

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace { get; }
		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IElectricMotorVoltageLevel

		public virtual Volt VoltageLevel => GetDouble(XMLNames.VoltageLevel_Voltage).SI<Volt>();

		public virtual TableData FullLoadCurve => ReadFullLoadCurve();
		

		public virtual TableData EfficiencyMap => ReadTableData(XMLNames.PowerMap, XMLNames.PowerMap_Entry, new Dictionary<string, string> {
			{ XMLNames.PowerMap_OutShaftSpeed, XMLNames.PowerMap_OutShaftSpeed },
			{ XMLNames.PowerMap_Torque, XMLNames.PowerMap_Torque },
			{ XMLNames.PowerMap_ElectricPower, XMLNames.PowerMap_ElectricPower }
		});

		public virtual IList<IElectricMotorPowerMap> PowerMap => GetPowerMaps();

		#endregion

		protected virtual TableData ReadFullLoadCurve()
		{
			return ReadTableData(XMLNames.MaxTorqueCurve, XMLNames.MaxTorqueCurve_Entry, new Dictionary<string, string> {
				{ XMLNames.MaxTorqueCurve_OutShaftSpeed, XMLNames.MaxTorqueCurve_OutShaftSpeed },
				{ XMLNames.MaxTorqueCurve_MaxTorque, XMLNames.MaxTorqueCurve_MaxTorque },
				{ XMLNames.MaxTorqueCurve_MinTorque, XMLNames.MaxTorqueCurve_MinTorque }
			});
		}
		
		protected virtual TableData ReadDragCurve()
		{
			return ReadTableData(XMLNames.DragCurve, XMLNames.DragCurve_Entry, new Dictionary<string, string>
			{
				{XMLNames.DragCurve_OutShaftSpeed, XMLNames.DragCurve_OutShaftSpeed},
				{XMLNames.DragCurve_DragTorque, XMLNames.DragCurve_DragTorque}
			});
		}

		protected virtual TableData ReadConditioning()
		{
			return ReadTableData(XMLNames.Conditioning, XMLNames.Conditioning_Entry, new Dictionary<string, string> {
				{ XMLNames.Conditioning_CoolantTempInlet, XMLNames.Conditioning_CoolantTempInlet },
				{ XMLNames.Conditioning_CoolingPower, XMLNames.Conditioning_CoolingPower }
			});
		}
		
		protected virtual IList<IElectricMotorVoltageLevel> GetVoltageLevels()
		{
			var voltageLevelNodes = GetNodes(XMLNames.ElectricMachine_VoltageLevel);
			if (voltageLevelNodes.IsNullOrEmpty())
				return null;

			var voltageLevels = new List<IElectricMotorVoltageLevel>();

			foreach (XmlNode voltageLevelNode in voltageLevelNodes) {
				voltageLevels.Add(new XMLElectricMotorIEPCIInputDataProviderV2101(null, voltageLevelNode, null));
			}

			return voltageLevels;
		}

		
		private IList<IElectricMotorPowerMap> GetPowerMaps()
		{
			var powerMapNodes = GetNodes(XMLNames.PowerMap);
			if (powerMapNodes.IsNullOrEmpty())
				return null;

			var powerMaps = new List<IElectricMotorPowerMap>();
			foreach (XmlNode powerMapNode in powerMapNodes)
			{
				powerMaps.Add(new ElectricMotorPowerMap(powerMapNode));
			}

			return powerMaps;
		}

		
		public class ElectricMotorPowerMap : AbstractXMLType, IElectricMotorPowerMap
		{
			private Dictionary<string, string> powerMapMapping = new Dictionary<string, string> {
				{ XMLNames.PowerMap_OutShaftSpeed, XMLNames.PowerMap_OutShaftSpeed },
				{ XMLNames.PowerMap_Torque, XMLNames.PowerMap_Torque },
				{ XMLNames.PowerMap_ElectricPower, XMLNames.PowerMap_ElectricPower }
			};
			
			public ElectricMotorPowerMap(XmlNode xmlNode) : base(xmlNode) { }
			
			#region Implementation of IElectricMotorPowerMap

			public virtual int Gear => Convert.ToInt32(GetAttribute(BaseNode, "gear"));
			public virtual TableData PowerMap => ReadPowerMap();

			private TableData ReadPowerMap()
			{
				var powerMapEntryNodes = GetNodes(XMLNames.PowerMap_Entry);
				return XMLHelper.ReadTableData(powerMapMapping, powerMapEntryNodes);
			}

			#endregion
		}
	}


	// ---------------------------------------------------------------------------------------


	public class XMLElectricMotorDeclarationInputDataProviderV2101 : XMLCommonElectricMotorDeclarationInputData,
		IXMLElectricMotorDeclarationInputData
	{
		public  static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V2101_JOBS;
		public const string XSD_TYPE = "ElectricMachineSystemMeasuredDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricMotorVoltageLevel> _voltageLevels;

		public XMLElectricMotorDeclarationInputDataProviderV2101(
			XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IElectricMotorDeclarationInputData
		
		public virtual Volt TestVoltageOverload =>
			GetDouble(XMLNames.ElectricMachine_TestVoltageOverload).SI<Volt>();

		public virtual bool DcDcConverterIncluded => GetBool(XMLNames.ElectricMachine_DcDcConverterIncluded);

		public virtual string IHPCType => GetString(XMLNames.ElectricMachine_IHPCType);

		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels =>
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public virtual TableData DragCurve => ReadDragCurve();


		public virtual TableData Conditioning => ElementExists(XMLNames.Conditioning) 
			? ReadConditioning() : null;
		
		public virtual double OverloadRecoveryFactor { get; }

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorIHPCDeclarationInputDataProviderV2101 : XMLElectricMotorDeclarationInputDataProviderV2101
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V2101_JOBS;
		public new const string XSD_TYPE = "ElectricMachineSystemIHPCMeasuredDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLElectricMotorIHPCDeclarationInputDataProviderV2101(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile) { }
	}
	
	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorIEPCIInputDataProviderV2101 : XMLCommonElectricMotorDeclarationInputData, IXMLIEPCInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V2101_JOBS;
		public const string XSD_TYPE = "IEPCMeasuredDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IXMLDeclarationVehicleData _vehicle;
		private IList<IElectricMotorVoltageLevel> _voltageLevels;
		private IList<IGearEntry> _gears;


		public XMLElectricMotorIEPCIInputDataProviderV2101(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			_vehicle = vehicle;
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Overrides of AbstractCommonComponentType

		public override CertificationMethod CertificationMethod
		{
			get
			{
				var certMethod = GetString(XMLNames.Component_Gearbox_CertificationMethod, required: false) ??
								GetString(XMLNames.Component_CertificationMethod, required: false);

				if (certMethod != null && certMethod == "Measured for complete component")
					return CertificationMethod.Measured;

				return certMethod != null
					? EnumHelper.ParseEnum<CertificationMethod>(certMethod)
					: CertificationMethod.Measured;
			}
		}

		#endregion


		#region Implementation of IIEPCDeclarationInputData

		public virtual Volt TestVoltageOverload =>
			GetDouble(XMLNames.ElectricMachine_TestVoltageOverload).SI<Volt>();

		public virtual bool DifferentialIncluded => GetBool(XMLNames.IEPC_DifferentialIncluded);

		public virtual bool DesignTypeWheelMotor => GetBool(XMLNames.IEPC_DesignTypeWheelMotor);

		public virtual int? NrOfDesignTypeWheelMotorMeasured => ElementExists(XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured) ?
			(int?)Convert.ToInt32(GetString(XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured)) : null;

		public virtual IList<IGearEntry> Gears =>
			_gears ?? (_gears = GetGearEntries());
		
		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels =>
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public virtual TableData DragCurve => ReadDragCurve();

		public virtual TableData Conditioning => ElementExists(XMLNames.Conditioning)
			? ReadConditioning() : null;

		#endregion
		
		private IList<IGearEntry> GetGearEntries()
		{
			var gearNodes = GetNodes(XMLNames.Gear_EntryName);
			if (gearNodes.IsNullOrEmpty())
				return null;

			var gears = new List<IGearEntry>();
			foreach (XmlNode gearNode in gearNodes)
			{
				gears.Add(new GearEntry(gearNode));
			}

			return gears;
		}

		public class GearEntry : AbstractXMLType, IGearEntry
		{
			public GearEntry(XmlNode node) : base(node) { }

			#region Implementation of IGearEntry

			public virtual int GearNumber => Convert.ToInt32(GetAttribute(BaseNode, XMLNames.Gear_GearNumber_Attr));
			
			public virtual double Ratio => GetDouble(XMLNames.Gear_Ratio);

			public virtual NewtonMeter MaxOutputShaftTorque => ElementExists(XMLNames.Gear_MaxOutputShaftTorque)
				? GetDouble(XMLNames.Gear_MaxOutputShaftTorque).SI<NewtonMeter>()
				: null;

			public virtual PerSecond MaxOutputShaftSpeed => ElementExists(XMLNames.Gear_MaxOutputShaftSpeed)
				? GetDouble(XMLNames.Gear_MaxOutputShaftSpeed).SI<PerSecond>()
				: null;

			#endregion
		}
		
		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

}
