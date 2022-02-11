using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile
{
    public abstract class LorryCustomerInformationFile : AbstractCustomerReport
    {
		//protected XNamespace _cif = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		//public LorryManufacturerReportBase(IManufacturerReportFactory MRFReportFactory) : base(MRFReportFactory) { }

		//protected void GenerateReport(string outputDataType)
		//{
		//	Report = new XDocument(new XElement(_mrf + "VectoOutput",
		//		new XAttribute("xmlns", _mrf),
		//		new XAttribute(XNamespace.Xmlns + "xsi", xsi),
		//		new XAttribute(xsi + "type", $"{outputDataType}"),
		//		Vehicle,
		//		new XElement(_mrf + "Results")));
		//}

		protected LorryCustomerInformationFile(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }
	}

	public class ConventionalLorry_CIF : LorryCustomerInformationFile
	{
		public ConventionalLorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetConventionalLorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_PxLorry_CIF : LorryCustomerInformationFile
	{
		public HEV_PxLorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			var vehicle = _cifFactory.GetHEV_PxLorryVehicleType();
			Vehicle = _cifFactory.GetHEV_PxLorryVehicleType().GetElement(inputData);
		}
		#endregion
	}

	public class HEV_S2_Lorry_CIF : LorryCustomerInformationFile
	{
		public HEV_S2_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class HEV_S3_Lorry_CIF : LorryCustomerInformationFile
	{
		public HEV_S3_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class HEV_S4_Lorry_CIF : LorryCustomerInformationFile
	{
		public HEV_S4_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class HEV_IEPC_Lorry_CIF : LorryCustomerInformationFile
	{
		public HEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class PEV_E2_Lorry_CIF : LorryCustomerInformationFile
	{
		public PEV_E2_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class PEV_E3_Lorry_CIF : LorryCustomerInformationFile
	{
		public PEV_E3_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class PEV_E4_Lorry_CIF : LorryCustomerInformationFile
	{
		public PEV_E4_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class PEV_IEPC_Lorry_CIF : LorryCustomerInformationFile
	{
		public PEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
