using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules
{
	public interface IM14 : IAbstractModule
	{
		Kilogram TotalCycleFCGrams { get; }

		Liter TotalCycleFCLitres { get; }
	}
}
