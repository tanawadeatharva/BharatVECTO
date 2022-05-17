using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class LorryManufacturerReportBase : AbstractManufacturerReport
	{
		


		public LorryManufacturerReportBase(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory)
		{

		}

		protected void GenerateReport(string outputDataType)
		{
			Report = new XDocument(new XElement(Mrf + "VectoOutput",
				new XAttribute("xmlns", Mrf),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(xsi + "type", $"{outputDataType}"),
				Vehicle,
				new XElement(Mrf + "Results")));
		}
	}

	internal class ConventionalLorryManufacturerReport : LorryManufacturerReportBase
	{
		

		public ConventionalLorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory)
		{
			
		}

		#region Overrides of AbstractManufacturerReport


		public override string OutputDataType =>
			XMLNames.MRF_OutputDataType_ConventionalLorryManufacturerOutputDataType;

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventionalLorryVehicleType().GetElement(inputData);
			
		}

		
		#endregion

	}

	internal class HEV_Px_IHPC_LorryManufacturerReport : LorryManufacturerReportBase
	{
		
		public HEV_Px_IHPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-Px_IHPCLorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHCP_LorryVehicleType().GetElement(inputData);

			GenerateReport(OutputDataType);
		}



		#endregion
	}

	internal class HEV_S2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-S2_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_LorryVehicleType().GetElement(inputData);
			GenerateReport(OutputDataType);
		}

		#endregion
	}

	internal class HEV_S3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public override string OutputDataType => "HEV-S3_LorryManufacturerOutputDataType";

		public HEV_S3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory)
		{

		}

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_LorryVehicleType().GetElement(inputData);
			GenerateReport(OutputDataType);
		}

		#endregion
	}

	internal class HEV_S4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-S4_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_IEPC_S_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_IEPC_S_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEV-IEPC-S_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_IEPC_S_LorryVehicleType().GetElement(inputData);
			
		}

		#endregion
	}

	internal class PEV_E2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-E2_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E2_LorryVehicleType().GetElement(inputData);
			
		}

		#endregion
	}

	internal class PEV_E3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-E3_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class PEV_E4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-E4_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class PEV_IEPC_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_IEPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEV-IEPC_LorryManufacturerOutputDataType";

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_IEPC_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}
}
