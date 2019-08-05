using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData {
	public static class WHRPowerReader
	{
		public static WHRPowerMap ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException(string.Format("File {0}: {1}", fileName, e.Message), e);
			}
		}

		public static WHRPowerMap Create(DataTable data)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid) {
				LoggingObject.Logger<FuelConsumptionMap>().Warn(
					"FuelConsumptionMap: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}",
					Fields.EngineSpeed, Fields.Torque, Fields.ElectricPower,
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}
			var delaunayMap = new DelaunayMap("FuelConsumptionMap");

			if (!headerValid) {
				data.Columns[0].ColumnName = Fields.EngineSpeed;
				data.Columns[1].ColumnName = Fields.Torque;
				// column with idx==2 is fuel consumption in csv files
				data.Columns[3].ColumnName = Fields.ElectricPower;
			}

			foreach (DataRow row in data.Rows) {
				try {
					var entry = new WHRPowerMap.Entry (
						engineSpeed: row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
						torque: row.ParseDouble(Fields.Torque).SI<NewtonMeter>(),
						electricPower:row.ParseDouble(Fields.ElectricPower).SI<Watt>()
					);
					delaunayMap.AddPoint(entry.Torque.Value(),
										(headerValid ? row.ParseDouble(Fields.EngineSpeed) : row.ParseDouble(0)).RPMtoRad().Value(),
										entry.ElectricPower.Value());
				} catch (Exception e) {
					throw new VectoException(string.Format("WHR Map - Line {0}: {1}", data.Rows.IndexOf(row), e.Message), e);
				}
			}

			delaunayMap.Triangulate();
			return new WHRPowerMap(delaunayMap);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.EngineSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.ElectricPower);
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
			public const string ElectricPower = "whr power";
		}
	}
}