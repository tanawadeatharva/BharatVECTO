using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class WHRPowerReader
	{
		public static WHRPowerMap ReadFromFile(string fileName, WHRType type)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data, type);
			} catch (Exception e) {
				throw new VectoException(string.Format("File {0}: {1}", fileName, e.Message), e);
			}
		}

		public static WHRPowerMap Create(TableData data, WHRType type)
		{
			string whrColumn = null;
			type = type & (WHRType.ElectricalOutput | WHRType.MechanicalOutputDrivetrain);
			switch (type) {
				case WHRType.MechanicalOutputDrivetrain: 
					whrColumn = Fields.MechanicalPower;
					break;
				case WHRType.ElectricalOutput:
					whrColumn = Fields.ElectricPower;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

			var headerValid = HeaderIsValid(data.Columns, whrColumn);
			if (!headerValid) {
				LoggingObject.Logger<FuelConsumptionMap>().Warn(
					"WHRMap: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}",
					Fields.EngineSpeed, Fields.Torque, Fields.ElectricPower,
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}

			if (!headerValid) {
				if (data.SourceType == DataSourceType.CSVFile && data.Columns.Count < 4) {
					throw new VectoException("FC-Map has to contain at least 4 columns when WHR is used");
				}

				data.Columns[0].ColumnName = Fields.EngineSpeed;
				data.Columns[1].ColumnName = Fields.Torque;

				// column with idx==2 is fuel consumption in csv files
				if (type == WHRType.ElectricalOutput) {
					data.Columns[3].ColumnName = Fields.ElectricPower;
				}
				if (type == WHRType.MechanicalOutputDrivetrain) {
					if (data.Columns.Count > 3) {
						data.Columns[4].ColumnName = Fields.MechanicalPower;
					} else {
						data.Columns[3].ColumnName = Fields.MechanicalPower;
					}
				}
			}

			var delaunayMap = new DelaunayMap(type.IsElectrical() ? "WHRMapEl" : "WHRMapMech");
			
			foreach (DataRow row in data.Rows) {
				try {
					var engineSpeed = row.ParseDouble(Fields.EngineSpeed).RPMtoRad();
					var torque = row.ParseDouble(Fields.Torque).SI<NewtonMeter>();
					var electricPower =  row.ParseDouble(whrColumn).SI<Watt>();

					delaunayMap?.AddPoint(torque.Value(),engineSpeed.Value(),electricPower.Value());
				} catch (Exception e) {
					throw new VectoException(string.Format("WHR Map - Line {0}: {1}", data.Rows.IndexOf(row), e.Message), e);
				}
			}

			delaunayMap.Triangulate();
			
			return new WHRPowerMap(delaunayMap);
		}

		private static bool HeaderIsValid(DataColumnCollection columns, string whrColumn)
		{
			return columns.Contains(Fields.EngineSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(whrColumn);
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
			/// [W]
			/// </summary>
			public const string ElectricPower = "whr power electric";

			/// <summary>
			/// [W]
			/// </summary>
			public const string MechanicalPower = "whr power mechanical";
		}
	}
}
