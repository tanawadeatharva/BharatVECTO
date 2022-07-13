using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
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
				new XAttribute(xsi + XMLNames.XSIType, $"{outputDataType}"),
				Vehicle,
				new XElement(_mrf + "Results")));
		}
	}

	internal class Conventional_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
    {
		public Conventional_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ConventionalCompletedBusManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport(OutputDataType);

		}

		#endregion
	}

	internal class HEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public HEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType { get; }

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

		public override string OutputDataType => "PEV_CompletedBusManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport(OutputDataType);
		}

		#endregion
	}

	internal class Exempted_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public Exempted_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ExemptedCompletedBusManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
