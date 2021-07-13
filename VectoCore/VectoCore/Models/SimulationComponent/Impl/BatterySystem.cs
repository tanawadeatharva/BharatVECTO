using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class BatterySystem : VectoSimulationComponent, IElectricEnergyStorage, IElectricEnergyStoragePort
	{
		public class BatteryString
		{
			protected readonly List<Battery> _batteries;
			
			private AmpereSecond _capacity;

			public BatteryString()
            {
				_batteries = new List<Battery>();
            }

			public IReadOnlyList<Battery> Batteries => _batteries;

			public void AddBattery(Battery bat)
			{
				_batteries.Add(bat);
				// Todo: update some properties?
				_capacity = null;
			}

			public Volt OpenCircuitVoltage => _batteries.Sum(x => x.InternalVoltage);

			public Ohm InternalResistance => _batteries.Sum(x => x.InternalResistance);

			public AmpereSecond Capacity => _capacity ?? (_capacity = _batteries.Min(x => x.Capacity));

			public double SoC => _batteries.Min(x => x.StateOfCharge * x.Capacity) / Capacity;

			public Watt MaxDischargePower(Second dt)
			{
				var maxDischargeCurrent = MaxDischargeCurrent(dt);
					var maxDischargePower = OpenCircuitVoltage * maxDischargeCurrent +
											maxDischargeCurrent * InternalResistance * maxDischargeCurrent;
					var maxPower = -OpenCircuitVoltage / (4 * InternalResistance) * OpenCircuitVoltage;
					return VectoMath.Max(maxDischargePower, maxPower);
				
			}

			public Ampere MaxChargeCurrent(Second dt)
			{
				return _batteries.Min(x => x.MaxChargeCurrent(dt));
			}

			public Ampere MaxDischargeCurrent(Second dt)
			{
				return _batteries.Max(x => x.MaxDischargeCurrent(dt));
			}

			public Watt MaxChargePower(Second dt)
			{
				var maxCurrent = MaxChargeCurrent(dt);
				return OpenCircuitVoltage * MaxChargeCurrent(dt) +
						maxCurrent * InternalResistance * maxCurrent;
			}

			public IList<IRESSResponse> Request(Second absTime, Second dt, Watt powerDemand, bool dryRun)
			{
				var current = 0.SI<Ampere>();
				if (!powerDemand.IsEqual(0)) {
					var solutions = VectoMath.QuadraticEquationSolver(InternalResistance.Value(), OpenCircuitVoltage.Value(),
						-powerDemand.Value());
					current = SelectSolution(solutions, powerDemand.Value(), dt);
				}

				return _batteries.Select(x => {
					var demand = (x.InternalVoltage + x.InternalResistance * current) * current;
					return x.Request(absTime, dt, demand, dryRun);
				}).ToList();
			}

			private Ampere SelectSolution(double[] solutions, double sign, Second dt)
			{
				var maxCurrent = Math.Sign(sign) < 0
					? MaxDischargeCurrent(dt)
					: MaxChargeCurrent(dt);
				return solutions.Where(x => Math.Sign(sign) == Math.Sign(x) && Math.Abs(x).IsSmallerOrEqual(Math.Abs(maxCurrent.Value()), 1e-3)).Min().SI<Ampere>();
			}
		}

		protected internal readonly Dictionary<int, BatteryString> Batteries = new Dictionary<int, BatteryString>();
		
		private AmpereSecond _totalCapacity;

		public BatterySystem(IVehicleContainer dataBus, BatterySystemData batterySystemData) : base(dataBus)
		{
			var idx = 0;
			foreach (var entry in batterySystemData.Batteries) {
				var bat = new Battery(null, entry.Item2, idx++);
				if (!Batteries.ContainsKey(entry.Item1)) {
					Batteries[entry.Item1] = new BatteryString();
				}
				Batteries[entry.Item1].AddBattery(bat);
			}
		}

		#region Overrides of VectoSimulationComponent

		public override void CommitSimulationStep(Second time, Second simulationInterval, IModalDataContainer container)
		{
			foreach (var battery in Batteries) {
				foreach (var b in battery.Value.Batteries) {
					b.CommitSimulationStep(time, simulationInterval, container);
				}
			}
			base.CommitSimulationStep(time, simulationInterval, container);
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}


		#endregion

		#region Implementation of IBatteryProvider

		public IElectricEnergyStoragePort MainBatteryPort => this;

		#endregion

		#region Implementation of IRESSInfo

		public Volt InternalVoltage => Batteries.Values.Select(x => x.OpenCircuitVoltage).Average();

		public double StateOfCharge => Batteries.Values.Sum(bs => bs.SoC * bs.Capacity) / TotalCapacity;

		public AmpereSecond TotalCapacity =>
			_totalCapacity ?? (_totalCapacity = Batteries.Values.Sum(bs => bs.Capacity));

		public WattSecond StoredEnergy { get; }
		
		public Watt MaxChargePower(Second dt)
		{
			return Batteries.Values.Sum(bs => bs.MaxChargePower(dt));

		}

		public Watt MaxDischargePower(Second dt)
		{
			return Batteries.Values.Sum(bs => bs.MaxDischargePower(dt));
		}

		public double MinSoC { get; }
		public double MaxSoC { get; }

		#endregion

		#region Implementation of IElectricEnergyStoragePort

		public void Initialize(double initialSoC)
		{
			foreach (var b in Batteries.Values.SelectMany(bs => bs.Batteries)) {
				b.Initialize(initialSoC);
			}
		}

		public IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun)
		{
			var maxChargePower = MaxChargePower(dt);
			var maxDischargePower = MaxDischargePower(dt);

			if (powerDemand.IsGreater(maxChargePower, Constants.SimulationSettings.InterpolateSearchTolerance) ||
				powerDemand.IsSmaller(maxDischargePower, Constants.SimulationSettings.InterpolateSearchTolerance)) {
				return PowerDemandExceeded(absTime, dt, powerDemand, maxDischargePower, maxChargePower, dryRun);
			}

			var averageSoC = (Batteries.Values.Sum(x => x.SoC * x.Capacity) / TotalCapacity).Value();
			var powerDemands = new Dictionary<int, Watt>();
			var limitBB = new Dictionary<int, Watt>();
			var distributedPower = 0.SI<Watt>();
			do {
				distributedPower = 0.SI<Watt>();
				var remainingPower = powerDemand - (limitBB.Sum(x => x.Value) ?? 0.SI<Watt>());
				powerDemands.Clear();
				foreach (var bs in Batteries) {
					if (limitBB.ContainsKey(bs.Key)) {
						continue;
					}
					var delta = 1 - Math.Sign(remainingPower.Value()) * (bs.Value.SoC - averageSoC) / averageSoC;
					var power = bs.Value.Capacity / TotalCapacity * delta * remainingPower;
					if (!power.IsBetween(bs.Value.MaxDischargePower(dt), bs.Value.MaxChargePower(dt))) {
						limitBB[bs.Key] = power.LimitTo(bs.Value.MaxDischargePower(dt), bs.Value.MaxChargePower(dt));
					} else {
						powerDemands[bs.Key] = power;
						distributedPower += power;
					}
				}
			} while (!distributedPower.IsEqual(powerDemand));

			powerDemands = powerDemands.Concat(limitBB).ToDictionary(x => x.Key, x=> x.Value);

			var responses = new Dictionary<int, IList<IRESSResponse>>();
			foreach (var entry in powerDemands) {
				responses[entry.Key] = Batteries[entry.Key].Request(absTime, dt, entry.Value, dryRun);
			}

			if (dryRun) {
				return new RESSDryRunResponse(this) {
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = responses.Sum(x => x.Value.Sum(b => b.LossPower)),
					//StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity
				};
			}

			if (responses.All(bb => bb.Value.All(b => b is RESSResponseSuccess))) {
				return new RESSResponseSuccess(this) {
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = responses.Sum(x => x.Value.Sum(b => b.LossPower)),
					//StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity
				};
			}
			throw new NotImplementedException("batterypack no success");
		}


		private IRESSResponse PowerDemandExceeded(Second absTime, Second dt, Watt powerDemand, Watt maxDischargePower,
			Watt maxChargePower, bool dryRun)
		{
			var maxPower = powerDemand < 0 ? maxDischargePower : maxChargePower;

			var maxChargeCurrent = Batteries.Values.Sum(bs => bs.MaxChargeCurrent(dt));
			var maxDischargeCurrent = Batteries.Values.Sum(bs => bs.MaxDischargeCurrent(dt));
			var current = powerDemand < 0 ? maxDischargeCurrent : maxChargeCurrent;
			var internalResistance = (1 / Batteries.Values.Sum(bs => 1 / bs.InternalResistance.Value())).SI<Ohm>();

			var batteryLoss = current * internalResistance * current;

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
	}
}