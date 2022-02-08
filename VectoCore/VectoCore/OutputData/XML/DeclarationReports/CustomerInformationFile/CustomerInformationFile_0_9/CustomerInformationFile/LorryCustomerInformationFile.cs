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

		public override void InitializeVehicleData(IVehicleDeclarationInputData inputData)
		{
			Vehicle = _cifFactory.GetConventionalLorryVehicleType(inputData);
		}

		#endregion
	}
}
