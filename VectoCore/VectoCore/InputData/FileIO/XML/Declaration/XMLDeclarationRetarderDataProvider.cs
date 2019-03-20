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

using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationRetarderDataProvider : AbstractDeclarationXMLComponentDataProvider, IRetarderInputData
	{
		private RetarderType? _type;
		private double? _ratio;

		public XMLDeclarationRetarderDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Retarder,
				XMLNames.ComponentDataWrapper);
		}

		public XMLDeclarationRetarderDataProvider(XDocument xml, RetarderType type, double ratio)
		{
			_type = type;
			_ratio = ratio;
			if (xml.Document != null) {
				Navigator = xml.Document.CreateNavigator();
				Manager = new XmlNamespaceManager(Navigator.NameTable ?? new NameTable());
				Helper = new XPathHelper(ExecutionMode.Declaration);
				Manager.AddNamespace(Constants.XML.DeclarationNSPrefix, Constants.XML.VectoDeclarationDefinitionsNS);
				Manager.AddNamespace(Constants.XML.RootNSPrefix, Constants.XML.VectoDeclarationComponentNS);

				XBasePath = Helper.Query(Helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
										Helper.NSPrefix(XMLNames.Component_Retarder, Constants.XML.RootNSPrefix),
										XMLNames.ComponentDataWrapper);
				SourceType = DataSourceType.Embedded;
			}	
		}


		public override bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public RetarderType Type
		{
			get { return _type ?? InputData.XMLJob.XMLVehicle.RetarderType; }
		}

		public double Ratio
		{
			get { return _ratio ?? InputData.XMLJob.XMLVehicle.RetarderRatio; }
		}

		public TableData LossMap
		{
			get
			{
				return ReadTableData(AttributeMappings.RetarderLossmapMapping,
					Helper.Query(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry));
			}
		}
	}
}
