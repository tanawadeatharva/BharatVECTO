using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;

namespace TUGraz.VectoMockup.Reports
{
    public class MockupVehicleInformationFile : IXMLVehicleInformationFile, IXMLMockupReport
    {
		private XDocument _report;
		private XNamespace _tns;


		public MockupVehicleInformationFile(IXMLVehicleInformationFile vehicleInformationFile)
		{

		}



		//private XDocument _report;
		//private VectoRunData _modelData;


		//#region Implementation of IXMLMockupReport

		//public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		//{
		//	throw new NotImplementedException();
		//}

		//public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		//{
		//	throw new NotImplementedException();
		//}

		//#endregion

		//#region Implementation of IXMLMultistageReport

		//public void Initialize(VectoRunData modelData)
		//{
		//	_modelData = modelData;
		//}

		//public XDocument Report => _report;

		//public void GenerateReport()
		//{
		//	throw new NotImplementedException();
		//}

		//#endregion

		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData)
		{
			throw new NotImplementedException();
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			throw new NotImplementedException();
		}

		public void GenerateReport(XElement fullReportHash)
		{
			throw new NotImplementedException();
		}

		public XDocument Report => _report;

		public XNamespace Tns => _tns;

		#endregion

		#region Implementation of IXMLMockupReport

		public void WriteMockupResult(IResultEntry resultValue)
		{
			
		}

		public void WriteMockupSummary(IResultEntry resultValue)
		{
		}

		public void WriteExemptedResults()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
