using System;
using System.Collections.Generic;
using System.Dynamic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public interface IFollowUpRunCreator
	{
		bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action beforeNextRun);
	}


	public class NoFollowUpRunCreator : LoggingObject, IFollowUpRunCreator
	{
		#region Implementation of IFollowUpRunCreator

		public bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action beforeNextRun)
		{
			return false;
		}

		#endregion
	}

	/// <summary>
	/// This class decides if a run is executed another time, stores the relevant data from the current run and prepares the VehicleContainer for the next run
	/// </summary>
	public class FollowUpRunCreator : LoggingObject, IFollowUpRunCreator
	{
		private string original_modfile_suffix = null;
		private int iteration = 0;

		private readonly IIterativeRunStrategy _strategy;

		public FollowUpRunCreator(IIterativeRunStrategy strategy)
		{
			_strategy = strategy;
		}

		/// <summary>
		/// Determines if a run should be simulated again, if the run should be simulated again,
		/// necessary simulation components are reset, the rundata is adjusted and the runAgainAction is executed
		/// </summary>
		/// <param name="runAgainAction"></param>
		/// <param name="run"></param>
		/// <return>true if the run is executed again, false otherwise</return>
		public bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action beforeNextRun)
		{
			var modalDataContainer = run.GetContainer().ModalData;
			var vectoRunData = run.GetContainer().RunData;
			if (!_strategy.RunAgain(iteration, modalDataContainer, vectoRunData) || !run.FinishedWithoutErrors) {
				return false;
			}
			original_modfile_suffix = original_modfile_suffix ?? (original_modfile_suffix = vectoRunData.ModFileSuffix);

	
			Log.Info(string.Format("Run {0} again!", run.RunName));

			beforeNextRun();

			vectoRunData.ModFileSuffix = original_modfile_suffix + (iteration + 1);
			_strategy.UpdateRunData(iteration, modalDataContainer, vectoRunData);
			iteration++;
			runAgainAction(vectoRunData);
			return true;
		}
	}
}