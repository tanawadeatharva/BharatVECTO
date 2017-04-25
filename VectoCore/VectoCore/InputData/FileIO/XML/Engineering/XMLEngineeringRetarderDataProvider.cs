using System.Collections.Generic;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringRetarderDataProvider : AbstractEngineeringXMLComponentDataProvider, IRetarderInputData
	{
		public XMLEngineeringRetarderDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument retarderDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, retarderDocument, xmlBasePath, fsBasePath) {}

		public RetarderType Type
		{
			get { return InputData._vehicleInputData.RetarderType; }
		}

		public double Ratio
		{
			get { return InputData._vehicleInputData.RetarderRatio; }
		}

		public TableData LossMap
		{
			get
			{
				if (ElementExists(Helper.Query(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry))) {
					var mapping = new Dictionary<string, string> {
						{ XMLNames.Retarder_RetarderLossmap_RetarderSpeed_Attr, RetarderLossMapReader.Fields.RetarderSpeed },
						{ XMLNames.Retarder_RetarderLossmap_TorqueLoss_Attr, RetarderLossMapReader.Fields.TorqueLoss }
					};
					return ReadTableData(AttributeMappings.RetarderLossmapMapping,
						Helper.Query(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry));
				}
				return ReadCSVResourceFile(XMLNames.Retarder_RetarderLossMap);
			}
		}
	}
}