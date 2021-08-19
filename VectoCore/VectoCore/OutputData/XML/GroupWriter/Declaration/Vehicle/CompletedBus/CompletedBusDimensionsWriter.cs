using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    class CompletedBusDimensionsWriter_V2_10_2 : GroupWriter, IVehicleDeclarationGroupWriter
    {
		public CompletedBusDimensionsWriter_V2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }

		public XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle)
		{
			return new XElement[] {
				new XElement(_writerNamespace + XMLNames.Bus_HeightIntegratedBody,
					vehicle.Height.ConvertToMilliMeter()),
				new XElement(_writerNamespace + XMLNames.Bus_VehicleLength, vehicle.Length.ConvertToMilliMeter()),
				new XElement(_writerNamespace + XMLNames.Bus_VehicleWidth, vehicle.Width.ConvertToMilliMeter()),
				new XElement(_writerNamespace + XMLNames.Bus_EntranceHeight,
					vehicle.EntranceHeight.ConvertToMilliMeter())
			};
		}
	}
}
