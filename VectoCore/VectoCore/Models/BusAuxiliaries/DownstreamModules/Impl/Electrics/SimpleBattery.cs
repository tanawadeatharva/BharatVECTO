using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class SimpleBattery : ISimpleBattery
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

		public void Request(Watt watt, Second seconds)
		{
			SOC += watt * seconds / Capacity;
		}
	}
}
