using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public abstract class AbstractMrfXmlType : IMrfXmlType
    {


		protected XNamespace _mrf = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9";
		protected readonly IManufacturerReportFactory _mrfFactory;


		public abstract XElement GetXmlType(IDeclarationInputDataProvider inputData);

		protected AbstractMrfXmlType(IManufacturerReportFactory mrfFactory)
		{
			_mrfFactory = mrfFactory;
		}
	}
}
