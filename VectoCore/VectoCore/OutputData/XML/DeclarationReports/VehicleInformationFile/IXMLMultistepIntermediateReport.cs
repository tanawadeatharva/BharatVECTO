using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile
{
	public interface IXMLMultistepIntermediateReport
	{
		void Initialize(VectoRunData modelData);
		XDocument Report { get; }
		void GenerateReport();
	}
}