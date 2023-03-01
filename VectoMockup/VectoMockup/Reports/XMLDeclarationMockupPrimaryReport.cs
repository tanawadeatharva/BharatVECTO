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

	/// <summary>
	/// Create MRF and VIF for primary bus
	/// </summary>
	public class XMLDeclarationMockupPrimaryReport : XMLDeclarationReportPrimaryVehicle
	{
		private readonly bool _exempted;

		public XMLDeclarationMockupPrimaryReport(IReportWriter writer,
			IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory, 
			IVIFReportFactory vifFactory,
			bool exempted) : base(writer,
			mrfFactory,
			vifFactory)
		{
			_exempted = exempted;
		}

		#region Overrides of XMLDeclarationReportPrimaryVehicle

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
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			VehicleInformationFile.GenerateReport(fullReportHash);
		}

	}

}