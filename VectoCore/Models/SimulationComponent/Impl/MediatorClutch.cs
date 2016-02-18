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
	public class MediatorClutch : Clutch
	{
		public MediatorClutch(IVehicleContainer container) : base(container) {}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			var angularVelocityIn = DataBus.CycleData.LeftSample.AngularVelocity;
			angularVelocity = angularVelocity ?? 0.RPMtoRad();
			var torqueIn = torque * angularVelocity / angularVelocityIn;

			var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);

			if (!dryRun && retVal is ResponseUnderload && torque.IsEqual(0) && angularVelocity.IsEqual(0)) {
				var x1 = angularVelocityIn;
				var y1 = ((ResponseUnderload)retVal).Delta;
				angularVelocityIn = SearchAlgorithm.InterpolateQuadratic(x1, y1, 100.RPMtoRad(),
					r => ((ResponseDryRun)r).DeltaDragLoad,
					x => NextComponent.Request(absTime, dt, torqueIn, x),
					r => ((ResponseUnderload)r).Delta < Constants.SimulationSettings.EnginePowerSearchTolerance);
				retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn);
			}

			retVal.ClutchPowerRequest = torque * angularVelocity;
			return retVal;
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