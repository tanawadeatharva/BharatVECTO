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

using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringRetarderDataProvider : AbstractEngineeringXMLComponentDataProvider, IRetarderInputData
	{
		public XMLEngineeringRetarderDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument retarderDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, retarderDocument, xmlBasePath, fsBasePath) {}

		public RetarderType Type
		{
			get { return InputData.VehicleData.RetarderType; }
		}

		public double Ratio
		{
			get { return InputData.VehicleData.RetarderRatio; }
		}

		public TableData LossMap
		{
			get
			{
				if (ElementExists(Helper.Query(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry))) {
					return ReadTableData(AttributeMappings.RetarderLossmapMapping,
						Helper.Query(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry));
				}
				return ReadCSVResourceFile(XMLNames.Retarder_RetarderLossMap);
			}
		}
	}
}