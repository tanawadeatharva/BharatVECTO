using System;
using System.Collections.Generic;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineAuxiliary : StatefulVectoSimulationComponent<EngineAuxiliary.EngineAuxState>, IEngineAuxInProvider,
		IEngineAuxPort
	{
		public const string DirectAuxiliaryId = "";

		private readonly Dictionary<string, Func<PerSecond, Watt>> _auxDict = new Dictionary<string, Func<PerSecond, Watt>>();
		private readonly Dictionary<string, Watt> _powerDemands = new Dictionary<string, Watt>();

		public EngineAuxiliary(IVehicleContainer container) : base(container) {}

		public IEngineAuxPort Port()
		{
			return this;
		}

		public NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torque, PerSecond angularSpeed,
			bool dryRun = false)
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
			_powerDemands.Clear();
			var powerDemand = 0.SI<Watt>();

			foreach (var kv in _auxDict) {
				var demand = kv.Value(engineSpeed);
				powerDemand += demand;
				_powerDemands[kv.Key] = demand;
			}
			return powerDemand;
		}

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
			container[ModalResultField.P_aux] = sum;
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