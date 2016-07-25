/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Component for a combustion engine.
	/// </summary>
	public class CombustionEngine : StatefulVectoSimulationComponent<CombustionEngine.EngineState>, ICombustionEngine,
		ITnOutPort
	{
		public bool PT1Disabled { get; set; }

		public enum EngineOperationMode
		{
			Idle,
			Drag,
			FullDrag,
			Load,
			FullLoad,
			Stopped,
			Undef
		}

		protected const int EngineIdleSpeedStopThreshold = 100;
		protected const double MaxTorqueExceededThreshold = 1.05;
		protected const double ZeroThreshold = 0.0001;
		protected const double FullLoadMargin = 0.01;

		protected readonly Watt StationaryIdleFullLoadPower;

		internal readonly CombustionEngineData ModelData;

		protected IEngineAuxPort EngineAux;

		public CombustionEngine(IVehicleContainer cockpit, CombustionEngineData modelData, bool pt1Disabled = false)
			: base(cockpit)
		{
			PT1Disabled = pt1Disabled;
			ModelData = modelData;

			PreviousState.OperationMode = EngineOperationMode.Idle;
			//PreviousState.EnginePower = 0.SI<Watt>();
			PreviousState.EngineSpeed = ModelData.IdleSpeed;
			PreviousState.dt = 1.SI<Second>();

			StationaryIdleFullLoadPower = ModelData.FullLoadCurve.FullLoadStationaryTorque(ModelData.IdleSpeed) *
										ModelData.IdleSpeed;
		}

		#region IEngineCockpit

		public PerSecond EngineSpeed
		{
			get { return PreviousState.EngineSpeed; }
		}

		public NewtonMeter EngineTorque
		{
			get { return PreviousState.EngineTorque; }
		}

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return ModelData.FullLoadCurve.FullLoadStationaryTorque(angularSpeed) * angularSpeed;
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return ModelData.FullLoadCurve.DragLoadStationaryPower(angularSpeed);
		}

		public PerSecond EngineIdleSpeed
		{
			get { return ModelData.IdleSpeed; }
		}

		public PerSecond EngineRatedSpeed
		{
			get { return ModelData.FullLoadCurve.RatedSpeed; }
		}

		public PerSecond EngineN95hSpeed
		{
			get { return ModelData.FullLoadCurve.N95hSpeed; }
		}

		public ICombustionEngineIdleController IdleController
		{
			get { return EngineIdleController ?? (EngineIdleController = new CombustionEngineIdleController(this)); }
		}

		protected CombustionEngineIdleController EngineIdleController { get; set; }

		#endregion

		#region ITnOutProvider

		public ITnOutPort OutPort()
		{
			return this;
		}

		#endregion

		public void Connect(IEngineAuxPort aux)
		{
			EngineAux = aux;
		}

		#region ITnOutPort

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("Engine Powertrain Power Request: torque: {0}, angularVelocity: {1}, power: {2}", torque, angularVelocity,
				torque * angularVelocity);

			return DoHandleRequest(absTime, dt, torque, angularVelocity, dryRun);
		}

		protected virtual IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torqueOut,
			PerSecond angularVelocity, bool dryRun)
		{
			if (angularVelocity == null) {
				// if the clutch disengages the idle controller should take over!
				throw new VectoSimulationException("angular velocity is null! Clutch open without IdleController?");
			}
			if (angularVelocity < ModelData.IdleSpeed.Value() - EngineIdleSpeedStopThreshold) {
				CurrentState.OperationMode = EngineOperationMode.Stopped;
			}

			CurrentState.dt = dt;
			CurrentState.EngineSpeed = angularVelocity;
			var avgEngineSpeed = (PreviousState.EngineSpeed + CurrentState.EngineSpeed) / 2.0;
			CurrentState.EngineTorqueOut = torqueOut;
			CurrentState.FullDragTorque = ModelData.FullLoadCurve.DragLoadStationaryTorque(avgEngineSpeed);
			var dynamicFullLoadPower = ComputeFullLoadPower(avgEngineSpeed, dt);
			CurrentState.DynamicFullLoadTorque = dynamicFullLoadPower / avgEngineSpeed;
			CurrentState.InertiaTorqueLoss =
				Formulas.InertiaPower(angularVelocity, PreviousState.EngineSpeed, ModelData.Inertia, dt) /
				avgEngineSpeed;

			var auxTorqueDemand = EngineAux == null
				? 0.SI<NewtonMeter>()
				: EngineAux.PowerDemand(absTime, dt, CurrentState.EngineTorqueOut,
					CurrentState.EngineTorqueOut + CurrentState.InertiaTorqueLoss, angularVelocity, dryRun);
			// compute the torque the engine has to provide. powertrain + aux + its own inertia
			var totalTorqueDemand = torqueOut + auxTorqueDemand + CurrentState.InertiaTorqueLoss;

			Log.Debug("EngineInertiaTorque: {0}", CurrentState.InertiaTorqueLoss);
			Log.Debug("Drag Curve: torque: {0}, power: {1}", CurrentState.FullDragTorque,
				CurrentState.FullDragTorque * avgEngineSpeed);

			Log.Debug("Dynamic FullLoad: torque: {0}, power: {1}", CurrentState.DynamicFullLoadTorque, dynamicFullLoadPower);

			ValidatePowerDemand(totalTorqueDemand); // requires CurrentState.FullDragTorque and DynamicfullLoad to be set!

			// get max. torque as limited by gearbox. gearbox only limits torqueOut!
			NewtonMeter gearboxFullLoad = null;
			var curve = DataBus.GearFullLoadCurve;
			if (curve != null) {
				// if the current gear has a full-load curve, limit the max. torque to the 
				// gbx. full-load and continue (remmber the delta for further below)
				gearboxFullLoad = curve.FullLoadStationaryTorque(avgEngineSpeed);
			}

			var deltaFull = ComputeDelta(torqueOut, totalTorqueDemand, CurrentState.DynamicFullLoadTorque, gearboxFullLoad, true);
			var deltaDrag = ComputeDelta(torqueOut, totalTorqueDemand, CurrentState.FullDragTorque,
				gearboxFullLoad != null ? -gearboxFullLoad : null, false);

			if (dryRun) {
				return new ResponseDryRun {
					DeltaFullLoad = deltaFull * avgEngineSpeed,
					DeltaDragLoad = deltaDrag * avgEngineSpeed,
					EnginePowerRequest = torqueOut * avgEngineSpeed,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
					Source = this,
				};
			}

			if (
				(deltaFull * avgEngineSpeed).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				//throw new VectoSimulationException(
				Log.Error(
					"Unexpected condition: requested torque_out is above gearbox full-load and engine is below drag load! deltaFull: {0}, deltaDrag: {1}",
					deltaFull, deltaDrag);
			}

			var minTorque = CurrentState.FullDragTorque;
			var maxTorque = CurrentState.DynamicFullLoadTorque;
			if (gearboxFullLoad != null) {
				minTorque = VectoMath.Max(minTorque, -gearboxFullLoad);
				maxTorque = VectoMath.Min(maxTorque, gearboxFullLoad);
			}

			CurrentState.EngineTorque = VectoMath.Limit(totalTorqueDemand, minTorque, maxTorque);
			CurrentState.EnginePower = CurrentState.EngineTorque * avgEngineSpeed;

			if (totalTorqueDemand.IsGreater(0.SI<NewtonMeter>()) &&
				(deltaFull * avgEngineSpeed).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power exceeds fullload power: delta: {0}", deltaFull);
				return new ResponseOverload {
					AbsTime = absTime,
					Delta = deltaFull * avgEngineSpeed,
					EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
					Source = this,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
				};
			}

			if (totalTorqueDemand.IsSmaller(0.SI<NewtonMeter>()) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power is below drag power: delta: {0}", deltaDrag);
				return new ResponseUnderload {
					AbsTime = absTime,
					Delta = deltaDrag * avgEngineSpeed,
					EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
					Source = this,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
				};
			}

			UpdateEngineState(CurrentState.EnginePower, avgEngineSpeed);

			return new ResponseSuccess {
				EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
				AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
				EngineSpeed = angularVelocity,
				Source = this
			};
		}

		private NewtonMeter ComputeDelta(NewtonMeter torqueOut, NewtonMeter totalTorqueDemand, NewtonMeter maxEngineTorque,
			NewtonMeter maxGbxtorque, bool motoring)
		{
			var deltaGbx = maxGbxtorque != null ? torqueOut - maxGbxtorque : null;
			var deltaEngine = totalTorqueDemand - maxEngineTorque;

			if (deltaGbx == null) {
				return deltaEngine;
			}
			return motoring ? VectoMath.Max(deltaGbx, deltaEngine) : VectoMath.Min(deltaGbx, deltaEngine);
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			var auxDemand = EngineAux == null ? 0.SI<NewtonMeter>() : EngineAux.Initialize(torque, angularSpeed);
			PreviousState = new EngineState {
				EngineSpeed = angularSpeed,
				dt = 1.SI<Second>(),
				InertiaTorqueLoss = 0.SI<NewtonMeter>(),
				StationaryFullLoadTorque = ModelData.FullLoadCurve.FullLoadStationaryTorque(angularSpeed),
				FullDragTorque = ModelData.FullLoadCurve.DragLoadStationaryTorque(angularSpeed),
				EngineTorque = torque + auxDemand,
				EnginePower = (torque + auxDemand) * angularSpeed,
			};
			PreviousState.DynamicFullLoadTorque = PreviousState.StationaryFullLoadTorque;

			return new ResponseSuccess { Source = this, EnginePowerRequest = PreviousState.EnginePower };
		}

		/// <summary>
		/// Validates the requested power demand [W].
		/// </summary>
		protected virtual void ValidatePowerDemand(NewtonMeter torqueDemand)
		{
			if (CurrentState.FullDragTorque >= 0 && torqueDemand < 0) {
				throw new VectoSimulationException("P_engine_drag > 0! Tq_drag: {1}, Tq_eng: {2},  n_eng_avg: {0} [1/min] ",
					CurrentState.EngineSpeed.Value().RadToRPM(), CurrentState.FullDragTorque, CurrentState.EngineTorque);
			}

			if (CurrentState.DynamicFullLoadTorque <= 0 && torqueDemand > 0) {
				throw new VectoSimulationException("P_engine_full < 0! Tq_drag: {1}, Tq_eng: {2},  n_eng_avg: {0} [1/min] ",
					CurrentState.EngineSpeed.Value().RadToRPM(), CurrentState.FullDragTorque, CurrentState.EngineTorque);
			}
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgEngineSpeed = (PreviousState.EngineSpeed + CurrentState.EngineSpeed) / 2.0;

			container[ModalResultField.P_eng_fcmap] = CurrentState.EngineTorque * avgEngineSpeed;
			container[ModalResultField.P_eng_out] = CurrentState.EngineTorqueOut * avgEngineSpeed;
			container[ModalResultField.P_eng_inertia] = CurrentState.InertiaTorqueLoss * avgEngineSpeed;

			container[ModalResultField.n_eng_avg] = avgEngineSpeed;
			container[ModalResultField.T_eng_fcmap] = CurrentState.EngineTorque;

			container[ModalResultField.P_eng_full] = CurrentState.DynamicFullLoadTorque * avgEngineSpeed;
			container[ModalResultField.P_eng_drag] = CurrentState.FullDragTorque * avgEngineSpeed;
			container[ModalResultField.Tq_full] = CurrentState.DynamicFullLoadTorque;
			container[ModalResultField.Tq_drag] = CurrentState.FullDragTorque;

			var fc = ModelData.ConsumptionMap.GetFuelConsumption(CurrentState.EngineTorque, avgEngineSpeed);

			//TODO mk-2015-11-11: calculate aux start stop correction
			var fcAux = fc;

			var fcWHTC = fcAux * ModelData.WHTCCorrectionFactor;
			var fcAAUX = fcWHTC;
			var advancedAux = EngineAux as BusAuxiliariesAdapter;
			if (advancedAux != null) {
				advancedAux.DoWriteModalResults(container);
				fcAAUX = advancedAux.AAuxFuelConsumption;
			}
			var fcFinal = fcAAUX;

			container[ModalResultField.FCMap] = fc;
			container[ModalResultField.FCAUXc] = fcAux;
			container[ModalResultField.FCWHTCc] = fcWHTC;
			container[ModalResultField.FCAAUX] = fcAAUX;
			container[ModalResultField.FCFinal] = fcFinal;
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
			var advancedAux = EngineAux as BusAuxiliariesAdapter;
			if (advancedAux != null) {
				advancedAux.DoCommitSimulationStep();
			}
		}

		#endregion

		/// <summary>
		/// Updates the engine state dependend on the requested power [W].
		/// </summary>
		protected virtual void UpdateEngineState(Watt requestedEnginePower, PerSecond avgEngineSpeed)
		{
			if (requestedEnginePower < -ZeroThreshold) {
				CurrentState.OperationMode = IsFullLoad(requestedEnginePower, CurrentState.DynamicFullLoadTorque * avgEngineSpeed)
					? EngineOperationMode.FullLoad
					: EngineOperationMode.Load;
			} else if (requestedEnginePower > ZeroThreshold) {
				CurrentState.OperationMode = IsFullLoad(requestedEnginePower, CurrentState.FullDragTorque * avgEngineSpeed)
					? EngineOperationMode.FullDrag
					: EngineOperationMode.Drag;
			} else {
				// -ZeroThreshold <= requestedEnginePower <= ZeroThreshold
				CurrentState.OperationMode = EngineOperationMode.Idle;
			}
		}

		/// <summary>
		///     computes full load power from gear [-], angularVelocity [rad/s] and dt [s].
		/// </summary>
		protected Watt ComputeFullLoadPower(PerSecond angularVelocity, Second dt)
		{
			if (dt <= 0) {
				throw new VectoException("ComputeFullLoadPower cannot compute for simulation interval length 0.");
			}

			CurrentState.StationaryFullLoadTorque = ModelData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity);
			var stationaryFullLoadPower = CurrentState.StationaryFullLoadTorque * angularVelocity;
			Watt dynFullPowerCalculated;

			// disable pt1 behaviour if PT1Disabled is true, or if the previous enginepower is greater than the current stationary fullload power (in this case the pt1 calculation fails)
			if (PT1Disabled || PreviousState.EnginePower.IsGreaterOrEqual(stationaryFullLoadPower)) {
				dynFullPowerCalculated = stationaryFullLoadPower;
			} else {
				var pt1 = ModelData.FullLoadCurve.PT1(angularVelocity).Value();
				var powerRatio = (PreviousState.EnginePower / stationaryFullLoadPower).Value();
				var tStarPrev = pt1 * Math.Log(1.0 / (1 - powerRatio), Math.E).SI<Second>();
				var tStar = tStarPrev + PreviousState.dt;
				dynFullPowerCalculated = stationaryFullLoadPower * (1 - Math.Exp((-tStar / pt1).Value()));
			}

			// new check in vecto 3.x (according to Martin Rexeis)
			if (dynFullPowerCalculated < StationaryIdleFullLoadPower) {
				dynFullPowerCalculated = StationaryIdleFullLoadPower;
			}

			return dynFullPowerCalculated;
		}

		protected bool IsFullLoad(Watt requestedPower, Watt maxPower)
		{
			var testValue = requestedPower / maxPower - 1.0;
			return testValue.Abs() < FullLoadMargin;
		}

		public class EngineState
		{
			public EngineOperationMode OperationMode { get; set; }

			//public Second AbsTime { get; set; }

			// ReSharper disable once InconsistentNaming
			public Second dt { get; set; }

			public PerSecond EngineSpeed { get; set; }

			public NewtonMeter EngineTorque { get; set; }

			public NewtonMeter EngineTorqueOut { get; set; }

			public Watt EnginePower { get; set; }

			public NewtonMeter InertiaTorqueLoss { get; set; }

			// public Watt EnginePowerLoss { get; set; }

			//public Watt StationaryFullLoadPower { get; set; }

			//public Watt DynamicFullLoadPower { get; set; }

			public NewtonMeter StationaryFullLoadTorque { get; set; }

			public NewtonMeter DynamicFullLoadTorque { get; set; }

			//public Watt FullDragPower { get; set; }

			public NewtonMeter FullDragTorque { get; set; }

			// ReSharper disable once InconsistentNaming
		}

		protected class CombustionEngineIdleController : LoggingObject, ICombustionEngineIdleController
		{
			protected readonly double PeDropSlope = -0.75;
			protected readonly double PeDropOffset = 1.0;

			protected CombustionEngine Engine;

			protected Second IdleStart;
			protected Watt LastEnginePower;

			public CombustionEngineIdleController(CombustionEngine combustionEngine)
			{
				Engine = combustionEngine;
			}

			public ITnOutPort RequestPort { private get; set; }

			public void Reset()
			{
				IdleStart = null;
			}

			public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
				bool dryRun = false)
			{
				if (angularVelocity != null) {
					throw new VectoException("IdleController can only handle idle requests, i.e. angularVelocity == null!");
				}
				if (!torque.IsEqual(0)) {
					throw new VectoException("Torque has to be 0 for idle requests!");
				}
				if (IdleStart == null) {
					IdleStart = absTime;
					LastEnginePower = Engine.PreviousState.EnginePower;
				}
				IResponse retVal;

				var idleTime = absTime - IdleStart + dt;
				var prevEngineSpeed = Engine.PreviousState.EngineSpeed;
				var dragLoad = Engine.ModelData.FullLoadCurve.DragLoadStationaryPower(prevEngineSpeed);

				var nextEnginePower = (LastEnginePower - dragLoad) * VectoMath.Max(idleTime.Value() * PeDropSlope + PeDropOffset, 0) +
									dragLoad;

				var auxDemandResponse = RequestPort.Request(absTime, dt, torque, prevEngineSpeed, true);

				var deltaEnginePower = nextEnginePower - (auxDemandResponse.AuxiliariesPowerDemand ?? 0.SI<Watt>());
				var deltaTorque = deltaEnginePower / prevEngineSpeed;
				var deltaAngularSpeed = deltaTorque / Engine.ModelData.Inertia * dt;

				var nextAngularSpeed = prevEngineSpeed;
				if (deltaAngularSpeed > 0) {
					retVal = RequestPort.Request(absTime, dt, torque, nextAngularSpeed);
					return retVal;
				}

				nextAngularSpeed = prevEngineSpeed + deltaAngularSpeed;
				if (nextAngularSpeed < Engine.ModelData.IdleSpeed) {
					nextAngularSpeed = Engine.ModelData.IdleSpeed;
				}

				retVal = RequestPort.Request(absTime, dt, torque, nextAngularSpeed);
				retVal.Switch().
					Case<ResponseSuccess>().
					Case<ResponseUnderload>(r => {
						var angularSpeed = SearchAlgorithm.Search(nextAngularSpeed, r.Delta,
							Constants.SimulationSettings.EngineIdlingSearchInterval,
							getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
							evaluateFunction: n => RequestPort.Request(absTime, dt, torque, n, true),
							criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value());
						Log.Debug("Found operating point for idling. absTime: {0}, dt: {1}, torque: {2}, angularSpeed: {3}", absTime, dt,
							torque, angularSpeed);
						retVal = RequestPort.Request(absTime, dt, torque, angularSpeed);
					}).
					Default(r => { throw new UnexpectedResponseException("searching Idling point", r); });

				return retVal;
			}

			public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
			{
				return new ResponseSuccess() { Source = this };
			}
		}
	}
}