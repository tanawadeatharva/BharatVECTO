using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyResultDataViewModel : ObservableObject, IMainView
	{
		private ApplicationViewModel _applicationViewModel;
		private VectoXMLFile _jobFile;
		private ReportXMLFile _customerReport;
		private ReportXMLFile _manufacturerReport;

		public const string ToolTip_InvalidFileType = "Invalid File type!";
		public const string ToolTip_XMLValidationFailed = "XML validation failed!";
		public const string ToolTip_OK = "Correct file selected";
		public const string ToolTip_HashInvalid = "Incorrect digest value!";
		public const string ToolTip_None = "";


		public VerifyResultDataViewModel()
		{
			_jobFile = new VectoXMLFile(IoService, "Job File", IsJobFile, HashJobFile);
			_manufacturerReport = new ReportXMLFile(IoService, "Manufacturer Report", IsManufacturerReport, ValidateDocumentHash);
			_customerReport = new ReportXMLFile(IoService, "Customer Report", IsCustomerReport, ValidateDocumentHash);
			Files = new ObservableCollection<VectoXMLFile> { _jobFile, _manufacturerReport, _customerReport };

			CanonicalizationMethods = new ObservableCollection<string>() { "urn:vecto:xml:2017:canonicalization" };
			RaisePropertyChanged("CanonicalizationMethods");
			_customerReport.PropertyChanged += Update;
			_manufacturerReport.PropertyChanged += Update;
		}

		private void Update(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanged("ManufacturerReportValid");
			RaisePropertyChanged("CustomerReportReportValid");
		}


		public VerifyResultDataViewModel(ApplicationViewModel applicationViewModel) : this()
		{
			_applicationViewModel = applicationViewModel;
		}

		public ObservableCollection<string> CanonicalizationMethods { get; private set; }

		public string Name
		{
			get { return "Verify Result Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public VectoXMLFile JobFile
		{
			get { return _jobFile; }
		}


		public HashedXMLFile CustomerReport
		{
			get { return _customerReport; }
		}

		public HashedXMLFile ManufacturerReport
		{
			get { return _manufacturerReport; }
		}

		public ObservableCollection<VectoXMLFile> Files { get; private set; }

		public bool ManufacturerReportValid
		{
			get {
				return _manufacturerReport.Valid != null && _manufacturerReport.Valid.Value &&
						_manufacturerReport.JobDigest == _jobFile.DigestValueComputed;
			}
		}

		public bool CustomerReportReportValid
		{
			get {
				return _customerReport.Valid != null && _customerReport.Valid.Value &&
						_customerReport.JobDigest == _jobFile.DigestValueComputed;
			}
		}

		private void HashJobFile(XmlDocument xml, VectoXMLFile xmlViewModel)
		{
			try {
				var h = VectoHash.Load(xml);
				xmlViewModel.DigestValueComputed = h.ComputeHash();
			} catch (Exception e) {
				xmlViewModel.DigestValueComputed = "";
			}
		}

		private void ValidateDocumentHash(XmlDocument xml, VectoXMLFile xmlViewModel)
		{
			var report = xmlViewModel as ReportXMLFile;
			if (report == null) {
				return;
			}
			try {
				var h = VectoHash.Load(xml);
				try {
					report.DigestValueRead = h.ReadHash();
				} catch {
					report.DigestValueRead = "";
				}
				try {
					report.DigestValueComputed = h.ComputeHash();
				} catch {
					report.DigestValueComputed = "";
				}
				report.Valid = h.ValidateHash();
			} catch (Exception e) {
				report.Valid = false;
			}
		}
	}
}
