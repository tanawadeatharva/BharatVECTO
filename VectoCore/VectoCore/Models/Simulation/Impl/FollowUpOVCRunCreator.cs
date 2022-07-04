using System;
using System.Dynamic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
//	public interface IFollowUpRunCreator
//	{
//		public 
//	}

public interface IFollowUpRunCreator
{
	bool RunAgain(Action runAgainAction, IVectoRun run, Action beforeNextRun);
}


public class NoFollowUpRunCreator : LoggingObject, IFollowUpRunCreator
{
	#region Implementation of IFollowUpRunCreator

	public bool RunAgain(Action runAgainAction, IVectoRun run)
	{
		return false;
	}

	#endregion

	#region Implementation of IFollowUpRunCreator

	public bool RunAgain(Action runAgainAction, IVectoRun run, Action beforeNextRun)
	{
		throw new NotImplementedException();
	}

	#endregion
}
/// <summary>
	/// This class decides if a run is executed another time, stores the relevant data from the current run and prepares the VehicleContainer for the next run
	/// </summary>
	public class FollowUpOvcRunCreator : LoggingObject, IFollowUpRunCreator
{
	
		private int testRunCount = 1;

		private int iteration = 0;
		public FollowUpOvcRunCreator()
		{
		}

		/// <summary>
		/// Determines if a run should be simulated again, if the run should be simulated again,
		/// necessary simulation components are reset, the rundata is adjusted and the runAgainAction is executed
		/// </summary>
		/// <param name="runAgainAction"></param>
		/// <param name="run"></param>
		public bool RunAgain(Action runAgainAction, IVectoRun run, Action beforeNextRun)
		{
			var container = run.GetContainer();
			if (!ShouldRunAgain(run)) {
				return false;
			}

			beforeNextRun();

			

			//Assign a new modaldata container to the current run, preserve the current modaldatacontainer for finishing the simulation
			//var runData = container.RunData;
			
   //         container.ModalData = new ModalDataContainer(runData:runData, addReportResult:)
			
			//(container.ModalData as ModalDataContainer).PushResults();
			
			ResetSimulationComponents(container);
			UpdateRunData(container.RunData);//Stores results from current run on the stack of ModalDataContainer
			runAgainAction();
			ResetRunData(container.RunData);

			return true;
		}

		private void ResetRunData(VectoRunData containerRunData)
		{
			
		}

		private void UpdateRunData(VectoRunData runData)
		{
			runData.ModFileSuffix += ++iteration;
			runData.VehicleData.Loading -= 100d.SI<Kilogram>(); //TEST
			runData.VehicleData.Manufacturer += (testRunCount + 1);
		}

		private void ResetSimulationComponents(IVehicleContainer vehicleContainer)
		{
			vehicleContainer.RunStatus = VectoRun.Status.Pending;
			vehicleContainer.ResetComponents();
		}


		private bool ShouldRunAgain(IVectoRun run)
		{
			Log.Info(string.Format("Run {0} again!", run.RunName));
			return (testRunCount-- > 0);
		}
	}

}