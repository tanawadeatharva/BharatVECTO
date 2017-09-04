using System.Windows.Input;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.ViewModel
{
	public class VerifyResultDataViewModel : ObservableObject, IMainView
	{
		private ApplicationViewModel _applicationViewModel;
		private XMLFile _jobFile;
		private XMLFile _manufacturerReport;
		private XMLFile _customerReport;
		private string _jobDigestValue;
		private string _manufacturerDigestComputed;
		private string _manufacturerDigestRead;
		private string _customerDigestComputed;
		private string _customerDigestRead;
		private bool? _manufacturerReportValid;
		private bool? _customerReportValid;
		private string _manufacturerReportJobDigest;
		private string _customerReportJobDigest;
		private bool _manufacturerReportJobDigestValid;
		private bool _customerReportJobDigestValid;

		public VerifyResultDataViewModel() {}

		public VerifyResultDataViewModel(ApplicationViewModel applicationViewModel) : this()
		{
			_applicationViewModel = applicationViewModel;
			_jobFile = new XMLFile(_ioService, true);
			_manufacturerReport = new XMLFile(_ioService, true);
			_customerReport = new XMLFile(_ioService, true);
		}

		public string Name
		{
			get { return "Verify Result Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public XMLFile JobFile
		{
			get { return _jobFile; }
		}

		public XMLFile ManufacturerReport
		{
			get { return _manufacturerReport; }
		}

		public XMLFile CustomerReport
		{
			get { return _customerReport; }
		}

		public string JobDigestValueComputed
		{
			get { return _jobDigestValue; }
			private set {
				if (_jobDigestValue == value) {
					return;
				}
				_jobDigestValue = value;
				RaisePropertyChanged("JobDigestValueComputed");
			}
		}

		public string ManufacturerReportDigestValueComputed
		{
			get { return _manufacturerDigestComputed; }
			private set {
				if (_manufacturerDigestComputed == value) {
					return;
				}
				_manufacturerDigestComputed = value;
				RaisePropertyChanged("ManufacturerReportDigestValueComputed");
			}
		}

		public string ManufacturerReportDigestValueRead
		{
			get { return _manufacturerDigestRead; }
			private set {
				if (_manufacturerDigestRead == value) {
					return;
				}
				_manufacturerDigestRead = value;
				RaisePropertyChanged("ManufacturerReportDigestValueRead");
			}
		}

		public bool? ManufacturerReportValid
		{
			get { return _manufacturerReportValid; }
			private set {
				if (_manufacturerReportValid == value) {
					return;
				}
				_manufacturerReportValid = value;
				RaisePropertyChanged("ManufacturerReportValid");
			}
		}

		public string CustomerReportDigestValueComputed
		{
			get { return _customerDigestComputed; }
			private set {
				if (_customerDigestComputed == value) {
					return;
				}
				_customerDigestComputed = value;
				RaisePropertyChanged("CustomerReportDigestValueComputed");
			}
		}

		public string CustomerReportDigestValueRead
		{
			get { return _customerDigestRead; }
			private set {
				if (_customerDigestRead == value) {
					return;
				}
				_customerDigestRead = value;
				RaisePropertyChanged("CustomerReportDigestValueRead");
			}
		}

		public bool? CustomerReportValid
		{
			get { return _customerReportValid; }
			private set {
				if (_customerReportValid == value) {
					return;
				}
				_customerReportValid = value;
				RaisePropertyChanged("CustomerReportValid");
			}
		}

		public string ManufacturerReportJobDigestValue
		{
			get { return _manufacturerReportJobDigest; }
			private set {
				if (_manufacturerReportJobDigest == value) {
					return;
				}
				_manufacturerReportJobDigest = value;
				RaisePropertyChanged("ManufacturerReportJobDigestValue");
			}
		}

		public string CustomerReportJobDigestValue
		{
			get { return _customerReportJobDigest; }
			private set {
				if (_customerReportJobDigest == value) {
					return;
				}
				_customerReportJobDigest = value;
				RaisePropertyChanged("CustomerReportJobDigestValue");
			}
		}


		public bool ManufacturerReportJobDigestValid
		{
			get { return _manufacturerReportJobDigestValid; }
			private set {
				if (_manufacturerReportJobDigestValid == value) {
					return;
				}
				_manufacturerReportJobDigestValid = value;
				RaisePropertyChanged("ManufacturerReportJobDigestValid");
			}
		}

		public bool CustomerReportJobDigestValid
		{
			get { return _customerReportJobDigestValid; }
			private set {
				if (_customerReportJobDigestValid == value) {
					return;
				}
				_customerReportJobDigestValid = value;
				RaisePropertyChanged("CustomerReportJobDigestValid");
			}
		}
	}
}
