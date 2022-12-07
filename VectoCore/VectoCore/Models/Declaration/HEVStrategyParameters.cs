using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class HEVStrategyParameters : LookupData<string, HEVStrategyParameters.Entry>
	{
		
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".HEV_Strategy_Parameters_fequiv_40soc_Lorries.csv"; // todo rename csv file...

		public double LookupEquivalenceFactor(MissionType mission, VehicleClass hdvClass, LoadingType loading)
		{
            var entry = Lookup(hdvClass.GetClassNumber()).cycleDict[mission];

			switch (loading) {
				case LoadingType.LowLoading:
					return entry.Item1;
				case LoadingType.ReferenceLoad:
					return entry.Item2;
				default:
					throw new ArgumentOutOfRangeException(nameof(loading), loading, null);
			}
		}

        public override Entry Lookup(string key)
        {
            return base.Lookup(key.RemoveWhitespace());
        }

        protected override void ParseData(DataTable table)
        {
            var vehicleClasses = table.Rows.Cast<DataRow>().Select(row => row.Field<string>("vehiclegroup"));


            var cycleEntries = table.Rows.Cast<DataRow>().Select(row => new CycleEntry
            {
                VehicleGroup = row.Field<string>("vehiclegroup").RemoveWhitespace(),
                LongHaul = SplitStringToDoubleTuple(row.Field<string>("longhaul")),
                RegionalDelivery = SplitStringToDoubleTuple(row.Field<string>("regionaldelivery")),
                UrbanDelivery = SplitStringToDoubleTuple(row.Field<string>("urbandelivery")),
                MunicipalUtility = SplitStringToDoubleTuple(row.Field<string>("municipalutility")),
                Construction = SplitStringToDoubleTuple(row.Field<string>("construction"))
            });


            foreach (var cycleEntry in cycleEntries)
            {
                foreach (string vehClass in cycleEntry.VehicleGroup.Split('/'))
                {
                    var newEntry = new Entry
                    {
                        VehicleGroup = vehClass,
                        cycleDict = new Dictionary<MissionType, Tuple<double, double>>()
                    };

                    newEntry.cycleDict.Add(MissionType.LongHaul, cycleEntry.LongHaul);
                    newEntry.cycleDict.Add(MissionType.RegionalDelivery, cycleEntry.RegionalDelivery);
                    newEntry.cycleDict.Add(MissionType.UrbanDelivery, cycleEntry.UrbanDelivery);
                    newEntry.cycleDict.Add(MissionType.MunicipalUtility, cycleEntry.MunicipalUtility);
                    newEntry.cycleDict.Add(MissionType.Construction, cycleEntry.Construction);

                    Data.Add(newEntry.VehicleGroup, newEntry);
                }
            }
        }

        public struct CycleEntry
        {
            public string VehicleGroup;
            public Tuple<double, double> LongHaul;
            public Tuple<double, double> RegionalDelivery;
            public Tuple<double, double> UrbanDelivery;
            public Tuple<double, double> MunicipalUtility;
            public Tuple<double, double> Construction;

        }

        public struct Entry
        {
            public string VehicleGroup;
            public Dictionary<MissionType, Tuple<double, double>> cycleDict;
        }


        private Tuple<double, double> SplitStringToDoubleTuple(string input)
        {
            var arr = input.Split('/');
            return input.IsNullOrEmpty() ? Tuple.Create(0.0, 0.0) : Tuple.Create(Convert.ToDouble(arr[0]), Convert.ToDouble(arr[1]));
        }
    }
}
