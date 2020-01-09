using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ActuationsMapReader
	{
		public static readonly string[] Header = new[] { Fields.ConsumerName, Fields.CycleName, Fields.Actuations };

		public static IActuationsMap Read(string fileName)
		{
			return Create(VectoCSVFile.Read(fileName), Path.GetFullPath(fileName));
		}

		public static IActuationsMap ReadStream(Stream str)
		{
			return Create(VectoCSVFile.ReadStream(str), null);
		}

		public static IActuationsMap Create(DataTable data, string source)
		{
			if (!HeaderIsValid(data.Columns)) {
				throw new VectoException("Invalid header for pneumatic actuations. expected: {0}, got: {1}",
					string.Join(", ", Header),
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}

			var retVal = new Dictionary<ActuationsKey, int>();
			foreach (DataRow row in data.Rows) {
				var key = new ActuationsKey(row.Field<string>(Fields.ConsumerName), row.Field<string>(Fields.CycleName));
				if (retVal.ContainsKey(key)) {
					throw new VectoException("Duplicate entries in pneumatic actuations map! {0} / {1}", key.ConsumerName, key.CycleName);
				}

				retVal[key] = row.Field<string>(Fields.Actuations).ToInt();
			}
			return new ActuationsMap(retVal, source);
		}

		private static bool HeaderIsValid(DataColumnCollection cols)
		{
			return Header.All(x => cols.Contains(x));
		}

		public class Fields
		{
			public const string ConsumerName = "ConsumerName";
			public const string CycleName = "CycleName";
			public const string Actuations = "Actuations";
		}

	}
}
