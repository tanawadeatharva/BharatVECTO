using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class HEV_VehicleSequenceGroup : AbstractReportOutputGroup
    {
		public HEV_VehicleSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var ihpc = vehicleData.Components?.GearboxInputData?.Type == GearboxType.IHPC;
			var dualFuel = vehicleData.Components?.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1) ?? false;

			var result = new List<XElement>()
			{
				new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, dualFuel),
				new XElement(_mrf + "HEVArchitecture", ihpc ? GearboxType.IHPC.ToXMLFormat() : vehicleData.ArchitectureID.GetLabel()),
				new XElement(_mrf + "OffVehicleChargingCapability", vehicleData.OVC),
				vehicleData.OVC ? new XElement(_mrf + "OffVehicleChargingMaxPower", vehicleData.MaxChargingPower.ValueAsUnit("kW", 1)) : null,
			};
			result.Add(_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			result.Add(_mrfFactory.GetBoostingLimitationsType().GetElement(inputData.JobInputData.Vehicle));
			return result;
		}

		#endregion
	}
}
