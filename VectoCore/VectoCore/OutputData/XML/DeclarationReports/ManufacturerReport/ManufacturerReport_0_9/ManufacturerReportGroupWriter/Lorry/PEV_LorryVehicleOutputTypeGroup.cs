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
    internal class PEV_LorryVehicleOutputTypeGroup : AbstractReportOutputGroup
    {
		public PEV_LorryVehicleOutputTypeGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var result = new List<XElement>();
			result.AddRange(_mrfFactory.GetGeneralLorryVehicleOutputGroup().GetElements(inputData));
			result.Add(new XElement(_mrf + XMLNames.Vehicle_SleeperCab, vehicleData.SleeperCab));
			result.AddRange(_mrfFactory.GetPEV_VehicleSequenceGroup().GetElements(inputData));
			return result;
		}

		#endregion
	}
}
