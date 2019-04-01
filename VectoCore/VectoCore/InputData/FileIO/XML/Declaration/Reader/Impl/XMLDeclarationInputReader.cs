using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLDeclarationInputReaderV10 : AbstractComponentReader, IXMLDeclarationInputDataReader
	{
		private XmlNode JobNode;
		protected IXMLDeclarationInputData InputData;
		private IDeclarationJobInputData _jobData;

		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLDeclarationInputReaderV10(IXMLDeclarationInputData inputData, XmlNode baseNode, bool verifyXML) : base(
			inputData, baseNode, verifyXML)
		{
			JobNode = baseNode;
			InputData = inputData;
		}

		#region Implementation of IXMLDeclarationInputReader

		public IDeclarationJobInputData JobData
		{
			get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputDeclaration, JobCreator)); }
		}

		#endregion

		private IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateJobData(version, BaseNode, InputData, InputData.DataSource.SourceFile);
			var jobNode = 
			job.Reader = Factory.CreateJobReader(version, job, JobNode, VerifyXML);
			return job;
		}
	}
}
