using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.OutputData;


namespace TUGraz.VectoMockup.Simulation.RundataFactories
{
    public class VectoMockUpRunDataFactoryFactory : IVectoRunDataFactoryFactory
    {
        #region Implementation of IVectoRunDataFactoryFactory



        public IVectoRunDataFactory CreateDeclarationRunDataFactory(IInputDataProvider inputDataProvider, IDeclarationReport report,
            IVTPReport vtpReport)
        {
            if (inputDataProvider == null)
                throw new ArgumentNullException(nameof(inputDataProvider));

            switch (inputDataProvider)
            {
                case IVTPDeclarationInputDataProvider vtpProvider:
                    return CreateRunDataReader(vtpProvider, vtpReport);
                case ISingleBusInputDataProvider singleBusProvider:
                    return CreateRunDataReader(singleBusProvider, report);
                case IDeclarationInputDataProvider declDataProvider:
                    return CreateRunDataReader(declDataProvider, report);
                case IMultistageVIFInputData multistageVifInputData:
                    return CreateRunDataReader(multistageVifInputData, report);
                default:
                    break;
            }
            throw new VectoException("Unknown InputData for Declaration Mode!");
        }

        private IVectoRunDataFactory CreateRunDataReader(IMultistageVIFInputData multistageVifInputData, IDeclarationReport report)
        {
			if (multistageVifInputData.VehicleInputData == null)
			{
				return new MockupMultistageCompletedBusRunDataFactory(
					multistageVifInputData.MultistageJobInputData,
					report);
			}
			else {
				return new DeclarationModeMultistageBusVectoRunDataFactory(multistageVifInputData, report);
			}
		}

        private IVectoRunDataFactory CreateRunDataReader(IDeclarationInputDataProvider declDataProvider, IDeclarationReport report)
        {
            var vehicleCategory = declDataProvider.JobInputData.Vehicle.VehicleCategory;
            if (vehicleCategory.IsLorry())
            {
                return new MockupLorryVectoRunDataFactory(declDataProvider, report);
            }

            if (vehicleCategory.IsBus())

                switch (declDataProvider.JobInputData.Vehicle.VehicleCategory)
                {
                    case VehicleCategory.HeavyBusCompletedVehicle:
                        throw new NotImplementedException();
                        return new DeclarationModeCompletedBusVectoRunDataFactory(declDataProvider, report);
                    case VehicleCategory.HeavyBusPrimaryVehicle:
                        return new PrimaryBusMockupRunDataFactory(declDataProvider, report);
                    default:
                        break;
                }

            throw new Exception(
                $"Could not create RunDataFactory for Vehicle Category{vehicleCategory}");
        }

        private IVectoRunDataFactory CreateRunDataReader(ISingleBusInputDataProvider singleBusProvider, IDeclarationReport report)
        {
            throw new NotImplementedException();
        }

        private IVectoRunDataFactory CreateRunDataReader(IVTPDeclarationInputDataProvider vtpProvider, IVTPReport vtpReport)
        {
            throw new NotImplementedException();
        }

        public IVectoRunDataFactory CreateEngineeringRunDataFactory(IEngineeringInputDataProvider inputDataProvider)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
