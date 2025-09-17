using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;

namespace TUGraz.VectoMockup.Reports
{
	public class MockupManufacturerReport : IXMLManufacturerReport
	{
		private AbstractManufacturerReport _ixmlManufacturerReportImplementation;

		
		public MockupManufacturerReport(IXMLManufacturerReport originalManufacturerReport)
		{
			_ixmlManufacturerReportImplementation = originalManufacturerReport as AbstractManufacturerReport;
		}


		#region Implementation of IXMLManufacturerReport


		public void Initialize(VectoRunData modelData)
		{
			_ixmlManufacturerReportImplementation.Initialize(modelData);
		}

		public XDocument Report
		{
			get
			{
				var report = _ixmlManufacturerReportImplementation.Report;
				return report;
			}
		}

		public void WriteResult(IResultEntry resultValue)
		{
			_ixmlManufacturerReportImplementation.WriteResult(resultValue);
		}


		public void GenerateReport()
		{
			_ixmlManufacturerReportImplementation.GenerateReport();
		}

		#endregion



	}
}