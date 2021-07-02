using System;
using System.Configuration;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.ViewModel.Implementation.Document
{
	public class SimulationOnlyDeclarationJob : ViewModelBase, IDocumentViewModel, IJobViewModel
	{
		#region Implementation of IDocumentViewModel

		public string DocumentName
		{
			get => _documentName;
			set => _documentName = value;
		}

		public XmlDocumentType DocumentType
		{
			get => _documentType;
			set => _documentType = value;
		}

		public DataSource DataSource
		{
			get => _dataSource;
			set => _dataSource = value;
		}

		public IEditViewModel EditViewModel { get; set; }

		private bool _selected;
		private XmlDocumentType _documentType;
		private string _documentName;
		private DataSource _dataSource;

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

		#endregion

		public SimulationOnlyDeclarationJob(DataSource dataSource, string name, XmlDocumentType documentType)
		{
			_documentType = documentType;
			_dataSource = dataSource;
			_documentName = name;
		}

	}

}