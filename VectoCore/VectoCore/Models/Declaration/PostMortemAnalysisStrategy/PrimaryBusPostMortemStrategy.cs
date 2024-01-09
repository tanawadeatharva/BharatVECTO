using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy
{

	public class PrimaryBusPostMortemStrategy : IPostMortemAnalyzeStrategy
	{
		protected static readonly MeterPerSecond MinSpeed = 5.KMPHtoMeterPerSecond();

		#region Implementation of IPostMortemAnalyzeStrategy

		public bool AbortSimulation(IVehicleContainer container, Exception exception)
		{
			if (container.RunData.Mission.MissionType != MissionType.Interurban) {
				// for now only consider interurban cycle
				return true;
			}

			if (container.RunData.Loading != LoadingType.ReferenceLoad) {
				// for now only consider reference load
				return true;
			}

			if (!container.RunData.Mission.BusParameter.DoubleDecker) {
				// for now only consider double decker buses
				return true;
			}
			if (!container.DrivingCycleInfo.RoadGradient.IsGreater(0)) {
				// road gradient must be greater than 0
				return true;
			}

			if (!container.VehicleInfo.VehicleSpeed.IsGreater(MinSpeed)) {
				// vehicle has to be almost stopped
				return true;
			}
			
			var maxGradability = GetMaxGradability(container);
			if (maxGradability.IsSmaller(container.DrivingCycleInfo.RoadGradient)) {
				// the vehicle cannot go this steep uphill passage...
				return false;
			}
			return true;
		}

		private Radian GetMaxGradability(IVehicleContainer container)
		{
			return 0.SI<Radian>();
		}

		#endregion
	}
}