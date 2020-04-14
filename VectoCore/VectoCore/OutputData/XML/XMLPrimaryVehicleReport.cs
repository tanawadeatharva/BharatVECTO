using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLPrimaryVehicleReport
	{
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryVehicleInformation:HeavyBus:v0.1";
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected XNamespace RootNS = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryVehicleInformation";

		protected XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected XElement Results;

		private bool _allSuccess = true;


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
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));

			//retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(
				new XElement(
					RootNS + XMLNames.VectoPrimaryVehicleReport,

					//new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute("xmlns", tns),

					//new XAttribute(XNamespace.Xmlns + "pbus", tns),
					new XAttribute(XNamespace.Xmlns + "pif", RootNS),
					new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(
						xsi + "schemaLocation",
						string.Format("{0} {1}/DEV/VectoOutputPrimaryVehicleInformation.xsd", RootNS, AbstractXMLWriter.SchemaLocationBaseUrl)),
					new XElement(
						RootNS + XMLNames.Report_DataWrap,
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
#if CERTIFICATION_RELEASE // add nothing to version number
#else
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		public virtual void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehiclePIFType"),
				new XElement(tns + XMLNames.ManufacturerPrimaryVehicle, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.ManufacturerAddressPrimaryVehicle, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(
					tns + XMLNames.Component_Date, XmlConvert.ToString(modelData.VehicleData.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, modelData.VehicleData.VehicleCategory.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_Articulated, modelData.VehicleData.InputData.Articulated),
				new XElement(tns + XMLNames.TPMLM, modelData.VehicleData.GrossVehicleMass.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_IdlingSpeed, modelData.EngineData.IdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_RetarderType, modelData.Retarder.Type.ToXMLFormat()),
				modelData.Retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, modelData.Retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(
					tns + XMLNames.Vehicle_AngledriveType, (modelData.AngledriveData?.Type ?? AngledriveType.None).ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				GetADAS(modelData.VehicleData.ADAS),
				GetTorqueLimits(modelData),
				VehicleComponents(modelData, fuelModes)
			);
		}

		private XElement GetADAS(VehicleData.ADASData adasData)
		{
			var ns = XNamespace.Get(adasData.InputData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Namespace);
			const string adasPrefix = "adas";
			return new XElement(
				tns + XMLNames.Vehicle_ADAS,
				new XAttribute(XNamespace.Xmlns + adasPrefix, ns.NamespaceName),
				new XAttribute(
					xsi + "type",
					string.Format("{0}:{1}", adasPrefix, adasData.InputData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Name)),
				XElement.Parse(adasData.InputData.XMLSource.OuterXml).Elements()
			);
		}

		private XElement GetTorqueLimits(VectoRunData modelData)
		{
			var inputData = modelData.VehicleData.InputData.XMLSource;
			var tcLimits = inputData.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Vehicle_TorqueLimits));
			if (tcLimits == null) {
				return null;
			}

			var ns = XNamespace.Get(tcLimits.SchemaInfo.SchemaType.QualifiedName.Namespace);
			const string adasPrefix = "tcl";
			return new XElement(
				tns + XMLNames.Vehicle_TorqueLimits,
				new XAttribute(XNamespace.Xmlns + adasPrefix, ns.NamespaceName),
				new XAttribute(
					xsi + "type", string.Format("{0}:{1}", adasPrefix, tcLimits.SchemaInfo.SchemaType.QualifiedName.Name)),
				XElement.Parse(tcLimits.OuterXml).Elements()
			);
		}

		private XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return new XElement(
				tns + XMLNames.Vehicle_Components,
				new XAttribute(xsi + "type", "VehicleComponentsPIFType"),
				GetEngineDescription(modelData.EngineData, fuelModes),
				GetGearboxDescription(modelData.GearboxData),
				GetAngledriveDescription(modelData.AngledriveData),
				GetAxlegearDescription(modelData.AxleGearData),
				GetAxleWheelsDescription(modelData),
				GetAuxiliariesDescription(modelData)
			);
		}

		protected virtual XElement GetAxleWheelsDescription(VectoRunData modeldData)
		{
			var axles = modeldData.VehicleData.InputData.Components.AxleWheels.XMLSource;

			var ns = XNamespace.Get(axles.SchemaInfo.SchemaType.QualifiedName.Namespace);

			//const string adasPrefix = "axl";
			return new XElement(
				tns + XMLNames.Component_AxleWheels,

				//new XAttribute(XNamespace.Xmlns + adasPrefix, ns.NamespaceName),
				new XAttribute(XNamespace.Xmlns + "pbus", tns),
				new XAttribute("xmlns", ns.NamespaceName),
				new XAttribute(
					xsi + "type", axles.SchemaInfo.SchemaType.QualifiedName.Name),
				XElement.Parse(axles.OuterXml).Elements()
			);
		}



		private XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var busAuxiliaries = modelData.BusAuxiliaries;
			var busAuxXML = busAuxiliaries.InputData.XMLSource;
			var ns = XNamespace.Get(busAuxXML.FirstChild.SchemaInfo.SchemaType.QualifiedName.Namespace);
			const string auxPrefix = "aux";
			return new XElement(
				tns + XMLNames.Component_Auxiliaries,
				new XElement(
					tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XNamespace.Xmlns + auxPrefix, ns.NamespaceName),
					new XAttribute(
						xsi + "type", string.Format("{0}:{1}", auxPrefix, busAuxXML.FirstChild.SchemaInfo.SchemaType.QualifiedName.Name)),
					XElement.Parse(busAuxXML.InnerXml).Elements()
				));
		}

		private XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return WrapComponent(
				XMLNames.Component_Axlegear, "AxlegearDataPIFType",
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Component_AppVersion, axleGearData.InputData.AppVersion),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_Angledrive,
				GetCommonDescription(angledriveData),
				new XElement(tns + XMLNames.Component_AppVersion, angledriveData.InputData.AppVersion),
				new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio.ToXMLFormat(3)));
		}

		private XElement GetGearboxDescription(GearboxData gearboxData)
		{
			var retVal = WrapComponent(
				"Transmission", "TransmissionDataPIFType",
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(
					tns + XMLNames.Gearbox_Gears,
					new XAttribute(xsi + "type", "TransmissionGearsPIFType"),
					gearboxData.Gears.Select(
						x =>
							new XElement(
								tns + XMLNames.Gearbox_Gears_Gear,
								new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, x.Key),
								new XElement(tns + XMLNames.Gearbox_Gear_Ratio, x.Value.Ratio.ToXMLFormat(3)),
								x.Value.MaxTorque != null
									? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, x.Value.MaxTorque.ToXMLFormat(0))
									: null,
								x.Value.MaxSpeed != null
									? new XElement(tns + XMLNames.Gearbox_Gear_MaxSpeed, x.Value.MaxSpeed.AsRPM.ToXMLFormat(0))
									: null)))
			);
			return retVal;

		}

		private XElement GetEngineDescription(CombustionEngineData engineData, List<List<FuelData.Entry>> fuelModes)
		{
			XNamespace v23 = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;

			var fuels = new List<XElement>();
			foreach (var mode in engineData.InputData.EngineModes) {
				fuels.Add(
					new XElement(
						tns + XMLNames.Engine_FuelModes,
						new XElement(tns + XMLNames.Engine_IdlingSpeed, mode.IdleSpeed.AsRPM.ToXMLFormat(0)),
						new XElement(
							tns + XMLNames.Engine_FullLoadAndDragCurve,
							FullLoadCurveReader.Create(mode.FullLoadCurve, true).FullLoadEntries.Select(
								x => new XElement(
									tns + XMLNames.Engine_FullLoadCurve_Entry,
									new XAttribute(XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr, x.EngineSpeed.AsRPM.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_MaxTorque_Attr, x.TorqueFullLoad.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_DragTorque_Attr, x.TorqueDrag.ToXMLFormat(2)))
							)),
						new XElement(
							tns + "Fuels",
							mode.Fuels.Select(x => new XElement(tns + "FuelType", x.FuelType.ToXMLFormat())))
					)
				);
			}

			var retVal = WrapComponent(
				XMLNames.Component_Engine, "EngineDataPIFType",
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_Displacement, engineData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedSpeed, engineData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_MaxTorque, engineData.InputData.MaxTorqueDeclared.ToXMLFormat(0)),
				new XElement(
					tns + XMLNames.Engine_WHRType,
					new XElement(
						v23 + XMLNames.Engine_WHR_MechanicalOutputICE, (engineData.WHRType & WHRType.MechanicalOutputICE) != 0),
					new XElement(
						v23 + XMLNames.Engine_WHR_MechanicalOutputIDrivetrain,
						(engineData.WHRType & WHRType.MechanicalOutputDrivetrain) != 0),
					new XElement(v23 + XMLNames.Engine_WHR_ElectricalOutput, (engineData.WHRType & WHRType.ElectricalOutput) != 0)
				),
				fuels
			);
			return retVal;
		}

		private XElement WrapComponent(
			string elementName, string dataType, object[] commonElements, params object[] specificElements)
		{
			return new XElement(
				tns + elementName,
				new XElement(
					tns + XMLNames.ComponentDataWrapper,
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
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, data.InputData.AppVersion)
			};
		}

		protected virtual object[] GetCommonDescription(GearboxData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer),
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Component_Gearbox_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, data.InputData.AppVersion)
			};
		}

		protected virtual object[] GetCommonDescription(SimulationComponentData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer),
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc)),
			};
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			_allSuccess &= resultEntry.Status == VectoRun.Status.Success;

			//if (resultEntry.Status == VectoRun.Status.Success) {
			//	_weightedPayload += resultEntry.Payload * resultEntry.WeightingFactor;
			//	_weightedCo2 += resultEntry.CO2Total / resultEntry.Distance * resultEntry.WeightingFactor;
			//}
			Results.Add(
				new XElement(
					tns + XMLNames.Report_Result_Result,
					new XAttribute(
						XMLNames.Report_Result_Status_Attr,
						resultEntry.Status == VectoRun.Status.Success ? "success" : "error"),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, resultEntry.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Report_Result_Mission, resultEntry.Mission.ToXMLFormat()),
					new XElement(
						tns + XMLNames.Report_ResultEntry_SimulationParameters,
						new XElement(
							tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
							XMLHelper.ValueAsUnit(resultEntry.Payload, XMLNames.Unit_kg, 0)),
						new XElement(
							tns + XMLNames.Report_Result_Payload, XMLHelper.ValueAsUnit(resultEntry.Payload, XMLNames.Unit_kg, 2)),
						new XElement(
							tns + XMLNames.Report_ResultEntry_PassengerCount,
							resultEntry.PassengerCount),
						new XElement(
							tns + XMLNames.Report_Result_FuelMode,
							resultEntry.FuelData.Count > 1
								? XMLNames.Report_Result_FuelMode_Val_Dual
								: XMLNames.Report_Result_FuelMode_Val_Single)
					),
					GetResults(resultEntry)));
		}

		private object[] GetResults(XMLDeclarationReport.ResultEntry resultEntry)
		{
			switch (resultEntry.Status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running: return null; // should not happen!
				case VectoRun.Status.Success: return GetSuccessResultEntry(resultEntry);
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					return new object[] {
						new XElement(tns + "Error", resultEntry.Error)
					};
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private object[] GetSuccessResultEntry(XMLDeclarationReport.ResultEntry result)
		{
			var retVal = new List<XElement>();

			foreach (var fuel in result.FuelData) {
				var fcResult = new XElement(
					tns + XMLNames.Report_Results_Fuel,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()));
				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Result_EnergyConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/km"),
						(result.FuelConsumptionFinal[fuel.FuelType] * fuel.LowerHeatingValueVecto /
						result.Distance.ConvertToKiloMeter() / 1e6)
						.Value().ToMinSignificantDigits(3, 3)));
				retVal.Add(fcResult);
			}

			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3, 1)));

			return retVal.Cast<object>().ToArray();
		}
	}

}
