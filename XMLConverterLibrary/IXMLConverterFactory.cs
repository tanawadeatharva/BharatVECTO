using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public interface IXMLConverterFactory
	{
		ErrorOr<IXMLEntityConverter> GetConverter(string fromVersion, string toVersion);

		XmlDocumentType DocumentType { get; }

		IEnumerable<Tuple<string, string>> SupportedConversions { get; }
	}
}
