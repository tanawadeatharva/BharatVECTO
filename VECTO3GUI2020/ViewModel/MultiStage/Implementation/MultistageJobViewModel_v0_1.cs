using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
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

	public interface IMultiStageJobViewModel : IDeclarationMultistageJobInputData, IMultistageVIFInputData, IMultistageBusInputDataProvider, IJobViewModel, IEditViewModel
	{
		IManufacturingStageViewModel ManufacturingStageViewModel { get; }
	}


	public class MultiStageJobViewModel_v0_1 : ViewModelBase, IMultiStageJobViewModel
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

		public MultiStageJobViewModel_v0_1(IMultistageBusInputDataProvider inputData, IMultiStageViewModelFactory vmFactory, IMultistageDependencies multistageDependencies, IXMLInputDataReader inputDataReader)
		{
			
			_dataSource = inputData.DataSource;
			_jobInputData = inputData.JobInputData;
			_inputData = inputData;
			_vmFactory = vmFactory;
			_consolidateManufacturingStage = _jobInputData.ConsolidateManufacturingStage;
			_manufacturingStages = _jobInputData.ManufacturingStages;
			_primaryVehicle = _jobInputData.PrimaryVehicle;
			_dialogHelper = multistageDependencies.DialogHelperLazy;
			_inputDataReader = inputDataReader;
			_manufacturingStageViewModel =
				vmFactory.GetManufacturingStageViewModel(_consolidateManufacturingStage);

			// QUESTION: HEV/PEV ?
			//var hybridElectric = inputData.PrimaryVehicleData.Vehicle.HybridElectricHDV;
			//_manufacturingStageViewModel.VehicleViewModel.PrimaryVehicleHybridElectric = hybridElectric;
			_multistageDependencies = multistageDependencies;
		}


		#region Commands



		private ICommand _closeWindowCommand;
		public ICommand CloseWindowCommand
		{
			get
			{
				return _closeWindowCommand ?? new RelayCommand<Window>(window => CloseWindow(window, _dialogHelper.Value), window => true);
			}
		}


		private ICommand _saveVifCommand;

		public ICommand SaveVIFCommand
		{
			get
			{
				return _saveVifCommand ?? new RelayCommand(() => {
					if (_manufacturingStageViewModel.Vehicle is IMultistageVehicleViewModel vehicleViewModel)
					{
						if (vehicleViewModel.HasErrors) {
							var errorMessage = "Vehicle\n";
							var vehicleErrorInfo = vehicleViewModel as IDataErrorInfo;
							errorMessage += vehicleErrorInfo.Error.Replace(",", "\n");

							var auxiliariesErrorInfo =
								vehicleViewModel.MultistageAuxiliariesViewModel as IDataErrorInfo;
							if (!auxiliariesErrorInfo.Error.IsNullOrEmpty()) {
								errorMessage += "Auxiliaries\n";
								errorMessage += auxiliariesErrorInfo.Error.Replace(",", "\n");
							}


							//_dialogHelper.Value.ShowMessageBox("Vehicle\n" + string.Join("\n", vehicleViewModel.Errors.Values) 
							//												+ (vehicleViewModel.MultistageAuxiliariesViewModel.HasErrors ? ("\nAuxiliaries\n" + string.Join("\n", vehicleViewModel.MultistageAuxiliariesViewModel.Errors.Values)) : ""),
								//"Error");

							_dialogHelper.Value.ShowMessageBox(errorMessage, "Error", MessageBoxButton.OK,
									MessageBoxImage.Error);
							return;
						}
					} else {
						throw new NotImplementedException();
					}


					var outputFile = _multistageDependencies.DialogHelperLazy.Value.SaveToXMLDialog(Settings.Default.DefaultFilePath);
					if (outputFile == null) {
						return;
					}
					SaveVif(outputFile:outputFile);
				}, () => true);
			}
		}

		public void SaveVif(string outputFile)
		{
			SaveVif(vifData:this, outputFile:outputFile, dialogHelper:_dialogHelper.Value);
		}

		public static void SaveVif(IMultistageVIFInputData vifData, FileOutputVIFWriter writer, IDialogHelper dialogHelper = null)
		{
			SaveVif(vifData, null, writer, dialogHelper);
		}

		public static void SaveVif(IMultistageVIFInputData vifData, string outputFile,
			FileOutputVIFWriter writer = null, IDialogHelper dialogHelper = null)
		{
			try {


				if (writer == null) {
					var numberOfManufacturingStages =
						vifData.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? 0;
					writer = new FileOutputVIFWriter(outputFile, numberOfManufacturingStages);
				}

				var inputData =
					new XMLDeclarationVIFInputData(vifData.MultistageJobInputData, vifData.VehicleInputData);

				var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer);

				var jobContainer = new JobContainer(new MockSumWriter()); //TODO: Replace with real sumwriter

				var runs = factory.SimulationRuns().ToList();
				foreach (var run in runs) {
					jobContainer.AddRun(run);
				}

				jobContainer.Execute();
				jobContainer.WaitFinished();

				using (var reader = XmlReader.Create(writer.XMLMultistageReportFileName)) {
					var validator = new XMLValidator(reader);
					var valid = validator.ValidateXML(XmlDocumentType.MultistageOutputData);
					if (!valid) {
						dialogHelper?.ShowMessageBox($"Error writing file {validator.ValidationError}", "Error",
							MessageBoxButton.OK, MessageBoxImage.Error);
						Debug.WriteLine("Invalid Outputfile");
						return;
					} else {
						dialogHelper?.ShowMessageBox($"Written to {writer.XMLMultistageReportFileName}", "Info",
							MessageBoxButton.OK, MessageBoxImage.Information);
						Debug.WriteLine($"Written to {writer.XMLMultistageReportFileName}");
					}
				}

			}catch (Exception e) {
				dialogHelper?.ShowMessageBox($"{e.Message}", "Error writing VIF", MessageBoxButton.OK,
					MessageBoxImage.Error);

			}
			
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
					var errorMessage = "Vehicle\n";
					var vehicleErrorInfo = vehicleViewModel as IDataErrorInfo;
					errorMessage += vehicleErrorInfo.Error.Replace(",", "\n");

					var auxiliariesErrorInfo =
						vehicleViewModel.MultistageAuxiliariesViewModel as IDataErrorInfo;
					if (!auxiliariesErrorInfo.Error.IsNullOrEmpty())
					{
						errorMessage += "\n Auxiliaries \n";
						errorMessage += auxiliariesErrorInfo.Error.Replace(",", "\n");
					}

					_dialogHelper.Value.ShowMessageBox(errorMessage, "Error", MessageBoxButton.OK,
						MessageBoxImage.Error);
					return;

					//_dialogHelper.Value.ShowMessageBox("Vehicle\n" + string.Join("\n", vehicleViewModel.Errors.Values)
					//												+ (vehicleViewModel.MultistageAuxiliariesViewModel.HasErrors ? ("\nAuxiliaries\n" + string.Join("\n", vehicleViewModel.MultistageAuxiliariesViewModel.Errors.Values)) : ""),
					//	"Error");
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

			
			var valid = false;
			var validationError = "";
			try {
				var validator = new XMLValidator(xDoc.ToXmlDocument());
				valid = validator.ValidateXML(XmlDocumentType.DeclarationJobData);
				validationError = validator.ValidationError;
			} catch (Exception e) {
				_dialogHelper.Value.ShowMessageBox(messageBoxText:(e.Message + "\n" + e.InnerException), caption:"Error saving File");
			}
			if (!valid) {
				_dialogHelper.Value.ShowMessageBox($"Invalid Document: {validationError}", "Error");
				var tempFile = Path.GetTempFileName();
				try {
					xDoc.Save(tempFile, SaveOptions.OmitDuplicateNamespaces);
					LoadVehicleData(tempFile);
					File.Delete(tempFile);
				} catch (Exception e) {
					_dialogHelper.Value.ShowMessageBox(e.Message, "Error");
					throw;
				}
		

			} else {
				xDoc.Save(filename, SaveOptions.OmitDuplicateNamespaces);
				LoadVehicleData(filename);
			}
		}


		private ICommand _loadVehicleDataCommand;
		private readonly Lazy<IDialogHelper> _dialogHelper;
		private readonly IXMLInputDataReader _inputDataReader;
		private string _vehicleInputDataFilePath = null;
		private readonly IMultistageDependencies _multistageDependencies;
		private readonly DataSource _dataSource;
		private readonly IMultistageBusInputDataProvider _inputData;
		private bool _selected;

		public ICommand LoadVehicleDataCommand
		{
			get
			{
				return _loadVehicleDataCommand ?? new RelayCommand(LoadVehicleDataExecute, () => true);
			}
		}

		private void LoadVehicleDataExecute()
		{
			var fileName = _dialogHelper.Value.OpenXMLFileDialog();
			if (fileName == null) {
				return;
			}

			LoadVehicleData(fileName);
			return;
		}

		private bool LoadVehicleData(string fileName)
		{
			try {
				var inputData = (IDeclarationInputDataProvider)_inputDataReader.Create(fileName);
				var vehicleInputData = inputData.JobInputData.Vehicle;
				_manufacturingStageViewModel.SetInputData(vehicleInputData);

				VehicleInputDataFilePath = fileName;
			} catch (Exception e) {
				_dialogHelper.Value.ShowMessageBox(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			return true;
		}

		public string VehicleInputDataFilePath
		{
			get => _vehicleInputDataFilePath;
			set => SetProperty(ref _vehicleInputDataFilePath, value);
		}
		#endregion





		#region Implementation of IInputDataProvider

		public string DocumentName => Path.GetFileNameWithoutExtension(_inputData.DataSource.SourceFile);

		public XmlDocumentType DocumentType => XmlDocumentType.MultistageOutputData;

		public DataSource DataSource => _dataSource;

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeEdited
		{
			get => true;
			set => throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMultistageVIFInputData

		public IVehicleDeclarationInputData VehicleInputData => _manufacturingStageViewModel.Vehicle;

		public IMultistageBusInputDataProvider MultistageJobInputData => this;


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

		#region Implementation of IEditViewModel

		public string Name => "Multistage";

		#endregion
	}


}