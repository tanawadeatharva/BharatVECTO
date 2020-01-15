using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLPrimaryVehicleReport
	{
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryBusInformation:HeavyBus:v0.1";
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected XNamespace RootNS = "urn:tugraz:ivt:VectoAPI:PrimaryVehicleInformation";

		protected XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected XElement Results;

		public XMLPrimaryVehicleReport()
		{
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public XDocument Report { get; private set; }


		public void GenerateReport(XElement resultSignature)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			var results = new XElement(Results);

			//retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(new XElement(RootNS + XMLNames.VectoPrimaryVehicleReport,
									//new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
									new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
									new XAttribute("xmlns", tns),
									new XAttribute(XNamespace.Xmlns + "pif", RootNS),
									new XAttribute(XNamespace.Xmlns + "di", di),
									new XAttribute(xsi + "schemaLocation",
													string.Format("{0} {1}VectoOutputPrimaryVehicleInformation.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl)),
									new XElement(RootNS + XMLNames.Report_DataWrap,
										new XAttribute(xsi + "type", "PrimaryVehicleHeavyBusDataType"),
												VehiclePart,
												new XElement(tns + XMLNames.Report_ResultData_Signature, resultSignature),
												results,
												GetApplicationInfo())
						)
			);
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(retVal);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			var h = VectoHash.Load(stream);
			Report = h.AddHash();
		}

		private XElement GetApplicationInfo()
		{
			var versionNumber = VectoSimulationCore.VersionNumber;
#if CERTIFICATION_RELEASE
// add nothing to version number
#else
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
								new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
								new XElement(tns + XMLNames.Report_ApplicationInfo_Date,
											XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}
		public virtual void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XElement(tns + XMLNames.ManufacturerPrimaryVehicle, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.ManufacturerAddressPrimaryVehicle, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(modelData.VehicleData.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(
					tns + XMLNames.Vehicle_GrossVehicleMass,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 1)),
				new XElement(
					tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.VehicleData.DualFuelVehicle),
				new[] {
					new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
					new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
					new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null),
					GetADAS(modelData.VehicleData.ADAS),
					GetTorqueLimits(modelData.EngineData),
					VehicleComponents(modelData, fuelModes)
				}
			);
		}

		private XElement GetADAS(VehicleData.ADASData adasData)
		{
			return new XElement(tns + XMLNames.Vehicle_ADAS,
								new XElement(tns + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			);
		}

		private XElement GetTorqueLimits(CombustionEngineData modelData)
		{
			var limits = new List<XElement>();
			var maxTorque = modelData.FullLoadCurves[0].MaxTorque;
			for (uint i = 1; i < modelData.FullLoadCurves.Count; i++) {
				if (!maxTorque.IsEqual(modelData.FullLoadCurves[i].MaxTorque, 1e-3.SI<NewtonMeter>())) {
					limits.Add(
						new XElement(
							tns + XMLNames.Vehicle_TorqueLimits_Entry,
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, i),
							new XAttribute(
								XMLNames.XMLManufacturerReport_torqueLimit,
								modelData.FullLoadCurves[i].MaxTorque.ToXMLFormat(0)),
							new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_Nm),
							new XAttribute(
								XMLNames.XMLManufacturerReport_torqueLimitPercent,
								(modelData.FullLoadCurves[i].MaxTorque / maxTorque * 100).ToXMLFormat(1))));
				}
			}

			return limits.Count == 0
				? null
				: new XElement(tns + XMLNames.Vehicle_TorqueLimits, limits.Cast<object>().ToArray());
		}

		private XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return new XElement(tns + XMLNames.Vehicle_Components,
								GetEngineDescription(modelData.EngineData, fuelModes)
								//GetGearboxDescription(modelData.GearboxData),
								//GetAngledriveDescription(modelData.AngledriveData),
								//GetAxlegearDescription(modelData.AxleGearData),
								//GetAxleWheelsDescription(modelData.VehicleData),
								//GetAuxiliariesDescription(modelData.Aux)
			);
		}

		private XElement GetEngineDescription(CombustionEngineData engineData, List<List<FuelData.Entry>> fuelModes)
		{
			XNamespace v23 = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
			return WrapComponent(XMLNames.Component_Engine, "EngineDataPIFType",
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Component_AppVersion, "TODO!"),
				new XElement(tns + XMLNames.Engine_Displacement, engineData.Displacement.ConvertToCubicDeziMeter().ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedSpeed, engineData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(tns + "MaxEngineTorque", "TODO!"),
				new XElement(tns + XMLNames.Engine_WHRType,
					new XElement(v23 + XMLNames.Engine_WHR_MechanicalOutputICE, (engineData.WHRType & WHRType.MechanicalOutputICE) != 0),
					new XElement(v23 + XMLNames.Engine_WHR_MechanicalOutputIDrivetrain, (engineData.WHRType & WHRType.MechanicalOutputDrivetrain) != 0),
					new XElement(v23 + XMLNames.Engine_WHR_ElectricalOutput, (engineData.WHRType & WHRType.ElectricalOutput) != 0)
				)
				);
		}

		private XElement WrapComponent(string elementName, string dataType, object[] commonElements, params object[] specificElements)
		{
			return new XElement(tns + elementName, 
				new XElement(tns + XMLNames.ComponentDataWrapper,
				new XAttribute(xsi + "type", dataType),
				commonElements,
				specificElements));
		}


		private object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer), 
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc))
			};
		}
	}
}
