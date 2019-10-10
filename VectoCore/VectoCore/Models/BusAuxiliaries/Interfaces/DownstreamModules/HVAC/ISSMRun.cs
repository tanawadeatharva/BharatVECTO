using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMRun
	{
		double HVACOperation { get; }
		Kelvin TCalc { get; }
		Kelvin TemperatureDelta { get; }
		Watt QWall { get; }
		Watt WattsPerPass { get; }
		Watt Solar { get; }
		Watt TotalW { get; }
		//double TotalKW { get; }
		Watt FuelW { get; }
		Watt TechListAmendedFuelW { get; }
	}
}
