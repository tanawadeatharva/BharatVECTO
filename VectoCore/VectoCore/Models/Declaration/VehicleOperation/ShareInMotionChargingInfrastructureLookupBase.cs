using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{

	public class ShareInMotionChargingInfrastructureLookup
	{
		protected CatenaryShareInMotionChargingInfrastructureLookup _catenaryLookup =
			new CatenaryShareInMotionChargingInfrastructureLookup();

		protected TrolleyShareInMotionChargingInfrastructureLookup _trolleyLookup =
			new TrolleyShareInMotionChargingInfrastructureLookup();

		protected GroundRailWirelessShareInMotionChargingInfrastructureLookup _grWirelessLookup =
			new GroundRailWirelessShareInMotionChargingInfrastructureLookup();

		public Entry Lookup(VehicleClass hdvClass, MissionType missionType)
		{
			return new Entry {
				ShareCatenary = _catenaryLookup.Lookup(hdvClass, missionType),
				ShareTrolley = _trolleyLookup.Lookup(hdvClass, missionType),
				ShareGroundRail = _grWirelessLookup.Lookup(hdvClass, missionType),
				ShareWireless = _grWirelessLookup.Lookup(hdvClass, missionType)
			};
		}

		public class Entry
		{
			public double ShareCatenary;
			public double ShareTrolley;
			public double ShareGroundRail;
			public double ShareWireless;
		}
	}



    public abstract class ShareInMotionChargingInfrastructureLookupBase : LookupData<string, MissionType, double>
	{
		protected override void ParseData(DataTable table)
		{
			foreach (DataRow tableRow in table.Rows) {
				var group = tableRow.Field<string>("vehiclegroup");
				foreach (DataColumn col in table.Columns.Cast<DataColumn>().Skip(table.Columns.IndexOf("longhaul"))) {
					var mission = col.Caption.ParseEnum<MissionType>();
					if (tableRow.Field<string>(col) == "---") {
						continue;
					}

					var val = tableRow.ParseDouble(col);
					foreach (var g in group.Split('/')) {
						Data.Add(Tuple.Create<string, MissionType>(g.RemoveWhitespace(), mission), val);
					}
				}
			}
		}

		public double Lookup(VehicleClass hdvClass, MissionType mission)
		{
			return Lookup(hdvClass.GetClassNumberWithoutSubSuffix(), mission.GetNonEMSMissionType());
		}

    }

	public class CatenaryShareInMotionChargingInfrastructureLookup : ShareInMotionChargingInfrastructureLookupBase
	{
		#region Overrides of LookupData

		protected override string ResourceId => "TUGraz.VectoCore.Resources.Declaration.VehicleOperation.InMotionShareCatenary.csv";

		protected override string ErrorMessage => "Error looking up share of in-motion charging infrastructure Catenary {0} {1}";

        #endregion
    }

	public class TrolleyShareInMotionChargingInfrastructureLookup : ShareInMotionChargingInfrastructureLookupBase
	{
		#region Overrides of LookupData

		protected override string ResourceId => "TUGraz.VectoCore.Resources.Declaration.VehicleOperation.InMotionShareTrolley.csv";

		protected override string ErrorMessage => "Error looking up share of in-motion charging infrastructure Trolley {0} {1}";

		#endregion
	}

	public class GroundRailWirelessShareInMotionChargingInfrastructureLookup : ShareInMotionChargingInfrastructureLookupBase
	{
		#region Overrides of LookupData

		protected override string ResourceId => "TUGraz.VectoCore.Resources.Declaration.VehicleOperation.InMotionShareGroundRailWireless.csv";

		protected override string ErrorMessage => "Error looking up share of in-motion charging infrastructure Ground Rail / Wireless {0} {1}";

		#endregion
	}

}
