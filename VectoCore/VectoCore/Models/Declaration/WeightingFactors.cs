using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class WeightingFactors : LookupData
	{
		private readonly Dictionary<WeightingGroup, Dictionary<Tuple<MissionType, LoadingType>, double>> Data = new Dictionary<WeightingGroup, Dictionary<Tuple<MissionType, LoadingType>, double>>();

		public IDictionary<Tuple<MissionType, LoadingType>, double> Lookup(WeightingGroup group)
		{
			return new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(Data[group]);
		}

		#region Overrides of LookupData

		protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".CO2Standards.MissionProfileWeights.csv"; } }
		protected override string ErrorMessage { get { return "No Weighting Factors found for Weighting Group {0}"; } }
		protected override void ParseData(DataTable table)
		{
			var loadingTypes = new[] { LoadingType.LowLoading, LoadingType.ReferenceLoad };
			var missions = new[] {
				MissionType.LongHaul, MissionType.LongHaulEMS, MissionType.RegionalDelivery, MissionType.RegionalDeliveryEMS,
				MissionType.UrbanDelivery, MissionType.MunicipalUtility, MissionType.Construction
			};
			foreach (DataRow row in table.Rows) {
				var weightingGroup = WeightingGroupHelper.Parse(row.Field<string>("weightinggroup"));
				if (!Data.ContainsKey(weightingGroup)) {
					Data[weightingGroup] = new Dictionary<Tuple<MissionType, LoadingType>, double>();
				}
				foreach (var missionType in missions) {
					var factors = row.Field<string>(missionType.GetName());
					if (string.IsNullOrWhiteSpace(factors)) {
						continue;
					}

					var factorsPerLoading = factors.Split('/');
					if (factorsPerLoading.Length != loadingTypes.Length) {
						throw new VectoException("Number of entries in MissionProfileWeights does not match expected payloads");
					}
					for (var i = 0; i < loadingTypes.Length; i++) {
						var cycleAndPayload = Tuple.Create(missionType, loadingTypes[i]);
						Data[weightingGroup][cycleAndPayload] = factorsPerLoading[i].ToDouble(0);
					}
				}
			}

			foreach (var entry in Data) {
				var sum = entry.Value.Sum(item => item.Value);
				if (!sum.IsEqual(1.0, 1e-12)) {
					throw new VectoException("Weighting Factors for {0} do not sum up to 1.0! sum: {1}", entry.Key, sum);
				}
			}
		}

		#endregion
	}
}
