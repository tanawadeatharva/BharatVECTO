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
			RaisePropertyChanged("ManufacturerReportValid");
			RaisePropertyChanged("CustomerReportReportValid");
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

		public bool ManufacturerReportValid
		{
			get {
				return _manufacturerReport.Valid != null && _manufacturerReport.Valid.Value &&
						_manufacturerReport.JobDigestValue == _jobFile.DigestValueComputed;
			}
		}

		public bool CustomerReportReportValid
		{
			get {
				return _customerReport.Valid != null && _customerReport.Valid.Value &&
						_customerReport.JobDigestValue == _jobFile.DigestValueComputed;
			}
		}

	}
}
