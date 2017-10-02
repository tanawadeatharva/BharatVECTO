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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using HashingTool.Helper;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel.UserControl
{
	public class VectoJobFile : VectoXMLFile
	{
		private bool? _componentDataValid;
		private string _jobValidToolTip;
		private string _vin;
		private DateTime? _jobDate;


		public VectoJobFile(string name, Func<XmlDocument, IErrorLogger, bool?> contentCheck,
			Action<XmlDocument, VectoXMLFile> hashValidation = null) : base(name, true, contentCheck, hashValidation)
		{
			_xmlFile.PropertyChanged += JobFilechanged;
			Components = new ObservableCollection<ComponentEntry>();
			JobDataValid = null;
		}

		public ObservableCollection<ComponentEntry> Components { get; private set; }

		public bool? JobDataValid
		{
			get { return _componentDataValid; }
			set {
				if (_componentDataValid == value) {
					return;
				}
				_componentDataValid = value;
				JobValidToolTip = value != null && !value.Value
					? HashingHelper.ToolTipComponentHashInvalid
					: HashingHelper.ToolTipOk;
				RaisePropertyChanged("JobDataValid");
			}
		}

		public string JobValidToolTip
		{
			get { return _jobValidToolTip; }
			set {
				if (_jobValidToolTip == value) {
					return;
				}
				_jobValidToolTip = value;
				RaisePropertyChanged("JobValidToolTip");
			}
		}

		public string VehicleIdentificationNumber
		{
			get { return _vin; }
			set {
				if (_vin == value) {
					return;
				}
				_vin = value;
				RaisePropertyChanged("VehicleIdentificationNumber");
			}
		}

		public DateTime? JobCreationDate
		{
			get { return _jobDate; }
			set {
				if (_jobDate == value) {
					return;
				}
				_jobDate = value;
				RaisePropertyChanged("JobCreationDate");
			}
		}

		private void JobFilechanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != GeneralUpdate) {
				return;
			}
			DoValidateHash();
			VehicleIdentificationNumber = DoReadVIN();
			JobCreationDate = DoReadJobDate();

			RaisePropertyChanged(GeneralUpdate);
		}

		private DateTime? DoReadJobDate()
		{
			if (_xmlFile.Document == null || _xmlFile.IsValid != XmlFileStatus.ValidXML || _xmlFile.ContentValid == null ||
				!_xmlFile.ContentValid.Value) {
				return null;
			}
			var nodes = _xmlFile.Document.SelectNodes(string.Format("//*[local-name()='{0}']", XMLNames.Component_Date));
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			return XmlConvert.ToDateTime(nodes[0].InnerText, XmlDateTimeSerializationMode.Local);
		}

		private string DoReadVIN()
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

		private void DoValidateHash()
		{
			if (_xmlFile.Document == null || _xmlFile.IsValid != XmlFileStatus.ValidXML || _xmlFile.ContentValid == null ||
				!_xmlFile.ContentValid.Value) {
				Components.Clear();
				DigestValueComputed = "";
				DigestMethod = "";
				SetCanonicalizationMethod(new string[] { });
				JobDataValid = null;
				return;
			}
			try {
				Components.Clear();
				_xmlFile.XMLValidationErrors.Clear();
				var h = VectoHash.Load(_xmlFile.Document);
				var allValid = true;
				var components = h.GetContainigComponents().GroupBy(s => s)
					.Select(g => new { Entry = g.Key, Count = g.Count() });
				foreach (var component in components) {
					if (component.Entry == VectoComponents.Vehicle) {
						continue;
					}
					for (var i = 0; i < component.Count; i++) {
						var entry = new ComponentEntry();
						entry.Component = component.Count == 1
							? component.Entry.XMLElementName()
							: string.Format("{0} ({1})", component.Entry.XMLElementName(), i + 1);
						entry.Valid = h.ValidateHash(component.Entry, i);
						entry.CanonicalizationMethod = h.GetCanonicalizationMethods(component.Entry, i).ToArray();
						entry.DigestMethod = h.GetDigestMethod(component.Entry, i);
						entry.DigestValueRead = h.ReadHash(component.Entry, i);
						entry.DigestValueComputed = h.ComputeHash(component.Entry, i);
						entry.CertificationNumber = h.GetCertificationNumber(component.Entry, i);
						entry.CertificationDate = h.GetCertificationDate(component.Entry, i);
						if (!entry.Valid) {
							_xmlFile.LogError(
								string.Format(
									"Digest Value mismatch for component \"{0}\". Read digest value: \"{1}\", computed digest value \"{2}\"",
									entry.Component, entry.DigestValueRead, entry.DigestValueComputed));
						}
						Components.Add(entry);
						allValid &= entry.Valid;
					}
				}

				DigestValueComputed = h.ComputeHash();
				JobDataValid = allValid;
			} catch (Exception e) {
				DigestValueComputed = "";
				JobDataValid = false;
				_xmlFile.LogError(e.Message);
			}
		}
	}
}
