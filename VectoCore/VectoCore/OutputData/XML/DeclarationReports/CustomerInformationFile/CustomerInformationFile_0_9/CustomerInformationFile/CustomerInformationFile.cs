using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile
{
    public abstract class CustomerInformationFile : AbstractCustomerReport
    {
        public static XNamespace Cif => XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9");

		protected void GenerateReport(string outputDataType)
		{
			Report = new XDocument(new XElement(Cif + "VectoOutput",
				new XAttribute("xmlns", Cif),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(XNamespace.Xmlns + "mrf", LorryManufacturerReportBase.Mrf),
				new XAttribute(xsi + "type", $"{outputDataType}"),
				Vehicle));
			//;new XElement(Cif + "Results")));
		}

        protected CustomerInformationFile(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }
	}
    #region LorryCIF
    public class ConventionalLorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_ConventionalLorryOutputType;
		public ConventionalLorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetConventionalLorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_PxLorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_HEV_Px_LorryOutputType;
		public HEV_PxLorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			var vehicle = _cifFactory.GetHEV_PxLorryVehicleType();
			Vehicle = _cifFactory.GetHEV_PxLorryVehicleType().GetElement(inputData);
		}
		#endregion
	}

	public class HEV_S2_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_HEV_S2_LorryOutputType;
		public HEV_S2_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_S3_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_HEV_S3_LorryOutputType;
		public HEV_S3_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_S4_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_HEV_S4_LorryOutputType;
		public HEV_S4_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_HEV_IEPC_S_LorryOutputType;
		public HEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_IEPC_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class PEV_E2_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_PEV_E2_LorryOutputType;
		public PEV_E2_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class PEV_E3_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_PEV_E3_LorryOutputType;
		public PEV_E3_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class PEV_E4_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_PEV_E4_LorryOutputType;
		public PEV_E4_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E4_LorryVehicleType().GetElement(inputData);

		}

		#endregion
	}

	public class PEV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_PEV_IEPC_LorryOutputType;
		public PEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_IEPC_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

    #endregion









    #region CompletedBus
    public class Conventional_CompletedBusCIF : CustomerInformationFile
	{
		public override string OutputDataType => "Conventional_CompletedBusOutputType";

		public Conventional_CompletedBusCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory)
        {
        }

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
			Vehicle = _cifFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
		}
    }


	public class HEV_CompletedBusCIF : CustomerInformationFile
	{
		public override string OutputDataType => "HEV_CompletedBusOutputType";
		public HEV_CompletedBusCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class PEV_CompletedBusCIF : CustomerInformationFile
	{
		public override string OutputDataType => "PEV_CompletedBusOutputType";
		public PEV_CompletedBusCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	#endregion
}
