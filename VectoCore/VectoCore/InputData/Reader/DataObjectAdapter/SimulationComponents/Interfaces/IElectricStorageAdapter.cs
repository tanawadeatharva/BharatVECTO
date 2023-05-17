using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IElectricStorageAdapter
    {
        BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryInputData,
            VectoSimulationJobType jobType,
            bool ovc);

        SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData reessInputData);
    }
}