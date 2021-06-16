using System;
using System.Collections.Generic;
using VECTO3GUI2020.ViewModel.Interfaces;
using Ninject;
using System.Diagnostics;
using System.Windows.Input;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.Views;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region Member
        private IJobListViewModel _jobListVm;
        private IMainViewModel _bottomView;
		#endregion
        
        #region Commands
        private ICommand _openSettings;

		private ICommand _switchTopView;
		private IWindowHelper _windowHelper;
		private ISettingsViewModel _settingsViewModel;
		private IMainViewModel _currentViewModelTop;

		private Dictionary<string, IMainViewModel> _viewModels = new Dictionary<string, IMainViewModel>(StringComparer.InvariantCultureIgnoreCase);
		private readonly AboutViewModel _aboutViewModel;

		#endregion

		

        public MainWindowViewModel(IWindowHelper windowHelper, ISettingsViewModel settingsViewModel, IJobListViewModel jobListViewModel, IOutputViewModel outputViewModel, AboutViewModel aboutVm)
		{
			_windowHelper = windowHelper;
			_settingsViewModel = settingsViewModel;
			_jobListVm = jobListViewModel;
			_aboutViewModel = aboutVm;
			_currentViewModelTop = _jobListVm;
			_bottomView = outputViewModel;
			
			//_bottomView = new TestViewModel();
            _viewModels.Add("Jobs", _jobListVm);
            _viewModels.Add("Settings", _settingsViewModel);
			_viewModels.Add("About", _aboutViewModel);



		}

		public ICommand SwitchTopView
		{
			get
			{
				return _switchTopView ?? (_switchTopView = new RelayCommand<string>(SwitchTopViewModel, (s) => true));
			}
		}

		public void SwitchTopViewModel(string key)
		{
			CurrentViewModelTop = _viewModels[key];
		}

		public bool JobsSelected
		{
			get
			{
				return CurrentViewModelTop == _jobListVm;
			}
		}

		public bool SettingsSelected
		{
			get
			{ 
				return CurrentViewModelTop == _settingsViewModel;
			}
		}

		public bool AboutSelected
		{
			get
			{
				return CurrentViewModelTop == _aboutViewModel;
			}
		}

		public IMainViewModel CurrentViewModelTop
        {
            get { return _currentViewModelTop;

            }
			set
			{
				if (SetProperty(ref _currentViewModelTop, value)) {
					OnPropertyChanged(nameof(JobsSelected));
					OnPropertyChanged(nameof(SettingsSelected));
					OnPropertyChanged(nameof(AboutSelected));
				}
			}
		}

        public IMainViewModel CurrentViewModelBottom
        {
            get { return _bottomView; }
            set { _bottomView = value; }
        }


		#region CommandImplementations
        #region CommandOpenSettings
        public ICommand OpenSettings
        {
            get{
                return _openSettings ?? (ICommand)new RelayCommand(OpenSettingsExecute);
            }
            private set
            {

            }
        }

        private void OpenSettingsExecute()
		{
            _windowHelper.ShowWindow(_settingsViewModel);
		}


        #region newMultiStage

		public ICommand NewInterimFile => _jobListVm.NewManufacturingStageFile;





		#endregion

        #endregion
















        #endregion

    }
}
