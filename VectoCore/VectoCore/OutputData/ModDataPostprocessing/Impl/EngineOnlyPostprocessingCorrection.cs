using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
	public class EngineOnlyPostprocessingCorrection : IModalDataPostProcessor
	{
		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			return new EngineOnlyCorrectedModalData(modData);
		}
	}
}