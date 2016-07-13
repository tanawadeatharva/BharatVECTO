using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	// todo mk-2016-07-12: add dummy angular gear?

	public class AngularGear : TransmissionComponent
	{
		public AngularGear(IVehicleContainer container, AngularGearData modelData) : base(container, modelData.AngularGear) {}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			var retVal = base.Request(absTime, dt, torque, angularVelocity, dryRun);
			retVal.AngularGearPowerRequest = torque * (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			return retVal;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			//todo mk 2016-07-12: write angular gear modal results
			var avgAngularVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			//container[ModalResultField.P_axle_loss] = (CurrentState.InTorque - CurrentState.OutTorque / ModelData.Ratio) *avgAngularVelocity;
			//container[ModalResultField.P_axle_in] = CurrentState.InTorque * avgAngularVelocity;
		}
	}
}