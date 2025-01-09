using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
    public class SpecificResistances : LookupData<BatteryType, SpecificResistances.SpecificResistancesData>
    {
        protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".SpecificResistances.csv";
        protected override string ErrorMessage => "SpecificResistances Lookup Error: no value found. Key: '{0}'";

        public struct SpecificResistancesData 
        {
            public double Ri_2;
            public double Ri_10;
            public double Ri_20;
            public double Ri_120;

            public double GetValue(string variable)
            {
                var modifiedVariable = variable.Replace('-', '_');

                switch (modifiedVariable)
                {
                    case nameof(Ri_2): return Ri_2;
                    case nameof(Ri_10): return Ri_10;
                    case nameof(Ri_20): return Ri_20;
                    case nameof(Ri_120): return Ri_120;
                    default: throw new ArgumentOutOfRangeException($"{variable} is not part of SpecificResistancesData");
                }
            }
        }

        public override SpecificResistancesData Lookup(BatteryType key)
        {
            return base.Lookup(key);
        }

        protected override void ParseData(DataTable table)
        {
            var ri120 = nameof(SpecificResistancesData.Ri_120).ToLower();

            foreach (DataRow row in table.Rows)
            {    
                var val = new SpecificResistancesData()
                {
                    Ri_2 = row.ParseDouble(nameof(SpecificResistancesData.Ri_2).ToLower()),
                    Ri_10 = row.ParseDouble(nameof(SpecificResistancesData.Ri_10).ToLower()),
                    Ri_20 = row.ParseDouble(nameof(SpecificResistancesData.Ri_20).ToLower()),
                    Ri_120 = !row.IsNull(ri120) && !String.IsNullOrWhiteSpace(row[ri120].ToString()) ? row.ParseDouble(ri120) : double.NaN
                };

                var type = (BatteryType)Enum.Parse(typeof(BatteryType), row["type"].ToString());

                Data.Add(type, val);
            }
        }

    }
}
