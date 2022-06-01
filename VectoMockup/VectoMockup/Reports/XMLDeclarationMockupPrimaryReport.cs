using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoMockup.Reports
{
	public class XMLDeclarationMockupPrimaryReport : XMLDeclarationReportPrimaryVehicle_09
	{
		public XMLDeclarationMockupPrimaryReport(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			bool writePIF = false) : base(writer,
			mrfFactory,
			cifFactory,
			writePIF)
		{








		}

		protected override void InstantiateReports(VectoRunData modelData)
		{
			base.InstantiateReports(modelData);
			PrimaryReport = new MockupPrimaryReport(PrimaryReport);
		}

		#region Overrides of XMLDeclarationReportPrimaryVehicle_09

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			//Do nothing
		}

		protected override void WriteResult(ResultEntry result)
		{
			(ManufacturerRpt as IXMLMockupReport).WriteMockupResult(result);
			(PrimaryReport as IXMLMockupReport).WriteMockupResult(result);
		}

		#endregion

		#region Overrides of XMLDeclarationReportPrimaryVehicle

		protected override void GenerateReports()
		{
			(ManufacturerRpt as IXMLMockupReport).WriteMockupSummary(Results.First());
			(PrimaryReport as IXMLMockupReport).WriteMockupSummary(Results.First());
			ManufacturerRpt.GenerateReport();
			var fullReportHash = CreateDummySig();
			//CustomerRpt.GenerateReport(fullReportHash);
			PrimaryReport.GenerateReport(fullReportHash);
		}

		protected virtual XElement CreateDummySig()
		{
			XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
			return new XElement(
				di + XMLNames.DI_Signature_Reference,
				new XElement(
					di + XMLNames.DI_Signature_Reference_DigestMethod,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "null")),
				new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "NOT AVAILABLE")
			);
		}
	}





		#endregion
}