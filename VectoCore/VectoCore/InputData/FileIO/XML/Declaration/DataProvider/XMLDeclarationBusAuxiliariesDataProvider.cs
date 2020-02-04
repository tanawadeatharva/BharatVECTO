using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider {
	public class XMLDeclarationBusAuxiliariesDataProviderV26 : AbstractXMLType, IXMLBusAuxiliariesDeclarationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public const string XSD_TYPE = "PrimaryVehicleAuxiliaryDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private const string RATIO_ATTRIBUTE = "ratio";
		private const string CURRENT_ATTRIBUTE = "current";
		private const string SMART_CURRENT_ATTRIBUTE = "smartCurrent";
		private const string IDLE_NODE_NAME = "Idle";
		private const string TRANSACTION_NODE_NAME = "Traction";
		private const string OVERRUN_NODE_NAME = "Overrun";
		

		public XMLDeclarationBusAuxiliariesDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		public XmlNode XMLSource { get { return BaseNode; } }

		public string FanTechnology
		{
			get { return GetNode(new[] { "Fan", XMLNames.Auxiliaries_Auxiliary_Technology }).InnerText; }
		}

		public IList<string> SteeringPumpTechnology
		{
			get { return GetNodes(new[] { "SteeringPump", XMLNames.Auxiliaries_Auxiliary_Technology }).Cast <XmlNode>().Select(x => x.InnerText).ToList(); }
		}

		public IElectricSupplyDeclarationData ElectricSupply
		{
			get
			{
				return new ElectricSupplyDeclarationData {
					Alternators = ReadAlternators(),
					ResultCards = ReadResultCards(),
					SmartElectrics = GetBool(XMLNames.Bus_Smart_Electrics)
			}; 
			}
		}

		public IElectricConsumersDeclarationData ElectricConsumers { get; }

		public IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get
			{
				return new PneumaticSupplyDeclarationData {
					Clutch =  GetString(XMLNames.Vehicle_Clutch),
					CompressorSize = GetString(XMLNames.Bus_SizeOfAirSupply),
					Ratio = GetDouble(XMLNames.Bus_CompressorRatio),
					SmartAirCompression = GetBool(XMLNames.Bus_SmartCompressionSystem),
					SmartRegeneration = GetBool(XMLNames.Bus_SmartRegenerationSystem)
				};
			}
		}

		public IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get {
				return new PneumaticConsumersDeclarationData {
					AirsuspensionControl = ConsumerTechnologyHelper.Parse(GetString(XMLNames.Bus_AirsuspensionControl)),
					AdBlueDosing = GetBool(XMLNames.Bus_AdBlueDosing) ? ConsumerTechnology.Pneumatically : ConsumerTechnology.Electrically,
					DoorDriveTechnology = ConsumerTechnologyHelper.Parse(GetString(XMLNames.Bus_DoorDriveTechnology))
				};

			}
		}

		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get
			{
				return new HVACBusAuxiliariesDeclarationData {
					AdjustableCoolantThermostat = GetBool(XMLNames.Bus_AdjustableCoolantThermostat),
					EngineWasteGasHeatExchanger = GetBool(XMLNames.Bus_EngineWasteGasHeatExchanger),
				};
			}
		}

		#endregion


		private List<IAlternatorDeclarationInputData> ReadAlternators()
		{
			var alternators = GetNodes(XMLNames.Bus_AlternatorTechnology);
			if (alternators.IsNullOrEmpty())
				return null;

			var currentAlternators = new List<IAlternatorDeclarationInputData>();
			for (int i = 0; i < alternators.Count; i++)
			{
				var technology = alternators[i]?.InnerText;
				var ratio = alternators[i]?.Attributes?[RATIO_ATTRIBUTE].Value.ToDouble();
				if (ratio == null)
					continue;
				currentAlternators.Add(new AlternatorInputData(technology, (double)ratio));
			}

			return currentAlternators;
		}

		private IResultCardDeclarationInputData ReadResultCards()
		{
			var resultCards = GetNodes(XMLNames.Bus_ResultCards);
			if (resultCards.IsNullOrEmpty())
				return null;
			
			var idles = new List<IResultCardEntry>();
			var tractions = new List<IResultCardEntry>();
			var overruns = new List<IResultCardEntry>();

			for (int i = 0; i < resultCards[0].ChildNodes.Count; i++)
			{

				foreach (XmlNode entry in resultCards[0].ChildNodes[i])
				{
					var current = GetAttribute(entry, CURRENT_ATTRIBUTE).ToDouble().SI<Ampere>();
					var smartCurrent = GetAttribute(entry, SMART_CURRENT_ATTRIBUTE).ToDouble().SI<Ampere>();

					if (entry?.ParentNode?.Name == IDLE_NODE_NAME)
						idles.Add(new ResultCardEntry(current, smartCurrent));
					if (entry?.ParentNode?.Name == TRANSACTION_NODE_NAME)
						tractions.Add(new ResultCardEntry(current, smartCurrent));
					if (entry?.ParentNode?.Name == OVERRUN_NODE_NAME)
						overruns.Add(new ResultCardEntry(current, smartCurrent));
				}
			}

			if (idles.Count > 0 || tractions.Count > 0 || overruns.Count > 0)
			{
				return new ResultCardDeclarationInputData
				{
					Idle = idles,
					Overrun = overruns,
					Traction = tractions
				};
			}

			return null;
		}


	}

	public class XMLDeclarationCompleteBusAuxiliariesDataProviderV26 : AbstractXMLType, IXMLBusAuxiliariesDeclarationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public const string XSD_TYPE = "CompletedVehicleAuxiliaryDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationCompleteBusAuxiliariesDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		public XmlNode XMLSource { get { return BaseNode; } }

		public string FanTechnology { get { return null; } }

		public IList<string> SteeringPumpTechnology { get { return null; } }


		public IElectricSupplyDeclarationData ElectricSupply
		{
			get
			{
				var alternators = GetNodes(XMLNames.Bus_AlternatorTechnology);

				if (alternators?.Count > 0) {
					var currentAlternators = new List<IAlternatorDeclarationInputData>();
					for (int i = 0; i < alternators.Count; i++) {
						var technology =  alternators[i]?.InnerText;
						var ratio = alternators[i]?.Attributes?["ratio"].Value.ToDouble();
						if(ratio == null)
							continue;
						currentAlternators.Add(new AlternatorInputData(technology, (double)ratio ));
					}

					return new ElectricSupplyDeclarationData {Alternators = currentAlternators};
				}

				return null;
			}
		}

		public IElectricConsumersDeclarationData ElectricConsumers
		{
			get
			{
				return new ElectricConsumersDeclarationData
				{
					DayrunninglightsLED = GetBool(XMLNames.Bus_Dayrunninglights),
					HeadlightsLED =  GetBool(XMLNames.Bus_Headlights),
					PositionlightsLED = GetBool(XMLNames.Bus_Positionlights),
					BrakelightsLED = GetBool(XMLNames.Bus_Brakelights),
					InteriorLightsLED = GetBool(XMLNames.Bus_Interiorlights)
				};
			}
		}

		public IPneumaticSupplyDeclarationData PneumaticSupply { get { return null; } }
	
		public IPneumaticConsumersDeclarationData PneumaticConsumers { get { return null;} }

		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get
			{
				var hvac = new HVACBusAuxiliariesDeclarationData
				{
					SystemConfiguration = XmlConvert.ToInt32(GetString(XMLNames.Bus_SystemConfiguration)),
					CompressorType = new CompressorType( GetString(XMLNames.Bus_DriverAC), GetString(XMLNames.Bus_PassengerAC)),
					AuxHeaterPower = XmlConvert.ToInt32(GetString(XMLNames.Bus_AuxiliaryHeaterPower)),
					DoubleGlasing = GetBool(XMLNames.Bus_DoubleGlasing),
					HeatPump = GetBool(XMLNames.Bus_HeatPump),
					AdjustableAuxiliaryHeater = GetBool(XMLNames.Bus_AdjustableAuxiliaryHeater),
					SeparateAirDistributionDucts = GetBool(XMLNames.Bus_SeparateAirDistributionDucts)
				};
				return hvac;
			}
		}

		#endregion
	}
}