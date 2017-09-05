using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.ViewModel
{
	public class VerifyResultDataViewModel : ObservableObject, IMainView
	{
		private ApplicationViewModel _applicationViewModel;
		private HashedXMLFile _jobFile;
		private HashedXMLFile _customerReport;
		private HashedXMLFile _manufacturerReport;

		public VerifyResultDataViewModel()
		{
			_jobFile = new HashedXMLFile(_ioService, "Job File");
			_manufacturerReport = new HashedXMLFile(_ioService, "Manufacturer Report");
			_customerReport = new HashedXMLFile(_ioService, "Customer Report");
			Files = new ObservableCollection<HashedXMLFile>();
			Files.Add(_jobFile);
			Files.Add(_manufacturerReport);
			Files.Add(_customerReport);

			CanonicalizationMethods = new ObservableCollection<string>() { "urn:vecto:xml:2017:canonicalization" };
			RaisePropertyChanged("CanonicalizationMethods");
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

		public HashedXMLFile JobFile
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

		public ObservableCollection<HashedXMLFile> Files { get; private set; }


		public class HashedXMLFile : ObservableObject
		{
			private XMLFile _xmlFile;

			private string _manufacturerDigestComputed;
			private string _manufacturerDigestRead;
			private bool? _valid;
			private string _name;


			public HashedXMLFile(IOService ioService, string name)
			{
				_ioService = ioService;
				_xmlFile = new XMLFile(_ioService, true);
				Name = name;

				CanonicalizationMethods = new[] {
					"urn:vecto:xml:2017:canonicalization",
					"http://www.w3.org/2001/10/xml-exc-c14n#"
				};
				Valid = true;
			}

			public HashedXMLFile() {}


			public XMLFile XMLFile
			{
				get { return _xmlFile; }
			}

			public string Name
			{
				get { return _name; }
				private set {
					if (_name == value) {
						return;
					}
					_name = value;
					RaisePropertyChanged("Name");
				}
			}

			public string[] CanonicalizationMethods { get; private set; }


			public string DigestValueComputed
			{
				get { return _manufacturerDigestComputed; }
				private set {
					if (_manufacturerDigestComputed == value) {
						return;
					}
					_manufacturerDigestComputed = value;
					RaisePropertyChanged("DigestValueComputed");
				}
			}

			public string DigestValueRead
			{
				get { return _manufacturerDigestRead; }
				private set {
					if (_manufacturerDigestRead == value) {
						return;
					}
					_manufacturerDigestRead = value;
					RaisePropertyChanged("DigestValueRead");
				}
			}

			public bool? Valid
			{
				get { return _valid; }
				private set {
					if (_valid == value) {
						return;
					}
					_valid = value;
					RaisePropertyChanged("Valid");
				}
			}
		}
	}
}
