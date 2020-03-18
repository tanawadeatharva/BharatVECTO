using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class HybridController : IElectricMotorControl, IPowerTrainComponent
	{
		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
			PerSecond currOutAngularVelocity, bool dryRun)
		{
			throw new System.NotImplementedException();
		}

		public NewtonMeter MaxDriveTorque(PerSecond avgSpeed, Second dt)
		{
			throw new System.NotImplementedException();
		}

		public NewtonMeter MaxDragTorque(PerSecond avgSpeed, Second dt)
		{
			throw new System.NotImplementedException();
		}

		public ITnInPort InPort()
		{
			throw new System.NotImplementedException();
		}

		public ITnOutPort OutPort()
		{
			throw new System.NotImplementedException();
		}
	}
}