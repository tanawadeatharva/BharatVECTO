using System.IO;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class AbstractEngineeringXMLComponentDataProvider : AbstractDeclarationXMLComponentDataProvider
	{
		protected new readonly XMLEngineeringInputDataProvider InputData;

		protected readonly string FSBasePath;


		protected readonly XPathDocument XMLDocument;

		//protected const string VehiclePath = "/VectoInputEngineering/Vehicle";

		public AbstractEngineeringXMLComponentDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument document, string xmlBasePath, string fsBasePath)
		{
			XMLDocument = document;
			XBasePath = xmlBasePath;
			FSBasePath = fsBasePath;
			InputData = xmlEngineeringJobInputDataProvider;
			Navigator = document.CreateNavigator();
			Manager = new XmlNamespaceManager(Navigator.NameTable);
			Helper = new XPathHelper(ExecutionMode.Engineering);
			Helper.AddNamespaces(Manager);

			Source = fsBasePath;
			SourceType = DataSourceType.Embedded;
		}


		public override bool SavedInDeclarationMode
		{
			get { return false; }
		}

		public override string Vendor
		{
			get { return GetElementValue(XMLNames.Component_Vendor); }
		}

		public override string ModelName
		{
			get { return GetElementValue(XMLNames.Component_MakeAndModel); }
		}

		public override string Creator
		{
			get { return GetElementValue(XMLNames.Component_Creator); }
		}

		public override string Date
		{
			get { return GetElementValue(XMLNames.Component_Date); }
		}

		public override string TypeId
		{
			get { return GetElementValue(XMLNames.Component_TypeId); }
		}

		public override string DigestValue
		{
			get { return ""; }
		}

		public override IntegrityStatus IntegrityStatus
		{
			get { return IntegrityStatus.Unknown; }
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
			get
			{
				return Helper.Query(Helper.QueryConstraint(XMLNames.ExternalResource, XMLNames.ExtResource_Type_Attr,
					XMLNames.ExtResource_Type_Value_CSV));
			}
		}
	}
}