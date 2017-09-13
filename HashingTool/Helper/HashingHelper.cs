using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using HashingTool.ViewModel;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing;

namespace HashingTool.Helper
{
	public static class HashingHelper
	{
		public const string ToolTipInvalidFileType = "Invalid File type!";
		public const string ToolTipXMLValidationFailed = "XML validation failed!";
		public const string ToolTipOk = "Correct file selected";
		public const string ToolTipHashInvalid = "Incorrect digest value!";
		public const string ToolTipNone = "";
		public static string ToolTipComponentHashInvalid = "Job-Data validation failed!";

		public static bool? IsManufacturerReport(XmlDocument x, Collection<string> errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoManufacturerReport;
			if (!valid) {
				errorLog.Add(String.Format("Invalid XML file given ({0}). Expected Manufacturer Report XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoManufacturerReport));
			}
			return valid;
		}

		public static bool? IsCustomerReport(XmlDocument x, Collection<string> errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement != null && x.DocumentElement.LocalName == XMLNames.VectoCustomerReport;
			if (!valid) {
				errorLog.Add(String.Format("Invalid XML file given ({0}). Expected Customer Report XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoCustomerReport));
			}
			return valid;
		}

		public static bool? IsJobFile(XmlDocument x, Collection<string> errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoInputDeclaration &&
						x.DocumentElement.FirstChild.LocalName == XMLNames.Component_Vehicle;
			if (!valid) {
				errorLog.Add(String.Format("Invalid XML file given ({0}/{1}). Expected Vehicle XML ({2}/{3})!",
					x.DocumentElement.LocalName, x.DocumentElement.FirstChild.LocalName, XMLNames.VectoInputDeclaration,
					XMLNames.Component_Vehicle));
			}
			return valid;
		}

		public static bool? IsComponentFile(XmlDocument x, Collection<string> errorLog)
		{
			if (x.DocumentElement == null) {
				return null;
			}

			if (x.DocumentElement.LocalName != XMLNames.VectoInputDeclaration) {
				errorLog.Add(String.Format("Invalid XML file given ({0}). Expected Component XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoInputDeclaration));

				return false;
			}

			var localName = x.DocumentElement.FirstChild.LocalName;
			var components = new[] {
				VectoComponents.Engine, VectoComponents.Airdrag, VectoComponents.Angledrive, VectoComponents.Axlegear,
				VectoComponents.Gearbox, VectoComponents.Retarder, VectoComponents.TorqueConverter, VectoComponents.Tyre
			};
			var valid = components.Where(c => c.XMLElementName() == localName).Any();
			if (!valid) {
				errorLog.Add(String.Format("Invalid XML file given ({0}). Expected Component XML ({1})!",
					localName, String.Join(", ", components.Select(c => c.XMLElementName()))));
			}
			return valid;
		}


		public static void HashJobFile(XmlDocument xml, VectoXMLFile xmlViewModel)
		{
			try {
				var h = VectoHash.Load(xml);
				xmlViewModel.DigestValueComputed = h.ComputeHash();
			} catch (Exception e) {
				xmlViewModel.XMLFile.XMLValidationErrors.Add(e.Message);
				xmlViewModel.DigestValueComputed = "";
			}
		}

		public static void ValidateDocumentHash(XmlDocument xml, VectoXMLFile xmlViewModel)
		{
			var report = xmlViewModel as ReportXMLFile;
			if (report == null) {
				return;
			}
			try {
				var h = VectoHash.Load(xml);
				try {
					report.DigestValueRead = h.ReadHash();
				} catch {
					report.DigestValueRead = "";
				}
				try {
					report.DigestValueComputed = h.ComputeHash();
				} catch {
					report.DigestValueComputed = "";
				}
				var valid = h.ValidateHash();
				report.ValidTooltip = valid ? ToolTipOk : ToolTipHashInvalid;
				report.Valid = valid;
			} catch (Exception e) {
				report.XMLFile.XMLValidationErrors.Add(e.Message);
				report.Valid = false;
			}
		}
	}
}
