using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class ADASBenefits : LookupData<VehicleClass, ADASCombination, MissionType, LoadingType, double>
	{
		private readonly Dictionary<Tuple<VehicleClass, ADASCombination>, Dictionary<Tuple<MissionType, LoadingType>, double>> _data = new Dictionary<Tuple<VehicleClass, ADASCombination>, Dictionary<Tuple<MissionType, LoadingType>, double>>();
		
		#region Overrides of LookupData

		public override double Lookup(VehicleClass group, ADASCombination adasCombination, MissionType mission, LoadingType loading)
		{
			var groupAndAdas = Tuple.Create(group, adasCombination);
			var missionAndLoading = Tuple.Create(mission, loading);
			if (!_data.ContainsKey(groupAndAdas)) {
				return 1.0;
			}
			if (!_data[groupAndAdas].ContainsKey(missionAndLoading)) {
				return 1.0;
			}
			return _data[groupAndAdas][missionAndLoading];
		}

		protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".ADAS.ADAS_Benefits.csv"; } }
		protected override string ErrorMessage { get { return "ADAS Benefits Lookup Error: No value found."; } }
		protected override void ParseData(DataTable table)
		{
			var loadingTypes = new[] { LoadingType.LowLoading, LoadingType.ReferenceLoad };
			foreach (DataRow row in table.Rows) {
				var groupAndAdas = Tuple.Create(VehicleClassHelper.Parse(row.Field<string>("vehiclegroup")), new ADASCombination() { ID = row.Field<string>("adascombination")});
				if (!_data.ContainsKey(groupAndAdas)) {
					_data[groupAndAdas] = new Dictionary<Tuple<MissionType, LoadingType>, double>();
				}
				foreach (var missionType in new[] { MissionType.LongHaul, MissionType.LongHaulEMS, MissionType.RegionalDelivery, MissionType.RegionalDeliveryEMS, MissionType.UrbanDelivery}) {
					var benefits = row.Field<string>(missionType.GetName());
					if (string.IsNullOrWhiteSpace(benefits)) {
						continue;
					}

					var benefitsPerLoading = benefits.Split('/');
					for (var i = 0; i < loadingTypes.Length; i++) {
						var cycleAndPayload = Tuple.Create(missionType, loadingTypes[i]);
						_data[groupAndAdas][cycleAndPayload] = BenefitFacor(benefitsPerLoading[i]);
					}
				}
			}
		}

		private double BenefitFacor(string benefitStr)
		{
			var benefit = benefitStr.Replace("%", "").ToDouble();
			return 1 + benefit / 100.0;
		}

		#endregion

	}
}
