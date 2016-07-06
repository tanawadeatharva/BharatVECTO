using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	internal sealed class Payloads : LookupData<Kilogram, Payloads.PayloadEntry>
	{
		internal sealed class PayloadEntry
		{
			public Kilogram Payload50Percent;
			public Kilogram Payload75Percent;
		}

		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.Payloads.csv";

		public Payloads()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		[Obsolete("Call Lookup50Percent, Lookup75Percent or LookupTrailer!", true)]
		private new PayloadEntry Lookup(Kilogram grossVehicleWeight)
		{
			throw new NotImplementedException("Call Lookup50Percent, Lookup75Percent or LookupTrailer!");
		}

		public Kilogram Lookup50Percent(Kilogram grossVehicleWeight)
		{
			var section = Data.GetSection(d => d.Key > grossVehicleWeight);
			return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key,
				section.Item1.Value.Payload50Percent, section.Item2.Value.Payload50Percent,
				grossVehicleWeight);
		}

		public Kilogram Lookup75Percent(Kilogram grossVehicleWeight)
		{
			var section = Data.GetSection(d => d.Key > grossVehicleWeight);
			return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key,
				section.Item1.Value.Payload75Percent, section.Item2.Value.Payload75Percent,
				grossVehicleWeight);
		}

		public Kilogram LookupTrailer(Kilogram grossVehicleWeight, Kilogram curbWeight)
		{
			return 0.75 * (grossVehicleWeight - curbWeight);
		}

		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);
			Data = table.Rows.Cast<DataRow>()
				.ToDictionary(
					kv => kv.ParseDouble("grossvehicleweight").SI<Kilogram>(),
					kv => new PayloadEntry {
						Payload50Percent = kv.ParseDouble("payload50%").SI<Kilogram>(),
						Payload75Percent = kv.ParseDouble("payload75%").SI<Kilogram>()
					});
		}
	}
}