using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyResultDataViewModel : ObservableObject, IMainView
	{
		private readonly VectoJobFile _jobFile;
		private readonly ReportXMLFile _customerReport;
		private readonly ManufacturerReportXMLFile _manufacturerReport;


		public VerifyResultDataViewModel()
		{
			_jobFile = new VectoJobFile("Job File", HashingHelper.IsJobFile, HashingHelper.HashJobFile);
			_manufacturerReport = new ManufacturerReportXMLFile("Manufacturer Report", HashingHelper.IsManufacturerReport,
				HashingHelper.ValidateDocumentHash);
			_manufacturerReport.JobData = _jobFile;
			_customerReport = new ReportXMLFile("Customer Report", HashingHelper.IsCustomerReport,
				HashingHelper.ValidateDocumentHash);
			_customerReport.JobData = _jobFile;
			Files = new ObservableCollection<VectoXMLFile> { _jobFile, _manufacturerReport, _customerReport };

			ErrorsAndWarnings = new CompositeCollection();

			AddErrorCollection(_jobFile.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_customerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.ValidationErrors);
			AddErrorCollection(_customerReport.ValidationErrors);

			RaisePropertyChanged("CanonicalizationMethods");
		}

		private void AddErrorCollection(ObservableCollection<string> errorCollection)
		{
			ErrorsAndWarnings.Add(new CollectionContainer { Collection = errorCollection });
			errorCollection.CollectionChanged += UpdateCount;
		}

		private void UpdateCount(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged("ErrorCount");
		}

		public int ErrorCount
		{
			get { return ErrorsAndWarnings.Cast<CollectionContainer>().Sum(entry => (entry.Collection as ICollection).Count); }
		}


		public string Name
		{
			get { return "Verify Result Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public VectoJobFile JobFile
		{
			get { return _jobFile; }
		}


		public ReportXMLFile CustomerReport
		{
			get { return _customerReport; }
		}

		public ManufacturerReportXMLFile ManufacturerReport
		{
			get { return _manufacturerReport; }
		}

		public ObservableCollection<VectoXMLFile> Files { get; private set; }

		public CompositeCollection ErrorsAndWarnings { get; private set; }
	}
}
