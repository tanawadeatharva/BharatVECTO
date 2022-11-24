using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class BatteryElectricPostprocessingCorrection : IModalDataPostProcessor
	{
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			return new NoCorrectionModalData(modData);
		}

		#endregion
	}
}