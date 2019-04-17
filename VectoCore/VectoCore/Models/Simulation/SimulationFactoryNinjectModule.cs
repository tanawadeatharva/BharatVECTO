using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.Models.Simulation
{
	public class SimulationFactoryNinjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<ISimulatorFactoryFactory>().ToFactory();

			Bind<ISimulatorFactory>().To<SimulatorFactory>();

			Bind<IDeclarationReport>().To<NullDeclarationReport>();
			Bind<IVTPReport>().To<NullVTPReport>();

			//if (Kernel != null && !Kernel.HasModule(typeof(PowertrainBuilderInjectModule).FullName)) {
			//	Kernel.Load(new[] { new PowertrainBuilderInjectModule() });
			//}
		}

		#endregion
	}

	internal class NullDeclarationReport : IDeclarationReport
	{
		#region Implementation of IDeclarationReport

		public void InitializeReport(VectoRunData modelData)
		{

		}

		public void PrepareResult(LoadingType loading, Mission mission, VectoRunData runData)
		{

		}

		public void AddResult(LoadingType loadingType, Mission mission, VectoRunData runData, IModalDataContainer modData)
		{

		}

		#endregion
	}

	internal class NullVTPReport : IVTPReport
	{
		#region Implementation of IDeclarationReport

		public void InitializeReport(VectoRunData modelData)
		{

		}

		public void PrepareResult(LoadingType loading, Mission mission, VectoRunData runData)
		{

		}

		public void AddResult(LoadingType loadingType, Mission mission, VectoRunData runData, IModalDataContainer modData)
		{

		}

		#endregion

		#region Implementation of IVTPReport

		public IVectoHash InputDataHash { set { } }

		public IManufacturerReport ManufacturerRecord { set { } }

		public IVectoHash ManufacturerRecordHash { set { } }

		#endregion
	}
}
