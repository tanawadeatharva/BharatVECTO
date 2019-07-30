using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class RoadSweeperAuxiliary
	{
		protected IDataBus DataBus;

		public RoadSweeperAuxiliary(IVehicleContainer container)
		{
			DataBus = container;
		}

		public bool Active(Second absTime)
		{
			return DataBus.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping;
		}

		public Watt PowerDemand(PerSecond engineSpeed, Second absTime, Second dt, bool dryRun)
		{
			var left = DataBus.CycleData.LeftSample;
			//var acc = DataBus.DriverAcceleration;
			//var vehicleSpeed = DataBus.VehicleSpeed;

			var powerDemand = 0.SI<Watt>();

			if (left.PTOActive == PTOActivity.PTOActivityRoadSweeping) {
				// todo: iterate over cycle from left to right and compute pto energy and then average power demand
				powerDemand = left.PTOPowerDemandDuringDrive;
			}

			return  powerDemand;

		}
	}
}
