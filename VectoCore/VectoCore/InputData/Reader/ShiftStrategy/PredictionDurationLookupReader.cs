using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy
{
	public static class PredictionDurationLookupReader
	{
		public static PredictionDurationLookup ReadFromStream(Stream stream)
		{
			var data = VectoCSVFile.ReadStream(stream);
			return Create(data);
		}

		public static PredictionDurationLookup ReadFromFile(string filename)
		{
			try {
				var data = VectoCSVFile.Read(filename);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException("Error while reading ShiftStrategy PredictionDurationLookup File: " + e.Message);
			}
		}

		private static PredictionDurationLookup Create(TableData data)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("ShiftStrategy PredictionDuration must consist of 2 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("ShiftStrategy PredictionDuration must consist of at least two rows.");
			}

			if (!HeaderIsValid(data.Columns)) {
				LoggingObject.Logger<PredictionDurationLookup>()
							.Warn(
								"ShiftStrategy PredictionDuration Curve: Header Line is not valid. Expected: '{0}, {1}', Got: {3}",
								Fields.SpeedRatio, Fields.PredictionTimeRatio,
								string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

				data.Columns[0].Caption = Fields.SpeedRatio;
				data.Columns[1].Caption = Fields.PredictionTimeRatio;
			}

			return new PredictionDurationLookup(
				data.Rows.Cast<DataRow>()
					.Select(
						r => new KeyValuePair<double, double>(
							r.ParseDouble(Fields.SpeedRatio),
							r.ParseDouble(Fields.PredictionTimeRatio)
						))
					.OrderBy(x => x.Key)
					.ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.SpeedRatio) &&
					columns.Contains(Fields.PredictionTimeRatio);
		}

		public static class Fields
		{
			public const string SpeedRatio = "v_post/v_curr";

			public const string PredictionTimeRatio = "dt_pred/dt_shift";
		}
	}
}
