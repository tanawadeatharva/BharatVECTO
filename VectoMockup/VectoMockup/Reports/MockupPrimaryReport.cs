using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoMockup.Reports
{
    internal class MockupPrimaryReport : IXMLPrimaryVehicleReport, IXMLMockupReport
    {
		
		private readonly IXMLPrimaryVehicleReport _primaryVehicleReportImplementation;
		private VectoRunData _modelData;

		private XElement Results;
		public MockupPrimaryReport(IXMLPrimaryVehicleReport primaryVehicleReportImplementation)
		{
			_primaryVehicleReportImplementation = primaryVehicleReportImplementation;
			Results = new XElement(_primaryVehicleReportImplementation.Tns + XMLNames.Report_Results);
		}


		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_primaryVehicleReportImplementation.Initialize(modelData, fuelModes);
			_modelData = modelData;
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			_primaryVehicleReportImplementation.WriteResult(result);
		}

		public XNamespace Tns => _primaryVehicleReportImplementation.Tns;

		#endregion

		#region Implementation of IXMLMockupReport

		public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			var xElement = MockupResultReader.GetCIFMockupResult(Tns.NamespaceName, resultValue, Tns + "Result", _modelData);
			Results.Add(xElement);
		}

		public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.AddFirst(new XElement(Tns + "Status", "success"));
			Results.AddFirst(new XComment("Always prints success at the moment"));
		}

		public void GenerateReport(XElement fullReportHash)
		{
			_primaryVehicleReportImplementation.GenerateReport(fullReportHash);
		}

		public XDocument Report
		{
			get
			{
				var report = _primaryVehicleReportImplementation.Report;
				var resultsElement = report.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Results}']");
				resultsElement.First().ReplaceWith(Results);
				return report;
			}
		}

		#endregion
	}
}
