using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace HashingTool.ViewModel.UserControl
{
	public class HashedXMLFile : VectoXMLFile
	{
		protected string _digestValueRead;

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
	}
}
