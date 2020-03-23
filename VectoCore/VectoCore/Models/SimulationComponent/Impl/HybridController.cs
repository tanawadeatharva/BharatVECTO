using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class HybridController : StatefulProviderComponent<HybridController.HybridControllerState, ITnOutPort, ITnInPort, ITnOutPort>, IPowerTrainComponent, ITnInPort, ITnOutPort, IElectricMotorControl
	{
		public HybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es, IHybridControlledGearbox gbx, SwitchableClutch clutch) : base(container) { }


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


		
		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}	
		
		public class HybridControllerState { }

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
		{
			return NextComponent.Request(absTime, dt, outTorque, outAngularVelocity);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{

			return NextComponent.Initialize(outTorque, outAngularVelocity);
		}
	}


}