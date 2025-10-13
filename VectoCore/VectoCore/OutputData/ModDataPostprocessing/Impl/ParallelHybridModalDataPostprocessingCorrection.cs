using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public class ParallelHybridModalDataPostprocessingCorrection : ConventionalModalDataPostprocessingCorrection
    {
        #region Overrides of ModalDataPostprocessingCorrection

        protected override ICorrectedModalData DoApplyCorrection(IModalDataContainer modData, VectoRunData runData)
        {
            var r = base.DoApplyCorrection(modData, runData);

            var etaChtBatWeighted = 1.0;
            var electricEnergyConsumption = 0.SI<WattSecond>();

            if (runData.OVCMode == OvcHevMode.ChargeDepleting && runData.Mission != null) {
                var vehicleOperation = DeclarationData.VehicleOperation.LookupVehicleOperation(runData.Mission.BusParameter?.BusGroup ?? runData.VehicleData.VehicleClass, runData.Mission.MissionType);
                etaChtBatWeighted =
                    DeclarationData.CalculateChargingEfficiencyOVCHEV(runData, vehicleOperation).EtaChargingWeighted;
                electricEnergyConsumption = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
            }

			r.ElectricEnergyConsumption_SoC = electricEnergyConsumption;
            r.ElectricEnergyConsumption_Final = electricEnergyConsumption / etaChtBatWeighted;
            return r;
        }

        #endregion
    }

	public class BatteryOnlyHybridModalDataPostprocessingCorrection : BatteryElectricPostprocessingCorrection
	{
		#region Overrides of BatteryElectricPostprocessingCorrection

        protected override ICorrectedModalData DoApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var r = base.DoApplyCorrection(modData, runData);
			foreach (var fuel in modData.FuelData) {
				if (r.FuelCorrection.ContainsKey(fuel.FuelType)) {
                    continue;
				}
				r.FuelCorrection[fuel.FuelType] = new ZeroFuelConsumptionCorrection(fuel, modData.Distance, modData.Duration);
			}
			return r;
		}

		protected override AbstractCorrectedModalData GetModalDataCorrection(IModalDataContainer modData)
		{
			return new BatteryOnlyHybridCorrectedModalData(modData);
		}

		#endregion
	}
}