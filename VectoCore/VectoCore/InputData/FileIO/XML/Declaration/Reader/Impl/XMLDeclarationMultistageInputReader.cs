using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLDeclarationMultistageInputReaderV01 : AbstractComponentReader, IXMLDeclarationMultistageVehicleInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IDeclarationMultistageJobInputData _jobData;

		protected IXMLMultistageInputDataProvider InputData;

		protected XmlNode JobNode;

		public XMLDeclarationMultistageInputReaderV01(IXMLMultistageInputDataProvider inputData, XmlNode baseNode)
			: base(inputData, baseNode)
		{
			JobNode = baseNode;
			InputData = inputData;
		}

		public IDeclarationMultistageJobInputData JobData
		{
			get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoOutputMultistage, JobCreator)); }
		}

		protected virtual IDeclarationMultistageJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateMultiStageJobData(version, BaseNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateMultistageJobReader(version, job, JobNode);
			return job;

		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLMultistageJobReaderV01 : AbstractComponentReader, IXMLMultistageJobReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLDeclarationMultistageJobInputData InputData;
		protected IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		protected IList<IManufacturingStageInputData> _manufacturingStages;

		private XmlNodeList _manufacturingNodeStages;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }



		public XMLMultistageJobReaderV01(IXMLDeclarationMultistageJobInputData inputData, XmlNode baseNode)
			: base(inputData, baseNode)
		{
			InputData = inputData;
			SetManufacturingStageNodes();
		}
		
		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get { return _primaryVehicle ?? (_primaryVehicle = CreateComponent(XMLNames.Bus_PrimaryVehicle, PrimaryVehicleCreator)); }
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get
			{
				if (_manufacturingNodeStages.IsNullOrEmpty())
					return null;

				return _manufacturingStages ?? (_manufacturingStages = ManufacturingStagesCreator());
			}
		}

		protected IPrimaryVehicleInformationInputDataProvider PrimaryVehicleCreator(string version, XmlNode node,
			string arg3)
		{
			var primaryVehicle = Factory.CreatePrimaryMultistageVehicleData(version, node, arg3);
			primaryVehicle.Reader = Factory.CreatePrimaryVehicleBusInputReader(version, primaryVehicle, node.FirstChild);
			return primaryVehicle;
		}

		private IList<IManufacturingStageInputData> ManufacturingStagesCreator()
		{
			var stages = new List<IManufacturingStageInputData>();

			foreach (XmlNode manufacturingNodeStage in _manufacturingNodeStages) {
				var version = XMLHelper.GetXsdType(manufacturingNodeStage?.SchemaInfo.SchemaType);
				stages.Add(ManufacturingStageCreator(version, manufacturingNodeStage));
			}
			return stages;
		}
		
		protected IManufacturingStageInputData ManufacturingStageCreator(string version, XmlNode node)
		{
			var stage = Factory.CreateMultistageData(version, node, null);
			stage.Reader = Factory.CreateMultistageDataReader(version, stage, node);
			return stage;
		}

		private void SetManufacturingStageNodes()
		{
			_manufacturingNodeStages = BaseNode.SelectNodes(XMLHelper.QueryLocalName(XMLNames.ManufacturingStage));
		}
	}

	// ---------------------------------------------------------------------------------------
	
	public class XMLMultistageEntryReaderV01 : AbstractComponentReader, IXMLMultistageReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ManufacturingStageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		
		protected readonly XmlNode JobNode;
		protected readonly IXMLMultistageEntryInputDataProvider _multistageData;
		protected IApplicationInformation _applicationInformation;
		protected IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLMultistageEntryReaderV01(IXMLMultistageEntryInputDataProvider multistageData, XmlNode node) : base(
			multistageData, node)
		{
			JobNode = node;
			_multistageData = multistageData;
		}
		
		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Tag_Vehicle, VehicleCreator)); }
		}

		private IVehicleDeclarationInputData VehicleCreator(string version, XmlNode node, string arg3)
		{
			var vehicle = Factory.CreateVehicleData(version, null, node, arg3);

			if(vehicle.ComponentNode != null)
				vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			
			if(vehicle.ADASNode != null)
				vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);
			
			return vehicle;
		}

		public IApplicationInformation ApplicationInformation 
		{ 
			get
			{
				return _applicationInformation ??
						(_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator));
			}
		}

		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}

		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}
	}
	
	// ---------------------------------------------------------------------------------------


	public class XMLMultistagePrimaryVehicleReaderV01 : AbstractComponentReader, IXMLDeclarationPrimaryVehicleBusInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "PrimaryVehicleDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		protected XmlNode JobNode;
		protected IDeclarationJobInputData _jobData;
		protected IXMLPrimaryVehicleBusInputData _primaryInputData;
		protected IApplicationInformation _applicationInformation;
		protected IResultsInputData _resultsInputData;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }


		public XMLMultistagePrimaryVehicleReaderV01(IXMLPrimaryVehicleBusInputData inputData, XmlNode baseNode) : base(inputData, baseNode)
		{
			JobNode = baseNode;
			_primaryInputData = inputData;
		}

		public virtual IDeclarationJobInputData JobData
		{
			get
			{
				return _jobData ?? (_jobData = CreateComponent(XMLNames.Tag_Vehicle, JobCreator));
			}
		}


		protected IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreatePrimaryVehicleJobData(version, BaseNode, _primaryInputData,
				(_primaryInputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreatePrimaryVehicleJobReader(version, job, JobNode);
			return job;
		}
		
		public IResultsInputData ResultsInputData
		{
			get
			{
				return _resultsInputData ??
					   (_resultsInputData = CreateComponent(XMLNames.Report_Results, ResultsInputDataCreator));
			}
		}

		protected IResultsInputData ResultsInputDataCreator(string version, XmlNode node, string arg3)
		{
			return Factory.CreateResultsInputDataReader(version, node);
		}
		
		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}
		
		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}
		
		public IApplicationInformation ApplicationInformation
		{
			get
			{
				return _applicationInformation ??
					  (_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator));
			}
		}
	}
}