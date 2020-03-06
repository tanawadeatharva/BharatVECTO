using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class CompletedBusSegments : LookupData<int , VehicleCode,string , Segment>
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
			get { return "ERROR: Could not find the declaration segment for vehicle. Number of Axels: {0}, VehicleCode: {1}";}
		}
		
		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{

			return LookupCompletedBusVehicle(numberOfAxles, vehicleCode, vehicleParameterGroup);

			//if (primaryVehicle)
			//{
			//	//return LookupCompleteVehicle(vehicleCategory, axleConfiguration, articulated);
			//}

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

			var firstRow = rows.First();
			var segment = new Segment
			{
				Found =  true,
				Missions = CreateMissions(rows),
				
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

					var lowEntry = row.Field<string>("lowentry");
					var bodyHeight = row.Field<string>("bodyheight");

					var mission = new Mission {
						MissionType = missionType,
						DefaultCDxA = row.ParseDouble("cdxastandard").SI<SquareMeter>(),
						BusParameter = new BusParameters {
							NumberOfAxles = row.Field<string>("numaxles").ToInt(0),
							VehicleCode = VehicleCodeHelper.Parse(row.Field<string>("vehiclecode")),
							BodyHeight =  bodyHeight == "-" ?null : row.ParseDouble("bodyheight").SI<Meter>(),
							RegistrationClasses = RegistrationClassHelper.Parse(row.Field<string>("registrationclasses")),
							LowEntry = lowEntry == "-" ? (bool?)null : int.Parse(lowEntry) != 0,
							//Passengers lower Deck ?!? 
							//Body Height?!?
							VehicleParameterGroup = row.Field<string>("vehicleparametergroup"),
							PassengersHeavyUrban = row.Field<string>("heavyurban") == string.Empty ? 0 : row.ParseDouble("heavyurban"),
							PassengersUrban = row.Field<string>("urban") == string.Empty ? 0 : row.ParseDouble("urban"),
							PassengersSuburban = row.Field<string>("suburban") == string.Empty ? 0 : row.ParseDouble("suburban"),
							PassengersInterurban = row.Field<string>("interurban") == string.Empty ? 0 : row.ParseDouble("interurban"),
							PassengersCoach = row.Field<string>("coach") == string.Empty ? 0 : row.ParseDouble("coach"),
							AirDragMeasurement = row.ParseBoolean("airdragmeasurement")
						}
					};

					missions.Add(mission);
				}
			}
			
			return missions.ToArray();
		}



	}
}
