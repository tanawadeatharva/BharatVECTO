/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLCustomerReport
	{
		protected readonly XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected readonly XElement Results;

		protected readonly XNamespace tns;
		protected readonly XNamespace di;
		private bool allSuccess = true;

		public XMLCustomerReport()
		{
			di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:COCOutput:v0.4";
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + "Results");
		}

		public void Initialize(VectoRunData modelData, Segment segment)
		{
			VehiclePart.Add(
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, modelData.VehicleData.LegislativeClass),
				new XElement(tns + "VehicleGroup", segment.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, modelData.VehicleData.GrossVehicleWeight.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, modelData.VehicleData.CurbWeight.ToXMLFormat(0)),
				new XElement(tns + "EngineRatedPower", modelData.EngineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(tns + "EngineDisplacement",
					modelData.EngineData.Displacement.ConvertTo().Cubic.Centi.Meter.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_FuelType, modelData.EngineData.FuelType.ToXMLFormat()),
				new XElement(tns + "TransmissionMainCertificationMethod", modelData.GearboxData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, modelData.GearboxData.Type.ToXMLFormat()),
				new XElement(tns + "GearsCount", modelData.GearboxData.Gears.Count),
				new XElement(tns + "Retarder", modelData.Retarder.Type.IsDedicatedComponent()),
				new XElement(tns + "AxleRatio", modelData.AxleGearData.AxleGear.Ratio.ToXMLFormat(3))
			);
			InputDataIntegrity = new XElement(tns + "InputDataSignature",
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

		public void AddResult(
			DeclarationReport<XMLDeclarationReport.ResultEntry>.ResultContainer<XMLDeclarationReport.ResultEntry> entry)
		{
			foreach (var resultEntry in entry.ModData) {
				allSuccess &= resultEntry.Value.Status == VectoRun.Status.Success;
				Results.Add(new XElement(tns + "Result",
					new XAttribute("status", resultEntry.Value.Status.ToString().ToLower()),
					new XElement(tns + "Mission", entry.Mission.ToXMLFormat()),
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
						new XElement("Error", resultEntry.Value.Error)
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
			var vectodll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "VectoCore.dll").GetName();
			return new XElement(tns + "ApplicationInformation",
				new XElement(tns + "SimulationToolVersion", vectodll.Version),
				new XElement(tns + "Date", XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		public void GenerateReport(XElement resultSignature)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + "Status", allSuccess ? "success" : "error"));
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(new XElement(tns + "VectoCustomerInformation",
					new XAttribute("schemaVersion", "0.4"),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute("xmlns", tns),
					new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(xsi + "schemaLocation",
						string.Format("{0} {1}VectoCOC.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl)),
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