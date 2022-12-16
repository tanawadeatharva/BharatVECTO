using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public interface IResultsWriterFactory
    {
        IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
            bool exempted);

		IResultsWriter GetMRFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
			bool exempted);

		IResultsWriter GetVIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
			bool exempted);

    }
}