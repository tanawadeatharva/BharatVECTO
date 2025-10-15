using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IGearboxDataAdapter
    {
        GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
            IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes);

        GearboxData CreateGearboxData(IVehicleDeclarationInputData vehicle, VectoRunData runData,
            IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes, IGearboxDeclarationInputData gearbox,
            ITorqueConverterDeclarationInputData torqueConverter);

        GearboxData CreateGearboxData(VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalculator, IIEPCDeclarationInputData iepc);

        ShiftStrategyParameters CreateGearshiftData(double axleRatio,
            PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount);
    }
}