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
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineAuxiliary : StatefulVectoSimulationComponent<EngineAuxiliary.EngineAuxState>, IEngineAuxInProvider,
		IEngineAuxPort
	{
		public const string DirectAuxiliaryId = "";

		private readonly Dictionary<string, Func<PerSecond, Watt>> _auxDict = new Dictionary<string, Func<PerSecond, Watt>>();
		private Dictionary<string, Watt> _powerDemands = new Dictionary<string, Watt>();

		public EngineAuxiliary(IVehicleContainer container) : base(container) {}

		public IEngineAuxPort Port()
		{
			return this;
		}

		public NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			return ComputePowerDemand(avgAngularSpeed) / avgAngularSpeed;
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			PreviousState.AngularSpeed = angularSpeed;
			return ComputePowerDemand(angularSpeed) / angularSpeed;
		}

		private Watt ComputePowerDemand(PerSecond engineSpeed)
		{
			_powerDemands = _auxDict.ToDictionary(kv => kv.Key, kv => kv.Value(engineSpeed));
			return _powerDemands.Values.Sum(p => p);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			foreach (var kv in _powerDemands.Where(kv => !string.IsNullOrWhiteSpace(kv.Key))) {
				container[kv.Key] = kv.Value;
			}
			container[ModalResultField.P_aux] = _powerDemands.Values.Sum(p => p);
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		public void AddConstant(string auxId, Watt powerDemand)
		{
			_auxDict[auxId] = speed => powerDemand;
		}

		public void AddDirect()
		{
			_auxDict[DirectAuxiliaryId] = speed => DataBus.CycleData.LeftSample.AdditionalAuxPowerDemand;
		}

		public void AddMapping(string auxId, AuxiliaryData data)
		{
			if (!DataBus.CycleData.LeftSample.AuxiliarySupplyPower.ContainsKey("Aux_" + auxId)) {
				var error = string.Format("driving cycle does not contain column for auxiliary: {0}", auxId);
				Log.Error(error);
				throw new VectoException(error);
			}

			_auxDict[auxId] = speed => {
				var powerSupply = DataBus.CycleData.LeftSample.AuxiliarySupplyPower["Aux_" + auxId];
				var nAuxiliary = speed * data.TransmissionRatio;
				var powerAuxOut = powerSupply / data.EfficiencyToSupply;
				var powerAuxIn = data.GetPowerDemand(nAuxiliary, powerAuxOut);
				return powerAuxIn / data.EfficiencyToEngine;
			};
		}

		public class EngineAuxState
		{
			public PerSecond AngularSpeed;
		}
	}
}