using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{

	internal interface IXMLMockupReport
	{
		void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue);
	}
	internal abstract class AbstractManufacturerReport : IXMLManufacturerReport, IXMLMockupReport
    {
        protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		public static XNamespace Mrf => XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");


		protected readonly IManufacturerReportFactory _mRFReportFactory;

		protected XElement Results { get; set; }
		protected XElement Vehicle { get; set; }

		public abstract string OutputDataType { get; } //also used as name for the mockup result element

		protected AbstractManufacturerReport(IManufacturerReportFactory MRFReportFactory)
		{
			_mRFReportFactory = MRFReportFactory;
		}

		#region Implementation of IXMLManufacturerReport

		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			InitializeVehicleData(modelData.InputData);
			Results = new XElement(Mrf + XMLNames.Report_Results);
		}

		public XDocument Report { get; protected set; }

		private List<XMLDeclarationReport.ResultEntry> results = new List<XMLDeclarationReport.ResultEntry>();
		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			

		}


		public void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue)
		{

			Results.Add(MockupResultReader.GetMRFMockupResult(OutputDataType, resultValue, Mrf + "Result"));



		}

		

		public void GenerateReport()
		{
			Report = new XDocument(new XElement(Mrf + "VectoOutput",
					new XAttribute("xmlns", Mrf),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi),
					new XAttribute(xsi + "type", $"{OutputDataType}"),
					Vehicle,
					Results));
			
		}

		#endregion
	}

}
