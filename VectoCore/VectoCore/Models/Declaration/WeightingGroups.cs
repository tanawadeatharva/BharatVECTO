using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum WeightingGroup
	{
		Group4UD = 1,
		Group4RD,
		Group4LH,
		Group5RD,
		Group5LH,
		Group9RD,
		Group9LH,
		Group10RD,
		Group10LH,
		Unknown
	}

	public class WeightingGroupHelper
	{
		public const string Prefix = "Group";
		public static WeightingGroup Parse(string groupStr)
		{
			return (Prefix + groupStr.Replace("-", "")).ParseEnum<WeightingGroup>();
		}
	}

	public class WeightingGroups : LookupData<VehicleClass, bool, Watt, WeightingGroup>
	{
		protected readonly List<Entry> Entries = new List<Entry>();

		#region Overrides of LookupData

		protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".CO2Standards.WeightingGroups.csv"; } }
		protected override string ErrorMessage { get {
			return "WeightingGroup Lookup Error: no entry found for group {0}, sleeper cab: {1}, engine rated power {2}";
		} }
		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				Entries.Add(new Entry() {
					VehicleGroup = VehicleClassHelper.Parse(row.Field<string>("vehiclegroup")),
					SleeperCab = "SleeperCab".Equals(row.Field<string>("cabintype"), StringComparison.InvariantCultureIgnoreCase),
					RatedPowerMin = row.ParseDouble("engineratedpowermin").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					RatedPowerMax = row.ParseDouble("engineratedpowermax").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					WeightingGroup = WeightingGroupHelper.Parse(row.Field<string>("weightinggroup"))
				});
			}
		}


		public override WeightingGroup Lookup(VehicleClass group, bool sleeperCab, Watt engineRatedPower)
		{
			var rows = Entries.FindAll(
				x => x.VehicleGroup == group && x.SleeperCab == sleeperCab && engineRatedPower >= x.RatedPowerMin &&
					engineRatedPower < x.RatedPowerMax);
			return rows.Count == 0 ? WeightingGroup.Unknown : rows.First().WeightingGroup;
		}
		#endregion

		protected class Entry
		{
			public VehicleClass VehicleGroup;
			public bool SleeperCab;
			public Watt RatedPowerMin;
			public Watt RatedPowerMax;
			public WeightingGroup WeightingGroup;
		}
	}
}
