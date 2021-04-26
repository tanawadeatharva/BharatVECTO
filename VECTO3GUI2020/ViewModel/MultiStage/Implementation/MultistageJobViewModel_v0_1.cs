using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	//AGGREGATE MULTISTAGEDEPENDENCIES

	public class MultiStageJobViewModel_v0_1 : ViewModelBase, IMultiStageJobViewModel, IMultistageVIFInputData, IMultistageBusInputDataProvider
	{
		private IDeclarationMultistageJobInputData _jobInputData;

		private IManufacturingStageViewModel _manufacturingStageViewModel;
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		private IList<IManufacturingStageInputData> _manufacturingStages;
		private IManufacturingStageInputData _consolidateManufacturingStage;
		private VectoSimulationJobType _jobType;
		private bool _inputComplete;
		private readonly IMultiStageViewModelFactory _vmFactory;

		public IManufacturingStageViewModel ManufacturingStageViewModel
		{
			get => _manufacturingStageViewModel;
			set => SetProperty(ref _manufacturingStageViewModel, value);
		}

		#region Commands
		private ICommand _saveCommand;

		public ICommand SaveCommand
		{
			get
			{
				return _saveCommand ?? new RelayCommand(() => {
					SaveFile(this);
				}, () => true);
			}
		}

		private static void SaveFile(IMultistageVIFInputData inputData)
		{
			var vifInputData = inputData;
			
		}


		private ICommand _loadVehicleDataCommand;
		private readonly Lazy<IDialogHelper> _dialogHelper;
		private readonly Lazy<IXMLInputDataReader> _inputDataReader;
		private string _vehicleInputDataFilePath = "Select Vehicle Input Data";

		public ICommand LoadVehicleDataCommand
		{
			get
			{
				return _loadVehicleDataCommand ?? new RelayCommand(LoadVehicleDataExecute, () => true);
			}
		}

		private void LoadVehicleDataExecute()
		{
			var fileName = _dialogHelper.Value.OpenXMLFileDialog(Settings.Default.DefaultFilePath);
			if (fileName == null) {
				return;
			}

			IDeclarationInputDataProvider inputData;
			IVehicleDeclarationInputData vehicleInputData;
			try {
				inputData = (IDeclarationInputDataProvider)_inputDataReader.Value.CreateDeclaration(fileName);
				vehicleInputData = inputData.JobInputData.Vehicle;
				_manufacturingStageViewModel.SetInputData(vehicleInputData);
			} catch (Exception e) {
				_dialogHelper.Value.ShowMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			
			return;
		}

		public string VehicleInputDataFilePath
		{
			get => _vehicleInputDataFilePath;
			set => SetProperty(ref _vehicleInputDataFilePath, value);
		}
		#endregion


		public MultiStageJobViewModel_v0_1(IMultistageBusInputDataProvider inputData, IMultiStageViewModelFactory vmFactory, IMultistageDependencies multistageDependencies )
		{

			_jobInputData = inputData.JobInputData;
			_vmFactory = vmFactory;
			_consolidateManufacturingStage = _jobInputData.ConsolidateManufacturingStage;
			_manufacturingStages =_jobInputData.ManufacturingStages;
			_primaryVehicle = _jobInputData.PrimaryVehicle;
			_dialogHelper = multistageDependencies.DialogHelperLazy;
			_inputDataReader = multistageDependencies.InputDataReaderLazy;
			_manufacturingStageViewModel =
				vmFactory.GetManufacturingStageViewModel(_consolidateManufacturingStage);
		}


		#region Implementation of IInputDataProvider

		public DataSource DataSource => throw new NotImplementedException();

		#endregion

		#region Implementation of IMultistageVIFInputData

		public IVehicleDeclarationInputData VehicleInputData => _manufacturingStageViewModel.Vehicle;

		public IMultistageBusInputDataProvider MultistageInputData => this;

		#endregion

		#region Implementation of IDeclarationInputDataProvider


		public IDeclarationMultistageJobInputData JobInputData => _jobInputData;

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => throw new NotImplementedException();

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData => _primaryVehicle;

		public XElement XMLHash => throw new NotImplementedException();

		#endregion

		#region Implementation of IDeclarationMultistageJobInputData

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get => _primaryVehicle;
			set => _primaryVehicle = value;
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get => _manufacturingStages;
			set => _manufacturingStages = value;
		}

		public IManufacturingStageInputData ConsolidateManufacturingStage
		{
			get => _consolidateManufacturingStage;
			set => _consolidateManufacturingStage = value;
		}

		public VectoSimulationJobType JobType
		{
			get => _jobType;
			set => _jobType = value;
		}

		public bool InputComplete
		{
			get => _inputComplete;
			set => _inputComplete = value;
		}

		#endregion
	}

	public interface IMultiStageJobViewModel : IDeclarationMultistageJobInputData
	{
		IManufacturingStageViewModel ManufacturingStageViewModel { get; }
	}
}