using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class FuelCell_LorryVehicleOutputTypeGroup : AbstractReportOutputGroup
    {
		public FuelCell_LorryVehicleOutputTypeGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup
		
		// todo amogoda: 2.2 - come up with fuelcell output type
		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_mrfFactory.GetGeneralLorryVehicleOutputGroup().GetElements(inputData));
			result.AddRange(_mrfFactory.GetFuelCell_lorryVehicleOutputSequenceGroup().GetElements(inputData));



			return result;
		}

		#endregion
	}
}
