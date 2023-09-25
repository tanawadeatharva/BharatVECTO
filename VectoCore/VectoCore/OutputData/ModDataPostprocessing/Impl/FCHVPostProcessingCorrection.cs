using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public class FCHVPostProcessingCorrection : BatteryElectricPostprocessingCorrection
    {
        #region Implementation of IModalDataPostProcessor

        public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
            //PEV Corrections
			var corrected = base.ApplyCorrection(modData, runData);


		



			return corrected;
		}

        #endregion
    }
}