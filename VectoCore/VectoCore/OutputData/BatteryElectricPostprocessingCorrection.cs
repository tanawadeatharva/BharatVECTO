using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class BatteryElectricPostprocessingCorrection : IModalDataPostProcessor
	{
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			
			return new PEVCorrectedModalData(modData) {
				ElectricEnergyConsumption = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int),
			};
		}

		#endregion
	}
}