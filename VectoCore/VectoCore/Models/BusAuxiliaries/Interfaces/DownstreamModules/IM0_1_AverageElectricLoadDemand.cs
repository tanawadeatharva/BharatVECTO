using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
{
	public interface IM0_1_AverageElectricLoadDemand
	{
		Ampere GetTotalAverageDemandAmpsIncludingBaseLoad { get; }
		Ampere GetTotalAverageDemandAmpsWithoutBaseLoad { get; }
	}
}