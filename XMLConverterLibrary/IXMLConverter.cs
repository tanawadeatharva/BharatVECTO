using ErrorOr;
using System;
using System.Collections.Generic;
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
