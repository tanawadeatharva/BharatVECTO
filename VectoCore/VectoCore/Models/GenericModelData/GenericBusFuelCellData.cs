using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{
    internal class GenericBusFuelCellData
    {
        private static string FuelCellEfficiencyMapFile = $"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.FuelCell_EfficiencyMap.vmap";

        private static double H2_FC_NCV => 120;
        private const string POWER_OUTPUT_COL = "powerOutput";
        private const string FUEL_CONSUMPTION_COL = "fuelConsumption";

        public static TableData CreateFuelCellPowerOutputMap(Watt FCSratedPower)
        {
            TableData powerMap = new TableData();
            powerMap.Columns.Add(POWER_OUTPUT_COL);
            powerMap.Columns.Add(FUEL_CONSUMPTION_COL);

            var effData = ReadCsvResource(FuelCellEfficiencyMapFile);

            foreach (DataRow row in effData.Rows)
            {
                var powerNormalized = row.ParseDouble("Power");
                var efficiency = row.ParseDouble("Efficiency");

                var powerOutput = powerNormalized * FCSratedPower.Value();
                var fuelConsumption = powerOutput / ((efficiency / 100) * (H2_FC_NCV / 3600));

                var newRow = powerMap.NewRow();
                newRow[POWER_OUTPUT_COL] = powerOutput.ToXMLFormat(2);
                newRow[FUEL_CONSUMPTION_COL] = fuelConsumption.ToXMLFormat(2);
                powerMap.Rows.Add(newRow);
            }

            return powerMap;
        }

        private static TableData ReadCsvResource(string ressourceId)
        {
            return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);
        }
    }
}
