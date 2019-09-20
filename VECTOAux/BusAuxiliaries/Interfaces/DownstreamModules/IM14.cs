using TUGraz.VectoCommon.Utils;


namespace DownstreamModules
{
	public interface IM14 : IAbstractModule
	{
		Kilogram TotalCycleFCGrams { get; }

		Liter TotalCycleFCLitres { get; }
	}
}
