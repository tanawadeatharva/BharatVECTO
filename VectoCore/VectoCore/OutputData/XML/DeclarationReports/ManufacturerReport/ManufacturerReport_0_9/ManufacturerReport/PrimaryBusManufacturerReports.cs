using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class PrimaryBus_ManufacturerReportBase : AbstractManufacturerReport
	{
		protected XNamespace _mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		protected string _mrfPrefix = "mrf";
		protected PrimaryBus_ManufacturerReportBase(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		protected void GenerateReport(string outputDataType)
		{
			Report = new XDocument(new XElement(_mrf + "VectoOutput",
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(xsi + "type", $"{_mrfPrefix}:{outputDataType}"),
				new XAttribute(XNamespace.Xmlns + _mrfPrefix, _mrf),
				Vehicle,
				new XElement(_mrf + "Results")));
		}
	}

	internal class Conventional_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
    {
		public Conventional_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_PrimaryBusVehicleType().GetXmlType(inputData);

			GenerateReport("ConventionalPrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class HEV_Px_IHPC_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_Px_IHPC_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHPC_PrimaryBusVehicleType().GetXmlType(inputData);


			GenerateReport("HEV-Px_IHPCPrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}


	internal class HEV_S2_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_S2_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_PrimaryBusVehicleType().GetXmlType(inputData);

			GenerateReport("HEV-S2_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class HEV_S3_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_S3_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_PrimaryBusVehicleType().GetXmlType(inputData);
			GenerateReport("HEV-S3_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class HEV_S4_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_S4_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_PrimaryBusVehicleType().GetXmlType(inputData);
			GenerateReport("HEV-S4_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}


	internal class HEV_IEPC_S_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public HEV_IEPC_S_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	internal class PEV_E2_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_E2_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E2_PrimaryBusVehicleType().GetXmlType(inputData);
			GenerateReport("PEV-E2_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}


	internal class PEV_E3_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_E3_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E3_PrimaryBusVehicleType().GetXmlType(inputData);
			GenerateReport("PEV-E3_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}


	internal class PEV_E4_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_E4_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(
			MRFReportFactory)
		{

		}

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E4_PrimaryBusVehicleType().GetXmlType(inputData);
			GenerateReport("PEV-E4_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}


	internal class PEV_IEPC_PrimaryBus_ManufacturerReport : PrimaryBus_ManufacturerReportBase
	{
		public PEV_IEPC_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_IEPC_PrimaryBusVehicleType().GetXmlType(inputData);
			GenerateReport("PEV-IEPC_PrimaryBusManufacturerOutputDataType");
		}

		#endregion
	}



}
