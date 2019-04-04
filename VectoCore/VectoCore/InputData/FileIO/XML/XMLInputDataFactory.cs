using System;
using System.IO;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.InputData.FileIO.XML
{
	public class XMLInputDataFactory : IXMLInputDataReader
	{
		[Inject]
		public IDeclarationInjectFactory DeclarationFactory { protected get; set; }

		[Inject]
		public IEngineeringInjectFactory EngineeringFactory { protected get; set; }

		public IInputDataProvider Create(string filename, bool verifyXML)
		{
			return ReadXmlDoc(XmlReader.Create(filename), filename, verifyXML);
		}

		public IInputDataProvider Create(Stream inputData, bool verifyXML)
		{
			return ReadXmlDoc(XmlReader.Create(inputData), null, verifyXML);
		}

		public IInputDataProvider Create(XmlReader inputData, bool verifyXML)
		{
			return ReadXmlDoc(inputData, null, verifyXML);
		}

		public IEngineeringInputDataProvider CreateEngineering(string filename, bool verifyXML)
		{
			return DoCreateEngineering(XmlReader.Create(filename), filename, verifyXML);
		}


		public IEngineeringInputDataProvider CreateEngineering(Stream inputData, bool verifyXML)
		{
			return DoCreateEngineering(XmlReader.Create(inputData), null, verifyXML);
		}

		public IEngineeringInputDataProvider CreateEngineering(XmlReader inputData, bool verifyXML)
		{
			return DoCreateEngineering(inputData, null, verifyXML);
		}


		public IDeclarationInputDataProvider CreateDeclaration(string filename, bool verifyXML)
		{
			return DoCreateDeclaration(XmlReader.Create(filename), filename, verifyXML);
		}

		public IDeclarationInputDataProvider CreateDeclaration(XmlReader inputData, bool verifyXML)
		{
			return DoCreateDeclaration(inputData, null, verifyXML);
		}


		private IDeclarationInputDataProvider DoCreateDeclaration(XmlReader inputData, string source, bool verifyXML)
		{
			var retVal = ReadXmlDoc(inputData, source, verifyXML) as IDeclarationInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in declaration mode!");
			}

			return retVal;
		}

		private IEngineeringInputDataProvider DoCreateEngineering(XmlReader inputData, string source, bool verifyXML)
		{
			var retVal = ReadXmlDoc(inputData, source, verifyXML) as IEngineeringInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in engineering mode!");
			}

			return retVal;
		}

		private IInputDataProvider ReadXmlDoc(XmlReader inputData, string source, bool verifyXML)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(inputData);
			if (xmlDoc.DocumentElement == null) {
				throw new VectoException("empty xml document!");
			}

			var documentType = XMLHelper.GetDocumentType(xmlDoc.DocumentElement.LocalName);
			if (documentType == null) {
				throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
			}

			if (verifyXML) {
				new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(documentType.Value);
			}

			switch (documentType.Value) {
				case XmlDocumentType.DeclarationJobData: return ReadDeclarationJob(xmlDoc, source, verifyXML);
				case XmlDocumentType.EngineeringJobData: return ReadEngineeringJob(xmlDoc, source, verifyXML);
				case XmlDocumentType.EngineeringComponentData:
				case XmlDocumentType.DeclarationComponentData:
				case XmlDocumentType.ManufacturerReport:
				case XmlDocumentType.CustomerReport:
					throw new VectoException("XML Document {0} not supported as simulation input!", documentType.Value);
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private IEngineeringInputDataProvider ReadEngineeringJob(XmlDocument xmlDoc, string source, bool verifyXML)
		{
			var versionNumber = XMLHelper.GetSchemaVersion(xmlDoc.DocumentElement);

			var input = EngineeringFactory.CreateInputProvider(versionNumber, xmlDoc, source);
			input.Reader = EngineeringFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement, verifyXML);
			return input;
		}

		private IDeclarationInputDataProvider ReadDeclarationJob(XmlDocument xmlDoc, string source, bool verifyXML)
		{
			var versionNumber = XMLHelper.GetSchemaVersion(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			try {
				var input = DeclarationFactory.CreateInputProvider(versionNumber, xmlDoc, source);
				input.Reader = DeclarationFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement, verifyXML);
				return input;
			} catch (Exception e) {
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
			}
		}
	}
}
