using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class CycleTorqueConverter : StatefulVectoSimulationComponent<TorqueConverter.TorqueConverterComponentState>
	{
		protected internal ITnOutPort NextComponent;
		private TorqueConverterData ModelData;

		public CycleTorqueConverter(IVehicleContainer container, TorqueConverterData modelData) : base(container)
		{
			ModelData = modelData;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity)
		{
			
			var operatingPoint = ModelData.LookupOperatingPoint(outAngularVelocity, inAngularVelocity, outTorque);

			PreviousState.OperatingPoint = operatingPoint;
			return NextComponent.Initialize(operatingPoint.InTorque, inAngularVelocity);
		}

		public IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity,
			bool dryRun = false)
		{
			var operatingPoint = ModelData.LookupOperatingPoint(outAngularVelocity, inAngularVelocity, outTorque);
			if (!dryRun) {
				CurrentState.OperatingPoint = operatingPoint;
			}
			return NextComponent.Request(absTime, dt, operatingPoint.InTorque, inAngularVelocity, dryRun);
		}

		public void Locked(
			NewtonMeter inTorque, PerSecond inAngularVelocity, NewtonMeter outTorque,
			PerSecond outAngularVelocity) { }

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			if (CurrentState.OperatingPoint == null) {
				container[ModalResultField.TorqueConverterTorqueRatio] = 1.0;
				container[ModalResultField.TorqueConverterSpeedRatio] = 1.0;
			} else {
				container[ModalResultField.TorqueConverterTorqueRatio] = CurrentState.OperatingPoint.TorqueRatio;
				container[ModalResultField.TorqueConverterSpeedRatio] = CurrentState.OperatingPoint.SpeedRatio;
			}
			container[ModalResultField.TC_TorqueIn] = CurrentState.InTorque;
			container[ModalResultField.TC_TorqueOut] = CurrentState.OutTorque;
			container[ModalResultField.TC_angularSpeedIn] = CurrentState.InAngularVelocity;
			container[ModalResultField.TC_angularSpeedOut] = CurrentState.OutAngularVelocity;

			var avgOutVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var avgInVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_TC_out] = CurrentState.OutTorque * avgOutVelocity;
			container[ModalResultField.P_TC_loss] = CurrentState.InTorque * avgInVelocity -
													CurrentState.OutTorque * avgOutVelocity;
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		#endregion
	}
}