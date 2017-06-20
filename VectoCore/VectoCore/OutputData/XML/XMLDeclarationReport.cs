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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport : DeclarationReport<XMLDeclarationReport.ResultEntry>
	{
		private XMLFullReport _fullReport;
		private XMLCustomerReport _customerReport;

		private IOutputDataWriter Writer;

		public class ResultEntry
		{
			public MeterPerSecond AverageSpeed { get; private set; }

			public Joule EnergyConsumptionTotal { get; private set; }

			public Kilogram CO2Total { get; private set; }

			public Kilogram FuelConsumptionTotal { get; private set; }

			public Meter Distance { get; private set; }

			public Scalar GearshiftCount { get; private set; }

			public Scalar FullLoadPercentage { get; private set; }

			public MeterPerSquareSecond MaxDeceleration { get; private set; }

			public MeterPerSquareSecond MaxAcceleration { get; private set; }

			public MeterPerSecond MaxSpeed { get; private set; }

			public MeterPerSecond MinSpeed { get; private set; }

			public string Error { get; private set; }

			public VectoRun.Status Status { get; private set; }

			public string StackTrace { get; private set; }

			public FuelType FuelType { get; private set; }

			public Kilogram Payload { get; private set; }

			public Kilogram TotalVehicleWeight { get; private set; }

			public CubicMeter CargoVolume { get; private set; }


			public void SetResultData(VectoRunData runData, IModalDataContainer data)
			{
				FuelType = data.FuelData.FuelType;
				Payload = runData.VehicleData.Loading;
				CargoVolume = runData.VehicleData.CargoVolume;
				TotalVehicleWeight = runData.VehicleData.TotalVehicleWeight;
				Status = data.RunStatus;
				Error = data.Error;
				StackTrace = data.StackTrace;
				AverageSpeed = data.Speed();
				MinSpeed = data.MinSpeed();
				MaxSpeed = data.MaxSpeed();
				MaxAcceleration = data.MaxAcceleration();
				MaxDeceleration = data.MaxDeceleration();
				FullLoadPercentage = data.EngineMaxLoadTimeShare();
				GearshiftCount = data.GearshiftCount();

				Distance = data.Distance();

				FuelConsumptionTotal = data.TimeIntegral<Kilogram>(ModalResultField.FCFinal);
				CO2Total = FuelConsumptionTotal * data.FuelData.CO2PerFuelWeight;
				EnergyConsumptionTotal = FuelConsumptionTotal * data.FuelData.LowerHeatingValue;
			}
		}

		public XMLDeclarationReport(IOutputDataWriter writer = null)
		{
			_fullReport = new XMLFullReport(); //new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
			_customerReport = new XMLCustomerReport();
			//CustomerReport = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

			Writer = writer;
		}

		public XDocument FullReport
		{
			get { return _fullReport.Report; }
		}

		public XDocument CustomerReport
		{
			get { return _customerReport.Report; }
		}


		protected override void DoAddResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			entry.SetResultData(runData, modData);
		}

		protected internal override void DoWriteReport()
		{
			foreach (var result in Missions.OrderBy(m => m.Key)) {
				_fullReport.AddResult(result.Value);
				_customerReport.AddResult(result.Value);
			}

			_fullReport.GenerateReport();
			var fullReportHash = GetSignature(_fullReport.Report);
			_customerReport.GenerateReport(fullReportHash);

			if (Writer != null) {
				using (var xmlWriter = new XmlTextWriter(Writer.WriteStream(ReportType.DeclarationReportXMLFulll), Encoding.UTF8)) {
					xmlWriter.Formatting = Formatting.Indented;
					_fullReport.Report.WriteTo(xmlWriter);
					xmlWriter.Flush();
					xmlWriter.Close();
				}

				using (var xmlWriter = new XmlTextWriter(Writer.WriteStream(ReportType.DeclarationReportXMLCOC), Encoding.UTF8)) {
					xmlWriter.Formatting = Formatting.Indented;
					_customerReport.Report.WriteTo(xmlWriter);
					xmlWriter.Flush();
					xmlWriter.Close();
				}
			}
		}

		private XElement GetSignature(XDocument report)
		{
			return report.XPathSelectElement("/*[local-name()='VectoOutput']/*[local-name()='Signature']/*");
		}


		protected override void DoInitializeReport(VectoRunData modelData, Segment segment)
		{
			_fullReport.Initialize(modelData, segment);
			_customerReport.Initialize(modelData, segment);
		}


		public static List<XElement> GetResults(XMLDeclarationReport.ResultEntry result, XNamespace tns, bool fullOutput)
		{
			var fuel = FuelData.Instance().Lookup(result.FuelType);
			var retVal = new List<XElement>();
			//FC
			retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "g/km"),
				(result.FuelConsumptionTotal.ConvertTo().Gramm / result.Distance.ConvertTo().Kilo.Meter).Value()
					.ToMinSignificantDigits(3, 1)));
			retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "g/t-km"),
				(result.FuelConsumptionTotal.ConvertTo().Gramm / result.Distance.ConvertTo().Kilo.Meter /
				result.Payload.ConvertTo().Ton).Value().ToMinSignificantDigits(3, 1)));
			retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "g/m³-km"),
				(result.FuelConsumptionTotal.ConvertTo().Gramm / result.Distance.ConvertTo().Kilo.Meter / result.CargoVolume).Value()
					.ToMinSignificantDigits(3, 1)));
			if (fullOutput) {
				retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "MJ/km"),
					(result.EnergyConsumptionTotal / result.Distance.ConvertTo().Kilo.Meter / 1e6).Value().ToMinSignificantDigits(3, 1)));
				retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "MJ/t-km"),
					(result.EnergyConsumptionTotal / result.Distance.ConvertTo().Kilo.Meter / result.Payload.ConvertTo().Ton / 1e6)
						.Value().ToMinSignificantDigits(3, 1)));
				retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "MJ/m³-km"),
					(result.EnergyConsumptionTotal / result.Distance.ConvertTo().Kilo.Meter / result.CargoVolume / 1e6).Value()
						.ToMinSignificantDigits(3, 1)));
			}
			if (fuel.FuelDensity != null) {
				retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "l/100km"),
					(result.FuelConsumptionTotal.ConvertTo().Gramm / fuel.FuelDensity / result.Distance.ConvertTo().Kilo.Meter * 100)
						.Value().ToMinSignificantDigits(3, 1)));
				retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "l/t-km"),
					(result.FuelConsumptionTotal.ConvertTo().Gramm / fuel.FuelDensity / result.Distance.ConvertTo().Kilo.Meter /
					result.Payload.ConvertTo().Ton).Value().ToMinSignificantDigits(3, 1)));
				retVal.Add(new XElement(tns + "FuelConsumption", new XAttribute("unit", "l/m³-km"),
					(result.FuelConsumptionTotal.ConvertTo().Gramm / fuel.FuelDensity / result.Distance.ConvertTo().Kilo.Meter /
					result.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
			}
			//CO2
			retVal.Add(new XElement(tns + "CO2", new XAttribute("unit", "g/km"),
				(result.CO2Total.ConvertTo().Gramm / result.Distance.ConvertTo().Kilo.Meter).Value().ToMinSignificantDigits(3, 1)));
			retVal.Add(new XElement(tns + "CO2", new XAttribute("unit", "g/t-km"),
				(result.CO2Total.ConvertTo().Gramm / result.Distance.ConvertTo().Kilo.Meter /
				result.Payload.ConvertTo().Ton).Value().ToMinSignificantDigits(3, 1)));
			retVal.Add(new XElement(tns + "CO2", new XAttribute("unit", "g/m³-km"),
				(result.CO2Total.ConvertTo().Gramm / result.Distance.ConvertTo().Kilo.Meter / result.CargoVolume).Value()
					.ToMinSignificantDigits(3, 1)));

			return retVal;
		}
	}
}