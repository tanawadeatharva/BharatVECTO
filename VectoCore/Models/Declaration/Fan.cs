using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class Fan : LookupData<MissionType, string, Watt>
	{
		private readonly Dictionary<Tuple<MissionType, string>, Watt> _data =
			new Dictionary<Tuple<MissionType, string>, Watt>();

		private const string DefaultTechnology = "Crankshaft mounted - Electronically controlled visco clutch (Default)";

		protected const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.VAUX.Fan-Tech.csv";

		public Fan()
		{
			ParseData(ReadCsvResource(ResourceId));
		}


		protected override void ParseData(DataTable table)
		{
			NormalizeTable(table);

			_data.Clear();
			foreach (DataRow row in table.Rows) {
				foreach (var mission in EnumHelper.GetValues<MissionType>()) {
					_data[Tuple.Create(mission, row.Field<string>("Technology"))] =
						row.ParseDouble(mission.ToString().ToLower()).SI<Watt>();
				}
			}
		}

		public override Watt Lookup(MissionType mission, string technology)
		{
			if (string.IsNullOrWhiteSpace(technology)) {
				technology = DefaultTechnology;
			}

			try {
				return _data[Tuple.Create(mission, technology)];
			} catch (KeyNotFoundException) {
				throw new VectoException("Auxiliary Lookup Error: No value found for Fan with key '{0}' in mission '{1}'",
					technology, mission);
			}
		}
	}
}