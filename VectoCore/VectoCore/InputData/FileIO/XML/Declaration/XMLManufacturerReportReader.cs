using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLManufacturerReportReader
	{
		public static IList<VectoComponents> GetContainingComponents(XmlDocument xmlDocument)
		{
			var retVal = new List<VectoComponents>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				var nodes = xmlDocument.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']/*[local-name()='Model']",
																XMLNames.VectoManufacturerReport, component.XMLElementName()));
				var count = nodes == null ? 0 : nodes.Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			foreach (var component in new[] { XMLNames.AxleWheels_Axles_Axle }) {
				var nodes = xmlDocument.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']",
																XMLNames.VectoManufacturerReport, component));
				var count = nodes == null ? 0 : nodes.Count;
				for (var i = 0; i < count; i++) {
					retVal.Add(VectoComponents.Tyre);
				}
			}
			return retVal;
		}

		public static string GetComponentDataDigestValue(XmlDocument xmlDocument, VectoComponents component, int i)
		{
			var node = GetNodes(xmlDocument, component, i);

			return ReadElementValue(node, XMLNames.DI_Signature_Reference_DigestValue);
		}

		public static XmlNode GetNodes(XmlDocument xmlDocument, VectoComponents component, int index)
		{
			var nodes = xmlDocument.SelectNodes(GetComponentQueryString(component == VectoComponents.Tyre ? "Axle" : component.XMLElementName()));
			if (nodes == null || nodes.Count == 0) {
				throw new Exception(string.Format("Component {0} not found", component));
			}
			if (index >= nodes.Count) {
				throw new Exception(string.Format("index exceeds number of components found! index: {0}, #components: {1}", index,
												nodes.Count));
			}
			return nodes[index];
		}

		static string GetComponentQueryString(string component = null)
		{
			if (component == null) {
				return "(//*[@id])[1]";
			}
			return string.Format("//*[local-name()='{0}']", component);
		}

		static string ReadElementValue(XmlNode xmlNode, string elementName)
		{
			var node = xmlNode.SelectSingleNode(string.Format("./*[local-name()='{0}']", elementName));
			if (node == null) {
				return null;
			}
			return node.InnerText;
		}
	}
}
