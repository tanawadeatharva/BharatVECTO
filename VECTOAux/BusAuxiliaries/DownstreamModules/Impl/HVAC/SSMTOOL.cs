using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.BusAuxiliaries.Util;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used by frmHVACTool
	// Replaces Spreadsheet model which does the same calculation
	// Version of which appears on the form title.
	public class SSMTOOL : ISSMTOOL
	{
		private string filePath;
		public ISSMGenInputs GenInputs { get; set; }
		public ISSMTechList TechList { get; set; }
		public ISSMCalculate Calculate { get; set; }
		public bool SSMDisabled { get; set; }
		public IHVACConstants HVACConstants { get; set; }

		// Repeat Warning Flags
		private bool CompressorCapacityInsufficientWarned;
		private bool FuelFiredHeaterInsufficientWarned;

		// Base Values
		public double ElectricalWBase
		{
			get {
				return SSMDisabled ? 0 : Calculate.ElectricalWBase; // .SI(Of Watt)()
			}
		}

		public double MechanicalWBase
		{
			get {
				return SSMDisabled ? 0 : Calculate.MechanicalWBase; // .SI(Of Watt)()
			}
		}

		public double FuelPerHBase
		{
			get {
				return SSMDisabled ? 0 : Calculate.FuelPerHBase; // .SI(Of LiterPerHour)()
			}
		}

		// Adjusted Values
		public double ElectricalWAdjusted
		{
			get {
				return SSMDisabled ? 0 : Calculate.ElectricalWAdjusted; // .SI(Of Watt)()
			}
		}

		public double MechanicalWBaseAdjusted
		{
			get {
				var mechAdjusted = SSMDisabled ? 0 : Calculate.MechanicalWBaseAdjusted;

				if (CompressorCapacityInsufficientWarned == false && (mechAdjusted) / (1000 * GenInputs.AC_COP) > GenInputs.AC_CompressorCapacitykW) {
					OnMessage(this, "HVAC SSM :AC-Compressor Capacity unable to service cooling, run continues as if capacity was sufficient.", AdvancedAuxiliaryMessageType.Warning);
					CompressorCapacityInsufficientWarned = true;
				}


				return mechAdjusted; // .SI(Of Watt)()
			}
		}

		public double FuelPerHBaseAdjusted
		{
			get {
				return SSMDisabled ? 0 : Calculate.FuelPerHBaseAdjusted; // .SI(Of LiterPerHour)()
			}
		}

		// Constructors
		public SSMTOOL(string filePath, HVACConstants hvacConstants, bool isDisabled = false, bool useTestValues = false)
		{
			this.filePath = filePath;
			this.SSMDisabled = isDisabled;
			this.HVACConstants = hvacConstants;

			GenInputs = new SSMGenInputs(useTestValues, FilePathUtils.fPATH(filePath));
			TechList = new SSMTechList(filePath, GenInputs, useTestValues);

			Calculate = new SSMCalculate(this);
		}

		// Clone values from another object of same type
		public void Clone(ISSMTOOL from)
		{
			var feedback = string.Empty;

			GenInputs.InjectFrom((SSMTOOL)from.GenInputs);

			TechList.Clear();

			foreach (var line in from.TechList.TechLines) {
				var newLine = new TechListBenefitLine(this.GenInputs);
				// newLine.InjectFrom()
				newLine.InjectFrom(line);
				TechList.Add(newLine, ref feedback);
			}
		}

		// Persistance Functions
		public bool Save(string filePath)
		{
			var returnValue = true;
			var settings = new JsonSerializerSettings();
			settings.TypeNameHandling = TypeNameHandling.Objects;

			// JSON METHOD
			try {
				var output = JsonConvert.SerializeObject(this, Formatting.Indented, settings);

				File.WriteAllText(filePath, output);
			} catch (Exception ex) {

				// Nothing to do except return false.
				returnValue = false;
			}

			return returnValue;
		}

		public bool Load(string filePath)
		{
			var returnValue = true;
			var settings = new JsonSerializerSettings();
			SSMTOOL tmpAux; // = New SSMTOOL(filePath, HVACConstants)

			settings.TypeNameHandling = TypeNameHandling.Objects;

			// JSON METHOD
			try {
				var output = File.ReadAllText(filePath);


				tmpAux = JsonConvert.DeserializeObject<SSMTOOL>(output, settings);

				tmpAux.TechList.SetSSMGeneralInputs(tmpAux.GenInputs);

				foreach (TechListBenefitLine tll in tmpAux.TechList.TechLines)

					tll.inputSheet = tmpAux.GenInputs;


				// This is where we Assume values of loaded( Deserialized ) object.
				Clone(tmpAux);
			} catch (Exception ex) {

				// Nothing to do except return false.

				returnValue = false;
			}

			return returnValue;
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

			var properties = this.GenInputs.GetType().GetProperties();

			foreach (var prop in properties) {

				// If Not prop.GetAccessors.IsReadOnly Then
				if (prop.CanWrite) {
					if (!prop.GetValue(this.GenInputs, null/* TODO Change to default(_) if this is not a reference type */).Equals(prop.GetValue(src.GenInputs, null/* TODO Change to default(_) if this is not a reference type */)))
						returnValue = false;
				}
			}

			return returnValue;
		}

		private bool compareTechListBenefitLines(ISSMTOOL source)
		{
			var src = (SSMTOOL)source;

			// Equal numbers of lines check
			if (this.TechList.TechLines.Count != src.TechList.TechLines.Count)
				return false;

			foreach (var tl in this.TechList.TechLines.OrderBy(o => o.Category).ThenBy(n => n.BenefitName)) {

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
		public double FuelPerHBaseAsjusted(double AverageUseableEngineWasteHeatKW)
		{
			if (SSMDisabled)
				return 0;

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
