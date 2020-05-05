using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Views;


namespace VECTO3GUI.ViewModel.Impl
{
	public class JoblistViewModel : ObservableObject, IJoblistViewModel
	{
		#region Members

		protected ObservableCollectionEx<JobEntry> _jobs;
		protected readonly ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		private readonly SettingsModel _settings;
		private string _firstContextMenu;

		private JobEntry _selectedJobEntry;
		private JobListModel _jobListModel;

		private ICommand _newJobCommand;
		private ICommand _editJobCommand;
		private ICommand _removeJobCommand;
		private ICommand _removeAllJobCommand;
		private ICommand _addJobCommand;
		private ICommand _openJobCommand;
		private ICommand _openSettingsCommand;
		private ICommand _exitMainCommand;
		private ICommand _addBusJobCommand;
		private ICommand _editCompletedFileCommand;
		private ICommand _moveJobUpCommand;
		private ICommand _moveJobDownCommand;
		private ICommand _startSimulationCommand;
		private ICommand _openInFolderCommand;

		#endregion

		#region Properties

		public JobEntry SelectedJobEntry
		{
			get { return _selectedJobEntry; }
			set
			{
				SetProperty(ref _selectedJobEntry, value);
				if (_selectedJobEntry != null)
				{
					var firstJobTyp = _selectedJobEntry.Header.JobType == JobType.SingleBusJob
						? JobFileType.PrimaryBusFile
						: JobFileType.PIFBusFile;
					FirstContextMenu = $"View {firstJobTyp.GetLable()}";
				}
			}
		}

		public ObservableCollectionEx<JobEntry> Jobs
		{
			get { return _jobs; }
			set { SetProperty(ref _jobs, value); }
		}



		public ObservableCollection<MessageEntry> Messages
		{
			get { return _messages; }
		}

		public string FirstContextMenu
		{
			get { return _firstContextMenu; }
			set { SetProperty(ref _firstContextMenu, value); }
		}
		#endregion


		public JoblistViewModel()
		{
			_settings = new SettingsModel();
			SetJobEntries();
		}

		private void SetJobEntries()
		{
			_jobListModel = new JobListModel();
			_jobs = new ObservableCollectionEx<JobEntry>(_jobListModel.GetJobEntries());
			_jobs.CollectionChanged += JobsCollectionChanged;
			_jobs.CollectionItemChanged += JobItemChanged;
		}

		private void JobItemChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Selected" && sender is JobEntry)
				UpdateJobListEntry((JobEntry)sender);
		}

		private void JobsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Reset:
					_jobListModel.SaveJobList(_jobs);
					break;
			}
		}

		#region Commands

		public ICommand RemoveJob
		{
			get
			{
				return _removeJobCommand ?? (_removeJobCommand = new RelayCommand<JobEntry>(DoRemoveJob, CanRemoveJob));
			}
		}
		private void DoRemoveJob(JobEntry jobEntry)
		{
			_jobs.Remove(jobEntry);
		}
		private bool CanRemoveJob(JobEntry jobEntry)
		{
			return jobEntry != null;
		}

		
		public ICommand RemoveAllJobs
		{
			get
			{
				return _removeAllJobCommand ?? (_removeAllJobCommand = new RelayCommand(DoRemoveAllJobs));
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
				return _editJobCommand ?? (_editJobCommand = new RelayCommand<JobEntry>(DoEditJob, CanEditJob));
			}
		}
		private bool CanEditJob(JobEntry jobEntry)
		{
			return jobEntry != null;
		}
		private void DoEditJob(JobEntry jobEntry)
		{
			var viewModel = GetBusJobViewModel(jobEntry.Header.JobType, jobEntry);
			var window = CreateBusJobOutputWindow(viewModel, jobEntry.Header.JobType);
			if (window.ShowDialog() != true)
				ResetBusJobEntries(jobEntry);
			else
				UpdateJobEntry(((IBusJobViewModel)viewModel).SavedJobEntry);
		}

		public ICommand EditCompletedFile
		{
			get
			{
				return _editCompletedFileCommand ??
						(_editCompletedFileCommand =
							new RelayCommand<JobEntry>(DoEditCompletedFile, CanEditCompletdFile));
			}
		}
		private bool CanEditCompletdFile(JobEntry jobEntry)
		{
			return jobEntry != null;
		}
		private void DoEditCompletedFile(JobEntry jobEntry)
		{
			var viewModel = ReadCompletedXmlFile(jobEntry);
			if (viewModel == null)
				return;

			var window = OutputWindowHelper.CreateOutputWindow(Kernel, viewModel);
			window.Show();
		}



		public ICommand CreateNewJob
		{
			get
			{
				return _newJobCommand ?? (_newJobCommand = new RelayCommand(DoNewJobCommand));
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
				return _openJobCommand ?? (_openJobCommand = new RelayCommand<JobFileType>(DoOpenJobCommand));
			}
		}
		private void DoOpenJobCommand(JobFileType jobFileType)
		{
			if (SelectedJobEntry == null)
				return;

			XMLViewModel xmlViewModel = null;

			switch (jobFileType) {
				case JobFileType.PIFBusFile:
				case JobFileType.PrimaryBusFile:
					xmlViewModel = new XMLViewModel(SelectedJobEntry.Body.PrimaryVehicle);
					break;
				case JobFileType.CompletedBusFile:
					xmlViewModel = new XMLViewModel(SelectedJobEntry.Body.CompletedVehicle);
					break;
			}
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
			var filePath = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.JobFilter);
			if (filePath.IsNullOrEmpty() || !IsNewJobFile(filePath.First()))
				return;

			var jobEntry = SerializeHelper.DeserializeToObject<JobEntry>(filePath.First());
			jobEntry.JobEntryFilePath = filePath.First();
			_jobs.Add(jobEntry);
		}


		public ICommand OpenSettings
		{
			get
			{
				return _openSettingsCommand ?? (_openSettingsCommand = new RelayCommand(DoOpenSettingsCommand));
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
				return _exitMainCommand ?? (_exitMainCommand = new RelayCommand<Window>(DoCloseMainCommand));
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
				return _addBusJobCommand ?? (_addBusJobCommand = new RelayCommand<JobType>(DoAddBusJobCommand));
			}
		}
		private void DoAddBusJobCommand(JobType jobType)
		{
			var viewModel = GetBusJobViewModel(jobType);
			var window = CreateBusJobOutputWindow(viewModel, jobType);
			if (window.ShowDialog() == true)
				AddBusJobEntry(((IBusJobViewModel)viewModel)?.SavedJobEntry);
		}


		public ICommand MoveJobUp
		{
			get { return _moveJobUpCommand ?? (_moveJobUpCommand = new RelayCommand<JobEntry>(DoMoveJobUpCommand)); }
		}
		private void DoMoveJobUpCommand(JobEntry jobEntry)
		{
			if (jobEntry == null)
				return;
			var index = _jobs.IndexOf(jobEntry);
			if (index - 1 >= 0)
				_jobs.Move(index, index - 1);
		}
		
		public ICommand MoveJobDown
		{
			get { return _moveJobDownCommand ?? (_moveJobDownCommand = new RelayCommand<JobEntry>(DoMoveJobDownCommand)); }
		}
		private void DoMoveJobDownCommand(JobEntry jobEntry)
		{
			if (jobEntry == null)
				return;
			var index = _jobs.IndexOf(jobEntry);
			if (index + 1 < _jobs.Count)
				_jobs.Move(index, index + 1);
		}
		

		public ICommand StartSimulation
		{
			get { return _startSimulationCommand ?? (_startSimulationCommand = new RelayCommand(DoStartSimulationCommand)); }
		}
		private void DoStartSimulationCommand()
		{

		}


		public ICommand OpenInFolder
		{
			get { return _openInFolderCommand ?? (_openInFolderCommand = new RelayCommand<JobEntry>(DoOpenInFolderCommand)); }
		}
		private void DoOpenInFolderCommand(JobEntry jobEntry)
		{
			if (jobEntry != null)
			{
				var dirPath = Path.GetDirectoryName(jobEntry.JobEntryFilePath);
				if (Directory.Exists(dirPath))
				{
					Process.Start("explorer.exe", dirPath);
				}
			}
		}
		

		#endregion

		private object GetBusJobViewModel(JobType jobType, JobEntry jobEntry = null)
		{
			var currentJobType = jobEntry?.Header.JobType ?? jobType;

			object viewModel = null;

			switch (currentJobType)
			{
				case JobType.SingleBusJob:
					viewModel = jobEntry == null
						? new SingleBusJobViewModel(Kernel, jobType)
						: new SingleBusJobViewModel(Kernel, jobEntry);
					break;
				case JobType.CompletedBusJob:
					viewModel = jobEntry == null
						? new CompletedBusJobViewModel(Kernel, jobType)
						: new CompletedBusJobViewModel(Kernel, jobEntry);
					break;
			}

			return viewModel;
		}

		private OutputWindow CreateBusJobOutputWindow(object viewModel, JobType jobType)
		{
			return OutputWindowHelper.CreateOutputWindow(Kernel, viewModel, $"Create {jobType.GetLabel()}",
				460, 200, ResizeMode.NoResize);
		}
		
		private void AddBusJobEntry(JobEntry jobEntry)
		{
			if (jobEntry == null)
				return;
			_jobs.Add(jobEntry);
		}

		private void ResetBusJobEntries(JobEntry jobEntry)
		{
			SerializeHelper.DeserializeToObject<JobEntry>(jobEntry.JobEntryFilePath);
		}

		private void UpdateJobEntry(JobEntry jobEntry)
		{
			SerializeHelper.SerializeToFile(jobEntry.JobEntryFilePath, jobEntry);
		}

		private void UpdateJobListEntry(JobEntry jobEntry)
		{
			for (int i = 0; i < Jobs.Count; i++)
			{
				if (Jobs[i].JobEntryFilePath == jobEntry.JobEntryFilePath)
				{
					Jobs[i].Selected = jobEntry.Selected;
				}
			}
			_jobListModel.SaveJobList(_jobs);
		}
		
		private IJobEditViewModel ReadCompletedXmlFile(JobEntry jobEntry)
		{
			var xmlInputReader = Kernel.Get<IXMLInputDataReader>();
			using (var reader = XmlReader.Create(jobEntry.Body.CompletedVehicle))
			{
				var readerResult = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
				return CreateCompleteBusVehicleViewModel(readerResult);
			}
		}
		
		private IJobEditViewModel CreateCompleteBusVehicleViewModel(IDeclarationInputDataProvider dataProvider)
		{
			_messages.Add(new MessageEntry
			{
				Message = "Edit File"
			});
			return dataProvider == null ? null : new CompleteVehicleBusJobViewModel(Kernel, dataProvider);
		}

		private bool IsNewJobFile(string filePath)
		{
			if (!_jobs.IsNullOrEmpty()) {
				for (int i = 0; i < _jobs.Count; i++) {
					if (_jobs[i].JobEntryFilePath == filePath)
						return false;
				}
			}

			return true;
		}
	}
}
