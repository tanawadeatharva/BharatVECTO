using Microsoft.Win32;
using Ninject;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
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

		public void JobDataGrid_OnDrop(object sender, DragEventArgs e)
		{
			throw new System.NotImplementedException();
		}




        #region Commands


		public IAsyncRelayCommand SimulationCommand
		{
			get => _simulationCommand ?? new AsyncRelayCommand(RunSimulationAsync, () => true);
		}

		private Task RunSimulationAsync(CancellationToken arg)
		{
            
            _outputViewModel.Messages.Add("hi");
			return null;

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
}
