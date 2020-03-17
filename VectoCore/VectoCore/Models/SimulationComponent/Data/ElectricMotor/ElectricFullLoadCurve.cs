using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data {
	public class ElectricFullLoadCurve
	{
		internal readonly List<FullLoadEntry> FullLoadEntries;


		internal ElectricFullLoadCurve(List<FullLoadEntry> entries)
		{
			FullLoadEntries = entries;
		}


		public NewtonMeter FullLoadDriveTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return -VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
				FullLoadEntries[idx - 1].FullDriveTorque, FullLoadEntries[idx].FullDriveTorque,
				angularVelocity);
		}

		public NewtonMeter FullGenerationTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return -VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
				FullLoadEntries[idx - 1].FullGenerationTorque, FullLoadEntries[idx].FullGenerationTorque,
				angularVelocity);
		}

		protected int FindIndex(PerSecond angularVelocity)
		{
			if (angularVelocity < FullLoadEntries.First().MotorSpeed)
			{
				return 1;
			}
			if (angularVelocity > FullLoadEntries.Last().MotorSpeed)
			{
				return FullLoadEntries.Count - 1;

			}
			for (var index = 1; index < FullLoadEntries.Count; index++)
			{
				if (angularVelocity >= FullLoadEntries[index - 1].MotorSpeed && angularVelocity <= FullLoadEntries[index].MotorSpeed)
				{
					return index;
				}
			}
			throw new VectoException("angular velocity {0} exceeds full-load curve. min: {1} max: {2}", angularVelocity, FullLoadEntries.First().MotorSpeed, FullLoadEntries.Last().MotorSpeed);

		}

		internal class FullLoadEntry
		{
			public PerSecond MotorSpeed { get; set; }

			public NewtonMeter FullDriveTorque { get; set; }

			public NewtonMeter FullGenerationTorque { get; set; }
		}

	}
}