using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IAirdragDataAdapter
    {
        AirdragData CreateAirdragData(
            IAirdragDeclarationInputData airdragInputData, Mission mission,
            Segment segment);

        AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);
    }
}