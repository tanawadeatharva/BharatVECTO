using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HashingTool.ViewModel.UserControl
{
	public class VehicleInformationXMLFile : ReportXMLFile
	{
		bool _isValidVif;

		public VehicleInformationXMLFile(
				string name,
				Func<XmlDocument, IErrorLogger, bool?> contentCheck,
				TUGraz.VectoCore.Utils.XmlDocumentType xmlDocumentType,
				Action<XmlDocument, VectoXMLFile> hashValidation = null)
			: base(name, contentCheck, xmlDocumentType, hashValidation)
		{
			_xmlFile.PropertyChanged += ValidateVif;
		}
		
		private void ValidateVif(object sender, PropertyChangedEventArgs e)
		{
			IsValidVif = JobDigestMatchesReport && (DigestValueRead == DigestValueComputed);
		}

		public bool IsValidVif
		{
			get
			{
				return _isValidVif;
			}
			set
			{
				_isValidVif = value;
				RaisePropertyChanged("IsValidVif");
			}
		}

	}
}
