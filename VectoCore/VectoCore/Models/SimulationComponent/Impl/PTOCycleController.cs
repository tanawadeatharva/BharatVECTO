using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class PTOCycleController : PowertrainDrivingCycle, IIdleController
	{
		public ITnOutPort RequestPort
		{
			set { NextComponent = value; }
		}

		protected Second IdleStart;

		public PTOCycleController(DrivingCycleData cycle) : base(null, cycle) {}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			if (outAngularVelocity != null) {
				throw new VectoException("{0} can only handle idle requests: AngularVelocity has to be null!", GetType().ToString());
			}
			if (!outTorque.IsEqual(0)) {
				throw new VectoException("{0} can only handle idle requests: Torque has to be 0!", GetType().ToString());
			}
			if (IdleStart == null) {
				IdleStart = absTime;
			}
			return base.Request(absTime - IdleStart, dt);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return new ResponseSuccess { Source = this };
		}

		public void Reset()
		{
			LeftSample.Reset();
			LeftSample.MoveNext();

			RightSample.Reset();
			RightSample.MoveNext();
			RightSample.MoveNext();

			IdleStart = null;
		}

		public Second GetNextCycleTime()
		{
			if (RightSample.Current == null)
				return null;

			return RightSample.Current.Time;
		}
	}
}