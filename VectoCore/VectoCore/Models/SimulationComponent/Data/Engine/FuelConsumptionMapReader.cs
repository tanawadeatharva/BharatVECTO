using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
{
	public static class FuelConsumptionMapReader
	{
		public static FuelConsumptionMap ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException(string.Format("File {0}: {1}", fileName, e.Message), e);
			}
		}

		public static FuelConsumptionMap Create(DataTable data)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid) {
				LoggingObject.Logger<FuelConsumptionMap>().Warn(
					"FuelConsumptionMap: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}",
					Fields.EngineSpeed, Fields.Torque, Fields.FuelConsumption,
					", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}
			var delaunayMap = new DelaunayMap("FuelConsumptionMap");

			foreach (DataRow row in data.Rows) {
				try {
					var entry = headerValid ? CreateFromColumNames(row) : CreateFromColumnIndizes(row);
					delaunayMap.AddPoint(entry.Torque.Value(),
						(headerValid ? row.ParseDouble(Fields.EngineSpeed) : row.ParseDouble(0)).RPMtoRad().Value(),
						entry.FuelConsumption.Value());
				} catch (Exception e) {
					throw new VectoException(string.Format("Line {0}: {1}", data.Rows.IndexOf(row), e.Message), e);
				}
			}

			delaunayMap.Triangulate();
			return new FuelConsumptionMap(delaunayMap);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.EngineSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.FuelConsumption);
		}

		private static FuelConsumptionMap.Entry CreateFromColumnIndizes(DataRow row)
		{
			return new FuelConsumptionMap.Entry(
				engineSpeed: row.ParseDouble(0).RPMtoRad(),
				torque: row.ParseDouble(1).SI<NewtonMeter>(),
				fuelConsumption:
					row.ParseDouble(2).SI().Gramm.Per.Hour.ConvertTo().Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>()
				);
		}

		private static FuelConsumptionMap.Entry CreateFromColumNames(DataRow row)
		{
			return new FuelConsumptionMap.Entry(
				engineSpeed: row.ParseDouble(Fields.EngineSpeed).SI().Rounds.Per.Minute.Cast<PerSecond>(),
				torque: row.ParseDouble(Fields.Torque).SI<NewtonMeter>(),
				fuelConsumption:
					row.ParseDouble(Fields.FuelConsumption)
						.SI()
						.Gramm.Per.Hour.ConvertTo()
						.Kilo.Gramm.Per.Second.Cast<KilogramPerSecond>()
				);
		}

		public static class Fields
		{
			/// <summary>
			/// [rpm]
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			/// [Nm]
			/// </summary>
			public const string Torque = "torque";

			/// <summary>
			/// [g/h]
			/// </summary>
			public const string FuelConsumption = "fuel consumption";
		}
	}
}