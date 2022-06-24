using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class AbstractVIFReport : IXMLPrimaryVehicleReport
	{
		private XDocument _report;
		private XNamespace _tns;

		protected readonly IVIFReportFactory _vifFactory;

		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		public static XNamespace VIF => XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

		public abstract string OutputDataType { get; }


		protected XElement Vehicle { get; set; }
		protected XElement Results { get; set; }
	

		protected AbstractVIFReport(IVIFReportFactory vifFactory)
		{
			_vifFactory = vifFactory;
		}
		
		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);
		
		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			InitializeVehicleData(modelData.InputData);
			Results = new XElement(VIF + XMLNames.Report_Results);
		}

		private List<XMLDeclarationReport.ResultEntry> results = new List<XMLDeclarationReport.ResultEntry>();
		public void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			results.Add(result);
		}

		public void GenerateReport(XElement fullReportHash)
		{
			//ToDo add missing namespaces and 
			Report = new XDocument(new XElement(VIF + "VectoOutputMultistep",
					new XAttribute("xmlns", VIF),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi)),
				Vehicle,
				Results
			);
		}

		public XDocument Report { get; protected set; }

		public XNamespace Tns => _tns;

		#endregion
	}
}
