using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.OutputData.XML {
	internal class XMLVTPReport : DeclarationReport<XMLDeclarationReport.ResultEntry>, IVTPReport
	{
		public const string CURRENT_SCHEMA_VERSION = "0.1";

		protected XElement VehiclePart;

		protected XElement Results;

		protected XNamespace tns;

		private IOutputDataWriter _writer;

		//protected XNamespace di;
		//private bool allSuccess = true;

		public XMLVTPReport(IOutputDataWriter writer)
		{
			//di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:VTPReport:v" + CURRENT_SCHEMA_VERSION;
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + "Results");

			_writer = writer;
		}


		#region Overrides of DeclarationReport<ResultEntry>

		protected override void DoAddResult(XMLDeclarationReport.ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			entry.SetResultData(runData, modData);
		}

		protected internal override void DoWriteReport()
		{
			var report = GenerateReport();
			if (_writer != null) {
				_writer.WriteReport(ReportType.DeclarationVTPReportXML, report);
			}
		}

		private XDocument GenerateReport()
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(new XElement(tns + XMLNames.VectoManufacturerReport,
									new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
									new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
									new XAttribute("xmlns", tns),
									//new XAttribute(XNamespace.Xmlns + "di", di),
									new XAttribute(xsi + "schemaLocation",
													string.Format("{0} {1}VTPReport.{2}.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl, CURRENT_SCHEMA_VERSION)),
									new XElement(tns + "Data",
												new XElement(VehiclePart)
												//results,
												//GetApplicationInfo()
											)
						)
			);

			return retVal;
		}

		public override void InitializeReport(VectoRunData modelData)
		{
			VehiclePart.Add(
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, modelData.VehicleData.GrossVehicleWeight.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, modelData.VehicleData.CurbWeight.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null),
				
				new XElement(tns + XMLNames.Vehicle_Components,
							GetEngineDescription(modelData.EngineData),
							GetGearboxDescription(modelData.GearboxData),
							GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
							GetRetarderDescription(modelData.Retarder),
							GetAngledriveDescription(modelData.AngledriveData),
							GetAxlegearDescription(modelData.AxleGearData),
							GetAirDragDescription(modelData.AirdragData),
							GetAxleWheelsDescription(modelData.VehicleData),
							GetAuxiliariesDescription(modelData.Aux)
				)
			);
		}

		#endregion

		private XElement GetEngineDescription(CombustionEngineData engineData)
		{
			return new XElement(tns + XMLNames.Component_Engine,
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_IdlingSpeed, engineData.IdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedSpeed, engineData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_Displacement,
					engineData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_FuelType, engineData.FuelType.ToXMLFormat())
				);
		}

		private XElement GetGearboxDescription(GearboxData gearboxData)
		{
			return new XElement(tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears.Last().Value.Ratio.ToXMLFormat(3))
				);
		}

		private XElement GetTorqueConverterDescription(TorqueConverterData torqueConverterData)
		{
			if (torqueConverterData == null) {
				return null;
			}
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				GetCommonDescription(torqueConverterData));
		}

		private XElement GetRetarderDescription(RetarderData retarder)
		{
			return new XElement(tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.Vehicle_RetarderType, retarder.Type.ToXMLFormat()),
				retarder.Type.IsDedicatedComponent() ? GetCommonDescription(retarder) : null);
		}

		private object GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) {
				return null;
			}
			return new XElement(tns + XMLNames.Component_Angledrive,
				GetCommonDescription(angledriveData),
				new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio));
		}

		private XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return new XElement(tns + XMLNames.Component_Axlegear,
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAirDragDescription(AirdragData airdragData)
		{
			if (airdragData.CertificationMethod == CertificationMethod.StandardValues) {
				return new XElement(tns + XMLNames.Component_AirDrag,
					new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
					new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
					);
			}
			return new XElement(tns + XMLNames.Component_AirDrag,
				new XElement(tns + XMLNames.Component_Model, airdragData.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, airdragData.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, airdragData.DigestValueInput),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
				);
		}

		private XElement GetAxleWheelsDescription(VehicleData vehicleData)
		{
			var retVal = new XElement(tns + XMLNames.Component_AxleWheels);
			var axleData = vehicleData.AxleData;
			for (var i = 0; i < axleData.Count; i++) {
				if (axleData[i].AxleType == AxleType.Trailer) {
					continue;
				}
				retVal.Add(GetAxleDescription(i + 1, axleData[i]));
			}

			return retVal;
		}

		private XElement GetAxleDescription(int i, Axle axle)
		{
			return new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i),
				new XElement(tns + XMLNames.Report_Tyre_TyreDimension, axle.WheelsDimension),
				new XElement(tns + XMLNames.Report_Tyre_TyreCertificationNumber, axle.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, axle.DigestValueInput),
				new XElement(tns + XMLNames.Report_Tyre_TyreRRCDeclared, axle.RollResistanceCoefficient.ToXMLFormat(4)),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres, axle.TwinTyres));
		}

		private XElement GetAuxiliariesDescription(IEnumerable<VectoRunData.AuxData> aux)
		{
			var auxData = aux.ToDictionary(a => a.ID);
			var auxList = new[] {
				AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
				AuxiliaryType.HVAC
			};
			var retVal = new XElement(tns + XMLNames.Component_Auxiliaries);
			foreach (var auxId in auxList) {
				foreach (var entry in auxData[auxId.Key()].Technology) {
					retVal.Add(new XElement(tns + GetTagName(auxId), entry));
				}
			}
			return retVal;
		}

		private string GetTagName(AuxiliaryType auxId)
		{
			return auxId.ToString() + "Technology";
		}

		private object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, data.DigestValueInput)
			};
		}

		private object[] GetCommonDescription(SimulationComponentData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, data.DigestValueInput)
			};
		}
	}
}