using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringAxlegearDataProvider : AbstractEngineeringXMLComponentDataProvider, IAxleGearInputData
	{
		public XMLEngineeringAxlegearDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument axlegearDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, axlegearDocument, xmlBasePath, fsBasePath) {}

		public double Ratio
		{
			get { return GetDoubleElementValue(XMLNames.Axlegear_Ratio); }
		}

		public TableData LossMap
		{
			get {
				if (ElementExists(Helper.Query(XMLNames.Axlegear_TorqueLossMap, XMLNames.Axlegear_TorqueLossMap_Entry))) {
					return ReadTableData(AttributeMappings.TransmissionLossmapMapping,
						Helper.Query(XMLNames.Axlegear_TorqueLossMap, XMLNames.Axlegear_TorqueLossMap_Entry));
				}
				return ReadCSVResourceFile(XMLNames.Axlegear_TorqueLossMap);
			}
		}

		public double Efficiency
		{
			get { return GetDoubleElementValue(Helper.Query(XMLNames.Axlegear_TorqueLossMap, XMLNames.Axlegear_Efficiency)); }
		}

		public AxleLineType LineType
		{
			get { return GetElementValue(XMLNames.Axlegear_LineType).ParseEnum<AxleLineType>(); }
		}
	}
}