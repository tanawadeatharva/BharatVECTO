using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class LorryManufacturerReportBase : AbstractManufacturerReport
	{
		public LorryManufacturerReportBase(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory)
		{

		}

		
	}

	internal class ConventionalLorryManufacturerReport : LorryManufacturerReportBase
	{
		

		public ConventionalLorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory)
		{
			
		}

		#region Overrides of AbstractManufacturerReport


		public override string OutputDataType =>
			XMLNames.MRF_OutputDataType_ConventionalLorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventionalLorryVehicleType().GetElement(inputData);
			
		}

		
		#endregion

	}

	internal class HEV_Px_IHPC_LorryManufacturerReport : LorryManufacturerReportBase
	{
		
		public HEV_Px_IHPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_Px_IHPCLorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHCP_LorryVehicleType().GetElement(inputData);
		}



		#endregion
	}

	internal class HEV_S2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_S2_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_S3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_S3_LorryManufacturerOutputDataType;

		public HEV_S3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory)
		{

		}

		#region Overrides of AbstractManufacturerReport

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_S4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_S4_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_IEPC_S_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_IEPC_S_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_IEPC_S_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_IEPC_S_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_F2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_F2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_F2_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_F2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_F3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_F3_LorryManufacturerOutputDataType;

		public HEV_F3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory)
			: base(MRFReportFactory, resultFactory)
		{
		}

		#region Overrides of AbstractManufacturerReport

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_F3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_F4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_F4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory)
			: base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_F4_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_F4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class HEV_IEPC_F_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_IEPC_F_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory)
			: base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_HEV_IEPC_F_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_IEPC_F_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class PEV_E2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_PEV_E2_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E2_LorryVehicleType().GetElement(inputData);
			
		}

		#endregion
	}

	internal class PEV_E3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_PEV_E3_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class PEV_E4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_PEV_E4_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class PEV_IEPC_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_IEPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => XMLNames.MRF_OutputDataType_PEV_IEPC_LorryManufacturerOutputDataType;

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_IEPC_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class Exempted_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public Exempted_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ExemptedLorryManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetExempted_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}
}
