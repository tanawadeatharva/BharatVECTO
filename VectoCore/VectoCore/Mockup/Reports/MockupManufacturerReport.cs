using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
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