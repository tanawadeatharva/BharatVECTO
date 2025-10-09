using System;
using TUGraz.VectoCommon.Models;
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

    public class DefaultPostMortemAnalyzer : LoggingObject, IPostMortemAnalyzer
	{
		protected readonly IPostMortemAnalyzeStrategy _strategy;

		public DefaultPostMortemAnalyzer(IPostMortemAnalyzeStrategy postMortemStrategy)
		{
			_strategy = postMortemStrategy;
		}

		#region Implementation of IPostMortemAnalyzer

		public bool AbortSimulation(IVehicleContainer container, Exception ex)
		{
			try {
				if (!_strategy.AbortSimulation(container, ex)) {
					container.RunStatus = VectoRun.Status.PrimaryBusSimulationIgnore;
					return false;
				}
			} catch (Exception e) {
				Log.Error("Exception during post-mortem analysis", e);
			}

			return true;
        }

		#endregion
	}



}