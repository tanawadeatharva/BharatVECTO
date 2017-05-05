using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoHashing.Util;

namespace TUGraz.VectoHashing
{
	public class VectoHash : IVectoHash
	{
		protected XmlDocument document;
		private XPathNavigator Navigator;
		private XmlNamespaceManager Manager;
		private XPathHelper Helper;

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
			document = doc;
			Navigator = doc.CreateNavigator();
			Manager = new XmlNamespaceManager(Navigator.NameTable);
			Helper = new XPathHelper(ExecutionMode.Declaration);
			Helper.AddNamespaces(Manager);
		}

		public IList<VectoComponents> GetContainigComponents()
		{
			var retVal = new List<VectoComponents>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				var count = Navigator.Select(string.Format("//*[local-name()='{0}']", component.XMLElementName()), Manager).Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			return retVal;
		}

		public string ComputeHash()
		{
			var toSign = GetIdForElement(GetComponentQueryString());
			var hash = XMLHashProvider.ComputeHash(document, toSign);
			return GetHashValue(hash, toSign);
		}

		public string ComputeHash(VectoComponents component, int index = 0)
		{
			var toSign = GetIdForElement(GetComponentQueryString(component), index);
			var hash = XMLHashProvider.ComputeHash(document, toSign);
			return GetHashValue(hash, toSign);
		}

		public XDocument AddHash()
		{
			var components = GetContainigComponents();
			if (components.Count > 1) {
				throw new Exception("can only add hash for a single component!");
			}
			if (components[0] == VectoComponents.Vehicle) {
				throw new Exception("adding hash for Vehicle is not supported");
			}
			var query = string.Format("//*[local-name()='{0}']/*[local-name()='Data']", components[0]);
			var node = document.SelectSingleNode(query);
			if (node == null) {
				throw new Exception(string.Format("'Data' element for component {0} not found!", components[0]));
			}
			var attributes = node.Attributes;
			var id = components[0].HashIdPrefix() + Guid.NewGuid().ToString("n").Substring(0, 20);
			var idSet = false;
			if (attributes != null && attributes[XMLNames.Component_ID_Attr] != null) {
				attributes[XMLNames.Component_ID_Attr].Value = id;
				idSet = true;
			}
			if (!idSet) {
				var attr = document.CreateAttribute(XMLNames.Component_ID_Attr);
				attr.Value = id;
				if (node.Attributes == null) {
					throw new Exception("failed to add 'id' attribute");
				}
				node.Attributes.Append(attr);
			}

			var hash = XMLHashProvider.ComputeHash(document, id);
			var sig = document.CreateElement(XMLNames.DI_Signature, node.NamespaceURI);

			if (node.ParentNode == null || hash.DocumentElement == null) {
				throw new Exception("Invalid format of document and/or created hash");
			}
			sig.AppendChild(document.ImportNode(hash.DocumentElement, true));
			node.ParentNode.AppendChild(sig);
			return document.ToXDocument();
		}

		public string ReadHash()
		{
			var toRead = GetIdForElement(GetComponentQueryString());
			return GetHashValue(document, toRead);
		}

		public string ReadHash(VectoComponents component, int index = 0)
		{
			var toRead = GetIdForElement(GetComponentQueryString(component), index);
			return GetHashValue(document, toRead);
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
			var node = document.SelectNodes(query);
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