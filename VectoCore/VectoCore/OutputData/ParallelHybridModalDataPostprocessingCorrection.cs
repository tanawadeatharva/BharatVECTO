using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class ParallelHybridModalDataPostprocessingCorrection : ModalDataPostprocessingCorrection
	{
		#region Overrides of ModalDataPostprocessingCorrection

		public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var r = base.ApplyCorrection(modData, runData);



			r.ElectricEnergyConsumption = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
			return r;
		}

		#endregion
	}
}