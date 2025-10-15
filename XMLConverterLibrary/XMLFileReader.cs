using ErrorOr;
using System.IO;
using System.Xml.Linq;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public class XMLFileReader : IXMLFileReader
	{
		public ErrorOr<XDocument> Read(string xmlFile, XmlDocumentType documentType)
		{
			if (!File.Exists(xmlFile)) {
				return Error.NotFound(description: $"XMLFileReader: file not found: {xmlFile}");
			}

			var validateResult = XMLUtils.ValidateXML(xmlFile, documentType);
			if (validateResult.IsError) {
				return validateResult.Errors;
			}

			return XDocument.Load(xmlFile);
		}

	}
}
