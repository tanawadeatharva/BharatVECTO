using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus
{
	public interface IMrfBusAuxGroup
	{
		IList<XElement> GetElements(IBusAuxiliariesDeclarationData busAuxiliaries);

	}
    internal class CompletedBus_HVACSystem_Group : AbstractReportOutputGroup, IMrfBusAuxGroup
    {
		public CompletedBus_HVACSystem_Group(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfBusAuxGroup

		public IList<XElement> GetElements(IBusAuxiliariesDeclarationData busAuxiliaries)
		{
			return new List<XElement>() {
				new XElement(_mrf + XMLNames.Bus_SystemConfiguration,
					busAuxiliaries.HVACAux.SystemConfiguration.ToXmlFormat()),
				new XElement(_mrf + XMLNames.Bus_HeatPumpTypeDriver,
					new XElement(_mrf + XMLNames.BusHVACHeatPumpCooling,
						busAuxiliaries.HVACAux.HeatPumpTypeCoolingDriverCompartment.GetLabel()),
					new XElement(_mrf + XMLNames.BusHVACHeatPumpHeating,
						busAuxiliaries.HVACAux.HeatPumpTypeHeatingDriverCompartment.GetLabel())),
				new XElement(_mrf + XMLNames.Bus_HeatPumpTypePassenger,
					new XElement(_mrf + XMLNames.BusHVACHeatPumpCooling,
						busAuxiliaries.HVACAux.HeatPumpTypeCoolingPassengerCompartment.GetLabel()),
					new XElement(_mrf + XMLNames.BusHVACHeatPumpHeating,
						busAuxiliaries.HVACAux.HeatPumpTypeHeatingPassengerCompartment.GetLabel())),

				new XElement(_mrf + XMLNames.Bus_DoubleGlazing, busAuxiliaries.HVACAux.DoubleGlazing),
				new XElement(_mrf + XMLNames.Bus_AdjustableAuxiliaryHeater, busAuxiliaries.HVACAux.AdjustableAuxiliaryHeater),
				new XElement(_mrf + XMLNames.Bus_SeparateAirDistributionDucts, busAuxiliaries.HVACAux.SeparateAirDistributionDucts)
			};
		}

		#endregion
	}
}
