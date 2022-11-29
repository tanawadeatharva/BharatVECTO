using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport
{
	public interface IXMLManufacturerReport
	{
		//void InitializeVehicleData(IDeclarationInputDataProvider inputData);
		void Initialize(VectoRunData modelData);
		XDocument Report { get; }
		void WriteResult(XMLDeclarationReport.ResultEntry resultValue);
		void GenerateReport();
	}
}