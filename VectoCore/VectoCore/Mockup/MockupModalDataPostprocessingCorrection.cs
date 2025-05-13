using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;

namespace TUGraz.VectoCore.Mockup
{
    public class MockupModalDataPostprocessingCorrection : ModalDataPostProcessingCorrectionBase
    {
        public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
        {
            if (runData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.FCHV, VectoSimulationJobType.IEPC_E, VectoSimulationJobType.FCHV_IEPC))
            {
                var corrected = new PEVCorrectedModalData(modData)
                {
                    CorrectedAirDemand = 0.SI<NormLiter>(),
                    DeltaAir = 0.SI<NormLiter>(),
                    WorkBusAux_elPS_SoC_ElRange = 0.SI<WattSecond>(),
                    ElectricEnergyConsumption_SoC = 1.SI<WattSecond>(), 
                    ElectricEnergyConsumption_Final = 1.SI<WattSecond>()
                };

                return corrected;
            }
            else
            {
                return new CorrectedModalData(modData);
            }
        }

    }
}
