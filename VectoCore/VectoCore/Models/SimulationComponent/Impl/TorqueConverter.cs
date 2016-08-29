using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TorqueConverter : StatefulVectoSimulationComponent<TorqueConverter.TorqueConverterComponentState>,
		ITnInPort, ITnOutPort
	{
		protected ATGearbox Gearbox;

		protected IShiftStrategy ShiftStrategy;

		protected TorqueConverterData ModelData;

		//protected bool SearchingTcOperatingPoint;

		public ITnOutPort NextComponent { protected internal get; set; }

		public TorqueConverter(ATGearbox gearbox, IShiftStrategy shiftStrategy, IVehicleContainer container,
			TorqueConverterData tcData) : base(container)
		{
			Gearbox = gearbox;
			ShiftStrategy = shiftStrategy;
			ModelData = tcData;
		}

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var operatingPoint = ModelData.FindOperatingPoint(outTorque, outAngularVelocity);

			var retVal = NextComponent.Initialize(operatingPoint.InTorque, operatingPoint.InAngularVelocity);
			PreviousState.SetState(operatingPoint.InTorque, operatingPoint.InAngularVelocity, operatingPoint.OutTorque,
				operatingPoint.OutAngularVelocity);
			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			if (dryRun) {
				var dryOperatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
				var deltaTorqueConverter = (outTorque - dryOperatingPoint.OutTorque) *
											(PreviousState.OutAngularVelocity + dryOperatingPoint.OutAngularVelocity) / 2.0;
				// operatingPoint.inAngularVelocity is for sure between engine idle speed and max TC speed
				var engineResponse =
					(ResponseDryRun)NextComponent.Request(absTime, dt, dryOperatingPoint.InTorque, dryOperatingPoint.InAngularVelocity,
						true);
				var deltaEngine = (engineResponse.DeltaFullLoad > 0 ? engineResponse.DeltaFullLoad : 0.SI<Watt>()) +
								(engineResponse.DeltaDragLoad < 0 ? -engineResponse.DeltaDragLoad : 0.SI<Watt>());
				if (deltaTorqueConverter.IsEqual(0) && deltaEngine.IsEqual(0)) {
					return new ResponseDryRun {
						Source = this,
						DeltaFullLoad = 0.SI<Watt>(),
						DeltaDragLoad = 0.SI<Watt>(),
						TorqueConverterOperatingPoint = dryOperatingPoint
					};
				}
				if (engineResponse.DeltaFullLoad > 0 || engineResponse.DeltaDragLoad < 0) {
					// engine is overloaded with current operating point, reduce torque...
					dryOperatingPoint =
						ModelData.GetOutTorqueAndSpeed(
							outTorque > 0 ? engineResponse.EngineMaxTorqueOut : engineResponse.EngineDragTorque,
							dryOperatingPoint.InAngularVelocity, null);
				}


				var delta = (outTorque - dryOperatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + dryOperatingPoint.OutAngularVelocity) / 2.0;
				//deltaTorqueConverter.Value() * (deltaEngine.IsEqual(0) ? 1 : deltaEngine.Value());
				return new ResponseDryRun() {
					Source = this,
					DeltaFullLoad = delta,
					DeltaDragLoad = delta,
					TorqueConverterOperatingPoint = dryOperatingPoint
				};
			}
			var operatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
			if (!outAngularVelocity.IsEqual(operatingPoint.OutAngularVelocity) || !outTorque.IsEqual(operatingPoint.OutTorque)) {
				// a different operating point was found...
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				if (!delta.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
					if (delta > 0) {
						return new ResponseOverload { Source = this, Delta = delta, TorqueConverterOperatingPoint = operatingPoint };
					}
					return new ResponseUnderload { Source = this, Delta = delta, TorqueConverterOperatingPoint = operatingPoint };
				}
			}
			var ratio = Gearbox.ModelData.Gears[Gearbox.Gear].TorqueConverterRatio;
			if (ShiftStrategy.ShiftRequired(absTime, dt, outTorque * ratio, outAngularVelocity / ratio, operatingPoint.InTorque,
				operatingPoint.InAngularVelocity, Gearbox.Gear, Gearbox.LastShift)) {
				return new ResponseGearShift() { Source = this };
			}
			CurrentState.SetState(operatingPoint.InTorque, operatingPoint.InAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.OperatingPoint = operatingPoint;
			var retVal = NextComponent.Request(absTime, dt, operatingPoint.InTorque, operatingPoint.InAngularVelocity, dryRun);
			return retVal;
		}

		protected TorqueConverterOperatingPoint FindOperatingPoint(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			try {
				var operatingPoint = ModelData.FindOperatingPoint(outTorque, outAngularVelocity);
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					throw new VectoException("Invalid operating point, inAngularVelocity below engine's idle speed: {0}",
						operatingPoint.InAngularVelocity);
				}
				if (operatingPoint.InAngularVelocity.IsGreater(ModelData.TorqueConverterSpeedLimit)) {
					operatingPoint = ModelData.GetOutTorque(ModelData.TorqueConverterSpeedLimit, outAngularVelocity);
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Debug(ve, "failed to find torque converter operating point, fallback: creeping");
				var tqOperatingPoint = ModelData.GetOutTorque(DataBus.EngineIdleSpeed, outAngularVelocity);
				return tqOperatingPoint;
			}
		}

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
			//SearchingTcOperatingPoint = false;
			AdvanceState();
		}

		public void Locked(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
		}

		public class TorqueConverterComponentState : SimpleComponentState
		{
			public TorqueConverterOperatingPoint OperatingPoint;
		}
	}
}