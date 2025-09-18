using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class PEV_VehicleSequenceGroup : AbstractReportOutputGroup
    {
		public PEV_VehicleSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>()
			{
				new XElement(_mrf + "PEVArchitecture", vehicleData.ArchitectureID.GetLabel()),
				new XElement(_mrf + "OffVehicleChargingCapability", vehicleData.OVC),
				new XElement(_mrf + "DynamicChargingTechnology", vehicleData.DynamicChargingTechnology.ToXMLFormat())
			};
			result.Add(_mrfFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			result.Add(_mrfFactory.GetBoostingLimitationsType().GetElement(inputData.JobInputData.Vehicle));
			return result;
		}

		#endregion
	}
}
