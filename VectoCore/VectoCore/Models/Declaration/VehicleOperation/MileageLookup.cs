using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{
    public class MileageLookup : LookupData<string, MissionType, MileageLookup.MileageEntry>
    {
		#region Overrides of LookupData
		protected override string ResourceId => "TUGraz.VectoCore.Resources.Declaration.VehicleOperation.AnnualMileage.csv";
		protected override string ErrorMessage => "Error looking up mileage";

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow tableRow in table.Rows) {
				var group = tableRow.Field<string>("vehiclegroup");
				var workingDays = tableRow.ParseDouble("workingdaysperyear");
				foreach (DataColumn col in table.Columns.Cast<DataColumn>().Skip(table.Columns.IndexOf("workingdaysperyear") + 1)) {
					MissionType mission = col.Caption.ParseEnum<MissionType>();
					if (tableRow.Field<string>(col) == "---") {
						continue;
					}
					var annualMileage = tableRow.ParseDouble(col).SI(Unit.SI.Kilo.Meter).Cast<Meter>();
					Data.Add(Tuple.Create<string, MissionType>(group, mission),
						new MileageEntry() {
							annualMileage = annualMileage,
							workingDaysPerYear = workingDays,
							dailyMileage = annualMileage / workingDays,
						});
				}
			}
		}
		#endregion






		public struct MileageEntry
		{
			public Meter annualMileage;
			public double workingDaysPerYear;
			public Meter dailyMileage;
		}
	}
}
