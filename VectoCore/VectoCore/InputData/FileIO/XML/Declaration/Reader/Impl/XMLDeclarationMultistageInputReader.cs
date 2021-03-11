using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
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
			var job = Factory.CreateMultiStageJobData(version, BaseNode, InputData,"foo"); //(InputData as IXMLResource).DataSource.SourceFile);
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

		public XMLMultistageJobReaderV01(IXMLDeclarationMultistageJobInputData inputData, XmlNode baseNode) : base(
			inputData, baseNode)
		{
			InputData = inputData;
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle {
			get { return null; }
		}
		public IList<IManufacturingStageInputData> ManufacturingStages {
			get {
				//InputData.ManufacturingStages.Select(x => CreateComponent(x, ManufacturingStageCreator)).ToList();
				return null;
			}
		}
	}
}