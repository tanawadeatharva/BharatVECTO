/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLManufacturerReport
	{
		protected XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected XElement Results;

		protected XNamespace tns;
		protected XNamespace di;
		private bool allSuccess = true;

		public XMLManufacturerReport()
		{
			di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.4";
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + "Results");
		}

		public void Initialize(VectoRunData modelData)
		{
			VehiclePart.Add(
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, modelData.VehicleData.GrossVehicleWeight.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, modelData.VehicleData.CurbWeight.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null),
				GetTorqueLimits(modelData.EngineData),
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
			InputDataIntegrity = new XElement(tns + XMLNames.Report_Input_Signature,
				modelData.InputDataHash == null ? CreateDummySig() : new XElement(modelData.InputDataHash));
		}

		private XElement CreateDummySig()
		{
			return new XElement(di + XMLNames.DI_Signature_Reference,
				new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "null")),
				new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "NOT AVAILABLE")
				);
		}

		private XElement GetTorqueLimits(CombustionEngineData modelData)
		{
			var limits = new List<XElement>();
			var maxTorque = modelData.FullLoadCurves[0].MaxTorque;
			for (uint i = 1; i < modelData.FullLoadCurves.Count; i++) {
				if (!maxTorque.IsEqual(modelData.FullLoadCurves[i].MaxTorque, 1e-3.SI<NewtonMeter>())) {
					limits.Add(new XElement(tns + XMLNames.Vehicle_TorqueLimits_Entry,
						new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, i),
						new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr,
							modelData.FullLoadCurves[i].MaxTorque.ToXMLFormat(0))));
				}
			}

			return limits.Count == 0
				? null
				: new XElement(tns + XMLNames.Vehicle_TorqueLimits, limits.Cast<object>().ToArray());
		}

		private XElement GetEngineDescription(CombustionEngineData engineData)
		{
			return new XElement(tns + XMLNames.Component_Engine,
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_IdlingSpeed, engineData.IdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedSpeed, engineData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_Displacement,
					engineData.Displacement.ConvertTo().Cubic.Centi.Meter.ToXMLFormat(0)),
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

		public void AddResult(
			DeclarationReport<XMLDeclarationReport.ResultEntry>.ResultContainer<XMLDeclarationReport.ResultEntry> entry)
		{
			foreach (var resultEntry in entry.ModData) {
				allSuccess &= resultEntry.Value.Status == VectoRun.Status.Success;
				Results.Add(new XElement(tns + XMLNames.Report_Result_Result,
					new XAttribute(XMLNames.Report_Result_Status_Attr,
						resultEntry.Value.Status == VectoRun.Status.Success ? "success" : "error"),
					new XElement(tns + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()),
					GetResults(resultEntry)));
			}
		}

		private object[] GetResults(KeyValuePair<LoadingType, XMLDeclarationReport.ResultEntry> resultEntry)
		{
			switch (resultEntry.Value.Status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running:
					return null; // should not happen!
				case VectoRun.Status.Success:
					return GetSuccessResultEntry(resultEntry.Value);
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					return new object[] {
						new XElement(tns + XMLNames.Report_Results_Error, resultEntry.Value.Error),
						new XElement(tns + XMLNames.Report_Results_ErrorDetails, resultEntry.Value.StackTrace),
					};
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private object[] GetSuccessResultEntry(XMLDeclarationReport.ResultEntry result)
		{
			return new object[] {
				new XElement(tns + XMLNames.Report_ResultEntry_Distance, new XAttribute(XMLNames.Report_Results_Unit_Attr, "km"),
					result.Distance.ConvertTo().Kilo.Meter.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Report_ResultEntry_SimulationParameters,
					new XElement(tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "kg"), result.TotalVehicleWeight.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Report_ResultEntry_Payload, new XAttribute(XMLNames.Report_Results_Unit_Attr, "kg"),
						result.Payload.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Report_ResultEntry_FuelType, result.FuelType.ToXMLFormat())
					),
				new XElement(tns + XMLNames.Report_ResultEntry_VehiclePerformance,
					new XElement(tns + XMLNames.Report_ResultEntry_AverageSpeed,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "km/h"), result.AverageSpeed.AsKmph.ToXMLFormat(1)),
					new XElement(tns + XMLNames.Report_ResultEntry_MinSpeed, new XAttribute(XMLNames.Report_Results_Unit_Attr, "km/h"),
						result.MinSpeed.AsKmph.ToXMLFormat(1)),
					new XElement(tns + XMLNames.Report_ResultEntry_MaxSpeed, new XAttribute(XMLNames.Report_Results_Unit_Attr, "km/h"),
						result.MaxSpeed.AsKmph.ToXMLFormat(1)),
					new XElement(tns + XMLNames.Report_ResultEntry_MaxDeceleration,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "m/s²"), result.MaxDeceleration.ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_ResultEntry_MaxAcceleration,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "m/s²"), result.MaxAcceleration.ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage,
						result.FullLoadPercentage.ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_ResultEntry_GearshiftCount, result.GearshiftCount.ToXMLFormat(0))
					),
				//FC
				XMLDeclarationReport.GetResults(result, tns, true).Cast<object>().ToArray()
			};
		}

		private XElement GetApplicationInfo()
		{
			var vectodll = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VectoCore.dll")).GetName();
			return new XElement(tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, vectodll.Version),
				new XElement(tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		public void GenerateReport()
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"));
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(new XElement(tns + XMLNames.VectoManufacturerReport,
				new XAttribute("schemaVersion", "0.4"),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "di", di),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoOutputManufacturer.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl)),
				new XElement(tns + "Data",
					vehicle,
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

		public XDocument Report { get; private set; }
	}
}
