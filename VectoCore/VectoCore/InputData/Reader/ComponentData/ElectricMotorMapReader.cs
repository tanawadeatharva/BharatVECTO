using System;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData {
	public static class ElectricMotorMapReader
	{
		public static EfficiencyMap Create(Stream data)
		{
			return Create(VectoCSVFile.ReadStream(data));
		}

		public static EfficiencyMap Create(DataTable data)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid)
			{
				LoggingObject.Logger<FuelConsumptionMap>().Warn(
					"Efficiencymap: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}",
					FuelConsumptionMapReader.Fields.EngineSpeed, FuelConsumptionMapReader.Fields.Torque, FuelConsumptionMapReader.Fields.FuelConsumption,
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.Torque;
				data.Columns[2].ColumnName = Fields.PowerElectrical;
			}
			var delaunayMap = new DelaunayMap("ElectricMotorEfficiencyMap Mechanicla to Electric");
			foreach (DataRow row in data.Rows)
			{
				try
				{
					var entry = CreateEntry(row);
					delaunayMap.AddPoint(-entry.Torque.Value(), entry.MotorSpeed.Value(), -entry.PowerElectrical.Value());
				}
				catch (Exception e)
				{
					throw new VectoException(string.Format("EfficiencyMap - Line {0}: {1}", data.Rows.IndexOf(row), e.Message), e);
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
			return columns.Contains(FuelConsumptionMapReader.Fields.EngineSpeed) && columns.Contains(FuelConsumptionMapReader.Fields.Torque) &&
					columns.Contains(FuelConsumptionMapReader.Fields.FuelConsumption);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";
			public const string Torque = "T";
			public const string PowerElectrical = "P_el";
		}
	}
}