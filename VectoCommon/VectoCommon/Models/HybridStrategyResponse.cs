using System;
using System.Collections.Generic;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models {
	public class HybridStrategyResponse
	{
		public Dictionary<PowertrainPosition, NewtonMeter> MechanicalAssistPower;
		public bool ShiftRequired { get; set; }
		public uint NextGear { get; set; }
		public bool GearboxInNeutral { get; set; }
		public bool CombustionEngineOn { get; set; }

		public HybridResultEntry EvaluatedSolution { get; set; }
	}

	[DebuggerDisplay("{U}: {Score} - G{Gear}")]
	public class HybridResultEntry
	{
		public double U { get; set; }

		public HybridStrategyResponse Setting { get; set; }

		public IResponse Response { get; set; }

		public double Score { get { return (FuelCosts + EqualityFactor * (BatCosts + ICEStartPenalty1) * SoCPenalty + ICEStartPenalty2) / GearshiftPenalty; } }

		public double FuelCosts { get; set; }

		public double BatCosts { get; set; }

		public double SoCPenalty { get; set; }

		public double EqualityFactor { get; set; }

		public double GearshiftPenalty { get; set; }

		public double ICEStartPenalty1 { get; set; }

		public double ICEStartPenalty2 { get; set; }

		public uint Gear { get; set; }

		public bool ICEOff { get; set; }

		public HybridConfigurationIgnoreReason IgnoreReason { get; set; }
	}

	[Flags]
	public enum HybridConfigurationIgnoreReason
	{
		NotEvaluated = 0,
		EngineSpeedTooLow = 1 << 2,
		EngineSpeedTooHigh = 1 << 3,
		EngineTorqueDemandTooHigh = 1 << 4,
		EngineTorqueDemandTooLow = 1 << 5,
		EngineSpeedAboveUpshift = 1 << 6,
		EngineSpeedBelowDownshift = 1 << 7,
		NoResponseAvailable = 1 << 8,
		Evaluated = 1 << 9,
	}
}