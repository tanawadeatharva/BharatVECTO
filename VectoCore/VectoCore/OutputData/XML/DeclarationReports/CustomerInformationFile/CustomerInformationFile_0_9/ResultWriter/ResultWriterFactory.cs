using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public class ResultWriterFactory : IResultsWriterFactory
	{
		private readonly IInternalResultWriterFactory _internalFactory;

		public ResultWriterFactory(IInternalResultWriterFactory internalFactory)
		{
			_internalFactory = internalFactory;
		}

		#region Implementation of IResultsWriterFactory

		public IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			try {
				return _internalFactory.GetCIFResultsWriter(
					new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(vehicleCategory,
						jobType, ovc, exempted));
			} catch (Exception e) {
				throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
			}
		}

		#endregion
	}
}