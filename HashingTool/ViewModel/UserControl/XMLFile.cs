using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
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

		public XMLFile(IOService ioservice, bool validate = false)
		{
			_ioService = ioservice;
			_validate = validate;
			XMLValidationErrors = new ObservableCollection<string>();
			Source = "";
			RaisePropertyChanged("ValidateInput");
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
			get { return new RelayCommand(ReadXMLFile, () => !_busy); }
		}


		private async void ReadXMLFile()
		{
			string filename;

			var stream = _ioService.OpenFileDialog(null, ".xml", "VECTO XML file|*.xml", out filename);
			if (stream == null) {
				return;
			}

			_busy = true;
			IsValid = null;
			XMLValidationErrors.Clear();
			Source = filename;

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
			Document = document;
			_busy = false;
		}

		private async void Validate(XmlReader xml)
		{
			try {
				IsValid = true;
				var validator = new XMLValidator(r => { IsValid = r; },
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
				IsValid = false;
				XMLValidationErrors.Add(e.Message);
			}
		}
	}
}
