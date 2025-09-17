using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public static class XMLUtils
	{
		private static readonly Dictionary<XmlDocumentType, Func<XDocument, string>> _versionGetters;
		private static readonly Dictionary<string, Dictionary<string, string>> _valueFixes;

		public const string V1_0 = "v1.0";
		public const string V2_0 = "v2.0";
		public const string V2_1 = "v2.1";
		public const string V2_2 = "v2.2";
		public const string V2_2_1 = "v2.2.1";
		public const string V2_3 = "v2.3";
		public const string V2_4 = "v2.4";
		public const string V2_5 = "v2.5";
		public const string V2_7 = "v2.7";

		static XMLUtils()
		{
			_versionGetters = new Dictionary<XmlDocumentType, Func<XDocument, string>>() {
				{  XmlDocumentType.DeclarationJobData, GetJobVersion }
			};

			_valueFixes = new Dictionary<string, Dictionary<string, string>>() {
				{
                    "Vehicle/ChassisConfiguration", new Dictionary<string, string>() { { "Rigid Truck", "Rigid Lorry" } }
                },
				{
                    "Auxiliaries/Data/SteeringPump/Technology", new Dictionary<string, string>() { { "Electric", "Electric driven pump" } }
                },
				{
                    "Vehicle/Components/Auxiliaries/Data/SteeringPump/Technology", new Dictionary<string, string>() { { "Electric", "Electric driven pump" } }
                }
			};
		}

        public static XName TypeAttribute => XMLDeclarationNamespaces.Xsi + XMLNames.XSIType;

        public static XNamespace GetTypeNamespaceOfElement(XDocument doc, XElement element)
        {
            var typeValue = element.Attribute(TypeAttribute)?.Value;
            var typePrefix = typeValue?.Split(':').First();

            return (typeValue != null) && typeValue.Contains(":")
                ? element.GetNamespaceOfPrefix(typePrefix) ?? doc.Root.GetNamespaceOfPrefix(typePrefix)
                : element.Attribute("xmlns")?.Value ?? element.Parent.Attribute("xmlns")?.Value ?? XMLUtils.GetDefaultNamespace(element);
        }

        public static void ResetTypeNamespace(XElement element, XNamespace typeNamespace)
        {
            var typeValue = element.Attribute(TypeAttribute).Value;
            var typeMain = typeValue.Split(':').Last();

            var newTypePrefix = typeNamespace.NamespaceName.Split(':').Last();
            element.SetAttributeValue(XNamespace.Xmlns + newTypePrefix, typeNamespace.NamespaceName);

            element.Attribute(TypeAttribute).Value = $"{newTypePrefix}:{typeMain}";
        }

        public static string QueryLocalName(params string[] nodePath)
		{
			return "./" + string.Join("",
				nodePath.Where(x => x != null).Select(x => $"/*[local-name()='{x}']").ToArray());
		}

		public static string GetVersion(XDocument doc, XmlDocumentType documentType)
		{
			return _versionGetters.ContainsKey(documentType) ? _versionGetters[documentType](doc) : null;
		}

		public static string GetJobVersion(XDocument doc)
		{
			var vehicleNode = doc.XPathSelectElement(QueryLocalName("Vehicle"));

			if (vehicleNode == null)
			{
				return null;
			}

			var versions = new List<string>();

            if (vehicleNode.Attribute(TypeAttribute) != null)
			{
                var typeValue = vehicleNode.Attribute(TypeAttribute).Value;
                var possibleTypePrefix = typeValue.Split(':').First();
				var ns = doc.Root.GetNamespaceOfPrefix(possibleTypePrefix) ?? vehicleNode.GetNamespaceOfPrefix(possibleTypePrefix);

				if (ns != null)
				{
					var typeVersion = ns.NamespaceName.Split(':').Last();
					versions.Add(typeVersion);
				}
            }

            var nameVersion = vehicleNode.Name.NamespaceName.Split(':').Last();
			versions.Add(nameVersion);

			var defaultNamespaceVersion =  vehicleNode.GetDefaultNamespace().NamespaceName.Split(':').Last();
            versions.Add(defaultNamespaceVersion);

            return versions.OrderByDescending(x => x).First();
		}

		public static ErrorOr<bool> ValidateXML(string xmlFile, XmlDocumentType documentType)
		{
			try {
				var xmlReader = System.Xml.XmlReader.Create(xmlFile);

				var validator = new XMLValidator(xmlReader);
				bool isValid = validator.ValidateXML(documentType);

				if (!isValid) {
					return Error.Failure(description: $"ValidateXML: {validator.ValidationError}");
				}
			}
			catch (Exception ex) {
				return Error.Failure(description: $"ValidateXML: {ex.Message}");
			}

			return true;
		}

		public static void SetElementName(XDocument doc, string elementPath, string name, XNamespace xs)
		{
			var node = doc.XPathSelectElement(QueryLocalName(elementPath.Split('/')));

			if (node == null)
			{
				return;
			}

			node.Name = xs + name;
		}

		public static void FixElementsValue(XDocument doc, string elementPath)
		{
			var nodes = doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));

            foreach (var node in nodes)
			{
                if (_valueFixes.ContainsKey(elementPath) && _valueFixes[elementPath].ContainsKey(node.Value))
                {
                    node.Value = _valueFixes[elementPath][node.Value];
                }
            }
		}

		public static IEnumerable<XElement> GetElements(XDocument doc, string elementPath)
		{
			return doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));
		}

        public static void SetElementsChildrenNamespace(XDocument doc, string elementPath, string version)
        {
            XNamespace xs = $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}";

            var nodes = doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));

			foreach (var node in nodes)
			{
				foreach (var item in node?.Elements())
				{
					if (item.Name.NamespaceName.StartsWith(XMLDeclarationNamespaces.DeclarationDefinition))
					{
						item.Name = xs + item.Name.LocalName;
					}
				}
			}
        }

        public static void SetElementsDescendantsNamespace(XDocument doc, string elementPath, string version)
		{
			var nodes = doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));

			XNamespace xs = $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}";

			foreach (var node in nodes)
			{
				foreach (var item in node.Descendants())
				{
					if (item.Name.NamespaceName.StartsWith(XMLDeclarationNamespaces.DeclarationDefinition))
					{
						item.Name = xs + item.Name.LocalName;
					}
				}
			}
		}

		public static XNamespace GetDefaultNamespace(XElement element)
		{
			var defaultNS = XElement.Parse(element.ToString()).GetDefaultNamespace();
			return (defaultNS != XNamespace.None) ? defaultNS : element.Name.Namespace;
        }

		public static void SetElementDescendantsNamespace(XElement node, string version)
		{
			XNamespace xs = $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}";

			foreach (var item in node?.Descendants())
			{
				if (item.Name.NamespaceName.StartsWith(XMLDeclarationNamespaces.DeclarationDefinition))
				{
					item.Name = xs + item.Name.LocalName;
				}
			}
		}

        public static void SetElementDescendantsNS(XElement node, XNamespace ns)
        {
            foreach (var item in node?.Descendants())
            {
                if (item.Name.NamespaceName.StartsWith(XMLDeclarationNamespaces.DeclarationDefinition))
                {
                    item.Name = ns + item.Name.LocalName;
                }
            }
        }

        public static void DeleteElement(XDocument doc, string elementPath)
		{
			var node = doc.XPathSelectElement(QueryLocalName(elementPath.Split('/')));

			node?.Remove();
		}

		public static void AddElementTo(XDocument doc, string parent, string name, string value)
		{
			var node = doc.XPathSelectElement(QueryLocalName(name));

			if (node != null)
			{
				return;
			}

			var parentNode = doc.XPathSelectElement(QueryLocalName(parent.Split('/')));

			parentNode.Add(new XElement(parentNode.Name.Namespace + name, value));
		}

		public static bool AddElementAfter(XDocument doc, string previous, string name, string value)
		{
			var node = doc.XPathSelectElement(QueryLocalName(name));

			if (node != null)
			{
				return false;
			}

			var previousNode = doc.XPathSelectElement(QueryLocalName(previous.Split('/')));

			if (previousNode == null)
			{
				return false;
			}

			previousNode.AddAfterSelf(new XElement(previousNode.Name.Namespace + name, value));

			return true;
		}

		public static void MoveElementUp(XDocument doc, string elementPath)
		{
			var node = doc.XPathSelectElement(QueryLocalName(elementPath.Split('/')));

			if (node == null)
			{
				return;
			}

			var parent = node.Parent;

			node.Remove();

			parent.AddAfterSelf(node);
		}

		public static void SetElementsNamespace(XDocument doc, string elementPath, string prefix)
		{
			var nodes = doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));

			foreach (var node in nodes)
			{
				XNamespace xnsPrefix = doc.Root.GetNamespaceOfPrefix(prefix) ?? $"{XMLDeclarationNamespaces.DeclarationDefinition}:{prefix}";

				node.Name = xnsPrefix + node.Name.LocalName;
			}
		}

        public static void SetElementNamespace(XElement node, string prefix)
        {
            XNamespace xnsPrefix = node.GetNamespaceOfPrefix(prefix) ?? $"{XMLDeclarationNamespaces.DeclarationDefinition}:{prefix}";

            node.Name = xnsPrefix + node.Name.LocalName;
        }

        public static void SetNodeNamespace(XElement node, XNamespace ns)
        {
            node.Name = ns + node.Name.LocalName;
        }

        public static void SetElementNamespace(XDocument doc, string elementPath, XNamespace ns)
		{
            var node = doc.XPathSelectElement(QueryLocalName(elementPath.Split('/')));

			if (node == null)
			{
				return;
			}

			node.Name = ns + node.Name.LocalName;
        }

		public static void SetElementsAttribute(XDocument doc, string elementPath, XName attr, string value)
		{
			var nodes = doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));

			foreach (var node in nodes)
			{
				node.SetAttributeValue(attr, value);
			}
		}

        public static void SetElementsValue(XDocument doc, string elementPath, string value)
        {
            var nodes = doc.XPathSelectElements(QueryLocalName(elementPath.Split('/')));

            foreach (var node in nodes)
            {
                node.Value = value;
            }
        }

        public static void SetElementsType(XDocument doc, string elementPath, string type)
		{
			SetElementsAttribute(doc, elementPath, TypeAttribute, type);
		}

        public static void ConvertDashToHyphen(XDocument doc, string xpath)
        {
            var nodes = doc.XPathSelectElements(XMLUtils.QueryLocalName(xpath.Split('/')));

            foreach (var node in nodes)
            {
                node.Value = node.Value.Replace('–', '-');
            }
        }
    }
}
