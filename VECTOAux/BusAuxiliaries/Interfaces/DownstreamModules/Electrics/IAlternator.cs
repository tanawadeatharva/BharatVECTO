using System.Collections.Generic;

namespace DownstreamModules.Electrics
{
	public class Table4Row
	{
		public double RPM;
		public double Efficiency;

		public Table4Row(double rpm, double eff)
		{
			this.RPM = rpm;
			this.Efficiency = eff;
		}
	}

	// Used By Combined Alternator.
	// Model based on CombinedALTS_V02_Editable.xlsx
	public interface IAlternator
	{
		// D6
		string AlternatorName { get; set; }

		// G6
		double PulleyRatio { get; set; }

		// S9
		double SpindleSpeed { get; }

		// S10
		double Efficiency { get; }

		// C10-D15
		List<AltUserInput> InputTable2000 { get; set; }

		// F10-G15
		List<AltUserInput> InputTable4000 { get; set; }

		// I10-J15
		List<AltUserInput> InputTable6000 { get; set; }

		// M10-N15
		List<Table4Row> RangeTable { get; set; }

		// Test Equality
		bool IsEqualTo(IAlternator other);
	}
}
