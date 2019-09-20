using System.Collections.Generic;

namespace DownstreamModules.HVAC
{
	public interface IEnvironmentalConditionsMap
	{
		bool Initialise();

		List<IEnvironmentalCondition> GetEnvironmentalConditions();
	}
}
