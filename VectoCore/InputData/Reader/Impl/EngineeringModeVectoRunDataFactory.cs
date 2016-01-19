using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class EngineeringModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected DriverData Driver;

		protected IInputDataProvider InputDataProvider;

		internal EngineeringModeVectoRunDataFactory(IInputDataProvider dataProvider)
		{
			InputDataProvider = dataProvider;
		}

		/// <summary>
		/// Iterate over all cycles defined in the JobFile and create a container with all data required for creating a simulation run
		/// </summary>
		/// <returns>VectoRunData instance for initializing the powertrain.</returns>
		public virtual IEnumerable<VectoRunData> NextRun()
		{
			var dao = new EngineeringDataAdapter();
			var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
			var engineData = dao.CreateEngineData(InputDataProvider.EngineInputData);

			return InputDataProvider.JobInputData().Cycles.Select(cycle => new VectoRunData {
				JobName = InputDataProvider.JobInputData().JobName,
				EngineData = engineData,
				GearboxData = dao.CreateGearboxData(InputDataProvider.GearboxInputData, engineData),
				AxleGearData = dao.CreateAxleGearData(InputDataProvider.AxleGearInputData),
				VehicleData = dao.CreateVehicleData(InputDataProvider.VehicleInputData),
				DriverData = driver,
				Aux = dao.CreateAuxiliaryData(InputDataProvider.AuxiliaryInputData()),
				Retarder = dao.CreateRetarderData(InputDataProvider.RetarderInputData),
				Cycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name),
				IsEngineOnly = InputDataProvider.JobInputData().EngineOnlyMode
			});
		}
	}
}