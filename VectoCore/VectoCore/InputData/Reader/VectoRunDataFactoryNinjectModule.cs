using Ninject.Modules;

namespace TUGraz.VectoCore.InputData.Reader
{
    class VectoRunDataFactoryNinjectModule : NinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IVectoRunDataFactoryFactory>().To<VectoRunDataFactoryFactory>().InSingletonScope();
		}

		#endregion
	}
}
