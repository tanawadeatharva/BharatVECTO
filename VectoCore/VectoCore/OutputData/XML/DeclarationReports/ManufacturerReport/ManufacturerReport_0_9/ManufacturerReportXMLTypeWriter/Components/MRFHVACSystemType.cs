using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
	public class PrimaryBusHVACSystemType : AbstractMrfXmlType
	{
		public PrimaryBusHVACSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var hvac = inputData.JobInputData.Vehicle.Components.BusAuxiliaries.HVACAux;
			return new XElement(_mrf + "HVACSystem",
				new XElement(_mrf + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat),
				new XElement(_mrf + XMLNames.Bus_EngineWasteGasHeatExchanger, hvac.EngineWasteGasHeatExchanger));
		}

		#endregion
	}
}