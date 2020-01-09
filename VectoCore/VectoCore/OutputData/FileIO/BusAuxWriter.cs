using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class BusAuxWriter
	{
		public static bool SaveAuxConfig(IAuxiliaryConfig auxCfg, string auxFile)
		{
			var returnValue = true;

			// JSON METHOD
			try {
				var body = new Dictionary<string, object>();
				var basePath = Path.GetDirectoryName(Path.GetFullPath(auxFile));
				body["ElectricalUserInputsConfig"] = SaveElectricalConfig(auxCfg.ElectricalUserInputsConfig, basePath);
				body["PneumaticAuxillariesConfig"] = SavePneumaticAuxconfig(auxCfg.PneumaticAuxillariesConfig);
				body["PneumaticUserInputsConfig"] = SavePneumaticUserConfig(auxCfg.PneumaticUserInputsConfig, basePath);
				//body["HvacUserInputsConfig"] = SaveHVACUserConfig();

				body["SSMFilePath"] = string.IsNullOrWhiteSpace(auxCfg.SSMInputs.Source)
					? ""
					: JSONFileWriter.GetRelativePath(auxCfg.SSMInputs.Source, basePath);
				body["EnvironmentalConditions"] = string.IsNullOrWhiteSpace(auxCfg.SSMInputs.EnvironmentalConditions.Source)
					? ""
					: JSONFileWriter.GetRelativePath(auxCfg.SSMInputs.EnvironmentalConditions.Source, basePath);
				body["SSMTechologies"] = string.IsNullOrWhiteSpace(auxCfg.SSMInputs.Technologies.Source)
					? ""
					: JSONFileWriter.GetRelativePath(auxCfg.SSMInputs.Technologies.Source, basePath);
				body["Actuations"] = new Dictionary<string, object>() {
					{"Brakes", auxCfg.Actuations.Braking },
					{"Park brake + 2 doors", auxCfg.Actuations.ParkBrakeAndDoors },
					{"Kneeling", auxCfg.Actuations.Kneeling },
					{"CycleTime", auxCfg.Actuations.CycleTime.Value() }
				};
				
				JSONInputDataFactory.WriteFile(JToken.FromObject(new Dictionary<string, object>() { { "Header", "AAUX" }, { "Body", body } }), auxFile);
			} catch (Exception) {
				returnValue = false;
			}

			return returnValue;
		}

		protected static Dictionary<string, object> SaveElectricalConfig(IElectricsUserInputsConfig electricalUserCfg, string auxPath)
		{
			var elData = new Dictionary<string, object>();
			// AlternatorGearEfficiency
			elData["AlternatorGearEfficiency"] = electricalUserCfg.AlternatorGearEfficiency;

			// AlternatorMap
			elData["AlternatorMap"] = JSONFileWriter.GetRelativePath(electricalUserCfg.AlternatorMap.Source, auxPath);

			// DoorActuationTimeSecond
			elData["DoorActuationTimeSecond"] = electricalUserCfg.DoorActuationTimeSecond.Value();

			// Electrical Consumer list

			var elConsumers = new List<object>();
			foreach (var entry in electricalUserCfg.ElectricalConsumers.Items) {
				var newConsumer = new Dictionary<string, object>();

				newConsumer["BaseVehicle"] = entry.BaseVehicle;
				newConsumer["Category"] = entry.Category;
				newConsumer["ConsumerName"] = entry.ConsumerName;
				newConsumer["NominalConsumptionAmps"] = entry.NominalConsumptionAmps.Value();
				newConsumer["PhaseIdle_TractionOn"] = entry.PhaseIdle_TractionOn;
				newConsumer["PowerNetVoltage"] = entry.PowerNetVoltage.Value();
				newConsumer["NumberInActualVehicle"] = entry.NumberInActualVehicle;
				newConsumer["Info"] = entry.Info;

				elConsumers.Add(newConsumer);
			}

			elData["ElectricalConsumers"] = elConsumers;

			// PowerNetVoltage
			elData["PowerNetVoltage"] = electricalUserCfg.PowerNetVoltage.Value();

			// ResultCardIdle
			var resultCard = new List<object>();
			foreach (var result in ((ResultCard)electricalUserCfg.ResultCardIdle).Results) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Amps.Value()},
					{"SmartAmps",result.SmartAmps.Value() }
				});
			}

			elData["ResultCardIdle"] = resultCard;


			// ResultCardOverrun
			resultCard.Clear();
			foreach (var result in ((ResultCard)electricalUserCfg.ResultCardOverrun).Results) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Amps.Value()},
					{"SmartAmps",result.SmartAmps.Value() }
				});
			}

			elData["ResultCardOverrun"] = resultCard;

			// ResultCardTraction
			resultCard.Clear();
			foreach (var result in ((ResultCard)electricalUserCfg.ResultCardTraction).Results) {
				resultCard.Add(new Dictionary<string, object>() {
					{ "Amps",  result.Amps.Value()},
					{"SmartAmps",result.SmartAmps.Value() }
				});
			}

			elData["ResultCardTraction"] = resultCard;

			// SmartElectrical
			elData["SmartElectrical"] = electricalUserCfg.SmartElectrical;

			return elData;
		}


		private static Dictionary<string, object> SavePneumaticUserConfig(IPneumaticUserInputsConfig pneumaticUserCfg, string auxPath)
		{
			var puData = new Dictionary<string, object>();

			puData["AdBlueDosing"] = pneumaticUserCfg.AdBlueDosing;
			puData["AirSuspensionControl"] = pneumaticUserCfg.AirSuspensionControl.ToString();
			puData["CompressorGearEfficiency"] = pneumaticUserCfg.CompressorGearEfficiency;
			puData["CompressorGearRatio"] = pneumaticUserCfg.CompressorGearRatio;
			puData["CompressorMap"] = pneumaticUserCfg.CompressorMap == null ? "" : JSONFileWriter.GetRelativePath(pneumaticUserCfg.CompressorMap.Source, auxPath);
			puData["Doors"] = pneumaticUserCfg.Doors;
			puData["KneelingHeightMillimeters"] = pneumaticUserCfg.KneelingHeightMillimeters.ConvertToMilliMeter().Value;
			//puData["RetarderBrake"] = pneumaticUserCfg.RetarderBrake;
			puData["SmartAirCompression"] = pneumaticUserCfg.SmartAirCompression;
			puData["SmartRegeneration"] = pneumaticUserCfg.SmartRegeneration;

			return puData;
		}

		protected static Dictionary<string, object> SavePneumaticAuxconfig(IPneumaticsConsumersDemand pneumaticAuxCfg)
		{
			var paData = new Dictionary<string, object>();

			paData["AdBlueNIperMinute"] = pneumaticAuxCfg.AdBlueInjection.ConvertToNlPerMin().Value;
			paData["AirControlledSuspensionNIperMinute"] = pneumaticAuxCfg.AirControlledSuspension.ConvertToNlPerMin().Value;
			paData["BrakingNIperKG"] = pneumaticAuxCfg.Braking.Value();
			paData["BreakingPerKneelingNIperKGinMM"] = pneumaticAuxCfg.BreakingWithKneeling.Value() / 1000;
			paData["DeadVolBlowOutsPerLitresperHour"] = pneumaticAuxCfg.DeadVolBlowOuts.ConvertToPerHour().Value;
			paData["DeadVolumeLitres"] = pneumaticAuxCfg.DeadVolume.Cast<CubicMeter>().ConvertToCubicDeziMeter().Value;
			paData["NonSmartRegenFractionTotalAirDemand"] = pneumaticAuxCfg.NonSmartRegenFractionTotalAirDemand;
			paData["PerDoorOpeningNI"] = pneumaticAuxCfg.DoorOpening.Cast<CubicMeter>().ConvertToCubicDeziMeter().Value;
			paData["PerStopBrakeActuationNIperKG"] = pneumaticAuxCfg.StopBrakeActuation.Value();
			paData["SmartRegenFractionTotalAirDemand"] = pneumaticAuxCfg.SmartRegenFractionTotalAirDemand;
			paData["OverrunUtilisationForCompressionFraction"] =
				pneumaticAuxCfg.OverrunUtilisationForCompressionFraction;

			return paData;
		}



		public static bool SaveSSMConfig(ISSMInputs ssmInput, string filePath)
		{
			var returnValue = true;
			try {
				var body = new Dictionary<string, object>();
				body["SSMDisabled"] = ssmInput.SSMDisabled;
				body["SSMInputs"] = SaveGenInputs(ssmInput);
				body["TechList"] = SaveTechlist(ssmInput);

				JSONInputDataFactory.WriteFile(JToken.FromObject(new Dictionary<string, object>() { { "Header", "AHSM" }, { "Body", body } }), filePath);

			} catch (Exception) {
				// Nothing to do except return false.
				returnValue = false;
			}

			return returnValue;
		}

		private static Dictionary<string, object> SaveGenInputs(ISSMInputs ssmInputs)
		{
			var retVal = new Dictionary<string, object>();

			retVal["BC_GFactor"] = ssmInputs.BoundaryConditions.GFactor;
			retVal["BC_HeatingBoundaryTemperature"] = ssmInputs.BoundaryConditions.HeatingBoundaryTemperature.AsDegCelsius;
			retVal["BC_CoolingBoundaryTemperature"] = ssmInputs.BoundaryConditions.CoolingBoundaryTemperature.AsDegCelsius;
			retVal["BC_HighVentilation"] = ssmInputs.BoundaryConditions.HighVentilation.ConvertToPerHour().Value;
			retVal["BC_lowVentilation"] = ssmInputs.BoundaryConditions.LowVentilation.ConvertToPerHour().Value;
			retVal["BC_SpecificVentilationPower"] = ssmInputs.BoundaryConditions.SpecificVentilationPower.ConvertToWattHourPerCubicMeter().Value;
			retVal["BC_AuxHeaterEfficiency"] = ssmInputs.BoundaryConditions.AuxHeaterEfficiency;
			retVal["BC_GCVDieselOrHeatingOil"] = ssmInputs.BoundaryConditions.GCVDieselOrHeatingOil.ConvertToKiloWattHourPerKilogramm().Value;
			retVal["BC_MaxTemperatureDeltaForLowFloorBusses"] = ssmInputs.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses.AsDegCelsius;
			retVal["BC_MaxPossibleBenefitFromTechnologyList"] = ssmInputs.BoundaryConditions.MaxPossibleBenefitFromTechnologyList;
			retVal["EC_EnviromentalTemperature"] = ssmInputs.EnvironmentalConditions.DefaultConditions.Temperature.AsDegCelsius;
			retVal["EC_Solar"] = ssmInputs.EnvironmentalConditions.DefaultConditions.Solar.Value();
			retVal["AC_CompressorType"] = ssmInputs.ACSystem.CompressorType.ToString();
			retVal["AC_CompressorCapacitykW"] = ssmInputs.ACSystem.CompressorCapacity.ConvertToKiloWatt().Value;
			retVal["VEN_VentilationOnDuringHeating"] = ssmInputs.Ventilation.VentilationOnDuringHeating;
			retVal["VEN_VentilationWhenBothHeatingAndACInactive"] = ssmInputs.Ventilation.VentilationWhenBothHeatingAndACInactive;
			retVal["VEN_VentilationDuringAC"] = ssmInputs.Ventilation.VentilationDuringAC;
			retVal["VEN_VentilationFlowSettingWhenHeatingAndACInactive"] = ssmInputs.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive;
			retVal["VEN_VentilationDuringHeating"] = ssmInputs.Ventilation.VentilationDuringHeating;
			retVal["VEN_VentilationDuringCooling"] = ssmInputs.Ventilation.VentilationDuringCooling;
			retVal["AH_FuelFiredHeaterkW"] = ssmInputs.AuxHeater.FuelFiredHeaterPower.ConvertToKiloWatt().Value;
			retVal["AH_FuelEnergyToHeatToCoolant"] = ssmInputs.AuxHeater.FuelEnergyToHeatToCoolant;
			retVal["AH_CoolantHeatTransferredToAirCabinHeater"] = ssmInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;

			return retVal;
		}

		private static List<object> SaveTechlist(ISSMInputs ssmInputs)
		{
			var retVal = new List<object>();

			foreach (var line in ssmInputs.Technologies.Items) {
				var tmp = new Dictionary<string, object>();
				tmp["Category"] = line.Category;
				tmp["BenefitName"] = line.BenefitName;
				tmp["LowFloorH"] = line.LowFloorH;
				tmp["LowFloorV"] = line.LowFloorV;
				tmp["LowFloorC"] = line.LowFloorC;
				tmp["SemiLowFloorH"] = line.SemiLowFloorH;
				tmp["SemiLowFloorV"] = line.SemiLowFloorV;
				tmp["SemiLowFloorC"] = line.SemiLowFloorC;
				tmp["RaisedFloorH"] = line.RaisedFloorH;
				tmp["RaisedFloorV"] = line.RaisedFloorV;
				tmp["RaisedFloorC"] = line.RaisedFloorC;
				tmp["OnVehicle"] = line.OnVehicle;
				tmp["ActiveVH"] = line.ActiveVH;
				tmp["ActiveVV"] = line.ActiveVV;
				tmp["ActiveVC"] = line.ActiveVC;
				retVal.Add(tmp);
			}

			return retVal;
		}

		//public bool Load(string filePath)
		//{
		//	var returnValue = true;

		//	try {
		//		var json = JSONInputDataFactory.ReadFile(filePath);
		//		var body = (JObject)json["Body"];

		//		SSMDisabled = body.GetEx<bool>("SSMDisabled");
		//		LoadGenInputs((JObject)body["SSMInputs"]);
		//	} catch (Exception) {

		//		// Nothing to do except return false.

		//		returnValue = false;
		//	}

		//	return returnValue;
		//}

		//private void LoadGenInputs(JObject genInput)
		//{
		//	SSMInputs.BoundaryConditions.GFactor = genInput.GetEx<double>("BC_GFactor");
		//	SSMInputs.BoundaryConditions.PassengerBoundaryTemperature = genInput.GetEx<double>("BC_PassengerBoundaryTemperature").DegCelsiusToKelvin();
		//	SSMInputs.BoundaryConditions.HeatingBoundaryTemperature = genInput.GetEx<double>("BC_HeatingBoundaryTemperature").DegCelsiusToKelvin();
		//	SSMInputs.BoundaryConditions.CoolingBoundaryTemperature = genInput.GetEx<double>("BC_CoolingBoundaryTemperature").DegCelsiusToKelvin();
		//	SSMInputs.BoundaryConditions.HighVentilation = genInput.GetEx<double>("BC_HighVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
		//	SSMInputs.BoundaryConditions.LowVentilation = genInput.GetEx<double>("BC_lowVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
		//	SSMInputs.BoundaryConditions.SpecificVentilationPower = genInput.GetEx<double>("BC_SpecificVentilationPower").SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();
		//	SSMInputs.BoundaryConditions.AuxHeaterEfficiency = genInput.GetEx<double>("BC_AuxHeaterEfficiency");
		//	SSMInputs.BoundaryConditions.GCVDieselOrHeatingOil = genInput.GetEx<double>("BC_GCVDieselOrHeatingOil").SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();
		//	SSMInputs.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses = genInput.GetEx<double>("BC_MaxTemperatureDeltaForLowFloorBusses").SI<Kelvin>();
		//	SSMInputs.BoundaryConditions.MaxPossibleBenefitFromTechnologyList = genInput.GetEx<double>("BC_MaxPossibleBenefitFromTechnologyList");
		//	SSMInputs.EnvironmentalConditions.EnviromentalTemperature = genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin();
		//	SSMInputs.EnvironmentalConditions.Solar = genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>();
		//	SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchFile = genInput.GetEx<string>("EC_EnviromentalConditions_BatchFile");
		//	SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchEnabled = genInput.GetEx<bool>("EC_EnviromentalConditions_BatchEnabled");
		//	SSMInputs.ACSystem.CompressorType = genInput.GetEx<string>("AC_CompressorType");
		//	SSMInputs.ACSystem.CompressorCapacity = genInput.GetEx<double>("AC_CompressorCapacitykW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		//	SSMInputs.Ventilation.VentilationOnDuringHeating = genInput.GetEx<bool>("VEN_VentilationOnDuringHeating");
		//	SSMInputs.Ventilation.VentilationWhenBothHeatingAndACInactive = genInput.GetEx<bool>("VEN_VentilationWhenBothHeatingAndACInactive");
		//	SSMInputs.Ventilation.VentilationDuringAC = genInput.GetEx<bool>("VEN_VentilationDuringAC");
		//	SSMInputs.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive = genInput.GetEx<string>("VEN_VentilationFlowSettingWhenHeatingAndACInactive");
		//	SSMInputs.Ventilation.VentilationDuringHeating = genInput.GetEx<string>("VEN_VentilationDuringHeating");
		//	SSMInputs.Ventilation.VentilationDuringCooling = genInput.GetEx<string>("VEN_VentilationDuringCooling");
		//	SSMInputs.AuxHeater.EngineWasteHeatkW = genInput.GetEx<double>("AH_EngineWasteHeatkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		//	SSMInputs.AuxHeater.FuelFiredHeaterkW = genInput.GetEx<double>("AH_FuelFiredHeaterkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		//	SSMInputs.AuxHeater.FuelEnergyToHeatToCoolant = genInput.GetEx<double>("AH_FuelEnergyToHeatToCoolant");
		//	SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater = genInput.GetEx<double>("AH_CoolantHeatTransferredToAirCabinHeater");
		//}
	}
}
