using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public static class SSMInputData
	{
		public static ISSMInputs ReadStream(Stream str, IVehicleData vehicleData, IEnvironmentalConditionsMap env)
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(new StreamReader(str)));
			var body = (JObject)json["Body"];

			var retVal = Create(body, vehicleData, null);
			return retVal;
		}

		public static ISSMInputs ReadFile(string fileName, IVehicleData vehicleData, IEnvironmentalConditionsMap env)
		{
			var json = JSONInputDataFactory.ReadFile(fileName);
			var body = (JObject)json["Body"];

			var retVal = Create(body, vehicleData, fileName);

			retVal.EnvironmentalConditionsMap = env;
			//retVal.Technologies = technologies;

			return retVal;
		}

		private static SSMInputs Create(JObject body, IVehicleData vehicleData, string fileName)
		{
			var genInput = ((JObject)body["SSMInputs"]);

			var retVal = new SSMInputs(fileName);
			retVal.SSMDisabled = body.GetEx<bool>("SSMDisabled");
			
			retVal.GFactor = genInput.GetEx<double>("BC_GFactor");
			//retVal.PassengerBoundaryTemperature = genInput.GetEx<double>("BC_PassengerBoundaryTemperature").DegCelsiusToKelvin();
			retVal.HeatingBoundaryTemperature = genInput.GetEx<double>("BC_HeatingBoundaryTemperature").DegCelsiusToKelvin();
			retVal.CoolingBoundaryTemperature = genInput.GetEx<double>("BC_CoolingBoundaryTemperature").DegCelsiusToKelvin();
			retVal.VentilationRate = genInput.GetEx<double>("BC_VentilationRate").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			//retVal.LowVentilation = genInput.GetEx<double>("BC_lowVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			retVal.SpecificVentilationPower = genInput.GetEx<double>("BC_SpecificVentilationPower").SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();
			retVal.AuxHeaterEfficiency = genInput.GetEx<double>("BC_AuxHeaterEfficiency");
			//retVal.GCVDieselOrHeatingOil = genInput.GetEx<double>("BC_GCVDieselOrHeatingOil").SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();
			//retVal.MaxTemperatureDeltaForLowFloorBusses = genInput.GetEx<double>("BC_MaxTemperatureDeltaForLowFloorBusses").SI<Kelvin>();
			retVal.MaxPossibleBenefitFromTechnologyList = genInput.GetEx<double>("BC_MaxPossibleBenefitFromTechnologyList");
			//retVal.EnviromentalTemperature = genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin();
			//retVal.Solar = genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>();
			retVal.DefaultConditions = new EnvironmentalConditionMapEntry(
				genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin(),
				genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>(), 1.0);
			//retVal.EnviromentalConditions_BatchFile = genInput.GetEx<string>("EC_EnviromentalConditions_BatchFile");
			//retVal.BatchMode = genInput.GetEx<bool>("EC_EnviromentalConditions_BatchEnabled");
			retVal.HVACCompressorType = ACCompressorTypeExtensions.ParseEnum(genInput.GetEx<string>("AC_CompressorType"));
			retVal.HVACMaxCoolingPower = genInput.GetEx<double>("AC_CompressorCapacitykW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			retVal.VentilationOnDuringHeating = genInput.GetEx<bool>("VEN_VentilationOnDuringHeating");
			retVal.VentilationWhenBothHeatingAndACInactive = genInput.GetEx<bool>("VEN_VentilationWhenBothHeatingAndACInactive");
			retVal.VentilationDuringAC = genInput.GetEx<bool>("VEN_VentilationDuringAC");
			//retVal.VentilationFlowSettingWhenHeatingAndACInactive = genInput.GetEx<string>("VEN_VentilationFlowSettingWhenHeatingAndACInactive").ParseEnum<VentilationLevel>();
			//retVal.VentilationDuringHeating = genInput.GetEx<string>("VEN_VentilationDuringHeating").ParseEnum<VentilationLevel>();
			//retVal.VentilationDuringCooling = genInput.GetEx<string>("VEN_VentilationDuringCooling").ParseEnum<VentilationLevel>();
			//retVal.EngineWasteHeatkW = genInput.GetEx<double>("AH_EngineWasteHeatkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			retVal.FuelFiredHeaterPower = genInput.GetEx<double>("AH_FuelFiredHeaterkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			retVal.FuelEnergyToHeatToCoolant = genInput.GetEx<double>("AH_FuelEnergyToHeatToCoolant");
			retVal.CoolantHeatTransferredToAirCabinHeater = genInput.GetEx<double>("AH_CoolantHeatTransferredToAirCabinHeater");

			retVal.Technologies = new TechnologyBenefits() {
				// TODO: MQ 2020-01-27 read from file!
				//CValueVariation = 
			};

			return retVal;
		}
	}
}
