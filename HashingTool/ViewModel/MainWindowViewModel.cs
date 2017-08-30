using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HashingTool.Views;

namespace HashingTool.ViewModel
{
	public class MainWindowViewModel : ObservableObject
	{
		private ICommand _changeViewCommand;

		private IMainView _currentView;
		private List<IMainView> _availableViews;
		private ICommand _homeView;

		public MainWindowViewModel()
		{
			//var homeView = new HomeViewModel();
			_availableViews = new List<IMainView> {
				new HashComponentDataViewModel(this),
				new VerifyInputDataViewModel(this)
			};

			CurrentViewModel = _availableViews[0];

			_homeView = new RelayCommand(() => CurrentViewModel = _availableViews[0]);
		}

		public List<IMainView> MainViewModels
		{
			get { return _availableViews ?? (_availableViews = new List<IMainView>()); }
		}

		public IMainView CurrentViewModel
		{
			get { return _currentView; }
			set {
				if (_currentView == value) {
					return;
				}
				_currentView = value;
				RaisePropertyChanged("CurrentViewModel");
			}
		}

		public ICommand ChangeViewCommand
		{
			get { return _changeViewCommand ?? (_changeViewCommand = new RelayCommand<IMainView>(ChangeViewModel)); }
		}

		public ICommand ShowHomeView
		{
			get { return _homeView; }
		}

		private void ChangeViewModel(IMainView mainView)
		{
			if (!MainViewModels.Contains(mainView)) {
				return;
			}

			CurrentViewModel = MainViewModels.FirstOrDefault(mv => mv == mainView);
		}
	}
}
