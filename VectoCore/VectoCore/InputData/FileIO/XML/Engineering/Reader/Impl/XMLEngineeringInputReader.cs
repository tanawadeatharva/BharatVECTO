using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal class XMLEngineeringInputReaderV07 : AbstractExternalResourceReader, IXMLEngineeringInputReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VectoJobEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected XmlNode JobNode;
		protected IXMLEngineeringInputData InputData;
		private IEngineeringJobInputData _jobData;
		private IDriverEngineeringInputData _driverModel;

		[Inject]
		public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLEngineeringInputReaderV07(IXMLEngineeringInputData inputData, XmlNode documentElement) :
			base(inputData, documentElement)
		{
			JobNode = documentElement;
			InputData = inputData;
		}

		public IEngineeringJobInputData JobData
		{
			get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputEngineering, JobCreator, false, requireDataNode: false)); }
		}

		public IDriverEngineeringInputData DriverModel
		{
			get { return _driverModel ?? (_driverModel = CreateComponent(XMLNames.Component_DriverModel, DriverModelCreator, requireDataNode:false)); }
		}


		public IEngineeringJobInputData JobCreator(string version, XmlNode baseNode, string filename)
		{
			var job = Factory.CreateJobData(version, JobNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateJobReader(version, job, JobNode);
			return job;
		}

		public IDriverEngineeringInputData DriverModelCreator(string version, XmlNode baseNode, string filename)
		{
			var driverData = Factory.CreateDriverData(version, InputData, baseNode, (InputData as IXMLResource).DataSource.SourceFile);
			driverData.Reader = Factory.CreateDriverReader(version, driverData, baseNode);
			return driverData;
		}
	}

	internal class XMLEngineeringInputReaderV10 : XMLEngineeringInputReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VectoJobEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringInputReaderV10(
			IXMLEngineeringInputData inputData, XmlNode documentElement) :
			base(inputData, documentElement) { }
	}
}
