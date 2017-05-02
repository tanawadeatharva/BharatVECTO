using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationAxlegearDataProvider : AbstractDeclarationXMLComponentDataProvider, IAxleGearInputData
	{
		public XMLDeclarationAxlegearDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Axlegear,
				XMLNames.ComponentDataWrapper);
		}

		public double Ratio
		{
			get { return GetDoubleElementValue(XMLNames.Axlegear_Ratio); }
		}

		public TableData LossMap
		{
			get {
				return ReadTableData(AttributeMappings.TransmissionLossmapMapping,
					Helper.Query(XMLNames.Axlegear_TorqueLossMap, XMLNames.Axlegear_TorqueLossMap_Entry));
			}
		}

		public double Efficiency
		{
			get { throw new VectoException("Efficiency not supported in Declaration Mode!"); }
		}
	}
}