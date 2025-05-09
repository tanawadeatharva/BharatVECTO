using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Lorry
{
    internal class HEV_LorryVehicleOutputTypeSequenceGroup : AbstractReportOutputGroup
    {
		public HEV_LorryVehicleOutputTypeSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>();

            var dualFuel = vehicleData.Components?.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1) ?? false;
            result.Add(new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, dualFuel));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_SleeperCab, vehicleData.SleeperCab));
                
			result.AddRange(_mrfFactory.GetHEV_VehicleSequenceGroup().GetElements(inputData));
			if (vehicleData.TankSystem.HasValue) {
				result.Add(new XElement(_mrf + "TankSystem", vehicleData.TankSystem.Value.ToString()));
			}

			return result;
		}

		#endregion
	}
}
