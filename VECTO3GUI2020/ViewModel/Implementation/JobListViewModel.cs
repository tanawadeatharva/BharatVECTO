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
			await RunSimulationAsync(cancellationTokenSource.Token,
				new Progress<MessageEntry>((message) => { _outputViewModel.Messages.Add(message); }),
				new Progress<int>((i) => _outputViewModel.Progress = i));
			SimulationRunning = false;
			_outputViewModel.Progress = 0;
			cancellationTokenSource.Dispose();
        }

		private async Task RunSimulationAsync(CancellationToken ct, IProgress<MessageEntry> outputMessages, IProgress<int> progress)
		{
            progress.Report(0);
			//for (int i = 0; i <= 100; i++) {
			//	await Task.Delay(0);
			//	progress.Report(i);
			//	if (ct.IsCancellationRequested) {
			//		return;
			//	}
			//}

			IDocumentViewModel[] jobs;
			lock (_jobsLock) {
				jobs = Jobs.Where(x => x.Selected).ToArray();
				if (jobs.Length == 0) {
                    outputMessages.Report(new MessageEntry() {
                        Message = "No Jobs Selected",
                        Time = DateTime.Now,
                        Type = MessageType.InfoMessage,
					});
				}
			}

            //TODO add output path to settings
			var outputPath = Settings.Default.DefaultFilePath;
			
			var sumFileWriter = new FileOutputWriter(outputPath);



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
					{/*
						WriteModalResults = true,
						ModalResults1Hz = true,
						Validate = true,
						ActualModalData = true,
						SerializeVectoRunData = true
						*/
					};
					foreach (var runId in jobContainer.AddRuns(runsFactory))
					{
						fileWriters.Add(runId, fileWriter);
					}

					// TODO MQ-20200525: Remove the following loop in production (or after evaluation of LAC!!

					/*
					if (!string.IsNullOrWhiteSpace(LookAheadMinSpeedOverride))
					{
						foreach (var run in jobContainer.Runs)
						{
							var tmpDriver = ((VectoRun)run.Run).GetContainer().RunData.DriverData;
							tmpDriver.LookAheadCoasting.Enabled = true;
							tmpDriver.LookAheadCoasting.MinSpeed = LookAheadMinSpeedOverride.ToDouble().KMPHtoMeterPerSecond();
						}
					}
					*/

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



		}

		private string GetOutputDirectory(string jobFilePath)
		{
			var outFile = jobFilePath;
			var OutputDirectory = Settings.Default.DefaultFilePath;
			if (!string.IsNullOrWhiteSpace(OutputDirectory))
			{
				if (Path.IsPathRooted(OutputDirectory))
				{
					outFile = Path.Combine(OutputDirectory, Path.GetFileName(jobFilePath) ?? "");
				}
				else
				{
					outFile = Path.Combine(Path.GetDirectoryName(jobFilePath) ?? "", OutputDirectory, Path.GetFileName(jobFilePath) ?? "");
				}
				if (!Directory.Exists(Path.GetDirectoryName(outFile)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(outFile));
				}
			}

			return outFile;
		}


		#region Commands

		public ICommand CancelSimulation
		{
			get
			{
				return _cancelSimulationCommand ?? new RelayCommand(() => { cancellationTokenSource.Cancel(); },
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
			} else {
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
            //Another possibility is to use IsAsync true property of Binding.
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
                    (IJobViewModel jobentry) =>
                    {
                        return (jobentry != null);
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
