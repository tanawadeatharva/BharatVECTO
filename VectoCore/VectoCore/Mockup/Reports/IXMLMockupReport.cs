using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoMockup.Reports
{
	internal interface IXMLMockupReport
	{
		void WriteMockupResult(IResultEntry resultValue);
		void WriteMockupSummary(IResultEntry resultValue);
		void WriteExemptedResults();
	}
}