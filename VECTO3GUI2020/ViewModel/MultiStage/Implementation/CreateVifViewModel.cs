using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using InteractiveDataDisplay.WPF;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using Newtonsoft.Json;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Model.Multistage;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public interface ICreateVifViewModel: IDocumentViewModel, IEditViewModel
	{
		bool LoadStageInput(string fileName);
		bool LoadPrimaryInput(string fileName);
		bool? ExemptedPrimary { get; set; }
		bool? StageInputExempted { get; set; }
		string PrimaryInputPath { get; set; }
		string StageInputPath { get; set; }
		void SaveJob(string path);
	}
    public class CreateVifViewModel : ViewModelBase, ICreateVifViewModel
	{
		private string _primaryInputPath;
		private string _stageInputPath;
		private readonly IDialogHelper _dialogHelper;
		private readonly IXMLInputDataReader _inputDataReader;
		private static uint _newVifCounter = 0;

		private bool? _exemptedPrimary;

		public bool? ExemptedPrimary
		{
			get => _exemptedPrimary;
			set => SetProperty(ref _exemptedPrimary, value);
		}

		private bool? _stageInputExempted;

		public bool? StageInputExempted
		{
			get => _stageInputExempted;
			set => SetProperty(ref _stageInputExempted, value);
		}

		public CreateVifViewModel(IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IAdditionalJobInfoViewModel additionalJobInfo)
		{
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
			Title = "Create VIF";
			SizeToContent = SizeToContent.WidthAndHeight;
			_documentName = $"New Vif {++_newVifCounter}";
		}

		public CreateVifViewModel(IDeclarationInputDataProvider inputData, 
			IDialogHelper dialogHelper, 
			IXMLInputDataReader inputDataReader, 
			IAdditionalJobInfoViewModel additionalJobInfo) : this(dialogHelper, inputDataReader, additionalJobInfo)
		{
			SetInputData(inputData);
		}


		private void SetInputData(IInputDataProvider inputData)
		{
			var inputDataProvider = inputData as JSONInputDataV10_PrimaryAndInterimBus;
			Debug.Assert(inputDataProvider != null);
			DataSource = inputData.DataSource;
			Title += $"- {Path.GetFileName(_dataSource.SourceFile)}";
			DocumentName = Path.GetFileNameWithoutExtension(_dataSource.SourceFile);

		}


		public string PrimaryInputPath
		{
			get => _primaryInputPath;
			set
			{
				if (SetProperty(ref _primaryInputPath, value)) {
					OnPropertyChanged(nameof(CanBeSimulated));
				}
			}
		}

		public string StageInputPath
		{
			get => _stageInputPath;
			set
			{
				if (SetProperty(ref _stageInputPath, value))
				{
					OnPropertyChanged(nameof(CanBeSimulated));
				}
			}
		}

		#region Commands

		private ICommand _selectPrimaryInputFileCommand;
		private ICommand _selectCompletedInputFileCommand;
		public ICommand SelectCompletedInputFileCommand
		{
			get => _selectCompletedInputFileCommand ?? (_selectCompletedInputFileCommand = new RelayCommand(() => {
				var selectedFile = _dialogHelper.OpenXMLFileDialog();
				LoadStageInput(selectedFile);


			}));
		}

		public ICommand SelectPrimaryInputFileCommand
		{
			get => _selectPrimaryInputFileCommand ?? (_selectPrimaryInputFileCommand = new RelayCommand(() => {
				var selectedFilePath = _dialogHelper.OpenXMLFileDialog();
				LoadPrimaryInput(selectedFilePath);

			}));
		}

		private IRelayCommand _saveJobCommand;

		public ICommand SaveJobCommand
		{
			get => _saveJobCommand ?? (_saveJobCommand = new RelayCommand(() => {
				if (_dataSource.SourceFile != null) {
					if (CanBeSaved()) {
						SaveJob(_dataSource.SourceFile);
					}
				}
			}, () => DataSource != null));
		}

		private bool CanBeSaved()
		{
			if (_primaryInputPath == null) {
				_dialogHelper.ShowMessageBox("At least Primary Vehicle has to be provided", "Info", MessageBoxButton.OK,
					MessageBoxImage.Information);
				return false;
			}

			return true;
		}

		public void SaveJob(string path)
		{
			if (path == null) {
				return;
			}

			var jsonJob = new JSONJob() {
				Header = new JSONJobHeader() {
					AppVersion = "Vecto3GUI2020",
					CreatedBy = Environment.UserName,
					Date = DateTime.Today,
					FileVersion = JSONJobHeader.PrimaryAndInterimVersion
				},
				Body = new JSONJobBody() {
					PrimaryVehicle = PrimaryInputPath,
					InterimStage = StageInputPath
				}
			};

			string jsonString = JsonConvert.SerializeObject(jsonJob, Formatting.Indented);

			
			Debug.WriteLine(jsonString);
			File.WriteAllText(path, jsonString);
			SetInputData(JSONInputDataFactory.ReadJsonJob(path));
		}

		private ICommand _saveJobAsCommand;

		public ICommand SaveJobAsCommand
		{
			get => _saveJobAsCommand ?? (_saveJobAsCommand = new RelayCommand(() => {
				if (CanBeSaved()) {
					var path = _dialogHelper.SaveToJsonDialog();
					SaveJob(path);
				}
			}));
		}

		public bool LoadStageInput(string fileName)
		{
			if (fileName == null)
			{
				return false;
			}

			var valid = true;
			IVehicleDeclarationInputData vehicleInputData = null;
			try
			{
				var inputData = _inputDataReader.Create(fileName) as IDeclarationInputDataProvider;
				vehicleInputData = inputData.JobInputData.Vehicle;
				var type = vehicleInputData.GetType();
				valid = (inputData != null) && (vehicleInputData is XMLDeclarationInterimStageBusDataProviderV28) || (vehicleInputData is XMLDeclarationExemptedInterimStageBusDataProviderV28);
			}
			catch (Exception e)
			{
				valid = false;
			}

			valid = valid && SetStageInputExempted(vehicleInputData.ExemptedVehicle);

			if (valid)
			{
				StageInputPath = fileName;
			}
			else
			{
				_dialogHelper.ShowMessageBox("Invalid File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}




			return valid;
		}


		public bool LoadPrimaryInput(string fileName)
		{
			if (fileName == null)
			{
				return false;
			}

			var valid = true;
			IDeclarationInputDataProvider inputData = null;
			try
			{
				inputData = _inputDataReader.Create(fileName) as IDeclarationInputDataProvider;
				valid = inputData != null && inputData.JobInputData.Vehicle.VehicleCategory.IsBus();
			}
			catch (Exception ex)
			{
				valid = false;
			}

			valid = valid && SetPrimaryInputExempted(inputData.JobInputData.Vehicle.ExemptedVehicle);



			if (valid)
			{
				PrimaryInputPath = fileName;
			}
			else
			{
				_dialogHelper.ShowMessageBox("Invalid File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}


			return valid;
		}

		private bool SetPrimaryInputExempted(bool primaryExempted)
		{
			var valid = StageInputExempted == null || StageInputExempted == primaryExempted;

			if (valid)
			{
				ExemptedPrimary = primaryExempted;
			}
			else
			{
				_dialogHelper.ShowMessageBox(
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error,
					messageBoxText: (primaryExempted
						? "Exempted primary vehicle not allowed for non-exempted interim/completed input"
						: "Only exempted input allowed for exempted interim/completed input"));
			}
			return valid;
		}

		private bool SetStageInputExempted(bool stageInputExempted)
		{
			var valid = ExemptedPrimary == null || ExemptedPrimary == stageInputExempted;

			if (valid)
			{
				StageInputExempted = stageInputExempted;
			}
			else
			{
				_dialogHelper.ShowMessageBox(
					caption: "Error",
					button: MessageBoxButton.OK,
					icon: MessageBoxImage.Error,
					messageBoxText: (stageInputExempted
						? "Exempted interim/complete input is invalid for non-exempted primary vehicle"
						: "Only exempted input allowed for selected primary vehicle"));
			}
			return valid;
		}







		#endregion

		#region Implementation of IDocumentViewModel
		private bool _selected;
		private string _documentName;
		private DataSource _dataSource;

		public string DocumentName
		{
			get => _documentName;
			set => SetProperty(ref _documentName, value);
		}


		public XmlDocumentType DocumentType => throw new NotImplementedException();

		public DataSource DataSource
		{
			get => _dataSource;
			set
			{
				if (SetProperty(ref _dataSource, value)) {
					_saveJobCommand.NotifyCanExecuteChanged();
				}
			}
		}

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated
		{
			get => PrimaryInputPath != null && StageInputPath != null;
			set => throw new NotImplementedException();
		}

		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IEditViewModel

		public string Name => DocumentName;



		#endregion
	}
}
