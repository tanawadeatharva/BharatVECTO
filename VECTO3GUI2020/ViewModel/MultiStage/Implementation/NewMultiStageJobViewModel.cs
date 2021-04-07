using System;
using System.CodeDom;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class NewMultiStageJobViewModel : ViewModelBase
	{
		private readonly Settings _settings = Settings.Default;
		private ICommand _addVifCommand;
		private string _vifPath;
		private IDialogHelper _dialogHelper;
		private IXMLInputDataReader _inputDataReader;
		private IMultiStageViewModelFactory _vmFactory;
		private IMultiStageJobViewModel _multistageJobViewModel;
		public string VifPath { get => _vifPath; set => SetProperty(ref _vifPath, value); }


		public IMultiStageJobViewModel MultiStageJobViewModel
		{
			get { return _multistageJobViewModel; }
			set { SetProperty(ref _multistageJobViewModel, value); }
		}



		public NewMultiStageJobViewModel(IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IMultiStageViewModelFactory vmFactory)
		{
			_inputDataReader = inputDataReader;
			_dialogHelper = dialogHelper;
			_vmFactory = vmFactory;
			Title = "New Multistage file";
			VifPath = "Select VIF File";
		}



		#region AddVifCommand

		public ICommand AddVifFile
		{
			get => _addVifCommand ?? new RelayCommand(AddVifFileExecute, () => true);
		}

		private void AddVifFileExecute()
		{
			
			var fileName = _dialogHelper.OpenXMLFileDialog(_settings.DefaultFilePath);
			IMultistageBusInputDataProvider inputDataProvider = null;
			try {
				inputDataProvider = _inputDataReader.Create(fileName) as IMultistageBusInputDataProvider;
			}
            catch(Exception e) {
				_dialogHelper.ShowMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			if (inputDataProvider == null) {

				_dialogHelper.ShowMessageBox("invalid input file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			MultiStageJobViewModel =
				_vmFactory.CreateMultiStageJobViewModel(inputDataProvider.GetType().ToString(), inputDataProvider);
			VifPath = fileName;
		}

		

		#endregion

	}
}
