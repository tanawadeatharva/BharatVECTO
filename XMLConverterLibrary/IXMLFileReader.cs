using ErrorOr;
using System.Xml.Linq;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public interface IXMLFileReader
	{
		ErrorOr<XDocument> Read(string xmlFile, XmlDocumentType documentType);
	}
}
