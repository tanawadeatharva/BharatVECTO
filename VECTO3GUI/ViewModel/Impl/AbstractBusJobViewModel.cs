using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI.Helper;
using VECTO3GUI.Model;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{

	public enum JobFileType
	{
		CompletedBusFile,
		PrimaryBusFile,
		PIFBusFile
	}

	public static class JobFileTypeHelper
	{
		public static string GetLable(this JobFileType fileType)
		{
			switch (fileType)
			{
				case JobFileType.CompletedBusFile:
					return "Completed Bus File";
				case JobFileType.PrimaryBusFile:
					return "Primary Bus File";
				case JobFileType.PIFBusFile:
					return "PIF Bus File";
				default:
					return string.Empty;
			}

		}
	}



	public abstract class AbstractBusJobViewModel : ViewModelBase, IBusJobViewModel
	{
		#region Members

		private JobFileType _firstFileType;
		private JobFileType _secondFileType;

		private string _firstLabelText;
		private string _secondLabelText;
		private string _firstFilePath;
		private string _secondFilePath;

		private ICommand _selectFirstFileCommand;
		private ICommand _selectSecondFileCommand;
		private ICommand _cancelCommand;
		private ICommand _saveCommand;

		protected SettingsModel Settings { get; private set; }

		protected JobType JobType;
		private readonly bool _editJob;

		#endregion

		#region Properties

		public JobEntry SavedJobEntry { get; private set; }

		public string FirstFilePath
		{
			get { return _firstFilePath; }
			set { SetProperty(ref _firstFilePath, value); }
		}

		public string SecondFilePath
		{
			get { return _secondFilePath; }
			set { SetProperty(ref _secondFilePath, value); }
		}

		public string FirstLabelText
		{
			get { return _firstLabelText; }
			set { SetProperty(ref _firstLabelText, value); }
		}

		public string SecondLabelText
		{
			get { return _secondLabelText; }
			set { SetProperty(ref _secondLabelText, value); }
		}

		public JobFileType FirstFileType
		{
			get { return _firstFileType; }
			set { SetProperty(ref _firstFileType, value); }
		}
		public JobFileType SecondFileType
		{
			get { return _secondFileType; }
			set { SetProperty(ref _secondFileType, value); }
		}

		#endregion

		protected AbstractBusJobViewModel(IKernel kernel, JobType jobType)
		{
			Init(kernel, jobType);
			_editJob = false;
		}

		protected AbstractBusJobViewModel(IKernel kernel, JobEntry jobEntry)
		{
			Init(kernel, jobEntry.JobType);
			SetJobEntryData(jobEntry);
			SavedJobEntry = jobEntry;
			_editJob = true;
		}

		private void Init(IKernel kernel, JobType jobType)
		{
			SecondLabelText = $"Select {JobFileType.CompletedBusFile.GetLable()}";
			Settings = new SettingsModel();
			SetFileTypes(jobType);
			Kernel = kernel;
		}

		private void SetFileTypes(JobType jobType)
		{
			JobType = jobType;

			_firstFileType = jobType == JobType.SingleBusJob
				? JobFileType.PrimaryBusFile
				: JobFileType.PIFBusFile;

			_secondFileType = JobFileType.CompletedBusFile;
		}

		private void SetJobEntryData(JobEntry jobEntry)
		{
			FirstFilePath = jobEntry.FirstFilePath;
			SecondFilePath = jobEntry.SecondFilePath;
		}

		protected abstract void SetFirstFileLabel();

		#region Commands

		public ICommand SelectFirstFileCommand
		{
			get
			{
				return _selectFirstFileCommand ??
					  (_selectFirstFileCommand = new RelayCommand<JobFileType>(DoSelectFirstFileCommand));
			}
		}
		private void DoSelectFirstFileCommand(JobFileType jobFileType)
		{
			FirstFilePath = OpenFileSelector(jobFileType, nameof(FirstFilePath));
		}

		public ICommand SelectSecondFileCommand
		{
			get
			{
				return _selectSecondFileCommand ??
						(_selectSecondFileCommand = new RelayCommand<JobFileType>(DoSelectSecondFileCommand));
			}
		}
		private void DoSelectSecondFileCommand(JobFileType jobFileType)
		{
			SecondFilePath = OpenFileSelector(jobFileType, nameof(SecondFilePath));
		}

		public ICommand CancelCommand
		{
			get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<Window>(DoCancelCommand)); }
		}
		private void DoCancelCommand(Window window)
		{
			window.Close();
		}

		public ICommand SaveCommand
		{
			get { return _saveCommand ?? (_saveCommand = new RelayCommand<Window>(DoSaveCommand, CanSaveCommand)); }
		}
		private bool CanSaveCommand(Window window)
		{
			return !HasErrors && !FirstFilePath.IsNullOrEmpty() && !SecondFilePath.IsNullOrEmpty();
		}
		private void DoSaveCommand(Window window)
		{
			window.DialogResult = true;
			if (!_editJob)
				SaveJob(window);
			else
				UpdateJobData();
		}

		#endregion

		private void SaveJob(Window window)
		{
			var jobFilePath = FileDialogHelper.SaveJobFileToDialog(Settings.XmlFilePathFolder);
			if (jobFilePath == null)
				return;

			var job = new JobEntry
			{
				JobEntryFilePath = jobFilePath,
				FirstFilePath = FirstFilePath,
				SecondFilePath = SecondFilePath,
				JobType = JobType
			};

			SerializeHelper.SerializeToFile(jobFilePath, job);
			SavedJobEntry = job;
			DoCancelCommand(window);

		}

		private void UpdateJobData()
		{
			SavedJobEntry.FirstFilePath = FirstFilePath;
			SavedJobEntry.SecondFilePath = SecondFilePath;
		}


		private string OpenFileSelector(JobFileType jobFileType, string textPropertyName)
		{
			var dialogResult = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.XMLFilter, Settings.XmlFilePathFolder);
			if (dialogResult == null)
				return null;

			var filePath = dialogResult.FirstOrDefault();
			var validationResult = IsValideXml(jobFileType, filePath);

			if (!validationResult)
				AddPropertyError(textPropertyName, $"Selected XML-File is not a valid {jobFileType.GetLable()}!");
			else
				RemovePropertyError(textPropertyName);

			return !validationResult ? null : filePath;
		}

		private bool IsValideXml(JobFileType jobFileType, string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return false;

			var xmlInputReader = Kernel.Get<IXMLInputDataReader>();

			using (var reader = XmlReader.Create(filePath))
			{

				var readerResult = xmlInputReader.Create(reader);
				if (readerResult is IDeclarationInputDataProvider)
				{

					var inputData = readerResult as IDeclarationInputDataProvider;
					if (jobFileType == JobFileType.CompletedBusFile &&
						inputData.JobInputData.Vehicle is XMLDeclarationCompletedBusDataProviderV26)
					{
						return true;
					}
					if (jobFileType == JobFileType.PrimaryBusFile &&
						inputData.JobInputData.Vehicle is XMLDeclarationPrimaryBusVehicleDataProviderV26)
					{
						return true;
					}
				}
				else if (readerResult is IPrimaryVehicleInformationInputDataProvider)
				{
					var inputData = readerResult as IPrimaryVehicleInformationInputDataProvider;
					if (jobFileType == JobFileType.PIFBusFile &&
						inputData.Vehicle is XMLDeclarationPrimaryVehicleBusDataProviderV01)
						return true;
				}
				return false;
			}
		}
	}
}
