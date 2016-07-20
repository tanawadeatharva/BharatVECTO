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
	/// <summary>
	/// LossMap for retarder.
	/// </summary>
	public class RetarderLossMap : SimulationComponentData
	{
		[ValidateObject] private List<RetarderLossEntry> _entries;
		private PerSecond _minSpeed;
		private PerSecond _maxSpeed;

		/// <summary>
		/// Gets the minimal defined speed of the retarder loss map.
		/// </summary>
		public PerSecond MinSpeed
		{
			get { return _minSpeed ?? (_minSpeed = _entries.Min(e => e.RetarderSpeed)); }
		}

		/// <summary>
		/// Gets the maximal defined speed of the retarder loss map.
		/// </summary>
		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = _entries.Max(e => e.RetarderSpeed)); }
		}

		/// <summary>
		/// Read the retarder loss map from a file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static RetarderLossMap ReadFromFile(string fileName)
		{
			try {
				return Create(VectoCSVFile.Read(fileName));
			} catch (Exception ex) {
				throw new VectoException("ERROR while loading RetarderLossMap: " + ex.Message);
			}
		}

		/// <summary>
		/// Create the retarder loss map from an appropriate datatable. (2 columns: Retarder Speed, Torque Loss)
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static RetarderLossMap Create(DataTable data)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("RetarderLossMap Data File must consist of 2 columns: Retarder Speed, Torque Loss");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("RetarderLossMap must contain at least 2 entries.");
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

		/// <summary>
		/// Calculates the retarder losses.
		/// </summary>
		/// <param name="angularVelocity"></param>
		/// <returns></returns>
		public NewtonMeter RetarderLoss(PerSecond angularVelocity)
		{
			var s = _entries.GetSection(e => e.RetarderSpeed < angularVelocity);
			return VectoMath.Interpolate(s.Item1.RetarderSpeed, s.Item2.RetarderSpeed, s.Item1.TorqueLoss, s.Item2.TorqueLoss,
				angularVelocity);
		}

		private static List<RetarderLossEntry> CreateFromColumnNames(DataTable data)
		{
			return data.Rows.Cast<DataRow>()
				.Select(row => new RetarderLossEntry {
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
			return data.Rows.Cast<DataRow>()
				.Select(row => new RetarderLossEntry {
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