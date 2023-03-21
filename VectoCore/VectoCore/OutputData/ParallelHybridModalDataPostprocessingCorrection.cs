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
			
			var etaChtBatWeighted = 1.0;
			var electricEnergyConsumption = 0.SI<WattSecond>();

			if (runData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting && runData.Mission != null) {
				var vehicleOperation = DeclarationData.VehicleOperation.LookupVehicleOperation(runData.VehicleData.VehicleClass, runData.Mission.MissionType);
				(_, _, etaChtBatWeighted) =
					DeclarationData.CalculateChargingEfficiencyOVCHEV(runData.MaxChargingPower, vehicleOperation,
						runData.BatteryData);
				electricEnergyConsumption = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
			}

			r.ElectricEnergyConsumption_SoC = electricEnergyConsumption;
			r.ElectricEnergyConsumption_Final = electricEnergyConsumption / etaChtBatWeighted;
			return r;
		}

		#endregion
	}
}