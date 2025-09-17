using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;

namespace TUGraz.VectoMockup.Reports
{
    public class MockupCustomerReport : IXMLCustomerReport
    {
		private readonly AbstractCustomerReport _originalCustomerReport;

		public MockupCustomerReport(IXMLCustomerReport originalReport)
        {
			_originalCustomerReport = originalReport as AbstractCustomerReport;
        }

        #region Implementation of IXMLCustomerReport

        public void Initialize(VectoRunData modelData)
        {
            _originalCustomerReport.Initialize(modelData);
        }

        public XDocument Report
        {
            get
            {
                var report = _originalCustomerReport.Report;
                return report;
			}
        }

        public void WriteResult(IResultEntry resultValue)
        {
            _originalCustomerReport.WriteResult(resultValue);
        }

        public void GenerateReport(XElement resultSignature)
        {
            _originalCustomerReport.GenerateReport(resultSignature);
        }

        #endregion


    }
}