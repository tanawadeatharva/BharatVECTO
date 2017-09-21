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
			_customerReport = new ReportXMLFile("Customer Report", HashingHelper.IsCustomerReport,
				HashingHelper.ValidateDocumentHash);
			Files = new ObservableCollection<VectoXMLFile> { _jobFile, _manufacturerReport, _customerReport };

			ErrorsAndWarnings = new CompositeCollection();

			AddErrorCollection(_jobFile.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_customerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.ValidationErrors);

			RaisePropertyChanged("CanonicalizationMethods");
			_customerReport.PropertyChanged += Update;
			_manufacturerReport.PropertyChanged += Update;
			_jobFile.PropertyChanged += Update;
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

		private void Update(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "UPDATED") {
				return;
			}
			UpdateReportJobDigest(_manufacturerReport);
			UpdateReportJobDigest(_customerReport);

			_manufacturerReport.JobComponents = _jobFile.Components.ToArray();
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

		private void UpdateReportJobDigest(ReportXMLFile reportXML)
		{
			if (reportXML.FileIntegrityValid == null || !reportXML.FileIntegrityValid.Value || _jobFile.XMLFile.Document == null) {
				reportXML.JobDigestValueComputed = "";
				return;
			}
			try {
				var h = VectoHash.Load(_jobFile.XMLFile.Document);
				var jobDigest = h.ComputeHash(reportXML.JobCanonicalizationMethodRead,
					reportXML.JobDigestMethodRead);
				reportXML.JobDigestValueComputed = jobDigest;
			} catch (Exception) {
				reportXML.JobDigestValueComputed = "";
			}
		}
	}
}
