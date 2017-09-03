/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class Segments : LookupData<VehicleCategory, AxleConfiguration, Kilogram, Kilogram, Segment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".SegmentTable.csv"; }
		}

		protected override string ErrorMessage
		{
			get {
				return
					"ERROR: Could not find the declaration segment for vehicle. Category: {0}, AxleConfiguration: {1}, GrossVehicleWeight: {2}";
			}
		}

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, Kilogram curbWeight)
		{
			return Lookup(vehicleCategory, axleConfiguration, grossVehicleMassRating, curbWeight, false);
		}

		public Segment Lookup(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, Kilogram curbWeight, bool considerInvalid)
		{
            //if (grossVehicleMassRating == null || grossVehicleMassRating < 7.5.SI().Ton) {
		    if (grossVehicleMassRating == null || grossVehicleMassRating < 7.5.SI(Unit.SI.Ton)){
                throw new VectoException("Gross vehicle mass must be greater than 7.5 tons");
			}

			var row = GetSegmentDataRow(vehicleCategory, axleConfiguration, grossVehicleMassRating, considerInvalid);
			if (row == null) {
				return new Segment() { Found = false };
			}

			var segment = new Segment {
				Found = true,
                //GrossVehicleWeightMin = row.ParseDouble("gvw_min").SI().Ton.Cast<Kilogram>(),
			    GrossVehicleWeightMin = row.ParseDouble("gvw_min").SI(Unit.SI.Ton).Cast<Kilogram>(),
                //GrossVehicleWeightMax = row.ParseDouble("gvw_max").SI().Ton.Cast<Kilogram>(),
			    GrossVehicleWeightMax = row.ParseDouble("gvw_max").SI(Unit.SI.Ton).Cast<Kilogram>(),
                VehicleCategory = vehicleCategory,
				AxleConfiguration = axleConfiguration,
				VehicleClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass")),
				AccelerationFile =
					RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
												row.Field<string>(".vaccfile")),
				Missions = CreateMissions(ref grossVehicleMassRating, curbWeight, row),
				VehicleHeight = LookupHeight(vehicleCategory, axleConfiguration, grossVehicleMassRating),
				DesignSpeed = row.ParseDouble("designspeed").KMPHtoMeterPerSecond(),
				GrossVehicleMassRating = grossVehicleMassRating,
				CdADefault = string.IsNullOrEmpty(row["cdxa_default"].ToString()) ? null : row.ParseDouble("cdxa_default").SI<SquareMeter>(),
				CdAConstruction = string.IsNullOrEmpty(row["cdxa_construction"].ToString())
					? null
					: row.ParseDouble("cdxa_construction").SI<SquareMeter>(),
				MunicipalBodyWeight = string.IsNullOrEmpty(row["bodyweight_municipalutility"].ToString())
					? null
					: row.ParseDouble("bodyweight_municipalutility").SI<Kilogram>()
			};

			return segment;
		}

		private DataRow GetSegmentDataRow(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, bool considerInvalid)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(r => {
					var isValid = r.Field<string>("valid");
					var category = r.Field<string>("vehiclecategory");
					var axleConf = r.Field<string>("axleconf.");
                    //var massMin = r.ParseDouble("gvw_min").SI().Ton;
				    var massMin = r.ParseDouble("gvw_min").SI(Unit.SI.Ton);
                    //var massMax = r.ParseDouble("gvw_max").SI().Ton;
				    var massMax = r.ParseDouble("gvw_max").SI(Unit.SI.Ton);
                    return (considerInvalid || isValid == "1")
							&& category == vehicleCategory.ToString()
							&& axleConf == axleConfiguration.GetName()
						// MK 2016-06-07: normally the next condition should be "mass > massMin", except for 7.5t where is should be ">="
						// in any case ">=" is also correct, because the segment table is sorted by weight.
							&& massMin <= grossVehicleMassRating && grossVehicleMassRating <= massMax;
				});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(ErrorMessage, vehicleCategory, axleConfiguration.GetName(),
					grossVehicleMassRating);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e); 
			}
			return row;
		}

		public Meter LookupHeight(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating)
		{
			var row = GetSegmentDataRow(vehicleCategory, axleConfiguration, grossVehicleMassRating, true);

			var vehicleHeight = row.ParseDouble("height").SI<Meter>();
			var vehicleClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass"));

			if (vehicleClass == VehicleClass.Class9) {
				// VECTO-471: for class 9 take similar height than rigid with same maximum gross vehicle weight (class 1, 2, 3 or 4).
				var rigidGVWrow = _segmentTable.AsEnumerable().FirstOrDefault(r => {
                    //var massMin = r.ParseDouble("gvw_min").SI().Ton;
				    var massMin = r.ParseDouble("gvw_min").SI(Unit.SI.Ton);
                    //var massMax = r.ParseDouble("gvw_max").SI().Ton;
				    var massMax = r.ParseDouble("gvw_max").SI(Unit.SI.Ton);
                    return new[] { "1", "2", "3", "4" }.Contains(r.Field<string>("hdvclass"))
							&& massMin <= grossVehicleMassRating && grossVehicleMassRating <= massMax;
				});
				if (rigidGVWrow != null) {
					vehicleHeight = rigidGVWrow.ParseDouble("height").SI<Meter>();
				}
			}

			return vehicleHeight;
		}

		/// <summary>
		/// Looks up the default CdxA value for the cross wind correction.
		/// </summary>
		public SquareMeter LookupCdA(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating)
		{
			var row = GetSegmentDataRow(vehicleCategory, axleConfiguration, grossVehicleMassRating, true);
			return row.SI<SquareMeter>("cdxa_default");
		}

		private static Mission[] CreateMissions(ref Kilogram grossVehicleWeight, Kilogram curbWeight, DataRow row)
		{
			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>();
			var missions = new List<Mission>();
			foreach (var missionType in missionTypes.Where(m => row.Field<string>(m.ToString()) != "-")) {
				var body = DeclarationData.StandardBodies.Lookup(row.Field<string>("body"));

				var maxGVW = Constants.SimulationSettings.MaximumGrossVehicleWeight;
				var trailers = new List<MissionTrailer>();
				if (missionType.IsEMS()) {
					maxGVW = Constants.SimulationSettings.MaximumGrossVehicleWeightEMS;
					var trailerList = row.Field<string>("ems").Split('+');
					var trailerWeightShares = row.Field<string>("traileraxles" + GetMissionSuffix(missionType)).Split('/');
					if (trailerList.Length != trailerWeightShares.Length) {
						throw new VectoException(
							"Error in segmentation table: number of trailers and list of weight shares does not match!");
					}
					trailers.AddRange(
						trailerWeightShares.Select((t, i) => CreateTrailer(trailerList[i], t.ToDouble() / 100.0, i == 0)));
				} else {
					if (ShouldTrailerBeUsed(row, missionType)) {
						var trailerValue = row.Field<string>("trailer");
						if (string.IsNullOrWhiteSpace(trailerValue)) {
							throw new VectoException("Error in segmentation table: trailer weight share is defined but not trailer type!");
						}
						trailers.Add(CreateTrailer(trailerValue, GetTrailerAxleWeightDistribution(row, missionType), true));
					}
				}

				//var semiTrailerField = row.Field<string>("semitrailer");
				//var semiTrailer = !string.IsNullOrWhiteSpace(semiTrailerField)
				//	? DeclarationData.StandardBodies.Lookup(semiTrailerField)
				//	: StandardBodies.Empty;

				//trailer += semiTrailer;

				// limit gvw to MaxGVW (40t)
				var gvw =
					VectoMath.Min(
						grossVehicleWeight + trailers.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0),
						maxGVW);
				var maxLoad = gvw - curbWeight - body.CurbWeight -
							trailers.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);

				var payloads = row.Field<string>(missionType.ToString()).Split('/');
				var vehicleWeight = curbWeight + body.CurbWeight;
				Kilogram refLoad, lowLoad = 0.SI<Kilogram>();
				if (payloads.Length == 2) {
					lowLoad = GetLoading(payloads[0], grossVehicleWeight, vehicleWeight, trailers, true);
					refLoad = GetLoading(payloads[1], grossVehicleWeight, vehicleWeight, trailers, false);
				} else {
					refLoad = GetLoading(row.Field<string>(missionType.ToString()), grossVehicleWeight, vehicleWeight, trailers, false);
				}

				refLoad = refLoad.LimitTo(0.SI<Kilogram>(), maxLoad);
				lowLoad = lowLoad.LimitTo(0.SI<Kilogram>(), maxLoad);

				var mission = new Mission {
					MissionType = missionType,
					CrossWindCorrectionParameters = row.Field<string>("crosswindcorrection" + GetMissionSuffix(missionType, true)),
					CycleFile =
						RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
													missionType.ToString().Replace("EMS", "") +
													Constants.FileExtensions.CycleFile),
					AxleWeightDistribution = GetAxleWeightDistribution(row, missionType),
					BodyCurbWeight = body.CurbWeight,
					Trailer = trailers,
					MinLoad = 0.SI<Kilogram>(),
					MaxLoad = maxLoad,
					RefLoad = refLoad,
					LowLoad = lowLoad,
					TotalCargoVolume = body.CargoVolume + trailers.Sum(t => t.CargoVolume).DefaultIfNull(0),
				};
				missions.Add(mission);
			}
			return missions.ToArray();
		}

		private static Kilogram GetLoading(string payloadStr, Kilogram grossVehicleWeight, Kilogram vehicleWeight,
			IEnumerable<MissionTrailer> trailers, bool lowLoading)
		{
			var refLoadValue = payloadStr.ToDouble(double.NaN);
			if (double.IsNaN(refLoadValue)) {
				var vehiclePayload = DeclarationData.GetPayloadForGrossVehicleWeight(grossVehicleWeight, payloadStr)
					.LimitTo(0.SI<Kilogram>(), grossVehicleWeight - vehicleWeight);
				var trailerPayload = trailers.Sum(
					t => DeclarationData.GetPayloadForTrailerWeight(t.TrailerGrossVehicleWeight, t.TrailerCurbWeight, lowLoading))
					.DefaultIfNull(0);
					return vehiclePayload + trailerPayload;
			}
			return refLoadValue.SI<Kilogram>();
		}

		/// <summary>
		/// Checks if a trailer should be used for the current missionType.
		/// </summary>
		private static bool ShouldTrailerBeUsed(DataRow row, MissionType missionType)
		{
			return !string.IsNullOrWhiteSpace(row.Field<string>("traileraxles" + GetMissionSuffix(missionType)));
		}

		private static double GetTrailerAxleWeightDistribution(DataRow row, MissionType missionType)
		{
			var trailerAxles =
				row.Field<string>("traileraxles" + GetMissionSuffix(missionType));
			if (!string.IsNullOrWhiteSpace(trailerAxles)) {
				return trailerAxles.ToDouble() / 100.0;
			}
			return 0;
		}

		private static double[] GetAxleWeightDistribution(DataRow row, MissionType missionType)
		{
			var axleDistribution = row.Field<string>("truckaxles" + GetMissionSuffix(missionType));
			if (string.IsNullOrWhiteSpace(axleDistribution)) {
				return new double[]{};
			}
			return axleDistribution.Split('/').ToDouble().Select(x => x / 100.0).ToArray();
		}

		private static string GetMissionSuffix(MissionType missionType, bool ignoreEMS = false)
		{
			return "-" +
					(missionType.IsEMS() && ignoreEMS
						? ""
						: (missionType.GetNonEMSMissionType() == MissionType.LongHaul ? "longhaul" : "other")) +
					(missionType.IsEMS() ? "ems" : "");
		}

		private static MissionTrailer CreateTrailer(string trailerValue, double axleWeightShare, bool firstTrailer)
		{
			var trailerType = TrailterTypeHelper.Parse(trailerValue);
			var trailer = DeclarationData.StandardBodies.Lookup(trailerType.ToString());
			return new MissionTrailer {
				TrailerType = trailerType,
				TrailerWheels = trailer.Wheels,
				TrailerAxleWeightShare = axleWeightShare,
				TrailerCurbWeight = trailer.CurbWeight,
				TrailerGrossVehicleWeight = trailer.GrossVehicleWeight,
				DeltaCdA = trailer.DeltaCrossWindArea[firstTrailer ? 0 : 1],
				CargoVolume = trailer.CargoVolume
			};
		}
	}
}