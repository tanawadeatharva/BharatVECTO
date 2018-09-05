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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public class XMLValidator
	{
		private readonly Action<XmlSeverityType, ValidationEvent> _validationErrorAction;
		private readonly Action<bool> _resultAction;
		private bool _valid;
		private readonly XmlDocument _doc;

		private static Dictionary<XmlDocumentType, Tuple<string, string[]> > schemaFilenames = new Dictionary<XmlDocumentType, Tuple<string, string[]>>() {
			{XmlDocumentType.DeclarationJobData, Tuple.Create("VectoInput{0}.xsd", new [] {"1.0"}) },
			{XmlDocumentType.DeclarationComponentData, Tuple.Create("VectoComponent{0}.xsd", new [] {"1.0"}) },
			{XmlDocumentType.EngineeringData, Tuple.Create("VectoEngineeringInput{0}.xsd", new [] {"0.7"}) },
			{XmlDocumentType.ManufacturerReport, Tuple.Create("VectoOutputManufacturer{0}.xsd", new [] {"0.4", "0.5", "0.6"}) },
			{ XmlDocumentType.CustomerReport , Tuple.Create("VectoOutputCustomer{0}.xsd", new [] {"0.4", "0.5"})},
		};

		private XMLValidator(Action<bool> resultaction, Action<XmlSeverityType, ValidationEvent> validationErrorAction)
		{
			_validationErrorAction = validationErrorAction ?? ((x, y) => { });
			_resultAction = resultaction ?? (x => { });
			_valid = false;
		}

		public XMLValidator(XmlReader document, Action<bool> resultaction = null, Action<XmlSeverityType, ValidationEvent> validationErrorAction = null):this(resultaction,validationErrorAction)
		{
			_doc = new XmlDocument();
			_doc.Load(document);	
		}

		public XMLValidator(XmlDocument document, Action<bool> resultaction = null, Action<XmlSeverityType, ValidationEvent> validationErrorAction = null) : this(resultaction, validationErrorAction)
		{
			_doc = document;
		}

		public bool ValidateXML(XmlDocumentType docType)
		{ 
			_valid = true;
			if (_doc.DocumentElement == null) {
				throw new Exception("empty XML document");
			}
			var version = _doc.DocumentElement.GetAttribute("schemaVersion");
			_doc.Schemas = GetXMLSchema(docType, version);
			_doc.Validate(ValidationCallBack);
			return _valid;
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			_resultAction(false);
			_valid = false;
			_validationErrorAction(args.Severity, new ValidationEvent { ValidationEventArgs = args });
		}

		private static XmlSchemaSet GetXMLSchema(XmlDocumentType docType, string version)
		{
			var xset = new XmlSchemaSet() { XmlResolver = new XmlResourceResolver() };

			foreach (var entry in EnumHelper.GetValues<XmlDocumentType>()) {
				if ((entry & docType) == 0) {
					continue;
				}
				Stream resource;
				var schemaFile = GetSchemaFilename(entry,  version);
				if (schemaFile == null) {
					continue;
				}
				try {
					resource= RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema, schemaFile);
				} catch (Exception e) {
					throw new Exception(string.Format("Unknown XML schema! version: {0}, xml document type: {1} ({2})", entry, version, schemaFile), e);
				}
				var reader = XmlReader.Create(resource, new XmlReaderSettings(), "schema://");
				xset.Add(XmlSchema.Read(reader, null));
			}
			xset.Compile();
			return xset;
		}

		public static string GetSchemaFilename(XmlDocumentType type, string version)
		{
			if (!schemaFilenames.ContainsKey(type)) {
				throw new Exception(string.Format("Invalid argument {0} - only use single flags", type));
			}
			var entry = schemaFilenames[type];
			return !entry.Item2.Contains(version) ? null : string.Format(entry.Item1, string.IsNullOrWhiteSpace(version) ? "" : "." + version);
		} 

		[Flags]
		public enum XmlDocumentType
		{
			DeclarationJobData = 1<<1,
			DeclarationComponentData = 1<<3,
			EngineeringData = 1<<4,
			ManufacturerReport = 1<<5,
			CustomerReport = 1<<6,
		}
	}

	public class ValidationEvent
	{
		public Exception Exception;
		public ValidationEventArgs ValidationEventArgs;
	}
}
