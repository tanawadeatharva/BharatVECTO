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
	public interface IXMLFileReader
	{
		ErrorOr<XDocument> Read(string xmlFile, XmlDocumentType documentType);
	}
}
