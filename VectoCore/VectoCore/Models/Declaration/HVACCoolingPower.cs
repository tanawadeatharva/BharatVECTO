using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration {
	public class HVACCoolingPower
	{
		private static HVACLookup DriverCoolingPower = new HVACLookup(".Buses.HVACCoolingPowerDriver");
		private static HVACLookup PassengerCoolingPower = new HVACLookup(".Buses.HVACCoolingPowerPassenger");

		public Watt DriverMaxCoolingPower(BusHVACSystemConfiguration configuration, MissionType mission)
		{
			return DriverCoolingPower.Lookup(configuration, mission).SI<Watt>();
		}

		public Watt PassengerMaxCoolingPower(BusHVACSystemConfiguration configuration, MissionType mission, CubicMeter volume)
		{
			return PassengerCoolingPower.Lookup(configuration, mission).SI<WattPerCubicMeter>() * volume;
		}


		private class HVACLookup : LookupData<BusHVACSystemConfiguration, MissionType, double>{
			public HVACLookup(string resource)
			{
				ResourceId = DeclarationData.DeclarationDataResourcePrefix + resource;
			}

			#region Overrides of LookupData

			protected override string ResourceId { get; }
			protected override string ErrorMessage { get { return "No entry found for configuration {0}, mission {1}"; } }
			protected override void ParseData(DataTable table)
			{
				var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>().Where(
					m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
						table.Columns.Contains(m.ToString())).ToList();
				foreach (DataRow row in table.Rows) {
					foreach (var missionType in missionTypes) {
						Data.Add(Tuple.Create(BusHVACSystemConfigurationHelper.Parse(row.Field<string>("configuration")), 
							missionType), row.Field<double>(missionType.GetLabel()) * 1000);
					}
				}
			}

			#endregion
		}
	}
}