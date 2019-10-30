using System.Linq;
using System.Text;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used by frmHVACTool
	// Replaces Spreadsheet model which does the same calculation
	// Version of which appears on the form title.
	public class SSMTOOL : ISSMTOOL
	{
		

		// Constructors
		public SSMTOOL(ISSMInputs ssmInput)
		{

			SSMInputs = ssmInput; //new SSMInputs(Path.GetDirectoryName(filePath));
			TechList = new SSMTechList(SSMInputs.BusParameters.BusFloorType);
			TechList.TechLines = ssmInput.Technologies.Items;

			Calculate = new SSMCalculate(this);
			EngineWasteHeatkW = 0.SI<Watt>();
		}

		public ISSMInputs SSMInputs { get;  }

		public ISSMBoundaryConditions BoundaryConditions { get; set; }

		public ISSMTechList TechList { get; set; }
		public ISSMCalculate Calculate { get; set; }
		public bool SSMDisabled { get { return SSMInputs.SSMDisabled; } }
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

		public Watt EngineWasteHeatkW { get; protected set; }

		

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

			var properties = SSMInputs.GetType().GetProperties();

			foreach (var prop in properties) {

				// If Not prop.GetAccessors.IsReadOnly Then
				if (prop.CanWrite) {
					var v1 = prop.GetValue(SSMInputs, null);
					var v2 = prop.GetValue(src.SSMInputs, null);
					if (v1 == null ^ v2 == null) {
						return false;
					}
					if (v1 != null && !v1.Equals(v2)) {
						return false;
					}
				}
			}

			return true;
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
			if (SSMDisabled) {
				return 0.SI<KilogramPerSecond>();
			}

			// Set Engine Waste Heat
			//SSMInputs.AuxHeater.EngineWasteHeatkW = AverageUseableEngineWasteHeatKW;
			EngineWasteHeatkW = AverageUseableEngineWasteHeatKW;

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
