using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class HEV_LorryVehicleOutputTypeGroup : AbstractReportOutputGroup
    {
		public HEV_LorryVehicleOutputTypeGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup
		
		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_mrfFactory.GetGeneralLorryVehicleOutputGroup().GetElements(inputData));
			result.AddRange(_mrfFactory.GetHEV_lorryVehicleOutputSequenceGroup().GetElements(inputData));



			return result;
		}

		#endregion
	}
}
