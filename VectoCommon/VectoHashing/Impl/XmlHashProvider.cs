using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace TUGraz.VectoHashing.Impl
{
	public class XMLHashProvider
	{
		public static XmlDocument ComputeHash(XmlDocument doc, string elementId)
		{
			if (doc == null /*|| doc.DocumentElement == null*/) {
				throw new Exception("Invalid Document");
			}
			var signedXml = new SignedXml(doc);
			var reference = new Reference("#" + elementId) {
				DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"
			};
			reference.AddTransform(new XmlDsigVectoTransform());
			reference.AddTransform(new XmlDsigExcC14NTransform());


			signedXml.AddReference(reference);
			signedXml.ComputeSignature(HMAC.Create());
			var xmlDigitalSignature = reference.GetXml();

			var sigdoc = new XmlDocument();
			sigdoc.CreateElement("Signature");
			sigdoc.AppendChild(sigdoc.ImportNode(xmlDigitalSignature, true));

			return sigdoc;
		}
	}
}