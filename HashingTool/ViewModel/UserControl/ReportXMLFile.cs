using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace HashingTool.ViewModel.UserControl
{
	public class ReportXMLFile : HashedXMLFile
	{
		private string _jobDigestValueReadRead;
		private string _jobDigestMethodRead;
		private string[] _jobCanonicalizationMethodRead;
		private string _jobDigestComputed;
		private bool _jobDigestValid;

		public ReportXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null)
			: base(name, contentCheck, hashValidation)
		{
			_xmlFile.PropertyChanged += ReadJobDigest;
		}

		private void ReadJobDigest(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "UPDATED") {
				return;
			}

			var jobDigest = "";
			var jobDigestMethod = "";
			var jobc14NMethod = new string[] { };

			if (_xmlFile.Document != null && _xmlFile.Document.DocumentElement != null) {
				var digestValueNode =
					_xmlFile.Document.SelectSingleNode("//*[local-name()='InputDataSignature']//*[local-name()='DigestValue']");
				if (digestValueNode != null) {
					jobDigest = digestValueNode.InnerText;
				}
				var digestMethodNode =
					_xmlFile.Document.SelectSingleNode(
						"//*[local-name()='InputDataSignature']//*[local-name()='DigestMethod']/@Algorithm");
				if (digestMethodNode != null) {
					jobDigestMethod = digestMethodNode.InnerText;
				}

				var c14NtMethodNodes =
					_xmlFile.Document.SelectNodes("//*[local-name()='InputDataSignature']//*[local-name()='Transform']/@Algorithm");
				if (c14NtMethodNodes != null) {
					jobc14NMethod = (from XmlNode node in c14NtMethodNodes select node.InnerText).ToArray();
				}
			}
			JobCanonicalizationMethodRead = jobc14NMethod;
			JobDigestMethodRead = jobDigestMethod;
			JobDigestValueRead = jobDigest;
			RaisePropertyChanged("UPDATED");
		}

		public string JobDigestMethodRead
		{
			get { return _jobDigestMethodRead; }
			set {
				if (_jobDigestMethodRead == value) {
					return;
				}
				_jobDigestMethodRead = value;
				RaisePropertyChanged("JobDigestMethodRead");
			}
		}

		public string[] JobCanonicalizationMethodRead
		{
			get { return _jobCanonicalizationMethodRead; }
			set {
				if (_jobCanonicalizationMethodRead == value) {
					return;
				}
				_jobCanonicalizationMethodRead = value;
				RaisePropertyChanged("JobCanonicalizationMethodRead");
			}
		}

		public string JobDigestValueRead
		{
			get { return _jobDigestValueReadRead; }
			internal set {
				if (_jobDigestValueReadRead == value) {
					return;
				}
				_jobDigestValueReadRead = value;
				RaisePropertyChanged("JobDigestValueRead");
			}
		}

		public string JobDigestValueComputed
		{
			get { return _jobDigestComputed; }
			set {
				if (_jobDigestComputed == value) {
					JobDigestValid = _jobDigestComputed == JobDigestValueRead;
					return;
				}
				_jobDigestComputed = value;
				RaisePropertyChanged("JobDigestValueComputed");
				JobDigestValid = _jobDigestComputed == JobDigestValueRead;
			}
		}

		public bool JobDigestValid
		{
			get { return _jobDigestValid; }
			set {
				if (_jobDigestValid == value) {
					return;
				}
				_jobDigestValid = value;
				RaisePropertyChanged("JobDigestValid");
			}
		}
	}
}
