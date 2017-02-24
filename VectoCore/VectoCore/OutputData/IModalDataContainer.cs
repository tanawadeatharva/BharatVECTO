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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public interface IModalDataFilter
	{
		ModalResults Filter(ModalResults data);
		string ID { get; }
	}

	public interface IModalDataContainer
	{
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

		IEnumerable<T> GetValues<T>(ModalResultField key);

		IEnumerable<T> GetValues<T>(DataColumn col);

		Dictionary<string, DataColumn> Auxiliaries { get; set; }
		T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>;

		void SetDataValue(string fieldName, object value);

		void AddAuxiliary(string id, string columnName = null);

		/// <summary>
		/// clear the modal data after the simulation
		/// called after the simulation is finished and the sum-entries have been written
		/// </summary>
		void FinishSimulation();
	}

	public static class ModalDataContainerExtensions
	{
		public static SI Max(this IModalDataContainer data, ModalResultField field)
		{
			return data.GetValues<SI>(field).Max();
		}

		public static SI Min(this IModalDataContainer data, ModalResultField field)
		{
			return data.GetValues<SI>(field).Min();
		}

		public static SI Average(this IEnumerable<SI> self, Func<SI, bool> filter)
		{
			var values = self.Where(filter ?? (x => x != null && !double.IsNaN(x.Value()))).ToList();
			return values.Any() ? values.Sum() / values.Count : null;
		}

		/// <summary>
		/// Returns a default value if the SI object is null.
		/// </summary>
		/// <typeparam name="T">The SI Type.</typeparam>
		/// <param name="self">The SI Instance.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>If self is null, the default value as SI-Type is returned. Otherwise self is returned.</returns>
		/// <code>
		/// NewtonMeter t = null;
		/// var x = t.DefaultIfNull(0);
		/// </code>
		public static T DefaultIfNull<T>(this T self, double defaultValue) where T : SIBase<T>
		{
			return self ?? defaultValue.SI<T>();
		}

		public static MeterPerSquareSecond AccelerationsPositive(this ModalDataContainer data)
		{
			return
				data.GetValues<MeterPerSquareSecond>(ModalResultField.acc)
					.Where(x => x < -0.125)
					.DefaultIfEmpty(0.SI<MeterPerSquareSecond>())
					.Average();
		}

		public static MeterPerSquareSecond AccelerationsNegative(this ModalDataContainer data)
		{
			return
				data.GetValues<MeterPerSquareSecond>(ModalResultField.acc)
					.Where(x => x < -0.125)
					.DefaultIfEmpty(0.SI<MeterPerSquareSecond>())
					.Average();
		}

		public static Scalar AccelerationTimeShare(this ModalDataContainer data)
		{
			var accelerationTimeShare = data.Data.Rows.Cast<DataRow>()
				.Select(x => new {
					a = x.Field<MeterPerSquareSecond>((int)ModalResultField.acc),
					dt = x.Field<Second>((int)ModalResultField.simulationInterval)
				})
				.Where(x => x.a > 0.125).Sum(x => x.dt).DefaultIfNull(0);
			return 100 * (accelerationTimeShare / data.Duration()).Cast<Scalar>();
		}

		public static Scalar DecelerationTimeShare(this ModalDataContainer data)
		{
			var decelerationTimeShare = data.Data.Rows.Cast<DataRow>()
				.Select(x => new {
					a = x.Field<MeterPerSquareSecond>((int)ModalResultField.acc),
					dt = x.Field<Second>((int)ModalResultField.simulationInterval)
				})
				.Where(x => x.a < -0.125).Sum(x => x.dt).DefaultIfNull(0);
			return 100 * (decelerationTimeShare / data.Duration()).Cast<Scalar>();
		}

		public static Scalar CruiseTimeShare(this ModalDataContainer data)
		{
			var cruiseTime = data.Data.Rows.Cast<DataRow>()
				.Select(x => new {
					v = x.Field<MeterPerSecond>((int)ModalResultField.v_act),
					a = x.Field<MeterPerSquareSecond>((int)ModalResultField.acc),
					dt = x.Field<Second>((int)ModalResultField.simulationInterval)
				})
				.Where(x => x.v >= 0.1 && x.a.IsBetween(-0.125, 0.125)).Sum(x => x.dt).DefaultIfNull(0);
			return 100 * (cruiseTime / data.Duration()).Cast<Scalar>();
		}

		public static Scalar StopTimeShare(this ModalDataContainer data)
		{
			var stopTime = data.Data.Rows.Cast<DataRow>()
				.Select(x => new {
					v = x.Field<MeterPerSecond>((int)ModalResultField.v_act),
					dt = x.Field<Second>((int)ModalResultField.simulationInterval)
				})
				.Where(x => x.v < 0.1).Sum(x => x.dt) ?? 0.SI<Second>();
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
			var paEngine = data.TimeIntegral<WattSecond>(ModalResultField.P_eng_inertia);
			var paGearbox = data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_inertia);
			return paEngine + paGearbox;
		}

		public static WattSecond WorkClutch(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_clutch_loss);
		}

		public static WattSecond WorkGearshift(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_shift_loss);
		}

		public static WattSecond WorkGearbox(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_loss);
		}

		public static WattSecond WorkAxlegear(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_axle_loss);
		}

		public static WattSecond WorkRetarder(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_ret_loss);
		}

		public static WattSecond WorkAngledrive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_angle_loss);
		}

		public static WattSecond WorkTorqueConverter(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_TC_loss);
		}

		public static Second Duration(this IModalDataContainer data)
		{
			var time = data.GetValues<Second>(ModalResultField.time).ToList();
			var dt = data.GetValues<Second>(ModalResultField.simulationInterval).ToList();
			if (time.Count == 1) {
				return time.First();
			}
			return time.Max() - time.Min() + dt.First() / 2 + dt.Last() / 2;
		}

		public static Meter Distance(this IModalDataContainer data)
		{
			var max = data.Max(ModalResultField.dist);
			var min = data.Min(ModalResultField.dist);
			return max == null || min == null ? null : (max - min).Cast<Meter>();
		}

		public static WattSecond WorkTotalMechanicalBrake(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_brake_loss);
		}

		public static WattSecond WorkVehicleInertia(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_veh_inertia) +
					data.TimeIntegral<WattSecond>(ModalResultField.P_wheel_inertia);
		}

		public static WattSecond WorkAuxiliaries(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_aux);
		}

		public static WattSecond WorkRoadGradientResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_slope);
		}

		public static WattSecond WorkRollingResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_roll);
		}

		public static WattSecond WorkAirResistance(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_air);
		}

		public static WattSecond EngineWorkPositive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_eng_out, x => x > 0);
		}

		public static WattSecond EngineWorkNegative(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_eng_out, x => x < 0);
		}

		public static WattSecond TotalEngineWorkPositive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_eng_fcmap, x => x > 0);
		}

		public static WattSecond TotalEngineWorkNegative(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_eng_fcmap, x => x < 0);
		}

		public static Watt PowerWheelPositive(this IModalDataContainer data)
		{
			return data.TimeIntegral<WattSecond>(ModalResultField.P_wheel_in, x => x > 0) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionWHTC(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / distance;
		}

		public static KilogramPerSecond FuelConsumptionWHTCPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCWHTCc) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionAuxStartStop(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAUXc) / distance;
		}

		public static KilogramPerSecond FuelConsumptionAAUXPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAAUX) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionAAUX(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAAUX) / distance;
		}

		public static KilogramPerSecond FuelConsumptionAuxStartStopPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCAUXc) / data.Duration();
		}

		public static KilogramPerSecond FuelConsumptionFinalPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) / data.Duration();
		}

		public static KilogramPerMeter FuelConsumptionFinal(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) / distance;
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
			return data.TimeIntegral<Kilogram>(ModalResultField.FCFinal) * Physics.CO2PerFuelWeight / distance;
		}

		public static KilogramPerSecond FCMapPerSecond(this IModalDataContainer data)
		{
			return data.TimeIntegral<Kilogram>(ModalResultField.FCMap) / data.Duration();
		}

		public static KilogramPerMeter FCMapPerMeter(this IModalDataContainer data)
		{
			var distance = data.Distance();
			if (distance == null || distance.IsEqual(0)) {
				return null;
			}
			return data.TimeIntegral<Kilogram>(ModalResultField.FCMap) / distance;
		}

		
		public static Watt TotalPowerEnginePositiveAverage(this IModalDataContainer data)
		{
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval);
			var values = data.GetValues<Watt>(ModalResultField.P_eng_fcmap)
				.Zip(simulationIntervals, (value, dt) => new { Dt = dt, Value = value * dt })
				.Where(v => v.Value > 0).ToList();
			if (values.Any()) {
				return values.Sum(v => v.Value) / Duration(data);
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
			var simulationIntervals = data.GetValues<Second>(ModalResultField.simulationInterval).ToArray();
			var auxValues = data.GetValues<Watt>(auxCol).ToArray();
			var sum = 0.SI<WattSecond>();
			for (var i = 0; i < simulationIntervals.Length; i++) {
				if (auxValues[i] != null && simulationIntervals[i] != null) {
					sum += auxValues[i] * simulationIntervals[i];
				}
			}
			return sum;
		}

		
	}
}