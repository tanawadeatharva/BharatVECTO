using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile
{
	public interface IXMLVehicleInformationFile
	{
		void Initialize(VectoRunData modelData);
		void WriteResult(XMLDeclarationReport.ResultEntry result);
		void GenerateReport(XElement fullReportHash);
		XDocument Report { get; }
		XNamespace Tns { get; }
	}
}