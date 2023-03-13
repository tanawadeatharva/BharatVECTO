using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.Implementation.Document
{
    internal class CompletedBusV7ViewModel : ViewModelBase, IJobViewModel
    {
		private bool _selected;
		private DataSource _dataSource;

		private IMultistageVIFInputData _inputData;
		private string _documentName;

		#region Implementation of IDocumentViewModel

		public CompletedBusV7ViewModel(IInputDataProvider inputData)
		{
			_dataSource = inputData.DataSource;
			_inputData = inputData as IMultistageVIFInputData;
			if (_inputData == null) {
				throw new VectoException("Invalid input file");
			}

			_documentName = Path.GetFileNameWithoutExtension(_inputData.DataSource.SourceFile);
		}

		public string DocumentName => _documentName;

		public XmlDocumentType? DocumentType => null;

		public string DocumentTypeName => "CompletedBus";

		public DataSource DataSource => _dataSource;

		public IEditViewModel EditViewModel => null;

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
	}
}
