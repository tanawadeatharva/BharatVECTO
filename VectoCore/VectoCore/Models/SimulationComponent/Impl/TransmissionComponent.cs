using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public abstract class TransmissionComponent :
		StatefulVectoSimulationComponent<TransmissionComponent.TransmissionState>, IPowerTrainComponent, ITnInPort,
		ITnOutPort
	{
		protected ITnOutPort NextComponent;
		[ValidateObject] internal readonly TransmissionData ModelData;

		public class TransmissionState : SimpleComponentState
		{
			public TransmissionLossMap.LossMapResult TorqueLossResult;
			public NewtonMeter TorqueLoss = 0.SI<NewtonMeter>();
		}

		protected TransmissionComponent(IVehicleContainer container, TransmissionData modelData)
			: base(container)
		{
			ModelData = modelData;
		}

		public virtual ITnInPort InPort()
		{
			return this;
		}

		public virtual ITnOutPort OutPort()
		{
			return this;
		}

		public virtual void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public virtual IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			Log.Debug("request: torque: {0}, angularVelocity: {1}", torque, angularVelocity);

			var inAngularVelocity = angularVelocity * ModelData.Ratio;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + angularVelocity) / 2.0;

			var torqueLossResult = ModelData.LossMap.GetTorqueLoss(avgOutAngularVelocity, torque);
			var inTorque = torque / ModelData.Ratio + torqueLossResult.Value;

			CurrentState.SetState(inTorque, inAngularVelocity, torque, angularVelocity);
			CurrentState.TorqueLossResult = torqueLossResult;

			var retVal = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);
			return retVal;
		}

		public virtual IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var inAngularVelocity = angularVelocity * ModelData.Ratio;
			var torqueLossResult = ModelData.LossMap.GetTorqueLoss(angularVelocity, torque);
			var inTorque = torque / ModelData.Ratio + torqueLossResult.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, torque, angularVelocity);
			PreviousState.TorqueLossResult = torqueLossResult;

			return NextComponent.Initialize(inTorque, inAngularVelocity);
		}

		protected override void DoCommitSimulationStep()
		{
			if (CurrentState.TorqueLossResult.Extrapolated) {
				Log.Warn("{2} LossMap data was extrapolated: range for loss map is not sufficient: n:{0}, torque:{1}",
					CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque, GetType().Name);

				if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
					throw new VectoException(
						"{2} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{0}, torque:{1}",
						CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque, GetType().Name);
				}
			}
			AdvanceState();
		}
	}
}