using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoMockup.Reports
{
	public class XMLDeclarationMockupPrimaryReport : XMLDeclarationReportPrimaryVehicle_09
	{
		private readonly bool _exempted;

		public XMLDeclarationMockupPrimaryReport(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory, 
			IVIFReportFactory vifFactory,
			bool exempted,
			bool writePIF = false) : base(writer,
			mrfFactory,
			cifFactory,
			vifFactory,
			writePIF)
		{
			_exempted = exempted;
		}

		protected override void InstantiateReports(VectoRunData modelData)
		{
			base.InstantiateReports(modelData);
			//VehicleInformationFile = new MockupPrimaryVehicleInformationFile(VehicleInformationFile);
		}

		#region Overrides of XMLDeclarationReportPrimaryVehicle_09

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			//Do nothing
		}

		protected override void WriteResult(ResultEntry result)
		{
			(ManufacturerRpt as IXMLMockupReport).WriteMockupResult(result);
			(VehicleInformationFile as IXMLMockupReport).WriteMockupResult(result);
		}

		#endregion

		#region Overrides of XMLDeclarationReportPrimaryVehicle

		protected override void GenerateReports()
		{
			if (!_exempted) {
				(ManufacturerRpt as IXMLMockupReport).WriteMockupSummary(Results.First());
				(VehicleInformationFile as IXMLMockupReport).WriteMockupSummary(Results.First());
			} else {
				(ManufacturerRpt as IXMLMockupReport).WriteExemptedResults();
				(VehicleInformationFile as IXMLMockupReport).WriteExemptedResults();
			}

			ManufacturerRpt.GenerateReport();
			var fullReportHash = CreateDummySig();
			//CustomerRpt.GenerateReport(fullReportHash);
			VehicleInformationFile.GenerateReport(fullReportHash);
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