using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface ILossMap
	{
		/// <summary>
		/// Calculates the torque loss.
		/// </summary>
		NewtonMeter GetTorqueLoss(PerSecond angularVelocity);
	}
}