using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockComponent : VectoSimulationComponent, ITnOutPort
	{
		public MockComponent() : base(null) {}
		protected override void DoWriteModalResults(IModalDataContainer container) {}

		protected override void DoCommitSimulationStep() {}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			if (dryRun)
				return new ResponseDryRun();
			else
				return new ResponseSuccess();
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return new ResponseSuccess();
		}
	}
}