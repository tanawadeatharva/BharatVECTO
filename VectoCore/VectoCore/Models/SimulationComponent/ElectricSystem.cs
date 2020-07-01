using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public class ElectricSystem : StatefulVectoSimulationComponent<ElectricSystem.State>, IElectricSystem, IElectricAuxConnecor, IElectricChargerConnector, IBatteryConnector
	{

		protected readonly List<IBatteryAuxPort> Consumers = new List<IBatteryAuxPort>();

		protected IBatteryChargePort Charger;

		protected IBattery Battery;

		public ElectricSystem(IVehicleContainer container) : base(container) { }

		public IElectricSystemResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
		{
			var auxDemand = Consumers.Sum(x => x.PowerDemand(absTime, dt, dryRun)).DefaultIfNull(0);
			var chargePower = Charger == null ? 0.SI<Watt>() : Charger.PowerDemand(absTime, dt, powerDemand, auxDemand, dryRun);
			var totalPowerDemand = powerDemand + chargePower - auxDemand;

			var batResponse = Battery.MainBatteryPort.Request(absTime, dt, totalPowerDemand, dryRun);

			var response = dryRun
				? (AbstractElectricSystemResponse)new ElectricSystemDryRunResponse(this)
				: new ElectricSystemResponseSuccess(this);

			if (batResponse is BatteryOverloadResponse)
			{
				response = new ElectricSystemOverloadResponse(this);
			}
			if (batResponse is BatteryUnderloadResponse)
			{
				response = new ElectricSystemUnderloadResponse(this);
			}

			if (!dryRun)
			{
				CurrentState.SetState(powerDemand, auxDemand, chargePower, batResponse.BatteryPower);
			}

			response.AbsTime = absTime;
			response.SimulationInterval = dt;
			response.BatteryResponse = batResponse;
			response.BatteryPowerDemand = totalPowerDemand;
			response.ConsumerPower = powerDemand;
			response.AuxPower = auxDemand;
			response.ChargingPower = chargePower;
			return response;
		}

		public Watt ElectricAuxPower { get { return PreviousState.AuxPower; } }
		public Watt ChargePower { get { return PreviousState.ChargePower; } }
		public Watt BatteryPower { get { return PreviousState.BatteryPower; } }
		public Watt ConsumerPower { get { return PreviousState.ConsumerPower; } }

		protected override void DoWriteModalResults(Second absTime, Second dt, IModalDataContainer container)
		{
			container[ModalResultField.P_aux_el] = CurrentState.AuxPower;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}


		#region Implementation of IBatteryChargeProvider

		public void Connect(IBatteryChargePort charger)
		{
			Charger = charger;
		}

		#endregion

		#region Implementation of IBatteryAuxOutProvider

		public void Connect(IBatteryAuxPort aux)
		{
			if (Consumers.Contains(aux)) { return; }
			Consumers.Add(aux);
		}

		#endregion

		#region Implementation of IBatteryConnector

		public void Connect(IBattery battery)
		{
			if (Battery != null)
			{
				throw new VectoException("Battery is already connected!");
			}
			Battery = battery;
		}

		#endregion

		#region Implementation of IBatteryInfo

		public Volt InternalCellVoltage
		{
			get { return Battery.InternalCellVoltage; }
		}

		public double StateOfCharge
		{
			get { return Battery.StateOfCharge; }
		}

		public Watt MaxChargePower(Second dt)
		{
			return Battery.MaxChargePower(dt);
		}

		public Watt MaxDischargePower(Second dt)
		{
			return Battery.MaxDischargePower(dt);
		}


		#endregion

		public class State
		{
			public Watt AuxPower = 0.SI<Watt>();
			public Watt ChargePower = 0.SI<Watt>();
			public Watt ConsumerPower = 0.SI<Watt>();
			public Watt BatteryPower = 0.SI<Watt>();

			public void SetState(Watt powerDemand, Watt auxDemand, Watt chargePower, Watt batteryPower)
			{
				AuxPower = auxDemand;
				ChargePower = chargePower;
				ConsumerPower = powerDemand;
				BatteryPower = batteryPower;
			}
		}

	}
}