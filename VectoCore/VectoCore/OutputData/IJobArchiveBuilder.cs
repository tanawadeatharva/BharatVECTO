
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.OutputData
{
    public  interface IJobArchiveBuilder
    {
        ISimulatorFactory SimulatorFactory { get; set; }

        void Build();
    }
}
