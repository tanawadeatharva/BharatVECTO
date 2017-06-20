/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class AbstractEngineeringXMLComponentDataProvider : AbstractDeclarationXMLComponentDataProvider
	{
		protected new readonly XMLEngineeringInputDataProvider InputData;

		protected readonly string FSBasePath;


		protected readonly XPathDocument XMLDocument;

		//protected const string VehiclePath = "/VectoInputEngineering/Vehicle";

		public AbstractEngineeringXMLComponentDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument document, string xmlBasePath, string fsBasePath)
		{
			XMLDocument = document;
			XBasePath = xmlBasePath;
			FSBasePath = fsBasePath;
			InputData = xmlEngineeringJobInputDataProvider;
			Navigator = document.CreateNavigator();
			Manager = new XmlNamespaceManager(Navigator.NameTable);
			Helper = new XPathHelper(ExecutionMode.Engineering);
			Helper.AddNamespaces(Manager);

			Source = fsBasePath;
			SourceType = DataSourceType.Embedded;
		}


		public override bool SavedInDeclarationMode
		{
			get { return false; }
		}

		public override string Manufacturer
		{
			get { return GetElementValue(XMLNames.Component_Manufacturer); }
		}

		public override string Model
		{
			get { return GetElementValue(XMLNames.Component_Model); }
		}

		
		public override string Date
		{
			get { return GetElementValue(XMLNames.Component_Date); }
		}

		public override string DigestValue
		{
			get { return ""; }
		}

		public override string CertificationNumber
		{
			get { return "N.A."; }
		}

		public override CertificationMethod CertificationMethod
		{
			get {  return CertificationMethod.NotCertified;}
		}

		
		protected TableData ReadCSVResourceFile(string relPath)
		{
			if (!ElementExists(Helper.Query(relPath, ExtCsvResourceTag))) {
				throw new VectoException("Failed to read {0} resource", relPath);
			}
			var file =
				GetAttributeValue(
					Helper.Query(relPath, ExtCsvResourceTag), XMLNames.ExtResource_File_Attr);
			var fullFilename = Path.Combine(FSBasePath ?? "", file);
			if (file == null || !File.Exists(fullFilename)) {
				throw new VectoException("{1} file not found: {0}", file, relPath);
			}
			return VectoCSVFile.Read(fullFilename);
		}

		protected string ExtCsvResourceTag
		{
			get {
				return Helper.Query(Helper.QueryConstraint(XMLNames.ExternalResource, XMLNames.ExtResource_Type_Attr,
					XMLNames.ExtResource_Type_Value_CSV));
			}
		}
	}
}