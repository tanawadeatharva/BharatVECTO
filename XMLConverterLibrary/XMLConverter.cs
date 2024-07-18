using ErrorOr;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public class XMLConverter : IXMLConverter
	{
		private readonly IXMLFileReader _xmlFileReader;
		private readonly IXMLConverterFactory _xmlConverterFactory;
		private readonly IXMLFileWriter _xmlFileWriter;

		public XMLConverter(
			IXMLFileReader xmlFileReader, 
			IXMLConverterFactory xmlConverterFactory, 
			IXMLFileWriter xmlFileWriter) 
		{
			_xmlFileReader = xmlFileReader;
			_xmlConverterFactory = xmlConverterFactory;
			_xmlFileWriter = xmlFileWriter;
		}

		public XmlDocumentType DocumentType => _xmlConverterFactory.DocumentType;

		public IEnumerable<Tuple<string, string>> SupportedConversions => _xmlConverterFactory.SupportedConversions;

		public ErrorOr<string> Convert(string xmlFile, string toVersion)
		{
			var readResult = _xmlFileReader.Read(xmlFile, _xmlConverterFactory.DocumentType);
			if (readResult.IsError) {
				return readResult.Errors;
			}

			var convertResult = Convert(readResult.Value, toVersion);
			if (convertResult.IsError) {  
				return convertResult.Errors; 
			}
			
			var outputPath = _xmlFileWriter.CreateOutputFilePath(xmlFile);
			var writeResult = _xmlFileWriter.Write(convertResult.Value, _xmlConverterFactory.DocumentType, outputPath);

			return writeResult;
		}

		private ErrorOr<XDocument> Convert(XDocument source, string toVersion)
		{
			string fromVersion = XMLUtils.GetVersion(source, _xmlConverterFactory.DocumentType);

			ErrorOr<IXMLEntityConverter> getConverterResult = _xmlConverterFactory.GetConverter(fromVersion, toVersion);
			if (getConverterResult.IsError) {
				return getConverterResult.Errors;
			}

			try {
				return getConverterResult.Value.Convert(source);
			}
			catch (Exception ex) {
				return Error.Failure(description: $"XMLConverter: {ex.Message}");
			}
		}

	}
}
