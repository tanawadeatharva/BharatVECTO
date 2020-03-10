using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class CompletedBusSegments : LookupData<int , VehicleCode, string , Segment>
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
			get { return "ERROR: Could not find the declaration segment for vehicle. Number of Axles: {0}, VehicleCode: {1}, VehicleParameterGroup {2}";}
		}
		
		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			return LookupCompletedBusVehicle(numberOfAxles, vehicleCode, vehicleParameterGroup);
		}
		

		#endregion


		private Segment LookupCompletedBusVehicle(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var rows = _segmentTable.AsEnumerable().Where(
				r => {
					var currentNumberOfAxles = r.Field<string>("numaxles").ToInt(0);
					var currentVehicleCode =  VehicleCodeHelper.Parse(r.Field<string>("vehiclecode"));
					var currentVehicleParam = r.Field<string>("vehicleparametergroup");

					return currentNumberOfAxles == numberOfAxles 
							&& currentVehicleCode == vehicleCode 
							&& currentVehicleParam == vehicleParameterGroup;
				}).ToList();
			if (rows.Count == 0) {
				return new Segment { Found = false };
			}

			var segment = new Segment
			{
				Found =  true,
				Missions = CreateMissions(rows)
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

					var lowEntry = row.Field<string>("lowentry");
					var bodyHeight = row.Field<string>("bodyheight");
					var passengersLowerDeck = row.Field<string>("passengerslowerdeck");
					var vehicleCode = VehicleCodeHelper.Parse(row.Field<string>("vehiclecode"));
					var vehicleParameterGroup = row.Field<string>("vehicleparametergroup");

					var mission = new Mission {
						MissionType = missionType,
						DefaultCDxA = row.ParseDouble("cdxastandard").SI<SquareMeter>(),
						AirDragMeasurement = row.ParseBoolean("airdragmeasurement"),
						BusParameter = new BusParameters {
							NumberOfAxles = row.Field<string>("numaxles").ToInt(0),
							IsArticulated = int.Parse(row.Field<string>("articulated")) != 0,
							FloorType = GetFloorType(row.Field<string>("floortype")),
							VehicleCode = vehicleCode,
							RegistrationClasses = RegistrationClassHelper.Parse(row.Field<string>("registrationclasses")),
							LowEntry = lowEntry == "-" ? (bool?)null : int.Parse(lowEntry) != 0,
							NumberPassengersLowerDeck = passengersLowerDeck == "-" ? 0 : row.ParseDouble("passengerslowerdeck"),
							PassengersSeatsLowerOrEqual = GetPassengersLowerOrEqualValue(passengersLowerDeck, vehicleParameterGroup, vehicleCode),
							BodyHeight = bodyHeight == "-" ? null : row.ParseDouble("bodyheight").SI<Meter>(),
							BodyHeightLowerOrEqual = GetBodyHeightLowerOrEqualValue(bodyHeight, vehicleParameterGroup, vehicleCode),
							VehicleParameterGroup = vehicleParameterGroup,
							PassengersHeavyUrban = row.Field<string>("heavyurban") == string.Empty ? 0 : row.ParseDouble("heavyurban"),
							PassengersUrban = row.Field<string>("urban") == string.Empty ? 0 : row.ParseDouble("urban"),
							PassengersSuburban = row.Field<string>("suburban") == string.Empty ? 0 : row.ParseDouble("suburban"),
							PassengersInterurban = row.Field<string>("interurban") == string.Empty ? 0 : row.ParseDouble("interurban"),
							PassengersCoach = row.Field<string>("coach") == string.Empty ? 0 : row.ParseDouble("coach"),
							AxleLoadDistribution = GetAxleLoadDistribution(row.Field<string>("axleloaddistribution")),
							VehicleEquipment = GetVehicleEquipment(row)
						}
					};

					missions.Add(mission);
				}
			}
			
			return missions.ToArray();
		}

		
		private bool? GetPassengersLowerOrEqualValue(string numberOfPassengers, string vehicleParameterGroup, VehicleCode vehicleCode)
		{
			if (numberOfPassengers != "-") {
				if (VehicleCodeHelper.IsDoubleDeckBus(vehicleCode)) {
					if (vehicleParameterGroup.EndsWith("e")) {
						return true;
					}
					return false;
				}
			}
			return null;
		}

		private bool? GetBodyHeightLowerOrEqualValue(string bodyHeight, string vehicleParameterGroup,
			VehicleCode vehicleCode)
		{
			if (bodyHeight != "-") {
				if (!VehicleCodeHelper.IsDoubleDeckBus(vehicleCode)) {
					if (vehicleParameterGroup.EndsWith("b")) {
						return true;
					}

					return false;
				}
			}
			return null;
		}

		private AxleLoadDistribution GetAxleLoadDistribution(string axleLoadDistribution)
		{
			return new AxleLoadDistribution(axleLoadDistribution);
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
		
		private FloorType GetFloorType(string field)
		{
			switch (field)
			{
				case "high": return FloorType.HighFloor;
				case "low": return FloorType.LowFloor;
				default: return FloorType.Unknown;
			}
		}
	}
}
