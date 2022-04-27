using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
    public abstract class ComponentWriter
    {
		protected readonly XNamespace _writerNamespace;

		public ComponentWriter(XNamespace writerNamespace)
		{
			_writerNamespace = writerNamespace;
		}
    }
}
