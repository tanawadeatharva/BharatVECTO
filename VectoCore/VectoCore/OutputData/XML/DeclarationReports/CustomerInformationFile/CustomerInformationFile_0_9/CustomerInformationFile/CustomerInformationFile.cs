using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;

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
		public ConventionalLorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetConventionalLorryVehicleType().GetElement(inputData);
			GenerateReport("ConventionalLorryOutputType");
		}

		#endregion
	}

	public class HEV_PxLorry_CIF : CustomerInformationFile
	{
		public HEV_PxLorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			var vehicle = _cifFactory.GetHEV_PxLorryVehicleType();
			Vehicle = _cifFactory.GetHEV_PxLorryVehicleType().GetElement(inputData);
			GenerateReport("HEV_Px_LorryOutputType");
		}
		#endregion
	}

	public class HEV_S2_Lorry_CIF : CustomerInformationFile
	{
		public HEV_S2_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S2_LorryVehicleType().GetElement(inputData);
			GenerateReport("HEV_S2_LorryOutputType");
		}

		#endregion
	}

	public class HEV_S3_Lorry_CIF : CustomerInformationFile
	{
		public HEV_S3_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S3_LorryVehicleType().GetElement(inputData);
			GenerateReport("HEV_S3_LorryOutputType");
		}

		#endregion
	}

	public class HEV_S4_Lorry_CIF : CustomerInformationFile
	{
		public HEV_S4_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S4_LorryVehicleType().GetElement(inputData);
			GenerateReport("HEV_S4_LorryOutputType");
		}

		#endregion
	}

	public class HEV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public HEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_IEPC_LorryVehicleType().GetElement(inputData);
			GenerateReport("HEV_IEPC_S_LorryOutputType");
		}

		#endregion
	}

	public class PEV_E2_Lorry_CIF : CustomerInformationFile
	{
		public PEV_E2_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E2_LorryVehicleType().GetElement(inputData);
			GenerateReport("PEV_E2_LorryOutputType");
		}

		#endregion
	}

	public class PEV_E3_Lorry_CIF : CustomerInformationFile
	{
		public PEV_E3_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E3_LorryVehicleType().GetElement(inputData);
			GenerateReport("PEV_E3_LorryOutputType");
		}

		#endregion
	}

	public class PEV_E4_Lorry_CIF : CustomerInformationFile
	{
		public PEV_E4_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E4_LorryVehicleType().GetElement(inputData);
			GenerateReport("PEV_E4_LorryOutputType");
		}

		#endregion
	}

	public class PEV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public PEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_IEPC_LorryVehicleType().GetElement(inputData);
			GenerateReport("PEV_IEPC_LorryOutputType");
		}

		#endregion
	}

    #endregion









    #region CompletedBus
    public class Conventional_CompletedBusCIF : CustomerInformationFile
    {
        public Conventional_CompletedBusCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory)
        {
        }

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
			Vehicle = _cifFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport("Conventional_CompletedBusOutputType");
        }
    }


	public class HEV_CompletedBusCIF : CustomerInformationFile
	{
		public HEV_CompletedBusCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport("HEV_CompletedBusOutputType");
		}
	}

	public class PEV_CompletedBusCIF : CustomerInformationFile
	{
		public PEV_CompletedBusCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_CompletedBusVehicleType().GetElement(inputData);
			GenerateReport("PEV_CompltedBusOutputType");
		}
	}

	#endregion
}
