using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class PTOEngineCycleController : PowertrainDrivingCycle, ICombustionEngineIdleController
	{
		public ITnOutPort RequestPort { get; set; }
		protected Second IdleStart;

		public PTOEngineCycleController(IVehicleContainer container, DrivingCycleData cycle)
			: base(container, cycle) {}

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
			return base.Request(absTime, dt);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return new ResponseSuccess { Source = this };
		}

		public void Reset()
		{
			LeftSample = Data.Entries.GetEnumerator();
			LeftSample.MoveNext();

			RightSample = Data.Entries.GetEnumerator();
			RightSample.MoveNext();
			RightSample.MoveNext();

			IdleStart = null;
		}
	}
}