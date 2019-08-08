using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Common {
	public abstract class AbstractXMLResource : AbstractXMLType, IXMLResource
	{
		protected string SourceFile;

		public AbstractXMLResource(XmlNode node, string source) : base(node)
		{
			SourceFile = source;
		}

		public virtual DataSource DataSource
		{
			get { return new DataSource() { SourceFile = SourceFile, SourceVersion = SourceVersion, SourceType = SourceType }; }
		}


		protected string SourceVersion { get { return XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace); } }

		protected abstract XNamespace SchemaNamespace { get; }

		protected abstract DataSourceType SourceType { get; }
	}
}