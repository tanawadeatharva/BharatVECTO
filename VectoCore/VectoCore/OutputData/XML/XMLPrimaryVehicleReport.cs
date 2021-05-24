using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLPrimaryVehicleReport
	{
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected XNamespace v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace v21 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1";
		protected XNamespace v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.3";
		protected XNamespace v26 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.6";
		protected XNamespace v28 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.8";
		protected XNamespace v10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
		
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
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));

			var vehicleId = $"{VectoComponents.Vehicle.HashIdPrefix()}{GetGUID()}";

			retVal.Add(
				new XElement(XMLNames.VectoOutputMultistage,
					new XAttribute("xmlns", tns),
					new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "v2.0", v20),
					new XAttribute(XNamespace.Xmlns + "v2.1", v21),
					new XAttribute(XNamespace.Xmlns + "v2.3", v23),
					new XAttribute(XNamespace.Xmlns + "v2.6", v26),
					new XAttribute(XNamespace.Xmlns + "v2.8", v28),
					new XAttribute(xsi + "schemaLocation", $"{tns.NamespaceName} "+ @"V:\VectoCore\VectoCore\Resources\XSD/VectoOutputMultistage.0.1.xsd"),

					new XElement(XMLNames.Bus_PrimaryVehicle,
						new XElement(tns + XMLNames.Report_DataWrap,
							new XAttribute(XMLNames.Component_ID_Attr, vehicleId),
							new XAttribute(xsi + "type", "PrimaryVehicleDataType"),
							 VehiclePart,
							InputDataIntegrity,
							new XElement(tns + "ManufacturerRecordSignature", resultSignature),
							results,
							GetApplicationInfo()),
						GetInputdataSignature(vehicleId))
					)
				);

				//var stream = new MemoryStream();
				//var writer = new StreamWriter(stream);
				//writer.Write(retVal);
				//writer.Flush();
				//stream.Seek(0, SeekOrigin.Begin);

				//var h = VectoHash.Load(stream);
				//Report = h.AddHash();
				Report = retVal;
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
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(modelData.VehicleData.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Bus_LegislativeCategory, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + "ChassisConfiguration", modelData.VehicleData.VehicleCategory.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_Articulated, modelData.VehicleData.InputData.Articulated),
				new XElement(tns + XMLNames.TPMLM, modelData.VehicleData.InputData.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_IdlingSpeed, modelData.EngineData.IdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_RetarderType, modelData.Retarder.Type.ToXMLFormat()),
				modelData.Retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, modelData.Retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(tns + XMLNames.Vehicle_AngledriveType, (modelData.AngledriveData?.Type ?? AngledriveType.None).ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				GetADAS(modelData.VehicleData.ADAS),
				GetTorqueLimits(modelData),
				VehicleComponents(modelData, fuelModes)
			);

			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
											modelData.InputDataHash == null ? XMLHelper.CreateDummySig(di) : new XElement(modelData.InputDataHash));
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
				GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
				GetAngledriveDescription(modelData.AngledriveData),
				GetAxlegearDescription(modelData.AxleGearData),
				GetAxleWheelsDescription(modelData),
				GetAuxiliariesDescription(modelData)
			);
		}

		private XElement GetTorqueConverterDescription(TorqueConverterData torqueConverter)
		{
			if (torqueConverter == null)
				return null;
			
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				new XElement(tns + XMLNames.Report_DataWrap,
					new XAttribute(xsi + "type", "TorqueConverterDataPIFType"),
					GetCommonDescription(torqueConverter),
					new XElement(tns + XMLNames.Component_AppVersion, torqueConverter.AppVersion), 
					new XElement(tns + XMLNames.TorqueConverter_Characteristics,
						GetTorqueConverterCharacteristics(torqueConverter.TCData))
				));
		}

		private List<XElement> GetTorqueConverterCharacteristics(DataTable tcData)
		{
			var characteristics = new List<XElement>();

			foreach (DataRow row in tcData.Rows) {
				var elementEntry = new XElement(tns + XMLNames.TorqueConverter_Characteristics_Entry,
					new XAttribute(XMLNames.TorqueConverterData_SpeedRatio_Attr,
						row[TorqueConverterDataReader.Fields.SpeedRatio].ToString()),
					new XAttribute(XMLNames.TorqueConverterData_TorqueRatio_Attr,
						row[TorqueConverterDataReader.Fields.TorqueRatio].ToString()),
					new XAttribute(XMLNames.TorqueConverterDataMapping_InputTorqueRef_Attr,
						row[TorqueConverterDataReader.Fields.CharacteristicTorque].ToString())
				); 
				characteristics.Add(elementEntry);
			}

			return characteristics;
		}
		
		protected virtual XElement GetAxleWheelsDescription(VectoRunData modeldData)
		{
			var axles = modeldData.VehicleData.InputData.Components.AxleWheels.XMLSource;
			var axlesNode = axles.SelectSingleNode(".//*[local-name()='Axles']");
			var axlesNodes = axlesNode.SelectNodes(".//*[local-name()='Axle']");
			
			return new XElement(tns + XMLNames.Component_AxleWheels,
					new XElement(tns + XMLNames.Report_DataWrap, 
						new XAttribute(xsi + "type", "AxleWheelsDataPIFType"),
						new XElement(tns + XMLNames.AxleWheels_Axles,
							GetAxlesNodes(axlesNodes)
						)));
		}


		private List<XElement> GetAxlesNodes(XmlNodeList nodes)
		{
			var axles = new List<XElement>();
			foreach (XmlNode axleNode in nodes) {

				var axleNumber = axleNode.Attributes[XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr].Value;
				var axle = new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
							new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, axleNumber),
							new XAttribute(XNamespace.Xmlns + "v2.0", v20),
							new XAttribute(xsi + "type", "v2.0:AxleDataDeclarationType"),
								XElement.Parse(axleNode.OuterXml.Replace("xsi:type=\"TyreDataDeclarationType\"", "xsi:type=\"v2.0:TyreDataDeclarationType\"")).Elements());
				
				axles.Add(axle);
			}

			return axles;
		}



		private XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var busAuxiliaries = modelData.BusAuxiliaries;
			var busAuxXML = busAuxiliaries.InputData.XMLSource;

			var dataElement = new XElement(tns+ XMLNames.ComponentDataWrapper,
				new XAttribute(xsi + "type", "AuxiliaryDataPIFType"),
				new XAttribute("xmlns", tns.NamespaceName),

				XElement.Parse(busAuxXML.InnerXml).Elements());
			dataElement = RemoveNamespace(dataElement);			

			
			return new XElement(tns + XMLNames.Component_Auxiliaries,
				 dataElement );
		}


		private XElement RemoveNamespace(XElement elements)
		{
			foreach (XElement e in elements.DescendantsAndSelf())
			{
				if (e.Name.Namespace != XNamespace.None)
					e.Name = e.Name.LocalName;
			}

			return elements;
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
			if (angledriveData == null) 
				return null;
			
			return WrapComponent(XMLNames.Component_Angledrive, "AngledriveDataPIFType",
				GetCommonDescription(angledriveData),
					new XElement(tns + XMLNames.Component_AppVersion, angledriveData.InputData.AppVersion),
					new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio.ToXMLFormat(3))
				);
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
							XMLHelper.ValueAsUnit(resultEntry.TotalVehicleMass, XMLNames.Unit_kg, 2)),
						new XElement(
							tns + XMLNames.Report_Result_Payload, XMLHelper.ValueAsUnit(resultEntry.Payload, XMLNames.Unit_kg, 2)),
						new XElement(
							tns + XMLNames.Report_ResultEntry_PassengerCount,
							resultEntry.PassengerCount?.ToXMLFormat(2) ?? "NaN"),
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
						new XElement(tns + "Error", resultEntry.Error),
						new XElement(tns + "ErrorDetails", resultEntry.StackTrace), 
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
						(result.FuelConsumptionFinal[fuel.FuelType].EnergyDemand /
						result.Distance.ConvertToKiloMeter() / 1e6)
						.Value().ToMinSignificantDigits(5, 5)));
				retVal.Add(fcResult);
			}

			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3, 2)));

			return retVal.Cast<object>().ToArray();
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

	}

}
