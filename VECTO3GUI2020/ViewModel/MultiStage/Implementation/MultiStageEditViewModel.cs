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

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public interface IMultiStageEditViewModel
	{

	}

	public class MultiStageEditViewModel : ViewModelBase, IMultiStageEditViewModel
	{
		private readonly Settings _settings = Settings.Default;
		private ICommand _addVifCommand;
		private string _vifPath;
		private IDialogHelper _dialogHelper;
		private IXMLInputDataReader _inputDataReader;
		private IVehicleViewModel _vehicleViewModel;
		private IComponentViewModelFactory _componentViewModelFactory;
		private int _stageCount;
		public string VifPath { get => _vifPath; set => SetProperty(ref _vifPath, value); }
		public int StageCount
		{
			get => _stageCount;
			set => SetProperty(ref _stageCount, value);
		}


		public IVehicleViewModel VehicleViewModel
		{
			get => _vehicleViewModel;
			set => SetProperty(ref _vehicleViewModel, value);
		}




		public MultiStageEditViewModel(IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IComponentViewModelFactory componentViewModelFactory)
		{
			_inputDataReader = inputDataReader;
			_dialogHelper = dialogHelper;
			_componentViewModelFactory = componentViewModelFactory;
			Title = "New interim stage file";
			VifPath = "Select VIF File";
		}



		#region AddVifCommand

		public ICommand AddVifFile
		{
			get => _addVifCommand ?? new RelayCommand(addVifFileExecute, () => true);
		}

		private void addVifFileExecute()
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
			
			
			var lastManufacturingStageInputData = inputDataProvider.JobInputData.ManufacturingStages.Last();

			VifPath = fileName;

			_stageCount = inputDataProvider.JobInputData.ManufacturingStages.Count + 1;
			
			
			


			VehicleViewModel =
				_componentViewModelFactory.CreateVehicleViewModel(lastManufacturingStageInputData.Vehicle);

		}

		#endregion

	}
}
