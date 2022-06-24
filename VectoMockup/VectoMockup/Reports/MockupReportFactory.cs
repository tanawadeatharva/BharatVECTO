using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoMockup.Reports
{
    class MockupReportFactory : IXMLDeclarationReportFactory, IMockupDeclarationReportFactory
    {
        private readonly IManufacturerReportFactory _mrfFactory;
        private readonly ICustomerInformationFileFactory _cifFactory;
		private readonly IVIFReportFactory _vifFactory;


        #region Implementation of IXMLDeclarationReportFactory


        public MockupReportFactory(IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory)
        {
            _mrfFactory = mrfFactory;
            _cifFactory = cifFactory;
        }
        public IDeclarationReport CreateReport(IInputDataProvider input, IOutputDataWriter outputWriter)
        {
            switch (input)
            {
                case IMultistageBusInputDataProvider multistageBusInputDataProvider:
                    break;
                case ISingleBusInputDataProvider singleBusInputDataProvider:
                    return new XMLDeclarationReport(outputWriter);
                case IDeclarationInputDataProvider declarationInputDataProvider:
                    return CreateDeclarationReport(declarationInputDataProvider, outputWriter);
                case IMultiStageTypeInputData multiStageTypeInputData:
                    break;
                case IMultistageVIFInputData multistageVifInputData:
                    return CreateDeclarationReport(multistageVifInputData, outputWriter);
                case IPrimaryVehicleInformationInputDataProvider primaryVehicleInformationInputDataProvider:
                    break;
                case IVTPDeclarationInputDataProvider vtpProvider:
                    {
                        return null;
                    }
                case IMultistagePrimaryAndStageInputDataProvider multistagePrimaryAndStageInputData:
                    {
                        return null; //No Report for this type (handled in two simulation steps)
                    }
                default:
                    break;

            }
            throw new VectoException($"Could not create Declaration Report for {input.GetType()}");
        }

        public IVTPReport CreateVTPReport(IInputDataProvider input, IOutputDataWriter outputWriter)
        {
            return input is IVTPDeclarationInputDataProvider
                ? new XMLVTPReport(outputWriter)
                : null;
        }

        private IDeclarationReport CreateDeclarationReport(IMultistageVIFInputData multistageVifInputData, IOutputDataWriter outputDataWriter)
        {
			if (multistageVifInputData.VehicleInputData == null)
            {
                var reportCompleted = new XMLDeclarationMockupReportCompletedVehicle(outputDataWriter, _mrfFactory, _cifFactory, true)
                {
                    PrimaryVehicleReportInputData = multistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
                };
                return reportCompleted;
            }
            else {
				//throw new NotImplementedException();
                var report = new XMLDeclarationReportMultistageBusVehicle(outputDataWriter);
                return report;
            }
        }

        private IDeclarationReport CreateDeclarationReport(IDeclarationInputDataProvider declarationInputDataProvider,
            IOutputDataWriter outputDataWriter)
        {
            var vehicleCategory = declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory;
            if (vehicleCategory.IsLorry())
            {
                return new XMLDeclarationMockupReport(outputDataWriter, _mrfFactory, _cifFactory, declarationInputDataProvider.JobInputData.Vehicle.ExemptedVehicle);
            }

            if (vehicleCategory.IsBus())
            {
                switch (declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory)
                {
                    case VehicleCategory.HeavyBusCompletedVehicle:
                        throw new NotImplementedException();
                        return new XMLDeclarationReportCompletedVehicle(outputDataWriter,
                            declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory ==
                            VehicleCategory.HeavyBusPrimaryVehicle)
                        {
                            PrimaryVehicleReportInputData = declarationInputDataProvider.PrimaryVehicleData,
                        };
                    case VehicleCategory.HeavyBusPrimaryVehicle:
                        return new XMLDeclarationMockupPrimaryReport(outputDataWriter, _mrfFactory, _cifFactory,
							declarationInputDataProvider.JobInputData.Vehicle.ExemptedVehicle,
                            declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory ==
                            VehicleCategory.HeavyBusPrimaryVehicle);

                    default:
                        break;
                }
            }

            throw new Exception(
                $"Could not create DeclarationReport for Vehicle Category{vehicleCategory}");
        }

        public IVTPReport CreateVTPReport(IVTPDeclarationInputDataProvider input, IOutputDataWriter outputWriter)
        {
            throw new NotImplementedException();
        }

        #endregion

		#region Implementation of IMockupDeclarationReportFactory

		public IManufacturerReportFactory MrfFactory => _mrfFactory;
		public ICustomerInformationFileFactory CifFactory => _cifFactory;

		#endregion
	}

	internal interface IMockupDeclarationReportFactory
	{
		IManufacturerReportFactory MrfFactory { get; }
        ICustomerInformationFileFactory CifFactory { get; }
	}
}
