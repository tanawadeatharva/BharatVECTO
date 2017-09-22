using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using HashingTool.ViewModel;
using HashingTool.ViewModel.UserControl;
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
		public const string ToolTipHashValid = "File integrity verified";
		public const string ToolTipNone = "";
		public static string ToolTipComponentHashInvalid = "Job-Data validation failed!";

		public static bool? IsManufacturerReport(XmlDocument x, IErrorLogger errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoManufacturerReport;
			if (!valid) {
				errorLog.LogError(String.Format("Invalid XML file given ({0}). Expected Manufacturer Report XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoManufacturerReport));
			}
			return valid;
		}

		public static bool? IsCustomerReport(XmlDocument x, IErrorLogger errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement != null && x.DocumentElement.LocalName == XMLNames.VectoCustomerReport;
			if (!valid) {
				errorLog.LogError(String.Format("Invalid XML file given ({0}). Expected Customer Report XML ({1})!",
					x.DocumentElement.LocalName, XMLNames.VectoCustomerReport));
			}
			return valid;
		}

		public static bool? IsJobFile(XmlDocument x, IErrorLogger errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoInputDeclaration &&
						x.DocumentElement.FirstChild.LocalName == XMLNames.Component_Vehicle;
			if (!valid) {
				errorLog.LogError(String.Format("Invalid XML file given ({0}/{1}). Expected Vehicle XML ({2}/{3})!",
					x.DocumentElement.LocalName, x.DocumentElement.FirstChild.LocalName, XMLNames.VectoInputDeclaration,
					XMLNames.Component_Vehicle));
			}
			return valid;
		}

		public static bool? IsComponentFile(XmlDocument x, IErrorLogger errorLog)
		{
			if (x.DocumentElement == null) {
				return null;
			}

			if (x.DocumentElement.LocalName != XMLNames.VectoInputDeclaration) {
				errorLog.LogError(String.Format("Invalid XML file given ({0}). Expected Component XML ({1})!",
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
				errorLog.LogError(String.Format("Invalid XML file given ({0}). Expected Component XML ({1})!",
					localName, String.Join(", ", components.Select(c => c.XMLElementName()))));
			}
			return valid;
		}


		public static void HashJobFile(XmlDocument xml, VectoXMLFile xmlViewModel)
		{
			try {
				var h = VectoHash.Load(xml);
				xmlViewModel.DigestValueComputed = h.ComputeHash();
				xmlViewModel.DigestMethod = h.GetDigestMethod();
				xmlViewModel.SetCanonicalizationMethod(h.GetCanonicalizationMethods());
			} catch (Exception e) {
				xmlViewModel.XMLFile.LogError(e.Message);
				xmlViewModel.DigestValueComputed = "";
			}
		}

		public static void ValidateDocumentHash(XmlDocument xml, VectoXMLFile xmlFileModel)
		{
			var hashedXML = xmlFileModel as HashedXMLFile;
			if (hashedXML == null) {
				return;
			}

			try {
				var h = VectoHash.Load(xml);
				hashedXML.DigestMethod = h.GetDigestMethod();
				hashedXML.SetCanonicalizationMethod(h.GetCanonicalizationMethods());
				try {
					hashedXML.DigestValueRead = h.ReadHash();
					var dateNode = xml.SelectSingleNode("//*[local-name()='Date']");
					hashedXML.Date = dateNode != null
						? XmlConvert.ToDateTime(dateNode.InnerText, XmlDateTimeSerializationMode.RoundtripKind)
						: (DateTime?)null;
				} catch {
					hashedXML.DigestValueRead = "";
				}
				try {
					hashedXML.DigestValueComputed = h.ComputeHash();
				} catch {
					hashedXML.DigestValueComputed = "";
				}
				var valid = h.ValidateHash();
				hashedXML.FileIntegrityTooltip = valid ? ToolTipHashValid : ToolTipHashInvalid;
				hashedXML.FileIntegrityValid = valid;
			} catch (Exception e) {
				hashedXML.XMLFile.LogError(e.Message);
				hashedXML.FileIntegrityValid = false;
			}
		}
	}
}
