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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringAuxiliariesDataProviderV07 : AbstractXMLType, IXMLAuxiliairesData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AuxiliariesDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAuxiliariesDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		public IXMLAuxiliaryReader Reader { protected get; set; }

		#region Implementation of IAuxiliariesEngineeringInputData

		public virtual IList<IAuxiliaryEngineeringInputData> Auxiliaries
		{
			get {
				var auxNodes = GetNodes(XMLNames.Auxiliaries_Auxiliary);
				if (auxNodes == null || auxNodes.Count == 0) {
					return new List<IAuxiliaryEngineeringInputData>();
				}

				var retVal = new List<IAuxiliaryEngineeringInputData>();
				foreach (XmlNode auxNode in auxNodes) {
					retVal.Add(Reader.CreateAuxiliary(auxNode));
				}

				return retVal;
			}
		}

		public virtual AuxiliaryModel AuxiliaryAssembly
		{
			get { return AuxiliaryModel.Classic; }
		}

		public virtual string AuxiliaryVersion
		{
			get { return ""; }
		}

		public virtual string AdvancedAuxiliaryFilePath
		{
			get { return ""; }
		}

		#endregion
	}

	internal class XMLEngineeringAuxiliariesDataProviderV10 : XMLEngineeringAuxiliariesDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AuxiliariesComponentEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAuxiliariesDataProviderV10(
			IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }
	}

	internal class XMLAuxiliaryEngineeringDataV07 : AbstractXMLType, IXMLAuxiliaryData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AuxiliaryEntryEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		protected string BasePath;

		protected AuxiliaryDataInputData AuxData;

		public XMLAuxiliaryEngineeringDataV07(XmlNode node, string basePath) : base(node)
		{
			BasePath = basePath;

			//ReadAuxData();
		}

		protected virtual AuxiliaryDataInputData ReadAuxData()
		{
			var id = BaseNode.Attributes?.GetNamedItem("id")?.InnerText ?? "";
			var childNode = BaseNode.SelectSingleNode("./*");
			if (childNode == null) {
				throw new VectoException("No auxiliary data found! ID: {0}", id);
			}

			if (childNode.LocalName == XMLNames.ExternalResource) {
				var auxFile = childNode.Attributes?.GetNamedItem(XMLNames.ExtResource_File_Attr).InnerText;
				if (string.IsNullOrWhiteSpace(auxFile) || !File.Exists(Path.Combine(BasePath, auxFile))) {
					throw new VectoException("Auxiliary resource file {0} not found! Aux: {1}", auxFile, id);
				}

				var retVal = new AuxiliaryDataInputData() {
					AuxiliaryType = AuxiliaryDemandType.Mapping,
					ID = id,
					DataSource =
						new DataSource() { SourceType = DataSourceType.CSVFile, SourceVersion = "0", SourceFile = auxFile }
				};
				AuxiliaryFileHelper.FillAuxiliaryDataInputData(retVal, Path.Combine(BasePath, auxFile));
				return retVal;
			}

			if (childNode.LocalName == XMLNames.Auxiliaries_Auxiliary_ConstantAuxLoad) {
				return new AuxiliaryDataInputData {
					ID = "ConstantAux",
					AuxiliaryType = AuxiliaryDemandType.Constant,
					ConstantPowerDemand = childNode.InnerText.ToDouble().SI<Watt>(),
					DataSource =
						new DataSource() { SourceType = DataSourceType.XMLEmbedded, SourceVersion = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace) }
				};
			}

			return new AuxiliaryDataInputData() {
				AuxiliaryType = AuxiliaryDemandType.Mapping,
				ID = id,
				TransmissionRatio =
					BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Auxiliaries_Auxiliary_TransmissionRatioToEngine))
							?.InnerText
							.ToDouble() ?? 0,
				EfficiencyToEngine =
					BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Auxiliaries_Auxiliary_EfficiencyToEngine))?.InnerText
							.ToDouble() ?? 0,
				EfficiencyToSupply =
					BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Auxiliaries_Auxiliary_EfficiencyAuxSupply))?.InnerText
							.ToDouble() ?? 0,
				DemandMap = XMLHelper.ReadTableData(
					AttributeMappings.AuxMapMapping,
					BaseNode.SelectNodes(
						XMLHelper.QueryLocalName(XMLNames.Auxiliaries_Auxiliary_AuxMap, XMLNames.Auxiliaries_Auxiliary_AuxMap_Entry))),
				DataSource =
					new DataSource() { SourceType = DataSourceType.XMLEmbedded, SourceVersion = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace) }
			};
		}

		protected virtual XNamespace SchemaNamespace {  get { return NAMESPACE_URI; } }

		#region Implementation of IAuxiliaryEngineeringInputData

		public virtual string ID
		{
			get { return AuxData?.ID ?? (AuxData = ReadAuxData()).ID; }
		}

		public virtual AuxiliaryDemandType AuxiliaryType
		{
			get { return AuxData?.AuxiliaryType ?? (AuxData = ReadAuxData()).AuxiliaryType; }
		}

		public virtual double TransmissionRatio
		{
			get { return AuxData?.TransmissionRatio ?? (AuxData = ReadAuxData()).TransmissionRatio; }
		}

		public virtual double EfficiencyToEngine
		{
			get { return AuxData?.EfficiencyToEngine ?? (AuxData = ReadAuxData()).EfficiencyToEngine; }
		}

		public virtual double EfficiencyToSupply
		{
			get { return AuxData?.EfficiencyToSupply ?? (AuxData = ReadAuxData()).EfficiencyToSupply; }
		}

		public virtual TableData DemandMap
		{
			get { return AuxData?.DemandMap ?? (AuxData = ReadAuxData()).DemandMap; }
		}

		public virtual Watt ConstantPowerDemand
		{
			get { return AuxData?.ConstantPowerDemand ?? (AuxData = ReadAuxData()).ConstantPowerDemand; }
		}

		public DataSource DataSource
		{
			get { return AuxData?.DataSource ?? (AuxData = ReadAuxData()).DataSource; }
		}

		#endregion
	}

	internal class XMLAuxiliaryEngineeringDataV10 : XMLAuxiliaryEngineeringDataV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AuxiliaryEntryEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLAuxiliaryEngineeringDataV10(XmlNode node, string basePath) : base(node, basePath) { }

		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }
	}
}
