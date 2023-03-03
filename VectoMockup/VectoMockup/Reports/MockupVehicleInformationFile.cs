using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoMockup.Reports
{
	internal class MockupInterimVehicleInformationFile : IXMLMultistepIntermediateReport, IXMLMockupReport
	{
		private readonly IXMLMultistepIntermediateReport _intermediateVifImplementation;

		public MockupInterimVehicleInformationFile(IXMLMultistepIntermediateReport vifImplementation)
		{
			_intermediateVifImplementation = vifImplementation;
		}

		#region Implementation of IXMLMultistepIntermediateReport

		public void Initialize(VectoRunData modelData)
		{
			_intermediateVifImplementation.Initialize(modelData);
		}

		public XDocument Report => _intermediateVifImplementation.Report;

		public void GenerateReport()
		{
			_intermediateVifImplementation.GenerateReport();
		}

		#endregion

		#region Implementation of IXMLMockupReport

		public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			
		}

		public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		{
			
		}

		public void WriteExemptedResults()
		{
			
		}

		#endregion
	}


	internal class MockupPrimaryVehicleInformationFile : IXMLVehicleInformationFile, IXMLMockupReport
    {
		
		private readonly IXMLVehicleInformationFile _vehicleInformationFileImplementation;
		private VectoRunData _modelData;

		private XElement Results;
		public MockupPrimaryVehicleInformationFile(IXMLVehicleInformationFile vehicleInformationFileImplementation)
		{
			_vehicleInformationFileImplementation = vehicleInformationFileImplementation;
			Results = new XElement(_vehicleInformationFileImplementation.Tns + XMLNames.Report_Results);
		}


		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData)
		{
			_vehicleInformationFileImplementation.Initialize(modelData);
			_modelData = modelData;
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			_vehicleInformationFileImplementation.WriteResult(result);
		}

		public XNamespace Tns => _vehicleInformationFileImplementation.Tns;

		#endregion

		#region Implementation of IXMLMockupReport

		public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			var xElement = MockupResultReader.GetVIFMockupResult(Tns.NamespaceName, resultValue, Tns + "Result", _modelData);
			Results.Add(xElement);
		}

		public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.AddFirst(new XElement(Tns + "Status", "success"));
			Results.AddFirst(new XComment("Always prints success at the moment"));
		}

		public void WriteExemptedResults()
		{
			Results.Add(new XElement(Tns + "Status", "success"));
			Results.Add(new XElement(Tns + "ExemptedVehicle"));
		}
		

		public void GenerateReport(XElement fullReportHash)
		{ 
			_vehicleInformationFileImplementation.GenerateReport(fullReportHash);
		}

		public XDocument Report
		{
			get
			{
				var report = _vehicleInformationFileImplementation.Report;
				var resultsElement = report.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Results}']");
				resultsElement.First().ReplaceWith(Results);
				return report;
			}
		}

		#endregion
	}
}
