using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.LorryManufacturerReport
{
    internal class Conventional_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
    {
		public Conventional_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_PrimaryBusVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_Px_IHPC_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public HEV_Px_IHPC_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHPC_PrimaryBusVehicleType().GetXmlType(inputData);
		}

		#endregion
	}


	internal class HEV_S2_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public HEV_S2_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_PrimaryBusVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_S3_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public HEV_S3_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_PrimaryBusVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class HEV_S4_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public HEV_S4_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_PrimaryBusVehicleType().GetXmlType(inputData);
		}

		#endregion
	}


	internal class HEV_IEPC_S_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public HEV_IEPC_S_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotFiniteNumberException();
		}

		#endregion
	}

	internal class PEV_E2_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public PEV_E2_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}


	internal class PEV_E3_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public PEV_E3_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}


	internal class PEV_E4_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public PEV_E4_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}


	internal class PEV_IEPC_PrimaryBus_ManufacturerReport : AbstractManufacturerReport
	{
		public PEV_IEPC_PrimaryBus_ManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}



}
