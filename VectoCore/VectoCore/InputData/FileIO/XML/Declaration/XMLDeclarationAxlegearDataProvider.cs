using System.ComponentModel.DataAnnotations;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

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

		public AxleLineType LineType
		{
			get {
				var value = GetElementValue(XMLNames.Axlegear_LineType);
				return value.ParseEnum<AxleLineType>();
			}
		}
	}
}