using Ninject;
using VECTO3.ViewModel.Interfaces;
using VECTO3.Views;

namespace VECTO3.ViewModel.Impl
{
	public class MainWindowViewModel : ObservableObject, IMainWindowViewModel
	{
		
		private IMainView _currentViewModel;

		public MainWindowViewModel(IKernel kernel)
		{
			Kernel = kernel;
			CurrentViewModel = Kernel.Get<IJoblistViewModel>();
		}

		

		public IMainView CurrentViewModel
		{
			get { return _currentViewModel; }
			set { SetProperty(ref _currentViewModel, value); }
		}


	}
}
