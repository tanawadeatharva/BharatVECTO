using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class ParallelHybridModalDataPostprocessingCorrection : ModalDataPostprocessingCorrection
	{
		#region Overrides of ModalDataPostprocessingCorrection

		public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var r = base.ApplyCorrection(modData, runData);
			
			var vehicleOperation = DeclarationData.VehicleOperation.LookupVehicleOperation(runData.VehicleData.VehicleClass, runData.Mission.MissionType);
			var etaChtBatWeighted = 1.0;

			if (runData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting) {
				(_, _, etaChtBatWeighted) =
					DeclarationData.CalculateChargingEfficiencyOVCHEV(runData.MaxChargingPower, vehicleOperation,
						runData.BatteryData);
			}

			r.ElectricEnergyConsumption_SoC = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
			r.ElectricEnergyConsumption_Final = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) / etaChtBatWeighted;
			return r;
		}

		#endregion
	}
}