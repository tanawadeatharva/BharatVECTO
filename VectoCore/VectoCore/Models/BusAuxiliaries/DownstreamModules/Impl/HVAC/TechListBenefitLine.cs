using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	
	// Used by SSMTOOL Class, refer to original spreadsheet model
	// Or PDF Model Document which articulates the same spreadsheet functionality
	// But within the context of the Vecto interpretation of the same.

	public class TechBenefitLines : ITechlistBenefitLines
	{
		#region Implementation of ITechlistBenefitLines

		public TechBenefitLines(IReadOnlyList<ITechListBenefitLine> items, string source)
		{
			Items = items;
			Source = source;
		}

		public IReadOnlyList<ITechListBenefitLine> Items { get; }

		public string Source { get; }

		#endregion
	}

	public class TechListBenefitLine : ITechListBenefitLine
	{
		//private float _h, _vh, _vv, _vc, _c;
		public FloorType BusFloorType { protected get; set; }

		//public string Units { get; set; }
		public string Category { get; set; }
		public string BenefitName { get; set; }
		public double LowFloorH { get; set; }
		public double LowFloorV { get; set; }
		public double LowFloorC { get; set; }

		public double SemiLowFloorH { get; set; }
		public double SemiLowFloorV { get; set; }
		public double SemiLowFloorC { get; set; }

		public double RaisedFloorH { get; set; }
		public double RaisedFloorV { get; set; }
		public double RaisedFloorC { get; set; }

		public bool OnVehicle { get; set; }
		public bool ActiveVH { get; set; }
		public bool ActiveVV { get; set; }
		public bool ActiveVC { get; set; }

		public double H
		{
			get {
				var returnValue = 0.0;

				// =IF($M49=0,0,IF(AND($M49=1,'INPUT & RESULTS SHEET'!$D$6="low floor"),'TECH LIST INPUT'!D49, IF(AND($M49=1,'INPUT & RESULTS SHEET'!$D$6="semi low floor"),'TECH LIST INPUT'!G49,'TECH LIST INPUT'!J49)))
				if (!OnVehicle) {
					return returnValue;
				}

				switch (BusFloorType) {
					case FloorType.LowFloor: return LowFloorH;
					case FloorType.SemiLowFloor: return SemiLowFloorH;
					case FloorType.HighFloor: return RaisedFloorH;
					default: return returnValue;
				}
			}
		}

		public double VH
		{
			get {
				const double floorValue = 0;

				if (!OnVehicle) {
					return floorValue;
				}

				// Active
				if (!ActiveVH) {
					return 0;
				}

				// Get floor value
				switch (BusFloorType) {
					case FloorType.LowFloor: return LowFloorV;
					case FloorType.SemiLowFloor: return SemiLowFloorV;
					case FloorType.HighFloor: return RaisedFloorV;
					default: return floorValue;
				}
			}
		}

		public double VV
		{
			get {
				var floorValue = 0.0;

				if (!OnVehicle) {
					return floorValue;
				}

				// Active
				if (!ActiveVV) {
					return floorValue;
				}

				// Get floor value
				switch (BusFloorType) {
					case FloorType.LowFloor: return LowFloorV;
					case FloorType.SemiLowFloor: return SemiLowFloorV;
					case FloorType.HighFloor: return RaisedFloorV;
					default: return floorValue;
				}
			}
		}

		public double VC
		{
			get {
				var floorValue = 0.0;

				if (!OnVehicle) {
					return floorValue;
				}

				// Active
				if (!ActiveVC) {
					return floorValue;
				}

				// Get floor value
				switch (BusFloorType) {
					case FloorType.LowFloor: return LowFloorV;
					case FloorType.SemiLowFloor: return SemiLowFloorV;
					case FloorType.HighFloor: return RaisedFloorV;
					default: return floorValue;
				}
			}
		}

		public double C
		{
			get {
				var returnValue = 0.0;

				if (!OnVehicle)
					return returnValue;

				switch (BusFloorType) {
					case FloorType.LowFloor: return LowFloorC;
					case FloorType.SemiLowFloor: return SemiLowFloorC;
					case FloorType.HighFloor: return RaisedFloorC;
					default: return returnValue;
				}
			}
		}

		public TechListBenefitLine() { }

		

		public TechListBenefitLine(
			string category, string benefitName, double lowFloorH, double lowFloorV,
			double lowFloorC, double semiLowFloorH, double semiLowFloorV, double semiLowFloorC, double raisedFloorH,
			double raisedFloorV, double raisedFloorC, bool onVehicle, bool activeVH, bool activeVV,
			bool activeVC
		)
		{
			//BusFloorType = geninputs;
			//Units = units;
			Category = category;
			BenefitName = benefitName;
			LowFloorH = lowFloorH;
			LowFloorV = lowFloorV;
			LowFloorC = lowFloorC;
			SemiLowFloorH = semiLowFloorH;
			SemiLowFloorV = semiLowFloorV;
			SemiLowFloorC = semiLowFloorC;
			RaisedFloorH = raisedFloorH;
			RaisedFloorV = raisedFloorV;
			RaisedFloorC = raisedFloorC;
			OnVehicle = onVehicle;
			//LineType = lineType;
			ActiveVH = activeVH;
			ActiveVV = activeVV;
			ActiveVC = activeVC;
		}

		// Operator Overloads
		public static bool operator ==(TechListBenefitLine op1, TechListBenefitLine op2)
		{
			if ((op1.Category == op2.Category && op1.BenefitName == op2.BenefitName && op1.ActiveVC == op2.ActiveVC &&
				op1.ActiveVH == op2.ActiveVH && op1.ActiveVV == op2.ActiveVV && /*op1.LineType == op2.LineType &&*/
				op1.LowFloorC == op2.LowFloorC && op1.LowFloorV == op2.LowFloorV && op1.LowFloorH == op2.LowFloorH &&
				op1.SemiLowFloorC == op2.SemiLowFloorC && op1.SemiLowFloorH == op2.SemiLowFloorH &&
				op1.SemiLowFloorV == op2.SemiLowFloorV && op1.RaisedFloorC == op2.RaisedFloorC &&
				op1.RaisedFloorH == op2.RaisedFloorH && op1.RaisedFloorV == op2.RaisedFloorV && op1.OnVehicle == op2.OnVehicle 
				/*&& op1.Units == op2.Units*/)) {
				return true;
			}

			return false;
		}

		public static bool operator !=(TechListBenefitLine op1, TechListBenefitLine op2)
		{
			if ((op1.Category != op2.Category || op1.BenefitName != op2.BenefitName || op1.ActiveVC != op2.ActiveVC ||
				op1.ActiveVH != op2.ActiveVH || op1.ActiveVV != op2.ActiveVV || /*op1.LineType != op2.LineType ||*/
				op1.LowFloorC != op2.LowFloorC || op1.LowFloorV != op2.LowFloorV || op1.LowFloorH != op2.LowFloorH ||
				op1.SemiLowFloorC != op2.SemiLowFloorC || op1.SemiLowFloorH != op2.SemiLowFloorH ||
				op1.SemiLowFloorV != op2.SemiLowFloorV || op1.RaisedFloorC != op2.RaisedFloorC ||
				op1.RaisedFloorH != op2.RaisedFloorH || op1.RaisedFloorV != op2.RaisedFloorV || op1.OnVehicle != op2.OnVehicle /*||
				op1.Units != op2.Units*/)) {
				return true;
			}

			return false;
		}

		#region Equality members

		protected bool Equals(TechListBenefitLine other)
		{
			return Equals(BusFloorType, other.BusFloorType) && /*string.Equals(Units, other.Units) &&*/
					string.Equals(Category, other.Category) && string.Equals(BenefitName, other.BenefitName) &&
					LowFloorH.Equals(other.LowFloorH) && LowFloorV.Equals(other.LowFloorV) && LowFloorC.Equals(other.LowFloorC) &&
					SemiLowFloorH.Equals(other.SemiLowFloorH) && SemiLowFloorV.Equals(other.SemiLowFloorV) &&
					SemiLowFloorC.Equals(other.SemiLowFloorC) && RaisedFloorH.Equals(other.RaisedFloorH) &&
					RaisedFloorV.Equals(other.RaisedFloorV) && RaisedFloorC.Equals(other.RaisedFloorC) &&
					OnVehicle == other.OnVehicle && ActiveVH == other.ActiveVH && ActiveVV == other.ActiveVV &&
					ActiveVC == other.ActiveVC /* && LineType == other.LineType*/;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}

			return Equals((TechListBenefitLine)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				var hashCode = BusFloorType.GetHashCode();
				//hashCode = (hashCode * 397) ^ (Units != null ? Units.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BenefitName != null ? BenefitName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ LowFloorH.GetHashCode();
				hashCode = (hashCode * 397) ^ LowFloorV.GetHashCode();
				hashCode = (hashCode * 397) ^ LowFloorC.GetHashCode();
				hashCode = (hashCode * 397) ^ SemiLowFloorH.GetHashCode();
				hashCode = (hashCode * 397) ^ SemiLowFloorV.GetHashCode();
				hashCode = (hashCode * 397) ^ SemiLowFloorC.GetHashCode();
				hashCode = (hashCode * 397) ^ RaisedFloorH.GetHashCode();
				hashCode = (hashCode * 397) ^ RaisedFloorV.GetHashCode();
				hashCode = (hashCode * 397) ^ RaisedFloorC.GetHashCode();
				hashCode = (hashCode * 397) ^ OnVehicle.GetHashCode();
				hashCode = (hashCode * 397) ^ ActiveVH.GetHashCode();
				hashCode = (hashCode * 397) ^ ActiveVV.GetHashCode();
				hashCode = (hashCode * 397) ^ ActiveVC.GetHashCode();
				//hashCode = (hashCode * 397) ^ (int)LineType;
				return hashCode;
			}
		}

		#endregion

		public void CloneFrom(ITechListBenefitLine source)
		{
			//this.Units = source.Units;
			Category = source.Category;
			BenefitName = source.BenefitName;
			LowFloorH = source.LowFloorH;
			LowFloorV = source.LowFloorV;
			LowFloorC = source.LowFloorC;

			SemiLowFloorH = source.SemiLowFloorH;
			SemiLowFloorV = source.SemiLowFloorV;
			SemiLowFloorC = source.SemiLowFloorC;

			RaisedFloorH = source.RaisedFloorH;
			RaisedFloorV = source.RaisedFloorV;
			RaisedFloorC = source.RaisedFloorC;

			OnVehicle = source.OnVehicle;
			ActiveVH = source.ActiveVH;
			ActiveVV = source.ActiveVV;
			ActiveVC = source.ActiveVC;
			//LineType = source.LineType;
		}

		public bool IsEqualTo(ITechListBenefitLine source)
		{
			var mySource = (TechListBenefitLine)source;
			if (ReferenceEquals(mySource, null)) {
				return false;
			}

			return this == mySource;
		}
	}
}
