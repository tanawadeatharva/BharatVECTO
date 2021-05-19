using System.Windows;
using Ninject;
using Ninject.Extensions.ChildKernel;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.Model;
using TUGraz.VectoCore;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Model.Implementation;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Vehicle;
using VECTO3GUI2020.ViewModel;

namespace VECTO3GUI2020
{
    public partial class App : Application
    {

        private IKernel container;

		protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ConfigureMainWindow();
            
        }

        private void ConfigureContainer()
        {
            container = new StandardKernel(
                new VectoNinjectModule(),
                new JobEditModule(),
                new ComponentModule(),
				new DocumentModule(),
				new XMLWriterFactoryModule(),
                new FactoryModule(),
                new MultistageModule()
			);



			container.Bind<IJobListViewModel>().To<JobListViewModel>();
            container.Bind<IMainWindowViewModel>().To<MainWindowViewModel>();
            container.Bind<IMainViewModel>().To<JobListViewModel>();
            container.Bind<ISettingsViewModel>().To<SettingsViewModel>();
			container.Bind<IOutputViewModel>().To<OutputViewModel>().InSingletonScope();
            container.Bind<ISettingsModel>().To<SettingsModel>();

			container.Bind<IDialogHelper>().To<DialogHelper>().InSingletonScope();
			container.Bind<IWindowHelper>().To<WindowHelper>();

		}

        private void ConfigureMainWindow()
        {
			var mainwindow = container.Get<MainWindow>();
            this.MainWindow = mainwindow;
			Application.Current.MainWindow = mainwindow;
            this.MainWindow.Show();
		}

    }

}
