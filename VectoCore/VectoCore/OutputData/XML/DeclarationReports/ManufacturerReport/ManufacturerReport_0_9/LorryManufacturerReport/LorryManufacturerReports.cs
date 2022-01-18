using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.LorryManufacturerReport
{
    internal class ConventionalLorryManufacturerReport : AbstractManufacturerReport
    {


		public ConventionalLorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory)
		{
			
		}

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventionalLorryVehicleType().GetXmlType(inputData);
		}

		#endregion

	}

	internal class HEV_Px_IHPC_LorryManufacturerReport : AbstractManufacturerReport
	{
		public HEV_Px_IHPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHCP_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_S2_LorryManufacturerReport : AbstractManufacturerReport
	{
		public HEV_S2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_S3_LorryManufacturerReport : AbstractManufacturerReport
	{
		public HEV_S3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_S4_LorryManufacturerReport : AbstractManufacturerReport
	{
		public HEV_S4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_IEPC_S_LorryManufacturerReport : AbstractManufacturerReport
	{
		public HEV_IEPC_S_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_IEPC_S_LorryComponentsType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class PEV_E2_LorryManufacturerReport : AbstractManufacturerReport
	{
		public PEV_E2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E2_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class PEV_E3_LorryManufacturerReport : AbstractManufacturerReport
	{
		public PEV_E3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E3_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class PEV_E4_LorryManufacturerReport : AbstractManufacturerReport
	{
		public PEV_E4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E4_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class PEV_IEPC_LorryManufacturerReport : AbstractManufacturerReport
	{
		public PEV_IEPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}
}
