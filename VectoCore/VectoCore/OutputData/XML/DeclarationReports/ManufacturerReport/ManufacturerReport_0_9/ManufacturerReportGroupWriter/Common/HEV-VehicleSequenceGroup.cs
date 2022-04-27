using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class HEV_VehicleSequenceGroup : AbstractReportOutputGroup
    {
		public HEV_VehicleSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>()
			{
				new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, vehicleData.DualFuelVehicle),
				new XElement(_mrf + "HEVArchitecture", vehicleData.ArchitectureID.GetLabel()),
				new XElement(_mrf + "OffVehicleChargingCapability", vehicleData.OvcHev),
				vehicleData.OvcHev ? new XElement(_mrf + "OffVehicleChargingMaxPower", vehicleData.MaxChargingPower.ToXMLFormat(0)) : null,
			};
			result.Add(_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			result.Add(_mrfFactory.GetBoostingLimitationsType().GetElement(inputData.JobInputData.Vehicle));
			return result;
		}

		#endregion
	}
}
