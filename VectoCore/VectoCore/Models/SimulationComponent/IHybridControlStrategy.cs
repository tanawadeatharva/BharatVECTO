using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IHybridControlStrategy
	{
		HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun);
		HybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);
		void CommitSimulationStep(Second time, Second simulationInterval);
		IHybridController Controller { set; }
		void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container);
	}
}