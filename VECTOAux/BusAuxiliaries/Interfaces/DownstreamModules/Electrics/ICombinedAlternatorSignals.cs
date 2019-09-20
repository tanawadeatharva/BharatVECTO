using TUGraz.VectoCommon.Utils;

namespace DownstreamModules.Electrics
{
	// Used by CombinedAlternator
	public interface ICombinedAlternatorSignals
	{
		int NumberOfAlternators { get; set; }
		double CrankRPM { get; set; }
		Ampere CurrentDemandAmps { get; set; }
	}
}
