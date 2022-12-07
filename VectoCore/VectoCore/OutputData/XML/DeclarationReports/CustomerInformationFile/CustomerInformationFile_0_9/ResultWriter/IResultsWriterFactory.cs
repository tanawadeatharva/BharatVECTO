using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriterFactory
	{
		IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
			bool exempted);
	}
}