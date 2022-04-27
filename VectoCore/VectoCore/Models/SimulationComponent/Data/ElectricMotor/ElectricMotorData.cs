using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class ElectricMotorData
	{
		[SIRange(double.MinValue, double.MaxValue)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[SIRange(0, 1)]
		public double OverloadRegenerationFactor { get; internal set; }

		public double RatioADC { get; internal set; }

		public TransmissionLossMap TransmissionLossMap { get; internal set; }

		public double[] RatioPerGear { get; set; }

		[ValidateObject]
		public VoltageLevelData EfficiencyData { get; internal set; }

		public DragCurve DragCurve { get; internal set; }

		// not read direcly from input but calculated in a pre-processing step
		public OverloadData Overload { get; internal set; }
	}

	public class VoltageLevelData
	{
		private PerSecond _maxSpeed;

		public IList<ElectricMotorVoltageLevelData> VoltageLevels { get; internal set; }


		public PerSecond MaxSpeed =>
			_maxSpeed ?? (_maxSpeed = VoltageLevels
				.Min(v => VectoMath.Min(v.EfficiencyMap.MaxSpeed, v.FullLoadCurve.MaxSpeed)));

		
		public NewtonMeter EfficiencyMapLookupTorque(Volt voltage, Watt electricPower, PerSecond avgSpeed, NewtonMeter maxEmTorque)
		{
			if (avgSpeed.IsEqual(0.RPMtoRad()) || avgSpeed.IsGreater(MaxSpeed)) {
				return 0.SI<NewtonMeter>();
			}
			var (a, b) = GetSection(voltage);
			var r1 = a.EfficiencyMap.LookupTorque(electricPower, avgSpeed, maxEmTorque);
			var r2 = b.EfficiencyMap.LookupTorque(electricPower, avgSpeed, maxEmTorque);

			if (r1 is null && r2 is null) {
				return null;
			}

			// if one of the values is limited by EM, but the other is not (is null): use maxEmTorque instead
			if (r1 is null) {
				r1 = maxEmTorque;
			}
			if (r2 is null) {
				r2 = maxEmTorque;
			}

			var retVal = VectoMath.Interpolate(a.Voltage, b.Voltage,
				r1, r2, voltage);
			var elPwr = LookupElectricPower(voltage, avgSpeed, retVal, true);
			if (elPwr.ElectricalPower != null && electricPower.IsEqual(elPwr.ElectricalPower, 1e-3.SI<Watt>())) {
				return retVal;
			}

			var searchResult = SearchAlgorithm.Search(retVal, electricPower - elPwr.ElectricalPower,
				interval: 10.SI<NewtonMeter>(),
				getYValue: x => (Watt)x - electricPower,
				evaluateFunction: x => LookupElectricPower(voltage, avgSpeed, x, true).ElectricalPower,
				criterion: x => ((Watt)x - electricPower).Value(),
				searcher: this
			);

			return searchResult;
			//return null;
			//throw new NotImplementedException("EfficientyMapLookupTorque");
		}

		public EfficiencyMap.EfficiencyResult LookupElectricPower(Volt voltage, PerSecond avgSpeed, NewtonMeter torque, bool allowExtrapolation = false)
		{
			var tuple = GetSection(voltage);

			var r1 = tuple.Item1.EfficiencyMap.LookupElectricPower(avgSpeed, torque, allowExtrapolation);
			var r2 = tuple.Item2.EfficiencyMap.LookupElectricPower(avgSpeed, torque, allowExtrapolation);

			if (r1 == null || r2 == null || r1.ElectricalPower == null || r2.ElectricalPower == null) {
				return new EfficiencyMap.EfficiencyResult() {
					ElectricalPower = null,
					Speed = avgSpeed,
					Torque = torque
				};
			}

			var pwr = VectoMath.Interpolate(tuple.Item1.Voltage, tuple.Item2.Voltage, r1.ElectricalPower,
				r2.ElectricalPower, voltage);

			return new EfficiencyMap.EfficiencyResult() {
				ElectricalPower = pwr,
				Extrapolated = r1.Extrapolated || r2.Extrapolated,
				Speed = avgSpeed,
				Torque = torque
			};
		}

		public NewtonMeter FullGenerationTorque(Volt voltage, PerSecond avgSpeed)
		{
			var tuple = GetSection(voltage);

			return VectoMath.Interpolate(tuple.Item1.Voltage, tuple.Item2.Voltage,
				tuple.Item1.FullLoadCurve.FullGenerationTorque(avgSpeed),
				tuple.Item2.FullLoadCurve.FullGenerationTorque(avgSpeed), voltage);
		}

		public NewtonMeter FullLoadDriveTorque(Volt voltage, PerSecond avgSpeed)
		{
			var (electricMotorVoltageLevelData, item2) = GetSection(voltage);

			return VectoMath.Interpolate(electricMotorVoltageLevelData.Voltage, item2.Voltage,
				electricMotorVoltageLevelData.FullLoadCurve.FullLoadDriveTorque(avgSpeed),
				item2.FullLoadCurve.FullLoadDriveTorque(avgSpeed), voltage);
		}

		protected (ElectricMotorVoltageLevelData, ElectricMotorVoltageLevelData) GetSection(Volt voltage)
		{
			if (voltage < VoltageLevels.First().Voltage) {
				return (VoltageLevels.First(), VoltageLevels.First());
			}

			if (voltage > VoltageLevels.Last().Voltage) {
				return (VoltageLevels.Last(), VoltageLevels.Last());
			}
			return VoltageLevels.GetSection(x => voltage > x.Voltage);
		}
	}

	public class ElectricMotorVoltageLevelData
	{
		[SIRange(0, double.MaxValue)]
		public Volt Voltage { get; internal set; }
		
		[ValidateObject]
		public ElectricMotorFullLoadCurve FullLoadCurve { get; internal set; }

		[ValidateObject]
		public EfficiencyMap EfficiencyMap { get; internal set; }


	}

	public class OverloadData
	{
		public NewtonMeter ContinuousTorque { get; internal set; }

		public Joule OverloadBuffer { get; internal set; }

		public Watt ContinuousPowerLoss { get; internal set; }
	}
}