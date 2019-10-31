using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface ISSMRun
	{
		
		Watt TotalW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
		Watt FuelW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
		Watt TechListAmendedFuelW(Kelvin environmentTemperature, WattPerSquareMeter solarFactor);
	}
}
