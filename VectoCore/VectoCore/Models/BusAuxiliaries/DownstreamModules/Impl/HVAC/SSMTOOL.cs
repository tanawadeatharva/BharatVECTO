using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omu.ValueInjecter;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used by frmHVACTool
	// Replaces Spreadsheet model which does the same calculation
	// Version of which appears on the form title.
	public class SSMTOOL : ISSMTOOL
	{
		private string FilePath;
		public ISSMGenInputs GenInputs { get; set; }
		public ISSMTechList TechList { get; set; }
		public ISSMCalculate Calculate { get; set; }
		public bool SSMDisabled { get; set; }
		public IHVACConstants HVACConstants { get; set; }

		// Repeat Warning Flags
		private bool CompressorCapacityInsufficientWarned;
		//private bool FuelFiredHeaterInsufficientWarned;

		// Base Values
		public Watt ElectricalWBase
		{
			get {
				return SSMDisabled ? 0.SI<Watt>() : Calculate.ElectricalWBase; // .SI(Of Watt)()
			}
		}

		public Watt MechanicalWBase
		{
			get {
				return SSMDisabled ? 0.SI<Watt>() : Calculate.MechanicalWBase; // .SI(Of Watt)()
			}
		}

		public KilogramPerSecond FuelPerHBase
		{
			get {
				return SSMDisabled ? 0.SI<KilogramPerSecond>() : Calculate.FuelPerHBase; // .SI(Of LiterPerHour)()
			}
		}

		// Adjusted Values
		public Watt ElectricalWAdjusted
		{
			get {
				return SSMDisabled ? 0.SI<Watt>() : Calculate.ElectricalWAdjusted; // .SI(Of Watt)()
			}
		}

		public Watt MechanicalWBaseAdjusted
		{
			get {
				var mechAdjusted = SSMDisabled ? 0.SI<Watt>() : Calculate.MechanicalWBaseAdjusted;

				if (CompressorCapacityInsufficientWarned == false && (mechAdjusted) / (1000 * GenInputs.AC_COP) > GenInputs.AC_CompressorCapacitykW) {
					OnMessage(this, "HVAC SSM :AC-Compressor Capacity unable to service cooling, run continues as if capacity was sufficient.", AdvancedAuxiliaryMessageType.Warning);
					CompressorCapacityInsufficientWarned = true;
				}


				return mechAdjusted; // .SI(Of Watt)()
			}
		}

		public KilogramPerSecond FuelPerHBaseAdjusted
		{
			get {
				return SSMDisabled ? 0.SI<KilogramPerSecond>() : Calculate.FuelPerHBaseAdjusted; // .SI(Of LiterPerHour)()
			}
		}

		// Constructors
		public SSMTOOL(string filePath, HVACConstants hvacConstants, bool isDisabled = false, bool useTestValues = false)
		{
			FilePath = filePath;
			SSMDisabled = isDisabled;
			HVACConstants = hvacConstants;

			GenInputs = new SSMGenInputs(Path.GetDirectoryName(filePath));
			TechList = new SSMTechList(GenInputs.BP_BusFloorType);
			TechList.TechLines = HVACTechBenefitsReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".Buses." +
																								"HVAC_TechList.csv"));

			Calculate = new SSMCalculate(this);
		}

		// Clone values from another object of same type
		//public void Clone(ISSMTOOL from)
		//{
		//	var feedback = string.Empty;

		//	GenInputs.InjectFrom(((SSMTOOL)from).GenInputs);

		//	TechList.Clear();

		//	foreach (var line in from.TechList.TechLines) {
		//		var newLine = new TechListBenefitLine(this.GenInputs);
		//		// newLine.InjectFrom()
		//		newLine.InjectFrom(line);
		//		TechList.Add(newLine, ref feedback);
		//	}
		//}

		// Persistance Functions
		public bool Save(string filePath)
		{
			var returnValue = true;
			//var settings = new JsonSerializerSettings();
			//settings.TypeNameHandling = TypeNameHandling.Objects;

			// JSON METHOD
			try {
				//var output = JsonConvert.SerializeObject(this, Formatting.Indented, settings);

				//File.WriteAllText(FilePath, output);

				var body = new Dictionary<string, object>();
				body["SSMDisabled"] = SSMDisabled;
				body["GenInputs"] = SaveGenInputs();
				body["TechList"] = SaveTechlist();


				JSONInputDataFactory.WriteFile(JToken.FromObject(new Dictionary<string, object>() { {"Header", "AHSM"}, { "Body",body } }), filePath);

			} catch (Exception ) {

				// Nothing to do except return false.
				returnValue = false;
			}

			return returnValue;
		}

		private Dictionary<string, object> SaveGenInputs()
		{
			var retVal = new Dictionary<string, object>();

			retVal["BC_GFactor"] = GenInputs.BC_GFactor;
			retVal["BC_PassengerBoundaryTemperature"] = GenInputs.BC_PassengerBoundaryTemperature.AsDegCelsius;
			retVal["BC_HeatingBoundaryTemperature"] = GenInputs.BC_HeatingBoundaryTemperature.AsDegCelsius;
			retVal["BC_CoolingBoundaryTemperature"] = GenInputs.BC_CoolingBoundaryTemperature.AsDegCelsius;
			retVal["BC_HighVentilation"] = GenInputs.BC_HighVentilation.ConvertToPerHour().Value;
			retVal["BC_lowVentilation"] = GenInputs.BC_lowVentilation.ConvertToPerHour().Value;
			retVal["BC_SpecificVentilationPower"] = GenInputs.BC_SpecificVentilationPower.ConvertToWattHourPerCubicMeter().Value;
			retVal["BC_AuxHeaterEfficiency"] = GenInputs.BC_AuxHeaterEfficiency;
			retVal["BC_GCVDieselOrHeatingOil"] = GenInputs.BC_GCVDieselOrHeatingOil.ConvertToKiloWattHourPerKilogramm().Value;
			retVal["BC_MaxTemperatureDeltaForLowFloorBusses"] = GenInputs.BC_MaxTemperatureDeltaForLowFloorBusses.AsDegCelsius;
			retVal["BC_MaxPossibleBenefitFromTechnologyList"] = GenInputs.BC_MaxPossibleBenefitFromTechnologyList;
			retVal["EC_EnviromentalTemperature"] = GenInputs.EC_EnviromentalTemperature.AsDegCelsius;
			retVal["EC_Solar"] = GenInputs.EC_Solar.Value();
			retVal["EC_EnviromentalConditions_BatchFile"] = GenInputs.EC_EnviromentalConditions_BatchFile;
			retVal["EC_EnviromentalConditions_BatchEnabled"] = GenInputs.EC_EnviromentalConditions_BatchEnabled;
			retVal["AC_CompressorType"] = GenInputs.AC_CompressorType;
			retVal["AC_CompressorCapacitykW"] = GenInputs.AC_CompressorCapacitykW.ConvertToKiloWatt().Value;
			retVal["VEN_VentilationOnDuringHeating"] = GenInputs.VEN_VentilationOnDuringHeating;
			retVal["VEN_VentilationWhenBothHeatingAndACInactive"] = GenInputs.VEN_VentilationWhenBothHeatingAndACInactive;
			retVal["VEN_VentilationDuringAC"] = GenInputs.VEN_VentilationDuringAC;
			retVal["VEN_VentilationFlowSettingWhenHeatingAndACInactive"] = GenInputs.VEN_VentilationFlowSettingWhenHeatingAndACInactive;
			retVal["VEN_VentilationDuringHeating"] = GenInputs.VEN_VentilationDuringHeating;
			retVal["VEN_VentilationDuringCooling"] = GenInputs.VEN_VentilationDuringCooling;
			retVal["AH_EngineWasteHeatkW"] = GenInputs.AH_EngineWasteHeatkW.ConvertToKiloWatt().Value;
			retVal["AH_FuelFiredHeaterkW"] = GenInputs.AH_FuelFiredHeaterkW.ConvertToKiloWatt().Value;
			retVal["AH_FuelEnergyToHeatToCoolant"] = GenInputs.AH_FuelEnergyToHeatToCoolant;
			retVal["AH_CoolantHeatTransferredToAirCabinHeater"] = GenInputs.AH_CoolantHeatTransferredToAirCabinHeater;

			return retVal;
		}

		private List<object> SaveTechlist()
		{
			var retVal = new List<object>();

			foreach (var line in TechList.TechLines) {
				var tmp = new Dictionary<string, object>();
				//tmp["Units"] = line.Units;
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
				//tmp["LineType"] = line.LineType;
				retVal.Add(tmp);
			}

			return retVal;
		}

		public bool Load(string filePath)
		{
			var returnValue = true;
			
			try {
				var json = JSONInputDataFactory.ReadFile(filePath);
				var body = (JObject)json["Body"];

				SSMDisabled = body.GetEx<bool>("SSMDisabled");
				LoadGenInputs((JObject)body["GenInputs"]);
			} catch (Exception ) {

				// Nothing to do except return false.

				returnValue = false;
			}

			return returnValue;
		}

		private void LoadGenInputs(JObject genInput)
		{
			GenInputs.BC_GFactor = genInput.GetEx<double>("BC_GFactor");
			GenInputs.BC_PassengerBoundaryTemperature = genInput.GetEx<double>("BC_PassengerBoundaryTemperature").DegCelsiusToKelvin();
			GenInputs.BC_HeatingBoundaryTemperature = genInput.GetEx<double>("BC_HeatingBoundaryTemperature").DegCelsiusToKelvin();
			GenInputs.BC_CoolingBoundaryTemperature = genInput.GetEx<double>("BC_CoolingBoundaryTemperature").DegCelsiusToKelvin();
			GenInputs.BC_HighVentilation = genInput.GetEx<double>("BC_HighVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			GenInputs.BC_lowVentilation = genInput.GetEx<double>("BC_lowVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			GenInputs.BC_SpecificVentilationPower = genInput.GetEx<double>("BC_SpecificVentilationPower").SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();
			GenInputs.BC_AuxHeaterEfficiency = genInput.GetEx<double>("BC_AuxHeaterEfficiency");
			GenInputs.BC_GCVDieselOrHeatingOil = genInput.GetEx<double>("BC_GCVDieselOrHeatingOil").SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();
			GenInputs.BC_MaxTemperatureDeltaForLowFloorBusses = genInput.GetEx<double>("BC_MaxTemperatureDeltaForLowFloorBusses").SI<Kelvin>();
			GenInputs.BC_MaxPossibleBenefitFromTechnologyList = genInput.GetEx<double>("BC_MaxPossibleBenefitFromTechnologyList");
			GenInputs.EC_EnviromentalTemperature = genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin();
			GenInputs.EC_Solar = genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>();
			GenInputs.EC_EnviromentalConditions_BatchFile = genInput.GetEx<string>("EC_EnviromentalConditions_BatchFile");
			GenInputs.EC_EnviromentalConditions_BatchEnabled = genInput.GetEx<bool>("EC_EnviromentalConditions_BatchEnabled");
			GenInputs.AC_CompressorType = genInput.GetEx<string>("AC_CompressorType");
			GenInputs.AC_CompressorCapacitykW = genInput.GetEx<double>("AC_CompressorCapacitykW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			GenInputs.VEN_VentilationOnDuringHeating = genInput.GetEx<bool>("VEN_VentilationOnDuringHeating");
			GenInputs.VEN_VentilationWhenBothHeatingAndACInactive = genInput.GetEx<bool>("VEN_VentilationWhenBothHeatingAndACInactive");
			GenInputs.VEN_VentilationDuringAC = genInput.GetEx<bool>("VEN_VentilationDuringAC");
			GenInputs.VEN_VentilationFlowSettingWhenHeatingAndACInactive = genInput.GetEx<string>("VEN_VentilationFlowSettingWhenHeatingAndACInactive");
			GenInputs.VEN_VentilationDuringHeating = genInput.GetEx<string>("VEN_VentilationDuringHeating");
			GenInputs.VEN_VentilationDuringCooling = genInput.GetEx<string>("VEN_VentilationDuringCooling");
			GenInputs.AH_EngineWasteHeatkW = genInput.GetEx<double>("AH_EngineWasteHeatkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			GenInputs.AH_FuelFiredHeaterkW = genInput.GetEx<double>("AH_FuelFiredHeaterkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			GenInputs.AH_FuelEnergyToHeatToCoolant = genInput.GetEx<double>("AH_FuelEnergyToHeatToCoolant");
			GenInputs.AH_CoolantHeatTransferredToAirCabinHeater = genInput.GetEx<double>("AH_CoolantHeatTransferredToAirCabinHeater");
		}

		// Comparison
		public bool IsEqualTo(ISSMTOOL source)
		{

			// In this methods we only want to compare the non Static , non readonly public properties of 
			// The class's General, User Inputs and  Tech Benefit members.

			return compareGenUserInputs(source) && compareTechListBenefitLines(source);
		}

		private bool compareGenUserInputs(ISSMTOOL source)
		{
			var src = (SSMTOOL)source;

			var returnValue = true;

			var properties = GenInputs.GetType().GetProperties();

			foreach (var prop in properties) {

				// If Not prop.GetAccessors.IsReadOnly Then
				if (prop.CanWrite) {
					if (!prop.GetValue(GenInputs, null/* TODO Change to default(_) if this is not a reference type */).Equals(prop.GetValue(src.GenInputs, null/* TODO Change to default(_) if this is not a reference type */)))
						returnValue = false;
				}
			}

			return returnValue;
		}

		private bool compareTechListBenefitLines(ISSMTOOL source)
		{
			var src = (SSMTOOL)source;

			// Equal numbers of lines check
			if (TechList.TechLines.Count != src.TechList.TechLines.Count)
				return false;

			foreach (var tl in TechList.TechLines.OrderBy(o => o.Category).ThenBy(n => n.BenefitName)) {

				// First Check line exists in other
				if (src.TechList.TechLines.Where(w => w.BenefitName == tl.BenefitName && w.Category == tl.Category).Count() != 1)
					return false;
				else {

					// check are equal

					var testLine = src.TechList.TechLines.First(w => w.BenefitName == tl.BenefitName && w.Category == tl.Category);

					if (!testLine.IsEqualTo(tl))
						return false;
				}
			}

			// All Looks OK
			return true;
		}

		// Overrides
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine(Calculate.ToString());


			return sb.ToString();
		}

		// Dynamicly Get Fuel having re-adjusted Engine Heat Waste, this was originally supposed to be Solid State. Late adjustment request 24/3/2015
		public KilogramPerSecond FuelPerHBaseAsjusted(Watt AverageUseableEngineWasteHeatKW)
		{
			if (SSMDisabled)
				return 0.SI<KilogramPerSecond>();

			// Set Engine Waste Heat
			GenInputs.AH_EngineWasteHeatkW = AverageUseableEngineWasteHeatKW;
			var fba = FuelPerHBaseAdjusted;

			// Dim FuelFiredWarning As Boolean = fba * GenInputs.BC_AuxHeaterEfficiency * HVACConstants.FuelDensity * GenInputs.BC_GCVDieselOrHeatingOil * 1000 > (AverageUseableEngineWasteHeatKW + GenInputs.AH_FuelFiredHeaterkW)

			// If Not FuelFiredHeaterInsufficientWarned AndAlso FuelFiredWarning Then

			// FuelFiredHeaterInsufficientWarned = True

			// OnMessage(Me, " HVAC SSM : Fuel fired heater insufficient for heating requirements, run continues assuming it was sufficient.", AdvancedAuxiliaryMessageType.Warning)

			// End If

			return fba;
		}

		// Events
		public event MessageEventHandler Message;

		// Raise Message Event.
		private void OnMessage(object sender, string message, AdvancedAuxiliaryMessageType messageType)
		{
			if (message != null) {
				object ssmtool = this;
				Message?.Invoke(ref ssmtool, message: message, messageType: messageType);
			}
		}
	}
}
