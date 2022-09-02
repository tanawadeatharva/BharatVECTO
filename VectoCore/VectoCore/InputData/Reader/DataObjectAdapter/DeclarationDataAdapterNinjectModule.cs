using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			#region HeavyLorry
			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.Conventional>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S2>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S3>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S4>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P1>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P2>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P3>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P4>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E2>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E3>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E4>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC>();
			#endregion HeavyLorry
		}

		#endregion
	}
}

