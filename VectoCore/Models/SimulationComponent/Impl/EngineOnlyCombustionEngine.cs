/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
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

		public EngineOnlyCombustionEngine(IVehicleContainer cockpit, CombustionEngineData data) : base(cockpit, data) {}

		// the behavior in engine-only mode differs a little bit from normal driving cycle simulation: in engine-only mode
		// certain amount of overload is tolerated.
		protected override IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun)
		{
			var requestedEnginePower = ComputeRequestedEnginePower(absTime, dt, torque, angularVelocity);

			ComputeFullLoadPower(angularVelocity, dt);

			ValidatePowerDemand(requestedEnginePower);

			CurrentState.EnginePower = LimitEnginePower(requestedEnginePower);

			if (dryRun) {
				return new ResponseDryRun {
					DeltaFullLoad = requestedEnginePower - CurrentState.DynamicFullLoadPower,
					DeltaDragLoad = requestedEnginePower - CurrentState.FullDragPower
				};
			}

			UpdateEngineState(CurrentState.EnginePower);

			// = requestedEnginePower; //todo + _currentState.EnginePowerLoss;
			CurrentState.EngineTorque = CurrentState.EnginePower / CurrentState.EngineSpeed;

			return new ResponseSuccess();
		}

		protected override Watt LimitEnginePower(Watt requestedEnginePower)
		{
			if (requestedEnginePower > CurrentState.DynamicFullLoadPower) {
				if (requestedEnginePower / CurrentState.DynamicFullLoadPower > MaxPowerExceededThreshold) {
					EnginePowerCorrections.Add(CurrentState.AbsTime);
					Log.Warn(
						"t: {0}  requested power > P_engine_full * 1.05 - corrected. P_request: {1}  P_engine_full: {2}",
						CurrentState.AbsTime, requestedEnginePower, CurrentState.DynamicFullLoadPower);
				}
				return CurrentState.DynamicFullLoadPower;
			}
			if (requestedEnginePower < CurrentState.FullDragPower) {
				if (requestedEnginePower / CurrentState.FullDragPower > MaxPowerExceededThreshold &&
					requestedEnginePower > -99999) {
					EnginePowerCorrections.Add(CurrentState.AbsTime);
					Log.Warn("t: {0}  requested power < P_engine_drag * 1.05 - corrected. P_request: {1}  P_engine_drag: {2}",
						CurrentState.AbsTime, requestedEnginePower, CurrentState.FullDragPower);
				}
				return CurrentState.FullDragPower;
			}
			return requestedEnginePower;
		}

		public IList<string> Warnings()
		{
			IList<string> retVal = new List<string>();
			retVal.Add(string.Format("Engine power corrected (>5%) in {0} time steps ", EnginePowerCorrections.Count));
			return retVal;
		}
	}
}