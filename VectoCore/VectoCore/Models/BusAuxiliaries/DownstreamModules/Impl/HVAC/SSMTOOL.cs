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
		public ISSMInputs SSMInputs { get; set; }

		public ISSMBoundaryConditions BoundaryConditions { get; set; }

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

				if (CompressorCapacityInsufficientWarned == false && (mechAdjusted) / (1000 * SSMInputs.ACSystem.COP) > SSMInputs.ACSystem.CompressorCapacity) {
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

			SSMInputs = new SSMInputs(Path.GetDirectoryName(filePath));
			TechList = new SSMTechList(SSMInputs.BusParameters.BusFloorType);
			TechList.TechLines = HVACTechBenefitsReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".Buses." +
																								"HVAC_TechList.csv"));

			Calculate = new SSMCalculate(this);
		}

		// Clone values from another object of same type
		//public void Clone(ISSMTOOL from)
		//{
		//	var feedback = string.Empty;

		//	SSMInputs.InjectFrom(((SSMTOOL)from).SSMInputs);

		//	TechList.Clear();

		//	foreach (var line in from.TechList.TechLines) {
		//		var newLine = new TechListBenefitLine(this.SSMInputs);
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
				body["SSMInputs"] = SaveGenInputs();
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

			retVal["BC_GFactor"] = SSMInputs.BoundaryConditions.GFactor;
			retVal["BC_PassengerBoundaryTemperature"] = SSMInputs.BoundaryConditions.PassengerBoundaryTemperature.AsDegCelsius;
			retVal["BC_HeatingBoundaryTemperature"] = SSMInputs.BoundaryConditions.HeatingBoundaryTemperature.AsDegCelsius;
			retVal["BC_CoolingBoundaryTemperature"] = SSMInputs.BoundaryConditions.CoolingBoundaryTemperature.AsDegCelsius;
			retVal["BC_HighVentilation"] = SSMInputs.BoundaryConditions.HighVentilation.ConvertToPerHour().Value;
			retVal["BC_lowVentilation"] = SSMInputs.BoundaryConditions.LowVentilation.ConvertToPerHour().Value;
			retVal["BC_SpecificVentilationPower"] = SSMInputs.BoundaryConditions.SpecificVentilationPower.ConvertToWattHourPerCubicMeter().Value;
			retVal["BC_AuxHeaterEfficiency"] = SSMInputs.BoundaryConditions.AuxHeaterEfficiency;
			retVal["BC_GCVDieselOrHeatingOil"] = SSMInputs.BoundaryConditions.GCVDieselOrHeatingOil.ConvertToKiloWattHourPerKilogramm().Value;
			retVal["BC_MaxTemperatureDeltaForLowFloorBusses"] = SSMInputs.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses.AsDegCelsius;
			retVal["BC_MaxPossibleBenefitFromTechnologyList"] = SSMInputs.BoundaryConditions.MaxPossibleBenefitFromTechnologyList;
			retVal["EC_EnviromentalTemperature"] = SSMInputs.EnvironmentalConditions.EnviromentalTemperature.AsDegCelsius;
			retVal["EC_Solar"] = SSMInputs.EnvironmentalConditions.Solar.Value();
			retVal["EC_EnviromentalConditions_BatchFile"] = SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchFile;
			retVal["EC_EnviromentalConditions_BatchEnabled"] = SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchEnabled;
			retVal["AC_CompressorType"] = SSMInputs.ACSystem.CompressorType;
			retVal["AC_CompressorCapacitykW"] = SSMInputs.ACSystem.CompressorCapacity.ConvertToKiloWatt().Value;
			retVal["VEN_VentilationOnDuringHeating"] = SSMInputs.Ventilation.VentilationOnDuringHeating;
			retVal["VEN_VentilationWhenBothHeatingAndACInactive"] = SSMInputs.Ventilation.VentilationWhenBothHeatingAndACInactive;
			retVal["VEN_VentilationDuringAC"] = SSMInputs.Ventilation.VentilationDuringAC;
			retVal["VEN_VentilationFlowSettingWhenHeatingAndACInactive"] = SSMInputs.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive;
			retVal["VEN_VentilationDuringHeating"] = SSMInputs.Ventilation.VentilationDuringHeating;
			retVal["VEN_VentilationDuringCooling"] = SSMInputs.Ventilation.VentilationDuringCooling;
			retVal["AH_EngineWasteHeatkW"] = SSMInputs.AuxHeater.EngineWasteHeatkW.ConvertToKiloWatt().Value;
			retVal["AH_FuelFiredHeaterkW"] = SSMInputs.AuxHeater.FuelFiredHeaterkW.ConvertToKiloWatt().Value;
			retVal["AH_FuelEnergyToHeatToCoolant"] = SSMInputs.AuxHeater.FuelEnergyToHeatToCoolant;
			retVal["AH_CoolantHeatTransferredToAirCabinHeater"] = SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;

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
				LoadGenInputs((JObject)body["SSMInputs"]);
			} catch (Exception ) {

				// Nothing to do except return false.

				returnValue = false;
			}

			return returnValue;
		}

		private void LoadGenInputs(JObject genInput)
		{
			SSMInputs.BoundaryConditions.GFactor = genInput.GetEx<double>("BC_GFactor");
			SSMInputs.BoundaryConditions.PassengerBoundaryTemperature = genInput.GetEx<double>("BC_PassengerBoundaryTemperature").DegCelsiusToKelvin();
			SSMInputs.BoundaryConditions.HeatingBoundaryTemperature = genInput.GetEx<double>("BC_HeatingBoundaryTemperature").DegCelsiusToKelvin();
			SSMInputs.BoundaryConditions.CoolingBoundaryTemperature = genInput.GetEx<double>("BC_CoolingBoundaryTemperature").DegCelsiusToKelvin();
			SSMInputs.BoundaryConditions.HighVentilation = genInput.GetEx<double>("BC_HighVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			SSMInputs.BoundaryConditions.LowVentilation = genInput.GetEx<double>("BC_lowVentilation").SI(Unit.SI.Per.Hour).Cast<PerSecond>();
			SSMInputs.BoundaryConditions.SpecificVentilationPower = genInput.GetEx<double>("BC_SpecificVentilationPower").SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>();
			SSMInputs.BoundaryConditions.AuxHeaterEfficiency = genInput.GetEx<double>("BC_AuxHeaterEfficiency");
			SSMInputs.BoundaryConditions.GCVDieselOrHeatingOil = genInput.GetEx<double>("BC_GCVDieselOrHeatingOil").SI(Unit.SI.Kilo.Watt.Hour.Per.Kilo.Gramm).Cast<JoulePerKilogramm>();
			SSMInputs.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses = genInput.GetEx<double>("BC_MaxTemperatureDeltaForLowFloorBusses").SI<Kelvin>();
			SSMInputs.BoundaryConditions.MaxPossibleBenefitFromTechnologyList = genInput.GetEx<double>("BC_MaxPossibleBenefitFromTechnologyList");
			SSMInputs.EnvironmentalConditions.EnviromentalTemperature = genInput.GetEx<double>("EC_EnviromentalTemperature").DegCelsiusToKelvin();
			SSMInputs.EnvironmentalConditions.Solar = genInput.GetEx<double>("EC_Solar").SI<WattPerSquareMeter>();
			SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchFile = genInput.GetEx<string>("EC_EnviromentalConditions_BatchFile");
			SSMInputs.EnvironmentalConditions.EnviromentalConditions_BatchEnabled = genInput.GetEx<bool>("EC_EnviromentalConditions_BatchEnabled");
			SSMInputs.ACSystem.CompressorType = genInput.GetEx<string>("AC_CompressorType");
			SSMInputs.ACSystem.CompressorCapacity = genInput.GetEx<double>("AC_CompressorCapacitykW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			SSMInputs.Ventilation.VentilationOnDuringHeating = genInput.GetEx<bool>("VEN_VentilationOnDuringHeating");
			SSMInputs.Ventilation.VentilationWhenBothHeatingAndACInactive = genInput.GetEx<bool>("VEN_VentilationWhenBothHeatingAndACInactive");
			SSMInputs.Ventilation.VentilationDuringAC = genInput.GetEx<bool>("VEN_VentilationDuringAC");
			SSMInputs.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive = genInput.GetEx<string>("VEN_VentilationFlowSettingWhenHeatingAndACInactive");
			SSMInputs.Ventilation.VentilationDuringHeating = genInput.GetEx<string>("VEN_VentilationDuringHeating");
			SSMInputs.Ventilation.VentilationDuringCooling = genInput.GetEx<string>("VEN_VentilationDuringCooling");
			SSMInputs.AuxHeater.EngineWasteHeatkW = genInput.GetEx<double>("AH_EngineWasteHeatkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			SSMInputs.AuxHeater.FuelFiredHeaterkW = genInput.GetEx<double>("AH_FuelFiredHeaterkW").SI(Unit.SI.Kilo.Watt).Cast<Watt>();
			SSMInputs.AuxHeater.FuelEnergyToHeatToCoolant = genInput.GetEx<double>("AH_FuelEnergyToHeatToCoolant");
			SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater = genInput.GetEx<double>("AH_CoolantHeatTransferredToAirCabinHeater");
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

			var properties = SSMInputs.GetType().GetProperties();

			foreach (var prop in properties) {

				// If Not prop.GetAccessors.IsReadOnly Then
				if (prop.CanWrite) {
					if (!prop.GetValue(SSMInputs, null/* TODO Change to default(_) if this is not a reference type */).Equals(prop.GetValue(src.SSMInputs, null/* TODO Change to default(_) if this is not a reference type */)))
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
			SSMInputs.AuxHeater.EngineWasteHeatkW = AverageUseableEngineWasteHeatKW;
			var fba = FuelPerHBaseAdjusted;

			// Dim FuelFiredWarning As Boolean = fba * SSMInputs.BC_AuxHeaterEfficiency * HVACConstants.FuelDensity * SSMInputs.BC_GCVDieselOrHeatingOil * 1000 > (AverageUseableEngineWasteHeatKW + SSMInputs.AH_FuelFiredHeaterkW)

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
