using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public interface IInternalResultWriterFactory
    {
        IResultsWriter GetCIFResultsWriter(
            VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification vehicleClasiClassification);

		IResultsWriter GetMRFResultsWriter(
			VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification vehicleClasiClassification);

		IResultsWriter GetVIFResultsWriter(
			VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification vehicleClasiClassification);

    }
}