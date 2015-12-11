/*
* Copyright 2015 Graz University of Technology
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
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class TransmissionLossMap : LoggingObject
	{
		[JsonProperty] private readonly List<GearLossMapEntry> _entries;

		private readonly double _ratio;

		/// <summary>
		/// The Loss map. [X=Input EngineSpeed, Y=Output Torque] => Z=Input Torque
		/// </summary>
		private readonly DelauneyMap _lossMap;

		/// <summary>
		/// The inverted loss map for range sanity checks. [X=Input EngineSpeed, Y=Input Torque] => Z=Output Torque
		/// </summary>
		private readonly DelauneyMap _invertedLossMap;

		private readonly NewtonMeter _minTorque = double.PositiveInfinity.SI<NewtonMeter>();
		private readonly NewtonMeter _maxTorque = double.NegativeInfinity.SI<NewtonMeter>();
		private readonly PerSecond _maxSpeed = double.NegativeInfinity.SI<PerSecond>();
		private readonly PerSecond _minSpeed = double.PositiveInfinity.SI<PerSecond>();


		public string GearName { get; protected set; }


		public static TransmissionLossMap ReadFromFile(string fileName, double gearRatio, string gearName)
		{
			DataTable data;
			try {
				data = VectoCSVFile.Read(fileName, true);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading TransmissionLossMap: " + ex.Message);
			}

			if (data.Columns.Count < 3) {
				throw new VectoException("TransmissionLossMap Data File for {0} must consist of at least 3 columns.", gearName);
			}

			if (data.Rows.Count < 4) {
				throw new VectoException(
					"TransmissionLossMap for {0} must consist of at least four lines with numeric values (below file header", gearName);
			}

			List<GearLossMapEntry> entries;
			if (HeaderIsValid(data.Columns)) {
				entries = CreateFromColumnNames(data);
			} else {
				Logger<TransmissionLossMap>().Warn(
					"TransmissionLossMap {5}: Header line is not valid. Expected: '{0}, {1}, {2}, <{3}>'. Got: '{4}'. Falling back to column index.",
					Fields.InputSpeed, Fields.InputTorque, Fields.TorqeLoss, Fields.Efficiency,
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()), gearName);

				entries = CreateFromColumIndizes(data);
			}

			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.InputSpeed) && columns.Contains(Fields.InputTorque) &&
					columns.Contains(Fields.TorqeLoss);
		}

		private static List<GearLossMapEntry> CreateFromColumnNames(DataTable data)
		{
			var hasEfficiency = data.Columns.Contains(Fields.Efficiency);
			return (from DataRow row in data.Rows
				select new GearLossMapEntry {
					InputSpeed = row.ParseDouble(Fields.InputSpeed).RPMtoRad(),
					InputTorque = row.ParseDouble(Fields.InputTorque).SI<NewtonMeter>(),
					TorqueLoss = row.ParseDouble(Fields.TorqeLoss).SI<NewtonMeter>(),
					Efficiency =
						(!hasEfficiency || row[Fields.Efficiency] == DBNull.Value || row[Fields.Efficiency] != null)
							? double.NaN
							: row.ParseDouble(Fields.Efficiency)
				}).ToList();
		}

		private static List<GearLossMapEntry> CreateFromColumIndizes(DataTable data)
		{
			var hasEfficiency = (data.Columns.Count >= 4);
			return (from DataRow row in data.Rows
				select new GearLossMapEntry {
					InputSpeed = row.ParseDouble(0).RPMtoRad(),
					InputTorque = row.ParseDouble(1).SI<NewtonMeter>(),
					TorqueLoss = row.ParseDouble(2).SI<NewtonMeter>(),
					Efficiency = (!hasEfficiency || row[3] == DBNull.Value || row[3] != null) ? double.NaN : row.ParseDouble(3)
				}).ToList();
		}

		private TransmissionLossMap(List<GearLossMapEntry> entries, double gearRatio, string gearName)
		{
			GearName = gearName;
			_ratio = gearRatio;
			_entries = entries;
			_lossMap = new DelauneyMap();
			foreach (var entry in _entries) {
				var outTorque = (entry.InputTorque - entry.TorqueLoss) * _ratio;
				_minTorque = VectoMath.Min(_minTorque, outTorque);
				_maxTorque = VectoMath.Max(_maxTorque, outTorque);

				_minSpeed = VectoMath.Min(_minSpeed, entry.InputSpeed);
				_maxSpeed = VectoMath.Max(_maxSpeed, entry.InputSpeed);

				_lossMap.AddPoint(entry.InputSpeed.Value(), outTorque.Value(), entry.InputTorque.Value());
			}

			_lossMap.Triangulate();
			_invertedLossMap = _lossMap.CreateInvertedMap();
		}


		/// <summary>
		///	Computes the INPUT torque given by the input-engineSpeed and the output-torque.
		/// </summary>
		/// <param name="inAngularVelocity">Angular speed at input side.</param>
		/// <param name="outTorque">Torque at output side (as requested by the previous componend towards the wheels).</param>
		/// <returns>Torque needed at input side (towards the engine).</returns>
		public NewtonMeter GetInTorque(PerSecond inAngularVelocity, NewtonMeter outTorque)
		{
			try {
				var inTorque = _lossMap.Interpolate(inAngularVelocity.Value(), outTorque.Value()).SI<NewtonMeter>();
				Log.Debug("GearboxLoss {0}: {1}", GearName, inTorque - outTorque);

				// todo (MK, 2015-12-07): extrapolate?

				// Limit input torque to a maximum value without losses (just torque/ratio)
				return VectoMath.Max(inTorque, outTorque / _ratio);
			} catch (VectoException) {
				Log.Error("{0} - Failed to interpolate in TransmissionLossMap. angularVelocity: {1}, torque: {2}", GearName,
					inAngularVelocity, outTorque);

				return outTorque / _ratio;
			}
		}

		/// <summary>
		///	Computes the OUTPUT torque given by the input engineSpeed and the output torque.
		/// </summary>
		/// <param name="inAngularVelocity">Angular speed at input side.</param>
		/// <param name="inTorque">Torque at output side (as requested by the previous componend towards the wheels).</param>
		/// <returns>Torque needed at input side (towards the engine).</returns>
		public NewtonMeter GetOutTorque(PerSecond inAngularVelocity, NewtonMeter inTorque)
		{
			return _invertedLossMap.Interpolate(inAngularVelocity.Value(), inTorque.Value()).SI<NewtonMeter>();
		}

		public GearLossMapEntry this[int i]
		{
			get { return _entries[i]; }
		}

		public class GearLossMapEntry
		{
			public PerSecond InputSpeed { get; set; }

			public NewtonMeter InputTorque { get; set; }

			public NewtonMeter TorqueLoss { get; set; }

			public double Efficiency { get; set; }
		}

		private static class Fields
		{
			/// <summary>[rpm]</summary>
			public const string InputSpeed = "Input Speed";

			/// <summary>[Nm]</summary>
			public const string InputTorque = "Input Torque";

			/// <summary>[Nm]</summary>
			public const string TorqeLoss = "Torque Loss";

			/// <summary>[-]</summary>
			public const string Efficiency = "Eff";
		}
	}
}