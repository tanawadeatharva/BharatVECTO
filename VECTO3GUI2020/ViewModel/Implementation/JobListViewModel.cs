using Microsoft.Win32;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using VECTO3GUI2020.Views;
using IDocumentViewModel = VECTO3GUI2020.ViewModel.Interfaces.Document.IDocumentViewModel;
using RelayCommand = VECTO3GUI2020.Util.RelayCommand;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VECTO3GUI2020.ViewModel.Implementation
{
    public class JobListViewModel : ViewModelBase, IJobListViewModel
    {
        #region Members and Properties
        private readonly Settings _settings = Settings.Default;


        private ICommand _addJobCommand;
        private ICommand _editJobCommand;
        private ICommand _removeJobCommand;
        private ICommand _moveJobUpCommand;
        private ICommand _moveJobDownCommand;
        private ICommand _viewXMLCommand;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private BackgroundWorker fileReadingBackgroundWorker;

		private object _jobsLock = new Object();
        private ObservableCollection<IDocumentViewModel> _jobs = new ObservableCollection<IDocumentViewModel>();
        public ObservableCollection<IDocumentViewModel> Jobs{ get => _jobs; set => SetProperty(ref _jobs, value);}

        private IDialogHelper _dialogHelper;
        private IWindowHelper _windowHelper;
        private IDocumentViewModelFactory _documentViewModelFactory;
		private ICommand _newMultiStageFileCommand;
		private IMultiStageViewModelFactory _multiStageViewModelFactory;
		private IAsyncRelayCommand _addJobAsync;
		private readonly IXMLInputDataReader _inputDataReader;
		private IAsyncRelayCommand _simulationCommand;
		private readonly IOutputViewModel _outputViewModel;

		


        
		#endregion


        

        public JobListViewModel()
        {
			BindingOperations.EnableCollectionSynchronization(Jobs, _jobsLock);
            InitFileBackGroundWorker();
            
            
        }


        public JobListViewModel(IDocumentViewModelFactory documentViewModelFactory,
            IXMLInputDataReader inputDataReader,
            IDialogHelper dialogHelper,
            IWindowHelper windowHelper,
			IMultiStageViewModelFactory multiStageViewModelFactory, IOutputViewModel outputViewModel) : this()
        {
            _documentViewModelFactory = documentViewModelFactory;
            _dialogHelper = dialogHelper;
            _windowHelper = windowHelper;
			_inputDataReader = inputDataReader;
			_multiStageViewModelFactory = multiStageViewModelFactory;
			_outputViewModel = outputViewModel;
		}



        private void InitFileBackGroundWorker()
        {
            fileReadingBackgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            fileReadingBackgroundWorker.DoWork += fileworker_DoWork;
            fileReadingBackgroundWorker.ProgressChanged += fileworker_ProgressChanged;
            fileReadingBackgroundWorker.RunWorkerCompleted += fileworker_RunWorkerCompleted;
        }

		private void fileworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Debug.WriteLine(e.ProgressPercentage);
		}

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private bool _simulationRunning = false;

		public bool SimulationRunning
		{
			get => _simulationRunning;
			set
			{
				SetProperty(ref _simulationRunning, value);
                OnPropertyChanged(nameof(SimulationCommand));
                OnPropertyChanged(nameof(CancelSimulation));
			}
		}

		private ICommand _cancelSimulationCommand;


		private async Task RunSimulationExecute()
		{
			cancellationTokenSource = new CancellationTokenSource();
			SimulationRunning = true;
			await Task.Run(() => RunSimulationAsync(cancellationTokenSource.Token,
				new Progress<MessageEntry>((message) => { _outputViewModel.Messages.Add(message); }),
				new Progress<int>((i) => _outputViewModel.Progress = i),
				new Progress<string>((msg) => _outputViewModel.StatusMessage = msg)));
			SimulationRunning = false;
			_outputViewModel.Progress = 0;
			cancellationTokenSource.Dispose();
        }

		private async Task RunSimulationAsync(CancellationToken ct, IProgress<MessageEntry> outputMessages,
			IProgress<int> progress, IProgress<string> status)
		{
            progress.Report(0);
			status.Report("starting...");
			
			IDocumentViewModel[] jobs;
			lock (_jobsLock) {
				jobs = Jobs.Where(x => x.Selected).ToArray();
				if (jobs.Length == 0) {
                    outputMessages.Report(new MessageEntry() {
                        Message = "No Jobs Selected",
                        Time = DateTime.Now,
                        Type = MessageType.InfoMessage,
					});
					status.Report("No jobs selected");
					return;
				}
			}

			var sumFileWriter = new FileOutputWriter(GetOutputDirectory(Jobs.First(x => x.Selected).DataSource.SourceFile));
			var sumContainer = new SummaryDataContainer(sumFileWriter);
			var jobContainer = new JobContainer(sumContainer);
			var mode = ExecutionMode.Declaration;

			var fileWriters = new Dictionary<int, FileOutputWriter>();
			var finishedRuns = new List<int>();

			var xmlReader = _inputDataReader;

			foreach (var jobEntry in jobs) {
				try
				{
					var fullFileName = Path.GetFullPath(jobEntry.DataSource.SourceFile);
					if (!File.Exists(fullFileName))
					{
						outputMessages.Report(new MessageEntry()
							{
								Type = MessageType.ErrorMessage,
								Message =
									$"File {Path.GetFileName(jobEntry.DataSource.SourceFile)} not found!"
							});
						continue;
					}

					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.StatusMessage,
							Message = $"Reading file {Path.GetFileName(fullFileName)}"
						});



					var extension = Path.GetExtension(jobEntry.DataSource.SourceFile);
					IInputDataProvider input = null;
					IXMLMultistageInputDataProvider multistageInput = null;

					var FileWriter = new FileOutputWriter(fullFileName);
					switch (extension) {
						case Constants.FileExtensions.VectoJobFile:
							input = JSONInputDataFactory.ReadJsonJob(fullFileName);
							var tmp = input as IDeclarationInputDataProvider;
							mode = tmp?.JobInputData.SavedInDeclarationMode ?? false
								? ExecutionMode.Declaration
								: ExecutionMode.Engineering;
							break;
						case ".xml":
							var xdoc = XDocument.Load(fullFileName);
							var rootNode = xdoc.Root?.Name.LocalName ?? "";
							if (XMLNames.VectoInputEngineering.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								input = xmlReader.CreateEngineering(fullFileName);
								mode = ExecutionMode.Engineering;
							} else if (XMLNames.VectoInputDeclaration.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								using (var reader = XmlReader.Create(fullFileName)) {
									input = xmlReader.CreateDeclaration(reader);
								}

								mode = ExecutionMode.Declaration;
							} else if (XMLNames.VectoOutputMultistage.Equals(rootNode,
								StringComparison.InvariantCultureIgnoreCase)) {
								using (var reader = XmlReader.Create(fullFileName)) {
									input = new XMLDeclarationVIFInputData(xmlReader.Create(fullFileName) as IMultistageBusInputDataProvider, null);
									FileWriter = new FileOutputVIFWriter(fullFileName,
										(jobEntry as MultiStageJobViewModel_v0_1).ManufacturingStages?.Count ?? 0);
								}

								mode = ExecutionMode.Declaration;
							}

							break;
					}

					if (input == null && multistageInput == null)
					{
						outputMessages.Report(
							new MessageEntry()
							{
								Type = MessageType.ErrorMessage,
								Message = $"No input provider for job {Path.GetFileName(fullFileName)}"
							});
						continue;
					}

					var fileWriter = new FileOutputWriter(GetOutputDirectory(fullFileName));
					var runsFactory = new SimulatorFactory(mode, input, fileWriter)
					{
						WriteModalResults = Settings.Default.WriteModalResults,
						ModalResults1Hz = Settings.Default.ModalResults1Hz,
						Validate = Settings.Default.Validate,
						ActualModalData = Settings.Default.ActualModalData,
						SerializeVectoRunData = Settings.Default.SerializeVectoRunData,
						
					};

					var stopwatch = new Stopwatch();
					stopwatch.Start();
					foreach (var runId in jobContainer.AddRuns(runsFactory))
					{
						if (ct.IsCancellationRequested) {
							outputMessages.Report(new MessageEntry()
							{
								Message = "Simulation canceled",
								Type = MessageType.StatusMessage,
							});
							return;
						}
						fileWriters.Add(runId, fileWriter);
					}
					stopwatch.Stop();


					// TODO MQ-20200525: Remove the following loop in production (or after evaluation of LAC!!

					
					//if (!string.IsNullOrWhiteSpace(LookAheadMinSpeedOverride))
					//{
					//	foreach (var run in jobContainer.Runs)
					//	{
					//		var tmpDriver = ((VectoRun)run.Run).GetContainer().RunData.DriverData;
					//		tmpDriver.LookAheadCoasting.Enabled = true;
					//		tmpDriver.LookAheadCoasting.MinSpeed = LookAheadMinSpeedOverride.ToDouble().KMPHtoMeterPerSecond();
					//	}
					//}
					

					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.StatusMessage,
							Message = $"Finished reading data for job {Path.GetFileName(fullFileName)}"
						});
				}
				catch (Exception ex)
				{
					/*
					MessageBox.Show(
						$"ERROR running job {Path.GetFileName(jobEntry.DataSource.SourceFile)}: {ex.Message}", "Error", MessageBoxButton.OK,
						MessageBoxImage.Exclamation);
					*/
					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.ErrorMessage, 
							Message = ex.Message
						});
				}
			}
			foreach (var cycle in jobContainer.GetCycleTypes())
			{
				outputMessages.Report(new MessageEntry()
				{
					Type = MessageType.StatusMessage, Message = $"Detected cycle {cycle.Name}: {cycle.CycleType}"
				});
			}

			outputMessages.Report(new MessageEntry() {
				Type = MessageType.StatusMessage,
				Message = $"Starting simulation ({jobs.Length} jobs, {jobContainer.GetProgress().Count} runs)",
			});

			var start = Stopwatch.StartNew();
			jobContainer.Execute(true);
			while (!jobContainer.AllCompleted)
			{
				if (ct.IsCancellationRequested)
				{
					jobContainer.Cancel();
					outputMessages.Report(new MessageEntry() {
						Message = "Simulation canceled",
						Type = MessageType.StatusMessage,
					});
					return;
				}

				var jobProgress = jobContainer.GetProgress();
				var sumProgress = jobProgress.Sum(x => x.Value.Progress);
				var duration = start.Elapsed.TotalSeconds;
				jobProgress.Select(x => x.Value.Progress);

				progress.Report(Convert.ToInt32(sumProgress * 100 / jobProgress.Count));
				status.Report(string.Format(
					"Duration: {0:F1}s, Current Progress: {1:P} ({2})", duration, sumProgress / jobProgress.Count,
					string.Join(", ", jobProgress.Select(x => string.Format("{0,4:P}", x.Value.Progress)))));
                var justFinished = jobProgress.Where(x => x.Value.Done & !finishedRuns.Contains(x.Key))
					.ToDictionary(x => x.Key, x => x.Value);
				//PrintRuns(justFinished, fileWriters);
				finishedRuns.AddRange(justFinished.Select(x => x.Key));
				await Task.Delay(100, ct);
			}
			start.Stop();

			var remainingRuns = jobContainer.GetProgress().Where(x => x.Value.Done && !finishedRuns.Contains(x.Key))
				.ToDictionary(x => x.Key, x => x.Value);
			//PrintRuns(remainingRuns, fileWriters);

			finishedRuns.Clear();
			fileWriters.Clear();
			foreach (var progressEntry in jobContainer.GetProgress())
			{
				outputMessages.Report(new MessageEntry()
				{
					Type = MessageType.StatusMessage,
					Message =
						string.Format("{0,-60} {1,8:P} {2,10:F2}s - {3}",
							$"{progressEntry.Value.RunName} {progressEntry.Value.CycleName} {progressEntry.Value.RunSuffix}",
							progressEntry.Value.Progress,
							progressEntry.Value.ExecTime / 1000.0,
							progressEntry.Value.Success ? "Success" : "Aborted")
				});
				if (!progressEntry.Value.Success)
				{
					outputMessages.Report(
						new MessageEntry()
						{
							Type = MessageType.StatusMessage,
							Message = progressEntry.Value.Error.Message
						}
					);
				}
			}
			foreach (var jobEntry in jobs)
			{
				var w = new FileOutputWriter(GetOutputDirectory(jobEntry.DataSource.SourceFile));
				foreach (var entry in new Dictionary<string, string>() { { w.XMLFullReportName, "XML ManufacturereReport" }, { w.XMLCustomerReportName, "XML Customer Report" }, { w.XMLVTPReportName, "VTP Report" }, { w.XMLPrimaryVehicleReportName, "Primary Vehicle Information File" } })
				{
					if (File.Exists(entry.Key))
					{
						outputMessages.Report(
							new MessageEntry()
							{
								Type = MessageType.StatusMessage,
								Message = string.Format(
									"{2} for '{0}' written to {1}", Path.GetFileName(jobEntry.DataSource.SourceFile), entry.Key, entry.Value),
								//Link = "<XML>" + entry.Key
							});
					}
				}
			}

			if (File.Exists(sumFileWriter.SumFileName))
			{
				outputMessages.Report(new MessageEntry()
				{
					Type = MessageType.StatusMessage,
					Message = string.Format("Sum file written to {0}", sumFileWriter.SumFileName),
					//Link = "<CSV>" + sumFileWriter.SumFileName
				});
			}

			outputMessages.Report(new MessageEntry()
			{
				Type = MessageType.StatusMessage,
				Message = string.Format("Simulation finished in {0:F1}s", start.Elapsed.TotalSeconds)
			});
		}
		private void PrintRuns(Dictionary<int, JobContainer.ProgressEntry> progress, Dictionary<int, FileOutputWriter> fileWriters, IProgress<MessageEntry> outputMessages)
		{ 
			foreach (var p in progress) {
				var modFilename = fileWriters[p.Key]
					.GetModDataFileName(p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix);
				var runName = string.Format("{0} {1} {2}", p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix);

			//	if (p.Value.Error != null)
			//	{
			//		SimulationWorker.ReportProgress(0, new VectoSimulationProgress()
			//		{
			//			Type = VectoSimulationProgress.MsgType.StatusMessage,
			//			Message = string.Format("Finished Run {0} with ERROR: {1}", runName,
			//				p.Value.Error.Message),
			//			Link = "<CSV>" + modFilename
			//		});
			//	}
			//	else
			//	{
			//		SimulationWorker.ReportProgress(0, new VectoSimulationProgress()
			//		{
			//			Type = VectoSimulationProgress.MsgType.StatusMessage,
			//			Message = string.Format("Finished run {0} successfully.", runName)
			//		});
			//	}
			//	if (File.Exists(modFilename))
			//	{
			//		SimulationWorker.ReportProgress(0, new VectoSimulationProgress()
			//		{
			//			Type = VectoSimulationProgress.MsgType.StatusMessage,
			//			Message = string.Format("Run {0}: Modal results written to {1}", runName, modFilename),
			//			Link = "<CSV>" + modFilename
			//		});
			//	}
			}
		}


		private string GetOutputDirectory(string jobFilePath)
		{
			var outFile = jobFilePath;
			var outputDirectory = Settings.Default.DefaultOutputPath;
			if (string.IsNullOrWhiteSpace(outputDirectory)) {
				return outFile;
			}

			outFile = Path.IsPathRooted(outputDirectory)
				? Path.Combine(outputDirectory, Path.GetFileName(jobFilePath) ?? "")
				: Path.Combine(Path.GetDirectoryName(jobFilePath) ?? "", outputDirectory,
					Path.GetFileName(jobFilePath) ?? "");
			if (!Directory.Exists(Path.GetDirectoryName(outFile))) {
				Directory.CreateDirectory(Path.GetDirectoryName(outFile));
			}

			return outFile;
		}


		#region Commands

		public ICommand CancelSimulation
		{
			get
			{
				return _cancelSimulationCommand ?? new RelayCommand(() => {
						_outputViewModel.Messages.Add(new MessageEntry() {
							Message="Canceling Simulation",
							Type=MessageType.StatusMessage,
						});
						cancellationTokenSource.Cancel();
					},
					() => SimulationRunning);
			}            
		}


		public IAsyncRelayCommand SimulationCommand
		{
			get
			{
				return _simulationCommand ?? new AsyncRelayCommand(RunSimulationExecute, () => !SimulationRunning);
			}
		}



		public ICommand NewManufacturingStageFile
		{
			get
			{
				return _newMultiStageFileCommand ?? new RelayCommand(NewManufacturingStageFileExecute, () => { return true; });
			}
		}

		private void NewManufacturingStageFileExecute()
		{
            _windowHelper.ShowWindow(_multiStageViewModelFactory.GetNewMultistageJobViewModel());
		}

		public IAsyncRelayCommand AddJobAsyncCommand
		{
			get
			{
				return _addJobAsync ?? new AsyncRelayCommand(AddJobExecuteAsync
					, () => true);
			}
		}

		private async Task<IDocumentViewModel> AddJobExecuteAsync()
		{
			var fileName = _dialogHelper.OpenXMLFileDialog();
			if (fileName != null) {
				return await AddJobAsync(fileName);
            }

			return null;

		}

		public async Task<IDocumentViewModel> AddJobAsync(string fileName)
		{
			if (fileName != null) {
				try {
					var result = await LoadFileAsync(fileName);
					Jobs.Add(result);
					return result;
				} catch (Exception e) {
					var errorString = "";
					errorString = $"{fileName}\n";
					errorString += e.Message;
					_dialogHelper.ShowMessageBox(errorString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}


			return null;
		}

		private Task<IDocumentViewModel> LoadFileAsync([NotNull] string fileName)
		{
			var xElement = new System.Xml.XmlDocument();
			xElement.Load(fileName);

			var documentType = XMLHelper.GetDocumentType(xElement?.DocumentElement?.LocalName);
			if (documentType == XmlDocumentType.MultistageOutputData) {
				var inputDataProvider = _inputDataReader.Create(fileName) as IMultistageBusInputDataProvider;
				return Task.FromResult(_multiStageViewModelFactory.GetMultiStageJobViewModel(inputDataProvider) as IDocumentViewModel);
			} else if (documentType == XmlDocumentType.DeclarationJobData) {
				//Remove
				var inputDataProvider = _inputDataReader.CreateDeclaration(fileName);
				var result = new SimulationOnlyDeclarationJob(inputDataProvider.DataSource,
					inputDataProvider.JobInputData.JobName, XmlDocumentType.DeclarationJobData) as IDocumentViewModel;
				return Task.FromResult(result);


			}else {
				throw new VectoXMLException($"{documentType.ToString()} not supported");
			}

			return null;
		}



		public ICommand AddJob
        {
            get
            {
                return _addJobCommand ?? new RelayCommand(AddJobExecute, () => { return true; });
            }
            private set
            {
                _addJobCommand = value;
                OnPropertyChanged();
            }
        }


        private void AddJobExecute()
        {
			IsLoading = true;
			var filename = _dialogHelper.OpenXMLFileDialog();
			if (filename != null)
            {
               LoadJob(filename);
			}
            else
            {
                IsLoading = false;
            }
        }

		public void LoadJob([NotNull] string fileName)
		{
			fileReadingBackgroundWorker.RunWorkerAsync(fileName);
        }


        public ICommand EditJob
        {
            get
            {
                return _editJobCommand ?? new Util.RelayCommand<IJobViewModel>(EditJobExecute,
                    (IJobViewModel jobentry) => {
						var canExecute = jobentry != null && jobentry.CanBeEdited;
                        return canExecute;
                    });
            }
            set
            {
                _editJobCommand = value;
                OnPropertyChanged();
            }
        }

        private void EditJobExecute(IDocumentViewModel selectedJob)
        {
			if (selectedJob == null) {
				return;
			}
            _windowHelper.ShowWindow(selectedJob.EditViewModel);
        }

        public ICommand ViewXMLFile
        {
            get
            {
                return _viewXMLCommand ?? new Util.RelayCommand<IJobViewModel>(ViewXMLFileExecute,
                    (IJobViewModel jobentry) =>
                    {
                        return (jobentry != null);
                    });
            }
            set
            {
                _viewXMLCommand = value;
                OnPropertyChanged();
            }
        }

        private void ViewXMLFileExecute(IJobViewModel selectedJob)
        {
            if (selectedJob == null) return;

 //TODO implement using WindowHelper.
            Debug.WriteLine("open XML File Viewer");
            //_kernel.Get<XMLViewer>().Show();


        }


        public ICommand RemoveJob
        {
            get
            {
                return _removeJobCommand ?? new Util.RelayCommand<IDocumentViewModel>(RemoveJobExecute, (IDocumentViewModel jobentry) =>
                {
                    return (jobentry != null);
                });
            }
            set
            {
                _removeJobCommand = value;
                OnPropertyChanged();
            }
        }

        private void RemoveJobExecute(IDocumentViewModel selectedDocument)
        {
            if (selectedDocument == null) return;


            Jobs.Remove(selectedDocument);
            OnPropertyChanged();
        }

        public ICommand moveJobUp
        {
            get
            {
                return _moveJobUpCommand ?? new Util.RelayCommand<IDocumentViewModel>(MoveJobUpExecute, (IDocumentViewModel jobentry) =>
                {
                    return (jobentry != null && Jobs.Count > 1 && Jobs.IndexOf(jobentry) != 0);
                });
            }
            set
            {
                _moveJobUpCommand = value;
                OnPropertyChanged();
            }

        }

        private void MoveJobUpExecute(IDocumentViewModel selectedJob)
        {
            if (selectedJob == null) return;
            var index = Jobs.IndexOf(selectedJob);
            if (index > 0)
                Jobs.Move(index, index - 1);

        }

        public ICommand moveJobDown
        {
            get
            {
                return _moveJobDownCommand ?? new Util.RelayCommand<IDocumentViewModel>(MoveJobDownExecute, (IDocumentViewModel jobentry) =>
                {
                    return (jobentry != null && Jobs.Count > 1 && Jobs.IndexOf(jobentry) != Jobs.Count - 1);
                });
            }
            set
            {
                _moveJobDownCommand = value;
                OnPropertyChanged();
            }

        }

        private void MoveJobDownExecute(IDocumentViewModel selectedJob)
        {
            Debug.WriteLine("move down command");

            if (selectedJob == null) return;
            var index = Jobs.IndexOf(selectedJob);
            if (index != Jobs.Count - 1)
                Jobs.Move(index, index + 1);

        }

        #endregion Commands

        #region BackgroundworkerXMLreading

        void fileworker_DoWork(object sender, DoWorkEventArgs e)
        {
            string filename = e.Argument as string;
            Debug.Assert(filename != null);

            try
            {
                var xElement = new System.Xml.XmlDocument();
                xElement.Load(filename);

                var documentType = XMLHelper.GetDocumentType(xElement?.DocumentElement?.LocalName);
                if (documentType == null)
                {
					Debug.WriteLine("Unknown Document Type");
                    e.Cancel = true;
                    return;
                }

				


				var result = _documentViewModelFactory.CreateDocumentViewModel((XmlDocumentType)documentType, filename);
                e.Result = result;
            }
            catch (Exception)
            {
				e.Cancel = true;
                throw;
            }
        }


        void fileworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.Assert(e.Result is IDocumentViewModel);
            Jobs.Add(e.Result as IDocumentViewModel);
            IsLoading = false;
        }

        #endregion
    }


	public class VectoSimulationProgress
	{
		public enum MsgType
		{
			StatusMessage,
			InfoMessage,
			Progress,
			LogError,
			LogWarning,

		}

		public string Message { get; set; }

		public MsgType Type { get; set; }

		public string Link { get; set; }
	}

}
