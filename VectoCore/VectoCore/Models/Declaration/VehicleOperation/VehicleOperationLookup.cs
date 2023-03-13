using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration.VehicleOperation
{
	public class VehicleOperationLookup
	{

		private MileageLookup _mileageLookup = new MileageLookup();

		private StationaryChargingDurationPerEventLookup _chargingDurationLookup =
			new StationaryChargingDurationPerEventLookup();

		private StationaryChargingPowerFromInfrastructureLookup _chargingPowerFromInfrastructureLookup =
			new StationaryChargingPowerFromInfrastructureLookup();

		private StationaryChargingEventsPerDayLookup _numberOfChargingEventsLookup =
			new StationaryChargingEventsPerDayLookup();

		private RealWorldUsageFactors _realWorldUsageFactors = new RealWorldUsageFactors();


		public VehicleOperationData LookupVehicleOperation(VehicleClass hdvClass, MissionType mission)
		{
			return new VehicleOperationData() {
				Mileage = _mileageLookup.Lookup(hdvClass, mission),
				StationaryChargingMaxPwrInfrastructure = _chargingPowerFromInfrastructureLookup.Lookup(hdvClass, mission),
				StationaryChargingDuringMission_AvgDurationPerEvent = _chargingDurationLookup.Lookup(hdvClass, mission),
				StationaryChargingDuringMission_NbrEvents = _numberOfChargingEventsLookup.Lookup(hdvClass, mission),
				RealWorldUsageFactors = _realWorldUsageFactors.Lookup(hdvClass),
			};
		}

		public class VehicleOperationData
		{
			public MileageLookup.MileageEntry Mileage { get; internal set; }

			public Watt StationaryChargingMaxPwrInfrastructure { get; internal set; }

			public Second StationaryChargingDuringMission_AvgDurationPerEvent { get; internal set; }

			public double StationaryChargingDuringMission_NbrEvents { get; internal set; }

			public RealWorldUsageFactors.Entry RealWorldUsageFactors { get; internal set; }

		}
	}
}