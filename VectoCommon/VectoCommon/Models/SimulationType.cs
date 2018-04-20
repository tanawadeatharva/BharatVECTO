using System;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	[Flags]
	public enum SimulationType
	{
		None = 0,
		EngineOnly = 1 << 0,
		DistanceCycle = 1 << 1,
		MeasuredSpeedCycle = 1 << 2,
		PWheel = 1 << 3,
		VerificationTest = 1 << 4
	}
}