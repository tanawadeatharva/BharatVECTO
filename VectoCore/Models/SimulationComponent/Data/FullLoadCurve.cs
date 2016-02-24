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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class FullLoadCurve : SimulationComponentData
	{
		private Watt _maxPower;
		private PerSecond _ratedSpeed;

		[Required, ValidateObject] internal List<FullLoadCurveEntry> FullLoadEntries;
		[Required] internal LookupData<PerSecond, Second> PT1Data;

		/// <summary>
		/// Get the rated speed from the given full-load curve (i.e. speed with max. power)
		/// </summary>
		[Required, SIRange(0, 5000 * Constants.RPMToRad)]
		public PerSecond RatedSpeed
		{
			get { return _ratedSpeed ?? ComputeRatedSpeed().Item1; }
		}

		/// <summary>
		/// Gets the maximum power.
		/// </summary>
		[Required, SIRange(0, 10000 * 5000 * Constants.RPMToRad)]
		public Watt MaxPower
		{
			get { return _maxPower ?? ComputeRatedSpeed().Item2; }
		}

		public static FullLoadCurve ReadFromFile(string fileName, bool declarationMode = false, bool engineFld = false)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data, declarationMode, engineFld);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading FullLoadCurve File: " + ex.Message);
			}
		}


		public static FullLoadCurve Create(DataTable data, bool declarationMode = false, bool engineFld = false)
		{
			if (engineFld) {
				if (data.Columns.Count < 3) {
					throw new VectoException("Engine FullLoadCurve Data File must consist of at least 3 columns.");
				}
			} else {
				if (data.Columns.Count < 2) {
					throw new VectoException("Gearbox FullLoadCurve Data File must consist of at least 2 columns.");
				}
			}

			if (data.Rows.Count < 2) {
				throw new VectoException(
					"FullLoadCurve must consist of at least two lines with numeric values (below file header)");
			}

			List<FullLoadCurveEntry> entriesFld;
			if (HeaderIsValid(data.Columns, engineFld)) {
				entriesFld = CreateFromColumnNames(data, engineFld);
			} else {
				Logger<FullLoadCurve>().Warn(
					"FullLoadCurve: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index.",
					Fields.EngineSpeed, Fields.TorqueFullLoad,
					Fields.TorqueDrag, ", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

				entriesFld = CreateFromColumnIndizes(data, engineFld);
			}

			LookupData<PerSecond, Second> tmp;
			if (declarationMode) {
				tmp = new PT1();
			} else {
				tmp = PT1Curve.Create(data);
			}
			entriesFld.Sort((entry1, entry2) => entry1.EngineSpeed.Value().CompareTo(entry2.EngineSpeed.Value()));
			return new FullLoadCurve { FullLoadEntries = entriesFld, PT1Data = tmp };
		}

		private static bool HeaderIsValid(DataColumnCollection columns, bool engineFld)
		{
			return columns.Contains(Fields.EngineSpeed)
					&& columns.Contains(Fields.TorqueFullLoad)
					&& (!engineFld || columns.Contains(Fields.TorqueDrag));
		}

		private static List<FullLoadCurveEntry> CreateFromColumnNames(DataTable data, bool engineFld)
		{
			return (from DataRow row in data.Rows
				select new FullLoadCurveEntry {
					EngineSpeed = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					TorqueFullLoad = row.ParseDouble(Fields.TorqueFullLoad).SI<NewtonMeter>(),
					TorqueDrag = (engineFld ? row.ParseDouble(Fields.TorqueDrag).SI<NewtonMeter>() : null)
				}).ToList();
		}

		private static List<FullLoadCurveEntry> CreateFromColumnIndizes(DataTable data, bool engineFld)
		{
			return (from DataRow row in data.Rows
				select new FullLoadCurveEntry {
					EngineSpeed = row.ParseDouble(0).RPMtoRad(),
					TorqueFullLoad = row.ParseDouble(1).SI<NewtonMeter>(),
					TorqueDrag = (engineFld ? row.ParseDouble(2).SI<NewtonMeter>() : null)
				}).ToList();
		}

		/// <summary>
		/// Compute the engine's rated speed from the given full-load curve (i.e. engine speed with max. power)
		/// </summary>
		protected Tuple<PerSecond, Watt> ComputeRatedSpeed()
		{
			var max = new Tuple<PerSecond, Watt>(0.SI<PerSecond>(), 0.SI<Watt>());
			for (var idx = 1; idx < FullLoadEntries.Count; idx++) {
				var currentMax = FindMaxPower(FullLoadEntries[idx - 1], FullLoadEntries[idx]);
				if (currentMax.Item2 > max.Item2) {
					max = currentMax;
				}
			}

			_ratedSpeed = max.Item1;
			_maxPower = max.Item2;

			return max;
		}

		private Tuple<PerSecond, Watt> FindMaxPower(FullLoadCurveEntry p1, FullLoadCurveEntry p2)
		{
			if (p1.EngineSpeed.IsEqual(p2.EngineSpeed)) {
				return Tuple.Create(p1.EngineSpeed, p1.TorqueFullLoad * p1.EngineSpeed);
			}

			if (p2.EngineSpeed < p1.EngineSpeed) {
				var tmp = p1;
				p1 = p2;
				p2 = tmp;
			}

			// y = kx + d
			var k = (p2.TorqueFullLoad - p1.TorqueFullLoad) / (p2.EngineSpeed - p1.EngineSpeed);
			var d = p2.TorqueFullLoad - k * p2.EngineSpeed;
			if (k.IsEqual(0)) {
				return Tuple.Create(p2.EngineSpeed, p2.TorqueFullLoad * p2.EngineSpeed);
			}
			var engineSpeedMaxPower = -d / (2 * k);
			if (engineSpeedMaxPower.IsSmaller(p1.EngineSpeed) || engineSpeedMaxPower.IsGreater(p2.EngineSpeed)) {
				if (k.IsGreater(0)) {
					return Tuple.Create(p2.EngineSpeed, p2.TorqueFullLoad * p2.EngineSpeed);
				}
				return Tuple.Create(p1.EngineSpeed, p1.TorqueFullLoad * p1.EngineSpeed);
			}
			var engineTorqueMaxPower = FullLoadStationaryTorque(engineSpeedMaxPower);
			return Tuple.Create(engineSpeedMaxPower, engineTorqueMaxPower * engineSpeedMaxPower);
		}

		public virtual NewtonMeter FullLoadStationaryTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed,
				FullLoadEntries[idx - 1].TorqueFullLoad, FullLoadEntries[idx].TorqueFullLoad,
				angularVelocity);
		}

		public virtual NewtonMeter DragLoadStationaryTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed,
				FullLoadEntries[idx - 1].TorqueDrag, FullLoadEntries[idx].TorqueDrag,
				angularVelocity);
		}

		/// <summary>
		/// Get item index for angularVelocity.
		/// </summary>
		protected int FindIndex(PerSecond angularVelocity)
		{
			int index;
			FullLoadEntries.GetSection(x => x.EngineSpeed < angularVelocity, out index,
				string.Format("requested rpm outside of FLD curve - extrapolating. rpm: {0}",
					angularVelocity.ConvertTo().Rounds.Per.Minute));
			return index + 1;
		}

		[DebuggerDisplay("n: {EngineSpeed}, fullTorque: {TorqueFullLoad}, dragTorque: {TorqueDrag}")]
		internal class FullLoadCurveEntry
		{
			[Required, SIRange(0, 5000 * Constants.RPMToRad)]
			public PerSecond EngineSpeed { get; set; }

			[Required, SIRange(0, 10000)]
			public NewtonMeter TorqueFullLoad { get; set; }

			[Required, SIRange(-10000, 0)]
			public NewtonMeter TorqueDrag { get; set; }

			#region Equality members

			protected bool Equals(FullLoadCurveEntry other)
			{
				return Equals(EngineSpeed, other.EngineSpeed) && Equals(TorqueFullLoad, other.TorqueFullLoad) &&
						Equals(TorqueDrag, other.TorqueDrag);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) {
					return false;
				}
				if (ReferenceEquals(this, obj)) {
					return true;
				}
				if (obj.GetType() != GetType()) {
					return false;
				}
				return Equals((FullLoadCurveEntry)obj);
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = (EngineSpeed != null ? EngineSpeed.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (TorqueFullLoad != null ? TorqueFullLoad.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (TorqueDrag != null ? TorqueDrag.GetHashCode() : 0);
					return hashCode;
				}
			}

			#endregion
		}

		private static class Fields
		{
			/// <summary>
			/// [rpm] engine speed
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			/// [Nm] full load torque
			/// </summary>
			public const string TorqueFullLoad = "full load torque";

			/// <summary>
			/// [Nm] motoring torque
			/// </summary>
			public const string TorqueDrag = "motoring torque";
		}
	}
}