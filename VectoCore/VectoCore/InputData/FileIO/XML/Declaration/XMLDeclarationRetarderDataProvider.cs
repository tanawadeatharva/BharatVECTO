using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationRetarderDataProvider : AbstractDeclarationXMLComponentDataProvider, IRetarderInputData
	{
		public XMLDeclarationRetarderDataProvider(XMLInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Retarder,
				XMLNames.ComponentDataWrapper);
		}


		public override bool SavedInDeclarationMode
		{
			get { return true; }
		}

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
				return ReadTableData(AttributeMappings.RetarderLossmapMapping,
					Helper.Query(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry));
			}
		}
	}
}