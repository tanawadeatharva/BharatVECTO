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
	public class MockupManufacturerReport : IXMLManufacturerReport, IXMLMockupReport
	{
		private readonly bool _exempted;
		private AbstractManufacturerReport _ixmlManufacturerReportImplementation;
		private VectoRunData _modelData;

		private XNamespace Mrf = AbstractManufacturerReport.Mrf_0_9;
		private readonly string _outputData;
		private XElement Results { get; set; }
		public MockupManufacturerReport(IXMLManufacturerReport originalManufacturerReport, bool exempted)
		{
			_exempted = exempted;
			_ixmlManufacturerReportImplementation = originalManufacturerReport as AbstractManufacturerReport;
			_outputData = _ixmlManufacturerReportImplementation.OutputDataType;

			Results = new XElement(Mrf + XMLNames.Report_Results);
		}

		public void WriteMockupResult(IResultEntry resultValue)
		{

			Results.Add(MockupResultReader.GetMRFMockupResult(_outputData, resultValue, Mrf + "Result", _modelData));

		}

		public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.AddFirst(new XElement(Mrf + "Status", "success"));
			Results.AddFirst(new XComment("Always prints success at the moment"));
			//Results.Add(MockupResultReader.GetMRFMockupResult(OutputDataType, resultValue, Mrf + "Summary", _ovc));
		}

		public void WriteExemptedResults()
		{
			Results.Add(new XElement(Mrf + "Status", "success"));
			Results.Add(new XElement(Mrf + "ExemptedVehicle"));
		}


		#region Implementation of IXMLManufacturerReport


		public void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			_ixmlManufacturerReportImplementation.InitializeVehicleData(inputData);
		}

		public void Initialize(VectoRunData modelData)
		{
			_modelData = modelData;
			_ixmlManufacturerReportImplementation.Initialize(modelData);
		}

		public XDocument Report
		{
			get
			{
				var report = _ixmlManufacturerReportImplementation.Report;
				report.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Results}']").Single().ReplaceWith(Results);

				return report;
			}
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
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