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
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLCustomerReport
	{
		public const string CURRENT_SCHEMA_VERSION = "0.5";

		protected readonly XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected readonly XElement Results;

		protected readonly XNamespace tns;
		protected readonly XNamespace di;
		private bool allSuccess = true;

		public XMLCustomerReport()
		{
			di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v" + CURRENT_SCHEMA_VERSION;
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + "Results");
		}

		public void Initialize(VectoRunData modelData)
		{
			var exempted = modelData.Exempted;
			VehiclePart.Add(
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				exempted ? null : new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleWeight, XMLNames.Unit_t, 1)),
					
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbWeight, XMLNames.Unit_kg)),
				exempted ? ExemptedData(modelData) : ComponentData(modelData)
				);
			InputDataIntegrity = new XElement(tns + "InputDataSignature",
				modelData.InputDataHash == null ? CreateDummySig() : new XElement(modelData.InputDataHash));
		}

		private XElement[] ExemptedData(VectoRunData modelData)
		{
			return null;
		}

		private XElement[] ComponentData(VectoRunData modelData)
		{
			return new[] {
				new XElement(
					tns + XMLNames.Report_Vehicle_EngineRatedPower,
					XMLHelper.ValueAsUnit(modelData.EngineData.RatedPowerDeclared, XMLNames.Unit_kW)),
				new XElement(
					tns + XMLNames.Report_Vehicle_EngineDisplacement,
					XMLHelper.ValueAsUnit(modelData.EngineData.Displacement, XMLNames.Unit_ltr, 1)),
				new XElement(tns + XMLNames.Engine_FuelType, modelData.EngineData.FuelType.ToXMLFormat()),
				new XElement(
					tns + XMLNames.Report_Vehicle_TransmissionCertificationMethod,
					modelData.GearboxData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, modelData.GearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, modelData.GearboxData.Gears.Count),
				new XElement(tns + XMLNames.Report_Vehicle_Retarder, modelData.Retarder.Type.IsDedicatedComponent()),
				new XElement(tns + XMLNames.Report_Vehicle_AxleRatio, modelData.AxleGearData.AxleGear.Ratio.ToXMLFormat(3)),
				new XElement(
					tns + XMLNames.Report_Vehicle_AverageRRC,
					modelData.VehicleData.AverageRollingResistanceTruck.ToXMLFormat(4))
			};

		}

		private XElement CreateDummySig()
		{
			return new XElement(di + XMLNames.DI_Signature_Reference,
				new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "null")),
				new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "NOT AVAILABLE")
				);
		}

		public void AddResult(
			DeclarationReport<XMLDeclarationReport.ResultEntry>.ResultContainer<XMLDeclarationReport.ResultEntry> entry)
		{
			foreach (var resultEntry in entry.ResultEntry) {
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
						new XElement(tns + "Error", resultEntry.Value.Error)
					};
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private object[] GetSuccessResultEntry(XMLDeclarationReport.ResultEntry result)
		{
			return new object[] {
				new XElement(tns + "Payload", new XAttribute("unit", "kg"), result.Payload.ToXMLFormat(0)),
				new XElement(tns + "FuelType", result.FuelType.ToXMLFormat()),
				new XElement(tns + "AverageSpeed", new XAttribute("unit", "km/h"), result.AverageSpeed.AsKmph.ToXMLFormat(1)),
				XMLDeclarationReport.GetResults(result, tns, false).Cast<object>().ToArray()
			};
		}

		private XElement GetApplicationInfo()
		{
			var versionNumber = VectoSimulationCore.VersionNumber;
#if RELEASE_CANDIDATE
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
				new XElement(tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		public void GenerateReport(XElement resultSignature)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"));
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(new XElement(tns + XMLNames.VectoCustomerReport,
				new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "di", di),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoOutputCustomer.{2}.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl, CURRENT_SCHEMA_VERSION)),
				new XElement(tns + "Data",
					vehicle,
					new XElement(tns + "ResultDataSignature", resultSignature),
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
