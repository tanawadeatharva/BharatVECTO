using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public abstract class AbstractReportOutputGroup : IReportOutputGroup
    {
		protected readonly IManufacturerReportFactory _mrfFactory;
		protected XNamespace _mrf = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9";

		protected AbstractReportOutputGroup(IManufacturerReportFactory mrfFactory)
		{
			_mrfFactory = mrfFactory;

		}

		#region Implementation of IMRFGroupWriter

		public abstract IList<XElement> GetElements(IDeclarationInputDataProvider inputData);

		#endregion
	}
}
