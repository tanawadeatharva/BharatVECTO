using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationPrimaryBusAuxiliariesDataProviderV210 : AbstractXMLType, IXMLBusAuxiliariesDeclarationData,
		IElectricSupplyDeclarationData, IPneumaticConsumersDeclarationData,
		IPneumaticSupplyDeclarationData, IHVACBusAuxiliariesDeclarationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;

		public const string XSD_TYPE = "AUX_Conventional_PrimaryBusType";
		public const string XSD_HEV_P_TYPE = "AUX_HEV-P_PrimaryBusType";
		
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_XSD_HEV_P_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_P_TYPE);


		public XMLDeclarationPrimaryBusAuxiliariesDataProviderV210(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		public virtual XmlNode XMLSource => BaseNode;

		public virtual string FanTechnology
		{
			get { return GetNode(new[] { "Fan", XMLNames.Auxiliaries_Auxiliary_Technology }).InnerText; }
		}

		public virtual IList<string> SteeringPumpTechnology
		{
			get
			{
				return GetNodes(new[] { "SteeringPump", XMLNames.Auxiliaries_Auxiliary_Technology })
					.Cast<XmlNode>().Select(x => x.InnerText).ToList();
			}
		}

		public virtual IElectricSupplyDeclarationData ElectricSupply => this;

		public virtual IElectricConsumersDeclarationData ElectricConsumers => null;

		public virtual IPneumaticSupplyDeclarationData PneumaticSupply => this;

		public virtual IPneumaticConsumersDeclarationData PneumaticConsumers => this;

		public virtual IHVACBusAuxiliariesDeclarationData HVACAux => this;

		#endregion

		#region Implementation of IElectricSupplyDeclarationData

		public AlternatorType AlternatorTechnology
		{
			get
			{
				return GetString(new[]
						{ XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_AlternatorTechnology })
					.ParseEnum<AlternatorType>();
			}
		}

		public virtual IList<IAlternatorDeclarationInputData> Alternators
		{
			get
			{
				return GetNodes(new[] { XMLNames.BusAux_ElectricSystem, "SmartAlternator" })
					.Cast<XmlNode>().Select(x => {
						var ratedCurrent = GetNode("RatedCurrent", x).InnerText.ToDouble().SI<Ampere>();
						var ratedVoltage = GetNode("RatedVoltage", x).InnerText.ToDouble().SI<Volt>();
						return new AlternatorInputData(ratedVoltage, ratedCurrent);
					})
					.Cast<IAlternatorDeclarationInputData>().ToList();
			}
		}

		public IList<IBusAuxElectricStorageDeclarationInputData> ElectricStorage
		{
			get
			{
				return GetNodes(new[] { XMLNames.BusAux_ElectricSystem, "Battery" })
					.Cast<XmlNode>().Select(x => {
						var ratedCapacity = GetNode("RatedCapacity", x).InnerText.ToDouble().SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>();
						var voltage = GetNode("NominalVoltage", x).InnerText.ToDouble().SI<Volt>();
						var technology = GetNode("BatteryTechnology", x).InnerText;
						return new BusAuxBatteryInputData(technology, voltage, ratedCapacity);
					})
					.Concat(GetNodes(new [] { XMLNames.BusAux_ElectricSystem, "Capacitor" }).Cast<XmlNode>()
						.Select(x => {
							var ratedCapacity = GetNode("RatedCapacitance", x).InnerText.ToDouble().SI<Farad>();
							var voltage = GetNode("RatedVoltage", x).InnerText.ToDouble().SI<Volt>();
							var technology = GetNode("CapacitorTechnology", x).InnerText;
							return new BusAuxCapacitorInputData(technology, voltage, ratedCapacity);
						}).Cast<IBusAuxElectricStorageDeclarationInputData>())
					.Cast<IBusAuxElectricStorageDeclarationInputData>().ToList();
			}
		}

		public virtual bool SmartElectrics
		{
			get { return GetBool(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_SmartElectrics }); }
		}

		public virtual Watt MaxAlternatorPower => ElementExists("MaxAlternatorPower") ? GetDouble("MaxAlternatorPower").SI<Watt>() : null;

		public virtual WattSecond ElectricStorageCapacity =>
			ElementExists("ElectricStorageCapacity")
				? GetDouble("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>()
				: null;

		#endregion


		#region Implementation of IPneumaticConsumersDeclarationData

		public virtual ConsumerTechnology AirsuspensionControl
		{
			get
			{
				return ConsumerTechnologyHelper.Parse(
					GetString(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_AirsuspensionControl }));
			}
		}

		public virtual ConsumerTechnology AdBlueDosing
		{
			get
			{
				return GetBool(
					new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SCRReagentDosing })
					? ConsumerTechnology.Pneumatically
					: ConsumerTechnology.Electrically;
			}
		}

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

		public CompressorDrive CompressorDrive => CompressorDriveHelper.Parse(GetString(XMLNames.CompressorDrive));

		public virtual string Clutch { get { return GetString(new[] { XMLNames.BusAux_PneumaticSystem, "Clutch" }); } }

		public virtual double Ratio
		{
			get { return GetDouble(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_CompressorRatio }); }
		}

		public virtual string CompressorSize
		{
			get { return GetString(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_CompressorSize }); }
		}

		public virtual bool SmartAirCompression
		{
			get
			{
				return GetBool(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem });
			}
		}

		public virtual bool SmartRegeneration
		{
			get
			{
				return GetBool(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem });
			}
		}

		#endregion

		#region Implementation of IHVACBusAuxiliariesDeclarationData

		public virtual BusHVACSystemConfiguration? SystemConfiguration => BusHVACSystemConfiguration.Unknown;

		public virtual HeatPumpType? HeatPumpTypeCoolingDriverCompartment => null;
		public virtual HeatPumpType? HeatPumpTypeHeatingDriverCompartment => null;
		public virtual HeatPumpType? HeatPumpTypeCoolingPassengerCompartment => null;
		public virtual HeatPumpType? HeatPumpTypeHeatingPassengerCompartment => null;

		public virtual Watt AuxHeaterPower => 0.SI<Watt>();

		public virtual bool? DoubleGlazing => false;

		public virtual bool HeatPump => false;

		public virtual bool? OtherHeatingTechnology => false;

		public virtual bool? AdjustableCoolantThermostat
		{
			get { return GetBool(new[] { "HVAC", "AdjustableCoolantThermostat" }); }
		}

		public virtual bool? AdjustableAuxiliaryHeater => false;

		public virtual bool EngineWasteGasHeatExchanger
		{
			get { return GetBool(new[] { "HVAC", "EngineWasteGasHeatExchanger" }); }
		}

		public virtual bool? SeparateAirDistributionDucts => false;

		public virtual bool? WaterElectricHeater => false;

		public virtual bool? AirElectricHeater => false;

		#endregion
	}

	public class XMLDeclarationPrimaryBusAuxiliariesDataProviderV01 : XMLDeclarationPrimaryBusAuxiliariesDataProviderV210
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "AuxiliaryDataPIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPrimaryBusAuxiliariesDataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }


		public override XmlNode XMLSource => BaseNode;
	}


	public class XMLDeclarationCompletedBusAuxiliariesDataProviderV210 : XMLDeclarationPrimaryBusAuxiliariesDataProviderV210, IElectricConsumersDeclarationData
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;

		public new const string XSD_TYPE = "AUX_Conventional_CompletedBusType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationCompletedBusAuxiliariesDataProviderV210(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) {}

		private bool IsBusHVACTagEmpty()
		{
			return SystemConfiguration == null && HeatPumpTypeCoolingDriverCompartment == null && HeatPumpTypeHeatingDriverCompartment == null &&
					HeatPumpTypeCoolingPassengerCompartment == null && HeatPumpTypeHeatingPassengerCompartment == null && AuxHeaterPower == null && DoubleGlazing == null && 
					AdjustableAuxiliaryHeater == null && SeparateAirDistributionDucts == null && WaterElectricHeater == null &&
					AirElectricHeater == null && OtherHeatingTechnology == null ;
		}

		public override XmlNode XMLSource => BaseNode;

		public override IHVACBusAuxiliariesDeclarationData HVACAux => IsBusHVACTagEmpty() ? null : this;


		public override BusHVACSystemConfiguration? SystemConfiguration =>
			ElementExists(XMLNames.Bus_SystemConfiguration) 
				? BusHVACSystemConfigurationHelper.Parse(GetString(XMLNames.Bus_SystemConfiguration)) : null;

		public override HeatPumpType? HeatPumpTypeCoolingDriverCompartment =>
			ElementExists(XMLNames.Bus_HeatPumpTypeDriver)
				? HeatPumpTypeHelper.Parse(GetString(new [] {XMLNames.Bus_HeatPumpTypeDriver, "Cooling"})) : (HeatPumpType?)null;

		public override HeatPumpType? HeatPumpTypeHeatingDriverCompartment => ElementExists(XMLNames.Bus_HeatPumpTypeDriver)
			? HeatPumpTypeHelper.Parse(GetString(new[] { XMLNames.Bus_HeatPumpTypeDriver, "Heating" })) : (HeatPumpType?)null;
		public override HeatPumpType? HeatPumpTypeCoolingPassengerCompartment => ElementExists(XMLNames.Bus_HeatPumpTypePassenger)
			? HeatPumpTypeHelper.Parse(GetString(new[] { XMLNames.Bus_HeatPumpTypePassenger, "Cooling" })) : (HeatPumpType?)null;
		public override HeatPumpType? HeatPumpTypeHeatingPassengerCompartment => ElementExists(XMLNames.Bus_HeatPumpTypePassenger)
			? HeatPumpTypeHelper.Parse(GetString(new[] { XMLNames.Bus_HeatPumpTypePassenger, "Heating" })) : (HeatPumpType?)null;

		public override Watt AuxHeaterPower =>
			ElementExists(XMLNames.Bus_AuxiliaryHeaterPower)
				? GetDouble(XMLNames.Bus_AuxiliaryHeaterPower).SI<Watt>() : null;

		public override bool? DoubleGlazing =>
			ElementExists(XMLNames.Bus_DoubleGlazing)
				? GetBool(XMLNames.Bus_DoubleGlazing) : (bool?)null;

		public override bool? AdjustableAuxiliaryHeater =>
			ElementExists(XMLNames.Bus_AdjustableAuxiliaryHeater)
				? GetBool(XMLNames.Bus_AdjustableAuxiliaryHeater) : (bool?)null;

		public override bool? SeparateAirDistributionDucts =>
			ElementExists(XMLNames.Bus_SeparateAirDistributionDucts)
				? GetBool(XMLNames.Bus_SeparateAirDistributionDucts) : (bool?)null;


		public override bool? WaterElectricHeater =>
			ElementExists(XMLNames.Bus_WaterElectricHeater)
				? GetBool(XMLNames.Bus_WaterElectricHeater) : (bool?)null;

		public override bool? AirElectricHeater =>
			ElementExists(XMLNames.Bus_AirElectricHeater)
				? GetBool(XMLNames.Bus_AirElectricHeater) : (bool?)null;

		public override bool? OtherHeatingTechnology =>
			ElementExists(XMLNames.Bus_OtherHeatingTechnology)
				? GetBool(XMLNames.Bus_OtherHeatingTechnology) : (bool?)null;

		private bool IsElectricConsumersTagEmpty()
		{
			return InteriorLightsLED == null && DayrunninglightsLED == null && PositionlightsLED == null &&
					HeadlightsLED == null && BrakelightsLED == null;
		}


		public override IElectricConsumersDeclarationData ElectricConsumers => IsElectricConsumersTagEmpty() ? null : this;

		public virtual bool? InteriorLightsLED
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_Interiorlights))
					return null;
				return GetBool(new[] { "LEDLights", XMLNames.Bus_Interiorlights });
			}
		}

		public virtual bool? DayrunninglightsLED
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_Dayrunninglights))
					return null;
				return GetBool(new[] { "LEDLights", XMLNames.Bus_Dayrunninglights });
			}
		}

		public virtual bool? PositionlightsLED
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_Positionlights))
					return null;
				return GetBool(new[] { "LEDLights", XMLNames.Bus_Positionlights });
			}
		}

		public virtual bool? HeadlightsLED
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_Headlights))
					return null;
				return GetBool(new[] { "LEDLights", XMLNames.Bus_Headlights });
			}
		}

		public virtual bool? BrakelightsLED
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_Brakelights))
					return null;
				return GetBool(new[] { "LEDLights", XMLNames.Bus_Brakelights });
			}
		}


		public override IPneumaticSupplyDeclarationData PneumaticSupply => null;

		public override IElectricSupplyDeclarationData ElectricSupply => null;

		public override IPneumaticConsumersDeclarationData PneumaticConsumers => null;

		public override string FanTechnology => null;

		public override IList<string> SteeringPumpTechnology => null;
	}
}
