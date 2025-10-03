using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
	public abstract class BuxAuxHVACGroupWriter_v2_4 : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
	{
		protected readonly IGroupWriterFactory _groupWriterFactory;

		protected BuxAuxHVACGroupWriter_v2_4(XNamespace writerNamespace, IGroupWriterFactory groupWriterFactory) : base(writerNamespace)
		{
			_groupWriterFactory = groupWriterFactory;
        }

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public abstract XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux);

		#endregion
	}

    public class BusAuxHVACConventionalGroupWriter : BuxAuxHVACGroupWriter_v2_4
    {
		public BusAuxHVACConventionalGroupWriter(XNamespace writerNamespace,
			IGroupWriterFactory groupWriterFactory) : base(writerNamespace, groupWriterFactory)
		{

		}

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public override XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			var elements = new List<XElement>();

			//if (aux.HVACAux?.SystemConfiguration != null) {
			//	elements.Add(new XElement(_writerNamespace + XMLNames.Bus_SystemConfiguration,
			//		aux.HVACAux.SystemConfiguration.ToXmlFormat()));

   //         }

            var groupElements = _groupWriterFactory
				.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxHVACHeatPumpSequenceGroup, _writerNamespace)
				.GetGroupElements(aux);

			elements.AddRange(groupElements);

			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_AuxiliaryHeaterPower,
				aux.HVACAux.AuxHeaterPower?.ToXMLFormat(0)));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_DoubleGlazing, aux.HVACAux.DoubleGlazing));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_AdjustableAuxiliaryHeater,
				aux.HVACAux.AdjustableAuxiliaryHeater));
			elements.Add(new XElement(_writerNamespace + XMLNames.Bus_SeparateAirDistributionDucts,
				aux.HVACAux.SeparateAirDistributionDucts));

			return elements.Where(xEl => !xEl.Value.IsNullOrEmpty()).ToArray();
		}

#endregion
	}

	public class BusAuxHVACxEVGroupWriter : BuxAuxHVACGroupWriter_v2_4
    {
		public BusAuxHVACxEVGroupWriter(XNamespace writerNamespace, IGroupWriterFactory groupWriterFactory) : base(writerNamespace, groupWriterFactory) { }

		#region Overrides of BuxAuxHVACGroupWriter_v2_4

		public override XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			var elements = new List<XElement>() {
				new XElement(_writerNamespace + XMLNames.Bus_WaterElectricHeater, aux.HVACAux.WaterElectricHeater),
				new XElement(_writerNamespace + XMLNames.Bus_AirElectricHeater, aux.HVACAux.AirElectricHeater),
				new XElement(_writerNamespace + XMLNames.Bus_OtherHeatingTechnology, aux.HVACAux.OtherHeatingTechnology)
			};

			return elements.Where(xEl => !xEl.Value.IsNullOrEmpty()).ToArray();
		}

		#endregion
	}
}
