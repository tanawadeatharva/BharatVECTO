using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data {
	public class ElectricMotorFullLoadCurve
	{
		internal readonly List<FullLoadEntry> FullLoadEntries;
		private NewtonMeter _maxDriveTorque;
		private NewtonMeter _maxGenerationTorque;
		private PerSecond _maxSpeed;


		internal ElectricMotorFullLoadCurve(List<FullLoadEntry> entries)
		{
			FullLoadEntries = entries;
		}


		public NewtonMeter FullLoadDriveTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
				FullLoadEntries[idx - 1].FullDriveTorque, FullLoadEntries[idx].FullDriveTorque,
				angularVelocity);
		}

		public NewtonMeter FullGenerationTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
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

		public NewtonMeter MaxDriveTorque
		{
			get { return _maxDriveTorque ?? (_maxDriveTorque = FullLoadEntries.Min(x => x.FullDriveTorque)); }
		}

		public NewtonMeter MaxGenerationTorque
		{
			get
			{
				return _maxGenerationTorque ??
						(_maxGenerationTorque = FullLoadEntries.Max(x => x.FullGenerationTorque));
			}
		}

		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = FullLoadEntries.Max(x => x.MotorSpeed)); }
		}

		[DebuggerDisplay("{MotorSpeed.AsRPM}: {FullDriveTorque} / {FullGenerationTorque}")]
		internal class FullLoadEntry
		{
			public PerSecond MotorSpeed { get; set; }

			public NewtonMeter FullDriveTorque { get; set; }

			public NewtonMeter FullGenerationTorque { get; set; }
		}

		public string[] SerializedEntries
		{
			get { return FullLoadEntries.Select(x => $"{x.MotorSpeed.AsRPM} {x.FullDriveTorque} {x.FullGenerationTorque}").ToArray(); }
		}


	}
}