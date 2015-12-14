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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AccelerationCurveData : SimulationComponentData
	{
		private List<KeyValuePair<MeterPerSecond, AccelerationEntry>> _entries;

		public static AccelerationCurveData ReadFromStream(Stream stream)
		{
			var data = VectoCSVFile.ReadStream(stream);
			return ParseData(data);
		}

		public static AccelerationCurveData ReadFromFile(string fileName)
		{
			DataTable data;
			try {
				data = VectoCSVFile.Read(fileName);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading AccelerationCurve File: " + ex.Message);
			}

			return ParseData(data);
		}

		private static AccelerationCurveData ParseData(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("Acceleration Limiting File must consist of 3 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Acceleration Limiting File must consist of at least two entries.");
			}

			if (HeaderIsValid(data.Columns)) {
				return CreateFromColumnNames(data);
			}
			Logger<AccelerationCurveData>().Warn(
				"Acceleration Curve: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}",
				Fields.Velocity, Fields.Acceleration, Fields.Deceleration,
				string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			return CreateFromColumnIndizes(data);
		}

		private static AccelerationCurveData CreateFromColumnIndizes(DataTable data)
		{
			return new AccelerationCurveData {
				_entries = data.Rows.Cast<DataRow>()
					.Select(r => new KeyValuePair<MeterPerSecond, AccelerationEntry>(
						r.ParseDouble(0).KMPHtoMeterPerSecond(),
						new AccelerationEntry {
							Acceleration = r.ParseDouble(1).SI<MeterPerSquareSecond>(),
							Deceleration = r.ParseDouble(2).SI<MeterPerSquareSecond>()
						}))
					.OrderBy(x => x.Key)
					.ToList()
			};
		}

		private static AccelerationCurveData CreateFromColumnNames(DataTable data)
		{
			return new AccelerationCurveData {
				_entries = data.Rows.Cast<DataRow>()
					.Select(r => new KeyValuePair<MeterPerSecond, AccelerationEntry>(
						r.ParseDouble(Fields.Velocity).KMPHtoMeterPerSecond(),
						new AccelerationEntry {
							Acceleration = r.ParseDouble(Fields.Acceleration).SI<MeterPerSquareSecond>(),
							Deceleration = r.ParseDouble(Fields.Deceleration).SI<MeterPerSquareSecond>()
						}))
					.OrderBy(x => x.Key)
					.ToList()
			};
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Velocity) &&
					columns.Contains(Fields.Acceleration) &&
					columns.Contains(Fields.Deceleration);
		}

		public AccelerationEntry Lookup(MeterPerSecond key)
		{
			var index = FindIndex(key);

			return new AccelerationEntry {
				Acceleration =
					VectoMath.Interpolate(_entries[index - 1].Key, _entries[index].Key,
						_entries[index - 1].Value.Acceleration,
						_entries[index].Value.Acceleration, key),
				Deceleration =
					VectoMath.Interpolate(_entries[index - 1].Key, _entries[index].Key,
						_entries[index - 1].Value.Deceleration,
						_entries[index].Value.Deceleration, key)
			};
		}

		protected int FindIndex(MeterPerSecond key)
		{
			var index = 1;
			if (key < _entries[0].Key) {
				Log.Error("requested velocity below minimum - extrapolating. velocity: {0}, min: {1}",
					key.ConvertTo().Kilo.Meter.Per.Hour, _entries[0].Key.ConvertTo().Kilo.Meter.Per.Hour);
			} else {
				index = _entries.FindIndex(x => x.Key > key);
				if (index <= 0) {
					index = (key > _entries[0].Key) ? _entries.Count - 1 : 1;
				}
			}
			return index;
		}

		public MeterPerSquareSecond MinDeceleration()
		{
			return _entries.Max(x => x.Value.Deceleration);
		}

		public MeterPerSquareSecond MaxDeceleration()
		{
			return _entries.Min(x => x.Value.Deceleration);
		}

		public MeterPerSquareSecond MinAcceleration()
		{
			return _entries.Min(x => x.Value.Acceleration);
		}

		public MeterPerSquareSecond MaxAcceleration()
		{
			return _entries.Max(x => x.Value.Acceleration);
		}

		[DebuggerDisplay("Acceleration: {Acceleration}, Deceleration: {Deceleration}")]
		public class AccelerationEntry
		{
			public MeterPerSquareSecond Acceleration { get; set; }
			public MeterPerSquareSecond Deceleration { get; set; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1">current speed of the vehicle</param>
		/// <param name="v2">desired speed of the vehicle at the end of acceleration/deceleration phase</param>
		/// <returns>distance required to accelerate/decelerate the vehicle from v1 to v2 according to the acceleration curve</returns>
		public Meter ComputeAccelerationDistance(MeterPerSecond v1, MeterPerSecond v2)
		{
			var index1 = FindIndex(v1);
			var index2 = FindIndex(v2);

			var distance = 0.SI<Meter>();
			for (var i = index2; i <= index1; i++) {
				distance += ComputeAccelerationSegmentDistance(i, v1, v2);
			}
			return distance;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i">segment of the acceleration curve to use [(i-1) ... i]</param>
		/// <param name="v1">current speed of the vehicle</param>
		/// <param name="v2">desired speed of the vehicle at the end of acceleration/deceleration phase</param>
		/// <returns>distance required to accelerate/decelerate the vehicle from v1 to v2 according to the acceleration curve</returns>
		private Meter ComputeAccelerationSegmentDistance(int i, MeterPerSecond v1, MeterPerSecond v2)
		{
			var leftEntry = _entries[i - 1]; // entry with lower velocity
			var rightEntry = _entries[i]; // entry with higher velocity

			v2 = VectoMath.Max(v2, leftEntry.Key); // min. velocity within current segment
			v1 = VectoMath.Min(v1, rightEntry.Key); // max. velocity within current segment

			if (leftEntry.Value.Deceleration.IsEqual(rightEntry.Value.Deceleration)) {
				// v(t) = a * t + v1  => t = (v2 - v1) / a
				// s(t) = a/2 * t^2 + v1 * t + s0  {s0 == 0}  => s(t)
				var acceleration = v2 > v1 ? leftEntry.Value.Acceleration : leftEntry.Value.Deceleration;
				return ((v2 - v1) * (v2 - v1) / 2.0 / acceleration + v1 * (v2 - v1) / acceleration).Cast<Meter>();
			}

			// a(v) = k * v + d
			// dv/dt = a(v) = d * v + d  ==> v(t) = sgn(k * v1 + d) * exp(-k * c) / k * exp(t * k) - d / k 
			// v(0) = v1  => c = - ln(|v1 * k + d|) / k
			// v(t) = (v1 + d / k) * exp(t * k) - d / k   => t = 1 / k * ln((v2 * k + d) / (v1 * k + d))
			// s(t) = m / k* exp(t * k) + b * t + c'   {m = v1 + d / k, b = -d / k}

			var k = (leftEntry.Value.Deceleration - rightEntry.Value.Deceleration) / (leftEntry.Key - rightEntry.Key);
			var d = leftEntry.Value.Deceleration - k * leftEntry.Key;
			if (v2 > v1) {
				k = (leftEntry.Value.Acceleration - rightEntry.Value.Acceleration) / (leftEntry.Key - rightEntry.Key);
				d = leftEntry.Value.Acceleration - k * leftEntry.Key;
			}
			var m = v1 + d / k;
			var b = -d / k;
			var c = 0.SI<Meter>() - m / k;
			var t = Math.Log(((v2 * k + d) / (v1 * k + d)).Cast<Scalar>()) / k;
			return m / k * Math.Exp((k * t).Value()) + b * t + c;
		}

		private static class Fields
		{
			public const string Velocity = "v";

			public const string Acceleration = "acc";

			public const string Deceleration = "dec";
		}
	}
}