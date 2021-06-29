using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;

namespace VECTO3GUI2020.ViewModel.Implementation
{
	public interface ICreateVifViewModel
	{

	}
    public class CreateVifViewModel : ViewModelBase, ICreateVifViewModel
	{
		private string _primaryInputFile;
		private string _completedInputFile;
		private readonly IDialogHelper _dialogHelper;
		private readonly IXMLInputDataReader _inputDataReader;


		public CreateVifViewModel(IDialogHelper dialogHelper, IXMLInputDataReader inputDataReader)
		{
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
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


		#endregion




	}
}
