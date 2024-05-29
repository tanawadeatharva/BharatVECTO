/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using TUGraz.VectoCommon.Models;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum WeightingGroup
	{
		Group1s = 1,
		Group1,
		Group2,
		Group3,
        Group4UD,
		Group4RD,
		Group4LH,
		Group5RD,
		Group5LH,
		Group9RD,
		Group9LH,
		Group10RD,
		Group10LH,

		Group11,
		Group12,
		Group16,

		// medium lorries
		Group51,
		Group52,
		Group53,
		Group54,
		Group55,
		Group56,

		// completed bus groups
		Group31a,
		Group31b1,
		Group31b2,
		Group31c,
		Group31d,
		Group31e,
		Group32a,
		Group32b,
		Group32c,
		Group32d,
		Group32e,
		Group32f,

		Group33a,
		Group33b1,
		Group33b2,
		Group33c,
		Group33d,
		Group33e,
		Group34a,
		Group34b,
		Group34c,
		Group34d,
		Group34e,
		Group34f,

		Group35a,
		Group35b1,
		Group35b2,
		Group35c,
		Group36a,
		Group36b,
		Group36c,
		Group36d,
		Group36e,
		Group36f,

		Group37a,
		Group37b1,
		Group37b2,
		Group37c,
		Group37d,
		Group37e,
		Group38a,
		Group38b,
		Group38c,
		Group38d,
		Group38e,
		Group38f,

		Group39a,
		Group39b1,
		Group39b2,
		Group39c,
		Group40a,
		Group40b,
		Group40c,
		Group40d,
		Group40e,
		Group40f,

        Unknown
    }

	public static class WeightingGroupHelper
	{
		public const string Prefix = "Group";
		public static WeightingGroup Parse(string groupStr)
		{
			return (Prefix + groupStr.Replace("-", "")).ParseEnum<WeightingGroup>();
		}

		public static string ToXMLFormat(this WeightingGroup group)
		{
			switch (group) {
				case WeightingGroup.Group1:
				case WeightingGroup.Group2:
				case WeightingGroup.Group3:
				case WeightingGroup.Group11:
				case WeightingGroup.Group12:
				case WeightingGroup.Group16:
				case WeightingGroup.Unknown:
					return Constants.NOT_AVAILABLE;
				case WeightingGroup.Group4UD:
				case WeightingGroup.Group4RD:
				case WeightingGroup.Group4LH:
				case WeightingGroup.Group5RD:
				case WeightingGroup.Group5LH:
				case WeightingGroup.Group9RD:
				case WeightingGroup.Group9LH:
				case WeightingGroup.Group10RD:
				case WeightingGroup.Group10LH:
					return Regex.Split(group.ToString().Replace(Prefix, ""), @"(\d+|\w+)").Where(x => !string.IsNullOrWhiteSpace(x)).Join("-");
				default:
					return Constants.NOT_AVAILABLE;
			}
		}
	}

	public class WeightingGroups : LookupData<VehicleClass, bool, Watt, bool, double?, WeightingGroup>
	{
		protected readonly List<Entry> Entries = new List<Entry>();

		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".CO2Standards.WeightingGroups.csv";

		protected override string ErrorMessage => "WeightingGroup Lookup Error: no entry found for group {0}, sleeper cab: {1}, engine rated power {2}";

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				Entries.Add(new Entry() {
					VehicleGroup = VehicleClassHelper.Parse(row.Field<string>("vehiclegroup")),
					IsElectric = "1".Equals(row.Field<string>("iselectric"), StringComparison.InvariantCultureIgnoreCase),
					SleeperCab = "SleeperCab".Equals(row.Field<string>("cabintype"), StringComparison.InvariantCultureIgnoreCase),
					RatedPowerMin = row.ParseDouble("engineratedpowermin").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					RatedPowerMax = row.ParseDouble("engineratedpowermax").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					ElectricRange = row.ParseDouble("electricrange").SI<Meter>(),
					WeightingGroup = WeightingGroupHelper.Parse(row.Field<string>("weightinggroup")),
				});
			}
		}


		public override WeightingGroup Lookup(VehicleClass group, bool sleeperCab, Watt engineRatedPower, bool isElectric = false,  double? electricRange = null)
		{
			WeightingGroup Lookup()
			{
				var rows = Entries.FindAll(
					e => e.VehicleGroup == group &&
					e.IsElectric == false &&
					e.SleeperCab == sleeperCab &&
					engineRatedPower >= e.RatedPowerMin &&
					engineRatedPower < e.RatedPowerMax);

				return rows.Count == 0 ? WeightingGroup.Unknown : rows.First().WeightingGroup;
			}

			/// Never meet the conditions for Electric Range if null.
			var operationalRange = electricRange == null ? double.MaxValue : electricRange.Value;

			var electricRows = Entries.FindAll(
				e => e.VehicleGroup == group &&
				e.IsElectric == isElectric &&
				e.SleeperCab == sleeperCab &&
				engineRatedPower >= e.RatedPowerMin &&
				engineRatedPower < e.RatedPowerMax &&
				operationalRange < e.ElectricRange);

			return electricRows.Count == 0 ? Lookup() : electricRows.First().WeightingGroup;
		}

		#endregion

		protected class Entry
		{
			public VehicleClass VehicleGroup;
			public bool IsElectric;
			public bool SleeperCab;
			public Watt RatedPowerMin;
			public Watt RatedPowerMax;
			public Meter ElectricRange;
			public WeightingGroup WeightingGroup;
		}
	}
}
