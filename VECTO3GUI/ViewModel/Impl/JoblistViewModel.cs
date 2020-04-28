using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ninject;
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

		private JobEntry _selectedJobEntry;
		private JobListModel _jobListModel;

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

		public ObservableCollectionEx<JobEntry> Jobs
		{
			get { return _jobs; }
			set { SetProperty(ref _jobs, value); }
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
			_jobListModel = new JobListModel();
			_jobs = new ObservableCollectionEx<JobEntry>(_jobListModel.GetJobEntries());
			_jobs.CollectionChanged += JobsCollectionChanged;
			_jobs.CollectionItemChanged += JobItemChanged;
		}

		private void JobItemChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "Selected" && sender is JobEntry )
				UpdateJobEntry((JobEntry) sender);
		}

		private void JobsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
					_jobListModel.SaveJobList(_jobs);
					break;
			}
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
						(_editJobCommand = new RelayCommand<JobEntry>(DoEditJob, CanEditJob));
			}
		}

		private bool CanEditJob(JobEntry jobEntry)
		{
			return jobEntry != null;
		}

		private void DoEditJob(JobEntry jobEntry)
		{
			var viewModel = GetBusJobViewModel(jobEntry.JobType, jobEntry);
			var window = CreateBusJobOutputWindow(viewModel, jobEntry.JobType);
			if(window.ShowDialog() != true)
			  ResetBusJobEntries(jobEntry);
			else
				UpdateJobEntry(((IBusJobViewModel)viewModel).SavedJobEntry);
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

			var xmlViewModel = new XMLViewModel(SelectedJobEntry.FirstFilePath);
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
					FirstFilePath = filePath.First(),
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
						(_addBusJobCommand = new RelayCommand<JobType>(DoAddBusJobCommand));
			}
		}
		private void DoAddBusJobCommand(JobType jobType)
		{
			var viewModel = GetBusJobViewModel(jobType);
			var window = CreateBusJobOutputWindow(viewModel, jobType);
			if(window.ShowDialog() == true)
				AddBusJobEntry(((IBusJobViewModel)viewModel)?.SavedJobEntry);
		}


		public ICommand MoveJobUp { get { return new RelayCommand(() => { }, () => false); } }
		public ICommand MoveJobDown { get { return new RelayCommand(() => { }, () => false); } }
		public ICommand StartSimulation { get { return new RelayCommand(() => { }, () => false); } }
		public ICommand JobEntrySetActive { get { return new RelayCommand(() => { }, () => false); } }


		#endregion




		private object GetBusJobViewModel(JobType jobType, JobEntry jobEntry = null)
		{
			var currentJobType = jobEntry?.JobType ?? jobType;

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
		//private IJobEditViewModel ReadJob(string jobFile)
		//{
		//	if (jobFile == null)
		//		return null;

		//	var ext = Path.GetExtension(jobFile);
		//	if (ext == Constants.FileExtensions.VectoXMLDeclarationFile)
		//	{

		//		var localName = GetLocalName(jobFile);
		//		var xmlInputReader = Kernel.Get<IXMLInputDataReader>();

		//		using (var reader = XmlReader.Create(jobFile))
		//		{
		//			if (localName == XMLNames.VectoPrimaryVehicleReport)
		//				return CreatePrimaryBusVehicleViewModel(xmlInputReader.Create(reader));

		//			if (localName == XMLNames.VectoInputDeclaration)
		//			{
		//				var readerResult = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
		//				if(readerResult?.JobInputData.Vehicle is XMLDeclarationCompletedBusDataProviderV26)
		//					return CreateCompleteBusVehicleViewModel(readerResult);
		//			}
		//		}
		//	}

		//	return null;
		//}

		//private string GetLocalName(string jobFilePath)
		//{
		//	var doc = XDocument.Load(jobFilePath);
		//	return doc.Root?.Name.LocalName;
		//}


		//private IJobEditViewModel CreateCompleteBusVehicleViewModel(IDeclarationInputDataProvider dataProvider)
		//{
		//	_messages.Add(new MessageEntry {
		//		Message = "Edit File"
		//	});
		//	return dataProvider == null ? null : new CompleteVehicleBusJobViewModel(Kernel, dataProvider);
		//}


		//private IJobEditViewModel CreatePrimaryBusVehicleViewModel(IInputDataProvider inputData)
		//{
		//	var dataProvider = inputData as IPrimaryVehicleInformationInputDataProvider;
		//	return dataProvider == null ? null : new PrimaryVehicleBusJobViewModel(Kernel, dataProvider);
		//}
	}





}
