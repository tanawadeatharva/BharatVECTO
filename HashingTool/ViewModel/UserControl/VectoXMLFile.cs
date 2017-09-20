using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using HashingTool.Helper;

namespace HashingTool.ViewModel.UserControl
{
	public class VectoXMLFile : ObservableObject
	{
		protected readonly XMLFileSelector _xmlFile;

		protected string _digestValueComputed;
		protected bool? _valid;
		protected string _name;
		protected string _tooltip;
		protected string _componentType;
		protected readonly Action<XmlDocument, VectoXMLFile> _validateHashes;
		private string _digestMethod;


		public VectoXMLFile(string name, bool validate, Func<XmlDocument, Collection<string>, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null)
		{
			_validateHashes = hashValidation;
			_xmlFile = new XMLFileSelector(IoService, validate, contentCheck);
			_xmlFile.PropertyChanged += FileChanged;
			Name = name;
			CanonicalizationMethods = new ObservableCollection<string>();

			Valid = null;
			ValidTooltip = HashingHelper.ToolTipNone;
		}

		protected virtual void FileChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "UPDATED") {
				return;
			}

			if (_xmlFile.IsValid == XmlFileStatus.ValidXML) {
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
			RaisePropertyChanged("UPDATED");
		}


		public XMLFileSelector XMLFile
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

		public ObservableCollection<string> CanonicalizationMethods { get; private set; }

		public void SetCanonicalizationMethod(IEnumerable<string> c14NMethods)
		{
			CanonicalizationMethods.Clear();
			foreach (var c14N in c14NMethods) {
				CanonicalizationMethods.Add(c14N);
			}
			RaisePropertyChanged("CanonicalizationMethods");
		}

		public string DigestMethod
		{
			get { return _digestMethod; }
			set {
				if (_digestMethod == value) {
					return;
				}
				_digestMethod = value;
				RaisePropertyChanged("DigestMethod");
			}
		}


		public string DigestValueComputed
		{
			get { return _digestValueComputed; }
			internal set {
				if (_digestValueComputed == value) {
					return;
				}
				_digestValueComputed = value;
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
}