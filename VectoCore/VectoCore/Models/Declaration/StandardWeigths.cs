using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class StandardWeigths : LookupData<string, StandardWeight>
	{
		private const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.StandardWeights.csv";

		public StandardWeigths()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public override StandardWeight Lookup(string id)
		{
			try {
				return Data[id];
			} catch (KeyNotFoundException) {
				throw new VectoException("StandardWeigths Lookup Error: No value found for ID '{0}'", id);
			}
		}

		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);

			Data = table.Rows.Cast<DataRow>()
				.ToDictionary(
					kv => kv.Field<string>("name"),
					kv => new StandardWeight {
						CurbWeight = kv.ParseDoubleOrGetDefault("curbweight").SI<Kilogram>(),
						GrossVehicleWeight = kv.ParseDoubleOrGetDefault("gvw").SI<Kilogram>()
					});
		}
	}

	public sealed class StandardWeight
	{
		public Kilogram CurbWeight;
		public Kilogram GrossVehicleWeight;
	}
}