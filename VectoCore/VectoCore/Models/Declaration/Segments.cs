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
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class Segments : LookupData<VehicleCategory, AxleConfiguration, Kilogram, Kilogram, Segment>
	{
		public Segments()
		{
			ParseData(ReadCsvResource(RessourceHelper.Namespace + "SegmentTable.csv"));
		}

		protected override void ParseData(DataTable table)
		{
			// normalize column names, remove whitespaces and lowercase
			foreach (DataColumn col in table.Columns) {
				table.Columns[col.ColumnName].ColumnName = col.ColumnName.ToLower().RemoveWhitespace();
			}
			SegmentTable = table.Copy();
		}

		private DataTable SegmentTable { get; set; }

		public override Segment Lookup(VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, Kilogram curbWeight)
		{
			if (grossVehicleMassRating == null || grossVehicleMassRating < 7.5.SI().Ton) {
				throw new VectoException("Gross vehicle mass must be greater than 7.5 tons");
			}

			DataRow row;
			try {
				row = SegmentTable.Rows.Cast<DataRow>().First(r => {
					var isValid = r.Field<string>("valid");
					var category = r.Field<string>("vehiclecategory");
					var axleConf = r.Field<string>("axleconf.");
					var massMin = r.ParseDouble("gvw_min").SI().Ton;
					var massMax = r.ParseDouble("gvw_max").SI().Ton;
					return isValid == "1"
							&& category == vehicleCategory.ToString()
							&& axleConf == axleConfiguration.GetName()
						// MK 2016-06-07: normally the next condition should be "mass > massMin", except for 7.5t where is should be ">="
						// in any case ">=" is also correct, because the segment table is sorted by weight.
							&& grossVehicleMassRating >= massMin
							&& grossVehicleMassRating <= massMax;
				});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					"ERROR: Could not find the declaration segment for vehicle. Category: {0}, AxleConfiguration: {1}, GrossVehicleMassRating: {2}",
					vehicleCategory, axleConfiguration.GetName(), grossVehicleMassRating);
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
				Missions = CreateMissions(grossVehicleMassRating, curbWeight, row).ToArray(),
				GrossVehicleMassRating = grossVehicleMassRating
			};

			return segment;
		}

		private static IEnumerable<Mission> CreateMissions(Kilogram grossVehicleMassRating, Kilogram curbWeight, DataRow row)
		{
			var trailerOnlyInLongHaul = row.Field<string>("vehiclecategory") == VehicleCategory.RigidTruck.ToString() &&
										row.Field<string>("traileraxles-longhaul") != "-" &&
										row.Field<string>("traileraxles-other") == "-";

			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>();
			foreach (var missionType in missionTypes.Where(m => row.Field<string>(m.ToString()) == "1")) {
				string vcdvField;
				string axleField;
				string trailerField;

				if (missionType == MissionType.LongHaul) {
					vcdvField = "crosswindcorrection-longhaul";
					axleField = "truckaxles-longhaul";
					trailerField = "traileraxles-longhaul";
				} else {
					vcdvField = "crosswindcorrection-other";
					axleField = "truckaxles-other";
					trailerField = "traileraxles-other";
				}

				var mission = new Mission {
					MissionType = missionType,
					CrossWindCorrection = row.Field<string>(vcdvField),
					MassExtra = row.ParseDouble("massextra-" + missionType.ToString().ToLower()).SI<Kilogram>(),
					CycleFile = RessourceHelper.ReadStream(RessourceHelper.Namespace + "MissionCycles." + missionType + ".vdri"),
					AxleWeightDistribution = row.Field<string>(axleField).Split('/').ToDouble().Select(x => x / 100.0).ToArray(),
					UseCdA2 = trailerOnlyInLongHaul && missionType != MissionType.LongHaul,
				};

				var trailerAxles = row.Field<string>(trailerField).Split('/');
				var count = 0;
				var weightPercent = 0.0;

				if (trailerAxles[0] != "-") {
					count = int.Parse(trailerAxles[1]);
					weightPercent = trailerAxles[0].ToDouble();
				}
				mission.TrailerAxleWeightDistribution = Enumerable.Repeat(weightPercent / count / 100.0, count).ToArray();

				mission.MinLoad = 0.SI<Kilogram>();
				mission.MaxLoad = grossVehicleMassRating - mission.MassExtra - curbWeight;

				var refLoadField = row.Field<string>("payload-" + missionType.ToString().ToLower());
				if (refLoadField == "R(pc)")
					mission.RefLoad = VectoMath.Min(DeclarationData.PayloadForGVW(grossVehicleMassRating, missionType), mission.MaxLoad);
				else if (refLoadField == "R(pc)+T") {
					// R(pc) + Trailer 3.4t + Loading Trailer 5.3t
					mission.RefLoad = VectoMath.Min(DeclarationData.PayloadForGVW(grossVehicleMassRating, missionType), mission.MaxLoad);
					mission.RefLoad += 3400.SI<Kilogram>() + 5300.SI<Kilogram>();
				} else {
					mission.RefLoad = VectoMath.Min(refLoadField.ToDouble().SI<Kilogram>(), mission.MaxLoad);
				}

				yield return mission;
			}
		}
	}
}