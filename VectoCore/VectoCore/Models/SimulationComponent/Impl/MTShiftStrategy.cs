using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class MTShiftStrategy : AMTShiftStrategy
	{
		public MTShiftStrategy(GearboxData data, IDataBus bus) : base(data, bus)
		{
			Data.EarlyShiftUp = false;
			Data.SkipGears = true;
		}

		protected override uint CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - Gearbox.LastDownshift).IsSmaller(10.SI<Second>())) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);
			if (nextGear == currentGear) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(0.1.SI<MeterPerSquareSecond>())) {
				// if less than 0.1 for next gear, don't shift
				if (nextGear - currentGear == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear > currentGear &&
					EstimateAccelerationForGear(currentGear + 1, outAngularVelocity).IsSmaller(0.1.SI<MeterPerSquareSecond>())) {
					return currentGear;
				}
			}

			return nextGear;
		}

		protected override uint CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			if ((absTime - Gearbox.LastUpshift).IsSmaller(10.SI<Second>())) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);
		}
	}
}