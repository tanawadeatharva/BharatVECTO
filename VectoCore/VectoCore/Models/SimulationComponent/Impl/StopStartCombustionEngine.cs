using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class StopStartCombustionEngine : CombustionEngine
	{
		protected double EngineStopStartUtilityFactor;
		private WattSecond EngineStartEnergy;

		public StopStartCombustionEngine(
			IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(
			container, modelData, pt1Disabled)
		{
			IgnitionOn = true;
			EngineStopStartUtilityFactor = container.RunData.DriverData.EngineStopStart.UtilityFactor;

			var engineRampUpEnergy = Formulas.InertiaPower(modelData.IdleSpeed, 0.RPMtoRad(), modelData.Inertia, modelData.EngineStartTime) * modelData.EngineStartTime;
			var engineDragEnergy = VectoMath.Abs(modelData.FullLoadCurves[0].DragLoadStationaryTorque(modelData.IdleSpeed)) *
									modelData.IdleSpeed / 2.0 * modelData.EngineStartTime;

			EngineStartEnergy = (engineRampUpEnergy + engineDragEnergy) * EngineStopStartUtilityFactor / DeclarationData.AlternaterEfficiency / DeclarationData.AlternaterEfficiency;
		}

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
			CurrentState.EnginePower = 0.SI<Watt>();
			CurrentState.dt = dt;

			if (!dryRun) {
				//EngineAux.TorqueDemand(absTime, dt, 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>(), ModelData.IdleSpeed);
				CurrentState.AuxPowerEngineOff = EngineAux.PowerDemandEngineOff();
			} else {
				return new ResponseDryRun {
					DeltaFullLoad = 0.SI<Watt>(),
					DeltaDragLoad = 0.SI<Watt>(),
					DeltaEngineSpeed = 0.RPMtoRad(),
					EnginePowerRequest = 0.SI<Watt>(),
					DynamicFullLoadPower = 0.SI<Watt>(),
					DragPower = 0.SI<Watt>(),
					AuxiliariesPowerDemand = 0.SI<Watt>(),
					EngineSpeed = 0.RPMtoRad(),
					Source = this,
				};
			}

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
				var engineStart = !PreviousState.IgnitionOn && CurrentState.IgnitionOn;
				container[ModalResultField.P_ice_start] = engineStart ? EngineStartEnergy / CurrentState.dt : 0.SI<Watt>();
				container[ModalResultField.P_aux_ice_off] = 0.SI<Watt>();
			} else {
				container[ModalResultField.P_ice_start] = 0.SI<Watt>();
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
			container[ModalResultField.P_aux_ice_off] = (CurrentState.AuxPowerEngineOff ?? 0.SI<Watt>()) * EngineStopStartUtilityFactor;


			var auxDemand = EngineAux.PowerDemandEngineOn(ModelData.IdleSpeed) / ModelData.IdleSpeed;

			var pWHRelMap = 0.SI<Watt>();
			var pWHRelCorr = 0.SI<Watt>();

			if (ModelData.WHRData != null) {
				var whrPwr = ModelData.WHRData.WHRMap.GetWHRPower(auxDemand, ModelData.IdleSpeed, DataBus.ExecutionMode != ExecutionMode.Declaration);

				pWHRelMap = whrPwr.ElectricPower * (1 - EngineStopStartUtilityFactor);
				pWHRelCorr = pWHRelMap * ModelData.WHRData.WHRCorrectionFactor;
			}
			container[ModalResultField.P_WHR_el_map] = pWHRelMap;
			container[ModalResultField.P_WHR_el_corr] = pWHRelCorr;

			foreach (var fuel in ModelData.Fuels) {
				var fc = 0.SI<KilogramPerSecond>();
				var fcNCVcorr = fc * fuel.FuelData.HeatingValueCorrection; // TODO: wird fcNCVcorr

				var fcWHTC = fcNCVcorr * WHTCCorrectionFactor(fuel.FuelData);
				var fcAAUX = fcWHTC;
				var advancedAux = EngineAux as BusAuxiliariesAdapter;
				if (advancedAux != null) {
					throw new VectoException("Engine Stop/Start with advanced auxiliaries not supported!");

					//advancedAux.DoWriteModalResults(container);
					//fcAAUX = advancedAux.AAuxFuelConsumption;
				}

				
				var result = fuel.ConsumptionMap.GetFuelConsumption(auxDemand, ModelData.IdleSpeed);

				var fcESS = result.Value * (1 - EngineStopStartUtilityFactor);
				var fcFinal = fcESS;

				container[ModalResultField.FCMap, fuel.FuelData] = fc;
				container[ModalResultField.FCNCVc, fuel.FuelData] = fcNCVcorr;
				container[ModalResultField.FCWHTCc, fuel.FuelData] = fcWHTC;
				container[ModalResultField.FCAAUX, fuel.FuelData] = fcAAUX;
				container[ModalResultField.FCEngineStopStart, fuel.FuelData] = fcESS;
				container[ModalResultField.FCFinal, fuel.FuelData] = fcFinal;
			}
		}

		
	}
}