using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class CompletedBusSegments : LookupData<int , VehicleCode, RegistrationClass, int, Meter, bool, Segment>
	{
		private const string COMPLETED_BUS_SEGMENTS_CSV = ".CompletedBusSegmentationTable.csv";

		private DataTable _segmentTable;

		#region  Overrides of LookupData


		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + COMPLETED_BUS_SEGMENTS_CSV; }
		}
		
		protected override string ErrorMessage
		{
			get { return "ERROR: Could not find the declaration segment for vehicle. numberOfAxles: {0}, vehicleCode: {1}, registrationClass: {2}, " +
						"passengersLowerDeck: {3}, bodyHeight: {4} , lowEntry: {5}";
			}
		}
		
		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(int numberOfAxles, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, Meter bodyHeight, bool lowEntry)
		{
			return LookupCompletedBusVehicle(numberOfAxles, vehicleCode, registrationClass, passengersLowerDeck, bodyHeight, lowEntry);
		}
		

		#endregion


		private Segment LookupCompletedBusVehicle(int numberOfAxles, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, Meter bodyHeight, bool lowEntry)
		{
			var rows = _segmentTable.AsEnumerable().Where(
				r => {
					var currentNumberOfAxles = r.Field<string>("numaxles").ToInt(0);
					var currentVehicleCode =  VehicleCodeHelper.Parse(r.Field<string>("vehiclecode"));
					var registrationClasses = RegistrationClassHelper.Parse(r.Field<string>("registrationclasses"));

					return  currentNumberOfAxles == numberOfAxles 
							&& currentVehicleCode == vehicleCode && registrationClasses.Contains(registrationClass);
				}).ToList();
			if (rows.Count == 0) {
				return new Segment { Found = false };
			}

			if (rows.Count > 1) {
				if (rows.Any(r => r.Field<string>("passengerslowerdeck") != "-")) {
					rows = rows.Where(
						r => {
							var limits = r.Field<string>("passengerslowerdeck").Split('-');
							return passengersLowerDeck.IsBetween(limits[0].ToInt(), limits[1].ToInt());
						}).ToList();
				} else if (rows.Any(r => r.Field<string>("bodyheight") != "-")) {
					rows = rows.Where(
						r => {
							var limits = r.Field<string>("bodyheight").Split('-');
							return bodyHeight > limits[0].ToDouble().SI<Meter>() && bodyHeight <= limits[1].ToDouble().SI<Meter>();
						}).ToList();
				} else if (rows.All(r => r.Field<string>("lowentry") != "-")) {
					rows = rows.Where(
						r => {
							var isLowEntry = r.Field<string>("lowentry") == "1";
							return isLowEntry == lowEntry;
						}).ToList();
				} else {
					throw new VectoException("Multiple segments found! {0}", rows.Count);
				}
			}
			
			var row = rows.First();
			if(!row.ParseBoolean("isvalid"))
				throw new VectoException("Only invalid vehicles found!");
			
			var segment = new Segment {
				Found =  true,
				Missions = CreateMissions(rows), 
				VehicleClass = VehicleClassHelper.Parse("CB" + row.Field<string>("vehicleparametergroup")),
			};

			return segment;
		}
		

		private Mission[] CreateMissions(List<DataRow> rows)
		{
			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>().Where(
				m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
					rows.First().Table.Columns.Contains(m.ToString())).ToList();
			
			var missions = new List<Mission>();

			foreach (var row in rows) {

				foreach (var missionType in missionTypes) {
					if (string.IsNullOrWhiteSpace(row.Field<string>(missionType.ToString()))){
						continue;
					}
					
					var mission = new Mission {
						MissionType = missionType,
						MinLoad = null,
						MaxLoad = null,
						RefLoad = 100.SI<Kilogram>(), // dummy value to trigger simulation with ref load
						LowLoad = 10.SI<Kilogram>(), // dummy value to trigger simulation with low load
						AxleWeightDistribution = GetAxleWeightDistribution(row),
						DefaultCDxA = row.ParseDouble("cdxastandard").SI<SquareMeter>(),
						BusParameter = new BusParameters {
							PassengerDensity = row.ParseDouble(missionType.ToString()).SI<PerSquareMeter>(),
							AirDragMeasurementAllowed = row.ParseBoolean("airdragmeasurement"),
							VehicleEquipment = GetVehicleEquipment(row),
							DoubleDecker =  VehicleCodeHelper.Parse(row.Field<string>("vehiclecode")).IsDoubleDeckBus()
						}
					};

					missions.Add(mission);
				}
			}
			
			return missions.ToArray();
		}

		private double[] GetAxleWeightDistribution(DataRow row)
		{
			var axleDistribution = row.Field<string>("axleloaddistribution");
			if (string.IsNullOrWhiteSpace(axleDistribution)) {
				return new double[] { };
			}

			return axleDistribution.Split('/').ToDouble().Select(x => x / 100.0).ToArray();
		}


		private VehicleEquipment GetVehicleEquipment(DataRow row)
		{
			var externalDisplays = row.Field<string>("externaldisplays") == string.Empty
				? (double?)null
				: row.ParseDouble("externaldisplays");

			var internalDisplays = row.Field<string>("internaldisplays") == string.Empty
				? (double?)null
				: row.ParseDouble("internaldisplays");

			var fridge = row.Field<string>("fridge") == string.Empty
				? (double?)null
				: row.ParseDouble("fridge");

			var kitchenStandard = row.Field<string>("kitchenStandard") == string.Empty
				? (double?)null
				: row.ParseDouble("kitchenStandard");

			return new VehicleEquipment {
				ExternalDisplays = externalDisplays,
				InternalDisplays = internalDisplays,
				Fridge = fridge,
				KitchenStandard = kitchenStandard
			};
		}
	}
}
