using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	// Used by CombinedAlternator
	public interface ICombinedAlternatorSignals
	{
		int NumberOfAlternators { get; set; }
		double CrankRPM { get; set; }
		Ampere CurrentDemandAmps { get; set; }
	}
}
