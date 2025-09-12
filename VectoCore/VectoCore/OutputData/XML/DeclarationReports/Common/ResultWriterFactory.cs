using System;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public class ResultWriterFactory : IResultsWriterFactory
    {
        //private readonly IInternalResultWriterFactory _internalFactory;
		private readonly IKernel _kernel;

        public ResultWriterFactory(IKernel kernel)
        {
			_kernel = kernel;
            //_internalFactory = kernel.Get<IInternalResultWriterFactory>();
        }

        #region Implementation of IResultsWriterFactory

        public IResultsWriter GetCIFResultsWriter(IDeclarationInputDataProvider inputData, string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
        {
            try {
				var internalFactory = _kernel.Get<IInternalResultWriterFactory>(ResultWriterNamingHelper.GetResultWriterName(inputData));
;                return internalFactory.GetCIFResultsWriter(
                    new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(
						XmlDocumentType.CustomerReport, vehicleCategory,
                        jobType.GetPowertrainArchitectureType(), ovc, exempted));
            } catch (Exception e) {
                throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
            }
        }

		public IResultsWriter GetMRFResultsWriter(IDeclarationInputDataProvider inputData, string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			try {
				var internalFactory = _kernel.Get<IInternalResultWriterFactory>(ResultWriterNamingHelper.GetResultWriterName(inputData));
                var resultsVehicleClassification = new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(
						XmlDocumentType.ManufacturerReport, vehicleCategory,
						jobType.GetPowertrainArchitectureType(), ovc, exempted);
				return internalFactory.GetMRFResultsWriter(resultsVehicleClassification);
			} catch (Exception e) {
				throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
			}
        }

		public IResultsWriter GetVIFResultsWriter(IDeclarationInputDataProvider inputData, string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			try {
				var internalFactory = _kernel.Get<IInternalResultWriterFactory>(ResultWriterNamingHelper.GetResultWriterName(inputData));
                return internalFactory.GetVIFResultsWriter(
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