using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.Simulation {
	public interface ISimulationPreprocessor
	{
		void RunPreprocessing(VectoRun container);
	}
}