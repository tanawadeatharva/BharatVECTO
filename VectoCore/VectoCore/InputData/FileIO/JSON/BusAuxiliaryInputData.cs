using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public static class BusAuxiliaryInputData
	{
		public static IAuxiliaryConfig ReadBusAuxiliaries(string filename, IVehicleData vehicleData)
		{
			var json = JSONInputDataFactory.ReadFile(filename);
			var body = (JObject)json["Body"];

			return LoadValues(body, Path.GetDirectoryName(filename), vehicleData);
		}

		private static AuxiliaryConfig LoadValues(JObject data, string baseDir, IVehicleData vehicleData)
		{
			var ec = LoadElectricalConfig((JObject)data["ElectricalUserInputsConfig"], baseDir);
			var pac = LoadPneumaticsAuxConfig((JObject)data["PneumaticAuxillariesConfig"], baseDir);
			var puc = LoadPneumaticUserConfig((JObject)data["PneumaticUserInputsConfig"], baseDir);
			var env = string.IsNullOrWhiteSpace(data["EnvironmentalConditions"]?.ToString())
				? DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions
				: EnvironmentalContidionsMapReader.ReadFile(data["EnvironmentalConditions"].ToString());
			var techList = string.IsNullOrWhiteSpace(data["SSMTechologies"]?.ToString())
				? DeclarationData.BusAuxiliaries.SSMTechnologyList
				: SSMTechnologiesReader.ReadFromFile(data["SSMTechologies"].ToString());
			var actuations = new Actuations() {
				Braking = data["Actuations"]?.GetEx<int>("Brakes") ?? 0,
				ParkBrakeAndDoors = data["Actuations"]?.GetEx<int>("Park brake + 2 doors") ?? 0,
				Kneeling = data["Actuations"]?.GetEx<int>("Kneeling") ?? 0,
				CycleTime = (data["Actiations"]?.GetEx<int>("CycleTime") ?? 3600).SI<Second>()
			};
				//ActuationsMapReader.Read(Path.Combine(baseDir, data.GetEx<string>("ActuationsMap")));
			var ssm = string.IsNullOrWhiteSpace(data["SSMFilePath"]?.ToString()) ?
				new SSMInputs(vehicleData, "", FuelData.Diesel) {
					EnvironmentalConditionsMap =  env,
					Technologies = techList
				}
				: SSMInputData.ReadFile(
				Path.Combine(baseDir, data["SSMFilePath"].ToString()), vehicleData, env,
				techList);
			return new AuxiliaryConfig( ) {
				ElectricalUserInputsConfig = ec,
				PneumaticAuxillariesConfig  = pac,
				PneumaticUserInputsConfig  = puc,
				SSMInputs = ssm,
				Actuations = actuations,
				VehicleData = vehicleData
			};
		}

		private static IElectricsUserInputsConfig LoadElectricalConfig(JObject elData, string baseDir)
		{
			var electricalUserInputsConfig = new ElectricsUserInputsConfig();

			// AlternatorGearEfficiency
			electricalUserInputsConfig.AlternatorGearEfficiency = elData.GetEx<double>("AlternatorGearEfficiency");

			// AlternatorMap
			electricalUserInputsConfig.AlternatorMap = AlternatorReader.ReadMap(Path.Combine(baseDir , elData.GetEx("AlternatorMap").Value<string>()));

			// DoorActuationTimeSecond
			electricalUserInputsConfig.DoorActuationTimeSecond = elData.GetEx<double>("DoorActuationTimeSecond").SI<Second>();


			electricalUserInputsConfig.AverageCurrentDemandInclBaseLoad = elData["ElectricalConsumers"]
				.GetEx<double>("AverageCurrentDemandInclBaseLoad").SI<Ampere>();
			electricalUserInputsConfig.AverageCurrentDemandWithoutBaseLoad = elData["ElectricalConsumers"]
				.GetEx<double>("AverageCurrentDemandWithoutBaseLoad").SI<Ampere>();

			// PowerNetVoltage
			//electricalUserInputsConfig.PowerNetVoltage = elData.GetEx<double>("PowerNetVoltage").SI<Volt>();

			// ResultCardIdle

			electricalUserInputsConfig.ResultCardIdle = new ResultCard(
				elData["ResultCardIdle"].Select(
					result => new SmartResult(
						result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>())).ToList());

			// ResultCardOverrun
			electricalUserInputsConfig.ResultCardOverrun = new ResultCard(
				elData["ResultCardOverrun"].Select(
					result => new SmartResult(
						result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>())).ToList());

			// ResultCardTraction
			electricalUserInputsConfig.ResultCardTraction = new ResultCard(
				elData["ResultCardTraction"].Select(
					result => new SmartResult(
						result.GetEx<double>("Amps").SI<Ampere>(), result.GetEx<double>("SmartAmps").SI<Ampere>())).ToList());

			// SmartElectrical
			electricalUserInputsConfig.SmartElectrical = elData.GetEx<bool>("SmartElectrical");
			return electricalUserInputsConfig;
		}

		private static IPneumaticsConsumersDemand LoadPneumaticsAuxConfig(JObject paData, string baseDir)
		{
			var pneumaticAuxillariesConfig = new PneumaticsConsumersDemand();
			pneumaticAuxillariesConfig.AdBlueInjection =
				paData.GetEx<double>("AdBlueNIperMinute").SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>();
			pneumaticAuxillariesConfig.AirControlledSuspension =
				paData.GetEx<double>("AirControlledSuspensionNIperMinute").SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>();
			pneumaticAuxillariesConfig.Braking = paData
				.GetEx<double>("BrakingNIperKG").SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>();
			//pneumaticAuxillariesConfig.BrakingWithRetarderNIperKG = paData
				//.GetEx<double>("BrakingWithRetarderNIperKG").SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>();
			pneumaticAuxillariesConfig.BreakingWithKneeling = paData
				.GetEx<double>("BreakingPerKneelingNIperKGinMM").SI(Unit.SI.Liter.Per.Kilo.Gramm.Milli.Meter)
				.Cast<NormLiterPerKilogramMeter>();
			pneumaticAuxillariesConfig.DeadVolBlowOuts =
				paData.GetEx<double>("DeadVolBlowOutsPerLitresperHour").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			pneumaticAuxillariesConfig.DeadVolume = paData.GetEx<double>("DeadVolumeLitres").SI<NormLiter>();
			pneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand =
				paData.GetEx<double>("NonSmartRegenFractionTotalAirDemand");
			pneumaticAuxillariesConfig.DoorOpening = paData.GetEx<double>("PerDoorOpeningNI").SI<NormLiter>();
			pneumaticAuxillariesConfig.StopBrakeActuation = paData
				.GetEx<double>("PerStopBrakeActuationNIperKG").SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>();
			pneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand =
				paData.GetEx<double>("SmartRegenFractionTotalAirDemand");
			pneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction =
				paData.GetEx<double>("OverrunUtilisationForCompressionFraction");
			return pneumaticAuxillariesConfig;
		}

		private static IPneumaticUserInputsConfig LoadPneumaticUserConfig(JObject puData, string baseDir)
		{
			var pneumaticUserInputsConfig = new PneumaticUserInputsConfig();
			//pneumaticUserInputsConfig.ActuationsMap = PneumaticActuationsMapReader.Read(Path.Combine(baseDir, puData.GetEx<string>("ActuationsMap")));
			pneumaticUserInputsConfig.AdBlueDosing = puData.GetEx<string>("AdBlueDosing").ParseEnum<ConsumerTechnology>();
			pneumaticUserInputsConfig.AirSuspensionControl =
				puData.GetEx<string>("AirSuspensionControl").ParseEnum<ConsumerTechnology>();
			pneumaticUserInputsConfig.CompressorGearEfficiency = puData.GetEx<double>("CompressorGearEfficiency");
			pneumaticUserInputsConfig.CompressorGearRatio = puData.GetEx<double>("CompressorGearRatio");
			var file = puData.GetEx<string>("CompressorMap");
			if (!string.IsNullOrWhiteSpace(file)) {
				pneumaticUserInputsConfig.CompressorMap = CompressorMapReader.ReadFile(Path.Combine(baseDir, file));
			}
			pneumaticUserInputsConfig.Doors = puData.GetEx<string>("Doors").ParseEnum<ConsumerTechnology>();
			pneumaticUserInputsConfig.KneelingHeight =
				puData.GetEx<double>("KneelingHeightMillimeters").SI(Unit.SI.Milli.Meter).Cast<Meter>();
			//pneumaticUserInputsConfig.RetarderBrake = puData.GetEx<bool>("RetarderBrake");
			pneumaticUserInputsConfig.SmartAirCompression = puData.GetEx<bool>("SmartAirCompression");
			pneumaticUserInputsConfig.SmartRegeneration = puData.GetEx<bool>("SmartRegeneration");
			return pneumaticUserInputsConfig;
		}
	}
}
