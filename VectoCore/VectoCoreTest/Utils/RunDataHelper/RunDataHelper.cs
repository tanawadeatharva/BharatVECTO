using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;

namespace TUGraz.VectoCore.Tests.Utils.RunDataHelper
{
    internal static class RunDataHelper
    {
		public static BatteryData SetUsableCapacity(this BatteryData batData, WattSecond usable)
		{
			if (usable < 0)
			{
				throw new ArgumentException("Energy must not be negative");
			}
			var currentUsable = batData.UseableStoredEnergy;
			var ratio = usable / currentUsable;

			batData.Capacity = batData.Capacity * ratio;


			return batData;
		}

		public static BatterySystemData SetUsableCapacity(this BatterySystemData batData, WattSecond usable)
		{
			if (usable < 0)
			{
				throw new ArgumentException("Energy must not be negative");
			}

			var currentUsable = batData.UseableStoredEnergy;

			var ratio = usable / currentUsable;

			foreach (var tuple in batData.Batteries)
			{
				tuple.Item2.SetUsableCapacity(tuple.Item2.UseableStoredEnergy * ratio);
			}

			return batData;
		}







	}
}
