using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class HybridStrategy : IHybridControlStrategy
	{
		private VectoRunData ModelData;

		public HybridStrategy(VectoRunData runData)
		{
			ModelData = runData;
		}

		public HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			return new HybridStrategyResponse() { MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>()};
		}

		public HybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return new HybridStrategyResponse()
				{ MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>() };
		}
	}

	
}