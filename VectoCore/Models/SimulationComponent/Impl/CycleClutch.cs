using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Clutch which mediates angularSpeed from powertrain (depending on vehicle speed) with engineSpeed directly set from driving cycle.
	/// Can be thought as: Clutch which is always slipping.
	/// </summary>
	public class CycleClutch : Clutch
	{
		public CycleClutch(IVehicleContainer container) : base(container) {}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			var angularVelocityIn = DataBus.CycleData.LeftSample.AngularVelocity;

			if (angularVelocity != null) {
				// engaged - act like transmission torque converter (convert torque for angularVelocity to torque for angularVelocityIn) 
				// convert requested power to equivalent torque with angularVelocityIn
				var torqueIn = torque * angularVelocity / angularVelocityIn;

				var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);
				retVal.ClutchPowerRequest = torque * angularVelocity;
				return retVal;
			} else {
				// disengaged -> clutch open
				var retVal = NextComponent.Request(absTime, dt, torque, angularVelocityIn, dryRun);
				return retVal;
			}
		}

		public override IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var angularVelocityIn = DataBus.CycleData.LeftSample.AngularVelocity;
			var torqueIn = torque * angularVelocity / angularVelocityIn;

			var retVal = NextComponent.Initialize(torqueIn, angularVelocityIn);
			retVal.ClutchPowerRequest = torque * angularVelocity;
			return retVal;
		}
	}
}