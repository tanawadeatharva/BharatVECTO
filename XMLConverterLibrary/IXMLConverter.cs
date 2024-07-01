using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public interface IXMLConverter
	{
		IEnumerable<Tuple<string, string>> SupportedConversions { get; }

		XmlDocumentType DocumentType { get; }

		ErrorOr<string> Convert(string xmlFile, string toVersion);
	}
}
