using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using HashingTool.Helper;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class HashComponentDataViewModel : ObservableObject, IMainView
	{
		private readonly ApplicationViewModel _applicationViewModel;
		private bool? _componentDataValid;
		private string _digestValue;
		//private ObservableCollection<string> _xmlValidationErrors = new ObservableCollection<string>();
		private Stream _stream;
		private XDocument _result;

		private IOService _ioService = new WPFIoService();
		private RelayCommand _saveCommand;
		private bool _busy;
		private string _source;

		public HashComponentDataViewModel()
		{
			XMLValidationErrors = new ObservableCollection<string>();
			_saveCommand = new RelayCommand(SaveDocument,
				() => !_busy && ComponentDataValid != null && ComponentDataValid.Value && _result != null);
			_busy = false;
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

		public ObservableCollection<string> XMLValidationErrors { get; set; }
		//{
		//	get { return _xmlValidationErrors; }
		//	private set {
		//		_xmlValidationErrors = value;
		//		RaisePropertyChanged("XMLValidationErrors");
		//	}
		//}

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

		public ICommand SetComponentData
		{
			get { return new RelayCommand(HashComponentData, () => !_busy); }
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

		private void HashComponentData()
		{
			string filename;

			var xml = _ioService.OpenFileDialog(null, ".xml", "VECTO XML file|*.xml", out filename);
			if (xml == null) {
				return;
			}

			_busy = true;
			ComponentDataValid = null;
			XMLValidationErrors.Clear();
			DigestValue = "";
			Source = filename;
			_stream = xml;
			DoComputeHash();
		}

		private async void DoComputeHash()
		{
			try {
				var h = VectoHash.Load(_stream);

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
					await validator.ValidateXML(ms);
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
