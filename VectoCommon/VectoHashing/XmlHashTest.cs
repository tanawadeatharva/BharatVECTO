using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace TUGraz.VectoHashing
{
	public class XmlHashTest
	{
		public string ComputeHash(XmlDocument doc)
		{
			var signedXml = new SignedXml(doc);
			var reference = new Reference("#elemID") {
				DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256"
			};
			reference.AddTransform(new XmlDsigC14NTransform());
			//reference.AddTransform(new XmlDsigVectoTransform());

			signedXml.AddReference(reference);
			signedXml.ComputeSignature(HMAC.Create());
			var xmlDigitalSignature = reference.GetXml();

			var sig = doc.CreateElement("Signature");
			sig.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

			doc.DocumentElement.AppendChild(sig);
			if (doc.FirstChild is XmlDeclaration) {
				doc.RemoveChild(doc.FirstChild);
			}
			var xmltw = new XmlTextWriter("simple_document_hashed.xml", new UTF8Encoding(false));
			doc.WriteTo(xmltw);
			xmltw.Close();
			var references = signedXml.SignedInfo.References;
			return Convert.ToBase64String(((Reference)references[0]).DigestValue);
		}
	}
}