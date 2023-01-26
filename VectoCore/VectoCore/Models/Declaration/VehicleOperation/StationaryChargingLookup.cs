using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{


	public abstract class StationaryChargingLookup<T> : LookupData<string, MissionType, double>
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

		public T Lookup(VehicleClass hdvClass, MissionType mission)
		{
			return ConvertValue(Lookup(hdvClass.GetClassNumber(), mission));
		}

		protected abstract T ConvertValue(double val);
	}


	public class StationaryChargingDurationPerEventLookup : StationaryChargingLookup<Second>
	{
		#region Overrides of LookupData

		protected override string ResourceId =>
			"TUGraz.VectoCore.Resources.Declaration.VehicleOperation.StationaryChargingDuration.csv";

		protected override string ErrorMessage => "Error looking up stationary charging duration per event";

		#endregion


		#region Overrides of StationaryChargingLookup<Second>

		protected override Second ConvertValue(double val)
		{
			return val.SI(Unit.SI.Hour).Cast<Second>();
		}

		#endregion
	}



	public class StationaryChargingPowerFromInfrastructureLookup : StationaryChargingLookup<Watt>
	{
		#region Overrides of LookupData

		protected override string ResourceId =>
			"TUGraz.VectoCore.Resources.Declaration.VehicleOperation.StationaryChargingPower.csv";

		protected override string ErrorMessage => "Error looking up stationary charging power from infrastructure";

		#endregion

		#region Overrides of StationaryChargingLookup<Watt>

		protected override Watt ConvertValue(double val)
		{
			return val.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		}

		#endregion
	}


	public class StationaryChargingEventsPerDayLookup : StationaryChargingLookup<double>
	{
		#region Overrides of LookupData

		protected override string ResourceId =>
			"TUGraz.VectoCore.Resources.Declaration.VehicleOperation.ChargingEventDuringMission.csv";

		protected override string ErrorMessage => "Error looking up Number of charging events during mission";

		#endregion

		#region Overrides of StationaryChargingLookup<double>

		protected override double ConvertValue(double val)
		{
			return val;
		}

		#endregion
	}

}