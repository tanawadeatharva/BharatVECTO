using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_1_0;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoMockup.Reports
{
	/// <summary>
	/// Create MRF and CIF for lorries
	/// </summary>
	internal class XMLDeclarationMockupReport : XMLDeclarationReport
    {
		private readonly bool _exempted;

		public XMLDeclarationMockupReport(IReportWriter writer, IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory, bool exempted) :
			base(writer, mrfFactory, cifFactory)
		{
			_exempted = exempted;
		}

		#region Overrides of XMLDeclarationReport09

		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			//Do nothing;
		}

		protected override void WriteResult(ResultEntry result)
		{
			ManufacturerRpt.WriteResult(result);
			CustomerRpt.WriteResult(result);
		}


		#endregion
	}
}
