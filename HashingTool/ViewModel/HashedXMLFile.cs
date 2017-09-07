using System;
using System.ComponentModel;
using System.Xml;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.ViewModel
{
	public class VectoXMLFile : ObservableObject
	{
		protected readonly XMLFile _xmlFile;

		protected string _manufacturerDigestComputed;
		protected bool? _valid;
		protected string _name;
		protected string _tooltip;
		protected readonly Func<XmlDocument, bool?> _contentCheck;
		protected string _componentType;
		protected readonly Action<XmlDocument, VectoXMLFile> _validateHashes;


		public VectoXMLFile(IOService ioService, string name, Func<XmlDocument, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null)
		{
			IoService = ioService;
			_validateHashes = hashValidation;
			_xmlFile = new XMLFile(IoService, true);
			_xmlFile.PropertyChanged += FileChanged;
			Name = name;
			_contentCheck = contentCheck;
			// TODO
			CanonicalizationMethods = new[] {
				"urn:vecto:xml:2017:canonicalization",
				"http://www.w3.org/2001/10/xml-exc-c14n#"
			};
			Valid = null;
			ValidTooltip = VerifyResultDataViewModel.ToolTip_None;
		}

		protected virtual void FileChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_xmlFile.IsValid != null && XMLFile.IsValid.HasValue && _xmlFile.IsValid.Value) {
				Valid = _contentCheck(_xmlFile.Document);
				if (Valid != null && Valid.Value) {
					ValidTooltip = VerifyResultDataViewModel.ToolTip_OK;
				} else {
					ValidTooltip = VerifyResultDataViewModel.ToolTip_InvalidFileType;
				}
			} else {
				Valid = false;
				ValidTooltip = VerifyResultDataViewModel.ToolTip_XMLValidationFailed;
			}

			if (Valid != null && Valid.Value && _validateHashes != null) {
				_validateHashes(_xmlFile.Document, this);
				//try {
				//	_validateHashes(_xmlFile.Document, this);
				//	Valid = result.Valid;
				//	ValidTooltip = result.Valid ? VerifyResultDataViewModel.ToolTip_OK : VerifyResultDataViewModel.ToolTip_HashInvalid;
				//	DigestValueComputed = result.DigestComputed;
				//	DigestValueRead = result.DigestRead;
				//} catch (Exception ex) {
				//	Valid = false;
				//	DigestValueComputed = "";
				//	DigestValueRead = "";
				//}
			}
		}


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
			internal set {
				if (_manufacturerDigestComputed == value) {
					return;
				}
				_manufacturerDigestComputed = value;
				RaisePropertyChanged("DigestValueComputed");
			}
		}


		public bool? Valid
		{
			get { return _valid; }
			internal set {
				if (_valid == value) {
					return;
				}
				_valid = value;
				RaisePropertyChanged("Valid");
			}
		}

		public string ValidTooltip
		{
			get { return _tooltip; }
			set {
				if (_tooltip == value) {
					return;
				}
				_tooltip = value;
				RaisePropertyChanged("ValidTooltip");
			}
		}

		public string Component
		{
			get { return _componentType; }
			set {
				if (_componentType == value) {
					return;
				}
				_componentType = value;
				RaisePropertyChanged("Component");
			}
		}
	}

	public class HashedXMLFile : VectoXMLFile
	{
		protected string _manufacturerDigestRead;

		public HashedXMLFile(IOService ioService, string name, Func<XmlDocument, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(ioService, name, contentCheck, hashValidation) {}

		public string DigestValueRead
		{
			get { return _manufacturerDigestRead; }
			internal set {
				if (_manufacturerDigestRead == value) {
					return;
				}
				_manufacturerDigestRead = value;
				RaisePropertyChanged("DigestValueRead");
			}
		}
	}

	public class ReportXMLFile : HashedXMLFile
	{
		private string _jobDigestRead;

		public ReportXMLFile(IOService ioService, string name, Func<XmlDocument, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null)
			: base(ioService, name, contentCheck, hashValidation)
		{
			_xmlFile.PropertyChanged += ReadJobDigest;
		}

		private void ReadJobDigest(object sender, PropertyChangedEventArgs e)
		{
			var jobDigest = "";
			if (e.PropertyName != "Document") {
				return;
			}
			if (_xmlFile.Document != null && _xmlFile.Document.DocumentElement != null) {
				var node =
					_xmlFile.Document.SelectSingleNode("//*[local-name()='InputDataSignature']//*[local-name()='DigestValue']");
				if (node != null) {
					jobDigest = node.InnerText;
				}
			}
			JobDigest = jobDigest;
		}

		public string JobDigest
		{
			get { return _jobDigestRead; }
			internal set {
				if (_jobDigestRead == value) {
					return;
				}
				_jobDigestRead = value;
				RaisePropertyChanged("JobDigest");
			}
		}
	}
}
