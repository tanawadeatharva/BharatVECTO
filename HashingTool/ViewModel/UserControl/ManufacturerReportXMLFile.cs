/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
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
		private bool _manufacturerReportValid;

		public ManufacturerReportXMLFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, contentCheck, hashValidation)
		{
			_xmlFile.PropertyChanged += UpdateComponents;
		}

		protected override void VerifyJobDataMatchesReport()
		{
			base.VerifyJobDataMatchesReport();

			DoUpdateComponentData();
		}

		private void UpdateComponents(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != GeneralUpdate) {
				return;
			}
			DoUpdateComponentData();
			RaisePropertyChanged(GeneralUpdate);
		}

		private void DoUpdateComponentData()
		{
			if (_xmlFile.Document == null || _xmlFile.Document.DocumentElement == null ||
				_xmlFile.IsValid != XmlFileStatus.ValidXML) {
				Components = new ComponentEntry[] { };
				RaisePropertyChanged("Components");
				return;
			}
			var components = GetContainigComponents().GroupBy(s => s)
				.Select(g => new { Entry = g.Key, Count = g.Count() });
			var jobComponents = _jobData == null ? new ViewModel.ComponentEntry[] { } : _jobData.Components.ToArray();
			_validationErrors.Clear();

			var hasComponentsFromJob = _jobData != null && _jobData.JobDataValid != null && _jobData.JobDataValid.Value &&
										jobComponents.Any();

			// iterate over components in manufacturer report, read out c14n, digest method, digest;
			// collect c14n, digest method, digest value read, certification nr., digest value from job (re-computed)
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
					// rename 'Axle' from report to 'Tyre' as in job
					if (entry.Component.StartsWith("Axle ")) {
						entry.Component = entry.Component.Replace("Axle", "Tyre");
						entry.CertificationNumber = ReadElementValue(node, XMLNames.Report_Tyre_TyreCertificationNumber);
					} else {
						entry.CertificationNumber = ReadElementValue(node, XMLNames.Report_Component_CertificationNumber) ??
													ReadElementValue(node, XMLNames.Report_Component_CertificationMethod);
					}
					componentData.Add(entry);
					if (!hasComponentsFromJob) {
						continue;
					}
					var jobComponent = jobComponents.Where(x => x.Component == entry.Component).ToArray();
					if (!jobComponent.Any()) {
						continue;
					}
					entry.DigestValueMatchesJobComponent = jobComponent.First().DigestValueComputed == entry.DigestValue;
					entry.DigestValueExpected = jobComponent.First().DigestValueComputed;

					if (entry.CertificationMethod == CertificationMethod.StandardValues.ToXMLFormat()) {
						continue;
					}
					entry.CertificationNumberMatchesJobComponent = jobComponent.First().CertificationNumber ==
																	entry.CertificationNumber;
					entry.CertificationNumberExpected = jobComponent.First().CertificationNumber;
				}
			}
			Components = componentData.ToArray();
			RaisePropertyChanged("Components");
			var certificationNumberMismatch =
				componentData.Where(
					x => x.CertificationNumberMatchesJobComponent != null && !x.CertificationNumberMatchesJobComponent.Value).ToArray();
			var digestMismatch = componentData.Where(x => x.DigestValueMatchesJobComponent == null || !x.DigestValueMatchesJobComponent.Value).ToArray();
			if (jobComponents.Any()) {
				foreach (var entry in certificationNumberMismatch) {
					_validationErrors.Add(
						string.Format(
							"Verifying Manufacturer Report: Certification number for component '{0}' does not match! Job-file: '{1}', Report: '{2}'",
							entry.Component, entry.CertificationNumberExpected, entry.CertificationNumber));
				}
				foreach (var entry in digestMismatch) {
					_validationErrors.Add(
						string.Format(
							"Verifying Manufacturer Report: Digest Value for component '{0}' does not match! Job-file: '{1}', Report: '{2}'",
							entry.Component, entry.DigestValueExpected, entry.DigestValue));
				}
			}

			ManufacturerReportValid = FileIntegrityValid != null && FileIntegrityValid.Value && hasComponentsFromJob && !certificationNumberMismatch.Any() && !digestMismatch.Any();
		}

		public bool ManufacturerReportValid
		{
			get { return _manufacturerReportValid; }
			set {
				if (_manufacturerReportValid == value) {
					return;
				}
				_manufacturerReportValid = value;
				RaisePropertyChanged("ManufacturerReportValid");
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
				var nodes = _xmlFile.Document.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']/*[local-name()='Model']",
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
