using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VectoXMLFile : ObservableObject
	{
		protected readonly XMLFile _xmlFile;

		protected string _manufacturerDigestComputed;
		protected bool? _valid;
		protected string _name;
		protected string _tooltip;
		protected readonly Func<XmlDocument, Collection<string>, bool?> _contentCheck;
		protected string _componentType;
		protected readonly Action<XmlDocument, VectoXMLFile> _validateHashes;


		public VectoXMLFile(string name, Func<XmlDocument, Collection<string>, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null)
		{
			_validateHashes = hashValidation;
			_xmlFile = new XMLFile(IoService, true, contentCheck);
			_xmlFile.PropertyChanged += FileChanged;
			Name = name;

			// TODO
			CanonicalizationMethods = new[] {
				"urn:vecto:xml:2017:canonicalization",
				"http://www.w3.org/2001/10/xml-exc-c14n#"
			};
			Valid = null;
			ValidTooltip = HashingHelper.ToolTipNone;
		}

		protected virtual void FileChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_xmlFile.IsValid != null && XMLFile.IsValid.HasValue && _xmlFile.IsValid.Value) {
				if (_xmlFile.HasContentValidation) {
					Valid = _xmlFile.ContentValid;
					if (Valid != null && Valid.Value) {
						ValidTooltip = HashingHelper.ToolTipOk;
					} else {
						ValidTooltip = HashingHelper.ToolTipInvalidFileType;
					}
				} else {
					ValidTooltip = HashingHelper.ToolTipOk;
				}
			} else {
				Valid = false;
				ValidTooltip = HashingHelper.ToolTipXMLValidationFailed;
			}

			if (Valid != null && Valid.Value && _validateHashes != null) {
				_validateHashes(_xmlFile.Document, this);
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

		public HashedXMLFile(IOService ioService, string name, Func<XmlDocument, Collection<string>, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, contentCheck, hashValidation) {}

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

		public ReportXMLFile(IOService ioService, string name, Func<XmlDocument, Collection<string>, bool?> contentCheck,
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

	public class VectoJobFile : VectoXMLFile
	{
		private bool _componentDataValid;
		private string _jobValidToolTip;


		public VectoJobFile(string name, Func<XmlDocument, Collection<string>, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, contentCheck, hashValidation)
		{
			_xmlFile.PropertyChanged += JobFilechanged;
			Components = new ObservableCollection<ComponentEntry>();
		}

		public ObservableCollection<ComponentEntry> Components { get; private set; }

		public bool JobDataValid
		{
			get { return _componentDataValid; }
			set {
				if (_componentDataValid == value) {
					return;
				}
				_componentDataValid = value;
				JobValidToolTip = value ? HashingHelper.ToolTipComponentHashInvalid : HashingHelper.ToolTipOk;
				RaisePropertyChanged("JobDataValid");
			}
		}

		public string JobValidToolTip
		{
			get { return _jobValidToolTip; }
			set {
				if (_jobValidToolTip == value) {
					return;
				}
				_jobValidToolTip = value;
				RaisePropertyChanged("JobValidToolTip");
			}
		}

		private void JobFilechanged(object sender, PropertyChangedEventArgs e)
		{
			DoValidateHash();
		}

		private void DoValidateHash()
		{
			if (_xmlFile.Document == null || _xmlFile.ContentValid == null || !_xmlFile.ContentValid.Value) {
				Components.Clear();
				DigestValueComputed = "";
				JobDataValid = false;
				return;
			}
			try {
				Components.Clear();
				_xmlFile.XMLValidationErrors.Clear();
				var h = VectoHash.Load(_xmlFile.Document);
				var allValid = true;
				var components = h.GetContainigComponents().GroupBy(s => s)
					.Select(g => new { Entry = g.Key, Count = g.Count() });
				foreach (var component in components) {
					if (component.Entry == VectoComponents.Vehicle) {
						continue;
					}
					for (var i = 0; i < component.Count; i++) {
						var entry = new ComponentEntry();
						entry.Component = component.Count == 1
							? component.Entry.XMLElementName()
							: string.Format("{0} ({1})", component.Entry.XMLElementName(), i + 1);
						entry.Valid = h.ValidateHash(component.Entry, i);
						entry.CanonicalizationMethod = new[] {
							"urn:vecto:xml:2017:canonicalization",
							"http://www.w3.org/2001/10/xml-exc-c14n#"
						};
						entry.DigestValueRead = h.ReadHash(component.Entry, i);
						entry.DigestValueComputed = h.ComputeHash(component.Entry, i);
						if (!entry.Valid) {
							_xmlFile.XMLValidationErrors.Add(
								string.Format("Digest Value mismatch for component \"{0}\". Read digest value: \"{1}\", computed digest value \"{2}\"",
									entry.Component, entry.DigestValueRead, entry.DigestValueComputed));
						}
						Components.Add(entry);
						allValid &= entry.Valid;
					}
				}

				DigestValueComputed = h.ComputeHash();
				JobDataValid = allValid;
			} catch (Exception e) {
				DigestValueComputed = "";
				JobDataValid = false;
				_xmlFile.XMLValidationErrors.Add(e.Message);
			}
		}
	}
}
