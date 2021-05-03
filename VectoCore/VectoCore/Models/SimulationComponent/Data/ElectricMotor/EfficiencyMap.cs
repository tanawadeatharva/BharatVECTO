using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor
{
	public class EfficiencyMap : LoggingObject
	{
		private readonly DelaunayMap _efficiencyMapMech2El;
		private PerSecond _maxSpeed;

		protected internal EfficiencyMap(DelaunayMap efficiencyMapMech2El)
		{
			_efficiencyMapMech2El = efficiencyMapMech2El;
		}


		public EfficiencyResult LookupElectricPower(PerSecond angularSpeed, NewtonMeter torque, bool allowExtrapolation = false)
		{
			var result = new EfficiencyResult();
			result.Torque = torque;
			var value = _efficiencyMapMech2El.Interpolate(torque, angularSpeed);
			if (value.HasValue)
			{
				result.ElectricalPower = value.Value.SI<Watt>();
				return result;
			}
			if (allowExtrapolation)
			{
				result.ElectricalPower = _efficiencyMapMech2El.Extrapolate(torque, angularSpeed).SI<Watt>();
				result.Extrapolated = true;
				return result;
			}
			return result;
		}

		public EfficiencyResult SearchMechanicalPower(Watt electricPower, PerSecond angularSpeed,
			bool allowExtrapolation = false)
		{
			if (electricPower.IsEqual(0))
			{
				return new EfficiencyResult
				{
					ElectricalPower = electricPower,
					Speed = angularSpeed,
					Torque = 0.SI<NewtonMeter>()
				};
			}
			var torque = electricPower / angularSpeed;
			var response = LookupElectricPower(angularSpeed, torque, true);
			var delta = response.ElectricalPower - electricPower;
			torque = SearchAlgorithm.Search(torque, delta, torque * 0.1,
				getYValue: result => ((EfficiencyMap.EfficiencyResult)result).ElectricalPower - electricPower,
				evaluateFunction: x => LookupElectricPower(angularSpeed, x, true),
				criterion: result => (((EfficiencyMap.EfficiencyResult)result).ElectricalPower - electricPower).Value());

			return new EfficiencyResult
			{
				ElectricalPower = electricPower,
				Speed = angularSpeed,
				Torque = torque
			};
		}

		public string[] SerializedEntries
		{
			get { return _efficiencyMapMech2El.Entries.Select(
												entry => $"{entry.Y.SI<PerSecond>().AsRPM} [rpm], {entry.X.SI<NewtonMeter>()}, {entry.Z.SI<Watt>()}")
											.ToArray();
			}
		}

		[JsonIgnore]
		public IReadOnlyCollection<EfficiencyMap.Entry> Entries
		{
			get
			{
				var entries = _efficiencyMapMech2El.Entries;
				var retVal = new EfficiencyMap.Entry[entries.Count];
				var i = 0;
				foreach (var entry in entries)
				{
					retVal[i++] = new EfficiencyMap.Entry(entry.Y.SI<PerSecond>(), entry.X.SI<NewtonMeter>(), entry.Z.SI<Watt>());
				}
				return retVal;
			}
		}

        public class Entry
		{
			public Entry(PerSecond speed, NewtonMeter torque, Watt powerElectrical)
			{
				MotorSpeed = speed;
				Torque = torque;
				PowerElectrical = powerElectrical;
			}

			public readonly PerSecond MotorSpeed;
			public readonly NewtonMeter Torque;
			public readonly Watt PowerElectrical;
		}

		public class EfficiencyResult
		{
			public PerSecond Speed;
			public NewtonMeter Torque;
			public Watt ElectricalPower;
			public bool Extrapolated;
		}

		public NewtonMeter LookupTorque(Watt batPower, PerSecond avgSpeed, NewtonMeter maxEmTorque)
		{
			var elPowerMaxEM = LookupElectricPower(avgSpeed, maxEmTorque, true);
			if (maxEmTorque < 0) { // distinguish between propulsion and recuperation
				if (!elPowerMaxEM.Extrapolated && elPowerMaxEM.ElectricalPower.IsGreaterOrEqual(batPower)) {
					// the battery can provide more electric power than the EM  - no limitation here
					return null;
				}
			} else {
				if (!elPowerMaxEM.Extrapolated && elPowerMaxEM.ElectricalPower.IsSmallerOrEqual(batPower)) {
					// the battery can provide more electric power than the EM  - no limitation here
					return null;
				}
			}

			if (batPower.IsEqual(0, 1e-3)) {
				return null;
			}

			if (avgSpeed.IsGreaterOrEqual(MaxSpeed)) {
				return 0.SI<NewtonMeter>();
			}

			try {
				var retVal = SearchAlgorithm.Search(
					maxEmTorque, elPowerMaxEM.ElectricalPower - batPower,
					-maxEmTorque * 0.1 * (maxEmTorque > 0 ? -1 : 1),
					getYValue: x => {
						var myX = (EfficiencyResult)x;
						return myX.ElectricalPower - batPower;
					},
					evaluateFunction: x => LookupElectricPower(avgSpeed, x, true),
					criterion: x => {
						var myX = (EfficiencyResult)x;
						return (myX.ElectricalPower - batPower).Value();
					});
				var tmp = LookupElectricPower(avgSpeed, retVal, true);
				if ((tmp.ElectricalPower - batPower).IsGreater(Constants.SimulationSettings.InterpolateSearchTolerance)) {
					// searched operating point is not accurate enough...
					retVal = SearchAlgorithm.Search(
						maxEmTorque, elPowerMaxEM.ElectricalPower - batPower,
						-maxEmTorque * 0.1 * (maxEmTorque > 0 ? -1 : 1),
						getYValue: x => {
							var myX = (EfficiencyResult)x;
							return (myX.ElectricalPower - batPower) * 1e3;
						},
						evaluateFunction: x => LookupElectricPower(avgSpeed, x, true),
						criterion: x => {
							var myX = (EfficiencyResult)x;
							return (myX.ElectricalPower - batPower).Value() * 1e3;
						});
				}
				return retVal;
			} catch (VectoSearchFailedException vsfe) {
				Log.Error("Failed to find mechanic power for given electric power! n_avg: {0} P_el: {1}; {2}", avgSpeed.AsRPM, batPower, vsfe.Message);
			}

			return null;
		}

		protected PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = _efficiencyMapMech2El.Entries.Select(x => x.Y).Max().SI<PerSecond>()); }
		}
	}
}