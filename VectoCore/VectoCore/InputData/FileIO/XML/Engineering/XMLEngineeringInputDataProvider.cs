using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringInputDataProvider : IEngineeringInputDataProvider
	{
		public readonly string FileName;

		internal XPathDocument Document;

		protected internal XMLEngineeringAuxiliaryDataProvider XMLEngineeringAuxiliaryData;
		protected internal XMLEngineeringDriverDataProvider XMLEngineeringDriverData;
		protected internal XMLEngineeringJobInputDataProvider XMLEngineeringJobData;
		protected internal XMLEngineeringVehicleDataProvider _vehicleInputData;
		protected internal XMLEngineeringAxlegearDataProvider _axleGearInputData;

		public XMLEngineeringInputDataProvider(string filename, bool verifyXml)
		{
			FileName = filename;
			ReadXMLDocument(File.OpenRead(filename), verifyXml);

			InitializeComponentDataProvider(verifyXml);
		}


		public XMLEngineeringInputDataProvider(Stream inputData, bool verifyXml)
		{
			FileName = ".";
			ReadXMLDocument(inputData, verifyXml);

			var nav = Document.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);

			var refNodes =
				nav.Select(
					"//" + helper.Query(helper.QueryConstraint(XMLNames.ExternalResource, XMLNames.ExtResource_File_Attr, null)),
					manager);
			if (refNodes.Count > 0) {
				throw new VectoException("XML input data with file references can not be read via stream!");
			}

			InitializeComponentDataProvider(verifyXml);
		}


		private void ReadXMLDocument(Stream inputData, bool verifyXml)
		{
			XmlReaderSettings settings = null;
			if (verifyXml) {
				settings = new XmlReaderSettings {
					ValidationType = ValidationType.Schema,
					ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
									//XmlSchemaValidationFlags.ProcessSchemaLocation |
									XmlSchemaValidationFlags.ReportValidationWarnings
				};
				settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
				settings.Schemas.Add(GetXMLSchema(""));
			}
			try {
				Document = new XPathDocument(XmlReader.Create(inputData, settings));
			} catch (XmlSchemaValidationException validationException) {
				throw new VectoException("Validation of input data failed", validationException);
			}
		}

		private void InitializeComponentDataProvider(bool verifyXml)
		{
			XmlReaderSettings settings = null;
			if (verifyXml) {
				settings = new XmlReaderSettings {
					ValidationType = ValidationType.Schema,
					ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
									//XmlSchemaValidationFlags.ProcessSchemaLocation |
									XmlSchemaValidationFlags.ReportValidationWarnings
				};
				settings.Schemas.Add(GetXMLSchema(""));
			}

			var helper = new XPathHelper(ExecutionMode.Engineering);
			XMLEngineeringJobData = new XMLEngineeringJobInputDataProvider(this, Document,
				helper.QueryAbs(helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix)),
				Path.GetDirectoryName(Path.GetFullPath(FileName)));
			if (XMLEngineeringJobData.EngineOnlyMode) {
				EngineInputData = new XMLEngineeringEngineDataProvider(this, Document,
					helper.QueryAbs(helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
						XMLNames.Component_Engine,
						XMLNames.ComponentDataWrapper), Path.GetDirectoryName(Path.GetFullPath(FileName)));
				return;
			}
			ReadVehicle(settings);

			XMLEngineeringDriverData = XMLEngineeringJobData.GetDriverData(settings);
			_axleGearInputData = _vehicleInputData.GetAxleGearInputData(settings);
			AngledriveInputData = _vehicleInputData.GetAngularGearInputData(settings);
			EngineInputData = _vehicleInputData.GetEngineInputData(settings);
			RetarderInputData = _vehicleInputData.GetRetarderInputData(settings);
			XMLEngineeringAuxiliaryData = _vehicleInputData.GetAuxiliaryData(settings);
			GearboxInputData = _vehicleInputData.GetGearboxData(settings);
			TorqueConverterInputData = GearboxInputData.TorqueConverter;
			PTOTransmissionInputData = _vehicleInputData.GetPTOData(settings);
			AirdragInputData = _vehicleInputData.GetAirdragInputData(settings);
		}

		private static void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Error) {
				throw new VectoException("Validation error: {0}", args.Message);
			}
		}

		private void ReadVehicle(XmlReaderSettings settings)
		{
			var helper = new XPathHelper(ExecutionMode.Engineering);


			var nav = Document.CreateNavigator();
			var vehiclePath = helper.QueryAbs(helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle);
			var manager = new XmlNamespaceManager(nav.NameTable);
			helper.AddNamespaces(manager);
			var vehicle = nav.SelectSingleNode(vehiclePath, manager);
			if (vehicle != null) {
				_vehicleInputData = new XMLEngineeringVehicleDataProvider(this, Document, vehiclePath,
					Path.GetDirectoryName(Path.GetFullPath(FileName)));
				return;
			}

			var extVehilePath = helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				helper.QueryConstraint(XMLNames.ExternalResource, "@component='Vehicle' and @type='xml'", null, ""));
			var extVehicle = nav.SelectSingleNode(extVehilePath, manager);
			if (extVehicle != null) {
				try {
					var vehicleFile = extVehicle.GetAttribute(XMLNames.ExtResource_File_Attr, "");
					var vehicleDocument = new XPathDocument(
						XmlReader.Create(Path.Combine(Path.GetDirectoryName(FileName) ?? "./", vehicleFile),
							settings));
					var vehicleCompPath =
						helper.QueryAbs(
							helper.NSPrefix("VectoComponentEngineering", Constants.XML.RootNSPrefix),
							XMLNames.Component_Vehicle);
					_vehicleInputData = new XMLEngineeringVehicleDataProvider(this, vehicleDocument, vehicleCompPath,
						Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FileName) ?? "./", vehicleFile))));
					return;
				} catch (XmlSchemaValidationException validationException) {
					throw new VectoException("Validation of XML-file for Vehicle failed", validationException);
				}
			}
			throw new VectoException("No Vehicle found");
		}

		private static XmlSchemaSet GetXMLSchema(string version)
		{
			var resource = RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema,
				"VectoEngineeringInput.xsd");
			var xset = new XmlSchemaSet() { XmlResolver = new XmlResourceResolver() };
			var reader = XmlReader.Create(resource, new XmlReaderSettings(), XmlResourceResolver.BaseUri);
			xset.Add(XmlSchema.Read(reader, null));
			xset.Compile();
			return xset;
		}

		public IEngineeringJobInputData JobInputData()
		{
			return XMLEngineeringJobData;
		}

		public IVehicleEngineeringInputData VehicleInputData
		{
			get { return _vehicleInputData; }
		}

		public IAirdragEngineeringInputData AirdragInputData { get; private set; }


		public IGearboxEngineeringInputData GearboxInputData { get; private set; }

		public ITorqueConverterEngineeringInputData TorqueConverterInputData { get; private set; }

		public IAxleGearInputData AxleGearInputData
		{
			get { return _axleGearInputData; }
		}

		public IAngledriveInputData AngledriveInputData { get; private set; }

		public IEngineEngineeringInputData EngineInputData { get; private set; }

		public IAuxiliariesEngineeringInputData AuxiliaryInputData()
		{
			return XMLEngineeringAuxiliaryData;
		}

		public IRetarderInputData RetarderInputData { get; private set; }

		public IDriverEngineeringInputData DriverInputData
		{
			get { return XMLEngineeringDriverData; }
		}

		public IPTOTransmissionInputData PTOTransmissionInputData { get; private set; }
	}
}