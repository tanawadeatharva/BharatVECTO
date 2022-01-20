using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class HEV_LorryVehicleOutputTypeSequenceGroup : AbstractMrfXmlGroup
    {
		public HEV_LorryVehicleOutputTypeSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>();
			result.Add(new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, vehicleData.DualFuelVehicle));
			result.AddRange(_mrfFactory.GetHEVVehicleSequenceGroup().GetElements(inputData));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_SleeperCab, vehicleData.SleeperCab));
			if (vehicleData.TankSystem.HasValue) {
				result.Add(new XElement(_mrf + XMLNames.Vehicle_NgTankSystem, vehicleData.TankSystem.Value.ToString()));
			}

			return result;
		}

		#endregion
	}
}
