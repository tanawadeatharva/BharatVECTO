using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{
    public interface IModalDataPostProcessor
    {
        ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData);
    }
}