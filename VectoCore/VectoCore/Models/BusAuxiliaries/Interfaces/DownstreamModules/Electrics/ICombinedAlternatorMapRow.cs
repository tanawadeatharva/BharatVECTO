using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	// Reflects stored data in pesisted CombinedAlternator Map .AALT
	public interface ICombinedAlternatorMapRow
	{
		string AlternatorName { get; set; }
		PerSecond RPM { get; set; }
		Ampere Amps { get; set; }
		double Efficiency { get; set; }
		double PulleyRatio { get; set; }
	}
}
