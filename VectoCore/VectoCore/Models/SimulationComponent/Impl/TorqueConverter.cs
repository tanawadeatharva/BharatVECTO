using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TorqueConverter : ITnInPort, ITnOutPort
	{
		protected internal ITnOutPort NextComponent;

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			throw new System.NotImplementedException();
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			throw new System.NotImplementedException();
		}
	}
}