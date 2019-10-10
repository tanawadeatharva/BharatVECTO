using System;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	// Used by the Combined Alternator Form/Classes to accept user input for the combined alternators efficiency
	// At different Current Demands
	public class AltUserInput
	{
		public double Amps;
		public double Eff;

		// Constructor
		public AltUserInput(double amps, double eff)
		{
			this.Amps = amps;
			this.Eff = eff;
		}

		// Equality
		public bool IsEqual(AltUserInput other, int rounding = 7)
		{
			return Math.Round(this.Amps, rounding) == Math.Round(other.Amps, rounding) &&
					Math.Round(this.Eff, rounding) == Math.Round(other.Eff, rounding);
		}
	}
}
