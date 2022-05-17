using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
	public abstract class AbstractCustomerReport : IXMLCustomerReport, IXMLMockupReport
    {
		protected readonly ICustomerInformationFileFactory _cifFactory;
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		public static XNamespace Cif => XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9");
		protected XElement Vehicle { get; set; }
		protected XElement Results { get; set; }

		protected abstract string OutputDataType { get; }

		private bool _ovc = false;
		protected AbstractCustomerReport(ICustomerInformationFileFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}


		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

		#region Implementation of IXMLCustomerReport

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.Ocv;
			Results = new XElement(Cif + "Results");
		}

		public XDocument Report { get; protected set; }

		private List<XMLDeclarationReport.ResultEntry> results = new List<XMLDeclarationReport.ResultEntry>();
		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			results.Add(resultValue);

		}

		public void GenerateReport(XElement resultSignature)
		{
			Report = new XDocument(new XElement(Cif + "VectoOutput",
				new XAttribute("xmlns", Cif),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(XNamespace.Xmlns + "mrf", LorryManufacturerReportBase.Mrf),
				new XAttribute(xsi + "type", $"{OutputDataType}"),
				Vehicle,
				Results));
		}

		#endregion

		#region Implementation of IXMLMockupReport

		public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.Add(MockupResultReader.GetCIFMockupResult(OutputDataType, resultValue, Cif + "Result", _ovc));
		}

		public void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue)
		{
			Results.AddFirst(new XElement(Cif + "Status", "success"));
			Results.AddFirst(new XComment("Always prints success at the moment"));
			Results.Add(MockupResultReader.GetCIFMockupResult(OutputDataType, resultValue, Cif + "Summary", _ovc));
		}

		#endregion
	}
}
