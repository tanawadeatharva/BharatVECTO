using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using HashingTool.Helper;
using HashingTool.Util;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class HashComponentDataViewModel : ObservableObject, IMainView
	{
		private readonly ApplicationViewModel _applicationViewModel;
		private string _digestValue;

		private XDocument _result;

		private readonly IOService _ioService = new WPFIoService();
		private readonly RelayCommand _saveCommand;
		private bool _busy;
		private XMLFile _sourceFile;
		private bool? _componentDataValid;

		public HashComponentDataViewModel()
		{
			XMLValidationErrors = new ObservableCollection<string>();
			_sourceFile = new XMLFile(_ioService, false);
			_sourceFile.PropertyChanged += SourceChanged;
			_saveCommand = new RelayCommand(SaveDocument,
				() => !_busy && ComponentDataValid != null && ComponentDataValid.Value && _result != null);
			_busy = false;

			CanonicalizaitionMethods = new ObservableCollection<string>() {
				"urn:vecto:xml:2017:canonicalization",
				"http://www.w3.org/2001/10/xml-exc-c14n#"
			};
		}

		private void SourceChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Document") {
				DoComputeHash();
			}
		}

		public HashComponentDataViewModel(ApplicationViewModel applicationViewModel) : this()
		{
			_applicationViewModel = applicationViewModel;
		}

		public string Name
		{
			get { return "Hash Component Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public XMLFile ComponentFile
		{
			get { return _sourceFile; }
			private set { _sourceFile = value; }
		}


		public ObservableCollection<string> XMLValidationErrors { get; set; }


		public string DigestValue
		{
			get { return _digestValue; }
			set {
				if (_digestValue == value) {
					return;
				}
				_digestValue = value;
				RaisePropertyChanged("DigestValue");
			}
		}

		public ICommand SaveHashedDocument
		{
			get { return _saveCommand; }
		}

		private void SaveDocument()
		{
			string filename;
			var stream = _ioService.SaveData(null, ".xml", "VECTO XML file|*.xml", out filename);
			if (stream == null) {
				return;
			}
			using (var writer = new XmlTextWriter(stream, Encoding.UTF8) {
				Formatting = Formatting.Indented,
				Indentation = 4
			}) {
				_result.WriteTo(writer);
				writer.Flush();
				writer.Close();
			}
		}

		public bool? ComponentDataValid
		{
			get { return _componentDataValid; }
			private set {
				if (_componentDataValid == value) {
					return;
				}
				_componentDataValid = value;
				RaisePropertyChanged("ComponentDataValid");
			}
		}

		public ObservableCollection<string> CanonicalizaitionMethods { get; private set; }

		private async void DoComputeHash()
		{
			try {
				_busy = true;
				ComponentDataValid = false;
				DigestValue = "";
				XMLValidationErrors.Clear();

				var h = VectoHash.Load(_sourceFile.Document);

				_result = h.AddHash();

				using (MemoryStream ms = new MemoryStream()) {
					using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true })) {
						_result.WriteTo(xw);
						xw.Flush();
					}
					ms.Flush();
					ms.Seek(0, SeekOrigin.Begin);
					ComponentDataValid = true;
					var validator = new XMLValidator(r => { ComponentDataValid = r; },
						(s, e) => {
							Application.Current.Dispatcher.Invoke(() => XMLValidationErrors.Add(
								string.Format("Validation {0} Line {2}: {1}", s == XmlSeverityType.Warning ? "WARNING" : "ERROR",
									e.ValidationEventArgs == null
										? e.Exception.Message +
										(e.Exception.InnerException != null ? Environment.NewLine + e.Exception.InnerException.Message : "")
										: e.ValidationEventArgs.Message,
									e.ValidationEventArgs == null ? 0 : e.ValidationEventArgs.Exception.LineNumber)));
						});
					await validator.ValidateXML(XmlReader.Create(ms));
				}
				if (ComponentDataValid != null && ComponentDataValid.Value) {
					DigestValue = h.ComputeHash();
				}
				_saveCommand.RaiseCanExecuteChanged();
			} catch (Exception e) {
				ComponentDataValid = false;
				DigestValue = "";
				XMLValidationErrors.Add(e.Message);
			} finally {
				_busy = false;
			}
		}
	}
}
