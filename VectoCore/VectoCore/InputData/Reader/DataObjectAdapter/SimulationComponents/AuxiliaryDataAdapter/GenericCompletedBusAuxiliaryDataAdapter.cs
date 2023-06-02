using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter
{
    public class GenericCompletedBusAuxiliaryDataAdapter : PrimaryBusAuxiliaryDataAdapter, ICompletedBusAuxiliaryDataAdapter
    {
        #region Implementation of ICompletedBusAuxiliaryDataAdapter

        public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
            IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
        {
            if (completedVehicle != null)
            {
                throw new ArgumentException("Completed Vehicle must not be provided");
            }
            return CreateBusAuxiliariesData(mission, primaryVehicle, runData);
        }

        #endregion
    }

    public class GenericCompletedPEVBusAuxiliaryDataAdapter : PrimaryBusPEVAuxiliaryDataAdapter,
        ICompletedBusAuxiliaryDataAdapter
    {
        #region Implementation of ICompletedBusAuxiliaryDataAdapter

        public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
            IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
        {
            if (completedVehicle != null)
            {
                throw new ArgumentException("Completed Vehicle must not be provided");
            }
            return CreateBusAuxiliariesData(mission, primaryVehicle, runData);
        }

        #endregion
    }
}