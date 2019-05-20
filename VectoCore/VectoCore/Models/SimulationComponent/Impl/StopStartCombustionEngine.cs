using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class StopStartCombustionEngine : CombustionEngine
	{
		
		public StopStartCombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }

		public override bool IgnitionOn { get; set; }

		#region Overrides of CombustionEngine

		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			
			return IgnitionOn ? base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun) : HandleEngineOffRequest(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		#endregion

		protected virtual IResponse HandleEngineOffRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			CurrentState.IgnitionOn = false;
			CurrentState.EngineSpeed = ModelData.IdleSpeed;
			CurrentState.EngineTorque = 0.SI<NewtonMeter>();

			return new ResponseSuccess() {
				EnginePowerRequest = 0.SI<Watt>(),
				DynamicFullLoadPower = 0.SI<Watt>(),
				DragPower = 0.SI<Watt>(),
				AuxiliariesPowerDemand = 0.SI<Watt>(),
				EngineSpeed = 0.RPMtoRad(),
				Source = this
			};
		}

		#region Overrides of CombustionEngine

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			if (IgnitionOn) {
				base.DoWriteModalResults(container);
			} else {
				DoWriteEngineOffResults(container);
			}

		}

		#endregion

		protected virtual void DoWriteEngineOffResults(IModalDataContainer container)
		{
			container[ModalResultField.P_eng_fcmap] = 0.SI<Watt>();
			container[ModalResultField.P_eng_out] = 0.SI<Watt>();
			container[ModalResultField.P_eng_inertia] = 0.SI<Watt>();

			container[ModalResultField.n_eng_avg] = 0.RPMtoRad();
			container[ModalResultField.T_eng_fcmap] = 0.SI<NewtonMeter>();

			container[ModalResultField.P_eng_full] = 0.SI<Watt>();
			container[ModalResultField.P_eng_full_stat] = 0.SI<Watt>();
			container[ModalResultField.P_eng_drag] = 0.SI<Watt>();
			container[ModalResultField.Tq_full] = 0.SI<NewtonMeter>();
			container[ModalResultField.Tq_drag] = 0.SI<NewtonMeter>();
			container[ModalResultField.IgnitionOn] = CurrentState.IgnitionOn;


			var fc = 0.SI<KilogramPerSecond>();
			var fcNCVcorr = fc * ModelData.FuelData.HeatingValueCorrection; // TODO: wird fcNCVcorr

			var fcWHTC = fcNCVcorr * WHTCCorrectionFactor;
			var fcAAUX = fcWHTC;
			var advancedAux = EngineAux as BusAuxiliariesAdapter;
			if (advancedAux != null) {
				throw new VectoException("Engine Stop/Start with advanced auxiliaries not supported!");
				//advancedAux.DoWriteModalResults(container);
				//fcAAUX = advancedAux.AAuxFuelConsumption;
			}
			var fcFinal = fcAAUX;

			container[ModalResultField.FCMap] = fc;
			container[ModalResultField.FCNCVc] = fcNCVcorr;
			container[ModalResultField.FCWHTCc] = fcWHTC;
			container[ModalResultField.FCAAUX] = fcAAUX;
			//container[ModalResultField.FCADAS] = fcADAS;
			container[ModalResultField.FCFinal] = fcFinal;
		}

		
	}
}