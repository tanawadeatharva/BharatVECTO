using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class PrimaryBus_ManufacturerReportBase : AbstractManufacturerReport
	{
		protected XNamespace _mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		protected PrimaryBus_ManufacturerReportBase(IManufacturerReportFactory mrfFactory, IResultsWriterFactory resultFactory) : base(mrfFactory, resultFactory) { }

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

	internal class Conventional_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
    {
		public Conventional_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_ConventionalPrimaryBusManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_PrimaryBusVehicleType().GetElement(inputData);
			
		}

		#endregion
	}

	internal class HEV_Px_IHPC_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_Px_IHPC_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-Px_IHPCPrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHPC_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}


	internal class HEV_S2_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_S2_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-S2_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_S3_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_S3_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-S3_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_S4_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_S4_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-S4_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_PrimaryBusVehicleType().GetElement(inputData);

		}

		#endregion
	}


	internal class HEV_IEPC_S_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_IEPC_S_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-IEPC-S_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_IEPC_S_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class PEV_E2_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_E2_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-E2_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E2_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}


	internal class PEV_E3_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_E3_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-E3_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E3_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}


	internal class PEV_E4_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_E4_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(
			MRFReportFactory, resultFactory)
		{

		}

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-E4_PrimaryBusManufacturerOutputDataType";
		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E4_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}


	internal class PEV_IEPC_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_IEPC_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-IEPC_PrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_IEPC_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class Exempted_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public Exempted_PrimaryBus_ManufacturerReport(IManufacturerReportFactory mrfFactory, IResultsWriterFactory resultFactory) : base(mrfFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ExemptedPrimaryBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetExempted_PrimaryBusVehicleType().GetElement(inputData);
		}

		#endregion
	}
}
