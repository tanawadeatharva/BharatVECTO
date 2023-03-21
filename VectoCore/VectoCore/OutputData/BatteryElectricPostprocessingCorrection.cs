using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class BatteryElectricPostprocessingCorrection : IModalDataPostProcessor
	{
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var chgEfficiency = DeclarationData.CalculateChargingEfficiencyPEV(runData);

			return new PEVCorrectedModalData(modData) {
				ElectricEnergyConsumption_SoC = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int),
				ElectricEnergyConsumption_Final = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) / chgEfficiency,
			};
		}

		#endregion
	}
}