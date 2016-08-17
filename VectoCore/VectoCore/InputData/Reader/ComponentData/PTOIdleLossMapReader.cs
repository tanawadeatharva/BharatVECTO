using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class PTOIdleLossMapReader
	{
		/// <summary>
		/// Read the retarder loss map from a file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static PTOLossMap ReadFromFile(string fileName)
		{
			try {
				return Create(VectoCSVFile.Read(fileName));
			} catch (Exception ex) {
				throw new VectoException("ERROR while loading PTO Idle LossMap: " + ex.Message);
			}
		}

		/// <summary>
		/// Create the pto idle loss map from an appropriate datatable. (2 columns: Engine Speed, PTO Torque)
		/// </summary>
		public static PTOLossMap Create(DataTable data)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("PTO Idle LossMap Data File must consist of 2 columns: {0}, {1}", Fields.EngineSpeed,
					Fields.PTOTorque);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("PTO Idle LossMap must contain at least 2 entries.");
			}

			if (!data.Columns.Contains(Fields.EngineSpeed) || !data.Columns.Contains(Fields.PTOTorque)) {
				data.Columns[0].ColumnName = Fields.EngineSpeed;
				data.Columns[1].ColumnName = Fields.PTOTorque;
				LoggingObject.Logger<RetarderLossMap>().Warn(
					"PTO Idle LossMap: Header Line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.EngineSpeed, Fields.PTOTorque, ", ".Join(data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}

			return new PTOLossMap(data.Rows.Cast<DataRow>()
				.Select(row => new PTOLossMap.Entry {
					EngineSpeed = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					PTOTorque = row.ParseDouble(Fields.PTOTorque).SI<NewtonMeter>()
				}).OrderBy(e => e.EngineSpeed).ToArray());
		}

		private static class Fields
		{
			/// <summary>
			///     [rpm]
			/// </summary>
			public const string EngineSpeed = "Engine Speed";

			/// <summary>
			///     [Nm]
			/// </summary>
			public const string PTOTorque = "PTO Torque";
		}
	}
}