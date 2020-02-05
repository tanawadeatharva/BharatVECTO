using System.Collections.Generic;
using System.Linq;
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

		public XmlNode XMLSource
		{
			get { return BaseNode; }
		}

		public string FanTechnology
		{
			get { return GetNode(new[] { "Fan", XMLNames.Auxiliaries_Auxiliary_Technology }).InnerText; }
		}

		public IList<string> SteeringPumpTechnology
		{
			get {
				return GetNodes(new[] { "SteeringPump", XMLNames.Auxiliaries_Auxiliary_Technology })
					.Cast<XmlNode>().Select(x => x.InnerText).ToList();
			}
		}

		public IElectricSupplyDeclarationData ElectricSupply
		{
			get { return this; }
		}

		public IElectricConsumersDeclarationData ElectricConsumers
		{
			get { return null; }
		}

		public IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get { return this; }
		}

		public IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get { return this; }
		}

		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get { return this; }
		}

		#endregion

		#region Implementation of IElectricSupplyDeclarationData

		public IList<IAlternatorDeclarationInputData> Alternators
		{
			get {
				return GetNodes(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_AlternatorTechnology })
					.Cast<XmlNode>().Select(
						x => new AlternatorInputData(
							x.InnerText, GetAttribute(x, XMLNames.BusAux_ElectricSystem_Alternator_Ratio_Attr).ToDouble()))
					.Cast<IAlternatorDeclarationInputData>().ToList();
			}
		}

		public bool SmartElectrics
		{
			get { return GetBool(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_SmartElectrics }); }
		}

		public Watt MaxAlternatorPower
		{
			get { return ElementExists("MaxAlternatorPower") ? GetDouble("MaxAlternatorPower").SI<Watt>() : null; }
		}

		public WattSecond ElectricStorageCapacity
		{
			get {
				return ElementExists("ElectricStorageCapacity")
					? GetDouble("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>()
					: null;
			}
		}

		#endregion


		#region Implementation of IPneumaticConsumersDeclarationData

		public ConsumerTechnology AirsuspensionControl
		{
			get {
				return ConsumerTechnologyHelper.Parse(
					GetString(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_AirsuspensionControl }));
			}
		}

		public ConsumerTechnology AdBlueDosing
		{
			get {
				return ConsumerTechnologyHelper.Parse(
					GetString(
						new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_AdBlueDosing }));
			}
		}

		public ConsumerTechnology DoorDriveTechnology
		{
			get {
				return ConsumerTechnologyHelper.Parse(
					GetString(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology }));
			}
		}

		#endregion

		#region Implementation of IPneumaticSupplyDeclarationData

		public double Ratio
		{
			get { return GetDouble(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_CompressorRatio }); }
		}

		public string CompressorSize
		{
			get { return GetString(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_CompressorSize }); }
		}

		public bool SmartAirCompression
		{
			get {
				return GetBool(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem });
			}
		}

		public bool SmartRegeneration
		{
			get {
				return GetBool(new[] { XMLNames.BusAux_PneumaticSystem, XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem });
			}
		}

		#endregion

		#region Implementation of IHVACBusAuxiliariesDeclarationData

		public BusHVACSystemConfiguration SystemConfiguration
		{
			get { return BusHVACSystemConfiguration.Unknown; }
		}

		public ACCompressorType CompressorTypeDriver
		{
			get { return ACCompressorType.None; }
		}

		public ACCompressorType CompressorTypePassenger
		{
			get { return ACCompressorType.None; }
		}

		public Watt AuxHeaterPower
		{
			get { return 0.SI<Watt>(); }
		}

		public bool DoubleGlasing
		{
			get { return false; }
		}

		public bool HeatPump
		{
			get { return false; }
		}

		public bool AdjustableCoolantThermostat
		{
			get { return GetBool(new[] { "HVAC", "AdjustableCoolantThermostat" }); }
		}

		public bool AdjustableAuxiliaryHeater
		{
			get { return false; }
		}

		public bool EngineWasteGasHeatExchanger
		{
			get { return GetBool(new[] { "HVAC", "EngineWasteGasHeatExchanger" }); }
		}

		public bool SeparateAirDistributionDucts
		{
			get { return false; }
		}

		#endregion
	}
}
