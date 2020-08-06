using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLCustomerReportCompletedBus : XMLCustomerReport
	{
		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleRecordFile { get; set; }

		public IVehicleDeclarationInputData PrimaryVehicle { get { return PrimaryVehicleRecordFile.Vehicle; } }

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleCompletedBusType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(
					tns + "PrimaryVehicle",
					new XElement(tns + XMLNames.Component_Manufacturer, PrimaryVehicle.Manufacturer),
					new XElement(tns + XMLNames.Component_ManufacturerAddress, PrimaryVehicle.ManufacturerAddress)
				),
				new XElement(
					tns + "CompletedVehicle",

					new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
					new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress)
				),

				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + "VehicleCategory", modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_RegisteredClass, modelData.VehicleData.RegisteredClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_VehicleCode, modelData.VehicleData.VehicleCode.ToXMLFormat()),

				new XElement(
					tns + XMLNames.TPMLM,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 1)),
				new XElement(
					tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.VehicleData.DualFuelVehicle),

				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
				new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
				new XElement(
					tns + "RegisteredPassengers",
					new XElement(tns + XMLNames.Bus_LowerDeck, modelData.VehicleData.InputData.NumberOfPassengersLowerDeck),
					new XElement(tns + XMLNames.Bus_UpperDeck, modelData.VehicleData.InputData.NumberOfPassengersUpperDeck)
				),
				new XElement(tns + XMLNames.Bus_LowEntry, modelData.VehicleData.InputData.LowEntry),
				new XElement(tns + XMLNames.Bus_HeighIntegratedBody, modelData.VehicleData.InputData.Height.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Bus_VehicleLength, modelData.VehicleData.InputData.Length.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Bus_VehicleWidth, modelData.VehicleData.InputData.Width.ToXMLFormat(3)),
				GetADAS(modelData.VehicleData.ADAS),
				ComponentData(
					modelData,
					PrimaryVehicleRecordFile.ResultsInputData.Results
											.Select(x => x.EnergyConsumption.Keys.Select(f => FuelData.Instance().Lookup(f, modelData.VehicleData.InputData.TankSystem)).ToList()).Distinct()
											.ToList())
			);
			
			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
				modelData.InputDataHash == null ? XMLHelper.CreateDummySig(di) : new XElement(modelData.InputDataHash));
		}

		internal void WriteResult(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			_allSuccess &= genericResult.Status == VectoRun.Status.Success;
			_allSuccess &= specificResult.Status == VectoRun.Status.Success;
			Results.Add(
				genericResult.Status == VectoRun.Status.Success && specificResult.Status == VectoRun.Status.Success
					? GetSuccessResultEntry(genericResult, specificResult, primaryResult)
					: GetErrorResultEntry(genericResult, specificResult, primaryResult));
		}

		private XElement GetErrorResultEntry(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{

			object[] content = null;
			if (genericResult.Status == VectoRun.Status.Pending || genericResult.Status == VectoRun.Status.Running ||
				specificResult.Status == VectoRun.Status.Pending || specificResult.Status == VectoRun.Status.Running) {
				content = new object[] {
					new XElement(
						tns + XMLNames.Report_Results_Error,
						string.Format("Simulation not finished! Status: {0} / {1}", genericResult.Status, specificResult.Status)),
					new XElement(tns + XMLNames.Report_Results_ErrorDetails, ""),
				}; // should not happen!
			}

			if (genericResult.Status == VectoRun.Status.Canceled || genericResult.Status == VectoRun.Status.Aborted || specificResult.Status == VectoRun.Status.Canceled || specificResult.Status == VectoRun.Status.Aborted) {
				content = new object[] {
					new XElement(tns + XMLNames.Report_Results_Error, genericResult.Error ?? "" + Environment.NewLine +  specificResult.Error ?? ""),
					new XElement(tns + XMLNames.Report_Results_ErrorDetails, genericResult.StackTrace ?? "" + Environment.NewLine + specificResult.StackTrace ?? ""),
				};
			}

			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				new XElement(tns + XMLNames.Report_Result_Mission, genericResult.Mission.ToXMLFormat()),
				GetSimulationParameters(specificResult, primaryResult),
				content);
		}

		private XElement GetSuccessResultEntry(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessType"),
				new XElement(tns + XMLNames.Report_Result_Mission, genericResult.Mission.ToXMLFormat()),
				//new XElement(tns + XMLNames.Report_ResultEntry_Distance, XMLHelper.ValueAsUnit(specificResult.Distance, XMLNames.Unit_km, 3)),
				GetSimulationParameters(specificResult, primaryResult),
				new XElement(
					tns + XMLNames.Report_ResultEntry_AverageSpeed,
					XMLHelper.ValueAsUnit(specificResult.AverageSpeed, XMLNames.Unit_kmph, 1)),
				GetFuelConsumptionResults(genericResult, specificResult, primaryResult)
			);

		}

		protected virtual XElement[] GetSimulationParameters(XMLDeclarationReport.ResultEntry result, IResult primaryResult)
		{
			return new XElement[] {
				new XElement(
					tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(result.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
				result.PassengerCount.HasValue && result.PassengerCount.Value > 0 ? new XElement(tns + "PassengerCount", result.PassengerCount.Value.ToMinSignificantDigits(3, 1)) : null,
				new XElement(tns + XMLNames.Report_Result_FuelMode, primaryResult.SimulationParameter.FuelMode)
			};
		}

		private XElement[] GetFuelConsumptionResults(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			var factor = XMLManufacturerReportCompletedBus.CalculateFactorMethodFactor(primaryResult, specificResult,
					genericResult);
			//var factor = specificResult.EnergyConsumptionTotal.Value() / genericResult.EnergyConsumptionTotal.Value();
			var retVal = new List<XElement>();

			var co2Sum = 0.SI<KilogramPerMeter>();
			foreach (var entry in primaryResult.EnergyConsumption) {
				var fcEnergy = entry.Value * factor;  // J/m
				var fuelData = FuelData.Instance().Lookup(entry.Key);
				var fcMass = fcEnergy / fuelData.LowerHeatingValueVecto; // kg/m
				co2Sum += fcMass * fuelData.CO2PerFuelWeight;

				var fcResult = new XElement(tns + XMLNames.Report_Results_Fuel, new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuelData.FuelType.ToXMLFormat()));
				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						fcMass.ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 1)),
					//new XElement(
					//	tns + XMLNames.Report_Results_FuelConsumption,
					//	new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					//	(fcMass / specificResult.Payload)
					//	.ConvertToGrammPerTonKilometer().ToMinSignificantDigits(3, 1)),
					specificResult.CargoVolume > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
							(fcMass.ConvertToGrammPerKiloMeter() / specificResult.CargoVolume)
							.Value().ToMinSignificantDigits(3, 1))
						: null,
					specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/p-km"),
							(fcMass.ConvertToGrammPerKiloMeter() / specificResult.PassengerCount.Value).ToMinSignificantDigits(3, 1))
						: null
				);
				if (fuelData.FuelDensity != null) {
					fcResult.Add(
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
							(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity * 100).Value().ToMinSignificantDigits(3, 1)),
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
							(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity /
							specificResult.Payload.ConvertToTon()).Value().ToMinSignificantDigits(3, 1)));
					if (specificResult.CargoVolume > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
								(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity /
								specificResult.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
					}
					if (specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/p-km"),
								(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity /
								specificResult.PassengerCount.Value).Value().ToMinSignificantDigits(3, 1))
						);
					}
				}
				retVal.Add(fcResult);
			}

			//CO2
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					co2Sum.ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 2)));
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					(co2Sum.ConvertToGrammPerKiloMeter() / specificResult.Payload.ConvertToTon()).ToMinSignificantDigits(3, 2)));
			if (specificResult.CargoVolume > 0)
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						(co2Sum.ConvertToGrammPerKiloMeter() / specificResult.CargoVolume).Value().ToMinSignificantDigits(3, 2)));
			if (specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0) {
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/p-km"),
						(co2Sum.ConvertToGrammPerKiloMeter() / specificResult.PassengerCount.Value).ToMinSignificantDigits(3, 2)));
			}

			_weightedPayload += specificResult.Payload * specificResult.WeightingFactor;
			_weightedCo2 += co2Sum * specificResult.WeightingFactor;

			return retVal.ToArray();
		}

		public override void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			throw new NotSupportedException();
		}

		
	}
}