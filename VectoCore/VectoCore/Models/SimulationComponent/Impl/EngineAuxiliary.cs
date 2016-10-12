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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineAuxiliary : StatefulVectoSimulationComponent<EngineAuxiliary.State>, IAuxInProvider,
		IAuxPort
	{
		protected readonly Dictionary<string, Func<PerSecond, Watt>> _auxiliaries =
			new Dictionary<string, Func<PerSecond, Watt>>();

		public EngineAuxiliary(IVehicleContainer container) : base(container) {}

		public IAuxPort Port()
		{
			return this;
		}

		public void AddConstant(string auxId, Watt powerDemand)
		{
			Add(auxId, _ => powerDemand);
		}

		public void AddCycle(string auxId)
		{
			Add(auxId, _ => DataBus.CycleData.LeftSample.AdditionalAuxPowerDemand);
		}

		public void AddMapping(string auxId, AuxiliaryData data)
		{
			if (!DataBus.CycleData.LeftSample.AuxiliarySupplyPower.ContainsKey(auxId)) {
				var error = string.Format("driving cycle does not contain column for auxiliary: {0}",
					Constants.Auxiliaries.Prefix + auxId);
				Log.Error(error);
				throw new VectoException(error);
			}

			Add(auxId, speed => {
				var powerSupply = DataBus.CycleData.LeftSample.AuxiliarySupplyPower[auxId];
				var nAuxiliary = speed * data.TransmissionRatio;
				var powerAuxOut = powerSupply / data.EfficiencyToSupply;
				var powerAuxIn = data.GetPowerDemand(nAuxiliary, powerAuxOut);
				return powerAuxIn / data.EfficiencyToEngine;
			});
		}

		public void Add(string auxId, Func<PerSecond, Watt> powerLossFunction)
		{
			_auxiliaries[auxId] = powerLossFunction;
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			PreviousState.AngularSpeed = angularSpeed;
			return ComputePowerDemand(angularSpeed) / angularSpeed;
		}

		public NewtonMeter TorqueDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			var avgAngularSpeed = (PreviousState.AngularSpeed != null)
				? (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0
				: CurrentState.AngularSpeed;
			if (avgAngularSpeed.IsGreater(0))
				return ComputePowerDemand(avgAngularSpeed) / avgAngularSpeed;
			return 0.SI<NewtonMeter>();
		}

		protected Watt ComputePowerDemand(PerSecond engineSpeed)
		{
			CurrentState.PowerDemands = new Dictionary<string, Watt>(_auxiliaries.Count);
			foreach (var item in _auxiliaries) {
				var value = item.Value(engineSpeed);
				if (value != null) {
					CurrentState.PowerDemands[item.Key] = value;
				}
			}
			return CurrentState.PowerDemands.Sum(kv => kv.Value);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var auxPowerDemand = 0.SI<Watt>();
			if (CurrentState.PowerDemands != null) {
				foreach (var kv in CurrentState.PowerDemands) {
					container[kv.Key] = kv.Value;
					// mk 2016-10-11: pto's should not be counted in sum auxiliary power demand
					if (kv.Key != Constants.Auxiliaries.IDs.PTOTransmission && kv.Key != Constants.Auxiliaries.IDs.PTOConsumer) {
						auxPowerDemand += kv.Value;
					}
				}
			}
			if (container[ModalResultField.P_aux] == null || container[ModalResultField.P_aux] == DBNull.Value) {
				// only overwrite if nobody else already wrote the total aux power

				container[ModalResultField.P_aux] = auxPowerDemand;
			}
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		public class State
		{
			public PerSecond AngularSpeed;
			public Dictionary<string, Watt> PowerDemands;
		}
	}
}