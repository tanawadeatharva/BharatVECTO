using System;
using System.Xml;
using Castle.Core.Internal;

namespace VECTO3GUI2020.Helper
{
	public static class XmlHelper
	{
		public static XmlDocument ReadXmlDocument(string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return null;

			var xmlDocument = new XmlDocument();
			
			using (var reader = new XmlTextReader(filePath)) {
				xmlDocument.Load(reader);	
			}

			return xmlDocument;
		}


		public static XmlNodeList GetComponentNodes(XmlDocument xmlDocument, string parentNode, string nodeName)
		{
			if (xmlDocument == null || parentNode.IsNullOrEmpty() || nodeName.IsNullOrEmpty())
				return null;

			return xmlDocument.SelectNodes($"//*[local-name()='{parentNode}']//*[local-name()='{nodeName}']");
		}

		//public static bool ValidateXDocument(XDocument xDocument, Action<bool> resultAction = null,
		//	Action<XmlSeverityType, ValidationEvent> validationErrorAction = null)
		//{
		//	var xmlDocument = xDocument.ToXmlDocument();
		//	if (xmlDocument == null)
		//		return false;

		//	var documentType = XMLHelper.GetDocumentType(xmlDocument.DocumentElement.LocalName);
		//	if (documentType == null)
		//	{
		//		throw new VectoException("unknown xml file! {0}", xmlDocument.DocumentElement.LocalName);
		//	}

		//	var validator = new XMLValidator(xmlDocument, resultAction, validationErrorAction);
		//	return validator.ValidateXML(documentType.Value); ;
		//}

		public static string GetXmlAbsoluteFilePath(string baseUri)
		{
			if (baseUri == null)
				return null;

			return Uri.UnescapeDataString(new Uri(baseUri).AbsolutePath);
		}
	}
}
