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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class RetarderLossMap : SimulationComponentData
	{
		[ValidateObject] private List<RetarderLossEntry> _entries;

		public static RetarderLossMap ReadFromFile(string fileName)
		{
			try {
				DataTable data;
				data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception ex) {
				throw new VectoException("ERROR while loading RetarderLossMap: " + ex.Message);
			}
		}

		public static RetarderLossMap Create(DataTable data)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("RetarderLossMap Data File must consist of 2 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("RetarderLossMap must consist of at least two entries.");
			}

			List<RetarderLossEntry> entries;
			if (HeaderIsValid(data.Columns)) {
				entries = CreateFromColumnNames(data);
			} else {
				Logger<RetarderLossMap>().Warn(
					"RetarderLossMap: Header Line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.RetarderSpeed, Fields.TorqueLoss,
					", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()));
				entries = CreateFromColumnIndizes(data);
			}
			entries.Sort((entry1, entry2) => entry1.RetarderSpeed.Value().CompareTo(entry2.RetarderSpeed.Value()));
			return new RetarderLossMap { _entries = entries };
		}

		public NewtonMeter RetarderLoss(PerSecond angularVelocity, bool allowExtrapolation)
		{
			if (angularVelocity < _entries.First().RetarderSpeed) {
				if (!allowExtrapolation) {
					throw new VectoSimulationException("angular velocity {0} below min. entry in retarder loss map ({1})",
						angularVelocity, _entries.First().RetarderSpeed);
				}
				Log.Warn("Extrapolating retarder losses! Angular velocity {0} below min. entry in retarder loss map ({1})",
					angularVelocity, _entries.First().RetarderSpeed);
			}
			if (angularVelocity > _entries.Last().RetarderSpeed) {
				if (!allowExtrapolation) {
					throw new VectoSimulationException("angular velocity {0} above max. entry in retarder loss map ({1})",
						angularVelocity, _entries.Last().RetarderSpeed);
				}
				Log.Warn("Extrapolating retarder losses! Angular velocity {0} above max. entry in retarder loss map ({1})",
					angularVelocity, _entries.Last().RetarderSpeed);
			}

			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(_entries[idx - 1].RetarderSpeed, _entries[idx].RetarderSpeed,
				_entries[idx - 1].TorqueLoss, _entries[idx].TorqueLoss, angularVelocity);
		}

		protected int FindIndex(PerSecond angularVelocity)
		{
			int idx;
			if (angularVelocity < _entries[0].RetarderSpeed) {
				Log.Info("requested rpm below minimum rpm in retarder loss map - extrapolating. n_eng_avg: {0}, rpm_min: {1}",
					angularVelocity.ConvertTo().Rounds.Per.Minute,
					_entries[0].RetarderSpeed.ConvertTo().Rounds.Per.Minute);
				idx = 1;
			} else {
				idx = _entries.FindIndex(x => x.RetarderSpeed > angularVelocity);
			}
			if (idx <= 0) {
				idx = angularVelocity > _entries[0].RetarderSpeed ? _entries.Count - 1 : 1;
			}
			return idx;
		}

		private static List<RetarderLossEntry> CreateFromColumnNames(DataTable data)
		{
			return (from DataRow row in data.Rows
				select new RetarderLossEntry {
					RetarderSpeed = row.ParseDouble(Fields.RetarderSpeed).RPMtoRad(),
					TorqueLoss = row.ParseDouble(Fields.TorqueLoss).SI<NewtonMeter>()
				}).ToList();
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.RetarderSpeed) && columns.Contains(Fields.TorqueLoss);
		}

		private static List<RetarderLossEntry> CreateFromColumnIndizes(DataTable data)
		{
			return (from DataRow row in data.Rows
				select
					new RetarderLossEntry {
						RetarderSpeed = row.ParseDouble(0).RPMtoRad(),
						TorqueLoss = row.ParseDouble(1).SI<NewtonMeter>()
					}).ToList();
		}

		private class RetarderLossEntry
		{
			[Required, SIRange(0, double.MaxValue)]
			public PerSecond RetarderSpeed { get; set; }

			[Required, SIRange(0, 500)]
			public NewtonMeter TorqueLoss { get; set; }
		}

		public static class Fields
		{
			/// <summary>
			///     [rpm]
			/// </summary>
			public const string RetarderSpeed = "Retarder Speed";

			/// <summary>
			///     [Nm]
			/// </summary>
			public const string TorqueLoss = "Torque Loss";
		}
	}
}