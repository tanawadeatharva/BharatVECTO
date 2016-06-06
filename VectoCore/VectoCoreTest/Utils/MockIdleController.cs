using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockIdleController : ICombustionEngineIdleController
	{
		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			//throw new System.NotImplementedException();
			return new ResponseSuccess();
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			//throw new System.NotImplementedException();
			return new ResponseSuccess();
		}

		public ITnOutPort RequestPort { get; set; }

		public void Reset()
		{
			//throw new System.NotImplementedException();
		}
	}
}