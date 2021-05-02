using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

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
		private ICommand _saveVifCommand;

		public ICommand SaveVIFCommand
		{
			get
			{
				return _saveVifCommand ?? new RelayCommand(() => {
					SaveVIF(this);
				}, () => true);
			}
		}

		private static void SaveVIF(IMultistageVIFInputData inputData)
		{
			var vifInputData = inputData;
		}

		private ICommand _saveInputDataCommand;
		private ICommand _saveInputDataAsCommand;

		public ICommand SaveInputDataCommand =>
			_saveInputDataCommand ?? new RelayCommand(() => {
				SaveInputDataExecute(filename:_vehicleInputDataFilePath);
			}, () => _vehicleInputDataFilePath != null);

		public ICommand SaveInputDataAsCommand =>
			_saveInputDataAsCommand ?? new RelayCommand(() => {
				SaveInputDataExecute(filename:null);
			}, () => true);

		private void SaveInputDataExecute(string filename)
		{
			if(_manufacturingStageViewModel.Vehicle is IMultistageVehicleViewModel vehicleViewModel)
			{
				if (vehicleViewModel.HasErrors) {
					_dialogHelper.Value.ShowMessageBox(string.Join("\n", vehicleViewModel.Errors.Values), "Error" );
					return;
				}
			}
			


			if (filename == null) {
				filename = _dialogHelper.Value.SaveToXMLDialog(Settings.Default.DefaultFilePath);
				if (filename == null) {
					return;
				}
			}

			var vehicleWriter =
				_multistageDependencies.XMLWriterFactory.CreateVehicleWriter(_manufacturingStageViewModel.Vehicle);



			var xElement = vehicleWriter.GetElement();
			var xDoc = xElement.CreateWrapperDocument(XMLNamespaces.V28);
			Debug.WriteLine(xElement.CreateWrapperDocument(XMLNamespaces.V28).ToString());

			var validator = new XMLValidator(xDoc.ToXmlDocument());
			var valid = validator.ValidateXML(XmlDocumentType.DeclarationJobData);
			if (!valid) {
				_dialogHelper.Value.ShowMessageBox($"Invalid Document: {validator.ValidationError}", "Error");
				xDoc.Save(filename, SaveOptions.OmitDuplicateNamespaces);
				LoadVehicleData(filename);
			} else {
				xDoc.Save(filename, SaveOptions.OmitDuplicateNamespaces);
			}
		}


		private ICommand _loadVehicleDataCommand;
		private readonly Lazy<IDialogHelper> _dialogHelper;
		private readonly Lazy<IXMLInputDataReader> _inputDataReader;
		private string _vehicleInputDataFilePath = null;
		private readonly IMultistageDependencies _multistageDependencies;

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

			LoadVehicleData(fileName);
			return;
		}

		private bool LoadVehicleData(string fileName)
		{
			IDeclarationInputDataProvider inputData;
			IVehicleDeclarationInputData vehicleInputData;
			try {
				inputData = (IDeclarationInputDataProvider)_inputDataReader.Value.CreateDeclaration(fileName);
				vehicleInputData = inputData.JobInputData.Vehicle;
				_manufacturingStageViewModel.SetInputData(vehicleInputData);

				VehicleInputDataFilePath = fileName;
			} catch (Exception e) {
				_dialogHelper.Value.ShowMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return true;
			}

			return false;
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
			_multistageDependencies = multistageDependencies;
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