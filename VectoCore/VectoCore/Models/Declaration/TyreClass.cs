using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class TyreClass : LookupData
	{
		private readonly List<Entry> _data = new List<Entry>();

		public string Lookup(double rrc)
		{
			var rrcRounded = Math.Round(rrc * 1000, 1, MidpointRounding.AwayFromZero);
			var entries = _data.FindAll(x => x.RRCMin <= rrcRounded && x.RRCMax >= rrcRounded);
			return entries.Count == 0 ? "G" : entries.First().Label;
		}

		#region Overrides of LookupData

		protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".TyreLabeling.csv"; } }
		protected override string ErrorMessage { get { return "No Tyre class found for RRC {0}"; } }
		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				_data.Add(new Entry {
					RRCMin = row.ParseDouble("rrc_min"),
					RRCMax = row.ParseDouble("rrc_max"),
					Label = row.Field<string>("energyefficiencyclass")
				});
			}
		}

		#endregion

		private class Entry
		{
			public double RRCMin;
			public double RRCMax;
			public string Label;
		}
	}
}
