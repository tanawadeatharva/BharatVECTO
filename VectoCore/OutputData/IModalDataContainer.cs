/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public interface IModalDataContainer
	{
		/// <summary>
		/// Identify which run this modaldata container is for
		/// </summary>
		string RunName { get; }

		/// <summary>
		/// Identify which cycle is simulated 
		/// </summary>
		string CycleName { get; }

		/// <summary>
		/// Custom suffix for this run, typically the loading type
		/// </summary>
		string RunSuffix { get; }

		/// <summary>
		/// Indexer for fields of the DataWriter. Accesses the data of the current step.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object this[ModalResultField key] { get; set; }

		/// <summary>
		/// Indexer for auxiliary fields of the DataWriter.
		/// </summary>
		/// <param name="auxId"></param>
		/// <returns></returns>
		object this[string auxId] { get; set; }

		bool HasTorqueConverter { get; set; }

		/// <summary>
		/// Commits the data of the current simulation step.
		/// </summary>
		void CommitSimulationStep();

		VectoRun.Status RunStatus { get; }


		/// <summary>
		/// Finishes the writing of the DataWriter.
		/// </summary>
		void Finish(VectoRun.Status runStatus);

		bool WriteModalResults { get; set; }

		IEnumerable<T> GetValues<T>(ModalResultField key);

		IEnumerable<T> GetValues<T>(DataColumn col);


		Dictionary<string, DataColumn> Auxiliaries { get; set; }

		void AddAuxiliary(string id);
	}

	public static class ModalDataWriterExtensions
	{
		public static SI Max(this IModalDataContainer data, ModalResultField field)
		{
			return data.GetValues<SI>(field).Max();
		}

		public static SI Min(this IModalDataContainer data, ModalResultField field)
		{
			return data.GetValues<SI>(field).Min();
		}

		public static SI Average(this IModalDataContainer data, ModalResultField field, Func<SI, bool> filter = null)
		{
			return data.GetValues<SI>(field).Average(filter);
		}

		public static SI Sum(this IModalDataContainer data, ModalResultField field, Func<SI, bool> filter = null)
		{
			return data.GetValues<SI>(field).Where(filter ?? (x => x != null)).Sum();
		}

		public static SI Sum(this IModalDataContainer data, DataColumn col, Func<SI, bool> filter = null)
		{
			return data.GetValues<SI>(col).Where(filter ?? (x => x != null)).Sum();
		}

		public static SI Average(this IEnumerable<SI> self, Func<SI, bool> filter)
		{
			var values = self.Where(filter ?? (x => x != null && !double.IsNaN(x.Value()))).ToList();
			return values.Any() ? values.Sum() / values.Count : null;
		}

		public static object DefaultIfNull(this object self)
		{
			return self ?? DBNull.Value;
		}

		public static T DefaultIfNull<T>(this T self, T defaultValue) where T : class
		{
			return self ?? defaultValue;
		}

		public static MeterPerSquareSecond AccelerationsPositive3SecondAverage(this IModalDataContainer data)
		{
			try {
				var acceleration3SecondAverage = AccelerationPer3Seconds(data);
				return acceleration3SecondAverage.Where(x => x > 0.125).Average();
			} catch (NullReferenceException) {
				return null;
			}
		}

		public static MeterPerSquareSecond AccelerationNoise(this IModalDataContainer data)
		{
			var avg = data.AccelerationAverage();
			var accelerationAverages = AccelerationPerSecond(data).ToList();
			if (accelerationAverages.Any()) {
				var sqareAvg = accelerationAverages.Select(x => (x - avg) * (x - avg)).Sum() / accelerationAverages.Count;
				return sqareAvg.Sqrt().Cast<MeterPerSquareSecond>();
			}
			return null;
		}

		public static MeterPerSquareSecond AverageAccelerations3SecondNegative(this IModalDataContainer data)
		{
			var acceleration3SecondAverage = AccelerationPer3Seconds(data).ToList();
			if (acceleration3SecondAverage.Any()) {
				return acceleration3SecondAverage.Where(x => x < -0.125).Average();
			}
			return null;
		}

		public static Scalar PercentAccelerationTime(this IModalDataContainer data)
		{
			var acceleration3SecondAverage = AccelerationPer3Seconds(data).ToList();
			if (acceleration3SecondAverage.Any()) {
				return 100.SI<Scalar>() * acceleration3SecondAverage.Count(x => x > 0.125) / acceleration3SecondAverage.Count;
			}
			return null;
		}

		public static Scalar PercentDecelerationTime(this IModalDataContainer data)
		{
			var acceleration3SecondAverage = AccelerationPer3Seconds(data).ToList();
			if (acceleration3SecondAverage.Any()) {
				return 100.SI<Scalar>() * acceleration3SecondAverage.Count(x => x < -0.125) / acceleration3SecondAverage.Count;
			}
			return null;
		}

		public static Scalar PercentCruiseTime(this IModalDataContainer data)
		{
			var acceleration3SecondAverage = AccelerationPer3Seconds(data).ToList();
			if (acceleration3SecondAverage.Any()) {
				return 100.SI<Scalar>() * acceleration3SecondAverage.Count(x => x.IsBetween(-0.125, -0.125)) /
						acceleration3SecondAverage.Count;
			}
			return null;
		}

		public static Scalar PercentStopTime(this IModalDataContainer data)
		{
			var stopTime = data.GetValues<MeterPerSecond>(ModalResultField.v_act)
				.Zip(data.SimulationIntervals(), (v, dt) => new { v, dt })
				.Where(x => x.v < 0.1).Select(x => x.dt).Sum() ?? 0.SI<Second>();
			return 100 * (stopTime / data.Duration()).Cast<Scalar>();
		}

		public static MeterPerSquareSecond AccelerationAverage(this IModalDataContainer data)
		{
			return data.TimeIntegral<MeterPerSecond>(ModalResultField.acc) / data.Duration();
		}

		public static Second[] SimulationIntervals(this IModalDataContainer data)
		{
			return data.GetValues<Second>(ModalResultField.simulationInterval).ToArray();
		}

		public static Meter AltitudeDelta(this IModalDataContainer data)
		{
			var altitudes = data.GetValues<Meter>(ModalResultField.altitude).ToList();
			var first = altitudes.First();
			var last = altitudes.Last();
			return first == null || last == null ? null : last - first;
		}

		public static WattSecond PowerAccelerations(this IModalDataContainer data)
		{
			var paEngine = data.TimeIntegral<WattSecond>(ModalResultField.PaEng);
			var paGearbox = data.TimeIntegral<WattSecond>(ModalResultField.PaGB);
			return paEngine + paGearbox;
		}

		public static WattSecond WorkTransmission(this IModalDataContainer data)
		{
			var plossdiff = data.TimeIntegral<WattSecond>(ModalResultField.PlossGB);
			var plossgb = data.TimeIntegral<WattSecond>(ModalResultField.PlossDiff);
			return plossdiff + plossgb;
		}

		public static WattSecond WorkRetarder(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.PlossRetarder);
		}

		public static WattSecond WorkTorqueConverter(this IModalDataContainer data)
		{
			//TODO (MK, 2015-11-10): return torque converter work - this was currently not possible because torque converter is not implemented.
			return 0.SI<WattSecond>();
		}

		public static Second Duration(this IModalDataContainer data)
		{
			var time = data.GetValues<Second>(ModalResultField.time).ToList();
			if (time.Count == 1) {
				return time.First();
			} else {
				return time.Max() - time.Min();
			}
		}

		public static Meter Distance(this IModalDataContainer data)
		{
			var max = data.Max(ModalResultField.dist);
			var min = data.Min(ModalResultField.dist);
			return max == null || min == null ? null : (max - min).Cast<Meter>();
		}

		public static WattSecond WorkTotalMechanicalBrake(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pbrake);
		}

		public static WattSecond WorkAuxiliaries(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Paux);
		}

		public static WattSecond WorkRoadGradientResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pgrad);
		}

		public static WattSecond WorkRollingResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Proll);
		}

		public static WattSecond WorkAirResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pair);
		}

		public static WattSecond EngineWorkPositive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pe_eng, x => x > 0);
		}

		public static WattSecond EngineWorkNegative(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pe_eng, x => x < 0);
		}

		public static Watt PowerBrake(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pbrake) / data.Duration();
		}

		public static Watt PowerWheelPositive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.Pwheel, x => x > 0) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionWHTCCorrected(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / distance;
		}

		public static KilogramPerSecond FuelConsumptionWHTCCorrectedPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionAuxStartStopCorrected(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAUXc) / distance;
		}

		public static KilogramPerSecond FuelConsumptionAuxStartStopCorrectedPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAUXc) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionFinal(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / distance;
		}

		public static SI FuelConsumptionFinalLiterPer100Kilometer(this IModalDataContainer data)
		{
			var fuelConsumptionFinal = data.FuelConsumptionFinal();
			if (fuelConsumptionFinal == null) {
				return null;
			}

			var fcVolumePerMeter = fuelConsumptionFinal / Physics.FuelDensity;
			return fcVolumePerMeter.ConvertTo().Cubic.Dezi.Meter * 100.SI().Kilo.Meter;
		}

		public static KilogramPerMeter CO2PerMeter(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCMap) * Physics.CO2PerFuelWeight / distance;
		}

		public static SI FuelConsumptionLiterPer100Kilometer(this IModalDataContainer data)
		{
			var fcVolumePerMeter = data.FuelConsumptionPerMeter() / Physics.FuelDensity;
			return fcVolumePerMeter.ConvertTo().Cubic.Dezi.Meter * 100.SI().Kilo.Meter;
		}

		public static KilogramPerSecond FuelConsumptionPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCMap) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionPerMeter(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCMap) / distance;
		}

		public static Watt EnginePowerNegativeAverage(this IModalDataContainer data)
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			var values = data.GetValues<Watt>(ModalResultField.Pe_eng)
				.Zip(simulationIntervals, (value, dt) => new { Dt = dt, Value = value * dt })
				.Where(v => v.Value < 0).ToList();
			if (values.Any()) {
				return values.Select(v => v.Value).Sum() / values.Select(v => v.Dt).Sum();
			}
			return 0.SI<Watt>();
		}

		public static Watt EnginePowerPositiveAverage(this IModalDataContainer data)
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			var values = data.GetValues<Watt>(ModalResultField.Pe_eng)
				.Zip(simulationIntervals, (value, dt) => new { Dt = dt, Value = value * dt })
				.Where(v => v.Value > 0).ToList();
			if (values.Any()) {
				return values.Select(v => v.Value).Sum() / values.Select(v => v.Dt).Sum();
			}
			return 0.SI<Watt>();
		}

		public static MeterPerSecond Speed(this IModalDataContainer data)
		{
			var distance = Distance(data);
			var duration = Duration(data);
			if (distance == null || duration == null || duration.IsEqual(0)) {
				return null;
			}
			return distance / duration;
		}

		public static WattSecond AuxiliaryWork(this IModalDataContainer data, DataColumn auxCol)
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			return data.GetValues<Watt>(auxCol).Zip(simulationIntervals, (value, dt) => value * dt).Sum().Cast<WattSecond>();
		}


		private static T TimeIntegral<T>(this IModalDataContainer data, ModalResultField field, Func<SI, bool> filter = null)
			where T : SIBase<T>
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			var filteredList = data.GetValues<SI>(field)
				.Zip(simulationIntervals, (value, dt) => new { value, dt })
				.Where(x => filter == null || filter(x.value)).ToList();

			return filteredList.Any()
				? filteredList.Select(x => (x.value == null ? SIBase<T>.Create(0) : x.value * x.dt)).Sum().Cast<T>()
				: SIBase<T>.Create(0);
		}

		private static IEnumerable<MeterPerSquareSecond> AccelerationPer3Seconds(IModalDataContainer data)
		{
			var accelerationAverages = AccelerationPerSecond(data).ToList();
			if (accelerationAverages.Count >= 3) {
				var runningAverage = (accelerationAverages[0] + accelerationAverages[1] + accelerationAverages[2]) / 3.0;

				yield return runningAverage;

				for (var i = 2; i < accelerationAverages.Count - 1; i++) {
					runningAverage -= accelerationAverages[i - 2] / 3.0;
					runningAverage += accelerationAverages[i + 1] / 3.0;
					yield return runningAverage;
				}
			}
		}


		/// <summary>
		/// Calculates the average acceleration for whole seconds.
		/// </summary>
		private static IEnumerable<MeterPerSquareSecond> AccelerationPerSecond(IModalDataContainer data)
		{
			var dtSum = 0.SI<Second>();
			var accAvg = 0.SI<MeterPerSecond>();

			var accValues = data.GetValues<MeterPerSquareSecond>(ModalResultField.acc);

			foreach (
				var value in accValues.Zip(SimulationIntervals(data), (acc, dt) => new { acc, dt }).Where(v => v.acc != null)) {
				var dt = value.dt;
				var acc = value.acc;

				while (dtSum + dt >= 1) {
					var diffDt = 1.SI<Second>() - dtSum;
					yield return (accAvg + acc * diffDt) / 1.SI<Second>();
					dt -= diffDt;
					dtSum = 0.SI<Second>();
					accAvg = 0.SI<MeterPerSecond>();
				}
				if (dt > 0) {
					accAvg += acc * dt;
					dtSum += dt;
				}
			}

			// return remaining data. acts like extrapolation to next whole second.
			if (dtSum > 0) {
				yield return accAvg / 1.SI<Second>();
			}
		}
	}
}