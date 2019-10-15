using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used By SSMTOOL Class.
	public class SSMTechList : ISSMTechList
	{
		private readonly List<ITechListBenefitLine> _techLines;
		private FloorType _busFloorType;

		// Constructors
		public SSMTechList(FloorType busFloorType)
		{
			_techLines = new List<ITechListBenefitLine>();
			BusFloorType = busFloorType;
		}

		public IReadOnlyList<ITechListBenefitLine> TechLines
		{
			get { return _techLines; }
			set {
				_techLines.Clear();
				var fb = "";
				foreach (var entry in value) {
					Add(entry, ref fb);
				}
			}
		}

		public FloorType BusFloorType
		{
			get { return _busFloorType; }
			set {
				_busFloorType = value;
				foreach (var entry in _techLines) {
					entry.BusFloorType = BusFloorType;
				}
			}
		}

		

		// Public Properties - Outputs Of Class
		public double CValueVariation
		{
			get {
				double a;

				a = TechLines.Sum(s => s.C);

				return a;
			}
		}

		
		public double HValueVariation
		{
			get {

				// Dim a,b As double
				return TechLines.Sum(s => s.H);
			}
		}

		

		public double VCValueVariation
		{
			get {
				return TechLines.Sum(s => s.VC); // -  VCValueVariationKW
			}
		}

		
		public double VHValueVariation
		{
			get {
				// Dim a,b As double

				return TechLines.Sum(s => s.VH);
			}
		}

		
		public double VVValueVariation
		{
			get {
				return TechLines.Sum(s => s.VV);
			}
		}

		// Member Management
		public bool Add(ITechListBenefitLine item, ref string feedback)
		{
			var initialCount = TechLines.Count;

			if (TechLines.Any(w => w.Category == item.Category && w.BenefitName == item.BenefitName)) {
				// Failure
				feedback = "Item already exists.";
				return false;
			}


			try {
				item.BusFloorType = BusFloorType;
				_techLines.Add(item);

				if (TechLines.Count == initialCount + 1) {

					// Success
					feedback = "OK";
					//_dirty = true;
					return true;
				}

				// Failure
				feedback = "The system was unable to add the new tech benefit list item.";
				return false;
			} catch (Exception ) {
				feedback = "The system threw an exception and was unable to add the new tech benefit list item.";
				return false;
			}
		}

		public void Clear()
		{
			if (_techLines.Count > 0)
				//_dirty = true;

			_techLines.Clear();
		}

		public ITechListBenefitLine Find(string category, string benefitName)
		{
			return _techLines.Find(f => f.Category == category && f.BenefitName == benefitName);
		}

		public bool Delete(ITechListBenefitLine item, ref string feedback)
		{
			var currentCount = TechLines.Count;

			if ((TechLines.Count(c => c.Category == item.Category && c.BenefitName == item.BenefitName) == 1)) {
				try {
					_techLines.RemoveAt(_techLines.FindIndex(c => c.Category == item.Category && c.BenefitName == item.BenefitName));

					if (TechLines.Count == currentCount - 1) {
						// This succeeded
						//_dirty = true;
						return true;
					} else {
						// No Exception, but this failed for some reason.
						feedback = "The system was unable to remove the item from the list.";
						return false;
					}
				} catch (Exception) {
					feedback = "An exception occured, the removal failed.";
					return false;
				}
			} else {
				feedback = "the item was not found in the list.";
				return false;
			}
		}

		public bool Modify(ITechListBenefitLine originalItem, ITechListBenefitLine newItem, ref string feedback)
		{
			var fi = _techLines.Find(f => f.Category == originalItem.Category && f.BenefitName == originalItem.BenefitName);

			if (fi != null) {
				try {
					fi.CloneFrom(newItem);

					if (newItem == fi) {
						// This succeeded
						//_dirty = true;
						return true;
					}

					// No Exception, but this failed for some reason.
					feedback = "The system was unable to remove the item from the list.";
					return false;
				} catch (Exception ) {
					feedback = "An exception occured, the update failed.";
					return false;
				}
			}

			feedback = "the item was not found so cannot be modified.";
			return false;
		}

	}
}
