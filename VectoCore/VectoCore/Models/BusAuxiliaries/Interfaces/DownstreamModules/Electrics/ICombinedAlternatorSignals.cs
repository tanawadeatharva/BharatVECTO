using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	// Used by CombinedAlternator
	public interface ICombinedAlternatorSignals
	{
		int NumberOfAlternators { get; set; }
		PerSecond CrankRPM { get; set; }
		Ampere CurrentDemandAmps { get; set; }
	}
}
