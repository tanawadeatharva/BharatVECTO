using System.Collections.Generic;

namespace DownstreamModules.HVAC
{
	public interface IEnvironmentalCondition
	{
		double GetTemperature();
		double GetSolar();
		double GetWeighting();
		double GetNormalisedWeighting(List<IEnvironmentalCondition> map);
	}
}
