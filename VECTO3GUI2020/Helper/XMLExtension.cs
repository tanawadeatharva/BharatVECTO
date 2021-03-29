using System;
using System.Diagnostics;
using System.Xml.Linq;
using Castle.Core.Resource;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing.Impl;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.Util.XML.Interfaces;


namespace VECTO3GUI2020.Helper
{
	public static class XMLExtension
	{
		
		public static XElement CreateSignatureElement(this IXMLComponentWriter componentWriter, XNamespace nameSpace, string uri, DigestData digestData, bool hash = false)
		{
			var di = XMLNamespaces.Di;
			var signatureElement = new XElement(nameSpace + XMLNames.DI_Signature);
			signatureElement.Add(new XElement(di + XMLNames.DI_Signature_Reference, new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, uri)));

			var refElement = signatureElement.FirstNode as XElement;
			refElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_Transforms));

			var transformsElement = refElement?.FirstNode as XElement;
			foreach (var digestDataCanonicalizationMethod in digestData.CanonicalizationMethods) {
				transformsElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, digestDataCanonicalizationMethod)));
			}
			
			refElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod, 
				new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, digestData.DigestMethod)));
			if (hash) {
				throw new NotImplementedException("Hashing not implemented");
			}
			refElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, ""));


			return signatureElement;
		}
	}
}