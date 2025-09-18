using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation
{
    public interface IPowertrainBuilder
    {
        IVehicleContainer Build(VectoRunData data, IModalDataContainer modData, ISumData sumWriter = null);

		IExemptedVehicleContainer BuildExempted(VectoRunData data);

        //IShiftStrategy GetShiftStrategy(IVehicleContainer container);

        //string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType, bool isTestPowerTrain = false);

    }
}