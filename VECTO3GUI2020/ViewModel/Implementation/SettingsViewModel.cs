using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private Settings _settings;

        private ICommand _changePath;


		private IDialogHelper _dialogHelper;


		private String _defaultFilePath;
        public String DefaultFilePath
        {
            get => _defaultFilePath;
            set
            {
                _settings.DefaultFilePath = value;
				_settings.Save();
                SetProperty(ref _defaultFilePath, value, "DefaultFilePath");
            }
        }
        public SettingsViewModel(IDialogHelper dialogHelper)
		{
			base.Title = "Settings";
            _settings = Settings.Default;
            _defaultFilePath = _settings.DefaultFilePath;
			_dialogHelper = dialogHelper;
		}


        public ICommand ChangeFilePath
        {
            get
            {
                return _changePath ?? new RelayCommand(()=>
                {


                    //C:\Users\Harry\source\repos\vecto-gui\VectoCore\VectoCoreTest\TestData\XML\XMLReaderDeclaration

					
                    var new_path = _dialogHelper.OpenFolderDialog(DefaultFilePath);

                    if (new_path != null)
                    {
						DefaultFilePath = new_path;
                    }


                }, () => { return true; });
            }
            private set
            {
                _changePath = value;
                OnPropertyChanged();
            }
        }


    }
}
