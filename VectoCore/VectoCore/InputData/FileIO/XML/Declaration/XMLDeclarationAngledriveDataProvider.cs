using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationAngledriveDataProvider : AbstractDeclarationXMLComponentDataProvider, IAngledriveInputData
	{
		public XMLDeclarationAngledriveDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Angledrive,
				XMLNames.ComponentDataWrapper);
		}

		public AngledriveType Type
		{
			get { return InputData._vehicleInputData.AngulargearType; }
		}

		public double Ratio
		{
			get { return GetDoubleElementValue(XMLNames.AngleDrive_Ratio); }
		}

		public TableData LossMap
		{
			get
			{
				return ReadTableData(AttributeMappings.TransmissionLossmapMapping,
					Helper.Query(
						XMLNames.AngleDrive_TorqueLossMap,
						XMLNames.Angledrive_LossMap_Entry));
			}
		}

		public double Efficiency
		{
			get { throw new VectoException("Efficiency not supported in Declaration Mode!"); }
		}
	}
}