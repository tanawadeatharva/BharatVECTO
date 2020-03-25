using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public class HybridStrategyResponse
	{
		public Dictionary<PowertrainPosition, NewtonMeter> MechanicalAssistPower;
		public bool ShiftRequired { get; set; }
		public uint NextGear { get; set; }
	}

	public interface IHybridControlStrategy
	{
		HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun);
		HybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);
	}
}