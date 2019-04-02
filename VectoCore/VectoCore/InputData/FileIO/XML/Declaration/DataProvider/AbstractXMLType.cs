using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider {
	public abstract class AbstractXMLType
	{
		protected readonly XmlNode BaseNode;

		protected AbstractXMLType(XmlNode node)
		{
			BaseNode = node;
		}

		protected bool ElementExists(string nodeName)
		{
			return GetNode(nodeName, BaseNode, required:false) != null;
		}

		protected string GetString(string nodeName, XmlNode basenode = null, bool required = true)
		{
			return GetNode(nodeName, basenode, required)?.InnerText;
		}

		protected double GetDouble(string nodeName, double? fallbackValue = null)
		{
			var node = GetNode(nodeName, required: fallbackValue != null);

			if (node == null && fallbackValue == null) {
				throw new VectoException("Node {0} not found in input data", nodeName);
			}

			return node?.InnerText.ToDouble() ?? fallbackValue.Value;
		}

		protected double GetDouble(string[] nodePath, double? fallbackValue = null)
		{
			var node = GetNode(nodePath, required: fallbackValue == null);

			if (node == null && fallbackValue == null) {
				throw new VectoException("Node {0} not found in input data", string.Join("/", nodePath));
			}

			return node?.InnerText.ToDouble() ?? fallbackValue.Value;
		}

		protected bool GetBool(string nodeName)
		{
			return XmlConvert.ToBoolean(GetNode(nodeName).InnerText);
		}

		protected XmlNode GetNode(string[] nodeName, XmlNode baseNode = null, bool required = true)
		{
			return DoGetNode(XMLHelper.QueryLocalName(nodeName), baseNode, required);
		}

		protected XmlNode GetNode(string nodeName, XmlNode baseNode = null, bool required = true)
		{
			return DoGetNode(XMLHelper.QueryLocalName(nodeName), baseNode, required);
		}

		private XmlNode DoGetNode(string xpathQuery, XmlNode baseNode, bool required)
		{
			var node = (baseNode ?? BaseNode)?.SelectSingleNode(xpathQuery);
			if (required && node == null) {
				throw new VectoException("Node {0} not found", xpathQuery);
			}

			return node;
		}

		protected XmlNodeList GetNodes(string nodeName, XmlNode baseNode = null)
		{
			return (baseNode ?? BaseNode).SelectNodes(XMLHelper.QueryLocalName(nodeName));
		}

		protected XmlNodeList GetNodes(string[] nodeName, XmlNode baseNode = null)
		{
			return (baseNode ?? BaseNode).SelectNodes(XMLHelper.QueryLocalName(nodeName));
		}

		protected string GetAttribute(XmlNode node, string attribute)
		{
			return node?.Attributes?.GetNamedItem(attribute)?.InnerText;
		}
	}
}