using System;
using System.Collections.Generic;
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
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport : DeclarationReport<XMLDeclarationReport.ResultEntry>
	{
		public class ResultEntry
		{
			public MeterPerSecond AverageSpeed;
			public SI FcLiterPer100Km;

			public SI Co2GramPerKilometer;

			public void SetResultData(IModalDataContainer data)
			{
				AverageSpeed = data.Speed();
				FcLiterPer100Km = data.FuelConsumptionFinalLiterPer100Kilometer() ?? 0.SI();

				Co2GramPerKilometer = (data.CO2PerMeter() ?? 0.SI<KilogramPerMeter>()).ConvertTo().Gramm.Per.Kilo.Meter;
			}
		}

		protected XElement VehicleConfiguration;
		protected XElement IntegrityStatus;


		public XMLDeclarationReport()
		{
			Report = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
		}

		public XDocument Report { get; private set; }


		protected override void DoAddResult(ResultEntry entry, LoadingType loadingType, Mission mission,
			IModalDataContainer modData)
		{
			entry.SetResultData(modData);
		}

		protected internal override void DoWriteReport()
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
			XNamespace vectoNs = @"VectoOutput.XSD";
			var vectoVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

			Report.Add(new XElement("VectoOutput",
				new XAttribute("schemaVersion", "0.1"),
				new XAttribute("type", "declaration"),
				//new XAttribute("xmlns", vectoNs.NamespaceName),
				new XAttribute(xsi + "noNamespaceSchemaLocation", vectoNs.NamespaceName),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XElement("DeclarationReport",
					new XAttribute("id", GetReportID()),
					new XElement("AppVersion", vectoVersion),
					new XElement("Date", XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
					VehicleConfiguration,
					IntegrityStatus,
					new XElement("SimulationResults",
						Missions.OrderBy(m => m.Key).Select((m, i) => GetResult(m.Value, i))
						)
					),
				new XElement("Signature")
				)
				);
		}


		private static XElement[] GetResult(ResultContainer<ResultEntry> result, int i)
		{
			var retVal = new List<XElement>();
			foreach (var pair in result.ModData) {
				var data = pair.Value;
				var loading = result.Mission.Loadings[pair.Key];

				retVal.Add(new XElement("SimulationRun",
					new XElement("DrivingCycle", result.Mission.MissionType.ToString()),
					new XElement("Loading",
						new XAttribute("unit", "kg"),
						loading.ToOutputFormat(1)),
					GetFuelConsumptionReport(data, loading),
					GetCO2Report(data, loading),
					new XElement("AvgSpeed",
						new XAttribute("unit", "km/h"),
						data.AverageSpeed.ConvertTo().Kilo.Meter.Per.Hour.ToOutputFormat(3)
						)
					)
					);
			}
			return retVal.ToArray();
		}

		private static XElement GetCO2Report(ResultEntry data, Kilogram loading)
		{
			var retVal = new XElement("CO2Results",
				new XElement("CO2",
					new XAttribute("unit", "g/km"), data.Co2GramPerKilometer.ToOutputFormat(3)));
			if (!loading.IsEqual(0)) {
				retVal.Add(new XElement("CO2",
					new XAttribute("unit", "g/t.km"), (data.Co2GramPerKilometer / loading.ConvertTo().Ton).ToOutputFormat(3)));
			}
			return retVal;
		}

		private static XElement GetFuelConsumptionReport(ResultEntry data, Kilogram loading)
		{
			var retVal = new XElement("FuelConsumptionResults",
				new XElement("FuelConsumption",
					new XAttribute("unit", "l/100km"), data.FcLiterPer100Km.ToOutputFormat(3)));
			if (!loading.IsEqual(0)) {
				retVal.Add(new XElement("FuelConsumption",
					new XAttribute("unit", "l/100t.km"), (data.FcLiterPer100Km / loading.ConvertTo().Ton).ToOutputFormat(3)));
			}
			return retVal;
		}

		private static string GetReportID()
		{
			var md5 = MD5.Create();
			return string.Format("VECTO-{0}",
				Convert.ToBase64String(
					md5.ComputeHash(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)))
					)
				);
		}

		protected override void DoInitializeReport(VectoRunData modelData, Segment segment)
		{
			VehicleConfiguration = new XElement("Vehicle",
				new XAttribute("id", modelData.VehicleData.TypeId),
				GetComponentAttributes(modelData.VehicleData));
			var components = new XElement("Components",
				GetComponentDescription("Engine", modelData.EngineData),
				GetComponentDescription("Gearbox", modelData.GearboxData),
				GetComponentDescription("Axlegear", modelData.AxleGearData)
				);
			if (modelData.Retarder.Type != RetarderType.None) {
				components.Add(GetComponentDescription("Retarder", modelData.Retarder));
			}
			components.Add(GetWheelsDescription(modelData.VehicleData.AxleData));
			components.Add(GetAuxiliariesDescription(modelData.Aux));
			VehicleConfiguration.Add(components);

			IntegrityStatus = new XElement("SignatureVerification");
			IntegrityStatus.Add(GetIntegrityStatus(modelData.VehicleData));
			IntegrityStatus.Add(GetIntegrityStatus(modelData.EngineData));
			IntegrityStatus.Add(GetIntegrityStatus(modelData.GearboxData));
			IntegrityStatus.Add(GetIntegrityStatus(modelData.AxleGearData));
			if (modelData.Retarder.Type != RetarderType.None) {
				IntegrityStatus.Add(GetIntegrityStatus(modelData.Retarder));
			}
		}

		private static XElement GetIntegrityStatus(SimulationComponentData component)
		{
			return new XElement("Entry",
				new XAttribute("ref", component.TypeId),
				new XAttribute("digest", component.DigestValue),
				new XAttribute("check", component.IntegrityStatus));
		}

		private static XElement GetAuxiliariesDescription(IEnumerable<VectoRunData.AuxData> aux)
		{
			var retVal = new XElement("Component",
				new XAttribute("type", "Auxiliaries"),
				new XAttribute("id", "AUX"));
			foreach (var auxData in aux) {
				retVal.Add(new XElement("Auxiliary",
					new XAttribute("name", auxData.ID),
					new XAttribute("technology", auxData.Technology)
					));
			}
			return retVal;
		}

		private static XElement GetWheelsDescription(List<Axle> axleData)
		{
			var retVal = new XElement("Component",
				new XAttribute("type", "AxleWheels"),
				new XAttribute("id", "AXL"));

			foreach (var axle in axleData.Where(axle => axle.AxleType != AxleType.Trailer)) {
				retVal.Add(new XElement("Axle",
					new XAttribute("twinTyres", axle.TwinTyres),
					new XAttribute("axleType", axle.AxleType.ToString()),
					new XElement("Dimension", axle.WheelsDimension),
					GetComponentAttributes(axle)));
			}

			return retVal;
		}

		private static XElement GetComponentDescription(string type, SimulationComponentData component)
		{
			return new XElement("Component",
				new XAttribute("type", type),
				new XAttribute("id", component.TypeId),
				GetComponentAttributes(component));
		}

		private static XElement[] GetComponentAttributes(SimulationComponentData component)
		{
			return new[] {
				new XElement("Vendor", component.Vendor),
				new XElement("MakeAndModel", component.ModelName),
				new XElement("TypeId", component.TypeId),
				new XElement("ComponentDataHash", component.DigestValue)
			};
		}
	}
}