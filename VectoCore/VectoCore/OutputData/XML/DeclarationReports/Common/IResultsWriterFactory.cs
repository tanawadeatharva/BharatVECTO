using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public interface IResultsWriterFactory
    {
        IResultsWriter GetCIFResultsWriter(IDeclarationInputDataProvider inputData, string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
            bool exempted);

		IResultsWriter GetMRFResultsWriter(IDeclarationInputDataProvider inputData, string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
			bool exempted);

		IResultsWriter GetVIFResultsWriter(IDeclarationInputDataProvider inputData, string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
			bool exempted);

    }
}