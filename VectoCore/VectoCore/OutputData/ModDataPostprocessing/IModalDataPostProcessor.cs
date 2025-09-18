using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{

	public interface IModalDataPostProcessorFactory
	{
		IModalDataPostProcessor GetPostProcessor(VectoSimulationJobType jobType);
	}

	
	public interface IModalDataPostProcessor
    {
        ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData);

	}
}