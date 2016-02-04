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

using System;
using System.Collections.Generic;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Auxiliary : VectoSimulationComponent, IAuxiliary, ITnInPort, ITnOutPort
	{
		public const string DirectAuxiliaryId = "";

		private readonly Dictionary<string, Func<PerSecond, Watt>> _auxDict = new Dictionary<string, Func<PerSecond, Watt>>();
		private readonly Dictionary<string, Watt> _powerDemands = new Dictionary<string, Watt>();

		protected ITnOutPort NextComponent;

		public Auxiliary(IVehicleContainer container) : base(container) {}

		#region ITnInProvider

		public ITnInPort InPort()
		{
			return this;
		}

		#endregion

		#region ITnOutProvider

		public ITnOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region ITnInPort

		void ITnInPort.Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		#endregion

		#region ITnOutPort

		IResponse ITnOutPort.Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun)
		{
			var currentAngularVelocity = angularVelocity ?? DataBus.EngineSpeed;
			var powerDemand = ComputePowerDemand(currentAngularVelocity);

			var retVal = NextComponent.Request(absTime, dt, torque + powerDemand / currentAngularVelocity, angularVelocity,
				dryRun);
			retVal.AuxiliariesPowerDemand = powerDemand;
			return retVal;
		}

		private Watt ComputePowerDemand(PerSecond engineSpeed)
		{
			_powerDemands.Clear();
			var powerDemand = 0.SI<Watt>();

			foreach (var kv in _auxDict) {
				var demand = kv.Value(engineSpeed);
				powerDemand += demand;
				_powerDemands[kv.Key] = demand;
			}
			return powerDemand;
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			var powerDemand = ComputePowerDemand(angularSpeed);
			return NextComponent.Initialize(torque + powerDemand / angularSpeed, angularSpeed);
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var sum = 0.SI<Watt>();
			foreach (var kv in _powerDemands) {
				sum += kv.Value;
				// todo: aux write directauxiliary somewhere to moddata .... probably Padd column??
				if (!string.IsNullOrWhiteSpace(kv.Key)) {
					container[kv.Key] = kv.Value;
				}
			}
			container[ModalResultField.Paux] = sum;
		}

		protected override void DoCommitSimulationStep() {}

		#endregion

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
	}
}