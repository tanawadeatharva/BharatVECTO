using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{


	public abstract class StationaryChargingLookup : LookupData<string, MissionType, double>
	{

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow tableRow in table.Rows) {
				var group = tableRow.Field<string>("vehiclegroup");
				foreach (DataColumn col in table.Columns.Cast<DataColumn>().Skip(table.Columns.IndexOf("longhaul"))) {
					MissionType mission = col.Caption.ParseEnum<MissionType>();
					if (tableRow.Field<string>(col) == "---") {
						continue;
					}

					double val = tableRow.ParseDouble(col);
					Data.Add(Tuple.Create<string, MissionType>(group, mission), val);
				}
			}
		}

		public double Lookup(VehicleClass hdvClass, MissionType mission)
		{
			return Lookup(hdvClass.GetClassNumber(), mission);
		}
	}


	public class StationaryChargingDurationPerEventLookup : StationaryChargingLookup
	{
		#region Overrides of LookupData

		protected override string ResourceId =>
			"TUGraz.VectoCore.Resources.Declaration.VehicleOperation.StationaryChargingDuration.csv";

		protected override string ErrorMessage => "Error looking up stationary charging duration per event";

		#endregion


	}



	public class StationaryChargingFromInfrastructureLookup : StationaryChargingLookup
	{
		#region Overrides of LookupData

		protected override string ResourceId =>
			"TUGraz.VectoCore.Resources.Declaration.VehicleOperation.StationaryChargingPower.csv";

		protected override string ErrorMessage => "Error looking up stationary charging power from infrastructure";

		#endregion
	}


	public class StationaryChargingEventsPerDayLookup : StationaryChargingLookup
	{
		#region Overrides of LookupData

		protected override string ResourceId =>
			"TUGraz.VectoCore.Resources.Declaration.VehicleOperation.ChargingEventDuringMission.csv";

		protected override string ErrorMessage => "Error looking up Number of charging events during mission";

		#endregion
	}

}