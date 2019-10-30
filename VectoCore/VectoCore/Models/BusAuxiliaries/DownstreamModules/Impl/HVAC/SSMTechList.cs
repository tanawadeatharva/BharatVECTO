using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used By SSMTOOL Class.
	public class SSMTechList : ISSMTechList
	{
		private readonly List<ISSMTechnology> _techLines;
		private FloorType _busFloorType;

		// Constructors
		public SSMTechList(FloorType busFloorType)
		{
			_techLines = new List<ISSMTechnology>();
			BusFloorType = busFloorType;
		}

		public IReadOnlyList<ISSMTechnology> TechLines
		{
			get { return _techLines; }
			set {
				_techLines.Clear();
				foreach (var item in value) {
					if (TechLines.Any(w => w.Category == item.Category && w.BenefitName == item.BenefitName)) {
						throw new ArgumentException("entry already exists");
					}
					item.BusFloorType = BusFloorType;
					_techLines.Add(item);
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

	}
}
