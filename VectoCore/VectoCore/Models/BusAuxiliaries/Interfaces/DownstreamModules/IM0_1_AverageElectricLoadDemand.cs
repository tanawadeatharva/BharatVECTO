using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
{
	public interface IM0_1_AverageElectricLoadDemand
	{
		Ampere TotalAverageDemandAmpsIncludingBaseLoad { get; }
		Ampere TotalAverageDemandAmpsWithoutBaseLoad { get; }
	}
}