/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	public abstract class AbstractXMLResource : AbstractXMLType, IXMLResource
	{
		protected string SourceFile;

		public AbstractXMLResource(XmlNode node, string source) : base(node)
		{
			SourceFile = source;
		}

		public virtual DataSource DataSource
		{
			get { return new DataSource() { SourceFile = SourceFile, SourceVersion = SourceVersion, SourceType = SourceType }; }
		}


		protected string SourceVersion { get { return XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace); } }

		protected abstract XNamespace SchemaNamespace { get;}

		protected abstract DataSourceType SourceType { get; }
	}


	public abstract class AbstractCommonComponentType : AbstractXMLResource
	{
		public AbstractCommonComponentType(XmlNode node, string source) : base(node, source) { }

		
		public bool SavedInDeclarationMode
		{
			get { return false; }
		}

		public string Manufacturer
		{
			get { return GetString(XMLNames.Component_Manufacturer); }
		}

		public string Model
		{
			get { return GetString(XMLNames.Component_Model); }
		}

		public string Date
		{
			get { return GetString(XMLNames.Component_Date); }
		}

		public virtual CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}

		public virtual string CertificationNumber
		{
			get { return "N.A."; }
		}

		public virtual DigestData DigestValue
		{
			get { return null; }
		}
	}
}
