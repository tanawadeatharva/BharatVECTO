using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationInputDataProviderMultistageV01 : AbstractXMLResource, IXMLMultistageInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IDeclarationMultistageJobInputData JobData;


		public XMLDeclarationInputDataProviderMultistageV01(XmlDocument xmlDoc, string fileName) : base(xmlDoc.DocumentElement, fileName)
		{

		}

		protected override XNamespace SchemaNamespace {
			get { return NAMESPACE_URI; }
		}
		protected override DataSourceType SourceType { get; }

		public IDeclarationMultistageJobInputData JobInputData {
			get { return JobData ?? (JobData = Reader.JobData); }
		}


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData {
			get {
				throw new NotImplementedException();
			}
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }
		public XElement XMLHash { get; }

		public IXMLDeclarationMultistageVehicleInputDataReader Reader { protected get; set; }

	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationMultistageJobInputDataV01 : AbstractXMLResource, IXMLDeclarationMultistageJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		private IList<IManufacturingStageInputData> _manufacturingStages;


		public XMLDeclarationMultistageJobInputDataV01(XmlNode node, IXMLMultistageInputDataProvider inputProvider, string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get { return _primaryVehicle ?? (_primaryVehicle = Reader.PrimaryVehicle); }
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get { return _manufacturingStages ?? (_manufacturingStages = Reader.ManufacturingStages); }
		}

		public IXMLMultistageJobReader Reader { get; set; }
		public IXMLMultistageInputDataProvider InputData { get; }
		protected override XNamespace SchemaNamespace { get; }
		protected override DataSourceType SourceType { get; }
	}
}