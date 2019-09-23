using System.Collections.Generic;

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface IEnvironmentalConditionsMap
	{
		bool Initialise();

		List<IEnvironmentalCondition> GetEnvironmentalConditions();
	}
}
