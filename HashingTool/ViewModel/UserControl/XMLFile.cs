using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using HashingTool.Helper;
using HashingTool.Util;

namespace HashingTool.ViewModel.UserControl
{
	public class XMLFile : ObservableObject
	{
		private string _source;
		private bool? _isValid;

		private bool _busy;

		private readonly bool _validate;
		private XmlDocument _document;
		private readonly Func<XmlDocument, bool?> _postVerification;
		private bool? _contentValid;
		private bool _hasContentValidation;

		public XMLFile(IOService ioservice, bool validate = false, Func<XmlDocument, bool?> contentCheck = null)
		{
			IoService = ioservice;
			_validate = validate;
			XMLValidationErrors = new ObservableCollection<string>();
			HasContentValidation = contentCheck != null;
			_postVerification = contentCheck ?? (x => null);
			Source = "";
			RaisePropertyChanged("ValidateInput");
			RaisePropertyChanged("HasContentValidation");
		}

		public XmlDocument Document
		{
			get { return _document; }
			private set {
				if (_document == value) {
					return;
				}
				_document = value;
				RaisePropertyChanged("Document");
			}
		}

		public string Source
		{
			get { return _source; }
			private set {
				if (_source == value) {
					return;
				}
				SetXMLFile(value);
				_source = value;
				RaisePropertyChanged("Source");
			}
		}

		public bool ValidateInput
		{
			get { return _validate; }
		}

		public bool? IsValid
		{
			get { return _isValid; }
			private set {
				if (_isValid == value) {
					return;
				}
				_isValid = value;
				RaisePropertyChanged("IsValid");
			}
		}

		public ObservableCollection<string> XMLValidationErrors { get; set; }

		public ICommand BrowseFileCommand
		{
			get { return new RelayCommand(BrowseXMLFile, () => !_busy); }
		}

		public ICommand SetXMLFileCommnd
		{
			get {  return new RelayCommand<string>(SetXMLFile, (f)=> !_busy);}
		}
		
		private async void SetXMLFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				Document = null;
				XMLValidationErrors.Clear();
				ContentValid = null;
				IsValid = null;
				return;
			}
			var stream = File.OpenRead(fileName);

			await LoadXMLFile(stream);
		}


		private async void BrowseXMLFile()
		{
			string filename;

			var stream = IoService.OpenFileDialog(null, ".xml", "VECTO XML file|*.xml", out filename);
			if (stream == null) {
				return;
			}

			await LoadXMLFile(stream);
			Source = filename;
		}

		private async Task LoadXMLFile(Stream stream)
		{
			_busy = true;
			IsValid = null;
			ContentValid = null;
			XMLValidationErrors.Clear();
			

			if (_validate) {
				var ms = new MemoryStream();
				await stream.CopyToAsync(ms);
				ms.Seek(0, SeekOrigin.Begin);
				stream.Seek(0, SeekOrigin.Begin);
				Validate(XmlReader.Create(ms));
			}
			var document = new XmlDocument();
			var reader = XmlReader.Create(stream);
			document.Load(reader);
			ContentValid = _postVerification(document);
			Document = document;
			_busy = false;
		}

		public bool HasContentValidation { get; private set; }

		public bool? ContentValid
		{
			get { return _contentValid; }
			set {
				if (_contentValid == value) {
					return;
				}
				_contentValid = value;
				RaisePropertyChanged("ContentValid");
			}
		}

		private async void Validate(XmlReader xml)
		{
			var valid = true;
			try {

				var validator = new XMLValidator(r => { valid = r; },
					(s, e) => {
						Application.Current.Dispatcher.Invoke(() => XMLValidationErrors.Add(
							string.Format("Validation {0} Line {2}: {1}", s == XmlSeverityType.Warning ? "WARNING" : "ERROR",
								e.ValidationEventArgs == null
									? e.Exception.Message +
									(e.Exception.InnerException != null ? Environment.NewLine + e.Exception.InnerException.Message : "")
									: e.ValidationEventArgs.Message,
								e.ValidationEventArgs == null ? 0 : e.ValidationEventArgs.Exception.LineNumber)));
					});
				await validator.ValidateXML(xml);
			} catch (Exception e) {
				XMLValidationErrors.Add(e.Message);
			} finally {
				IsValid = valid;
			}
		}
	}
}
