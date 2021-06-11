using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Core.Internal;

namespace VECTO3GUI2020.Util.XML
{
    public static class XMLNamespaces
    {
		public static readonly string DeclarationDefinition = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions";
		public static readonly string SchemaVersion = "2.0";
		public static XNamespace Xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		public static XNamespace Tns = "urn:tugraz:ivt:VectoAPI:DeclarationInput";
		public static XNamespace Tns_v20 = Tns.NamespaceName + ":v2.0";
		public static XNamespace V26 = DeclarationDefinition + ":DEV:v2.6";
		public static XNamespace V21 = DeclarationDefinition + ":v2.1";
		public static XNamespace V23 = DeclarationDefinition + ":DEV:v2.3";
		public static XNamespace V20 = DeclarationDefinition + ":v2.0";
		public static XNamespace V10 = DeclarationDefinition + ":v1.0";
		public static XNamespace V28 = DeclarationDefinition + ":DEV:v2.8";
		public static XNamespace Di = "http://www.w3.org/2000/09/xmldsig#";


		public static string DeclarationRootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationJob";



		private static readonly Dictionary<XNamespace, string> NamespacePrefix
			= new Dictionary<XNamespace, string> {
				{ Xsi, "xsi" },
				{ Tns, "tns" },
				{ Tns_v20, "tns" },
				{ V10, "v1.0"},
				{ V20, "v2.0"},
				{ V21, "v2.1"},
				{ V23, "v2.3"},
				{ V26, "v2.6"},
				{ Di, "di"},
				{ V28, "v2.8"},
			};

		public static string GetPrefix(XNamespace xNamespace)
		{
			if (xNamespace.NamespaceName.IsNullOrEmpty()) {
				return null;
			}
			string prefix = NamespacePrefix[xNamespace];
			return prefix;
		}

		public static string GetPrefix(string nameSpaceName)
		{
			XNamespace ns = nameSpaceName;
			return GetPrefix(ns);
		}
	}
}
