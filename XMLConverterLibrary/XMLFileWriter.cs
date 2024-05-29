using ErrorOr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public class XMLFileWriter : IXMLFileWriter
	{
		public const string OUTPUT_FOLDER = "XMLConverter_Output";

		public ErrorOr<string> Write(XDocument document, XmlDocumentType documentType, string filePath)
		{
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}
			catch (Exception ex) {
				return Error.Failure(description: ex.Message);
			}

			document.Save(filePath);

			var validateResult = XMLUtils.ValidateXML(filePath, documentType);
			if (validateResult.IsError) {
				return validateResult.Errors;
			}

			return filePath;
		}

		public string CreateOutputFilePath(string inputFilePath)
		{
			var directory = Path.GetDirectoryName(inputFilePath);
			var filename = Path.GetFileName(inputFilePath);

			return Path.Combine(Path.Combine(directory, OUTPUT_FOLDER), filename);
		}

	}
}
