using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{
	public class VehicleOperationLookup
	{

		private MileageLookup _mileageLookup = new MileageLookup();

		private StationaryChargingDurationPerEventLookup _chargingDurationLookup =
			new StationaryChargingDurationPerEventLookup();

		private StationaryChargingFromInfrastructureLookup _chargingFromInfrastructureLookup =
			new StationaryChargingFromInfrastructureLookup();

		private StationaryChargingEventsPerDayLookup _numberOfChargingEventsLookup =
			new StationaryChargingEventsPerDayLookup();



		public MileageLookup.MileageEntry LookupMileage(VehicleClass hdvClass, MissionType mission)
		{
			return _mileageLookup.Lookup(hdvClass.GetClassNumber(), mission);
		}

		public Second LookupChargingDurationPerEvent(VehicleClass hdvClass, MissionType mission)
		{
			return _chargingDurationLookup.Lookup(hdvClass, mission).SI(Unit.SI.Hour).Cast<Second>();
		}

		public Watt LookupMaxChargingPower(VehicleClass hdvClass, MissionType mission)
		{
			return _chargingFromInfrastructureLookup.Lookup(hdvClass, mission).SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		}

		public double LookupChargingEventsPerDay(VehicleClass hdvClass, MissionType mission)
		{
			return _numberOfChargingEventsLookup.Lookup(hdvClass, mission);
		}

	}
}