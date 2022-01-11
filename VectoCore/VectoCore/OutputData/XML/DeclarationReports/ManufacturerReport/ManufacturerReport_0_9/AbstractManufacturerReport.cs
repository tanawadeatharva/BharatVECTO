using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{
    internal abstract class AbstractManufacturerReport : IXMLManufacturerReport
    {
        protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		//protected XNamespace mrf = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9";
		
		protected readonly IManufacturerReportFactory _mRFReportFactory;

		protected readonly XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());


		public XElement Vehicle { get; protected set; }





		public AbstractManufacturerReport(IManufacturerReportFactory MRFReportFactory)
		{
			_mRFReportFactory = MRFReportFactory;
		}

		#region Implementation of IXMLManufacturerReport

		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			throw new NotImplementedException();
		}

		public XDocument Report { get; }
		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			throw new NotImplementedException();
		}

		public void GenerateReport()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
