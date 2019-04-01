using System;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public abstract class AbstractComponentReader
	{
		protected bool VerifyXML;
		protected readonly XmlNode BaseNode;
		protected readonly IXMLResource ParentComponent;

		public AbstractComponentReader(IXMLResource parent, XmlNode baseNode, bool verifyXML)
		{
			BaseNode = baseNode;
			ParentComponent = parent;
			VerifyXML = verifyXML;
		}

		protected virtual T CreateComponent<T>(
			string component, Func<string, XmlNode, string, T> componentCreator, bool createDummy = false)
		{
			var componentNode = BaseNode.LocalName == component
				? BaseNode
				: BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(component));
			var dataNode =
				componentNode?.SelectSingleNode(string.Format("./*[local-name()='{0}']", XMLNames.ComponentDataWrapper));
			if (componentNode != null) {
				var version = XMLHelper.GetSchemaVersion(dataNode ?? componentNode);
				return componentCreator(version, componentNode, ParentComponent.DataSource.SourceFile);
			}

			if (createDummy) {
				try {
					return componentCreator(null, null, null);
				} catch (Exception e) {
					throw new VectoException("failed to create dummy instance for component {0}", e, component);
				}
			}

			throw new VectoException("Component {0} not found!", component);
		}
	}
}
