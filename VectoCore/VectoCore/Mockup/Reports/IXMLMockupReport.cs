using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoMockup.Reports
{
	internal interface IXMLMockupReport
	{
		void WriteMockupResult(XMLDeclarationReport.ResultEntry resultValue);
		void WriteMockupSummary(XMLDeclarationReport.ResultEntry resultValue);
		void WriteExemptedResults();
	}
}