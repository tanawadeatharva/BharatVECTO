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
using System.IO;
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

		private readonly XMLDeclarationJobInputDataProvider _xmlJobData;


        public XMLDeclarationInputDataProvider(string filename, bool verifyXml) :
            this(XmlReader.Create(filename), filename, verifyXml)
        {
        }

        public XMLDeclarationInputDataProvider(XmlReader inputData, bool verifyXml) : this(inputData, "", verifyXml)
        {
            
        }

        protected XMLDeclarationInputDataProvider(XmlReader inputData, string source, bool verifyXml)
        {
            Source = source;
			var xmldoc = new XmlDocument();
			xmldoc.Load(inputData);

			if (verifyXml) {
				new XMLValidator(xmldoc, null, ValidationCallBack).ValidateXML(XMLValidator.XmlDocumentType.DeclarationJobData);
			}
			
			var h = VectoHash.Load(xmldoc);
			XMLHash = h.ComputeXmlHash();
			Document = new XPathDocument(new XmlNodeReader(xmldoc));
            
			_xmlJobData = new XMLDeclarationJobInputDataProvider(this);
		}

        public string Source { get; protected set; }

        private static void ValidationCallBack(XmlSeverityType severity, ValidationEvent evt)
		{
			if (severity == XmlSeverityType.Error) {
				var args = evt.ValidationEventArgs;
				throw new VectoException("Validation error: {0}" + Environment.NewLine +
										"Line: {1}", args.Message, args.Exception.LineNumber);
			}
		}

		public IDeclarationJobInputData JobInputData
		{
			get { return _xmlJobData; }
		}

		public XMLDeclarationJobInputDataProvider XMLJob
		{
			get { return _xmlJobData; }
		}

		public XElement XMLHash { get; private set; }
	}
}
