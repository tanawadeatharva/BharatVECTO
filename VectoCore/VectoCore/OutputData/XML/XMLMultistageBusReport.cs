using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.OutputData.XML
{
	public interface IXMLMultistageReport
	{
		void Initialize(VectoRunData modelData);
		XDocument Report { get; }
		void GenerateReport();
	}

	public class XMLMultistageBusReport: IXMLMultistageReport 
	{
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected XNamespace v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.3";
		protected XNamespace v28 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.8";
		protected XNamespace v10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";

		private XElement _primaryVehicle;
		private List<XElement> _manufacturingStages;
		private List<XAttribute> _namespaceAttributes;
		
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicleInputData;
		private IList<IManufacturingStageInputData> _manufacturingStageInputData;
		private IManufacturingStageInputData _consolidatedInputData;
		private IVehicleDeclarationInputData _vehicleInputData;


		public XDocument Report { get; protected set; }

		public XMLMultistageBusReport()
		{
			_manufacturingStages = new List<XElement>();
			_namespaceAttributes = new List<XAttribute>();
		}
		
		public void Initialize(VectoRunData modelData)
		{
			_primaryVehicleInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.PrimaryVehicle;
			_manufacturingStageInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.ManufacturingStages;
			_consolidatedInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage;
			_vehicleInputData = modelData.MultistageVIFInputData.VehicleInputData;

			SetInputXMLData(_primaryVehicleInputData.Vehicle.XMLSource);
		}

		#region Set current VIF Data
		
		private void SetInputXMLData(XmlNode primeVehicleNode)
		{
			var nodes = GetDocumentNodes(primeVehicleNode);
			var documentNode = GetDocumentNode(nodes);
			SetXmlNamespaceAttributes(nodes);
			
			var xDocument = XElement.Parse(documentNode.InnerXml);
			foreach (var xElement in xDocument.Descendants())
			{
				if(xElement.Name.LocalName == XMLNames.Bus_PrimaryVehicle)
					_primaryVehicle = xElement;
				else if (xElement.Name.LocalName == XMLNames.ManufacturingStage)
					_manufacturingStages.Add(xElement);
			}

			if (_manufacturingStages.Count == 0)
				_manufacturingStages = null;
		}


		private List<XmlNode> GetDocumentNodes(XmlNode primeVehicleNode)
		{
			var nodes = new List<XmlNode>();
			NodeParentSearch(primeVehicleNode, nodes);
			return nodes;
		}

		private XmlNode GetDocumentNode(List<XmlNode> nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return null;

			foreach (var node in nodes)
			{
				if (node.NodeType == XmlNodeType.Document)
					return node;
			}
			return null;
		}

		private void SetXmlNamespaceAttributes(List<XmlNode> nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return;
			XmlAttributeCollection namespaceAttributes = null;
			foreach (var node in nodes) {
				if (node.LocalName == XMLNames.VectoOutputMultistage) {
					namespaceAttributes = node.Attributes;
					break;
				}
			}

			if (namespaceAttributes == null || namespaceAttributes.Count == 0)
				return;

			foreach (XmlAttribute attribute in namespaceAttributes) {
				if (string.IsNullOrEmpty(attribute.Prefix)) 
					_namespaceAttributes.Add(new XAttribute(attribute.LocalName, attribute.Value));
				else if(attribute.Prefix == "xmlns")
					_namespaceAttributes.Add(new XAttribute(XNamespace.Xmlns + attribute.LocalName, attribute.Value));
				else if(attribute.Prefix == "xsi")
					_namespaceAttributes.Add(new XAttribute(xsi + attribute.LocalName, attribute.Value));
			}
		}
		
		private void NodeParentSearch(XmlNode currentNode, List<XmlNode> nodes)
		{
			if (currentNode?.ParentNode == null || nodes == null)
				return;

			nodes.Add(currentNode.ParentNode);
			NodeParentSearch(currentNode.ParentNode, nodes);
		}



		#endregion

		public void GenerateReport()
		{
			var retVal = new XDocument();
			retVal.Add(
				new XElement(tns + XMLNames.VectoOutputMultistage,
					_namespaceAttributes,
					_primaryVehicle,
					_manufacturingStages,
					GenerateInputManufacturingStage()
				)
			);
			Report = retVal;
		}

		#region Generate new manfuacturing Stage

		private XElement GenerateInputManufacturingStage()
		{
			var multistageId = $"{VectoComponents.VectoManufacturingStage.HashIdPrefix()}{GetGUID()}";
			var vehicleId = $"{VectoComponents.Vehicle.HashIdPrefix()}{GetGUID()}";

			return new XElement(tns + XMLNames.ManufacturingStage,
					new XAttribute("stageCount", GetStageNumber()),
					new XElement(tns + XMLNames.Report_DataWrap,
						new XAttribute(xsi + XMLNames.Attr_Type, "BusManufacturingStageDataType"),
						new XAttribute(XMLNames.Component_ID_Attr, multistageId),
						GetHashPreviousStageElement(),
						GetVehicleElement(vehicleId),
						GetApplicationInformation()),
					GetInputdataSignature(multistageId));
		}
		
		private int GetStageNumber()
		{
			if (_manufacturingStageInputData == null || _manufacturingStageInputData.Count == 0)
				return 2;

			return _manufacturingStageInputData.Last().StageCount + 1;
		}

		private XElement GetHashPreviousStageElement()
		{
			DigestData digitData;
			if (_manufacturingStageInputData == null || _manufacturingStageInputData.Count == 0) {
				digitData = _primaryVehicleInputData.VehicleSignatureHash;
			} else {
				digitData = _manufacturingStageInputData.Last().Signature;
			}

			return new XElement(tns + "HashPreviousStage",
				   digitData.ToXML(di));
		}

		
		private XElement GetVehicleElement(string vehicleId)
		{
			return new XElement(tns + XMLNames.Tag_Vehicle,
				new XAttribute(xsi + XMLNames.Attr_Type, "v2.8:InterimStageInputType"),
				new XAttribute(XMLNames.Component_ID_Attr, vehicleId),
				new XElement(v28 + XMLNames.Component_Manufacturer, _vehicleInputData.Manufacturer),
				new XElement(v28 + XMLNames.Component_ManufacturerAddress, _vehicleInputData.ManufacturerAddress),
				new XElement(v28 + XMLNames.Vehicle_VIN, _vehicleInputData.VIN),
				new XElement(v28 + XMLNames.Component_Date,
					XmlConvert.ToString(_vehicleInputData.Date, XmlDateTimeSerializationMode.Utc)),
				_vehicleInputData.Model != null
					? new XElement(v28 + XMLNames.Component_Model, _vehicleInputData.Model) : null,
				_vehicleInputData.LegislativeClass != null
					? new XElement(v28 + XMLNames.Bus_LegislativeCategory, _vehicleInputData.LegislativeClass.ToXMLFormat()) : null,
				_vehicleInputData.CurbMassChassis != null
					? new XElement(v28 + XMLNames.Bus_CorrectedActualMass, _vehicleInputData.CurbMassChassis.ToXMLFormat(0)) : null,
				_vehicleInputData.GrossVehicleMassRating != null
					? new XElement(v28 + XMLNames.TPMLM, _vehicleInputData.GrossVehicleMassRating.ToXMLFormat(0)) : null,
				GetAirdragModifiedMultistageEntry(),
				_vehicleInputData.TankSystem != null 
					? new XElement(v28 + XMLNames.Vehicle_NgTankSystem, _vehicleInputData.TankSystem.ToString()) : null,
				_vehicleInputData.RegisteredClass != null
					? new XElement(v28 + XMLNames.Vehicle_RegisteredClass, _vehicleInputData.RegisteredClass.ToXMLFormat()) : null,
				_vehicleInputData.NumberOfPassengersLowerDeck != null 
					? new XElement(v28 + XMLNames.Bus_NumberPassengerSeatsLowerDeck, _vehicleInputData.NumberOfPassengersLowerDeck) : null,
				_vehicleInputData.NumberOfPassengersStandingLowerDeck != null
					? new XElement(v28 + XMLNames.Bus_NumberPassengersStandingLowerDeck, _vehicleInputData.NumberOfPassengersStandingLowerDeck) : null,
				_vehicleInputData.NumberOfPassengersUpperDeck != null
					? new XElement(v28 + XMLNames.Bus_NumberPassengerSeatsUpperDeck, _vehicleInputData.NumberOfPassengersUpperDeck) : null,
				_vehicleInputData.NumberOfPassengersStandingUpperDeck != null
					? new XElement(v28 + XMLNames.Bus_NumberPassengersStandingUpperDeck, _vehicleInputData.NumberOfPassengersStandingUpperDeck) : null,
				_vehicleInputData.VehicleCode != null
					? new XElement(v28 + XMLNames.Vehicle_BodyworkCode, _vehicleInputData.VehicleCode.ToXMLFormat()) : null,
				_vehicleInputData.LowEntry != null
					? new XElement(v28 + XMLNames.Bus_LowEntry, _vehicleInputData.LowEntry) : null,
				_vehicleInputData.Height != null
					? new XElement(v28 + XMLNames.Bus_HeighIntegratedBody, _vehicleInputData.Height.ConvertToMilliMeter().ToXMLFormat(0)) : null,
				_vehicleInputData.Length != null
					? new XElement(v28 + XMLNames.Bus_VehicleLength, _vehicleInputData.Length.ConvertToMilliMeter().ToXMLFormat(0)) : null,
				_vehicleInputData.Width != null 
					? new XElement(v28 + XMLNames.Bus_VehicleWidth, _vehicleInputData.Width.ConvertToMilliMeter().ToXMLFormat(0)) : null,
				_vehicleInputData.EntranceHeight != null 
					? new XElement(v28 + XMLNames.Bus_EntranceHeight, _vehicleInputData.EntranceHeight.ConvertToMilliMeter().ToXMLFormat(0)) : null,
				_vehicleInputData.DoorDriveTechnology != null
					? new XElement(v28 + XMLNames.Bus_DoorDriveTechnology, _vehicleInputData.DoorDriveTechnology.ToXMLFormat()) : null,
				new XElement(v28 + XMLNames.Bus_VehicleDeclarationType, _vehicleInputData.VehicleDeclarationType.GetLabel()),
				GetADAS(_vehicleInputData.ADAS),
				GetBusVehicleComponents(_vehicleInputData.Components)
			);
		}

		private XElement GetAirdragModifiedMultistageEntry()
		{
			if (_consolidatedInputData?.Vehicle?.Components?.AirdragInputData == null)
				return null;

			switch (_vehicleInputData.AirdragModifiedMultistage) {
				case null:
					throw new VectoException("AirdragModifiedMultistage must be set if an airdrag component has been set in previous stages.");
				case false:
					return new XElement(v28 + XMLNames.Bus_AirdragModifiedMultistage, _vehicleInputData.AirdragModifiedMultistage);
			}
			return null;
		}

		private XElement GetADAS(IAdvancedDriverAssistantSystemDeclarationInputData adasData)
		{
			if (adasData == null)
				return null;
			
			return new XElement(
				v28 + XMLNames.Vehicle_ADAS,
				new XElement(v23 + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
				new XElement(v23 + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
				new XElement(v23 + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
				new XElement(v23 + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl.ToXMLFormat()),
				adasData.ATEcoRollReleaseLockupClutch != null
					? new XElement(v23 + XMLNames.Bus_ADAS_APTEcoRollReleaseLockupClutch, adasData.ATEcoRollReleaseLockupClutch)
					: null
			);
		}

		private XElement GetBusVehicleComponents(IVehicleComponentsDeclaration vehicleComponents)
		{
			var busAirdrag = GetBusAirdrag(vehicleComponents?.AirdragInputData);
			var busAux = GetBusAuxiliaries(vehicleComponents?.BusAuxiliaries);

			if (busAirdrag == null && busAux == null)
				return null;
			
			return new XElement(v28 + XMLNames.Vehicle_Components,
				new XAttribute(XNamespace.Xmlns + "v2.8", v28),
				new XAttribute(xsi + XMLNames.Attr_Type, "v2.8:CompletedVehicleComponentsDeclarationType"),
				busAirdrag,
				busAux
			);
		}

		private XElement GetBusAirdrag(IAirdragDeclarationInputData airdrag)
		{
			if (airdrag != null) 
				return GetAirdragElement(airdrag);

			return _vehicleInputData.AirdragModifiedMultistage == true
				? GetBusAirdragUseStandardValues()
				: null;
		}
		
		private XElement GetAirdragElement(IAirdragDeclarationInputData airdrag)
		{
			if (airdrag == null)
				return null;
			
			XmlNode airdragNode = null;
			if (airdrag is AbstractCommonComponentType)
				airdragNode = (airdrag as AbstractCommonComponentType).XMLSource;
			else {
				var type = airdrag.GetType();
				var property = type.GetProperty(nameof(AbstractCommonComponentType.XMLSource));
				if (property != null)
					airdragNode = (XmlNode)property.GetValue(airdrag, null);
			}

			if (airdragNode == null)
				return null;
			
			var dataNode = airdragNode.SelectSingleNode(".//*[local-name()='Data']");
			var signatureNode = airdragNode.SelectSingleNode(".//*[local-name()='Signature']");

			if (dataNode == null)
				throw new VectoException("Data node of airdrag component is null!");
			if(signatureNode == null) 
				throw new VectoException("Signature node of the given airdrag component is null!");

			return dataNode.NamespaceURI == v10.NamespaceName
				? GetAirdragXMLElementV1(dataNode, signatureNode)
				: GetAirdragXMLElementV2(dataNode, signatureNode);
		}

		private XElement GetAirdragXMLElementV1(XmlNode dataNode, XmlNode signatureNode)
		{
			return new XElement(v28 + XMLNames.Component_AirDrag,
				new XElement(v20 + XMLNames.Report_DataWrap,
					new XAttribute(xsi + XMLNames.Component_Type_Attr, XMLNames.AirDrag_Data_Type_Attr),
					new XAttribute("xmlns", v10.NamespaceName),
					dataNode.Attributes != null && dataNode.Attributes[XMLNames.Component_ID_Attr] != null ?
						new XAttribute(XMLNames.Component_ID_Attr, dataNode.Attributes[XMLNames.Component_ID_Attr].InnerText) : null,
					GetElements(dataNode.ChildNodes)
				),
				new XElement(v20 + XMLNames.DI_Signature,
					GetElements(signatureNode.ChildNodes)
				)
			);
		}

		private XElement GetAirdragXMLElementV2(XmlNode dataNode, XmlNode signatureNode)
		{
			var dataElement = XElement.Parse(dataNode.OuterXml);
			var signatureElement = XElement.Parse(signatureNode.OuterXml);
			dataElement.Attribute(XNamespace.Xmlns + "xsi")?.Remove();

			return new XElement(v28 + XMLNames.Component_AirDrag,
				dataElement,
				signatureElement);
		}
		
		private List<XElement> GetElements(XmlNodeList list)
		{
			var res = new List<XElement>();

			foreach (XmlNode node in list) {
				var element = XElement.Parse(node.OuterXml);
				element.Attribute("xmlns")?.Remove();
				res.Add(element);
			}

			return res;
		}
		
		private XElement GetBusAirdragUseStandardValues()
		{
			var id = $"{VectoComponents.Airdrag.HashIdPrefix()}{GetGUID()}";

			return new XElement(v28 + XMLNames.Component_AirDrag,
				new XElement(v20 + XMLNames.ComponentDataWrapper,
					new XAttribute(XNamespace.Xmlns + "v2.0", v20),
					new XAttribute(xsi + XMLNames.Attr_Type, "v2.8:AirDragModifiedUseStandardValueType"),
					new XAttribute(XMLNames.Component_ID_Attr, id)
				),
				new XElement(v20 + XMLNames.DI_Signature, XMLHelper.CreateDummySig(di)));
		}


		public XElement GetBusAuxiliaries(IBusAuxiliariesDeclarationData busAux)
		{
			if (busAux == null)
				return null;

			var electricSystemEntry = GetElectricSystem(busAux.ElectricConsumers);
			var hvacEntry = GetHVAC(busAux.HVACAux);

			if (electricSystemEntry == null && hvacEntry == null)
				return null;

			return new XElement(v28 + XMLNames.Component_Auxiliaries,

					new XElement(v28 + XMLNames.ComponentDataWrapper,
						new XAttribute(xsi + XMLNames.Attr_Type, "v2.8:CompletedVehicleAuxiliaryDataDeclarationType"),
						electricSystemEntry != null
							? GetElectricSystem(busAux.ElectricConsumers) : null,
						hvacEntry != null
							? GetHVAC(busAux.HVACAux) : null
					));
		}


		private XElement GetElectricSystem(IElectricConsumersDeclarationData electricConsumer)
		{
			if (electricConsumer == null)
				return null;

			if (electricConsumer.InteriorLightsLED == null && electricConsumer.DayrunninglightsLED == null &&
				electricConsumer.PositionlightsLED == null && electricConsumer.BrakelightsLED == null &&
				electricConsumer.HeadlightsLED == null)
				return null;
			
			return new XElement(v28 + XMLNames.BusAux_ElectricSystem,
				new XElement(v28 + XMLNames.BusAux_LEDLights,
					electricConsumer.InteriorLightsLED != null 
						? new XElement(v28 + XMLNames.Bus_Interiorlights, electricConsumer.InteriorLightsLED) : null,
					electricConsumer.DayrunninglightsLED != null
						? new XElement(v28 + XMLNames.Bus_Dayrunninglights, electricConsumer.DayrunninglightsLED) : null,
					electricConsumer.PositionlightsLED != null
						? new XElement(v28 + XMLNames.Bus_Positionlights, electricConsumer.PositionlightsLED) : null,
					electricConsumer.BrakelightsLED != null
						? new XElement(v28 + XMLNames.Bus_Brakelights, electricConsumer.BrakelightsLED) : null,
					electricConsumer.HeadlightsLED != null
						? new XElement(v28 + XMLNames.Bus_Headlights, electricConsumer.HeadlightsLED) : null
				));
		}

		private XElement GetHVAC(IHVACBusAuxiliariesDeclarationData hvac)
		{
			if (hvac == null)
				return null;

			if (hvac.SystemConfiguration == null &&
				hvac.HeatPumpModeDriverCompartment == null && hvac.HeatPumpTypeDriverCompartment == null &&
				hvac.HeatPumpModePassengerCompartments?.Any() != true && hvac.HeatPumpTypePassengerCompartments?.Any() != true &&
				hvac.AuxHeaterPower == null && hvac.DoubleGlazing == null && hvac.AdjustableAuxiliaryHeater == null &&
				hvac.SeparateAirDistributionDucts == null && hvac.WaterElectricHeater == null &&
				hvac.AirElectricHeater == null &&
				hvac.OtherHeatingTechnology == null)
				return null;

			return new XElement(v28 + XMLNames.BusAux_HVAC,
				hvac.SystemConfiguration != null
					? new XElement(v28 + XMLNames.Bus_SystemConfiguration, hvac.SystemConfiguration.GetXmlFormat()) : null,
				hvac.HeatPumpTypeDriverCompartment != null
					? new XElement(v28 + XMLNames.Bus_HeatPumpTypeDriver, hvac.HeatPumpTypeDriverCompartment.GetLabel()) : null,
				hvac.HeatPumpModeDriverCompartment != null
					? new XElement(v28 + XMLNames.Bus_HeatPumpModeDriver, hvac.HeatPumpModeDriverCompartment.GetLabel()) : null,
				GetHeatPumpPassengerCompartments(hvac.HeatPumpTypePassengerCompartments, hvac.HeatPumpModePassengerCompartments),
				hvac.AuxHeaterPower != null
					? new XElement(v28 + XMLNames.Bus_AuxiliaryHeaterPower, hvac.AuxHeaterPower.ToXMLFormat(0)) : null,
				hvac.DoubleGlazing != null
					? new XElement(v28 + XMLNames.Bus_DoubleGlazing, hvac.DoubleGlazing) : null,
				hvac.AdjustableAuxiliaryHeater != null
					? new XElement(v28 + XMLNames.Bus_AdjustableAuxiliaryHeater, hvac.AdjustableAuxiliaryHeater) : null,
				hvac.SeparateAirDistributionDucts != null
					? new XElement(v28 + XMLNames.Bus_SeparateAirDistributionDucts, hvac.SeparateAirDistributionDucts) : null,
				hvac.WaterElectricHeater != null
					? new XElement(v28 + XMLNames.Bus_WaterElectricHeater, hvac.WaterElectricHeater) : null,
				hvac.AirElectricHeater != null
					? new XElement(v28 + XMLNames.Bus_AirElectricHeater, hvac.AirElectricHeater) : null,
				hvac.OtherHeatingTechnology != null
					? new XElement(v28 + XMLNames.Bus_OtherHeatingTechnology, hvac.OtherHeatingTechnology) : null
			);
		}
		
		private IList<XElement> GetHeatPumpPassengerCompartments(IList<HeatPumpType> pumpTypes, IList<HeatPumpMode> pumpModes)
		{
			if (pumpTypes?.Any() != true || pumpModes?.Any() != true)
				return null;

			if (pumpTypes.Count != pumpModes.Count)
				return null;

			var result = new List<XElement>();

			for (int i = 0; i < pumpTypes.Count; i++) {
				result.Add(new XElement(v28 + XMLNames.Bus_HeatPumpTypePassenger, pumpTypes[i].GetLabel()));
				result.Add(new XElement(v28 + XMLNames.Bus_HeatPumpModePassenger, pumpModes[i].GetLabel()));
			}

			return result;
		}


		private XElement GetApplicationInformation()
		{
			return new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, VectoSimulationCore.VersionNumber),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		private XElement GetInputdataSignature(string multistageId)
		{
			return new XElement(tns + XMLNames.DI_Signature,
					new XElement(di + XMLNames.DI_Signature_Reference,
						new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, $"#{multistageId}"),
						new XElement(di + XMLNames.DI_Signature_Reference_Transforms,
							new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
								new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "urn:vecto:xml:2017:canonicalization")),
							new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
								new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "http://www.w3.org/2001/10/xml-exc-c14n#"))
						),
						new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "http://www.w3.org/2001/04/xmlenc#sha256")),
						new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, GetDigestValue(multistageId)))
				
				);
		}

		private string GetGUID()
		{
			return Guid.NewGuid().ToString("n").Substring(0, 20);
		}

		private string GetDigestValue(string multistageId)
		{
			var alg = SHA256.Create();
			alg.ComputeHash(Encoding.UTF8.GetBytes(multistageId));
			return Convert.ToBase64String(alg.Hash); 
		}


		#endregion
	}
}
