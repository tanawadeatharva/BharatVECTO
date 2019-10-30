using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMRun
	{
		//double HVACOperation { get; }
		//Kelvin TCalc { get; }
		//Kelvin TemperatureDelta { get; }
		//Watt QWall { get; }
		//Watt WattsPerPass { get; }
		//Watt Solar { get; }
		Watt TotalW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
		//double TotalKW { get; }
		Watt FuelW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
		Watt TechListAmendedFuelW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
	}
}
