using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class SimpleBattery : LoggingObject, ISimpleBattery
	{
		public SimpleBattery(WattSecond capacity, double soc = 0.9)
		{
			Capacity = capacity;
			SOC = soc;
		}

		#region Implementation of ISimpleBattery

		public double SOC { get; private set; }
		public WattSecond Capacity { get; }

		#endregion

		public void Request(WattSecond energy)
		{
			SOC += energy / Capacity;
			if (SOC > 1) {
				Log.Warn("SOC > 1!");
				SOC = 1;
			}
			if (SOC < 0) {
				Log.Warn("SOC < 0!");
				SOC = 0;
			}
		}
	}
}
