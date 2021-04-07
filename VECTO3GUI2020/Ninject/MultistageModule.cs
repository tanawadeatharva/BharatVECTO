using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
    public class MultistageModule : NinjectModule
    {
		public override void Load()
		{

			Bind<IViewModelBase>().To<NewMultiStageJobViewModel>();


			Bind<IMultiStageJobViewModel>().To<MultiStageJobViewModel_v0_1>()
				.Named(MultiStageJobViewModel_v0_1.INPUTPROVIDERTYPE);

			Bind<IVehicleViewModel>().To<DeclarationInterimStageBusVehicleViewModel_v2_8>()
				.Named(DeclarationInterimStageBusVehicleViewModel_v2_8.INPUTPROVIDERTYPE);

			Bind<IManufacturingStageViewModel>().To<ManufacturingStageViewModel_v0_1>()
				.Named(ManufacturingStageViewModel_v0_1.INPUTPROVIDERTYPE);

			Bind<IMultistageAirdragViewModel>().To<MultistageAirdragViewModel>();
		}
	}
}
