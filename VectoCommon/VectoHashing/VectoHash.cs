using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoHashing.Util;

namespace TUGraz.VectoHashing
{
	public class VectoHash : IVectoHash
	{
		protected XmlDocument Document;
		private XmlNamespaceManager Manager;

		public static VectoHash Load(string filename)
		{
			var doc = new XmlDocument();
			doc.Load(new XmlTextReader(filename));
			return new VectoHash(doc);
		}

		public static VectoHash Load(Stream stream)
		{
			var doc = new XmlDocument();
			doc.Load(new XmlTextReader(stream));
			return new VectoHash(doc);
		}

		public static VectoHash Load(XmlDocument doc)
		{
			return new VectoHash(doc);
		}

		protected VectoHash(XmlDocument doc)
		{
			Document = doc;
			Manager = new XmlNamespaceManager(doc.NameTable);
		}

		public IList<VectoComponents> GetContainigComponents()
		{
			var retVal = new List<VectoComponents>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				var count =
					Document.SelectNodes(string.Format("//*[local-name()='{0}']", component.XMLElementName()), Manager).Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			return retVal;
		}

		public string ComputeHash()
		{
			var toSign = GetIdForElement(GetComponentQueryString());
			var hash = XMLHashProvider.ComputeHash(Document, toSign);
			return GetHashValue(hash, toSign);
		}

		public string ComputeHash(VectoComponents component, int index = 0)
		{
			var toSign = GetIdForElement(GetComponentQueryString(component), index);
			var hash = XMLHashProvider.ComputeHash(Document, toSign);
			return GetHashValue(hash, toSign);
		}

		public XDocument AddHash()
		{
			var components = GetContainigComponents();
			if (components.Contains(VectoComponents.Vehicle)) {
				throw new Exception("adding hash for Vehicle is not supported");
			}
			if (components.Count > 1) {
				throw new Exception("input must not contain multiple components!");
			}
			var query = string.Format("//*[local-name()='{0}']/*[local-name()='Data']", components[0]);
			var node = Document.SelectSingleNode(query);
			if (node == null) {
				throw new Exception(string.Format("'Data' element for component '{0}' not found!", components[0]));
			}
			query = string.Format("//*[local-name()='{0}']/*[local-name()='Signature']", components[0]);
			var sigNodes = Document.SelectNodes(query);
			if (sigNodes != null && sigNodes.Count > 0) {
				throw new Exception("input data already contains a signature element");
			}

			var attributes = node.Attributes;
			var id = components[0].HashIdPrefix() + Guid.NewGuid().ToString("n").Substring(0, 20);
			var idSet = false;
			if (attributes != null && attributes[XMLNames.Component_ID_Attr] != null) {
				attributes[XMLNames.Component_ID_Attr].Value = id;
				idSet = true;
			}
			if (!idSet) {
				var attr = Document.CreateAttribute(XMLNames.Component_ID_Attr);
				attr.Value = id;
				if (node.Attributes == null) {
					throw new Exception("failed to add 'id' attribute");
				}
				node.Attributes.Append(attr);
			}

			var hash = XMLHashProvider.ComputeHash(Document, id);
			var sig = Document.CreateElement(XMLNames.DI_Signature, node.NamespaceURI);

			if (node.ParentNode == null || hash.DocumentElement == null) {
				throw new Exception("Invalid format of document and/or created hash");
			}
			sig.AppendChild(Document.ImportNode(hash.DocumentElement, true));
			node.ParentNode.AppendChild(sig);
			return Document.ToXDocument();
		}

		public string ReadHash()
		{
			var toRead = GetIdForElement(GetComponentQueryString());
			return GetHashValue(Document, toRead);
		}

		public string ReadHash(VectoComponents component, int index = 0)
		{
			var toRead = GetIdForElement(GetComponentQueryString(component), index);
			return GetHashValue(Document, toRead);
		}

		public bool ValidateHash()
		{
			return ReadHash().Equals(ComputeHash(), StringComparison.Ordinal);
		}

		public bool ValidateHash(VectoComponents component, int index = 0)
		{
			return StructuralComparisons.StructuralEqualityComparer.Equals(ReadHash(component, index),
				ComputeHash(component, index));
		}

		private static string GetComponentQueryString(VectoComponents? component = null)
		{
			if (component == null) {
				return "(//*[@id]/@id)[1]";
			}
			return component == VectoComponents.Vehicle
				? string.Format("//*[local-name()='{0}'/@id", component.Value.XMLElementName())
				: string.Format("//*[local-name()='{0}']/*[local-name()='Data']/@id", component.Value.XMLElementName());
		}

		private string GetIdForElement(string query, int index = 0)
		{
			var node = Document.SelectNodes(query);
			if (node == null) {
				return null;
			}
			var toSign = node[index].Value;
			return toSign;
		}

		private static string GetHashValue(XmlDocument hashed, string elementToHash)
		{
			var node = hashed.SelectSingleNode("//*[@URI='#" + elementToHash + "']/*[local-name() = 'DigestValue']");
			return node == null ? null : node.InnerText;
		}
	}
}