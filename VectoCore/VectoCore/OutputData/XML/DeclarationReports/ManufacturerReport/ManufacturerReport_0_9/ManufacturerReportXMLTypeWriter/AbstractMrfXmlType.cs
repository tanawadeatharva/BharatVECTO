using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
	public abstract class AbstractMrfXmlType
	{


		protected XNamespace _mrf = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9";
		protected readonly IManufacturerReportFactory _mrfFactory;

		protected AbstractMrfXmlType(IManufacturerReportFactory mrfFactory)
		{
			_mrfFactory = mrfFactory;
		}
	}
}
