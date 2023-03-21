using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			result.AddRange(_mrfFactory.GetGeneralLorryVehicleOutputGroup().GetElements(inputData));
			result.AddRange(new List<XElement>(){new XElement(_mrf + XMLNames.Vehicle_DualFuelVehicle, dualFuel),
				new XElement(_mrf + XMLNames.Vehicle_SleeperCab, inputData.JobInputData.Vehicle.SleeperCab),
				(inputData.JobInputData.Vehicle.TankSystem.HasValue ? new XElement(_mrf + XMLNames.Vehicle_NgTankSystem, inputData.JobInputData.Vehicle.TankSystem.Value.ToString()) : null),
				//If content is null, nothing is added. When passing a collection, items in the collection can be null. A null item in the collection has no effect on the tree.
				_mrfFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS)});
			return result;
		}

		#endregion
	}
}
