using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
    public class MultistageModule : NinjectModule
    {
		public override void Load()
		{
			Bind<IViewModelFactory>().ToFactory();
			Bind<IManufacturingStageEditViewModel>().To<ManufacturingStageEditViewModel>();
		}
	}
}
