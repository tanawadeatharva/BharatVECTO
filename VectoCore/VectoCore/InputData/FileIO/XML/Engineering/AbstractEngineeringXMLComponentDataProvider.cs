/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class AbstractEngineeringXMLComponentDataProvider //: AbstractDeclarationXMLComponentDataProvider
	{
		protected readonly XMLEngineeringInputDataProvider InputData;

		protected XPathNavigator Navigator;

		protected readonly string FSBasePath;

		protected string XBasePath = "";
		protected XmlNamespaceManager Manager;


		protected readonly string VehiclePath;

		protected XPathHelper Helper;

		protected readonly XmlDocument XMLDocument;

		protected AbstractEngineeringXMLComponentDataProvider(
			XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XmlDocument document, string xmlBasePath, string fsBasePath)
		{
			XMLDocument = document;
			XBasePath = xmlBasePath;
			FSBasePath = fsBasePath;
			InputData = xmlEngineeringJobInputDataProvider;
			Navigator = document.CreateNavigator();
			Manager = new XmlNamespaceManager(Navigator.NameTable ?? new NameTable());
			Helper = new XPathHelper(ExecutionMode.Engineering);
			Helper.AddNamespaces(Manager);

			SourceType = DataSourceType.Embedded;
		}

		public virtual DataSource DataSource
		{
			get { return new DataSource() { SourceFile = Source, SourceVersion = SourceVersion, SourceType = SourceType }; }
		}
		public string Source { get { return InputData.Source; } }
		public DataSourceType SourceType { get; protected set; }

		protected string SourceVersion { get { return XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace); } }

		protected string SchemaNamespace { get { return null; } }

		public bool SavedInDeclarationMode
		{
			get { return false; }
		}

		public  string Manufacturer
		{
			get { return GetElementValue(XMLNames.Component_Manufacturer); }
		}

		public  string Model
		{
			get { return GetElementValue(XMLNames.Component_Model); }
		}


		public  string Date
		{
			get { return GetElementValue(XMLNames.Component_Date); }
		}

		public  DigestData DigestValue
		{
			get { return null; }
		}

		public  string CertificationNumber
		{
			get { return "N.A."; }
		}

		public  CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}


		protected TableData ReadCSVResourceFile(string relPath)
		{
			if (!ElementExists(Helper.Query(relPath, ExtCsvResourceTag))) {
				throw new VectoException("Failed to read {0} resource", relPath);
			}
			var file =
				GetAttributeValue(
					Helper.Query(relPath, ExtCsvResourceTag), XMLNames.ExtResource_File_Attr);
			var fullFilename = Path.Combine(FSBasePath ?? "", file);
			if (file == null || !File.Exists(fullFilename)) {
				throw new VectoException("{1} file not found: {0}", file, relPath);
			}
			return VectoCSVFile.Read(fullFilename);
		}

		protected string ExtCsvResourceTag
		{
			get {
				return Helper.Query(Helper.QueryConstraint(XMLNames.ExternalResource, XMLNames.ExtResource_Type_Attr,
					XMLNames.ExtResource_Type_Value_CSV));
			}
		}

		protected bool ElementExists(string relativePath)
		{
			var path = Helper.Query(XBasePath, relativePath.Any() ? relativePath : null);

			var node = Navigator.SelectSingleNode(path, Manager);
			return node != null;
		}

		protected string GetElementValue(string relativePath)
		{
			var path = Helper.Query(XBasePath, relativePath.Any() ? relativePath : null);

			var node = Navigator.SelectSingleNode(path, Manager);
			if (node == null) {
				throw new VectoException("Node {0} not found in input data", path);
			}
			return node.InnerXml;
		}

		protected double GetDoubleElementValue(string relativePath)
		{
			return GetElementValue(relativePath).ToDouble();
		}

		protected string GetAttributeValue(string relativePath, string attrName)
		{
			var nodes =
				Navigator.Select(string.IsNullOrWhiteSpace(relativePath) ? XBasePath : Helper.Query(XBasePath, relativePath),
					Manager);
			if (nodes.Count == 0) {
				return null;
			}
			nodes.MoveNext();
			return nodes.Current.GetAttribute(attrName, "");
		}

		protected TableData ReadTableData(Dictionary<string, string> attributeMapping, string relativePath,
			XPathNavigator origin = null)
		{
			var startNode = origin ?? Navigator.SelectSingleNode(XBasePath, Manager);
			if (startNode == null) {
				throw new VectoException("start node for base-path {0} not found!", XBasePath);
			}
			var table = new TableData();
			foreach (var entry in attributeMapping) {
				if (startNode.Select(Helper.Query(relativePath, "@" + entry.Value), Manager).Count == 0) {
					continue;
				}
				table.Columns.Add(entry.Key);
			}
			var nodes = startNode.Select(relativePath, Manager);
			while (nodes.MoveNext()) {
				var row = table.NewRow();
				foreach (var attribute in attributeMapping) {
					if (nodes.Current.SelectSingleNode("@" + attribute.Value) != null) {
						row[attribute.Key] = nodes.Current.GetAttribute(attribute.Value, "");
					}
				}
				table.Rows.Add(row);
			}

			return table;
		}
	}
}