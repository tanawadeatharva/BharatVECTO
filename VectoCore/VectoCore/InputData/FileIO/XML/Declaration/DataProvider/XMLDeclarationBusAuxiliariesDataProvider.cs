using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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

		public IElectricSupplyDeclarationData ElectricSupply { get; }
		public IElectricConsumersDeclarationData ElectricConsumers { get; }
		public IPneumaticSupplyDeclarationData PneumaticSupply { get; }
		public IPneumaticConsumersDeclarationData PneumaticConsumers { get; }
		public IHVACBusAuxiliariesDeclarationData HVACAux { get; }

		#endregion
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

				if (alternators.Count > 0) {
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