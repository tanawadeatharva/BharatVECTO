using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal abstract class AbstractExternalResourceReader
	{
		protected XmlNode BaseNode;
		protected bool VerifyXML;
		protected IXMLResource ParentComponent;

		public AbstractExternalResourceReader(IXMLResource parent, XmlNode baseNode, bool verifyXML)
		{
			BaseNode = baseNode;
			VerifyXML = verifyXML;
			ParentComponent = parent;
		}

		protected virtual T CreateComponent<T>(
			string component, Func<string, XmlNode, string, T> componentCreator, bool allowExternalResource = true,
			bool createDummy = false)
		{
			var componentNode = BaseNode.LocalName == component
				? BaseNode
				: BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(component));
			var dataNode = componentNode?.SelectSingleNode(string.Format("./*[local-name()='{0}']", XMLNames.ComponentDataWrapper)) ?? componentNode;
			if (componentNode != null) {
				var version = XMLHelper.GetSchemaVersion(dataNode ?? componentNode);
				try {
					return componentCreator(version, componentNode, ParentComponent.DataSource.SourceFile);
				} catch (Exception e) {
					throw new VectoException("Failed to create component {0} version {1}.", e, component, version);
				}
			}

			if (!allowExternalResource && !createDummy) {
				throw new VectoException("Component {0} not found!", component);
			}

			var componentResourceNode = BaseNode.SelectSingleNode(
				string.Format(
					"./*[local-name()='{0}' and @{1}='{2}' and @{3}='{4}']", XMLNames.ExternalResource,
					XMLNames.ExtResource_Component_Attr, component, XMLNames.ExtResource_Type_Attr,
					XMLNames.ExtResource_Type_Value_XML));
			if (componentResourceNode != null) {
				try {
					var componentFile = componentResourceNode.Attributes?.GetNamedItem(XMLNames.ExtResource_File_Attr).InnerText;
					var fullFileName = componentFile == null ? null : Path.Combine(ParentComponent.DataSource.SourcePath, componentFile);
					if (componentFile == null || !File.Exists(fullFileName)) {
						throw new VectoException(
							"Referenced component file '{1}' for component '{0}' not found!", component, componentFile);
					}

					var componentDocument = new XmlDocument();
					componentDocument.Load(XmlReader.Create(fullFileName));
					if (VerifyXML) {
						new XMLValidator(componentDocument, null, XMLValidator.CallBackExceptionOnError).ValidateXML(
							XmlDocumentType.EngineeringJobData);
					}

					var version = XMLHelper.GetSchemaVersion(componentDocument.DocumentElement);
					try {
						return componentCreator(version, componentDocument.DocumentElement, fullFileName);
					} catch (Exception e) {
						throw new VectoException("Failed to create component {0} version {1}.", e, component, version);
					}
				} catch (XmlSchemaValidationException validationException) {
					throw new VectoException("Validation of XML-file for component {0} failed", validationException, component);
				}
			}

			if (createDummy) {
				try {
					return componentCreator(null, null, null);
				} catch (Exception e) {
					throw new VectoException("Failed to create dummy instance for component {0}.", e, component);
				}
			}

			throw new VectoException("Component {0} not found!", component);
		}
	}
}
