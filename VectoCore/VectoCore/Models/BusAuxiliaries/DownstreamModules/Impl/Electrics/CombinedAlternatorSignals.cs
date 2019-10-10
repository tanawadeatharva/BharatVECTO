using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	// Used by the CombinedAlternator class and any other related classes.
	public class CombinedAlternatorSignals : ICombinedAlternatorSignals
	{
		public double CrankRPM { get; set; }

		public Ampere CurrentDemandAmps { get; set; }

		// Number of alternators in the Combined Alternator
		public int NumberOfAlternators { get; set; }
	}
}
