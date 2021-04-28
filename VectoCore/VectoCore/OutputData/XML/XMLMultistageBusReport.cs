using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
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
		protected XNamespace v21 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1";
		protected XNamespace v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.3";
		protected XNamespace v26 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.6";
		protected XNamespace v28 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.8";

		protected XElement PrimaryVehicle;
		protected XElement InputDataSignature;
		protected XElement ManufacturerRecordSignature;
		protected XElement Results;
		protected XElement ApplicationInformation; 
		protected XElement PrimarySignature;


		private IPrimaryVehicleInformationInputDataProvider primaryVehicleInputData;
		private IList<IManufacturingStageInputData> manufacturingStageInputData;
		private IVehicleDeclarationInputData vehicleInputData;

		public XDocument Report { get; protected set; }

		public XMLMultistageBusReport()
		{
			PrimaryVehicle = new XElement(tns + XMLNames.Component_Vehicle);
			InputDataSignature = new XElement(tns + XMLNames.Report_InputDataSignature);
			ManufacturerRecordSignature = new XElement(tns + "ManufacturerRecordSignature");
			Results = new XElement(tns + XMLNames.Report_Results);
			PrimarySignature = new XElement(tns + XMLNames.DI_Signature);
			ApplicationInformation = new XElement(tns + XMLNames.Report_ApplicationInfo_ApplicationInformation);
		}



		public void Initialize(VectoRunData modelData)
		{
			primaryVehicleInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.PrimaryVehicle;
			manufacturingStageInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.ManufacturingStages;
			vehicleInputData = modelData.MultistageVIFInputData.VehicleInputData;
			
			SetPrimaryVehicle(primaryVehicleInputData);
			SetInputDataSignature(primaryVehicleInputData);
			SetManufacturerRecordSignature(primaryVehicleInputData);
			SetResults(primaryVehicleInputData);
			SetApplicationInformation(primaryVehicleInputData);
			SetPrimarySignature(primaryVehicleInputData);
		}



		public void GenerateReport()
		{
			var retVal = new XDocument();

			retVal.Add(
			
				new XElement(tns + "VectoOutputMultistage",
					new XAttribute("xmlns" , tns),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(XNamespace.Xmlns + "v2.0", v20),
					new XAttribute(XNamespace.Xmlns + "v2.1", v21),
					new XAttribute(XNamespace.Xmlns + "v2.3", v23),
					new XAttribute(XNamespace.Xmlns + "v2.6", v26),
					new XAttribute(XNamespace.Xmlns + "v2.8", v28),
					new XAttribute(
						xsi + "schemaLocation",
						string.Format("{0} {1}", tns, @"E:\VECTO_DEV\fk_vecto-dev\VectoCore\VectoCore\Resources\XSD/VectoOutputMultistage.0.1.xsd")),

					new XElement(tns + "PrimaryVehicle",

							new XElement(tns + XMLNames.Report_DataWrap,
								new XAttribute("id", "text"),
								new XAttribute(xsi + "type", "PrimaryVehicleDataType"),

									PrimaryVehicle,
									InputDataSignature,
									ManufacturerRecordSignature,
									Results,
									ApplicationInformation	
								),
								PrimarySignature
							)
						)
					);
			Report = retVal;

		}

		private void SetApplicationInformation(IPrimaryVehicleInformationInputDataProvider primaryData)
		{
			ApplicationInformation = new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, primaryData.ApplicationInformation.SimulationToolVersion),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(primaryData.ApplicationInformation.Date, XmlDateTimeSerializationMode.Utc)));
		}
		
		private void SetPrimarySignature(IPrimaryVehicleInformationInputDataProvider primaryData)
		{
			PrimarySignature.Add(primaryData.VehicleSignatureHash.ToXML(di));
		}

		private void SetResults(IPrimaryVehicleInformationInputDataProvider primaryData)
		{
			Results.Add(new XElement(tns+ XMLNames.Report_Result_Status, primaryData.ResultsInputData.Status));
			
			foreach (var resultEntry in primaryData.ResultsInputData.Results) {
				SetResult(resultEntry);
			}
		}

		private void SetResult(IResult result)
		{
			Results.Add(new XElement(
					tns + XMLNames.Report_Result_Result,
					new XAttribute(XMLNames.Report_Result_Status_Attr, result.ResultStatus),
						new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, result.VehicleGroup.GetClassNumber()),
						new XElement(tns + XMLNames.Report_Result_Mission, result.Mission.ToXMLFormat()),
						new XElement(tns + XMLNames.Report_ResultEntry_SimulationParameters,
							new XElement(
								tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
								XMLHelper.ValueAsUnit(result.SimulationParameter.TotalVehicleMass, XMLNames.Unit_kg, 2)),
							new XElement(
								tns + XMLNames.Report_Result_Payload,
								XMLHelper.ValueAsUnit(result.SimulationParameter.Payload, XMLNames.Unit_kg, 2)),
							new XElement(
								tns + XMLNames.Report_ResultEntry_PassengerCount,
								result.SimulationParameter.PassengerCount.ToXMLFormat(2)),
							new XElement(
								tns + XMLNames.Report_Result_FuelMode, result.SimulationParameter.FuelMode)
						),
						GetResultFuel(result.EnergyConsumption),
						GetResultCo2(result.CO2)
					)	
				);
		}

		private List<XElement> GetResultFuel(Dictionary<FuelType, JoulePerMeter> fuels)
		{
			var resultFuels = new List<XElement>();

			foreach (var fuel in fuels) {
				resultFuels.Add(
					new XElement(tns + XMLNames.Report_Results_Fuel,
						new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.Key.ToXMLFormat()),
						new XElement(tns + XMLNames.Report_Result_EnergyConsumption,
							XMLHelper.ValueAsUnit(fuel.Value.ConvertToMegaJouleperKilometer(), "MJ/km", 5))
						)
					);
			}

			return resultFuels;
		}

		private List<XElement> GetResultCo2(Dictionary<string, double> co2s)
		{
			var resultCo2 = new List<XElement>();

			foreach (var co2 in co2s) {
				resultCo2.Add(
					new XElement(tns + XMLNames.Report_Results_CO2,
							XMLHelper.ValueAsUnit(co2.Value, "g/km", 2)));
			}

			return resultCo2;
		}


		private void SetManufacturerRecordSignature(IPrimaryVehicleInformationInputDataProvider primaryData)
		{
			ManufacturerRecordSignature.Add(primaryData.ManufacturerRecordHash.ToXML(di));
		}

		private void SetInputDataSignature(IPrimaryVehicleInformationInputDataProvider primaryData)
		{
			InputDataSignature.Add(primaryData.PrimaryVehicleInputDataHash.ToXML(di));
		}
		
		private void SetPrimaryVehicle(IPrimaryVehicleInformationInputDataProvider primaryData)
		{
			var primaryVehicle = primaryData.Vehicle;
			
			PrimaryVehicle.Add(
				new XAttribute(xsi + "type", "VehiclePIFType"),
				new XElement(tns + XMLNames.ManufacturerPrimaryVehicle, primaryVehicle.Manufacturer),
				new XElement(tns + XMLNames.ManufacturerAddressPrimaryVehicle, primaryVehicle.ManufacturerAddress),
				new XElement(tns + XMLNames.Component_Model, primaryVehicle.Model),
				new XElement(tns + XMLNames.Vehicle_VIN, primaryVehicle.VIN),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(primaryVehicle.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, primaryVehicle.VehicleCategory.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, primaryVehicle.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_Articulated, primaryVehicle.Articulated),
				new XElement(tns + XMLNames.TPMLM, primaryVehicle.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_IdlingSpeed, primaryVehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_RetarderType, primaryVehicle.Components.RetarderInputData.Type.ToXMLFormat()),
				primaryVehicle.Components.RetarderInputData.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, primaryVehicle.Components.RetarderInputData.Ratio.ToXMLFormat(3))
					: null,
				new XElement(
					tns + XMLNames.Vehicle_AngledriveType, (primaryVehicle.Components?.AngledriveInputData?.Type ?? AngledriveType.None).ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, primaryVehicle.ZeroEmissionVehicle),
				GetADAS(primaryVehicle.ADAS),
				GetTorqueLimits(primaryVehicleInputData),
				GetVehicleComponents(primaryVehicle.Components));
		}

		private XElement GetADAS(IAdvancedDriverAssistantSystemDeclarationInputData adasData)
		{
			var ns = XNamespace.Get(adasData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Namespace);
			var versionNumber = GetNamespaceVersionNumber(ns);
			return new XElement(
				tns + XMLNames.Vehicle_ADAS,
				
				new XAttribute(
					xsi + "type",
					$"{versionNumber}:{adasData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Name}"),
				new XElement(ns + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
				new XElement(ns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
				new XElement(ns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
				new XElement(ns + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl.ToXMLFormat()),
				adasData.ATEcoRollReleaseLockupClutch != null 
					? new XElement(ns + XMLNames.Bus_ADAS_APTEcoRollReleaseLockupClutch, adasData.ATEcoRollReleaseLockupClutch)
					: null
				);
		}

		private XElement GetTorqueLimits(IPrimaryVehicleInformationInputDataProvider vehicleInputData)
		{
			var inputData = vehicleInputData.Vehicle.XMLSource;
			var tcLimits = inputData.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Vehicle_TorqueLimits));
			if (tcLimits == null)
			{
				return null;
			}

			var ns = XNamespace.Get(tcLimits.SchemaInfo.SchemaType.QualifiedName.Namespace);
			const string tclPrefix = "tcl";
			return new XElement(
				tns + XMLNames.Vehicle_TorqueLimits,
				new XAttribute(XNamespace.Xmlns + tclPrefix, ns.NamespaceName),
				new XAttribute(xsi + "type", string.Format("{0}:{1}", tclPrefix, tcLimits.SchemaInfo.SchemaType.QualifiedName.Name)),
				XElement.Parse(tcLimits.OuterXml).Elements()
			);
		}


		protected XElement GetVehicleComponents(IVehicleComponentsDeclaration vehicleComponents)
		{
			return new XElement(
				tns + XMLNames.Vehicle_Components,
				new XAttribute(xsi + "type", "VehicleComponentsPIFType"),
				
				GetEngineDescription(vehicleComponents.EngineInputData),
				GetGearboxDescription(vehicleComponents.GearboxInputData),
				GetTorqueConverterDescription(vehicleComponents.TorqueConverterInputData),
				GetAngledriveDescription(vehicleComponents.AngledriveInputData),
				GetAxelgearData(vehicleComponents.AxleGearInputData),
				GetAxleWheelDescription(vehicleComponents.AxleWheels),
				GetAuxiliariesDescription(vehicleComponents.BusAuxiliaries)
			);
		}



		#region Engine Data

		private XElement GetEngineDescription(IEngineDeclarationInputData engineData)
		{
			return new XElement(tns + XMLNames.Component_Engine,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "EngineDataPIFType"),
					GetCommonDescription(engineData),
					new XElement(tns + XMLNames.Engine_Displacement, engineData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_RatedSpeed, engineData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Engine_MaxTorque, engineData.MaxTorqueDeclared.ToXMLFormat(0)),
					new XElement(
						tns + XMLNames.Engine_WHRType,
						new XElement(
							v23 + XMLNames.Engine_WHR_MechanicalOutputICE, (engineData.WHRType & WHRType.MechanicalOutputICE) != 0),
						new XElement(
							v23 + XMLNames.Engine_WHR_MechanicalOutputIDrivetrain,
							(engineData.WHRType & WHRType.MechanicalOutputDrivetrain) != 0),
						new XElement(v23 + XMLNames.Engine_WHR_ElectricalOutput, (engineData.WHRType & WHRType.ElectricalOutput) != 0)
					),
					GetEngineModes(engineData)
				));
		}

		private List<XElement> GetEngineModes(IEngineDeclarationInputData engineData)
		{
			var fuels = new List<XElement>();
			var engineModes = engineData.EngineModes;

			foreach (var mode in engineModes) {
				fuels.Add(
					new XElement(
						tns + XMLNames.Engine_FuelModes,
						new XElement(tns + XMLNames.Engine_IdlingSpeed, mode.IdleSpeed.AsRPM.ToXMLFormat(0)),
						new XElement(
							tns + XMLNames.Engine_FullLoadAndDragCurve,
							FullLoadCurveReader.Create(mode.FullLoadCurve, true).FullLoadEntries.Select(
								x => new XElement(
									tns + XMLNames.Engine_FullLoadCurve_Entry,
									new XAttribute(XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr,
										x.EngineSpeed.AsRPM.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_MaxTorque_Attr,
										x.TorqueFullLoad.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_DragTorque_Attr,
										x.TorqueDrag.ToXMLFormat(2)))
							)),
						new XElement(
							tns + "Fuels",
							mode.Fuels.Select(x => new XElement(tns + "FuelType", x.FuelType.ToXMLFormat())))
					));
			}
			return fuels;
		}

		#endregion

		#region Gearbox/Transmission Data

		private XElement GetGearboxDescription(IGearboxDeclarationInputData transmissionData)
		{
			return new XElement(tns + XMLNames.Component_Transmission,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "TransmissionDataPIFType"),
					GetCommonDescription(transmissionData, true),
					new XElement(tns + XMLNames.Gearbox_TransmissionType, transmissionData.Type.ToXMLFormat()),
					new XElement(
						tns + XMLNames.Gearbox_Gears,
						new XAttribute(xsi + "type", "TransmissionGearsPIFType"),
						transmissionData.Gears.Select(
							x =>
								new XElement(
									tns + XMLNames.Gearbox_Gears_Gear,
									new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, x.Gear),
									new XElement(tns + XMLNames.Gearbox_Gear_Ratio, x.Ratio.ToXMLFormat(3)),
									x.MaxTorque != null
										? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, x.MaxTorque.ToXMLFormat(0))
										: null,
									x.MaxInputSpeed != null
										? new XElement(tns + XMLNames.Gearbox_Gear_MaxSpeed, x.MaxInputSpeed.AsRPM.ToXMLFormat(0))
										: null)))
				));
		}

		#endregion

		#region Torque Converter Data

		private XElement GetTorqueConverterDescription(ITorqueConverterDeclarationInputData tcData)
		{
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "TorqueConverterDataPIFType"),
					GetCommonDescription(tcData),
					GetTorqueConverterCharacteristics(tcData.TCData))
			);
		}

		private XElement GetTorqueConverterCharacteristics(TableData tcData)
		{
			if (tcData == null)
				return null;
			
			var characteristics = new List<XElement>();
			for (int i = 0; i < tcData.Rows.Count; i++) {
				var speedRatio = tcData.Rows[i][TorqueConverterDataReader.Fields.SpeedRatio].ToString();
				var torqueRatio = tcData.Rows[i][TorqueConverterDataReader.Fields.TorqueRatio].ToString();
				var torqueRef = tcData.Rows[i][TorqueConverterDataReader.Fields.CharacteristicTorque].ToString();

				characteristics.Add(
					new XElement(tns +  XMLNames.TorqueConverter_Characteristics_Entry,
						new XAttribute(XMLNames.TorqueConverterData_SpeedRatio_Attr, speedRatio),
						new XAttribute(XMLNames.TorqueConverterData_TorqueRatio_Attr, torqueRatio),
						new XAttribute(XMLNames.TorqueConverterDataMapping_InputTorqueRef_Attr, torqueRef)
					));
			}

			return characteristics.Count == 0 
				? null : new XElement(tns + XMLNames.TorqueConverter_Characteristics, characteristics);
		}

		#endregion

		#region Angledrive Data

		private XElement GetAngledriveDescription(IAngledriveInputData angledriveData)
		{
			return new XElement(tns + XMLNames.Component_Angledrive,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "AngledriveDataPIFType"),
					GetCommonDescription(angledriveData),
					new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Ratio
					)));
		}

		#endregion


		#region Axelgear Data
		
		private XElement GetAxelgearData(IAxleGearInputData axleGearData)
		{
			return new XElement(tns + XMLNames.Component_Axlegear,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "AxlegearDataPIFType"),
					GetCommonDescription(axleGearData),
					new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
					new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.Ratio.ToXMLFormat(3))));
		}

		#endregion

		#region AxleWheels Data

		private XElement GetAxleWheelDescription(IAxlesDeclarationInputData axleWheelsData)
		{
			return new XElement(
				tns + XMLNames.Component_AxleWheels,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "AxleWheelsDataPIFType"),
					XElement.Parse(axleWheelsData.XMLSource.InnerXml).Elements())
				);
		}


		#endregion


		#region Auxiliary Data

		private XElement GetAuxiliariesDescription(IBusAuxiliariesDeclarationData busAux)
		{
			var busAuxXML = busAux.XMLSource;

			return new XElement(
				tns + XMLNames.Component_Auxiliaries,
				new XElement(
					tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "AuxiliaryDataPIFType"),
					XElement.Parse(busAuxXML.InnerXml).Elements()
				));
		}
		

		#endregion



		protected virtual object[] GetCommonDescription(IComponentInputData componentInput, bool isTransmissionData = false)
		{
			var certificationMethod = componentInput.CertificationMethod != CertificationMethod.Measured ?
				componentInput.CertificationMethod.ToXMLFormat() : null;

			var certificationMethodTag = isTransmissionData
				? XMLNames.Component_Gearbox_CertificationMethod
				: XMLNames.Component_CertificationMethod;

			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, componentInput.Manufacturer),
				new XElement(tns + XMLNames.Component_Model, componentInput.Model),
				certificationMethod != null ? new XElement(tns + certificationMethodTag, certificationMethod) : null,
				new XElement(tns + XMLNames.Component_CertificationNumber, componentInput.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date,  XmlConvert.ToString(componentInput.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion,  componentInput.AppVersion)
			};
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
