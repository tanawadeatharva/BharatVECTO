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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringAuxiliaryDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IAuxiliariesEngineeringInputData
	{
		public XMLEngineeringAuxiliaryDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument auxDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, auxDocument, xmlBasePath, fsBasePath) {}


		public IList<IAuxiliaryDeclarationInputData> Auxiliaries
		{
			get { return AuxiliaryInputData().Cast<IAuxiliaryDeclarationInputData>().ToList(); }
		}

		IList<IAuxiliaryEngineeringInputData> IAuxiliariesEngineeringInputData.Auxiliaries
		{
			get { return AuxiliaryInputData().Cast<IAuxiliaryEngineeringInputData>().ToList(); }
		}

		private IEnumerable<AuxiliaryDataInputData> AuxiliaryInputData()
		{
			var retVal = new List<AuxiliaryDataInputData>();
			var auxiliaries = Navigator.Select(Helper.Query(XBasePath, XMLNames.Auxiliaries_Auxiliary), Manager);
			while (auxiliaries.MoveNext()) {
				var constantAux = auxiliaries.Current.SelectSingleNode(Helper.Query(XMLNames.Auxiliaries_Auxiliary_ConstantAuxLoad), Manager);
				if (constantAux == null) {
					retVal.Add(CreateMappingAuxiliary(auxiliaries));
				} else {
					retVal.Add(new AuxiliaryDataInputData() {
						ID = "ConstantAux",
						AuxiliaryType = AuxiliaryDemandType.Constant,
						ConstantPowerDemand = constantAux.ValueAsDouble.SI<Watt>()
					});
				}
			}
			return retVal;
		}

		protected AuxiliaryDataInputData CreateMappingAuxiliary(XPathNodeIterator auxiliaries)
		{
			var auxData = new AuxiliaryDataInputData {
				AuxiliaryType = AuxiliaryDemandType.Mapping,
				ID = auxiliaries.Current.GetAttribute("id", ""),
			};
			var node =
				auxiliaries.Current.SelectSingleNode(ExtCsvResourceTag, Manager);
			if (node != null) {
				var auxFile = node.GetAttribute(XMLNames.ExtResource_File_Attr, "");

				if (!File.Exists(Path.Combine(FSBasePath, auxFile))) {
					throw new VectoException("Auxiliary resource file {0} not found! Aux: {1}", auxFile, auxData.ID);
				}
				AuxiliaryFileHelper.FillAuxiliaryDataInputData(auxData, Path.Combine(FSBasePath, auxFile));
			} else {
				var transmissionRatio =
					auxiliaries.Current.SelectSingleNode(Helper.Query(XMLNames.Auxiliaries_Auxiliary_TransmissionRatioToEngine),
						Manager);
				if (transmissionRatio != null) {
					auxData.TransmissionRatio = transmissionRatio.ValueAsDouble;
				}
				var efficiencyEngine =
					auxiliaries.Current.SelectSingleNode(Helper.Query(XMLNames.Auxiliaries_Auxiliary_EfficiencyToEngine), Manager);
				if (efficiencyEngine != null) {
					auxData.EfficiencyToEngine = efficiencyEngine.ValueAsDouble;
				}
				var efficiencyAuxSupply =
					auxiliaries.Current.SelectSingleNode(Helper.Query(XMLNames.Auxiliaries_Auxiliary_EfficiencyAuxSupply), Manager);
				if (efficiencyAuxSupply != null) {
					auxData.EfficiencyToSupply = efficiencyAuxSupply.ValueAsDouble;
				}
				auxData.DemandMap = ReadTableData(AttributeMappings.AuxMapMapping,
					Helper.Query(XMLNames.Auxiliaries_Auxiliary_AuxMap, XMLNames.Auxiliaries_Auxiliary_AuxMap_Entry),
					auxiliaries.Current);

			}
			return auxData;
		}

		public AuxiliaryModel AuxiliaryAssembly
		{
			get { return AuxiliaryModel.Classic; }
		}

		public string AuxiliaryVersion
		{
			get { return ""; }
		}

		public string AdvancedAuxiliaryFilePath
		{
			get { return ""; }
		}
	}
}