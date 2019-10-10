using System.Collections.Generic;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
{
	public interface IEnvironmentalConditionsMap
	{
		bool Initialise();

		List<IEnvironmentalCondition> GetEnvironmentalConditions();
	}
}
