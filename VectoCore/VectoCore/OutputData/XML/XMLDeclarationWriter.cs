using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using TUGraz.IVT.VectoXML;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationWriter : AbstractXMLWriter
	{
		private XNamespace componentNamespace;
		//private readonly XNamespace _vectoNs = @"../../../API/VectoInput.xsd";


		public XMLDeclarationWriter(string vendor) : base(null, vendor)
		{
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.8";
			rootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.8";
			componentNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v0.8";
		}

		public XDocument GenerateVectoJob(IDeclarationInputDataProvider data)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

			//<tns:VectoInputDeclaration 
			//  xmlns="urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.6" 
			//  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" schemaVersion="0.6" 
			//  xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6" 
			//  xsi:schemaLocation="urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6 http://markus.quaritsch.at/VECTO/VectoInput.xsd">
			var job = new XDocument();
			job.Add(new XElement(rootNamespace + XMLNames.VectoInputDeclaration,
				new XAttribute("schemaVersion", SchemaVersion),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", rootNamespace),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoInput.xsd", rootNamespace, SchemaLocationBaseUrl)),
				CreateDeclarationJob(data))
				);
			return job;
		}

		public XDocument GenerateVectoComponent(IGearboxDeclarationInputData data,
			ITorqueConverterDeclarationInputData torqueConverter)
		{
			return GenerateComponentDocument(CreateGearbox(data, torqueConverter));
		}

		public XDocument GenerateVectoComponent(IAxleGearInputData data)
		{
			return GenerateComponentDocument(CreateAxlegear(data));
		}

		protected XDocument GenerateComponentDocument(XElement content)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var component = new XDocument();
			component.Add(new XElement(componentNamespace + XMLNames.VectoInputDeclaration,
				new XAttribute("schemaVersion", SchemaVersion),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", componentNamespace),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoComponent.xsd", componentNamespace, SchemaLocationBaseUrl)),
				content)
				);
			return component;
		}

		protected XElement[] CreateDeclarationJob(IDeclarationInputDataProvider data)
		{
			return new[] {
				CreateVehicle(data)
			};
		}

		protected XElement CreateVehicle(IDeclarationInputDataProvider data)
		{
			var retarder = data.RetarderInputData;
			var gearbox = data.GearboxInputData;
			var vehicle = data.VehicleInputData;
			var angledrive = data.AngledriveInputData;

			var aux = data.AuxiliaryInputData();
			var numSteeredaxles = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.SteeringPump).Technology.Count;
			//var pto = data.PTOTransmissionInputData;

			return new XElement(tns + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, "VEH-" + vehicle.Model),
				GetDefaultComponentElements(vehicle.TechnicalReportId, vehicle.Model, "N.A."),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, "N3"),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, GetVehicleCategoryXML(vehicle.VehicleCategory)),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, vehicle.CurbMassChassis.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, vehicle.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_IdlingSpeed, data.EngineInputData.IdleSpeed.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_RetarderType, GetRetarterTypeXML(retarder.Type)),
				retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(tns + XMLNames.Vehicle_AngledriveType, angledrive.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_PTO,
					new XElement(tns + XMLNames.Vehicle_PTO_ShaftsGearWheels, "none"),
					new XElement(tns + XMLNames.Vehicle_PTO_OtherElements, "none")),
				CreateTorqueLimits(vehicle),
				new XElement(tns + XMLNames.Vehicle_Components,
					CreateEngine(data.EngineInputData),
					CreateGearbox(gearbox, gearbox.Type.AutomaticTransmission() ? data.TorqueConverterInputData : null),
					angledrive.Type == AngledriveType.SeparateAngledrive ? CreateAngleDrive(angledrive) : null,
					retarder.Type.IsDedicatedComponent() ? CreateRetarder(retarder) : null,
					CreateAxlegear(data.AxleGearInputData),
					CreateAxleWheels(data.VehicleInputData),
					CreateAuxiliaries(data.AuxiliaryInputData()),
					CreateAirdrag(data.AirdragInputData)
					)
				);
		}


		protected XElement CreateEngine(IEngineDeclarationInputData data)
		{
			var id = string.Format("ENG-{0}", data.Model.RemoveWhitespace());
			var fld = EngineFullLoadCurve.Create(data.FullLoadCurve, true);
			return new XElement(tns + XMLNames.Component_Engine,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, string.Format("ENG-{0}", data.Model)),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(string.Format("ENG-{0}", data.Model), data.Model),
					new XElement(tns + XMLNames.Engine_Displacement, (data.Displacement.Value() * 1000 * 1000).ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_IdlingSpeed, data.IdleSpeed.AsRPM.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_RatedSpeed, fld.RatedSpeed.AsRPM.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_RatedPower, fld.FullLoadStationaryPower(fld.RatedSpeed).ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_MaxTorque, fld.MaxTorque.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_WHTCUrban, data.WHTCUrban.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_WHTCRural, data.WHTCRural.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_WHTCMotorway, data.WHTCMotorway.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_ColdHotBalancingFactor, data.ColdHotBalancingFactor.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Engine_CorrectionFactor_RegPer, "1.0000"),
					new XElement(tns + XMLNames.Engine_CorrecionFactor_NCV, "1.0000"),
					new XElement(tns + XMLNames.Engine_FuelType, "Diesel CI"),
					new XElement(tns + XMLNames.Engine_FuelConsumptionMap,
						EmbedDataTable(data.FuelConsumptionMap, AttributeMappings.FuelConsumptionMapMapping)),
					new XElement(tns + XMLNames.Engine_FullLoadAndDragCurve,
						EmbedDataTable(data.FullLoadCurve, AttributeMappings.EngineFullLoadCurveMapping)
						)
					),
				AddSignatureDummy(id)
				);
		}


		protected XElement CreateGearbox(IGearboxDeclarationInputData gbxData,
			ITorqueConverterDeclarationInputData torqueConverter)
		{
			var gears = new XElement(tns + XMLNames.Gearbox_Gears);
			var i = 1;
			foreach (var gearData in gbxData.Gears) {
				var gear = new XElement(tns + XMLNames.Gearbox_Gears_Gear,
					new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, i++),
					new XElement(tns + XMLNames.Gearbox_Gear_Ratio, gearData.Ratio.ToXMLFormat(3)),
					gearData.MaxTorque != null
						? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, gearData.MaxTorque.Value().ToXMLFormat(0))
						: null,
					gearData.MaxInputSpeed != null
						? new XElement(tns + XMLNames.Gearbox_Gear_MaxSpeed, gearData.MaxInputSpeed.AsRPM.ToXMLFormat(0))
						: null,
					new XElement(tns + XMLNames.Gearbox_Gear_TorqueLossMap,
						EmbedDataTable(gearData.LossMap, AttributeMappings.TransmissionLossmapMapping))
					);
				gears.Add(gear);
			}
			var id = string.Format("GBX-{0}", gbxData.Model.RemoveWhitespace());
			return new XElement(tns + XMLNames.Component_Gearbox,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, string.Format("GBX-{0}", gbxData.Model)),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(string.Format("GBX-{0}", gbxData.Model), gbxData.Model),
					new XElement(tns + XMLNames.Gearbox_TransmissionType, gbxData.Type.ToXMLFormat()),
					new XElement(tns + XMLNames.Component_Gearbox_CertificationMethod, "Standard values"),
					gears
					),
				AddSignatureDummy(id),
				gbxData.Type.AutomaticTransmission() ? CreateTorqueConverter(torqueConverter) : null
				);
		}


		private XElement CreateTorqueConverter(ITorqueConverterDeclarationInputData data)
		{
			if (data == null) {
				throw new Exception("Torque Converter is required!");
			}
			var id = string.Format("TC-{0}", data.Model.RemoveWhitespace());
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, id),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.TechnicalReportId, data.Model),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.TorqueConverter_Characteristics,
						EmbedDataTable(data.TCData, AttributeMappings.TorqueConverterDataMapping,
							precision: new Dictionary<string, uint>() {
								{ TorqueConverterDataReader.Fields.SpeedRatio, 4 }
							})
						)
					),
				AddSignatureDummy(id));
		}

		private XElement CreateAngleDrive(IAngledriveInputData data)
		{
			var id = string.Format("ANGL-{0}", data.Model.RemoveWhitespace());
			return new XElement(tns + XMLNames.Component_Angledrive,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, "ANGL-" + data.Model),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.TechnicalReportId, data.Model),
					new XElement(tns + XMLNames.AngleDrive_Ratio, data.Ratio.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.AngleDrive_TorqueLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.TransmissionLossmapMapping))),
				AddSignatureDummy(id));
		}

		public XElement CreateRetarder(IRetarderInputData data)
		{
			var id = string.Format("RET-{0}", data.Model.RemoveWhitespace());
			return new XElement(tns + XMLNames.Component_Retarder,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, "RET-none"),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.TechnicalReportId, data.Model),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.Retarder_RetarderLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.RetarderLossmapMapping)
						)
					),
				AddSignatureDummy(id)
				);
		}

		public XElement CreateAxlegear(IAxleGearInputData data)
		{
			var typeId = string.Format("AXLGEAR-{0:0.000}", data.Ratio);
			return new XElement(tns + XMLNames.Component_Axlegear,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, string.Format("AXL-{0}", data.Model)),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, typeId),
					GetDefaultComponentElements(typeId, "N.A."),
					new XElement(tns + "LineType", "Single portal axle"),
					new XElement(tns + XMLNames.Axlegear_Ratio, data.Ratio.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Component_CertificationMethod, "Standard values"),
					new XElement(tns + XMLNames.Axlegear_TorqueLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.TransmissionLossmapMapping))),
				AddSignatureDummy(typeId)
				);
		}

		public XElement CreateAxleWheels(IVehicleDeclarationInputData data)
		{
			var axleData = data.Axles;
			var numAxles = axleData.Count;
			var axles = new List<XElement>(numAxles);
			for (var i = 0; i < numAxles; i++) {
				var axle = axleData[i];
				axles.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i + 1),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_AxleType_Attr,
						i == 1 ? AxleType.VehicleDriven.ToString() : AxleType.VehicleNonDriven.ToString()),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres_Attr, axle.TwinTyres),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Steered, i == 0),
					CreateTyre(axle)
					));
			}

			return new XElement(tns + XMLNames.Component_AxleWheels,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					//new XAttribute(XMLNames.Component_ID_Attr,
					//	string.Format("AXLWHL-{0}", data.AxleConfiguration.GetName())),
					new XElement(tns + XMLNames.AxleWheels_Axles, axles))
				);
		}

		private XElement CreateTyre(IAxleDeclarationInputData axle)
		{
			var id = string.Format("TYRE-{0}", axle.Wheels).RemoveWhitespace().Replace("/", "_");
			return new XElement(tns + "Tyre",
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, id),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(string.Format("TYRE-{0}", axle.Wheels), axle.Wheels),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension, axle.Wheels),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCDeclared, axle.RollResistanceCoefficient.ToXMLFormat(4)),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_FzISO, axle.TyreTestLoad.Value().ToXMLFormat(0))
					),
				AddSignatureDummy(id));
		}

		public XElement CreateAuxiliaries(IAuxiliariesDeclarationInputData data)
		{
			var auxList = new Dictionary<AuxiliaryType, XElement>();
			foreach (var auxData in data.Auxiliaries) {
				var entry = new XElement(tns + AuxTypeToXML(auxData.Type),
					auxData.Technology.Select(x => new XElement(tns + XMLNames.Auxiliaries_Auxiliary_Technology, x)).ToArray<object>());
				auxList[auxData.Type] = entry;
			}
			var aux = new XElement(tns + XMLNames.ComponentDataWrapper);
			foreach (
				var key in
					new[] {
						AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
						AuxiliaryType.HVAC
					}) {
				aux.Add(auxList[key]);
			}
			return new XElement(tns + XMLNames.Component_Auxiliaries, aux);
		}

		private XElement CreateAirdrag(IAirdragDeclarationInputData data)
		{
			var id = string.Format("Airdrag-{0}", data.Model);
			return new XElement(tns + XMLNames.Component_AirDrag,
				new XAttribute(XMLNames.Component_CertificationNumber_Attr, string.Format("AD-{0}", data.Model)),
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetDefaultComponentElements(data.Model, "N.A."),
					new XElement(tns + "CdxA_0", data.AirDragArea.Value().ToXMLFormat(2)), // TODO
					new XElement(tns + "TransferredCdxA", data.AirDragArea.Value().ToXMLFormat(2)), // TODO
					new XElement(tns + XMLNames.AirDrag_DeclaredCdxA, data.AirDragArea.Value().ToXMLFormat(2))),
				AddSignatureDummy(id)
				);
		}

		private string AuxTypeToXML(AuxiliaryType type)
		{
			return type.ToString();
		}


		private XElement AddSignatureDummy(string id)
		{
			return new XElement(tns + XMLNames.DI_Signature,
				new XElement(di + XMLNames.DI_Signature_Reference,
					new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, "#" + id),
					new XElement(di + XMLNames.DI_Signature_Reference_Transforms,
						new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr,
								"http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithoutComments")),
						new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "urn:vecto:xml:2017:canonicalization"))
						),
					new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
						new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "http://www.w3.org/2001/04/xmlenc#sha256")),
					new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "")
					)
				);
		}

		protected XElement[] GetDefaultComponentElements(string componentId, string makeAndModel)
		{
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, Vendor),
				new XElement(tns + XMLNames.Component_Model, makeAndModel),
				new XElement(tns + XMLNames.Component_TechnicalReportId, componentId),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, "VectoCore"),
			};
		}

		protected XElement[] GetDefaultComponentElements(string vin, string makeAndModel, string address)
		{
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, Vendor),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, address),
				new XElement(tns + XMLNames.Component_Model, makeAndModel),
				new XElement(tns + XMLNames.Vehicle_VIN, vin),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
			};
		}
	}
}