using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
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

		private XElement _primaryVehicle;
		private List<XElement> _manufacturingStages;
		private List<XAttribute> _namespaceAttributes;
		
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicleInputData;
		private IList<IManufacturingStageInputData> _manufacturingStageInputData;
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
			_vehicleInputData = modelData.MultistageVIFInputData.VehicleInputData;

			SetInputXMLData(_primaryVehicleInputData.Vehicle.XMLSource);
		}

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
				_namespaceAttributes.Add(string.IsNullOrEmpty(attribute.Prefix)
					? new XAttribute(attribute.LocalName, attribute.Value)
					: new XAttribute(XNamespace.Xmlns + attribute.LocalName, attribute.Value));
			}
		}
		
		private void NodeParentSearch(XmlNode currentNode, List<XmlNode> nodes)
		{
			if (currentNode?.ParentNode == null || nodes == null)
				return;

			nodes.Add(currentNode.ParentNode);
			NodeParentSearch(currentNode.ParentNode, nodes);
		}


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


		private XElement GenerateInputManufacturingStage()
		{
			return new XElement(tns + XMLNames.ManufacturingStage,
					new XAttribute("stageCount", GetStageNumber()),
					new XElement(tns + XMLNames.Report_DataWrap,
						new XAttribute(xsi + XMLNames.Attr_Type, "BusManufacturingStageDataType"),
						new XAttribute("id", "AddContent"),
						GetHashPreviousStageElement(),
						GetVehicleElement(),
						GetApplicationInformation()),
					GetInputdataSignature());
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


		private XElement GetVehicleElement()
		{
			return new XElement(tns + XMLNames.Tag_Vehicle,
				new XAttribute(xsi + XMLNames.Attr_Type, "v2.8:InterimStageInputType"),
				new XAttribute("id", GetVehicleId()),
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
				_vehicleInputData.AirdragModifiedMultistage != null 
					? new XElement(v28 +  XMLNames.Bus_AirdragModifiedMultistage, _vehicleInputData.AirdragModifiedMultistage) : null,
				_vehicleInputData.TankSystem != null 
					? new XElement(v28 + XMLNames.Vehicle_NgTankSystem, _vehicleInputData.TankSystem.ToString()) : null,
				_vehicleInputData.RegisteredClass != null
					? new XElement(v28 + XMLNames.Vehicle_RegisteredClass, _vehicleInputData.RegisteredClass.ToXMLFormat()) : null,
				_vehicleInputData.NumberOfPassengersLowerDeck != null 
					? new XElement(v28 + XMLNames.Bus_NumberPassengersLowerDeck, _vehicleInputData.NumberOfPassengersLowerDeck.ToString()) : null,
				_vehicleInputData.NumberOfPassengersUpperDeck != null
					? new XElement(v28 + XMLNames.Bus_NumberPassengersUpperDeck, _vehicleInputData.NumberOfPassengersUpperDeck.ToString()) : null,
				_vehicleInputData.VehicleCode != null
					? new XElement(v28 + XMLNames.Vehicle_BodyworkCode, _vehicleInputData.VehicleCode.ToXMLFormat()) : null,
				_vehicleInputData.LowEntry != null
					? new XElement(v28 + XMLNames.Bus_LowEntry, _vehicleInputData.LowEntry) : null,
				_vehicleInputData.Height != null
					? new XElement(v28 + XMLNames.Bus_HeighIntegratedBody, _vehicleInputData.Height.Value() * 1000.000) : null,
				_vehicleInputData.Length != null
					? new XElement(v28 + XMLNames.Bus_VehicleLength, _vehicleInputData.Length.Value() * 1000) : null,
				_vehicleInputData.Width != null 
					? new XElement(v28 + XMLNames.Bus_VehicleWidth, _vehicleInputData.Width.Value() * 1000) : null,
				_vehicleInputData.EntranceHeight != null 
					? new XElement(v28 + XMLNames.Bus_EntranceHeight, _vehicleInputData.EntranceHeight.Value() * 1000) : null,
				_vehicleInputData.DoorDriveTechnology != null
					? new XElement(v28 + XMLNames.Bus_DoorDriveTechnology, _vehicleInputData.DoorDriveTechnology.ToXMLFormat()) : null,
				new XElement(v28 + XMLNames.Bus_VehicleDeclarationType, _vehicleInputData.VehicleDeclarationType),
				GetADAS(_vehicleInputData.ADAS),
				GetBusVehicleComponents(_vehicleInputData.Components)
			);
		}

		private string GetVehicleId()
		{
			return  $"{_vehicleInputData.VIN}-{GetStageNumber()}";
		}

		private XElement GetADAS(IAdvancedDriverAssistantSystemDeclarationInputData adasData)
		{
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
			var busAirdrag = GetBusAirdrag(vehicleComponents.AirdragInputData);
			var busAux = GetBusAuxiliaries(vehicleComponents.BusAuxiliaries);

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
			if (airdrag != null) {
				return GetAirdragElement(airdrag);
			}

			switch (_vehicleInputData.AirdragModifiedMultistage) {
				case true:
					return GetBusAirdragUseStandardValues();
				case false:
					return GetAirdragElement(_manufacturingStageInputData.Last().Vehicle.Components.AirdragInputData);
				default:
					return null;
			}
		}
		
		private XElement GetAirdragElement(IAirdragDeclarationInputData airdrag)
		{
			var component = airdrag as AbstractCommonComponentType;
			if (component == null)
				return null;

			var dataElement = XElement.Parse(component.XMLSource.FirstChild.OuterXml);
			var signatureElement = XElement.Parse(component.XMLSource.LastChild.OuterXml);
			dataElement.Attribute(XNamespace.Xmlns + "xsi")?.Remove();
			
			return new XElement(v28 + XMLNames.Component_AirDrag,
				dataElement,
				signatureElement);
		}


		private XElement GetBusAirdragUseStandardValues()
		{
			return new XElement(v28 + XMLNames.Component_AirDrag,
				new XElement(v20 + XMLNames.ComponentDataWrapper,
					new XAttribute(XNamespace.Xmlns + "v2.0", v20),
					new XAttribute(xsi + XMLNames.Attr_Type, "v2.8:AirDragModifiedUseStandardValueType"),
					new XAttribute(XMLNames.Component_ID_Attr, "standard")
				),
				new XElement(v20 + XMLNames.DI_Signature, XMLHelper.CreateDummySig(di)));
		}


		private XElement GetBusAuxiliaries(IBusAuxiliariesDeclarationData busAux)
		{
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
			if (hvac.SystemConfiguration == null &&
				hvac.HeatPumpModeDriverCompartment == null && hvac.HeatPumpTypeDriverCompartment == null &&
				hvac.HeatPumpModePassengerCompartment == null && hvac.HeatPumpTypePassengerCompartment == null &&
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
				hvac.HeatPumpTypePassengerCompartment != null
					? new XElement(v28 + XMLNames.Bus_HeatPumpTypePassenger, hvac.HeatPumpTypePassengerCompartment.GetLabel()) : null,
				hvac.HeatPumpModePassengerCompartment != null
					? new XElement(v28 + XMLNames.Bus_HeatPumpModePassenger, hvac.HeatPumpModePassengerCompartment.GetLabel()) : null,
				hvac.AuxHeaterPower != null
					? new XElement(v28 + XMLNames.Bus_AuxiliaryHeaterPower, hvac.AuxHeaterPower.Value()) : null,
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

		private XElement GetApplicationInformation()
		{
			return new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, VectoSimulationCore.VersionNumber),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		private XElement GetInputdataSignature()
		{
			return new XElement(tns + XMLNames.DI_Signature, XMLHelper.CreateDummySig(di));
		}

		private string GetNamespaceVersionNumber(XNamespace ns)
		{
			if (ns == null)
				return null;

			var pattern = @"(?<=:v)\d+\.\d+";
			var versionNumber = Regex.Match(ns.NamespaceName, pattern);
			return $"v{versionNumber}";
		}
	}
}
