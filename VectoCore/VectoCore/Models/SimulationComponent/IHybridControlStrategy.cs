using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public class HybridStrategyResponse
	{
		public Dictionary<PowertrainPosition, NewtonMeter> MechanicalAssistPower;
		public bool ShiftRequired { get; set; }
		public uint NextGear { get; set; }
		public bool GearboxInNeutral { get; set; }
		public bool CombustionEngineOn { get; set; }
	}

	public interface IHybridControlStrategy
	{
		HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun);
		HybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);
		void CommitSimulationStep(Second time, Second simulationInterval);
		IHybridController Controller { set; }
	}
}