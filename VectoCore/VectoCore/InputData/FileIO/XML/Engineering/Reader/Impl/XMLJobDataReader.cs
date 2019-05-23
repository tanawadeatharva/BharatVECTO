using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal class XMLJobDataReaderV07 : AbstractExternalResourceReader, IXMLJobDataReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VectoJobEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected XmlNode JobNode;
		protected IXMLEngineeringJobInputData JobData;

		private IVehicleEngineeringInputData _vehicle;
		private IEngineEngineeringInputData _engine;

		[Inject] public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLJobDataReaderV07(IXMLEngineeringJobInputData jobData, XmlNode jobNode) : base(
			jobData, jobNode)
		{
			JobNode = jobNode;
			JobData = jobData;
		}

		public IEngineEngineeringInputData CreateEngineOnly
		{
			get { return _engine ?? (_engine = CreateComponent(XMLNames.Component_Engine, (version, node, sourceFile) => Factory.CreateEngineOnlyEngine(version, node, sourceFile))); }
		}

		public IVehicleEngineeringInputData CreateVehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator, requireDataNode: false)); }
		}


		public IXMLCyclesDataProvider CreateCycles
		{
			get {
				var cyclesNode = JobNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.VectoJob_MissionCycles));
				var version = XMLHelper.GetXsdType(cyclesNode?.SchemaInfo.SchemaType);

				return Factory.CreateCycleData(version, JobData, JobNode, JobData.DataSource.SourcePath);
			}
		}


		private IVehicleEngineeringInputData VehicleCreator(string version, XmlNode componentNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, componentNode, sourceFile);
			vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);
			return vehicle;
		}

	}


	internal class XMLJobDataReaderV10 : XMLJobDataReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VectoJobEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLJobDataReaderV10(XMLEngineeringJobInputDataProviderV10 jobData, XmlNode jobNode) : base(jobData, jobNode) { }
	}
}
