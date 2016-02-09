using System;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class DummyRetarder : VectoSimulationComponent, IPowerTrainComponent, ITnInPort, ITnOutPort
	{
		protected ITnOutPort NextComponent;

		public DummyRetarder(IVehicleContainer dataBus) : base(dataBus) {}

		public ITnInPort InPort()
		{
			return this;
		}

		public ITnOutPort OutPort()
		{
			return this;
		}

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			return NextComponent.Request(absTime, dt, torque, angularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			return NextComponent.Initialize(torque, angularVelocity);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.P_ret_loss] = 0.SI<Watt>();
		}

		protected override void DoCommitSimulationStep() {}
	}
}