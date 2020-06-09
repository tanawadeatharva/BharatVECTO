using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ElectricFullLoadCurveReader
	{
		public static ElectricFullLoadCurve Create(DataTable data)
		{
			if (data.Columns.Count < 3) {
				throw new VectoException("Motor FullLoadCurve Data must contain at least 3 columns");
			}
			if (data.Rows.Count < 2) {
				throw new VectoException("Motor FullLoadCurve Data must contain at least 2 rows with numeric values");
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DrivingTorque;
				data.Columns[2].ColumnName = Fields.GenerationTorque;
			}

			return new ElectricFullLoadCurve(
				(from DataRow row in data.Rows
				select new ElectricFullLoadCurve.FullLoadEntry {
					MotorSpeed = row.ParseDouble(Fields.MotorSpeed).RPMtoRad(),
					FullDriveTorque = row.ParseDouble(Fields.DrivingTorque).SI<NewtonMeter>(),
					FullGenerationTorque = row.ParseDouble(Fields.GenerationTorque).SI<NewtonMeter>()
				}).ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection dataColumns)
		{
			return dataColumns.Contains(Fields.MotorSpeed) && dataColumns.Contains(Fields.DrivingTorque) &&
					dataColumns.Contains(Fields.GenerationTorque);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";

			public const string DrivingTorque = "T_drive";

			public const string GenerationTorque = "T_recuperation";
		}
	}

	public static class ElectricMotorDragCurveReader
	{
		public static DragCurve Create(DataTable data)
		{
			if (data.Columns.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 columns");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 rows with numeric values");
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DragTorque;
			}
			return new DragCurve(data.AsEnumerable().Cast<DataRow>().Select(x => new DragCurve.DragLoadEntry() {
				MotorSpeed = x.ParseDouble(Fields.MotorSpeed).RPMtoRad(),
				DragTorque = x.ParseDouble(Fields.DragTorque).SI<NewtonMeter>()
			}).ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.MotorSpeed) && columns.Contains(Fields.DragTorque);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";

			public const string DragTorque = "T_drag";
		}
	}
}
