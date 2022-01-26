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
	internal abstract class LorryManufacturerReportBase : AbstractManufacturerReport
	{
		protected XNamespace _mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		protected string _mrfPrefix = "mrf";
		public LorryManufacturerReportBase(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

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

	internal class ConventionalLorryManufacturerReport : LorryManufacturerReportBase
	{

		public ConventionalLorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory)
		{
			
		}

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventionalLorryVehicleType().GetXmlType(inputData);

			GenerateReport("ConventionalLorryManufacturerOutputDataType");
		}

		
		#endregion

	}

	internal class HEV_Px_IHPC_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_Px_IHPC_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_Px_IHCP_LorryVehicleType().GetXmlType(inputData);

			//TODO: REMOVE
			GenerateReport("HEV-Px_IHPCLorryManufacturerOutputDataType");
		}



		#endregion
	}

	internal class HEV_S2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S2_LorryVehicleType().GetXmlType(inputData);
			GenerateReport("HEV-S2_LorryManufacturerOutputDataType");
		}

		#endregion
	}

	internal class HEV_S3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S3_LorryVehicleType().GetXmlType(inputData);
			GenerateReport("HEV-S3_LorryManufacturerOutputDataType");
		}

		#endregion
	}

	internal class HEV_S4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_S4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_S4_LorryVehicleType().GetXmlType(inputData);
			GenerateReport("HEV-S4_LorryManufacturerOutputDataType");
		}

		#endregion
	}

	internal class HEV_IEPC_S_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public HEV_IEPC_S_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_IEPC_S_LorryComponentsType().GetXmlType(inputData);
			GenerateReport("HEV-IEPC-S_LorryManufacturerOutputDataType");
		}

		#endregion
	}

	internal class PEV_E2_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E2_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E2_LorryVehicleType().GetXmlType(inputData);

		}

		#endregion
	}

	internal class PEV_E3_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E3_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E3_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class PEV_E4_LorryManufacturerReport : LorryManufacturerReportBase
	{
		public PEV_E4_LorryManufacturerReport(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_E4_LorryVehicleType().GetXmlType(inputData);
		}

		#endregion
	}

	internal class PEV_IEPC_LorryManufacturerReport : LorryManufacturerReportBase
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
