using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalConditionsMap : IEnvironmentalConditionsMap
	{
		private readonly IReadOnlyList<IEnvironmentalConditionsMapEntry> _map;

		public EnvironmentalConditionsMap(IList<IEnvironmentalConditionsMapEntry> entries)
		{
			_map = new ReadOnlyCollection<IEnvironmentalConditionsMapEntry>(entries);
		}

		
		public IReadOnlyList<IEnvironmentalConditionsMapEntry> GetEnvironmentalConditions()
		{
			return _map;
		}

		public string[] SerializedEnvironmentalConditions
		{
			get { return _map.Select(e => $"{e.ID}: {e.Temperature.AsDegCelsius}, {e.Solar.SerializedValue}, {e.Weighting:F5}").ToArray(); }
		}
	}
}
