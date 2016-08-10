using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TorqueConverter : StatefulVectoSimulationComponent<SimpleComponentState>, ITnInPort, ITnOutPort
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
				if (deltaTorqueConverter.IsEqual(0)) {
					if (deltaEngine.IsEqual(0)) {
						return new ResponseDryRun { Source = this, DeltaFullLoad = deltaTorqueConverter };
					}
					return engineResponse;
				}

				var delta = deltaTorqueConverter.Value() * (deltaEngine.IsEqual(0) ? 1 : deltaEngine.Value());
				return new ResponseDryRun() {
					Source = this,
					DeltaFullLoad = delta.SI<Watt>()
				};
			}
			var operatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
			if (!outAngularVelocity.IsEqual(operatingPoint.OutAngularVelocity) || !outTorque.IsEqual(operatingPoint.OutTorque)) {
				// a different operating point was found...
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				if (delta > 0) {
					return new ResponseOverload { Source = this, Delta = delta };
				}
				return new ResponseUnderload { Source = this, Delta = delta };
			}
			var ratio = Gearbox.ModelData.Gears[Gearbox.Gear].TorqueConverterRatio;
			if (ShiftStrategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity / ratio, operatingPoint.InTorque * ratio,
				operatingPoint.InAngularVelocity, Gearbox.Gear, Gearbox.LastShift)) {
				return new ResponseGearShift() { Source = this };
			}
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

		public void CommitSimulationStep()
		{
			DoCommitSimulationStep();
		}

		public void WriteModalResults(IModalDataContainer container)
		{
			DoWriteModalResults(container);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			// TODO!
		}

		protected override void DoCommitSimulationStep()
		{
			//SearchingTcOperatingPoint = false;
			AdvanceState();
		}

		public void Locked(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
		}
	}
}