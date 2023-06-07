using System.Data;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{
	public class RealWorldUsageFactors : LookupData<string, RealWorldUsageFactors.Entry>
	{
		#region Overrides of LookupData

		protected override string ResourceId => "TUGraz.VectoCore.Resources.Declaration.VehicleOperation.RealWorldUsageFactors.csv";
		protected override string ErrorMessage => "Error looking up RealWorld Usage Factors";
		protected override void ParseData(DataTable table)
		{
			foreach (DataRow tableRow in table.Rows) {
				var group = tableRow.Field<string>("vehiclegroup");
				var startSoCBeforeMission = tableRow.ParseDouble("startsocbeforemission");
				var stationaryChargingDuringMission = tableRow.ParseDouble("stationarychargingduringmission");
				foreach (var g in group.Split('/')) {
					Data.Add(g.RemoveWhitespace(), new Entry() {
						StartSoCBeforeMission = startSoCBeforeMission,
						StationaryChargingDuringMission = stationaryChargingDuringMission,
					});
				}
			}
		}

		#endregion

		public Entry Lookup(VehicleClass hdvClass)
		{
			return Lookup(hdvClass.GetClassNumberWithoutSubSuffix());
		}

		public struct Entry
		{
			public double StartSoCBeforeMission { get; internal set; }

			public double StationaryChargingDuringMission { get; internal set; }
		}

	}
}