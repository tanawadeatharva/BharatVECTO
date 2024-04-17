using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    class CompletedBusDimensionsWriter_V2_4 : GroupWriter, IVehicleDeclarationGroupWriter
    {
		public CompletedBusDimensionsWriter_V2_4(XNamespace writerNamespace) : base(writerNamespace) { }

		public static XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle, XNamespace writerNamespace)
		{
			if (vehicle.IsAnyNull(vehicle.Height, vehicle.Length, vehicle.Width, vehicle.EntranceHeight)) {
				return new XElement[0];
			}
			return new XElement[] {
				new XElement(writerNamespace + XMLNames.Bus_HeightIntegratedBody,
					vehicle.Height.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(writerNamespace + XMLNames.Bus_VehicleLength, vehicle.Length.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(writerNamespace + XMLNames.Bus_VehicleWidth, vehicle.Width.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(writerNamespace + XMLNames.Bus_EntranceHeight,
					vehicle.EntranceHeight.ConvertToMilliMeter().ToXMLFormat(0))
			};
		}

		#region Implementation of IVehicleDeclarationGroupWriter

		public XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle)
		{
			return GetGroupElements(vehicle, _writerNamespace);
		}

		#endregion
	}
}
