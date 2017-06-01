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