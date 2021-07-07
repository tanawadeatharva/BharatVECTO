using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public interface ICreateVifViewModel: IDocumentViewModel, IEditViewModel
	{

	}
    public class CreateVifViewModel : ViewModelBase, ICreateVifViewModel
	{
		private string _primaryInputFile;
		private string _completedInputFile;
		private readonly IDialogHelper _dialogHelper;
		private readonly IXMLInputDataReader _inputDataReader;

		private static uint _newVifCounter = 0;

		public CreateVifViewModel(IDialogHelper dialogHelper, IXMLInputDataReader inputDataReader)
		{
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
			Title = "Create VIF";
			_documentName = $"New Vif {++_newVifCounter}";
		}


		public string PrimaryInputFile
		{
			get => _primaryInputFile;
			set => SetProperty(ref _primaryInputFile, value);
		}

		public string CompletedInputFile
		{
			get => _completedInputFile;
			set => SetProperty(ref _completedInputFile, value);
		}

		public ICommand SelectCompletedInputFileCommand
		{
			get => _selectCompletedInputFileCommand ?? (_selectCompletedInputFileCommand = new RelayCommand(() => {
				var selectedFile = _dialogHelper.OpenXMLFileDialog();
				
			}));
		}

		public ICommand SelectPrimaryInputFileCommand
		{
			get => _selectPrimaryInputFileCommand ?? (_selectPrimaryInputFileCommand = new RelayCommand(() => {
				PrimaryInputFile = _dialogHelper.OpenXMLFileDialog();

			}));
		}


		public bool CheckDocumentType(string fileName, XmlDocumentType expectedDocumentType)
		{
			var xElement = new System.Xml.XmlDocument();
			xElement.Load(fileName);

			var documentType = XMLHelper.GetDocumentType(xElement?.DocumentElement?.LocalName);
			return documentType.HasValue && documentType.Value == expectedDocumentType;
		}


		#region Commands

		private ICommand _selectPrimaryInputFileCommand;
		private ICommand _selectCompletedInputFileCommand;
		private bool _selected;
		private  string _documentName;

		#endregion


		#region Implementation of IDocumentViewModel
		public string DocumentName
		{
			get => _documentName;
			set => SetProperty(ref _documentName, value);
		}


		public XmlDocumentType DocumentType => throw new NotImplementedException();

		public DataSource DataSource => null;

		public IEditViewModel EditViewModel => this;

		public bool Selected
		{
			get => _selected;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated
		{
			get => true;
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
