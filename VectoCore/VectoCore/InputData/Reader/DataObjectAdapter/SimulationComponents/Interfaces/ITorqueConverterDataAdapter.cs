using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface ITorqueConverterDataAdapter
    {
        TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
            ITorqueConverterDeclarationInputData torqueConverter, double ratio,
            CombustionEngineData engineData);
    }
}