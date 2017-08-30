using System.Collections.Generic;
using System.Windows.Input;

namespace HashingTool.ViewModel
{
	public class HomeViewModel : ObservableObject, IMainView
	{
		private readonly ApplicationViewModel _applicationViewModel;

		public HomeViewModel() {}

		public HomeViewModel(ApplicationViewModel applicationViewModel)
		{
			_applicationViewModel = applicationViewModel;
		}


		public string Name
		{
			get { return "Home"; }
		}

		public List<IMainView> MainViewModels
		{
			get { return ApplicationViewModel.AvailableViews; }
		}

		public ICommand ChangeViewCommand
		{
			get { return _applicationViewModel == null ? null : _applicationViewModel.ChangeViewCommand; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

	}
}
