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
			try {
				var operatingPoint = ModelData.FindOperatingPoint(outTorque, outAngularVelocity);
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					throw new VectoException("Invalid operating point, inAngularVelocity below engine's idle speed: {0}",
						operatingPoint.InAngularVelocity);
				}
				if (operatingPoint.InAngularVelocity.IsGreater(ModelData.TorqueConverterSpeedLimit)) {
					// TODO!
					throw new NotImplementedException();
				}
				if (ShiftStrategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, operatingPoint.InTorque,
					operatingPoint.InAngularVelocity, Gearbox.Gear, Gearbox.LastShift)) {
					return new ResponseGearShift() { Source = this };
				}
				CurrentState.SetState(operatingPoint.InTorque, operatingPoint.InAngularVelocity, operatingPoint.OutTorque,
					operatingPoint.OutAngularVelocity);
				var retVal = NextComponent.Request(absTime, dt, operatingPoint.InTorque, operatingPoint.InAngularVelocity, dryRun);

				return retVal;
			} catch (VectoException ve) {
				Log.Debug(ve, "failed to find torque converter operating point, fallback: creeping");
				var tqOperatingPoint = ModelData.GetOutTorque(DataBus.EngineIdleSpeed, outAngularVelocity);
				var delta = (outTorque - tqOperatingPoint.OutTorque) * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
				if (dryRun) {
					return new ResponseDryRun() {
						Source = this,
						DeltaDragLoad = delta,
					};
				}
				if (delta.IsEqual(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance.SI<Watt>())) {
					return NextComponent.Request(absTime, dt, tqOperatingPoint.InTorque, tqOperatingPoint.InAngularVelocity, dryRun);
				}
				return new ResponseUnderload() {
					Source = this,
					Delta = (outTorque - tqOperatingPoint.OutTorque) * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0,
				};
			}
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			// TODO!
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}
	}
}