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

			return new PredictionDurationLookup(
				new LookupDataReader<double, double>(
						"ShiftStrategy PredictionDuration", new[] { Fields.SpeedRatio, Fields.PredictionTimeRatio })
					.Create(data,
							x => x.ParseDouble(Fields.SpeedRatio),
							y => y.ParseDouble(Fields.PredictionTimeRatio))
					.OrderBy(x => x.Key)
					.ToArray());
		}

		public static class Fields
		{
			public const string SpeedRatio = "v_post/v_curr";

			public const string PredictionTimeRatio = "dt_pred/dt_shift";
		}
	}
}
