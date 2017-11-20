using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class VTPCombustionEngine : CombustionEngine
    {
        public VTPCombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }

		protected override IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torqueReq,
			PerSecond angularVelocity, bool dryRun)
		{
			var powerDemand = angularVelocity * torqueReq;

			var avgEngineSpeed = GetEngineSpeed(angularVelocity);
			var torqueOut = powerDemand / avgEngineSpeed;


			var fullDragTorque = ModelData.FullLoadCurves[DataBus.Gear].DragLoadStationaryTorque(avgEngineSpeed);
			var fullLoadTorque = ModelData.FullLoadCurves[DataBus.Gear].FullLoadStationaryTorque(angularVelocity);
			
			var inertiaTorqueLoss =
				Formulas.InertiaPower(angularVelocity, PreviousState.EngineSpeed, ModelData.Inertia, dt) /
				avgEngineSpeed;

			var auxTorqueDemand = EngineAux == null
				? 0.SI<NewtonMeter>()
				: EngineAux.TorqueDemand(absTime, dt, torqueOut,
					torqueOut + inertiaTorqueLoss, angularVelocity, dryRun);
			// compute the torque the engine has to provide. powertrain + aux + its own inertia
			var totalTorqueDemand = torqueOut + auxTorqueDemand + inertiaTorqueLoss;

			Log.Debug("EngineInertiaTorque: {0}", inertiaTorqueLoss);
			Log.Debug("Drag Curve: torque: {0}, power: {1}", fullDragTorque, fullDragTorque * avgEngineSpeed);

			var deltaFull = totalTorqueDemand - fullLoadTorque;
			var deltaDrag = totalTorqueDemand - fullDragTorque;

			if (dryRun) {
				return new ResponseDryRun {
					DeltaFullLoad = deltaFull * avgEngineSpeed,
					DeltaDragLoad = deltaDrag * avgEngineSpeed,
					DeltaEngineSpeed = 0.RPMtoRad(),
					EnginePowerRequest = torqueOut * avgEngineSpeed,
					DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
					DragPower = fullDragTorque * avgEngineSpeed,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
					Source = this,
				};
			}
			CurrentState.dt = dt;
			CurrentState.EngineSpeed = angularVelocity;
			CurrentState.EngineTorqueOut = torqueOut;
			CurrentState.FullDragTorque = fullDragTorque;
			CurrentState.DynamicFullLoadTorque = fullLoadTorque;
			CurrentState.StationaryFullLoadTorque = fullLoadTorque;
			CurrentState.InertiaTorqueLoss = inertiaTorqueLoss;

			if ((deltaFull * avgEngineSpeed).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				//throw new VectoSimulationException(
				Log.Error(
					"Unexpected condition: requested torque_out is above gearbox full-load and engine is below drag load! deltaFull: {0}, deltaDrag: {1}",
					deltaFull, deltaDrag);
			}

			var minTorque = CurrentState.FullDragTorque;
			var maxTorque = CurrentState.DynamicFullLoadTorque;

			CurrentState.EngineTorque = totalTorqueDemand.LimitTo(minTorque, maxTorque);
			CurrentState.EnginePower = CurrentState.EngineTorque * avgEngineSpeed;

			if (totalTorqueDemand.IsGreater(0) &&
				(deltaFull * avgEngineSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power exceeds fullload power: delta: {0}", deltaFull);
				return new ResponseOverload {
					AbsTime = absTime,
					Delta = deltaFull * avgEngineSpeed,
					EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
					DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
					DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
					Source = this,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
				};
			}

			if (totalTorqueDemand.IsSmaller(0) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0, Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power is below drag power: delta: {0}", deltaDrag);
				return new ResponseUnderload {
					AbsTime = absTime,
					Delta = deltaDrag * avgEngineSpeed,
					EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
					DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
					DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
					Source = this,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
				};
			}

			//UpdateEngineState(CurrentState.EnginePower, avgEngineSpeed);

			return new ResponseSuccess {
				EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
				DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
				DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
				AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
				EngineSpeed = angularVelocity,
				Source = this
			};
		}


		protected override PerSecond GetEngineSpeed(PerSecond angularSpeed)
        {
            return DataBus.CycleData.LeftSample.EngineSpeed;
        }
    }
}