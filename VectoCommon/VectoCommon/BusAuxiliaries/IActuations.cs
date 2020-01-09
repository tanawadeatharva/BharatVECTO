using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries
{
	public interface IActuations
	{
		int Braking { get; }
		Second CycleTime { get; }
		int Kneeling { get; }
		int ParkBrakeAndDoors { get; }
	}
}