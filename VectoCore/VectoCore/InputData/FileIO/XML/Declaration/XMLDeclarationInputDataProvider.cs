using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationInputDataProvider : IDeclarationInputDataProvider
	{
		internal XPathDocument Document;

		private readonly IAuxiliariesDeclarationInputData XMLAuxiliaryData;
		private readonly IDriverDeclarationInputData XMLDriverData;
		private readonly IDeclarationJobInputData XMLJobData;
		protected internal readonly XMLDeclarationVehicleDataProvider _vehicleInputData;

		public XMLDeclarationInputDataProvider(XmlReader inputData, bool verifyXml)
		{
			if (verifyXml) {
				var settings = new XmlReaderSettings {
					ValidationType = ValidationType.Schema,
					ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
									//XmlSchemaValidationFlags.ProcessSchemaLocation |
									XmlSchemaValidationFlags.ReportValidationWarnings
				};
				settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
				settings.Schemas.Add(GetXMLSchema(""));

				inputData = XmlReader.Create(inputData, settings);
			}
			//Document = new XPathDocument(inputData);

			var xmldoc = new XmlDocument();
			xmldoc.Load(inputData);
			var h = VectoHash.Load(xmldoc);
			XMLHash = h.ComputeXmlHash();

			Document = new XPathDocument(new XmlNodeReader(xmldoc));

			//CheckInputDocument();

			XMLJobData = new XMLDeclarationJobInputDataProvider(this);
			_vehicleInputData = new XMLDeclarationVehicleDataProvider(this);
			AirdragInputData = new XMLDeclarationAirdragDataProvider(this);
			AxleGearInputData = new XMLDeclarationAxlegearDataProvider(this);
			AngledriveInputData = new XMLDeclarationAngledriveDataProvider(this);
			EngineInputData = new XMLDeclarationEngineDataProvider(this);
			GearboxInputData = new XMLDeclarationGearboxDataProvider(this);
			TorqueConverterInputData = new XMLDeclarationTorqueConverterDataProvider(this);
			RetarderInputData = new XMLDeclarationRetarderDataProvider(this);
			XMLDriverData = new XMLDeclarationDriverDataProvider(this);
			XMLAuxiliaryData = new XMLDeclarationAuxiliaryDataProvider(this);
			PTOTransmissionInputData = _vehicleInputData.GetPTOData();
		}

		private static void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Error) {
				throw new VectoException("Validation error: {0}" + Environment.NewLine +
										"Line: {1}", args.Message, args.Exception.LineNumber);
			}
		}

		private static XmlSchemaSet GetXMLSchema(string version)
		{
			var resource = RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema, "VectoInput.xsd");
			var xset = new XmlSchemaSet() { XmlResolver = new XmlResourceResolver() };
			var reader = XmlReader.Create(resource, new XmlReaderSettings(), XmlResourceResolver.BaseUri);
			xset.Add(XmlSchema.Read(reader, null));
			xset.Compile();
			return xset;
		}

		public IDeclarationJobInputData JobInputData()
		{
			return XMLJobData;
		}

		public IVehicleDeclarationInputData VehicleInputData
		{
			get { return _vehicleInputData; }
		}

		public IAirdragDeclarationInputData AirdragInputData { get; private set; }

		public IGearboxDeclarationInputData GearboxInputData { get; private set; }

		public ITorqueConverterDeclarationInputData TorqueConverterInputData { get; private set; }

		public IAxleGearInputData AxleGearInputData { get; private set; }

		public IAngledriveInputData AngledriveInputData { get; private set; }

		public IEngineDeclarationInputData EngineInputData { get; private set; }

		public IAuxiliariesDeclarationInputData AuxiliaryInputData()
		{
			return XMLAuxiliaryData;
		}

		public IRetarderInputData RetarderInputData { get; private set; }

		public IDriverDeclarationInputData DriverInputData
		{
			get { return XMLDriverData; }
		}

		public IPTOTransmissionInputData PTOTransmissionInputData { get; private set; }

		public XElement XMLHash { get; private set; }
	}
}