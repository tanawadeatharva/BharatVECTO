using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics {
	public interface ISimpleBattery {
		double SOC { get; }
		WattSecond Capacity { get; }
		void Request(WattSecond energy);
	}
}