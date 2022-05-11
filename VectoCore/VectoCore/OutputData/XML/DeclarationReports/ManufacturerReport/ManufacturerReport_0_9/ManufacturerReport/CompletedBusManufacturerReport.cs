using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class CompletedBusManufacturerReportBase : AbstractManufacturerReport
	{
		protected XNamespace _mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		public CompletedBusManufacturerReportBase(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		protected void GenerateReport(string outputDataType)
		{
			Report = new XDocument(new XElement(_mrf + "VectoOutput",
				new XAttribute("xmlns", _mrf),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(xsi + "type", $"{outputDataType}"),
				Vehicle,
				new XElement(_mrf + "Results")));
		}
	}

	internal class Conventional_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
    {
		public Conventional_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string XmlOutputType { get; }

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport("ConventionalCompletedBusManufacturerOutputDataType");

		}

		#endregion
	}

	internal class HEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public HEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string XmlOutputType { get; }

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport("HEVCompletedBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class PEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public PEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string XmlOutputType { get; }

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport("PEVCompletedBusManufacturerOutputDataType");
		}

		#endregion
	}
}
