using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Battery : StatefulVectoSimulationComponent<Battery.State>, IBattery, IBatteryPort
	{
		protected readonly BatteryData ModelData;

		public Battery(IVehicleContainer container, BatteryData modelData) : base(container)
		{
			ModelData = modelData;
		}

		#region Implementation of IBatteryProvider
		public IBatteryPort MainBatteryPort
		{
			get { return this; }
		}

		#endregion


		#region Implementation of IBatteryPort

		public void Initialize(double initialSoC)
		{
			if (initialSoC.IsSmaller(ModelData.MinSOC) || initialSoC.IsGreater(ModelData.MaxSOC))
			{
				throw new VectoException("SoC must be between {0} and {1}", ModelData.MinSOC, ModelData.MaxSOC);
			}
			PreviousState.StateOfCharge = initialSoC;
		}

		public IBatteryResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
		{
			var maxChargePower = MaxChargePower(dt);
			var maxDischargePower = MaxDischargePower(dt);

			if (powerDemand.IsGreater(maxChargePower, Constants.SimulationSettings.InterpolateSearchTolerance) ||
				powerDemand.IsSmaller(maxDischargePower, Constants.SimulationSettings.InterpolateSearchTolerance))
			{
				return PowerDemandExceeded(absTime, dt, powerDemand, maxDischargePower, maxChargePower, dryRun);
			}

			var internalResistance = ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge);
			var current = 0.SI<Ampere>();
			if (!powerDemand.IsEqual(0))
			{
				var solutions = VectoMath.QuadraticEquationSolver(internalResistance.Value(), InternalCellVoltage.Value(),
					-powerDemand.Value());
				current = SelectSolution(solutions, powerDemand.Value());
			}
			var batteryLoss = current * internalResistance * current;
			var currentCharge = ModelData.Capacity * PreviousState.StateOfCharge;

			if (dryRun)
			{
				return new BatteryDryRunResponse(this)
				{
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxBatteryLoadCharge = maxChargePower,
					MaxBatteryLoadDischarge = maxDischargePower,
					BatteryPower = powerDemand,
					BatteryLoss = batteryLoss,
					StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity

				};
			}
			CurrentState.SimulationInterval = dt;
			CurrentState.PowerDemand = powerDemand;
			CurrentState.TotalCurrent = current;
			CurrentState.BatteryLoss = batteryLoss;

			
			CurrentState.StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity;
			CurrentState.MaxChargePower = maxChargePower;
			CurrentState.MaxDischargePower = maxDischargePower;
			return new BatteryResponseSuccess(this)
			{
				AbsTime = absTime,
				SimulationInterval = dt,
				MaxBatteryLoadCharge = maxChargePower,
				MaxBatteryLoadDischarge = maxDischargePower,
				BatteryPower = powerDemand,
				BatteryLoss = batteryLoss,
				StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity
			};
		}

		private Ampere SelectSolution(double[] solutions, double sign)
		{
			return solutions.Where(x => Math.Sign(sign) == Math.Sign(x) && Math.Abs(x).IsSmallerOrEqual(ModelData.MaxCurrent.Value(), 1e-3)).Min().SI<Ampere>();
		}

		private IBatteryResponse PowerDemandExceeded(Second absTime, Second dt, Watt powerDemand, Watt maxDischargePower,
			Watt maxChargePower, bool dryRun)
		{
			var maxPower = powerDemand < 0 ? maxDischargePower : maxChargePower;

			var maxChargeCurrent = VectoMath.Min((ModelData.MaxSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt,
				ModelData.MaxCurrent);
			var maxDischargeCurrent = VectoMath.Max((ModelData.MinSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt,
				-ModelData.MaxCurrent);
			var current = powerDemand < 0 ? maxDischargeCurrent : maxChargeCurrent;

			var batteryLoss = current * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge) * current;

			AbstractBatteryResponse response;
			if (dryRun)
			{
				response = new BatteryDryRunResponse(this);
			}
			else
			{
				if (powerDemand > maxPower)
				{
					response = new BatteryOverloadResponse(this);
				}
				else
				{
					response = new BatteryUnderloadResponse(this);
				}
			}
			response.AbsTime = absTime;
			response.SimulationInterval = dt;
			response.MaxBatteryLoadCharge = maxChargePower;
			response.MaxBatteryLoadDischarge = maxDischargePower;
			response.BatteryPower = powerDemand;
			response.BatteryLoss = batteryLoss;

			return response;
		}

		#endregion


		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second absTime, Second dt, IModalDataContainer container)
		{
			var cellVoltage = ModelData.SOCMap.Lookup(PreviousState.StateOfCharge);
			container[ModalResultField.U0_bat] = cellVoltage;
			container[ModalResultField.U_bat_terminal] =
				cellVoltage +
				CurrentState.TotalCurrent *
				ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge); // adding both terms because pos. current charges the battery!
			container[ModalResultField.I_bat] = CurrentState.TotalCurrent;
			container[ModalResultField.BatteryStateOfCharge] = CurrentState.StateOfCharge.SI();
			container[ModalResultField.P_battery_terminal] = CurrentState.PowerDemand;
			container[ModalResultField.P_battery_int] = cellVoltage * CurrentState.TotalCurrent;
			container[ModalResultField.P_battery_loss] = CurrentState.BatteryLoss;
			container[ModalResultField.P_battery_charge_max] = CurrentState.MaxChargePower;
			container[ModalResultField.P_battery_discharge_max] = CurrentState.MaxDischargePower;

			container[ModalResultField.E_Bat] = CurrentState.StateOfCharge * cellVoltage * ModelData.Capacity;

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		#endregion


		#region Implementation of IBatteryInfo

		public Volt InternalCellVoltage
		{
			get { return ModelData.SOCMap.Lookup(PreviousState.StateOfCharge); }
		}


		public double StateOfCharge
		{
			get { return PreviousState.StateOfCharge; }
		}

		public WattSecond StoredEnergy
		{
			get {
				return PreviousState.StateOfCharge * ModelData.Capacity * ModelData.SOCMap.Lookup(PreviousState.StateOfCharge);
			}
		}

		public Watt MaxChargePower(Second dt)
		{
			var maxChargeCurrent = VectoMath.Min((ModelData.MaxSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt,
				ModelData.MaxCurrent);
			return InternalCellVoltage * maxChargeCurrent +
					maxChargeCurrent * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge) * maxChargeCurrent;
		}

		public Watt MaxDischargePower(Second dt)
		{
			var maxDischargeCurrent = VectoMath.Max(
				((ModelData.MinSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt).LimitTo(-ModelData.MaxCurrent,
					0.SI<Ampere>()), -ModelData.MaxCurrent);
			var cellVoltage = InternalCellVoltage;
			var maxDischargePower = InternalCellVoltage * maxDischargeCurrent +
									maxDischargeCurrent * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge) * maxDischargeCurrent;
			var maxPower = -cellVoltage / (4 * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge)) * cellVoltage;
			return VectoMath.Max(maxDischargePower, maxPower);
		}

		public Ampere MaxCurrent
		{
			get
			{
				var cellVoltage = ModelData.SOCMap.Lookup(PreviousState.StateOfCharge);
				return VectoMath.Max(-ModelData.MaxCurrent, -cellVoltage / (4 * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge)));
			}
		}

		#endregion

		public class State
		{
			public double StateOfCharge;

			public Second SimulationInterval;

			public Watt PowerDemand;

			public Ampere TotalCurrent;
			public Watt MaxChargePower;
			public Watt MaxDischargePower;
			public Watt BatteryLoss;
		}

	}
}