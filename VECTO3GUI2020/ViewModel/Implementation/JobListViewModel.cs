using Microsoft.Win32;
using Ninject;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;
using VECTO3GUI2020.Views;
using IDocumentViewModel = VECTO3GUI2020.ViewModel.Interfaces.Document.IDocumentViewModel;

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
		private IViewModelFactory _viewModelFactory;

		#endregion


        public JobListViewModel()
        {
            InitFileBackGroundWorker();
        }


        public JobListViewModel(IDocumentViewModelFactory documentViewModelFactory,
            IDialogHelper dialogHelper,
            IWindowHelper windowHelper,
			IViewModelFactory viewModelFactory) : this()
        {
            _documentViewModelFactory = documentViewModelFactory;
            _dialogHelper = dialogHelper;
            _windowHelper = windowHelper;
			_viewModelFactory = viewModelFactory;
		}



        private void InitFileBackGroundWorker()
        {
            fileReadingBackgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = false
            };
            fileReadingBackgroundWorker.DoWork += fileworker_DoWork;
            fileReadingBackgroundWorker.ProgressChanged += fileworker_ProgressChanged;
            fileReadingBackgroundWorker.RunWorkerCompleted += fileworker_RunWorkerCompleted;
        }





        #region Commands

		public ICommand NewManufacturingStageFile
		{
			get
			{
				return _newMultiStageFileCommand ?? new RelayCommand(NewManufacturingStageFileExecute, () => { return true; });
			}
		}

		private void NewManufacturingStageFileExecute()
		{
            _windowHelper.ShowWindow(_viewModelFactory.createManufacturingStageEditViewModel());
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
			string path = _settings.DefaultFilePath;
			var filename = _dialogHelper.OpenXMLFileDialog(path);

			if (filename != null)
            {
                fileReadingBackgroundWorker.RunWorkerAsync(filename);
			}
            else
            {
                IsLoading = false;
            }
        }


        public ICommand EditJob
        {
            get
            {
                return _editJobCommand ?? new RelayCommand<IJobViewModel>(EditJobExecute,
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
                return _viewXMLCommand ?? new RelayCommand<IJobViewModel>(ViewXMLFileExecute,
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
                return _removeJobCommand ?? new RelayCommand<IDocumentViewModel>(RemoveJobExecute, (IDocumentViewModel jobentry) =>
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
                return _moveJobUpCommand ?? new RelayCommand<IDocumentViewModel>(MoveJobUpExecute, (IDocumentViewModel jobentry) =>
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
                return _moveJobDownCommand ?? new RelayCommand<IDocumentViewModel>(MoveJobDownExecute, (IDocumentViewModel jobentry) =>
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

			//TODO: update usage of GetDocumentType;
   //         //Loading the file
			//try {
			//	var xElement = new System.Xml.XmlDocument();
			//	xElement.Load(filename);
			//	var documentType = XMLHelper.GetDocumentType(xElement.);
			//	if (documentType == null) {
			//		Debug.WriteLine("Unknown Document Type");
			//		e.Cancel = true;
			//		return;
			//	}

			//	var result = _documentViewModelFactory.CreateDocumentViewModel((XmlDocumentType)documentType, filename);
			//	e.Result = result;
			//} catch (Exception) {
			//	e.Cancel = true;
			//	throw;
			//}
		}

        void fileworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

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
