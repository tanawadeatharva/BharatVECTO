using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport : DeclarationReport<XMLDeclarationReport.ResultEntry>
	{
		private XMLFullReport _fullReport;
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
			//CustomerReport = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

			Writer = writer;
		}

		public XDocument FullReport
		{
			get { return _fullReport.Report; }
		}


		protected override void DoAddResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			entry.SetResultData(runData, modData);
		}

		protected internal override void DoWriteReport()
		{
			foreach (var result in Missions.OrderBy(m => m.Key)) {
				_fullReport.AddResult(result.Value);
			}

			if (Writer != null) {
				var xmlWriter = new XmlTextWriter(Writer.WriteStream(ReportType.DeclarationReportXMLFulll), Encoding.UTF8) {
					Formatting = Formatting.Indented
				};
				_fullReport.Report.WriteTo(xmlWriter);
				xmlWriter.Flush();
				xmlWriter.Close();
			}

			//var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			//var xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
			//XNamespace vectoNs = @"VectoOutput.XSD";
			//var vectoVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

			//FullReport.Add(new XElement("VectoOutput",
			//	new XAttribute("schemaVersion", "0.1"),
			//	new XAttribute("type", "declaration"),
			//	//new XAttribute("xmlns", vectoNs.NamespaceName),
			//	new XAttribute(xsi + "noNamespaceSchemaLocation", vectoNs.NamespaceName),
			//	new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
			//	new XElement("DeclarationReport",
			//		new XAttribute("id", GetReportID()),
			//		new XElement("AppVersion", vectoVersion),
			//		new XElement("Date", XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
			//		VehicleConfiguration,
			//		IntegrityStatus,
			//		new XElement("SimulationResults",
			//			Missions.OrderBy(m => m.Key).Select((m, i) => GetResult(m.Value, i))
			//			)
			//		),
			//	new XElement("Signature")
			//	)
			//	);
		}


		//private static XElement[] GetResult(ResultContainer<ResultEntry> result, int i)
		//{
		//	var retVal = new List<XElement>();
		//	foreach (var pair in result.ModData) {
		//		var data = pair.Value;
		//		var loading = result.Mission.Loadings[pair.Key];

		//		retVal.Add(new XElement("SimulationRun",
		//			new XElement("DrivingCycle", result.Mission.MissionType.ToString()),
		//			new XElement("Loading",
		//				new XAttribute("unit", "kg"),
		//				loading.ToOutputFormat(1)),
		//			GetFuelConsumptionReport(data, loading),
		//			GetCO2Report(data, loading),
		//			new XElement("AvgSpeed",
		//				new XAttribute("unit", "km/h"),
		//				data.AverageSpeed.ConvertTo().Kilo.Meter.Per.Hour.ToOutputFormat(3)
		//				)
		//			)
		//			);
		//	}
		//	return retVal.ToArray();
		//}

		//private static XElement GetCO2Report(ResultEntry data, Kilogram loading)
		//{
		//	var retVal = new XElement("CO2Results",
		//		new XElement("CO2",
		//			new XAttribute("unit", "g/km"), data.Co2GramPerKilometer.ToOutputFormat(3)));
		//	if (!loading.IsEqual(0)) {
		//		retVal.Add(new XElement("CO2",
		//			new XAttribute("unit", "g/t.km"), (data.Co2GramPerKilometer / loading.ConvertTo().Ton).ToOutputFormat(3)));
		//	}
		//	return retVal;
		//}

		//private static XElement GetFuelConsumptionReport(ResultEntry data, Kilogram loading)
		//{
		//	var retVal = new XElement("FuelConsumptionResults",
		//		new XElement("FuelConsumption",
		//			new XAttribute("unit", "l/100km"), data.FcLiterPer100Km.ToOutputFormat(3)));
		//	if (!loading.IsEqual(0)) {
		//		retVal.Add(new XElement("FuelConsumption",
		//			new XAttribute("unit", "l/100t.km"), (data.FcLiterPer100Km / loading.ConvertTo().Ton).ToOutputFormat(3)));
		//	}
		//	return retVal;
		//}


		protected override void DoInitializeReport(VectoRunData modelData, Segment segment)
		{
			_fullReport.Initialize(modelData, segment);
		}
	}
}