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
		void WriteResult(IResultEntry resultValue);
		void GenerateReport();
	}

	public interface IXMLManufacturerReportCompletedBus
	{
		void WriteResult(XMLDeclarationReport.ResultEntry genericResult,
			XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult);
	}
}