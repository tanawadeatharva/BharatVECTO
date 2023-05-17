using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IADASDataAdapter
    {
        VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemDeclarationInputData adas);
    }
}