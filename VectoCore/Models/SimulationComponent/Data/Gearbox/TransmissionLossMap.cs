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
		/// The Loss map. [X=Output EngineSpeed, Y=Output Torque] => Z=Torque Loss
		/// </summary>
		private readonly DelauneyMap _lossMap;

		/// <summary>
		/// The inverted loss map for range sanity checks. [X=Input EngineSpeed, Y=Input Torque] => Z=Output Torque
		/// </summary>
		private readonly DelauneyMap _invertedLossMap;

		/// <summary>
		/// True if the last access to GetInTorque was an extrapolation.
		/// </summary>
		public bool Extrapolated
		{
			get { return _lossMap.Extrapolated; }
		}

		public string GearName { get; protected set; }


		public static TransmissionLossMap ReadFromFile(string fileName, double gearRatio, string gearName)
		{
			try {
				var data = VectoCSVFile.Read(fileName, true);
				return Create(data, gearRatio, gearName);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading TransmissionLossMap: " + ex.Message);
			}
		}

		public static TransmissionLossMap Create(DataTable data, double gearRatio, string gearName)
		{
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
			_invertedLossMap = new DelauneyMap();
			foreach (var entry in _entries) {
				var outTorque = (entry.InputTorque - entry.TorqueLoss) * _ratio;
				var outSpeed = entry.InputSpeed.Value() / _ratio;
				_lossMap.AddPoint(outSpeed, outTorque.Value(), entry.TorqueLoss.Value());
				_invertedLossMap.AddPoint(entry.InputSpeed.Value(), entry.InputTorque.Value(), entry.TorqueLoss.Value());
			}

			_lossMap.Triangulate();
			_invertedLossMap.Triangulate();
		}


		/// <summary>
		///	Computes the torque loss (input side) given by the output gearbox speed and the output-torque.
		/// </summary>
		/// <param name="outAngularVelocity">Angular speed at output side.</param>
		/// <param name="outTorque">Torque at output side (as requested by the previous componend towards the wheels).</param>
		/// <returns>Torque loss as seen on input side (towards the engine).</returns>
		public NewtonMeter GetTorqueLoss(PerSecond outAngularVelocity, NewtonMeter outTorque)
		{
			var torqueLoss = _lossMap.Interpolate(outAngularVelocity.Value(), outTorque.Value(), true).SI<NewtonMeter>();

			Log.Debug("GearboxLoss {0}: {1}, outAngularVelocity: {2}, outTorque: {3}", GearName, torqueLoss,
				outAngularVelocity, outTorque);
			return torqueLoss;
		}

		///  <summary>
		/// 	Computes the OUTPUT torque given by the input engineSpeed and the output torque.
		///  </summary>
		///  <param name="inAngularVelocity">Angular speed at input side.</param>
		///  <param name="inTorque">Torque at output side (as requested by the previous componend towards the wheels).</param>
		/// <param name="allowExtrapolation"></param>
		/// <returns>Torque needed at input side (towards the engine).</returns>
		public NewtonMeter GetOutTorque(PerSecond inAngularVelocity, NewtonMeter inTorque, bool allowExtrapolation = false)
		{
			var torqueLoss =
				_invertedLossMap.Interpolate(inAngularVelocity.Value(), inTorque.Value(), allowExtrapolation).SI<NewtonMeter>();
			return (inTorque - torqueLoss) / _ratio;
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