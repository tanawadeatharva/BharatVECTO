using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATGearbox : AbstractGearbox<GearboxState>, IGearbox, ITnOutPort, ITnInPort,
		IClutchInfo
	{
		public ATGearbox(IVehicleContainer container, GearboxData gearboxModelData) : base(container, gearboxModelData)
		{
		
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			throw new System.NotImplementedException();
		}

		protected override void DoCommitSimulationStep()
		{
			throw new System.NotImplementedException();
		}

		
		public override IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			throw new System.NotImplementedException();
		}

		public override IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			throw new System.NotImplementedException();
		}

		public bool ClutchClosed(Second absTime)
		{
			throw new System.NotImplementedException();
		}

		public class GearboxState : SimpleComponentState {}
	}
}