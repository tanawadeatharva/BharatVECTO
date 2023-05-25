using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class BatteryLimits : LookupData<string, BatteryLimits.BatteryLimit>
	{
		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Buses.SmartES_Limits.csv";
		
		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>()
				.Select(r => Tuple.Create(r.Field<string>("technology"), r.ParseDouble("socrange") / 100, r.ParseDouble("c-rate")))
				.ToDictionary(e => e.Item1, e => new BatteryLimit(e.Item1, e.Item2, e.Item3));
		}

		#endregion

        public struct BatteryLimit
		{
			public BatteryLimit(string technology, double socRange, double cRate)
			{
				Technology = technology;
				SoC_Range = socRange;
				C_Rate = cRate;
			}
			
			public  string Technology { get; }

			public double SoC_Range { get; }

			public double C_Rate { get;  }
		}

		
	}
}