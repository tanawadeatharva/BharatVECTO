using System;
using System.Collections.Generic;

namespace TUGraz.VectoCore.Utils
{
	[Flags]
	public enum XmlDocumentType
	{
		DeclarationJobData = 1 << 1,
		DeclarationComponentData = 1 << 3,
		EngineeringJobData = 1 << 4,
		EngineeringComponentData = 1 << 5,
		ManufacturerReport = 1 << 6,
		CustomerReport = 1 << 7,
		MonitoringReport = 1 << 8,
	}

	
	public static class XMLDefinitions
	{
		//public const string SchemaBaseURL = "file:///E:/QUAM/Workspace/VECTO_quam/VectoCore/VectoCore/Resources/XSD/";
		public const string SCHEMA_BASE_LOCATION = "https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/";


		public const string ENGINEERING_INPUT_NAMESPACE_URI_V07 = "urn:tugraz:ivt:VectoAPI:EngineeringInput:v0.7";

		public const string ENGINEERING_DEFINITONS_NAMESPACE_V07 = "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v0.7";

		public const string ENGINEERING_INPUT_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:EngineeringInput:v1.0";

		public const string ENGINEERING_DEFINITONS_NAMESPACE_V10 = "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v1.0";

		public const string ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST = "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v1.0TEST";



		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";

		public const string DECLARATION_INPUT_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v1.0";

		public const string DECLARATION_COMPONENT_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v1.0";


		public const string DECLARATION_MANUFACTURER_REPORT_V05 = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.5";

		public const string DECLARATION_CUSTOMER_REPORT_V05 = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.5";

		public const string DECLARATION_VTP_REPORT_V01 = "urn:tugraz:ivt:VectoAPI:VTPReport:v0.1";


		public const string XML_SCHEMA_NAMESPACE = "http://www.w3.org/2001/XMLSchema-instance";


		// mapping of document type + version => supported schema files (+version)
		private static Dictionary<Tuple<XmlDocumentType, string>, IList<string>> schemaFilenames = new Dictionary<Tuple<XmlDocumentType, string>, IList<string>>();

		static XMLDefinitions()
		{
			RegisterKnownXMLSchemas();
		}

		private static void RegisterKnownXMLSchemas()
		{
			var declarationJob10 = Tuple.Create(XmlDocumentType.DeclarationJobData, "1.0");
			RegisterXMLSchema(declarationJob10, "VectoInput.1.0.xsd");

			var declarationComponent10 = Tuple.Create(XmlDocumentType.DeclarationComponentData, "1.0");
			RegisterXMLSchema(declarationComponent10, "VectoComponent.1.0.xsd");

			var engineeringInput07 = Tuple.Create(XmlDocumentType.EngineeringJobData, "0.7");
			RegisterXMLSchema(engineeringInput07, "VectoEngineeringInput.0.7.xsd");

			var engineeringInput10 = Tuple.Create(XmlDocumentType.EngineeringJobData, "1.0");
			RegisterXMLSchema(engineeringInput10, "VectoEngineeringInput.1.0.xsd");
			RegisterXMLSchema(engineeringInput10, "VectoEngineeringDefinitionsTEST.1.1.xsd");

			var engineeringComponent10 = Tuple.Create(XmlDocumentType.EngineeringComponentData, "1.0");
			RegisterXMLSchema(engineeringComponent10, "VectoEngineeringInput.1.0.xsd");

			var manufacturerReport = Tuple.Create(XmlDocumentType.ManufacturerReport, "0.5");
			RegisterXMLSchema(manufacturerReport, String.Format("VectoOutputManufacturer.{0}.xsd", "0.5"));

			var customerReport = Tuple.Create(XmlDocumentType.CustomerReport, "0.5");
			RegisterXMLSchema(customerReport, String.Format("VectoOutputCustomer.{0}.xsd", "0.5"));


		}

		private static void RegisterXMLSchema(Tuple<XmlDocumentType, string> doctypeversion, string schemafile)
		{
			if (!schemaFilenames.ContainsKey(doctypeversion)) {
				schemaFilenames[doctypeversion] = new List<string>();
			}
			schemaFilenames[doctypeversion].Add(schemafile);
		}

		public static IEnumerable<string> GetSchemaFilenames(XmlDocumentType type, string version)
		{
			var key = Tuple.Create(type, version);
			if (!schemaFilenames.ContainsKey(key)) {
				throw new Exception(String.Format("Invalid argument {0} - only use single flags", type));
			}
			return schemaFilenames[key];
		}

	}
}
