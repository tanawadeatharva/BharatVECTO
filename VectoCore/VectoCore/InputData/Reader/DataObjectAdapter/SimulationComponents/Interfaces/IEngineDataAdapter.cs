using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IEngineDataAdapter
    {
        CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle,
            IEngineModeDeclarationInputData mode, Mission mission);

        CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission);
    }
}