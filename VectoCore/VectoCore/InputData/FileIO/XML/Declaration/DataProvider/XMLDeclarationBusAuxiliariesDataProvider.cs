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
		IElectricSupplyDeclarationData, IResultCardDeclarationInputData, IPneumaticConsumersDeclarationData,
		IPneumaticSupplyDeclarationData
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

		public IHVACBusAuxiliariesDeclarationData HVACAux { get; }

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

		public IResultCardDeclarationInputData ResultCards
		{
			get {
				return ElementExists(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_ResultCards })
					? this
					: null;
			}
		}

		public bool SmartElectrics
		{
			get { return GetBool(new[] { XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_SmartElectrics }); }
		}

		#endregion

		#region Implementation of IResultCardDeclarationInputData

		public IList<IResultCardEntry> Idle
		{
			get {
				return GetNodes(
						new[] {
							XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_ResultCards, XMLNames.BusAux_ResultCard_Idle,
							XMLNames.BusAux_ResultCard_Entry
						})
					.Cast<XmlNode>().Select(
						x => new ResultCardEntry(
							GetAttribute(x, XMLNames.ResultCard_Entry_CurrentAttr).ToDouble().SI<Ampere>(),
							GetAttribute(x, XMLNames.ResultCard_Entry_SmartCurrent_Attr).ToDouble().SI<Ampere>()))
					.Cast<IResultCardEntry>()
					.ToList();
			}
		}

		public IList<IResultCardEntry> Traction
		{
			get {
				return GetNodes(
						new[] {
							XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_ResultCards, XMLNames.BusAux_ResultCard_Traction,
							XMLNames.BusAux_ResultCard_Entry
						})
					.Cast<XmlNode>().Select(
						x => new ResultCardEntry(
							GetAttribute(x, XMLNames.ResultCard_Entry_CurrentAttr).ToDouble().SI<Ampere>(),
							GetAttribute(x, XMLNames.ResultCard_Entry_SmartCurrent_Attr).ToDouble().SI<Ampere>()))
					.Cast<IResultCardEntry>()
					.ToList();
			}
		}

		public IList<IResultCardEntry> Overrun
		{
			get {
				return GetNodes(
						new[] {
							XMLNames.BusAux_ElectricSystem, XMLNames.BusAux_ElectricSystem_ResultCards, XMLNames.BusAux_ResultCard_Overrun,
							XMLNames.BusAux_ResultCard_Entry
						})
					.Cast<XmlNode>().Select(
						x => new ResultCardEntry(
							GetAttribute(x, XMLNames.ResultCard_Entry_CurrentAttr).ToDouble().SI<Ampere>(),
							GetAttribute(x, XMLNames.ResultCard_Entry_SmartCurrent_Attr).ToDouble().SI<Ampere>()))
					.Cast<IResultCardEntry>()
					.ToList();
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
	}
}
