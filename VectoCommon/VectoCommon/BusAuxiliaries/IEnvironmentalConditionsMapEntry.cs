using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IEnvironmentalConditionsMapEntry
	{
		int ID { get; }

		Kelvin Temperature { get; }
		WattPerSquareMeter Solar { get; }

		// already normalized weighting factor
		double Weighting { get; }


		//double GetNormalisedWeighting(IList<IEnvironmentalConditionsMapEntry> map);

		IReadOnlyDictionary<HeaterType, double> HeaterEfficiency { get; }

		IReadOnlyDictionary<HeatPumpType, double> HeatPumpCoP { get; }
	}
}
