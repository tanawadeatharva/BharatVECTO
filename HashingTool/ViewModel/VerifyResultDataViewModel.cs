using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml;
using HashingTool.Helper;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyResultDataViewModel : ObservableObject, IMainView
	{
		private readonly VectoJobFile _jobFile;
		private readonly ReportXMLFile _customerReport;
		private readonly ReportXMLFile _manufacturerReport;


		public VerifyResultDataViewModel()
		{
			_jobFile = new VectoJobFile("Job File", HashingHelper.IsJobFile, HashingHelper.HashJobFile);
			_manufacturerReport = new ReportXMLFile("Manufacturer Report", HashingHelper.IsManufacturerReport,
				HashingHelper.ValidateDocumentHash);
			_customerReport = new ReportXMLFile("Customer Report", HashingHelper.IsCustomerReport,
				HashingHelper.ValidateDocumentHash);
			Files = new ObservableCollection<VectoXMLFile> { _jobFile, _manufacturerReport, _customerReport };

			RaisePropertyChanged("CanonicalizationMethods");
			_customerReport.PropertyChanged += Update;
			_manufacturerReport.PropertyChanged += Update;
		}

		private void Update(object sender, PropertyChangedEventArgs e)
		{
			//RaisePropertyChanged("ManufacturerReportValid");
			//RaisePropertyChanged("CustomerReportReportValid");
			UpdateReportJobDigest(_manufacturerReport);
			UpdateReportJobDigest(_customerReport);
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

		public ReportXMLFile ManufacturerReport
		{
			get { return _manufacturerReport; }
		}

		public ObservableCollection<VectoXMLFile> Files { get; private set; }

		//public bool ManufacturerReportValid
		//{
		//	get {
		private void UpdateReportJobDigest(ReportXMLFile reportXML)
		{
			if (reportXML.Valid == null || !reportXML.Valid.Value) {
				reportXML.JobDigestValueComputed = "";
				return;
			}
			try {
				var h = VectoHash.Load(_jobFile.XMLFile.Document);
				var jobDigest = h.ComputeHash(reportXML.JobCanonicalizationMethodRead,
					reportXML.JobDigestMethodRead);
				reportXML.JobDigestValueComputed = jobDigest;
			} catch (Exception e) {
				reportXML.JobDigestValueComputed = "";
			}
		}

		//	}
		//}

		//public bool CustomerReportReportValid
		//{
		//	get {
		//		if (_customerReport.Valid == null || !_customerReport.Valid.Value) {
		//			return false;
		//		}
		//		try {
		//			var h = VectoHash.Load(_jobFile.XMLFile.Document);
		//			var jobDigest = h.ComputeHash(_customerReport.JobCanonicalizationMethodRead, _customerReport.JobDigestMethodRead);
		//			_customerReport.JobDigestValueComputed = jobDigest;
		//			return _customerReport.JobDigestValueRead == jobDigest;
		//		} catch (Exception e) {
		//			return false;
		//		}
		//	}
		//}
	}
}
