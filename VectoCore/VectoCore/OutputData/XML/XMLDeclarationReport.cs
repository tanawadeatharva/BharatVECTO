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
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport : DeclarationReport<XMLDeclarationReport.ResultEntry>
	{
		private readonly XMLManufacturerReport _manufacturerReport;
		private readonly XMLCustomerReport _customerReport;

		private readonly IOutputDataWriter _writer;

		private IDictionary<Tuple<MissionType, LoadingType>, double> _weightingFactors;

		public class ResultEntry
		{
			public MeterPerSecond AverageSpeed { get; private set; }

			public MeterPerSecond AverageDrivingSpeed { get; private set; }

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
			public PerSecond EngineSpeedDrivingMin { get; private set; }
			public PerSecond EngineSpeedDrivingAvg { get; private set; }
			public PerSecond EngineSpeedDrivingMax { get; private set; }

			public double AverageGearboxEfficiency { get; private set; }

			public double AverageAxlegearEfficiency { get; private set; }

			public double WeightingFactor { get; set; }


			public virtual void SetResultData(VectoRunData runData, IModalDataContainer data, double weightingFactor)
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

				var entriesDriving = data.GetValues(
					r => new {
						dt = r.Field<Second>((int)ModalResultField.simulationInterval),
						v = r.Field<MeterPerSecond>((int)ModalResultField.v_act),
						nEng = r.Field<PerSecond>((int)ModalResultField.n_eng_avg)
					}).Where(x => x.v.IsGreater(0)).ToArray();
				var drivingTime = entriesDriving.Sum(x => x.dt);

				AverageDrivingSpeed = entriesDriving.Sum(x => x.v * x.dt) / drivingTime;
				EngineSpeedDrivingAvg = (entriesDriving.Sum(x => (x.nEng * x.dt).Value()) / drivingTime.Value()).SI<PerSecond>();
				EngineSpeedDrivingMin = entriesDriving.Min(x => x.nEng);
				EngineSpeedDrivingMax = entriesDriving.Max(x => x.nEng);
				Distance = data.Distance();

				FuelConsumptionTotal = data.TimeIntegral<Kilogram>(ModalResultField.FCFinal);
				CO2Total = FuelConsumptionTotal * data.FuelData.CO2PerFuelWeight;
				EnergyConsumptionTotal = FuelConsumptionTotal * data.FuelData.LowerHeatingValueVecto;

				var gbxOutSignal = runData.Retarder.Type == RetarderType.TransmissionOutputRetarder
					? ModalResultField.P_retarder_in
					: (runData.AngledriveData == null ? ModalResultField.P_axle_in : ModalResultField.P_angle_in);
				var eGbxIn = data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
				var eGbxOut = data.TimeIntegral<WattSecond>(gbxOutSignal, x => x > 0);
				AverageGearboxEfficiency = eGbxOut / eGbxIn;

				var eAxlIn = data.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
				var eAxlOut = data.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, x => x > 0);
				AverageAxlegearEfficiency = eAxlOut / eAxlIn;

				WeightingFactor = weightingFactor;
			}

		}

		public XMLDeclarationReport(IOutputDataWriter writer = null)
		{
			_manufacturerReport = new XMLManufacturerReport();
			_customerReport = new XMLCustomerReport();
			
			_writer = writer;
		}

		public XDocument FullReport
		{
			get { return _manufacturerReport.Report; }
		}

		public XDocument CustomerReport
		{
			get { return _customerReport.Report; }
		}


		protected override void DoAddResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			var factor = _weightingFactors[Tuple.Create(runData.Mission.MissionType, runData.Loading)];
			entry.SetResultData(runData, modData, factor);
		}

		protected internal override void DoWriteReport()
		{
			foreach (var result in Missions.OrderBy(m => m.Key)) {
				_manufacturerReport.WriteResult(result.Value);
				_customerReport.WriteResult(result.Value);
			}

			_manufacturerReport.GenerateReport();
			var fullReportHash = GetSignature(_manufacturerReport.Report);
			_customerReport.GenerateReport(fullReportHash);

			if (_writer != null) {
				_writer.WriteReport(ReportType.DeclarationReportCustomerXML, _customerReport.Report);
				_writer.WriteReport(ReportType.DeclarationReportManufacturerXML, _manufacturerReport.Report);
			}
		}

		private XElement GetSignature(XDocument report)
		{
			return report.XPathSelectElement("/*[local-name()='VectoOutput']/*[local-name()='Signature']/*");
		}


		public override void InitializeReport(VectoRunData modelData)
		{
			var weightingGroup = DeclarationData.WeightingGroup.Lookup(
				modelData.VehicleData.VehicleClass, modelData.VehicleData.SleeperCab,
				modelData.EngineData.RatedPowerDeclared);
			_weightingFactors = weightingGroup == WeightingGroup.Unknown ? ZeroWeighting : DeclarationData.WeightingFactors.Lookup(weightingGroup);
			_manufacturerReport.Initialize(modelData);
			_customerReport.Initialize(modelData);
		}

		private static IDictionary<Tuple<MissionType, LoadingType>, double> ZeroWeighting
		{
			get {
				return new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(new Dictionary<Tuple<MissionType, LoadingType>, double>() {
					{ Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad), 0},
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad), 0},
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad), 0},
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.ReferenceLoad), 0},
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad), 0},
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad), 0},
					{ Tuple.Create(MissionType.Construction, LoadingType.LowLoading), 0},
					{ Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad), 0},
				});
			}
		}


		public static IEnumerable<XElement> GetResults(ResultEntry result, XNamespace tns, bool fullOutput)
		{
			var fuel = FuelData.Instance().Lookup(result.FuelType);
			var retVal = new List<XElement> {
				new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(result.FuelConsumptionTotal / result.Distance).ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 1)),
				new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					(result.FuelConsumptionTotal / result.Distance / result.Payload).ConvertToGrammPerTonKilometer().ToMinSignificantDigits(3, 1)),
				result.CargoVolume > 0
					? new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						(result.FuelConsumptionTotal.ConvertToGramm() / result.Distance.ConvertToKiloMeter() / result.CargoVolume)
							.Value
							().ToMinSignificantDigits(3, 1))
					: null
			};
			//FC
			if (fullOutput) {
				retVal.Add(new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/km"),
					(result.EnergyConsumptionTotal / result.Distance.ConvertToKiloMeter() / 1e6).Value().ToMinSignificantDigits(3, 1)));
				retVal.Add(new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/t-km"),
					(result.EnergyConsumptionTotal / result.Distance.ConvertToKiloMeter() / result.Payload.ConvertToTon() / 1e6)
						.Value().ToMinSignificantDigits(3, 1)));
				if (result.CargoVolume > 0)
					retVal.Add(new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/m³-km"),
						(result.EnergyConsumptionTotal / result.Distance.ConvertToKiloMeter() / result.CargoVolume / 1e6).Value()
							.ToMinSignificantDigits(3, 1)));
				}
			if (fuel.FuelDensity != null) {
				retVal.Add(new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
					(result.FuelConsumptionTotal.ConvertToGramm() / fuel.FuelDensity / result.Distance.ConvertToKiloMeter() * 100)
						.Value().ToMinSignificantDigits(3, 1)));
				retVal.Add(new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
					(result.FuelConsumptionTotal.ConvertToGramm() / fuel.FuelDensity / result.Distance.ConvertToKiloMeter() /
					 result.Payload.ConvertToTon()).Value().ToMinSignificantDigits(3, 1)));
				if (result.CargoVolume > 0)
					retVal.Add(new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
						(result.FuelConsumptionTotal.ConvertToGramm() / fuel.FuelDensity / result.Distance.ConvertToKiloMeter() /
						result.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
				}
			//CO2
			retVal.Add(new XElement(tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
				(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3, 1)));
			retVal.Add(new XElement(tns + XMLNames.Report_Results_CO2,
				new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
				(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() /
				 result.Payload.ConvertToTon()).ToMinSignificantDigits(3, 1)));
			if (result.CargoVolume > 0)
				retVal.Add(new XElement(tns + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() / result.CargoVolume).Value()
						.ToMinSignificantDigits(3, 1)));
			

			return retVal;
		}
	}
}