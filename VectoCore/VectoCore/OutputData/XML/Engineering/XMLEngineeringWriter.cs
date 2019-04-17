using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering
{
	internal class XMLEngineeringWriter : IXMLEngineeringWriter
	{
		protected readonly Dictionary<string, string> NamespaceLocationMap = new Dictionary<string, string>() {
			{ XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10, XMLDefinitions.SCHEMA_BASE_LOCATION + "VectoEngineeringInput.1.0.xsd" },
			{ XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10, XMLDefinitions.SCHEMA_BASE_LOCATION + "VectoEngineeringDefinitions.1.0.xsd"},
			{ XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST, XMLDefinitions.SCHEMA_BASE_LOCATION + "VectoEngineeringDefinitionsTEST.1.1.xsd"},
		};

		protected Dictionary<string, XNamespace> _namespaces = new Dictionary<string, XNamespace>();



		[Inject]
		public IEngineeringWriterFactory WriterFactory { protected get; set; }

		public XMLEngineeringWriter()
		{
			RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);
			RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			_namespaces.Add("", XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);

			Configuration = new WriterConfiguration { SingleFile = true, BasePath = "" };
		}

		public XDocument Write(IInputDataProvider inputData)
		{
			return Write(inputData as IEngineeringInputDataProvider);
		}

		public XDocument Write(IEngineeringInputDataProvider inputData)
		{
			var version = "1.0";
			var jobWriter = WriterFactory.CreateJobWriter(version, this, inputData);

			var retVal = new XDocument();
			var v10 = RegisterNamespace(XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10);

			var jobXML = jobWriter.WriteXML();
			retVal.Add(
				new XElement(
					v10 + XMLNames.VectoInputEngineering,
					GetNamespaceAttributes(),
					GetSchemaLocations(),
					jobXML
				)
			);
			return retVal;
		}

		public XDocument WriteComponent<T>(T componentInputData) where T : class, IComponentInputData
		{
			var retVal = new XDocument();

			var v10Inp = RegisterNamespace(XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10);
			var v10Def = RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			var xsns = RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);
			var writer = WriterFactory.GetWriter(componentInputData, this);

			// wrap every component except the vehicle in a Data element
			var componentXML = writer?.WriteXML(componentInputData);
			if (typeof(T) != typeof(IVehicleEngineeringInputData)) {
				componentXML = new object[] {new XElement(
					v10Def + XMLNames.ComponentDataWrapper,
					componentXML)};
			}
			retVal.Add(
				new XElement(
					v10Inp + XMLNames.VectoComponentEngineering,
					new XAttribute(
						xsns + "type", string.Format("{0}:VectoComponentEngineeringType", GetNSPrefix(v10Def.NamespaceName))),
					GetNamespaceAttributes(),
					GetSchemaLocations(),
					new XElement(
						v10Def + XMLWriterMapping.GetXMLTag(typeof(T)),
						 componentXML )
				)
			);
			return retVal;
		}

		private XAttribute GetSchemaLocations()
		{
			var xsns = RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);

			return new XAttribute(
				xsns + "schemaLocation",
				string.Join(" ", NamespaceLocationMap.Where(x => _namespaces.ContainsKey(x.Key)).Select(x => $"{x.Key} {x.Value}")));
		}

		public WriterConfiguration Configuration { get; set; }

		public string GetFilename<T>(T componentData, string suffix = null) where T : IComponentInputData
		{
			var formatString = XMLWriterMapping.GetFilenameTemplate(typeof(T));

			return
				Path.Combine(
					Configuration.BasePath,
					RemoveInvalidFileCharacters(
						string.Format(
							formatString, componentData.Manufacturer ?? "N.A.", componentData.Model ?? "N.A.",
							suffix != null ? "_" + suffix : null)));
		}


		public XNamespace RegisterNamespace(string namespaceUri)
		{
			if (_namespaces.ContainsKey(namespaceUri)) {
				return _namespaces[namespaceUri];
			}

			var ns = XNamespace.Get(namespaceUri);
			_namespaces.Add(namespaceUri, ns);
			return ns;
		}

		#region Implementation of IXMLEngineeringWriter

		#endregion

		protected virtual object[] GetNamespaceAttributes()
		{
			return _namespaces
				.Select(x => new XAttribute(x.Key == "" ? "xmlns" : XNamespace.Xmlns + GetNSPrefix(x.Key), x.Value)).Cast<object>()
				.ToArray();
		}

		public string GetNSPrefix(string xmlns)
		{
			if (xmlns == XMLDefinitions.XML_SCHEMA_NAMESPACE) {
				return "xsi";
			}
			if (xmlns == XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10) {
				return "tns";
			}

			if (xmlns.StartsWith("urn:tugraz:ivt:VectoAPI")) {
				var parts = xmlns.Split(':');
				return parts.Last();
			}

			throw new VectoException("unknown namespace!");
		}

		public string GetComponentFilename(IComponentInputData component)
		{
			string formatString = null;
			component.Switch()
					.Case<IEngineEngineeringInputData>(c => formatString = "ENG_{0}.xml");

			return RemoveInvalidFileCharacters(string.Format(formatString ?? "{0}", component.Model));
		}

		public string RemoveInvalidFileCharacters(string filename)
		{
			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			return r.Replace(filename, "");
		}
	}
}
