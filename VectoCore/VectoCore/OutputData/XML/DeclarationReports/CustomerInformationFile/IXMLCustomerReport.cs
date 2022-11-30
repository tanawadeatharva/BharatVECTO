using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile
{
	public interface IXMLCustomerReport
	{
		void Initialize(VectoRunData modelData);
		XDocument Report { get; }
		void WriteResult(XMLDeclarationReport.ResultEntry resultValue);
		void GenerateReport(XElement resultSignature);
	}

	public interface IXMLCustomerReportCompletedBus
	{
		void WriteResult(XMLDeclarationReport.ResultEntry genericResult,
			XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult);
	}
}