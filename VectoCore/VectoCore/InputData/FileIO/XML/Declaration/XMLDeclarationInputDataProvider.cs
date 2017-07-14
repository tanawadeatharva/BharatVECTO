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
		internal readonly XPathDocument Document;

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
				settings.ValidationEventHandler += ValidationCallBack;
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