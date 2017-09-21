using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel.UserControl
{
	public class ManufacturerReportXMLFile : ReportXMLFile
	{
		private ViewModel.ComponentEntry[] _jobComponents;

		private readonly ObservableCollection<string> _validationErrors = new ObservableCollection<string>();

		public ManufacturerReportXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, contentCheck, hashValidation)
		{
			_xmlFile.PropertyChanged += UpdateComponents;
		}

		public ViewModel.ComponentEntry[] JobComponents
		{
			set {
				if (_jobComponents == value) {
					return;
				}
				_jobComponents = value;
				DoUpdateComponents();
				RaisePropertyChanged("JobComponents");
				RaisePropertyChanged("ManufacturerReportValid");
			}
			private get { return _jobComponents; }
		}

		public ObservableCollection<string> ValidationErrors
		{
			get { return _validationErrors; }
		}

		private void UpdateComponents(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "UPDATED") {
				return;
			}
			DoUpdateComponents();
			RaisePropertyChanged("ManufacturerReportValid");
			RaisePropertyChanged("UPDATED");
		}

		private void DoUpdateComponents()
		{
			if (_xmlFile.Document == null || _xmlFile.Document.DocumentElement == null) {
				Components = new ComponentEntry[] { };
				VehicleIdentificationNumber = "";
				RaisePropertyChanged("Components");
				RaisePropertyChanged("VehicleIdentificationNumber");
				return;
			}
			var components = GetContainigComponents().GroupBy(s => s)
				.Select(g => new { Entry = g.Key, Count = g.Count() });
			var componentData = new List<ComponentEntry>();
			foreach (var component in components) {
				if (component.Entry == XMLNames.Component_Vehicle) {
					continue;
				}
				for (var i = 0; i < component.Count; i++) {
					var node = GetNodes(component.Entry, i);
					var entry = new ComponentEntry {
						Component = component.Count == 1
							? component.Entry
							: string.Format("{0} ({1})", component.Entry, i + 1),
						DigestValue = ReadElementValue(node, XMLNames.DI_Signature_Reference_DigestValue),
						CertificationMethod = ReadElementValue(node, XMLNames.Report_Component_CertificationMethod),
					};
					if (entry.Component.StartsWith("Axle ")) {
						entry.Component = entry.Component.Replace("Axle", "Tyre");
						entry.CertificationNumber = ReadElementValue(node, XMLNames.Report_Tyre_TyreCertificationNumber);
						entry.DigestValue = "Not Available";
					} else {
						entry.CertificationNumber = ReadElementValue(node, XMLNames.Report_Component_CertificationNumber) ??
													ReadElementValue(node, XMLNames.Report_Component_CertificationMethod);
					}
					if (JobComponents != null) {
						var jobComponent = JobComponents.Where(
							x => x.Component == entry.Component).ToArray();
						if (jobComponent.Any()) {
							entry.DigestValueMatchesJobComponent = entry.Component.StartsWith("Tyre ")
								? (bool?)null
								: (jobComponent.First().DigestValueRead == entry.DigestValue);
							entry.DigestValueExpected = jobComponent.First().DigestValueRead;
							if (entry.CertificationMethod != CertificationMethod.StandardValues.ToXMLFormat()) {
								entry.CertificationNumberMatchesJobComponent = jobComponent.First().CertificationNumber ==
																				entry.CertificationNumber;
								entry.CertificationNumberExpected = jobComponent.First().CertificationNumber;
							}
						}
					}
					componentData.Add(entry);
				}
			}
			Components = componentData.ToArray();
			VehicleIdentificationNumber = GetVehicleIdentificationNumber();

			RaisePropertyChanged("Components");
			RaisePropertyChanged("VehicleIdentificationNumber");
		}

		private string GetVehicleIdentificationNumber()
		{
			if (_xmlFile.Document == null || _xmlFile.IsValid != XmlFileStatus.ValidXML || _xmlFile.ContentValid == null ||
				!_xmlFile.ContentValid.Value) {
				return "";
			}
			var node = _xmlFile.Document.SelectSingleNode(string.Format("//*[local-name()='{0}']", XMLNames.Vehicle_VIN));
			if (node == null) {
				return "";
			}
			return node.InnerText;
		}

		public bool ManufacturerReportValid
		{
			get {
				_validationErrors.Clear();
				var componentsValid = JobComponents != null && JobComponents.Length > 0;
				if (Components == null || JobComponents == null || JobComponents.Length == 0) {
					return false;
				}

				foreach (var entry in Components) {
					// certification number is optional (iff standard values are used)
					var entryCertificationNbr = entry.CertificationNumberMatchesJobComponent == null ||
												entry.CertificationNumberMatchesJobComponent.Value;
					if (!entryCertificationNbr) {
						var msg =
							string.Format(
								"Verifying Manufacturer Report: Certification number for component '{0}' does not match! Job-File: '{1}', Report: '{2}'",
								entry.Component, entry.CertificationNumberExpected, entry.CertificationNumber);
						_validationErrors.Add(msg);
					}
					componentsValid &= entryCertificationNbr;
					// digest value is mandatory (except for tires)
					if (entry.Component.StartsWith("Tyre ")) {
						continue;
					}

					var entryDigest = entry.DigestValueMatchesJobComponent != null && entry.DigestValueMatchesJobComponent.Value;
					if (!entryDigest) {
						var msg =
							string.Format(
								"Verifying Manufacturer Report: Digest value for component '{0}' does not match! Job-File: '{1}', Report: '{2}'",
								entry.Component, entry.DigestValueExpected, entry.DigestValue);
						_validationErrors.Add(msg);
					}
					componentsValid &= entryDigest;
				}
				return JobDigestValid && componentsValid;
			}
		}

		private string ReadElementValue(XmlNode xmlNode, string elementName)
		{
			var node = xmlNode.SelectSingleNode(string.Format("./*[local-name()='{0}']", elementName));
			if (node == null) {
				return null;
			}
			return node.InnerText;
		}

		protected XmlNode GetNodes(string component, int index)
		{
			var nodes = _xmlFile.Document.SelectNodes(GetComponentQueryString(component));
			if (nodes == null || nodes.Count == 0) {
				throw new Exception(component == null
					? "No component found"
					: string.Format("Component {0} not found", component));
			}
			if (index >= nodes.Count) {
				throw new Exception(string.Format("index exceeds number of components found! index: {0}, #components: {1}", index,
					nodes.Count));
			}
			return nodes[index];
		}

		protected static string GetComponentQueryString(string component = null)
		{
			if (component == null) {
				return "(//*[@id])[1]";
			}
			return string.Format("//*[local-name()='{0}']", component);
		}

		protected IList<string> GetContainigComponents()
		{
			var retVal = new List<string>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				var nodes = _xmlFile.Document.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']",
					XMLNames.VectoManufacturerReport, component.XMLElementName()));
				var count = nodes == null ? 0 : nodes.Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(component.XMLElementName());
				}
			}
			foreach (var component in new[] { XMLNames.AxleWheels_Axles_Axle }) {
				var nodes = _xmlFile.Document.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']",
					XMLNames.VectoManufacturerReport, component));
				var count = nodes == null ? 0 : nodes.Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			return retVal;
		}

		public string VehicleIdentificationNumber { get; private set; }

		public ComponentEntry[] Components { get; private set; }

		public class ComponentEntry
		{
			public string Component { get; set; }

			public string CertificationNumber { get; set; }

			public string DigestValue { get; set; }

			public string CertificationMethod { get; set; }

			public bool? DigestValueMatchesJobComponent { get; set; }
			public bool? CertificationNumberMatchesJobComponent { get; set; }
			public string DigestValueExpected { get; set; }
			public string CertificationNumberExpected { get; set; }
		}
	}
}
