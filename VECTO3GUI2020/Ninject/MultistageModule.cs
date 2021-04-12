using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.Models.SimulationComponent;
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

			Bind<IViewModelBase>().To<NewMultiStageJobViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetNewMultistageJobViewModel());

			Bind<IMultiStageJobViewModel>().To<MultiStageJobViewModel_v0_1>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetMultiStageJobViewModel(null));

			Bind<IVehicleViewModel>().To<DeclarationInterimStageBusVehicleViewModel_v2_8>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f)=>f.GetInterimStageVehicleViewModel());

			Bind<IManufacturingStageViewModel>().To<ManufacturingStageViewModel_v0_1>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetManufacturingStageViewModel(null));

			Bind<IMultistageAirdragViewModel>().To<MultistageAirdragViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetMultistageAirdragViewModel());

			Bind<IMultistageAuxiliariesViewModel>().To<MultistageAuxiliariesViewModel>().
				NamedLikeFactoryMethod((IMultiStageViewModelFactory f) => f.GetAuxiliariesViewModel(null));
		}
	}
}
