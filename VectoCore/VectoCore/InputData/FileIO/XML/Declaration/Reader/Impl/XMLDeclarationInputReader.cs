using System.Xml;
using System.Xml.Linq;
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

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLDeclarationInputReaderV10(IXMLDeclarationInputData inputData, XmlNode baseNode) : base(
			inputData, baseNode)
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
			job.Reader = Factory.CreateJobReader(version, job, JobNode);
			return job;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInputReaderV20 : XMLDeclarationInputReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationInputReaderV20(IXMLDeclarationInputData inputData, XmlNode baseNode) : base(inputData, baseNode) { }
	}

}
