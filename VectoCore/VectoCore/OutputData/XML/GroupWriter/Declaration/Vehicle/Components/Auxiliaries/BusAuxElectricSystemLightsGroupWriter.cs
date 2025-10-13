using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    class BusAuxElectricSystemLightsGroupWriter : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		public BusAuxElectricSystemLightsGroupWriter(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Overrides of GroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			var elements = new XElement[] {
				new XElement(_writerNamespace + XMLNames.Bus_Interiorlights, aux.ElectricConsumers.InteriorLightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Dayrunninglights, aux.ElectricConsumers.DayrunninglightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Positionlights, aux.ElectricConsumers.PositionlightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Brakelights, aux.ElectricConsumers.BrakelightsLED),
				new XElement(_writerNamespace + XMLNames.Bus_Headlights, aux.ElectricConsumers.HeadlightsLED),
			};
			return elements.Where(xel => !xel.Value.IsNullOrEmpty()).ToArray();
		}

		#endregion
	}
}
