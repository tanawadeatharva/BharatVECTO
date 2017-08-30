using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HashingTool.Views;

namespace HashingTool.ViewModel
{
	public class ApplicationViewModel : ObservableObject
	{
		private ICommand _changeViewCommand;
		public static ICommand HomeView;

		private IMainView _currentView;
		public static List<IMainView> AvailableViews;
		

		public ApplicationViewModel()
		{
			var homeView = new HomeViewModel(this);
			AvailableViews = new List<IMainView> {
				new HashComponentDataViewModel(this),
				new VerifyInputDataViewModel(this)
			};

			CurrentViewModel = homeView;

			HomeView = new RelayCommand(() => CurrentViewModel = homeView);
		}

		public List<IMainView> MainViewModels
		{
			get { return AvailableViews ?? (AvailableViews = new List<IMainView>()); }
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

		public ICommand ShowHomeViewCommand
		{
			get { return HomeView; }
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
