using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{

	public class PostProcessingNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IModalDataPostProcessorFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(VectoSimulationJobType.BatteryElectricVehicle.ToString());
			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(VectoSimulationJobType.IEPC_E.ToString());
			Bind<IModalDataPostProcessor>().To<BatteryElectricPostprocessingCorrection>()
				.Named(VectoSimulationJobType.MultiplePowertrains.ToString());

			Bind<IModalDataPostProcessor>().To<SerialHybridModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.SerialHybridVehicle.ToString());
			Bind<IModalDataPostProcessor>().To<SerialHybridModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.IEPC_S.ToString());

			// todo amogoda: m12 - check usage of SerialHybridModalDataPostprocessingCorrection is correct 
			Bind<IModalDataPostProcessor>().To<SerialHybridModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.FCHV.ToString());
			Bind<IModalDataPostProcessor>().To<SerialHybridModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.FCHV_IEPC.ToString());

			Bind<IModalDataPostProcessor>().To<ParallelHybridModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.ParallelHybridVehicle.ToString());
			Bind<IModalDataPostProcessor>().To<ParallelHybridModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.IHPC.ToString());

			Bind<IModalDataPostProcessor>().To<EngineOnlyPostprocessingCorrection>()
				.Named(VectoSimulationJobType.EngineOnlySimulation.ToString());

			Bind<IModalDataPostProcessor>().To<ConventionalModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.ConventionalVehicle.ToString());
		}

		#endregion
	}
}