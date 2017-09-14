using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using HashingTool.Helper;
using HashingTool.Util;

namespace HashingTool.ViewModel.UserControl
{
	public enum XmlFileStatus
	{
		Unknown, // no file selected,				outline
		Invalid, // reading failed, no xml, etc.		red
		InvalidXML, // does not validate against known schemas
		IncorrectContent, // content validation failed			
		ValidXML //									green
	}

	public class XMLFile : ObservableObject
	{
		private string _source;
		private XmlFileStatus _isValid;

		private bool _busy;

		private readonly bool _validate;
		private XmlDocument _document;
		private readonly Func<XmlDocument, Collection<string>, bool?> _postVerification;
		private bool? _contentValid;
		private RelayCommand _browseFileCommand;

		public XMLFile(IOService ioservice, bool validate = false,
			Func<XmlDocument, Collection<string>, bool?> contentCheck = null)
		{
			IoService = ioservice;
			_validate = validate;
			_browseFileCommand = new RelayCommand(BrowseXMLFile, () => !_busy);
			XMLValidationErrors = new ObservableCollection<string>();
			HasContentValidation = contentCheck != null;
			_postVerification = contentCheck ?? ((x, c) => null);
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

		public XmlFileStatus IsValid
		{
			get { return _isValid; }
			private set {
				if (_isValid == value) {
					return;
				}
				_isValid = value;
				RaisePropertyChanged("IsValid");
				SetToolTip(value);
			}
		}

		private void SetToolTip(XmlFileStatus value)
		{
			var toolTip = "";
			switch (value) {
				case XmlFileStatus.Unknown:
					toolTip = "Select XML File file";
					break;
				case XmlFileStatus.Invalid:
					toolTip = "Invalid file";
					break;
				case XmlFileStatus.InvalidXML:
					toolTip = "XML Schema validation failed";
					break;
				case XmlFileStatus.IncorrectContent:
					toolTip = "Incorrect XML content";
					break;
				case XmlFileStatus.ValidXML:
					toolTip = "OK";
					break;
			}
			if (ToolTip == toolTip) {
				return;
			}
			ToolTip = toolTip;
			RaisePropertyChanged("ToolTip");
		}

		public string ToolTip { get; internal set; }

		public ObservableCollection<string> XMLValidationErrors { get; set; }

		public ICommand BrowseFileCommand
		{
			get { return _browseFileCommand; }
		}

		public ICommand SetXMLFileCommnd
		{
			get { return new RelayCommand<string>(SetXMLFile, f => !_busy); }
		}

		private async void SetXMLFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				Document = null;
				XMLValidationErrors.Clear();
				ContentValid = null;
				IsValid = XmlFileStatus.Unknown;
				return;
			}
			using (var stream = File.OpenRead(fileName)) {
				await LoadXMLFile(stream);
			}
		}


		private async void BrowseXMLFile()
		{
			string filename;

			using (var stream = IoService.OpenFileDialog(null, ".xml", "VECTO XML file|*.xml", out filename)) {
				if (stream == null) {
					return;
				}

				await LoadXMLFile(stream);
				Source = filename;
			}
		}

		private async Task LoadXMLFile(Stream stream)
		{
			_busy = true;
			IsValid = XmlFileStatus.Unknown;
			ContentValid = null;
			XMLValidationErrors.Clear();
			var fileValid = XmlFileStatus.ValidXML;
			bool? contentValid = null;
			try {
				var ms = new MemoryStream();
				if (_validate) {
					// copy stream beforehand if validation is needed later on
					await stream.CopyToAsync(ms);
					ms.Seek(0, SeekOrigin.Begin);
					stream.Seek(0, SeekOrigin.Begin);
				}

				var document = new XmlDocument();
				var reader = XmlReader.Create(stream);
				document.Load(reader);
				Document = document;
				var xmlValid = true;
				if (_validate) {
					xmlValid = await Validate(XmlReader.Create(ms));
					if (!xmlValid) {
						fileValid = XmlFileStatus.InvalidXML;
					}
				}
				if (HasContentValidation) {
					contentValid = _postVerification(document, XMLValidationErrors);
					if (xmlValid && (contentValid == null || !contentValid.Value)) {
						fileValid = XmlFileStatus.IncorrectContent;
					}
				}
			} catch (Exception e) {
				XMLValidationErrors.Add(e.Message);
				fileValid = XmlFileStatus.Invalid;
			} finally {
				
				IsValid = fileValid;
				ContentValid = contentValid;
				_busy = false;
				_browseFileCommand.RaiseCanExecuteChanged();
			}
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

		private async Task<bool> Validate(XmlReader xml)
		{
			var valid = true;
			try {
				var validator = new XMLValidator(r => { valid = r; },
					(s, e) => {
						Application.Current.Dispatcher.Invoke(
							() =>
								XMLValidationErrors.Add(string.Format("Validation {0} Line {2}: {1}",
									s == XmlSeverityType.Warning ? "WARNING" : "ERROR",
									e.ValidationEventArgs == null
										? e.Exception.Message +
										(e.Exception.InnerException != null ? Environment.NewLine + e.Exception.InnerException.Message : "")
										: e.ValidationEventArgs.Message,
									e.ValidationEventArgs == null ? 0 : e.ValidationEventArgs.Exception.LineNumber)));
					});
				await validator.ValidateXML(xml);
			} catch (Exception e) {
				XMLValidationErrors.Add(e.Message);
			}
			return valid;
		}
	}
}
