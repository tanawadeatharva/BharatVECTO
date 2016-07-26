using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class RetarderLossMapReader
	{
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

			List<RetarderLossMap.RetarderLossEntry> entries;
			if (HeaderIsValid(data.Columns)) {
				entries = CreateFromColumnNames(data);
			} else {
				LoggingObject.Logger<RetarderLossMap>().Warn(
					"RetarderLossMap: Header Line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.RetarderSpeed, Fields.TorqueLoss,
					", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()));
				entries = CreateFromColumnIndizes(data);
			}

			entries.Sort((entry1, entry2) => entry1.RetarderSpeed.Value().CompareTo(entry2.RetarderSpeed.Value()));
			return new RetarderLossMap(entries);
		}

		private static List<RetarderLossMap.RetarderLossEntry> CreateFromColumnNames(DataTable data)
		{
			return data.Rows.Cast<DataRow>()
				.Select(row => new RetarderLossMap.RetarderLossEntry {
					RetarderSpeed = DataTableExtensionMethods.ParseDouble(row, (string)Fields.RetarderSpeed).RPMtoRad(),
					TorqueLoss = DataTableExtensionMethods.ParseDouble(row, (string)Fields.TorqueLoss).SI<NewtonMeter>()
				}).ToList();
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.RetarderSpeed) && columns.Contains(Fields.TorqueLoss);
		}

		private static List<RetarderLossMap.RetarderLossEntry> CreateFromColumnIndizes(DataTable data)
		{
			return data.Rows.Cast<DataRow>()
				.Select(row => new RetarderLossMap.RetarderLossEntry {
					RetarderSpeed = row.ParseDouble(0).RPMtoRad(),
					TorqueLoss = row.ParseDouble(1).SI<NewtonMeter>()
				}).ToList();
		}

		private static class Fields
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