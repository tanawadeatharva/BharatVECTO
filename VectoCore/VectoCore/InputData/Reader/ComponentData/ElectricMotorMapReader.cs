using System;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData {
	public static class ElectricMotorMapReader
	{
		public static EfficiencyMap Create(Stream data, int count)
		{
			return Create(VectoCSVFile.ReadStream(data), count);
		}

		public static EfficiencyMap Create(DataTable data, int count)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid)
			{
				LoggingObject.Logger<FuelConsumptionMap>().Warn(
					"Efficiency Map: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}. Falling back to column index.",
					Fields.MotorSpeed, 
					Fields.Torque, 
					Fields.PowerElectrical,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.Torque;
				data.Columns[2].ColumnName = Fields.PowerElectrical;
			}
			var delaunayMap = new DelaunayMap("ElectricMotorEfficiencyMap Mechanical to Electric");
			foreach (DataRow row in data.Rows)
			{
				try
				{
					var entry = CreateEntry(row);
					if (entry.Torque.IsGreaterOrEqual(0)) {
						delaunayMap.AddPoint(-entry.Torque.Value() * count,
							entry.MotorSpeed.Value(), 
							-entry.PowerElectrical.Value() * count);
					} else {
						delaunayMap.AddPoint(-entry.Torque.Value() * count,
							entry.MotorSpeed.Value(),
							-entry.PowerElectrical.Value() * count);
					}
				}
                catch (Exception e)
				{
					throw new VectoException($"EfficiencyMap - Line {data.Rows.IndexOf(row)}: {e.Message}", e);
				}
			}

			delaunayMap.Triangulate();
			return new EfficiencyMap(delaunayMap);
		}

		private static EfficiencyMap.Entry CreateEntry(DataRow row)
		{
			return new EfficiencyMap.Entry(
				speed: row.ParseDouble(Fields.MotorSpeed).RPMtoRad(),
				torque: row.ParseDouble(Fields.Torque).SI<NewtonMeter>(),
				powerElectrical: row.ParseDouble(Fields.PowerElectrical).SI(Unit.SI.Kilo.Watt).Cast<Watt>());
		}


		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.MotorSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.PowerElectrical);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";
			public const string Torque = "T";
			public const string PowerElectrical = "P_el";
		}
	}
}