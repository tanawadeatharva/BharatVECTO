using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration {
	public sealed class BusSegments : LookupData<VehicleCategory, AxleConfiguration, bool, FloorType, bool, bool, Segment>
	{
		private DataTable _segmentTable;

		
		#region Overrides of LookupData

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".HeavyBusSegmentationTable.csv"; }
		}

		protected override string ErrorMessage {
			get {
				return
					"ERROR: Could not find the declaration segment for vehicle. Category: {0}, AxleConfiguration: {1}, GrossVehicleWeight: {2}";
			}
		}
		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration, bool articulated, FloorType entrance, bool doubleDecker, bool primaryVehicle)
		{
			if (primaryVehicle) {
				return LookupPrimaryVehicle(vehicleCategory, axleConfiguration, articulated);
			}
			throw new NotImplementedException("Completed Vechiles not implemented");
		}

		#endregion

		private Segment LookupPrimaryVehicle(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration, bool articulated)
		{
			var rows = _segmentTable.AsEnumerable().Where(
				r => {
					var productionState = r.Field<string>("productionstage").ToInt(0);
					var articulatedStr = r.Field<string>("articulated");
					var articulatedB = articulatedStr == "-" ? (bool?)null : int.Parse(articulatedStr) != 0;
					var numAxles = r.Field<string>("numaxles").ToInt(0);
					return productionState == 1 &&
							(!articulatedB.HasValue || articulatedB == articulated) &&
							axleConfiguration.NumAxles() == numAxles;
				}).ToList();
			if (rows.Count == 0) {
				return new Segment() {Found = false};
			}
			var firstRow = rows.First();
			var segment = new Segment {
				Found = true,
				GrossVehicleWeightMin = 0.SI<Kilogram>(),
				GrossVehicleWeightMax = firstRow.ParseDouble("tpmlm_max").SI(Unit.SI.Ton).Cast<Kilogram>(),
				VehicleCategory = vehicleCategory,
				AxleConfiguration = axleConfiguration,
				VehicleClass = VehicleClassHelper.Parse(firstRow.Field<string>("hdvsupergroup")),
				AccelerationFile =
					RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
												firstRow.Field<string>(".vaccfile")),
				Missions = CreateMissions(rows),
				DesignSpeed = firstRow.ParseDouble("designspeed").KMPHtoMeterPerSecond(),
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
					if (string.IsNullOrWhiteSpace(row.Field<string>(missionType.ToString()))) {
						continue;
					}
					var busArea = (row.ParseDouble("length").SI<Meter>() - Constants.BusParameters.DriverCompartmentLength) *
								row.ParseDouble("width").SI<Meter>();
					var passengerDensity = row.ParseDouble(missionType.ToString()).SI<PerSquareMeter>();
					var passengerCount = (busArea * passengerDensity).Value();
					var refLoad = passengerCount * missionType.GetAveragePassengerMass();
					var mission = new Mission {
						MissionType = missionType,
						CrossWindCorrectionParameters = row.Field<string>("crosswindcorrection"),
						CycleFile =
							RessourceHelper.ReadStream(
								DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
								missionType.ToString().Replace("EMS", "") +
								Constants.FileExtensions.CycleFile),
						AxleWeightDistribution = GetAxleWeightDistribution(row),
						CurbMass = row.ParseDouble("curbmass").SI<Kilogram>(),
						NumberPassengersLowerDeck = row.ParseDouble("passengerslowerdeck"),
						NumberPassengersUpperDeck = row.ParseDouble("passengersupperdeck"),
						BodyCurbWeight = 0.SI<Kilogram>(),
						Trailer = new List<MissionTrailer>(),
						MinLoad = null,
						MaxLoad = null,
						LowLoad = refLoad * 0.2,
						RefLoad = refLoad,
						VehicleHeight = row.ParseDouble("height").SI<Meter>(),
						VehicleLength = row.ParseDouble("length").SI<Meter>(),
						VehicleWidth = row.ParseDouble("width").SI<Meter>(),
						TotalCargoVolume = 0.SI<CubicMeter>(),
						DefaultCDxA = row.ParseDouble("cdxastandard").SI<SquareMeter>()
					};
					missions.Add(mission);
				}
			}

			return missions.ToArray();
		}

		private static double[] GetAxleWeightDistribution(DataRow row)
		{
			var axleDistribution = row.Field<string>("AxlesWeights");
			if (string.IsNullOrWhiteSpace(axleDistribution)) {
				return new double[] { };
			}

			return axleDistribution.Split('/').ToDouble().Select(x => x / 100.0).ToArray();
		}
	}
}