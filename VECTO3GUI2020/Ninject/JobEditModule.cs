using Ninject.Extensions.Factory;
using Ninject.Modules;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Trailer;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;

namespace VECTO3GUI2020.Ninject
{
    public class JobEditModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IJobEditViewModelFactory>().ToFactory(() => new UseFirstArgumentTypeAsNameInstanceProvider());
            Bind<IJobEditViewModel>().To<DeclarationJobEditViewModel_v1_0>().Named(DeclarationJobEditViewModel_v1_0.VERSION);
            Bind<IJobEditViewModel>().To<DeclarationJobEditViewModel_v2_0>().Named(DeclarationJobEditViewModel_v2_0.VERSION);

        }

    }
}
