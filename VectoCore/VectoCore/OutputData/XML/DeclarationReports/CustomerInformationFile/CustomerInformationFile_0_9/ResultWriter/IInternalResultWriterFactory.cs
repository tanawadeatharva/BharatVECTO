using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IInternalResultWriterFactory
	{
		IResultsWriter GetCIFResultsWriter(
			VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification vehicleClasiClassification);
	}
}