using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Controller
{
    internal class BatteryElectricIdleController : LoggingObject, IIdleController
	{
		private Second _idleStart;
		private ITnOutPort _requestPort;


		#region Implementation of ITnOutPort

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			if (outAngularVelocity != null)
			{
				throw new VectoException("IdleController can only handle idle requests, i.e. angularVelocity == null!");
			}
			if (!outTorque.IsEqual(0, 5e-2))
			{
				throw new VectoException("Torque has to be 0 for idle requests! {0}", outTorque);
			}

			if (dryRun) {
				throw new NotImplementedException();
			}
			if (_idleStart == null) {
				_idleStart = absTime;
			}

			var retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), 0.SI<PerSecond>(), dryRun);

			switch (retVal) {
				case ResponseSuccess _:
					break;
				default:
					throw new UnexpectedResponseException("Searching idling point", retVal);
			}

			return retVal;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IIdleController

		public ITnOutPort RequestPort
		{
			set => _requestPort = value;
			private get => _requestPort;
		}

		public void Reset()
		{
			_idleStart = null;
		}

		#endregion
	}
}
