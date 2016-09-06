/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class Segments : LookupData<VehicleCategory, AxleConfiguration, Kilogram, Kilogram, Segment>
	{
		private DataTable _segmentTable;


		protected override string ResourceId
		{
			get { return RessourceHelper.Namespace + "SegmentTable.csv"; }
		}

		protected override string ErrorMessage
		{
			get
			{
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
			if (grossVehicleMassRating == null || grossVehicleMassRating < 7.5.SI().Ton) {
				throw new VectoException("Gross vehicle mass must be greater than 7.5 tons");
			}

			DataRow row;
			try {
				row = _segmentTable.Rows.Cast<DataRow>().First(r => {
					var isValid = r.Field<string>("valid");
					var category = r.Field<string>("vehiclecategory");
					var axleConf = r.Field<string>("axleconf.");
					var massMin = r.ParseDouble("gvw_min").SI().Ton;
					var massMax = r.ParseDouble("gvw_max").SI().Ton;
					return (considerInvalid || isValid == "1")
							&& category == vehicleCategory.ToString()
							&& axleConf == axleConfiguration.GetName()
						// MK 2016-06-07: normally the next condition should be "mass > massMin", except for 7.5t where is should be ">="
						// in any case ">=" is also correct, because the segment table is sorted by weight.
							&& grossVehicleMassRating >= massMin
							&& grossVehicleMassRating <= massMax;
				});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(ErrorMessage, vehicleCategory, axleConfiguration.GetName(), grossVehicleMassRating);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}
			var segment = new Segment {
				GrossVehicleWeightMin = row.ParseDouble("gvw_min").SI().Ton.Cast<Kilogram>(),
				GrossVehicleWeightMax = row.ParseDouble("gvw_max").SI().Ton.Cast<Kilogram>(),
				VehicleCategory = vehicleCategory,
				AxleConfiguration = axleConfiguration,
				VehicleClass = VehicleClassHelper.Parse(row.Field<string>("hdvclass")),
				AccelerationFile = RessourceHelper.ReadStream(RessourceHelper.Namespace + "VACC." + row.Field<string>(".vaccfile")),
				Missions = CreateMissions(ref grossVehicleMassRating, curbWeight, row),
				GrossVehicleMassRating = grossVehicleMassRating
			};

			return segment;
		}

		private static Mission[] CreateMissions(ref Kilogram grossVehicleWeight, Kilogram curbWeight, DataRow row)
		{
			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>();
			var missions = new List<Mission>();
			foreach (var missionType in missionTypes.Where(m => row.Field<string>(m.ToString()) != "-")) {
				var body = DeclarationData.StandardBodies.Lookup(row.Field<string>("body"));

				var trailerIsUsed = ShouldTrailerBeUsed(row, missionType);
				var trailerField = row.Field<string>("trailer");
				var trailerType = trailerIsUsed && !string.IsNullOrWhiteSpace(trailerField)
					? trailerField.ParseEnum<TrailerType>()
					: TrailerType.None;
				var trailer = trailerIsUsed
					? DeclarationData.StandardBodies.Lookup(trailerField)
					: StandardBodies.Empty;

				var semiTrailerField = row.Field<string>("semitrailer");
				var semiTrailer = !string.IsNullOrWhiteSpace(semiTrailerField)
					? DeclarationData.StandardBodies.Lookup(semiTrailerField)
					: StandardBodies.Empty;

				trailer += semiTrailer;

				// limit gvw to MaxGVW (40t)
				var gvw = VectoMath.Min(grossVehicleWeight + trailer.GrossVehicleWeight,
					Constants.SimulationSettings.MaximumGrossVehicleWeight);
				var maxLoad = gvw - curbWeight - body.CurbWeight - trailer.CurbWeight;

				var refLoadValue = row.ParseDoubleOrGetDefault(missionType.ToString(), double.NaN);
				Kilogram refLoad;
				if (double.IsNaN(refLoadValue)) {
					refLoad = DeclarationData.GetPayloadForGrossVehicleWeight(grossVehicleWeight, missionType) +
							DeclarationData.GetPayloadForTrailerWeight(trailer.GrossVehicleWeight, trailer.CurbWeight);
				} else {
					refLoad = refLoadValue.SI<Kilogram>();
				}

				refLoad = VectoMath.Min(refLoad, maxLoad);

				var mission = new Mission {
					MissionType = missionType,
					CrossWindCorrection = row.Field<string>("crosswindcorrection" + GetMissionSuffix(missionType)),
					CycleFile = RessourceHelper.ReadStream(RessourceHelper.Namespace + "MissionCycles." + missionType + ".vdri"),
					AxleWeightDistribution = GetAxleWeightDistribution(row, missionType),
					CurbWeight = curbWeight,
					BodyCurbWeight = body.CurbWeight,
					BodyGrossVehicleWeight = grossVehicleWeight,
					TrailerType = trailerType,
					TrailerCurbWeight = trailer.CurbWeight,
					TrailerGrossVehicleWeight = trailer.GrossVehicleWeight,
					DeltaCdA = trailer.DeltaCrossWindArea,
					MinLoad = 0.SI<Kilogram>(),
					MaxLoad = maxLoad,
					RefLoad = refLoad,
					TrailerAxleWeightDistribution = GetTrailerAxleWeightDistribution(row, missionType),
				};
				missions.Add(mission);
			}
			return missions.ToArray();
		}

		/// <summary>
		/// Checks if a trailer should be used for the current missionType.
		/// </summary>
		private static bool ShouldTrailerBeUsed(DataRow row, MissionType missionType)
		{
			return !string.IsNullOrWhiteSpace(row.Field<string>("traileraxles" + GetMissionSuffix(missionType)));
		}

		private static double[] GetTrailerAxleWeightDistribution(DataRow row, MissionType missionType)
		{
			var trailerAxles =
				row.Field<string>("traileraxles" + GetMissionSuffix(missionType)).Split('/');
			if (!string.IsNullOrWhiteSpace(trailerAxles[0])) {
				var count = int.Parse(trailerAxles[1]);
				return (trailerAxles[0].ToDouble() / 100.0 / count).Repeat(count).ToArray();
			}
			return new double[0];
		}

		private static double[] GetAxleWeightDistribution(DataRow row, MissionType missionType)
		{
			return
				row.Field<string>("truckaxles" + GetMissionSuffix(missionType))
					.Split('/').ToDouble().Select(x => x / 100.0).ToArray();
		}

		private static string GetMissionSuffix(MissionType missionType)
		{
			return missionType == MissionType.LongHaul ? "-longhaul" : "-other";
		}
	}
}