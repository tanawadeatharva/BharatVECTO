using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public class ConventionalLorryVehicleXmlGroup : AbstractReportOutputGroup
    {
		public ConventionalLorryVehicleXmlGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var result = new List<XElement>();
			var dualFuel = vehicle.Components.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1);

			var tankSystem = vehicle.TankSystem.HasValue
				? vehicle.TankSystem.Value.ToString()
				: (vehicle.HydrogenStorageTechnology.HasValue
                    ? vehicle.HydrogenStorageTechnology.Value.ToString()
                    : null);

            result.AddRange(_mrfFactory.GetGeneralLorryVehicleOutputGroup().GetElements(inputData));
			result.AddRange(new List<XElement>(){
				new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, dualFuel),
				new XElement(_mrf + XMLNames.Vehicle_SleeperCab, inputData.JobInputData.Vehicle.SleeperCab),
                _mrfFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
                (tankSystem != null) ? new XElement(_mrf + "TankSystem", tankSystem) : null,
			});

			return result;
		}

		#endregion
	}
}
