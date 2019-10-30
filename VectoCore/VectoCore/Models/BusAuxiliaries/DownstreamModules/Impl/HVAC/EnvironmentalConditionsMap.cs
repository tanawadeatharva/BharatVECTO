using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.Util;

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
	}
}
