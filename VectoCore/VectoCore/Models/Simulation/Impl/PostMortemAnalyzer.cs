using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public interface IPostMortemAnalyzer
	{
		/**
		 * @returns true if the original exception shall be thrown
		 */
		bool AbortSimulation(IVehicleContainer container, Exception ex);

	}

	public class NoPostMortemAnalysis : IPostMortemAnalyzer
	{
		#region Implementation of IPostMortemAnalyzer

		public bool AbortSimulation(IVehicleContainer container, Exception ex)
		{
			return true;
		}

		#endregion
	}

    public class DefaultPostMortemAnalyzer : IPostMortemAnalyzer
	{
		protected readonly IPostMortemAnalyzeStrategy _strategy;

		public DefaultPostMortemAnalyzer(IPostMortemAnalyzeStrategy postMortemStrategy)
		{
			_strategy = postMortemStrategy;
		}

		#region Implementation of IPostMortemAnalyzer

		public bool AbortSimulation(IVehicleContainer container, Exception ex)
		{
			if (!_strategy.AbortSimulation(container, ex)) {
				//_strategy.M
				container.RunStatus = VectoRun.Status.PrimaryBusSimulationIgnore;
				return false;
			}
			return true;
		}

		#endregion
	}



}