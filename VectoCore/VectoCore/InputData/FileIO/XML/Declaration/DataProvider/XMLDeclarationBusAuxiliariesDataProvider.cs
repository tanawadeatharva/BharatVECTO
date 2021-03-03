using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationPrimaryBusAuxiliariesDataProviderV26 : AbstractXMLType, IXMLBusAuxiliariesDeclarationData,
		IElectricSupplyDeclarationData, IPneumaticConsumersDeclarationData,
		IPneumaticSupplyDeclarationData, IHVACBusAuxiliariesDeclarationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public const string XSD_TYPE = "PrimaryVehicleAuxiliaryDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		
		public XMLDeclarationPrimaryBusAuxiliariesDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		public virtual XmlNode XMLSource
		{
			get { return BaseNode; }
		}

		public virtual string FanTechnology
		{
			get { return GetNode(new[] { "Fan", XMLNames.Auxiliaries_Auxiliary_Technology }).InnerText; }
		}

		public virtual IList<string> SteeringPumpTechnology
		{
			get {
				return GetNodes(new[] { "SteeringPump", XMLNames.Auxiliaries_Auxiliary_Technology })
					.Cast<XmlNode>().Select(x => x.InnerText).ToList();
			}
		}

		public virtual IElectricSupplyDeclarationData ElectricSupply
		{
			get { return this; }
		}

		public virtual IElectricConsumersDeclarationData ElectricConsumers
		{
			get { return null; }
		}

		public virtual IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get { return this; }
		}

		public virtual IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get { return this; }
		}

		public virtual IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get { return this; }
		}

		#endregion

		#region Implementation of IElectricSupplyDeclarationData

		public virtual IList<IAlternatorDeclarationInputData> Alternators
		{
			get {
				return GetNodes(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_AlternatorTechnology })
					.Cast<XmlNode>().Select(x => new AlternatorInputData(x.InnerText))
					.Cast<IAlternatorDeclarationInputData>().ToList();
			}
		}

		public virtual bool SmartElectrics
		{
			get { return GetBool(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_SmartElectrics }); }
		}

		public virtual Watt MaxAlternatorPower
		{
			get { return ElementExists("MaxAlternatorPower") ? GetDouble("MaxAlternatorPower").SI<Watt>() : null; }
		}

		public virtual WattSecond ElectricStorageCapacity
		{
			get {
				return ElementExists("ElectricStorageCapacity")
					? GetDouble("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>()
					: null;
			}
		}

		#endregion


		#region Implementation of IPneumaticConsumersDeclarationData

		public virtual ConsumerTechnology AirsuspensionControl
		{
			get {
				return ConsumerTechnologyHelper.Parse(
					GetString(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_AirsuspensionControl }));
			}
		}

		public virtual ConsumerTechnology AdBlueDosing
		{
			get {
				return GetBool(
					new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_AdBlueDosing })
					? ConsumerTechnology.Pneumatically
					: ConsumerTechnology.Electrically;
			}
		}

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

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
			get {
				return GetBool(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem });
			}
		}

		public virtual bool SmartRegeneration
		{
			get {
				return GetBool(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem });
			}
		}

		#endregion

		#region Implementation of IHVACBusAuxiliariesDeclarationData

		public virtual BusHVACSystemConfiguration SystemConfiguration
		{
			get { return BusHVACSystemConfiguration.Unknown; }
		}

		public virtual HeatPumpType? HeatPumpTypeDriverCompartment { get { return null; } }
		public virtual HeatPumpMode? HeatPumpModeDriverCompartment { get { return null; } }
		public virtual HeatPumpType? HeatPumpTypePassengerCompartment { get { return null; } }
		public virtual HeatPumpMode? HeatPumpModePassengerCompartment { get { return null; } }

		public virtual ACCompressorType CompressorTypeDriver
		{
			get { return ACCompressorType.None; }
		}

		public virtual ACCompressorType CompressorTypePassenger
		{
			get { return ACCompressorType.None; }
		}

		public virtual Watt AuxHeaterPower
		{
			get { return 0.SI<Watt>(); }
		}

		public virtual bool DoubleGlazing
		{
			get { return false; }
		}

		public virtual bool HeatPump
		{
			get { return false; }
		}

		public virtual bool OtherHeatingTechnology
		{
			get { return false; }
		}

		public virtual bool AdjustableCoolantThermostat
		{
			get { return GetBool(new[] { "HVAC", "AdjustableCoolantThermostat" }); }
		}

		public virtual bool AdjustableAuxiliaryHeater
		{
			get { return false; }
		}

		public virtual bool EngineWasteGasHeatExchanger
		{
			get { return GetBool(new[] { "HVAC", "EngineWasteGasHeatExchanger" }); }
		}

		public virtual bool SeparateAirDistributionDucts
		{
			get { return false; }
		}

		public virtual bool WaterElectricHeater
		{
			get { return false; }
		}
		public virtual bool AirElectricHeater
		{
			get { return false; }
		}

		#endregion
	}

	public class XMLDeclarationCompleteBusAuxiliariesDataProviderV26 : XMLDeclarationPrimaryBusAuxiliariesDataProviderV26, IElectricConsumersDeclarationData
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "CompletedVehicleAuxiliaryDataDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationCompleteBusAuxiliariesDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		

		public override IList<string> SteeringPumpTechnology { get { return null; } }


		public override IElectricConsumersDeclarationData ElectricConsumers
		{
			get {
				return this;
			}
		}

		public override IPneumaticSupplyDeclarationData PneumaticSupply { get { return null; } }

		//public override IPneumaticConsumersDeclarationData PneumaticConsumers { get { return null; } }

		public override BusHVACSystemConfiguration SystemConfiguration
		{
			get { return BusHVACSystemConfigurationHelper.Parse(GetString(XMLNames.Bus_SystemConfiguration)); }
		}

		public override ACCompressorType CompressorTypeDriver
		{
			get { return ACCompressorTypeExtensions.ParseEnum(GetString(XMLNames.Bus_DriverAC)); }
		}

		public override ACCompressorType CompressorTypePassenger
		{
			get { return ACCompressorTypeExtensions.ParseEnum(GetString(XMLNames.Bus_PassengerAC)); }
		}

		public override Watt AuxHeaterPower
		{
			get { return GetDouble(XMLNames.Bus_AuxiliaryHeaterPower).SI<Watt>(); }
		}

		public override bool DoubleGlazing
		{
			get { return GetBool(XMLNames.Bus_DoubleGlazing); }
		}

		public override bool HeatPump
		{
			get { return GetBool(XMLNames.Bus_HeatPump); }
		}

		public override bool AdjustableCoolantThermostat
		{
			get { return GetBool(new[] { "HVAC", XMLNames.Bus_AdjustableCoolantThermostat }); }
		}

		public override bool AdjustableAuxiliaryHeater
		{
			get { return GetBool(XMLNames.Bus_AdjustableAuxiliaryHeater); }
		}

		public override bool EngineWasteGasHeatExchanger
		{
			get { return false; }
		}

		public override bool SeparateAirDistributionDucts
		{
			get { return GetBool(XMLNames.Bus_SeparateAirDistributionDucts); }
		}

		#endregion

		#region Implementation of IElectricConsumersDeclarationData

		public virtual bool InteriorLightsLED { get { return GetBool(new[] { "LEDLights", XMLNames.Bus_Interiorlights }); } }

		public virtual bool DayrunninglightsLED { get { return GetBool(new[] { "LEDLights", XMLNames.Bus_Dayrunninglights }); } }

		public virtual bool PositionlightsLED { get { return GetBool(new[] { "LEDLights", XMLNames.Bus_Positionlights }); } }

		public virtual bool HeadlightsLED { get { return GetBool(new[] { "LEDLights", XMLNames.Bus_Headlights }); } }

		public virtual bool BrakelightsLED { get { return GetBool(new[] { "LEDLights", XMLNames.Bus_Brakelights }); } }

		#endregion
	}

	public class XMLDeclarationCompleteBusAuxiliariesDataProviderV28 : XMLDeclarationCompleteBusAuxiliariesDataProviderV26
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V28;

		public new const string XSD_TYPE = "CompletedVehicleAuxiliaryDataDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationCompleteBusAuxiliariesDataProviderV28(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }


		public override HeatPumpType? HeatPumpTypeDriverCompartment
		{
			get { return HeatPumpTypeHelper.Parse(GetString(XMLNames.Bus_HeatPumpTypeDriver)); }
		}

		public override HeatPumpMode? HeatPumpModeDriverCompartment
		{
			get { return HeatPumpModeHelper.Parse(GetString(XMLNames.Bus_HeatPumpModeDriver)); }
		}

		public override HeatPumpType? HeatPumpTypePassengerCompartment
		{
			get { return HeatPumpTypeHelper.Parse(GetString(XMLNames.Bus_HeatPumpTypePassenger)); }
		}

		public override HeatPumpMode? HeatPumpModePassengerCompartment
		{
			get { return HeatPumpModeHelper.Parse(GetString(XMLNames.Bus_HeatPumpModePassenger)); }
		}

		public override bool WaterElectricHeater
		{
			get { return GetBool(XMLNames.Bus_WaterElectricHeater); }
		}

		public override bool AirElectricHeater
		{
			get { return GetBool(XMLNames.Bus_AirElectricHeater); }
		}

		public override bool OtherHeatingTechnology
		{
			get { return GetBool(XMLNames.Bus_OtherHeatingTechnology); }
		}
	}
}
