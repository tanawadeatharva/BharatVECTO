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
		protected XmlNode JobNode;
		protected IXMLDeclarationInputData InputData;
		protected IDeclarationJobInputData _jobData;

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

		public virtual IDeclarationJobInputData JobData
		{
			get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputDeclaration, JobCreator)); }
		}

		#endregion

		protected virtual IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateJobData(version, BaseNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			var jobNode =
				job.Reader = Factory.CreateJobReader(version, job, JobNode, VerifyXML);
			return job;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInputReaderV20 : XMLDeclarationInputReaderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public XMLDeclarationInputReaderV20(IXMLDeclarationInputData inputData, XmlNode baseNode, bool verifyXML) : base(inputData, baseNode, verifyXML) { }
	}

}
