using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader
{
	public class ShiftPolygonReader : LoggingObject
	{
		public static ShiftPolygon ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading ShiftPolygon: " + ex.Message);
			}
		}

		public static ShiftPolygon Create(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("ShiftPolygon Data File must contain exactly 3 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("ShiftPolygon must have at least two entries");
			}

			List<ShiftPolygon.ShiftPolygonEntry> entriesDown, entriesUp;
			if (HeaderIsValid(data.Columns)) {
				entriesDown = CreateFromColumnNames(data, Fields.AngularSpeedDown);
				entriesUp = CreateFromColumnNames(data, Fields.AngularSpeedUp);
			} else {
				Logger<ShiftPolygon>()
					.Warn(
						"ShiftPolygon: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index",
						Fields.Torque, Fields.AngularSpeedUp, Fields.AngularSpeedDown,
						", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()));
				entriesDown = CreateFromColumnIndizes(data, 1);
				entriesUp = CreateFromColumnIndizes(data, 2);
			}
			return new ShiftPolygon(entriesDown, entriesUp);
		}


		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Torque) && columns.Contains(Fields.AngularSpeedUp) &&
					columns.Contains((Fields.AngularSpeedDown));
		}

		private static List<ShiftPolygon.ShiftPolygonEntry> CreateFromColumnNames(DataTable data, string columnName)
		{
			return (from DataRow row in data.Rows
				select new ShiftPolygon.ShiftPolygonEntry {
					Torque = DataTableExtensionMethods.ParseDouble(row, (string)Fields.Torque).SI<NewtonMeter>(),
					AngularSpeed = row.ParseDouble(columnName).RPMtoRad(),
				}).ToList();
		}

		private static List<ShiftPolygon.ShiftPolygonEntry> CreateFromColumnIndizes(DataTable data, int column)
		{
			return (from DataRow row in data.Rows
				select
					new ShiftPolygon.ShiftPolygonEntry {
						Torque = row.ParseDouble(0).SI<NewtonMeter>(),
						AngularSpeed = row.ParseDouble(column).RPMtoRad(),
					}).ToList();
		}

		private static class Fields
		{
			/// <summary>
			///		[Nm] torque
			/// </summary>
			public const string Torque = "engine torque";

			/// <summary>
			///		[rpm] threshold for upshift
			/// </summary>
			public const string AngularSpeedUp = "upshift rpm";

			/// <summary>
			///		[rpm] threshold for downshift
			/// </summary>
			public const string AngularSpeedDown = "downshift rpm";
		}
	}
}