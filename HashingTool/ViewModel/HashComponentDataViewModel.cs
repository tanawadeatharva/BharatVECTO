using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using HashingTool.Helper;
using HashingTool.Util;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;
using TUGraz.VectoHashing.Impl;

namespace HashingTool.ViewModel
{
	public class HashComponentDataViewModel : VectoXMLFile, IMainView
	{
		private string _digestValue;

		private XDocument _result;

		private readonly RelayCommand _saveCommand;

		private bool? _componentDataValid;

		private bool _busy;

		public HashComponentDataViewModel()
			: base("Hash Component Data", false, HashingHelper.IsComponentFile)
		{
			_xmlFile.PropertyChanged += SourceChanged;
			_saveCommand = new RelayCommand(SaveDocument,
				() => !_busy && ComponentDataValid != null && ComponentDataValid.Value && _result != null);
		}


		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

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

		private void SourceChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "UPDATED") {
				DoComputeHash();
			}
		}

		private void SaveDocument()
		{
			string filename;
			var stream = IoService.SaveData(null, ".xml", "VECTO XML file|*.xml", out filename);
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

		private async void DoComputeHash()
		{
			if (_busy) {
				return;
			}
			if (_xmlFile.Document == null || _xmlFile.IsValid != XmlFileStatus.ValidXML) {
				ComponentDataValid = false;
				DigestValue = "";
				DigestMethod = "";
				SetCanonicalizationMethod(new string[] { });
				return;
			}

			try {
				_busy = true;
				ComponentDataValid = false;
				DigestValue = "";
				_xmlFile.XMLValidationErrors.Clear();
				SetCanonicalizationMethod(new string[] { });

				var h = VectoHash.Load(_xmlFile.Document);

				_result = h.AddHash();

				// validate generated component file
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
							Application.Current.Dispatcher.Invoke(() => _xmlFile.LogError(
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
					//var c14N = XMLHashProvider.DefaultCanonicalizationMethod.ToArray();
					//var digestMethod = XMLHashProvider.DefaultDigestMethod;
					//DigestValue = h.ComputeHash(c14N, digestMethod);
					using (MemoryStream ms = new MemoryStream()) {
						using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true })) {
							_result.WriteTo(xw);
							xw.Flush();
						}
						ms.Flush();
						ms.Seek(0, SeekOrigin.Begin);
						var h2 = VectoHash.Load(ms);
						DigestMethod = h2.GetDigestMethod();
						DigestValue = h2.ReadHash();
						SetCanonicalizationMethod(h2.GetCanonicalizationMethods());
					}
				}
			} catch (Exception e) {
				ComponentDataValid = false;
				DigestValue = "";
				_xmlFile.LogError(e.Message);
				SetCanonicalizationMethod(new string[] { });
				DigestMethod = "";
			} finally {
				_busy = false;
				_saveCommand.RaiseCanExecuteChanged();
			}
		}
	}
}
