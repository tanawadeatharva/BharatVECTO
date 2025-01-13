using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
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
                    new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(
						XmlDocumentType.CustomerReport, vehicleCategory,
                        jobType.GetPowertrainArchitectureType(), ovc, exempted));
            } catch (Exception e) {
                throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
            }
        }

		public IResultsWriter GetMRFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			try {
				var resultsVehicleClassification = new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(
						XmlDocumentType.ManufacturerReport, vehicleCategory,
						jobType.GetPowertrainArchitectureType(), ovc, exempted);
				return _internalFactory.GetMRFResultsWriter(resultsVehicleClassification);
			} catch (Exception e) {
				throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
			}
        }

		public IResultsWriter GetVIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			try {
				return _internalFactory.GetVIFResultsWriter(
					new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(
						XmlDocumentType.MultistepOutputData, vehicleCategory,
						jobType.GetPowertrainArchitectureType(), ovc, exempted));
			} catch (Exception e) {
				throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
			}
		}

		#endregion
    }
}