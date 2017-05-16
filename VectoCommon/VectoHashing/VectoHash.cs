using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		public static VectoHash Load(string filename)
		{
			var doc = new XmlDocument();
			XmlTextReader reader = null;
			try {
				reader = new XmlTextReader(filename);
				doc.Load(reader);
			} catch (Exception e) {
				throw new Exception("failed to read XML document", e);
			} finally {
				if (reader != null) {
					reader.Close();
				}
			}
			return new VectoHash(doc);
		}

		public static VectoHash Load(Stream stream)
		{
			var doc = new XmlDocument();
			try {
				doc.Load(new XmlTextReader(stream));
			} catch (Exception e) {
				throw new Exception("failed to read XML document", e);
			}
			return new VectoHash(doc);
		}

		public static VectoHash Load(XmlDocument doc)
		{
			return new VectoHash(doc);
		}

		protected VectoHash(XmlDocument doc)
		{
			Document = doc;
		}

		public IList<VectoComponents> GetContainigComponents()
		{
			var retVal = new List<VectoComponents>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				var count =
					Document.SelectNodes(string.Format("//*[local-name()='{0}']", component.XMLElementName())).Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			return retVal;
		}

		public string ComputeHash()
		{
			var nodes = Document.SelectNodes(GetComponentQueryString());
			if (nodes == null || nodes.Count == 0) {
				throw new Exception("No component found");
			}
			return DoComputeHash(nodes[0]);
		}

		public string ComputeHash(VectoComponents component, int index = 0)
		{
			var nodes = Document.SelectNodes(GetComponentQueryString(component));


			if (nodes == null || nodes.Count == 0) {
				throw new Exception(string.Format("Component {0} not found", component.XMLElementName()));
			}
			if (index >= nodes.Count) {
				throw new Exception(string.Format("index exceeds number of components found! index: {0}, #components: {1}", index,
					nodes.Count));
			}
			return DoComputeHash(nodes[index]);
		}

		private static string DoComputeHash(XmlNode dataNode)
		{
			var parent = dataNode.ParentNode;
			var componentId = dataNode.Attributes[XMLNames.Component_ID_Attr].Value;
			if (parent == null) {
				throw new Exception("Invalid structure of input XML!");
			}
			var newDoc = new XmlDocument();
			var node = newDoc.CreateElement("Dummy");
			newDoc.AppendChild(node);
			var newNode = newDoc.ImportNode(parent, true);
			node.AppendChild(newNode);

			var hash = XMLHashProvider.ComputeHash(newDoc, componentId);
			return GetHashValueFromSig(hash, componentId);
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
			if (components.Count == 0) {
				throw new Exception("input does not contain a known component!");
			}
			var query = string.Format("//*[local-name()='{0}']/*[local-name()='Data']", components[0].XMLElementName());
			var node = Document.SelectSingleNode(query);
			if (node == null) {
				throw new Exception(string.Format("'Data' element for component '{0}' not found!", components[0].XMLElementName()));
			}
			query = string.Format("//*[local-name()='{0}']/*[local-name()='Signature']", components[0].XMLElementName());
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
			var nodes = Document.SelectNodes(GetComponentQueryString());
			if (nodes == null || nodes.Count == 0) {
				throw new Exception("No component found");
			}
			return ReadHashValue(nodes[0]);
		}

		public string ReadHash(VectoComponents component, int index = 0)
		{
			var nodes = Document.SelectNodes(GetComponentQueryString(component));
			if (nodes == null || nodes.Count == 0) {
				throw new Exception(string.Format("Component {0} not found", component.XMLElementName()));
			}
			if (index >= nodes.Count) {
				throw new Exception(string.Format("index exceeds number of components found! index: {0}, #components: {1}", index,
					nodes.Count));
			}
			return ReadHashValue(nodes[index]);
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
				return "(//*[@id])[1]";
			}
			return component == VectoComponents.Vehicle
				? string.Format("//*[local-name()='{0}']", component.Value.XMLElementName())
				: string.Format("//*[local-name()='{0}']/*[local-name()='Data']", component.Value.XMLElementName());
		}

		private static string GetHashValueFromSig(XmlDocument hashed, string elementId)
		{
			var nodes = hashed.SelectNodes("//*[@URI='#" + elementId + "']/*[local-name() = 'DigestValue']");
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			if (nodes.Count > 1) {
				throw new Exception("Multiple DigestValue elements found!");
			}
			return nodes[0].InnerText;
		}

		private static string ReadHashValue(XmlNode dataNode)
		{
			var parent = dataNode.ParentNode;
			if (parent == null) {
				throw new Exception("Invalid structure of input XML!");
			}
			var elementToHash = dataNode.Attributes[XMLNames.Component_ID_Attr].Value;
			var nodes = parent.SelectNodes(".//*[@URI='#" + elementToHash + "']/*[local-name() = 'DigestValue']");
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			if (nodes.Count > 1) {
				throw new Exception("Multiple DigestValue elements found!");
			}
			return nodes[0].InnerText;
		}
	}
}