using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Ninject;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;
using System.Xml;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Views;


namespace VECTO3GUI.ViewModel.Impl
{
	public class JoblistViewModel : ObservableObject, IJoblistViewModel
	{
		#region Members

		protected readonly ObservableCollection<JobEntry> _jobs = new ObservableCollection<JobEntry>();
		protected readonly ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		private readonly SettingsModel _settings;

		private JobEntry _selectedJobEntry;

		#endregion


		#region Commands

		private ICommand _newJobCommand;
		private ICommand _editJobCommand;
		private ICommand _removeJobCommand;
		private ICommand _removeAllJobCommand;
		private ICommand _addJobCommand;
		private ICommand _openJobCommand;
		private ICommand _openSettingsCommand;
		private ICommand _exitCommand;
		private ICommand _exitMainCommand;
		private ICommand _addBusJobCommand;

		#endregion

		#region Properties

		public JobEntry SelectedJobEntry
		{
			get { return _selectedJobEntry; }
			set { SetProperty(ref _selectedJobEntry, value); }
		}

		public ObservableCollection<JobEntry> Jobs
		{
			get { return _jobs; }
		}

		public ObservableCollection<MessageEntry> Messages
		{
			get { return _messages; }
		}

		#endregion

		
		public JoblistViewModel()
		{
			_settings = new SettingsModel();
			SetJobEntries();
		}

		private void SetJobEntries()
		{
			var xmlFiles = Directory.GetFiles(_settings.XmlFilePathFolder, "*.xml");
			for (int i = 0; i < xmlFiles.Length; i++) {
				AddJobEntry(xmlFiles[i]);
			}
		}

		private void AddJobEntry(string jobFile)
		{
			_jobs.Add(new JobEntry()
			{
				Filename = jobFile,
				Selected = false,
				Sorting = _jobs.Count
			});
		}


		#region Implementation IJoblistViewModel

		public ICommand RemoveJob
		{
			get
			{
				return _removeJobCommand ??
						(_removeJobCommand = new RelayCommand(DoRemoveJob, CanRemoveJob));
			}
		}
		private void DoRemoveJob()
		{
			_jobs.Remove(SelectedJobEntry);
			SelectedJobEntry = null;
		}
		private bool CanRemoveJob()
		{
			return SelectedJobEntry != null;
		}


		public ICommand RemoveAllJobs
		{
			get
			{
				return _removeAllJobCommand ??
						(_removeAllJobCommand = new RelayCommand(DoRemoveAllJobs));
			}
		}
		private void DoRemoveAllJobs()
		{
			_jobs.Clear();
			SelectedJobEntry = null;
		}
		

		public ICommand EditJob
		{
			get
			{
				return _editJobCommand ??
						(_editJobCommand = new RelayCommand(DoEditJob, CanEditJob));
			}
		}
		private void DoEditJob()
		{
			var entry = SelectedJobEntry;
			try
			{
				var jobEditView = ReadJob(entry.Filename); //Kernel.Get<IJobEditViewModel>();
				if (jobEditView == null)
					return;

				var window = OutputWindowHelper.CreateOutputWindow(Kernel, jobEditView);
				window.Show();
			}
			catch (Exception e)
			{
				MessageBox.Show(
					"Failed to read selected job: " + Environment.NewLine + Environment.NewLine + e.Message, "Failed reading Job",
					MessageBoxButton.OK);
			}
		}
		private bool CanEditJob()
		{
			return SelectedJobEntry != null;
		}


		public ICommand CreateNewJob
		{
			get
			{
				return _newJobCommand ??
						(_newJobCommand = new RelayCommand(DoNewJobCommand));
			}
		}
		private void DoNewJobCommand()
		{
			var jobEditView = new CompleteVehicleBusJobViewModel(Kernel, null);
			var window = OutputWindowHelper.CreateOutputWindow(Kernel, jobEditView, "New File");
			window.Show();
		}


		public ICommand OpenJob
		{
			get
			{
				return _openJobCommand ??
						(_openJobCommand = new RelayCommand(DoOpenJobCommand));
			}
		}
		private void DoOpenJobCommand()
		{
			if (SelectedJobEntry == null)
				return;

			var xmlViewModel = new XMLViewModel(SelectedJobEntry.Filename);
			var window = OutputWindowHelper.CreateOutputWindow(Kernel, xmlViewModel, xmlViewModel.FileName);
			window.Show();
		}


		public ICommand AddJob
		{
			get
			{
				return _addJobCommand ?? (_addJobCommand = new RelayCommand(DoAddJob));
			}
		}
		private void DoAddJob()
		{
			var filePath = FileDialogHelper.ShowSelectFilesDialog(false, _settings.XmlFilePathFolder);
			if (filePath != null)
			{
				_jobs.Add(new JobEntry()
				{
					Filename = filePath.First(),
					Selected = false,
					Sorting = _jobs.Count
				});
			}
		}


		public ICommand OpenSettings
		{
			get
			{
				return _openSettingsCommand ??
					  (_openSettingsCommand = new RelayCommand(DoOpenSettingsCommand));
			}
		}
		private void DoOpenSettingsCommand()
		{
			var viewModel = new SettingsViewModel();
			var window = OutputWindowHelper.CreateOutputWindow(Kernel, viewModel, "Settings", 440, 200,
				ResizeMode.NoResize);
			window.ShowDialog();
		}

		public ICommand ExitMainCommand
		{
			get
			{
				return _exitMainCommand ??
						(_exitMainCommand = new RelayCommand<Window>(DoCloseMainCommand));
			}
		}
		private void DoCloseMainCommand(Window window)
		{
			window?.Close();
		}

		public ICommand AddBusJob
		{
			get
			{
				return _addBusJobCommand ??
						(_addBusJobCommand = new RelayCommand(DoAddBusJobCommand));
			}
		}

		private void DoAddBusJobCommand()
		{
			var viewModel = new BusJobViewModel();
			var window = OutputWindowHelper.CreateOutputWindow(Kernel, viewModel, "Create Bus Job");
			window.ShowDialog();
		}


		public ICommand MoveJobUp { get { return new RelayCommand(() => { }, () => false); } }
		public ICommand MoveJobDown { get { return new RelayCommand(() => { }, () => false); } }
		public ICommand StartSimulation { get { return new RelayCommand(() => { }, () => false); } }
		public ICommand JobEntrySetActive { get { return new RelayCommand(() => { }, () => false); } }


		#endregion


		private IJobEditViewModel ReadJob(string jobFile)
		{
			if (jobFile == null)
				return null;

			var ext = Path.GetExtension(jobFile);
			if (ext == Constants.FileExtensions.VectoXMLDeclarationFile)
			{

				var localName = GetLocalName(jobFile);
				var xmlInputReader = Kernel.Get<IXMLInputDataReader>();

				using (var reader = XmlReader.Create(jobFile))
				{
					if (localName == XMLNames.VectoPrimaryVehicleReport)
						return CreatePrimaryBusVehicleViewModel(xmlInputReader.Create(reader));
					
					if (localName == XMLNames.VectoInputDeclaration)
					{
						var readerResult = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
						if(readerResult?.JobInputData.Vehicle is XMLDeclarationCompletedBusDataProviderV26)
							return CreateCompleteBusVehicleViewModel(readerResult);
					}
				}
			}

			return null;
		}

		private string GetLocalName(string jobFilePath)
		{
			var doc = XDocument.Load(jobFilePath);
			return doc.Root?.Name.LocalName;
		}


		private IJobEditViewModel CreateCompleteBusVehicleViewModel(IDeclarationInputDataProvider dataProvider)
		{
			
			
			_messages.Add(new MessageEntry {
				Message = "Edit File"
			});
			return dataProvider == null ? null : new CompleteVehicleBusJobViewModel(Kernel, dataProvider);
		}


		private IJobEditViewModel CreatePrimaryBusVehicleViewModel(IInputDataProvider inputData)
		{
			var dataProvider = inputData as IPrimaryVehicleInformationInputDataProvider;
			return dataProvider == null ? null : new PrimaryVehicleBusJobViewModel(Kernel, dataProvider);
		}
	}



	#region Legacy

	//IInputDataProvider inputData = null;
	//var ext = Path.GetExtension(jobFile);
	//switch (ext) {
	//	case Constants.FileExtensions.VectoJobFile:
	//		inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
	//		break;
	//	case Constants.FileExtensions.VectoXMLDeclarationFile:
	//	//ToDo
	//	//case Constants.FileExtensions.VectoXMLJobFile:
	//		inputData = Kernel.Get<IXMLInputDataReader>().CreateDeclaration(jobFile);
	//		break;
	//	default:
	//		throw new UnsupportedFileVersionException(jobFile);
	//}

	//var retVal = CreateJobEditViewModel(inputData);

	//if (retVal == null) {
	//	throw new Exception("Unsupported job type");
	//}
	//return retVal;



	//private IJobEditViewModel CreateJobEditViewModel(IInputDataProvider inputData)
	//{
	//	IJobEditViewModel retVal = null;
	//	if (inputData is JSONInputDataV2) {
	//		var jsoninputData = inputData as JSONInputDataV2;
	//		if (jsoninputData.SavedInDeclarationMode) {
	//			retVal = new DeclarationJobViewModel(Kernel, jsoninputData);
	//		} else {
	//			if (jsoninputData.EngineOnlyMode) {
	//				retVal = new EngineOnlyJobViewModel(Kernel, jsoninputData);
	//			} else {
	//				// TODO!
	//			}
	//		}
	//	}
	//	//ToDo
	//	//if (inputData is XMLDeclarationInputDataProvider) {
	//	//	var declInput = inputData as IDeclarationInputDataProvider;
	//	//	retVal = new DeclarationJobViewModel(Kernel, declInput);
	//	//}
	//	return retVal;
	//}


	#endregion


}
