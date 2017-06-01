using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public abstract class AbstractDeclarationXMLComponentDataProvider
	{
		protected readonly XMLDeclarationInputDataProvider InputData;
		protected XPathNavigator Navigator;

		protected string XBasePath = "";
		protected XmlNamespaceManager Manager;


		protected readonly string VehiclePath;

		protected XPathHelper Helper;

		protected AbstractDeclarationXMLComponentDataProvider() {}

		protected AbstractDeclarationXMLComponentDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
		{
			InputData = xmlInputDataProvider;
			Navigator = xmlInputDataProvider.Document.CreateNavigator();
			Manager = new XmlNamespaceManager(Navigator.NameTable);
			Helper = new XPathHelper(ExecutionMode.Declaration);
			Helper.AddNamespaces(Manager);

			SourceType = DataSourceType.Embedded;
			Source = "";

			VehiclePath = Helper.QueryAbs(
				Helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle
				);
		}

		public string Source { get; protected set; }

		public DataSourceType SourceType { get; protected set; }

		public virtual bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual string Manufacturer
		{
			get { return GetElementValue(XMLNames.Component_Manufacturer); }
		}

		public virtual string Model
		{
			get { return GetElementValue(XMLNames.Component_Model); }
		}

		

		public virtual string Date
		{
			get { return GetElementValue(XMLNames.Component_Date); }
		}

		public virtual string TechnicalReportId
		{
			get { return GetElementValue(XMLNames.Component_TechnicalReportId); }
		}

		public virtual CertificationMethod CertificationMethod
		{
			get {
				var value = GetElementValue(XMLNames.Component_CertificationMethod);
				return value.ParseEnum<CertificationMethod>();
			}
		}

		public virtual string CertificationNumber
		{
			get { return GetAttributeValue("..", "certificationNumber"); }
		}

		public virtual string DigestValue
		{
			get { return GetElementValue("..//*[local-name()='DigestValue']"); }
		}

		

		protected bool ElementExists(string relativePath)
		{
			var path = Helper.Query(XBasePath, relativePath.Any() ? relativePath : null);
			//new StringBuilder(XBasePath + (relativePath.Any() ? "/" + relativePath : ""));

			var node = Navigator.SelectSingleNode(path, Manager);
			return node != null;
		}

		protected string GetElementValue(string relativePath)
		{
			var path = Helper.Query(XBasePath, relativePath.Any() ? relativePath : null);

			var node = Navigator.SelectSingleNode(path.ToString(), Manager);
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