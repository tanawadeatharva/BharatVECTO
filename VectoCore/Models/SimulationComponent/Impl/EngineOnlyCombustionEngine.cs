/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineOnlyCombustionEngine : CombustionEngine
	{
		protected readonly List<Second> EnginePowerCorrections = new List<Second>();

		public EngineOnlyCombustionEngine(IVehicleContainer cockpit, CombustionEngineData modelData)
			: base(cockpit, modelData) {}

		// the behavior in engine-only mode differs a little bit from normal driving cycle simulation: in engine-only mode
		// certain amount of overload is tolerated.
		protected override IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun)
		{
			CurrentState.dt = dt;
			CurrentState.EngineSpeed = angularVelocity;
			CurrentState.EngineTorqueOut = torque;
//			var requestedEnginePower = ComputeRequestedEnginePower(absTime, dt, torque, angularVelocity);

			var avgEngineSpeed = (PreviousState.EngineSpeed + CurrentState.EngineSpeed) / 2.0;

			var auxTorqueDemand = EngineAux == null
				? 0.SI<NewtonMeter>()
				: EngineAux.PowerDemand(absTime, dt, CurrentState.EngineTorqueOut, angularVelocity, dryRun);

			CurrentState.InertiaTorqueLoss =
				Formulas.InertiaPower(angularVelocity, PreviousState.EngineSpeed, ModelData.Inertia, dt) /
				avgEngineSpeed;
			var totalTorqueDemand = CurrentState.EngineTorqueOut + auxTorqueDemand + CurrentState.InertiaTorqueLoss;
			CurrentState.EngineTorque = totalTorqueDemand;

			CurrentState.FullDragTorque = ModelData.FullLoadCurve.DragLoadStationaryTorque(avgEngineSpeed);
			var dynamicFullLoadPower = ComputeFullLoadPower(angularVelocity, dt);
			CurrentState.DynamicFullLoadTorque = dynamicFullLoadPower / avgEngineSpeed;

			ValidatePowerDemand();

			CurrentState.EngineTorque = LimitEnginePower(CurrentState.EngineTorque, avgEngineSpeed, absTime);

			CurrentState.EnginePower = CurrentState.EngineTorque * avgEngineSpeed;
			if (dryRun) {
				return new ResponseDryRun {
					DeltaFullLoad = CurrentState.EnginePower - CurrentState.DynamicFullLoadTorque * avgEngineSpeed,
					DeltaDragLoad = CurrentState.EnginePower - CurrentState.FullDragTorque * avgEngineSpeed
				};
			}

			UpdateEngineState(CurrentState.EnginePower, avgEngineSpeed);

			// = requestedEnginePower; //todo + _currentState.EnginePowerLoss;
			CurrentState.EngineTorque = CurrentState.EnginePower / CurrentState.EngineSpeed;

			return new ResponseSuccess();
		}

		protected NewtonMeter LimitEnginePower(NewtonMeter requestedEngineTorque, PerSecond avgEngineSpeed, Second AbsTime)
		{
			if (requestedEngineTorque > CurrentState.DynamicFullLoadTorque) {
				if (requestedEngineTorque / CurrentState.DynamicFullLoadTorque > MaxTorqueExceededThreshold) {
					EnginePowerCorrections.Add(AbsTime);
					Log.Warn("t: {0}  requested power > P_engine_full * 1.05 - corrected. P_request: {1}  P_engine_full: {2}",
						AbsTime, requestedEngineTorque * avgEngineSpeed, CurrentState.DynamicFullLoadTorque * avgEngineSpeed);
				}
				return CurrentState.DynamicFullLoadTorque;
			}
			if (requestedEngineTorque < CurrentState.FullDragTorque) {
				if (requestedEngineTorque / CurrentState.FullDragTorque > MaxTorqueExceededThreshold &&
					requestedEngineTorque > -99999) {
					EnginePowerCorrections.Add(AbsTime);
					Log.Warn("t: {0}  requested power < P_engine_drag * 1.05 - corrected. P_request: {1}  P_engine_drag: {2}",
						AbsTime, requestedEngineTorque * avgEngineSpeed, CurrentState.FullDragTorque * avgEngineSpeed);
				}
				return CurrentState.FullDragTorque;
			}
			return requestedEngineTorque;
		}

		public IList<string> Warnings()
		{
			IList<string> retVal = new List<string>();
			retVal.Add(string.Format("Engine power corrected (>5%) in {0} time steps ", EnginePowerCorrections.Count));
			return retVal;
		}
	}
}