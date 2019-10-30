using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ElectricConsumerReader
	{
		public static readonly string[] Header = new[] {
			Fields.Category, Fields.Consumer, Fields.BaseVehicle, Fields.NominalAmps, Fields.PhaseIdle, Fields.NuminVehicle,
			Fields.Info
		};

		public static IElectricalConsumerList ReadStream(Stream str)
		{
			return Create(VectoCSVFile.ReadStream(str));
		}

		public static IElectricalConsumerList Create(TableData data)
		{
			if (!HeaderValid(data.Columns)) {
				throw new VectoException("Invalid header. Expected: {0}, got: {1}",
					string.Join(", ", Header),
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}

			var retVal = new List<IElectricalConsumer>();
			foreach (DataRow row in data.Rows) {
				retVal.Add(new ElectricalConsumer(
					row.ParseBoolean(Fields.BaseVehicle),
					row.Field<string>(Fields.Category),
					row.Field<string>(Fields.Consumer),
					row.ParseDouble(Fields.NominalAmps).SI<Ampere>(),
					row.ParseDouble(Fields.PhaseIdle),
					Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
					0,
					row.Field<string>(Fields.Info)
					));
			}

			return new ElectricalConsumerList( retVal);
		}

		private static bool HeaderValid(DataColumnCollection dataColumns)
		{
			return Header.All(h => dataColumns.Contains(h));
		}

		public static class Fields
		{
			public const string Category = "Category";
			public const string Consumer = "Consumer";
			public const string BaseVehicle = "Base Vehicle";
			public const string NominalAmps = "Nominal Amps";
			public const string PhaseIdle = "PhaseIdle/TractionOn";
			public const string NuminVehicle = "Num in Vehicle";
			public const string Info = "Info";

		}
	}
}
