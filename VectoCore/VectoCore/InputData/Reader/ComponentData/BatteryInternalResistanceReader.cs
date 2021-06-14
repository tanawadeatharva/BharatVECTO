using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class BatteryInternalResistanceReader
	{
		public static InternalResistanceMap Create(DataTable data, int packCount)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("Internal Resistance Map data must contain exactly two columns: {0}, {1}",Fields.StateOfCharge, Fields.InternalResistance);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Internal Resistance Map data must contain at least 2 entries!");
			}

			if (!data.Columns.Contains(Fields.StateOfCharge) || !data.Columns.Contains(Fields.InternalResistance)) {
				data.Columns[0].ColumnName = Fields.StateOfCharge;
				data.Columns[1].ColumnName = Fields.InternalResistance;
				LoggingObject.Logger<InternalResistanceMap>().Warn("Internal Resistance Map Header is invalid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.StateOfCharge, Fields.InternalResistance, string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			}
			return new InternalResistanceMap(data.Rows.Cast<DataRow>().Select(row => new InternalResistanceMap.InternalResistanceMapEntry() {
				SoC = row.ParseDouble(Fields.StateOfCharge) / 100,
				Resistance = row.ParseDouble(Fields.InternalResistance).SI<Ohm>() / packCount
			}).OrderBy(e => e.SoC).ToArray());

		}

		public static class Fields
		{
			public const string StateOfCharge = "SoC";

			public const string InternalResistance = "Ri";
		}

		public static InternalResistanceMap Create(Stream data, int packCount)
		{
			return Create(VectoCSVFile.ReadStream(data), packCount);
		}
	}
}