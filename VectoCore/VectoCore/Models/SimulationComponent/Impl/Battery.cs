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
	public class Battery : StatefulVectoSimulationComponent<Battery.State>, IElectricEnergyStorage, IElectricEnergyStoragePort
	{
		protected readonly BatteryData ModelData;

		public Battery(IVehicleContainer container, BatteryData modelData) : base(container)
		{
			ModelData = modelData;
		}

		#region Implementation of IBatteryProvider
		public IElectricEnergyStoragePort MainBatteryPort
		{
			get { return this; }
		}

		#endregion


		#region Implementation of IElectricEnergyStoragePort

		public void Initialize(double initialSoC)
		{
			if (initialSoC.IsSmaller(ModelData.MinSOC) || initialSoC.IsGreater(ModelData.MaxSOC))
			{
				throw new VectoException("SoC must be between {0} and {1}", ModelData.MinSOC, ModelData.MaxSOC);
			}
			PreviousState.StateOfCharge = initialSoC;
		}

		public IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
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
				var solutions = VectoMath.QuadraticEquationSolver(internalResistance.Value(), InternalVoltage.Value(),
					-powerDemand.Value());
				current = SelectSolution(solutions, powerDemand.Value());
			}
			var batteryLoss = current * internalResistance * current;
			var currentCharge = ModelData.Capacity * PreviousState.StateOfCharge;

			if (dryRun) {
				return new RESSDryRunResponse(this) {
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = batteryLoss,
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
			return new RESSResponseSuccess(this) {
				AbsTime = absTime,
				SimulationInterval = dt,
				MaxChargePower = maxChargePower,
				MaxDischargePower = maxDischargePower,
				PowerDemand = powerDemand,
				LossPower = batteryLoss,
				StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity
			};
		}

		private Ampere SelectSolution(double[] solutions, double sign)
		{
			var maxCurrent = Math.Sign(sign) < 0
				? ModelData.MaxCurrent.LookupMaxDischargeCurrent(PreviousState.StateOfCharge)
				: ModelData.MaxCurrent.LookupMaxChargeCurrent(PreviousState.StateOfCharge);
			return solutions.Where(x => Math.Sign(sign) == Math.Sign(x) && Math.Abs(x).IsSmallerOrEqual(Math.Abs(maxCurrent.Value()), 1e-3)).Min().SI<Ampere>();
		}

		private IRESSResponse PowerDemandExceeded(Second absTime, Second dt, Watt powerDemand, Watt maxDischargePower,
			Watt maxChargePower, bool dryRun)
		{
			var maxPower = powerDemand < 0 ? maxDischargePower : maxChargePower;

			var maxChargeCurrent = VectoMath.Min((ModelData.MaxSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt,
				ModelData.MaxCurrent.LookupMaxChargeCurrent(PreviousState.StateOfCharge));
			var maxDischargeCurrent = VectoMath.Max((ModelData.MinSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt,
				ModelData.MaxCurrent.LookupMaxDischargeCurrent(PreviousState.StateOfCharge));
			var current = powerDemand < 0 ? maxDischargeCurrent : maxChargeCurrent;

			var batteryLoss = current * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge) * current;

			AbstractRESSResponse response;
			if (dryRun) {
				response = new RESSDryRunResponse(this);
			} else {
				if (powerDemand > maxPower) {
					response = new RESSOverloadResponse(this);
				} else {
					response = new RESSUnderloadResponse(this);
				}
			}
			response.AbsTime = absTime;
			response.SimulationInterval = dt;
			response.MaxChargePower = maxChargePower;
			response.MaxDischargePower = maxDischargePower;
			response.PowerDemand = powerDemand;
			response.LossPower = batteryLoss;

			return response;
		}

		#endregion


		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second absTime, Second dt, IModalDataContainer container)
		{
			var cellVoltage = ModelData.SOCMap.Lookup(PreviousState.StateOfCharge);
			container[ModalResultField.U0_reess] = cellVoltage;
			container[ModalResultField.U_reess_terminal] =
				cellVoltage +
				CurrentState.TotalCurrent *
				ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge); // adding both terms because pos. current charges the battery!
			container[ModalResultField.I_reess] = CurrentState.TotalCurrent;
			container[ModalResultField.REESSStateOfCharge] = CurrentState.StateOfCharge.SI();
			container[ModalResultField.P_reess_terminal] = CurrentState.PowerDemand;
			container[ModalResultField.P_reess_int] = cellVoltage * CurrentState.TotalCurrent;
			container[ModalResultField.P_reess_loss] = CurrentState.BatteryLoss;
			container[ModalResultField.P_reess_charge_max] = CurrentState.MaxChargePower;
			container[ModalResultField.P_reess_discharge_max] = CurrentState.MaxDischargePower;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		#endregion


		#region Implementation of IRESSInfo

		public Volt InternalVoltage
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
				ModelData.MaxCurrent.LookupMaxChargeCurrent(PreviousState.StateOfCharge));
			return InternalVoltage * maxChargeCurrent +
					maxChargeCurrent * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge) * maxChargeCurrent;
		}

		public Watt MaxDischargePower(Second dt)
		{
			var maxDischargeCurrent = VectoMath.Max(
				((ModelData.MinSOC - PreviousState.StateOfCharge) * ModelData.Capacity / dt).LimitTo(ModelData.MaxCurrent.LookupMaxDischargeCurrent(PreviousState.StateOfCharge),
					0.SI<Ampere>()), ModelData.MaxCurrent.LookupMaxDischargeCurrent(PreviousState.StateOfCharge));
			var cellVoltage = InternalVoltage;
			var maxDischargePower = InternalVoltage * maxDischargeCurrent +
									maxDischargeCurrent * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge) * maxDischargeCurrent;
			var maxPower = -cellVoltage / (4 * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge)) * cellVoltage;
			return VectoMath.Max(maxDischargePower, maxPower);
		}

		public double MinSoC
		{
			get { return ModelData.MinSOC; }
		}
		public double MaxSoC
		{
			get { return ModelData.MaxSOC; }
		}

		//public Ampere MaxCurrent
		//{
		//	get
		//	{
		//		var cellVoltage = ModelData.SOCMap.Lookup(PreviousState.StateOfCharge);
		//		return VectoMath.Max(-ModelData.MaxCurrent, -cellVoltage / (4 * ModelData.InternalResistance.Lookup(PreviousState.StateOfCharge)));
		//	}
		//}

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