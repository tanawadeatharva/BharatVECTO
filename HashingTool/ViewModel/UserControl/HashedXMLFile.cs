using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

namespace HashingTool.ViewModel.UserControl
{
	public class HashedXMLFile : VectoXMLFile
	{
		protected string _digestValueRead;
		private DateTime? _date;

		public HashedXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, true, contentCheck, hashValidation) {}

		public string DigestValueRead
		{
			get { return _digestValueRead; }
			internal set {
				if (_digestValueRead == value) {
					return;
				}
				_digestValueRead = value;
				RaisePropertyChanged("DigestValueRead");
			}
		}

		protected override void FileChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FileChanged(sender, e);
			if (e.PropertyName != GeneralUpdate) {
				return;
			}

			if (_xmlFile.IsValid == XmlFileStatus.ValidXML && _validateHashes != null) {} else {
				DigestValueRead = "";
				DigestValueComputed = "";
				Date = null;
				SetCanonicalizationMethod(new string[] { });
				DigestMethod = "";
				Component = "";
			}
			RaisePropertyChanged(GeneralUpdate);
		}

		public DateTime? Date
		{
			get { return _date; }
			internal set {
				if (_date == value) {
					return;
				}
				_date = value;
				RaisePropertyChanged("Date");
			}
		}
	}
}
