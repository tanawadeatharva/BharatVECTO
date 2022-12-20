using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public interface IFollowUpRunCreatorFactory
	{
		IFollowUpRunCreator CreateFollowUpRunCreator(VectoRunData runData);
	}
	public class FollowUpRunCreatorFactory : IFollowUpRunCreatorFactory
	{
		#region Implementation of IFollowUpRunCreatorFactory

		public IFollowUpRunCreator CreateFollowUpRunCreator(VectoRunData runData)
		{
            if (runData.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining && runData.JobType == VectoSimulationJobType.ParallelHybridVehicle)
            {
				return new FollowUpOvcRunCreator();
            }
            else
            {
                return new NoFollowUpRunCreator();
            }
		}

		#endregion
	}
}