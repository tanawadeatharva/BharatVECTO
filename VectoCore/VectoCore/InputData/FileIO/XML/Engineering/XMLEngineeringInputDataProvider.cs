/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringInputDataProvider : IEngineeringInputDataProvider
	{
		public readonly string FileName;

		internal XPathDocument Document;

		protected internal XMLEngineeringJobInputDataProvider XMLEngineeringJobData;

		protected internal XMLEngineeringVehicleDataProvider VehicleData;
		protected internal XMLEngineeringDriverDataProvider XMLEngineeringDriverData;

		public XmlReaderSettings Settings { get; private set; }

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
				settings.ValidationEventHandler += ValidationCallBack;
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
			Settings = null;
			if (verifyXml) {
				Settings = new XmlReaderSettings {
					ValidationType = ValidationType.Schema,
					ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema |
									//XmlSchemaValidationFlags.ProcessSchemaLocation |
									XmlSchemaValidationFlags.ReportValidationWarnings
				};
				Settings.Schemas.Add(GetXMLSchema(""));
			}

			var helper = new XPathHelper(ExecutionMode.Engineering);
			XMLEngineeringJobData = new XMLEngineeringJobInputDataProvider(this, Document,
				helper.QueryAbs(helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix)),
				Path.GetDirectoryName(Path.GetFullPath(FileName)));
			if (XMLEngineeringJobData.EngineOnlyMode) {
				EngineOnlyInputData = new XMLEngineeringEngineDataProvider(this, Document,
					helper.QueryAbs(helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
						XMLNames.Component_Engine,
						XMLNames.ComponentDataWrapper), Path.GetDirectoryName(Path.GetFullPath(FileName)));
				return;
			}
			ReadVehicle(Settings);

			XMLEngineeringDriverData = XMLEngineeringJobData.GetDriverData();
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
				VehicleData = new XMLEngineeringVehicleDataProvider(this, Document, vehiclePath,
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
					VehicleData = new XMLEngineeringVehicleDataProvider(this, vehicleDocument, vehicleCompPath,
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

		public IEngineeringJobInputData JobInputData
		{
			get { return XMLEngineeringJobData; }
		}

		public IVehicleEngineeringInputData VehicleInputData
		{
			get { return VehicleData; }
		}

		public IEngineEngineeringInputData EngineOnlyInputData { get; private set; }

		public IDriverEngineeringInputData DriverInputData
		{
			get { return XMLEngineeringDriverData; }
		}
	}
}
