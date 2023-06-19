using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public class SimulatorFactoryEngineering : SimulatorFactory
    {
        public SimulatorFactoryEngineering(IInputDataProvider dataProvider, IOutputDataWriter writer, bool validate) : base(ExecutionMode.Engineering, writer, validate)
        {
            CreateEngineeringDataReader(dataProvider);
        }

		private void CreateEngineeringDataReader(IInputDataProvider dataProvider)
		{
			switch (dataProvider) {
				case IVTPEngineeringInputDataProvider vtpProvider when vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry():
					RunDataFactory = new EngineeringVTPModeVectoRunDataFactoryLorries(vtpProvider);
					return;
				case IVTPEngineeringInputDataProvider vtpProvider when vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus():
					throw new NotImplementedException();
					//RunDataFactory = new EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider);
					return;
				case IEngineeringInputDataProvider engDataProvider when engDataProvider.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation:
					RunDataFactory = new EngineOnlyVectoRunDataFactory(engDataProvider);
					return;
				case IEngineeringInputDataProvider engDataProvider:
					RunDataFactory = new EngineeringModeVectoRunDataFactory(engDataProvider);
					return;
				default:
					throw new VectoException("Unknown InputData for Engineering Mode!");
			}
		}

	}
}
