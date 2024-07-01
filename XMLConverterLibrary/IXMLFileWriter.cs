using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public interface IXMLFileWriter
	{
		ErrorOr<string> Write(XDocument document, XmlDocumentType documentType, string filePath);

		string CreateOutputFilePath(string inputFilePath);
	}
}
