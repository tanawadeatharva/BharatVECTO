using VECTO3GUI2020.ViewModel.Interfaces;
using Ninject;
using System.Diagnostics;
using System.Windows.Input;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.Views;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region Member
        private IMainViewModel _topView;
        private IMainViewModel _bottomView;
		#endregion
        
        #region Commands
        private ICommand _openSettings;
        private ICommand _editJob;
		private IWindowHelper _windowHelper;
		private ISettingsViewModel _settingsViewModel;

		#endregion


        public MainWindowViewModel(IWindowHelper windowHelper, ISettingsViewModel settingsViewModel)
		{
			_windowHelper = windowHelper;
			_settingsViewModel = settingsViewModel;
		}

        [Inject]
        public IMainViewModel CurrentViewModelTop
        {
            get { return _topView;

            }
            set { _topView = value; }
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

 

        #endregion


















        #endregion

    }
}
