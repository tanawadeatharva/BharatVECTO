using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationInputDataProviderMultistageV01 : AbstractXMLResource, IXMLMultistageInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IDeclarationMultistageJobInputData JobData;
		protected readonly XmlDocument Document;

		public XMLDeclarationInputDataProviderMultistageV01(XmlDocument xmlDoc, string fileName)
			: base(xmlDoc.DocumentElement, fileName)
		{
			Document = xmlDoc;

			var h = VectoHash.Load(xmlDoc);
			XMLHash = h.ComputeXmlHash();
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		public IDeclarationMultistageJobInputData JobInputData
		{
			get { return JobData ?? (JobData = Reader.JobData); }
		}


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData
		{
			get { throw new NotImplementedException(); }
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }
		public XElement XMLHash { get; }

		public IXMLDeclarationMultistageVehicleInputDataReader Reader { protected get; set; }

	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationMultistageJobInputDataV01 : AbstractXMLResource, IXMLDeclarationMultistageJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI =
			XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		private IList<IManufacturingStageInputData> _manufacturingStages;
		private IManufacturingStageInputData _concolidateManfacturingStage;


		public XMLDeclarationMultistageJobInputDataV01(XmlNode node, IXMLMultistageInputDataProvider inputProvider,
			string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
			SourceType = DataSourceType.XMLFile;
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get { return _primaryVehicle ?? (_primaryVehicle = Reader.PrimaryVehicle); }
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get { return _manufacturingStages ?? (_manufacturingStages = Reader.ManufacturingStages); }
		}

		public IManufacturingStageInputData ConsolidateManufacturingStage
		{
			get { return _concolidateManfacturingStage ?? (_concolidateManfacturingStage = Reader.ConsolidateManufacturingStage); }
		}

		public VectoSimulationJobType JobType
		{
			get { return VectoSimulationJobType.ConventionalVehicle; }
		}

		public bool InputComplete
		{
			get { return Reader.InputComplete; }
		}
		
		public IXMLMultistageJobReader Reader { protected get; set; }

		public IXMLMultistageInputDataProvider InputData { get; }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistagePrimaryVehicleInputDataV01 : AbstractXMLResource,
		IXMLPrimaryVehicleBusInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "PrimaryVehicleDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private readonly XmlNode _signatureNode;
		private IVehicleDeclarationInputData _vehicle;
		private IApplicationInformation _applicationInformation;
		private IResultsInputData _resultsInputData;
		private DigestData _primaryVehicleInputDataHash;
		private DigestData _vehicleSignatureHash;
		private DigestData _manufacturerRecordHash;
		
		public XMLDeclarationMultistagePrimaryVehicleInputDataV01(XmlNode xmlNode, string fileName)
			: base(xmlNode, fileName)
		{
			_signatureNode = xmlNode.LastChild;
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}


		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.JobData.Vehicle); }
		}

		public DigestData PrimaryVehicleInputDataHash
		{
			get { return _primaryVehicleInputDataHash ?? 
						(_primaryVehicleInputDataHash =  Reader.GetDigestData(GetNode("InputDataSignature")));}
		}

		public DigestData VehicleSignatureHash
		{
			get { return _vehicleSignatureHash ?? 
						(_vehicleSignatureHash = Reader.GetDigestData(_signatureNode));}
		}

		public DigestData ManufacturerRecordHash
		{
			get { return _manufacturerRecordHash ??
						(_manufacturerRecordHash =  Reader.GetDigestData(GetNode("ManufacturerRecordSignature"))); }
		}

		public IResultsInputData ResultsInputData
		{
			get { return _resultsInputData ?? (_resultsInputData = Reader.ResultsInputData); }
		}

		public IResult GetResult(VehicleClass vehicleClass, MissionType mission, string fuelMode, Kilogram payload)
		{
			return ResultsInputData.Results.FirstOrDefault(
				x => x.VehicleGroup == vehicleClass &&
					(x.SimulationParameter.Payload - payload).IsEqual(0, 1) && x.Mission == mission &&
					x.SimulationParameter.FuelMode.Equals(fuelMode, StringComparison.InvariantCultureIgnoreCase));
		}

		public XmlNode ResultsNode
		{
			get { return GetNode(XMLNames.Report_Results); }
		}

		public IApplicationInformation ApplicationInformation
		{
			get { return _applicationInformation ?? (_applicationInformation = Reader.ApplicationInformation); }
		}

		public XmlNode ApplicationInformationNode
		{
			get { return GetNode(XMLNames.Tag_ApplicationInformation); }
		}

		public XElement XMLHash { get; }

		public IXMLDeclarationPrimaryVehicleBusInputDataReader Reader { protected get; set; }

	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationMultistageTypeInputDataV01 : AbstractXMLResource, IXMLMultistageEntryInputDataProvider
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ManufacturingStageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private readonly XmlNode _signatureXmlNode;
		private IVehicleDeclarationInputData _vehicle;
		private IApplicationInformation _applicationInformation;
		private DigestData _hashPreviousStage;
		private DigestData _signature;


		public XMLDeclarationMultistageTypeInputDataV01(XmlNode xmlNode, string fileName) 
			: base(xmlNode, fileName)
		{
			_signatureXmlNode = xmlNode.LastChild;
		}
		
		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		public DigestData HashPreviousStage
		{
			get { return _hashPreviousStage ??
						(_hashPreviousStage = Reader.GetDigestData(GetNode("HashPreviousStage"))); }
		}

		public int StageCount
		{
			get { return Convert.ToInt32(GetAttribute(BaseNode, XMLNames.ManufacturingStage_StageCount)); }
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.Vehicle); }
		}

		public IApplicationInformation ApplicationInformation
		{
			get
			{
				return _applicationInformation ?? (_applicationInformation = Reader.ApplicationInformation);
			}
		}

		public DigestData Signature
		{
			get { return _signature ?? (_signature = Reader.GetDigestData(_signatureXmlNode)); }
		}

		public IXMLMultistageReader Reader { protected get; set; }
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationVIFInputData : IMultistageVIFInputData
	{

		private readonly IXMLInputDataReader _xmlInputReader;
		
		public XMLDeclarationVIFInputData(string vifFileName, IVehicleDeclarationInputData vehicleInput)
		{

			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();

			VehicleInputData = vehicleInput;
			MultistageInputData = CreateMultistageReader(vifFileName);
		}


		private IMultistageBusInputDataProvider CreateMultistageReader(string vifFileName)
		{
			var reader = XmlReader.Create(vifFileName);
			return _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
		}


		public DataSource DataSource
		{
			get { return null; }
		}
		public IVehicleDeclarationInputData VehicleInputData { get; }
		public IMultistageBusInputDataProvider MultistageInputData { get; }
	}

}