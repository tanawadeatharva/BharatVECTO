using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Impl
{
	public class CycleInputData : ICycleData
	{
		public string Name { get; internal set; }

		public DataTable CycleData { get; internal set; }
	}

	public class StartStopInputData : IStartStopInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSecond MaxSpeed { get; internal set; }

		public Second MinTime { get; internal set; }

		public Second Delay { get; internal set; }
	}

	public class LookAheadCoastingInputData : ILookaheadCoastingInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSquareSecond Deceleration { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }
	}

	public class OverSpeedEcoRollInputData : IOverspeedEcoRollInputData
	{
		public DriverData.DriverMode Mode { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }

		public MeterPerSecond OverSpeed { get; internal set; }

		public MeterPerSecond UnderSpeed { get; internal set; }
	}

	public class AuxiliaryDataInputData : IAuxiliaryInputData
	{
		public bool SavedInDeclarationMode { get; internal set; }

		public string ID { get; internal set; }

		public string Type { get; internal set; }

		public string Technology { get; internal set; }

		public IList<string> TechList { get; internal set; }

		public double TransmissionRatio { get; internal set; }

		public double EfficiencyToEngine { get; internal set; }

		public double EfficiencyToSupply { get; internal set; }

		public DataTable DemandMap { get; internal set; }
	}
}