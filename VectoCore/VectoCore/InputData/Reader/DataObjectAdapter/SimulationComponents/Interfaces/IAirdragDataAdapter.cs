using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IAirdragDataAdapter
    {
        AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragInputData,
			IVehicleInMotionChargingDeclaration imcData, Mission mission,
			Segment segment, OvcHevMode ovcMode, double cycleShareDistanceHighway);

        AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission);
    }
}