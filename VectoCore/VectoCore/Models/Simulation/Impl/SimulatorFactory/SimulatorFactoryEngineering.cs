using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			if (dataProvider is IVTPEngineeringInputDataProvider vtpProvider)
			{
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry())
				{
					DataReader = new EngineeringVTPModeVectoRunDataFactoryLorries(vtpProvider);
				}
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus())
				{
					DataReader = new EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider);
				}
				return;
			}
			if (dataProvider is IEngineeringInputDataProvider engDataProvider)
			{
				if (engDataProvider.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation)
				{
					DataReader = new EngineOnlyVectoRunDataFactory(engDataProvider);
				}
				else
				{
					DataReader = new EngineeringModeVectoRunDataFactory(engDataProvider);
				}
				return;
			}
			throw new VectoException("Unknown InputData for Engineering Mode!");
		}

		#region Overrides of SimulatorFactory

		public override IOutputDataWriter ReportWriter { get; protected set; }

		#endregion
	}
}
