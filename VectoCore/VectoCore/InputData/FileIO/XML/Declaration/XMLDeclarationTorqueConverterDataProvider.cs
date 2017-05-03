using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationTorqueConverterDataProvider : AbstractDeclarationXMLComponentDataProvider,
		ITorqueConverterDeclarationInputData
	{
		public XMLDeclarationTorqueConverterDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Gearbox,
				XMLNames.Component_TorqueConverter,
				XMLNames.ComponentDataWrapper);
		}

		public TableData TCData
		{
			get
			{
				return ReadTableData(AttributeMappings.TorqueConverterDataMapping,
					Helper.Query(XMLNames.TorqueConverter_Characteristics, XMLNames.TorqueConverter_Characteristics_Entry));
			}
		}
	}
}