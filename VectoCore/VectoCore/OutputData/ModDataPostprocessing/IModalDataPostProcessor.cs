using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{

	public interface IModalDataPostProcessorFactory
	{
		IModalDataPostProcessor GetPostProcessor(VectoSimulationJobType jobType);
	}

	
	public interface IModalDataPostProcessor
    {
        ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData);

	}
}