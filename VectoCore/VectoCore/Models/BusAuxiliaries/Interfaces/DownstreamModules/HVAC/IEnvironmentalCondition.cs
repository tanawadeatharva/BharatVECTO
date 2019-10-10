using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface IEnvironmentalCondition
	{
		Kelvin GetTemperature();
		WattPerSquareMeter GetSolar();
		double GetWeighting();
		double GetNormalisedWeighting(List<IEnvironmentalCondition> map);
	}
}
