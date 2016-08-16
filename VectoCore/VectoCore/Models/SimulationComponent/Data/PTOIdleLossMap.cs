using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// LossMap for PTO Idle losses.
	/// </summary>
	public class PTOIdleLossMap : SimulationComponentData
	{
		[ValidateObject] private readonly Entry[] _entries;

		protected internal PTOIdleLossMap(Entry[] entries)
		{
			_entries = entries;
		}

		/// <summary>
		/// Calculates the pto torque loss.
		/// </summary>
		/// <param name="angularVelocity"></param>
		/// <returns></returns>
		public NewtonMeter PTOTorqueLoss(PerSecond angularVelocity)
		{
			var s = _entries.GetSection(e => e.EngineSpeed < angularVelocity);
			return VectoMath.Interpolate(s.Item1.EngineSpeed, s.Item2.EngineSpeed, s.Item1.PTOTorque, s.Item2.PTOTorque,
				angularVelocity);
		}

		public class Entry
		{
			[Required, SIRange(0, double.MaxValue)] public PerSecond EngineSpeed;
			[Required, SIRange(0, 1000)] public NewtonMeter PTOTorque;
		}
	}
}