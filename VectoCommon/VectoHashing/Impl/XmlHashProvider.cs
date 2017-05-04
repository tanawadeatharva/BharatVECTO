using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace TUGraz.VectoHashing.Impl
{
	public class XMLHashProvider
	{
		public XmlDocument ComputeHash(XmlDocument doc, string elementId)
		{
			if (doc == null || doc.DocumentElement == null) {
				throw new Exception("Invalid Document");
			}
			var signedXml = new SignedXml(doc);
			var reference = new Reference("#" + elementId) {
				DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"
			};
			reference.AddTransform(new XmlDsigVectoTransform());
			reference.AddTransform(new XmlDsigC14NTransform());


			signedXml.AddReference(reference);
			signedXml.ComputeSignature(HMAC.Create());
			var xmlDigitalSignature = reference.GetXml();

			var sig = doc.CreateElement("Signature");
			sig.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

			doc.DocumentElement.AppendChild(sig);
			if (doc.FirstChild is XmlDeclaration) {
				doc.RemoveChild(doc.FirstChild);
			}
			return doc;
		}
	}
}