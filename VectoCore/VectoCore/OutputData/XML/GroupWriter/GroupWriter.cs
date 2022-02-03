using System.Collections.Generic;
using System.Configuration;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter
{
	public abstract class GroupWriter
	{
		protected readonly XNamespace _writerNamespace;

		//This constructor is just used to get the right parameter names when deriving from the class ;)
		protected GroupWriter(XNamespace writerNamespace)
		{
			_writerNamespace = writerNamespace;
		}
	}
}