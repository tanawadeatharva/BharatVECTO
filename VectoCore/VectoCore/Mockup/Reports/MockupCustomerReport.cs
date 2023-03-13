using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;

namespace TUGraz.VectoMockup.Reports
{
    public class MockupCustomerReport : IXMLCustomerReport, IXMLMockupReport
    {
		private readonly bool _exempted;
		private readonly AbstractCustomerReport _originalCustomerReport;
        private XNamespace Cif = AbstractCustomerReport.Cif_0_9;
        public MockupCustomerReport(IXMLCustomerReport originalReport, bool exempted)
        {
			_exempted = exempted;
			_originalCustomerReport = originalReport as AbstractCustomerReport;
            _outputDataType = _originalCustomerReport.OutputDataType;
            Results = new XElement(Cif + XMLNames.Report_Results);
        }

        private XElement Results;
        private readonly string _outputDataType;
        private VectoRunData _modelData;
        
        #region Implementation of IXMLCustomerReport

        public void Initialize(VectoRunData modelData)
        {
            _modelData = modelData;
            _originalCustomerReport.Initialize(modelData);
        }

        public XDocument Report
        {
            get
            {
                var report = _originalCustomerReport.Report;
                report.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Results}']").Single().ReplaceWith(Results);

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

        #region Implementation of IXMLMockupReport

        public void WriteMockupResult(IResultEntry resultValue)
        {
            Results.Add(MockupResultReader.GetCIFMockupResult(_outputDataType, resultValue, Cif + "Result", _modelData));
        }

        public void WriteMockupSummary(IResultEntry resultValue)
        {
            Results.AddFirst(new XElement(Cif + "Status", "success"));
            Results.AddFirst(new XComment("Always prints success at the moment"));
			if (!_modelData.VehicleData.InputData.VocationalVehicle) {
				Results.Add(MockupResultReader.GetCIFMockupResult(_outputDataType, resultValue, Cif + "Summary", _modelData));
            }
		}

		public void WriteExemptedResults()
		{
			Results.Add(new XElement(Cif + "Status", "success"));
			Results.Add(new XElement(Cif + "ExemptedVehicle"));
        }

		#endregion
    }
}