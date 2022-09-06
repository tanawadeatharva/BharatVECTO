using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			#region HeavyLorry
			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.Conventional>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S2>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S3>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S4>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P1>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P2>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P3>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P4>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E2>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E3>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E4>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC>();

			Bind<IDeclarationDataAdapter>().To<HeavyLorry.DeclarationDataAdapterHeavyLorry.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.Exempted>();
			#endregion HeavyLorry

			#region PrimaryBus

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.Conventional>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S2>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S3>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S4>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P1>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P2>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P2_5>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P3>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P4>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E2>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E3>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E4>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC>();

			Bind<IDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.Exempted>();

			#endregion

			#region CompletedBus Generic

			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.Conventional>();


			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S2>();


			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S3>();


			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S4>();

			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S4>();

			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P1>();


			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2>();


			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2_5>();


			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P3>();

			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P4>();
            Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E2>();
			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E3>();
			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E4>();
			Bind<IGenericCompletedBusDataAdapter>()
				.To<DeclarationDataAdapterGenericCompletedBus.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC>();

            #endregion

            #region CompletedBus Specific
            Bind<ISpecificCompletedBusDataAdapter>()
			    .To<DeclarationDataAdapterSpecificCompletedBus.Conventional>()
			    .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.Conventional>();


            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S2>();


            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S3>();


            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S4>();

            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S4>();

            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P1>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P1>();


            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2>();


            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P2_5>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2_5>();


            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P3>();

            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P4>();
            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E2>();
            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E3>();
            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E4>();
            Bind<ISpecificCompletedBusDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC>();
            #endregion
        }

        #endregion
    }
}

