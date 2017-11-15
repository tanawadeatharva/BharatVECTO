using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class VTPCombustionEngine : CombustionEngine
    {
        public VTPCombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }


        protected override PerSecond GetEngineSpeed(PerSecond angularSpeed)
        {
            return DataBus.CycleData.LeftSample.EngineSpeed;
        }
    }
}