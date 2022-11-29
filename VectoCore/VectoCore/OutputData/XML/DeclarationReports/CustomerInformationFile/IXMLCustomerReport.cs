using System.Xml.Linq;
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
}