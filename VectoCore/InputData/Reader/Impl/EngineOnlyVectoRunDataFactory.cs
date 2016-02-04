using System.Collections.Generic;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class EngineOnlyVectoRunDataFactory : EngineeringModeVectoRunDataFactory
	{
		internal EngineOnlyVectoRunDataFactory(IInputDataProvider dataProvider) : base(dataProvider) {}

		public override IEnumerable<VectoRunData> NextRun()
		{
			if (InputDataProvider == null) {
				Log.Warn("No valid data provider given");
				yield break;
			}
			var dao = new EngineeringDataAdapter();
			foreach (var cycle in InputDataProvider.JobInputData().Cycles) {
				var simulationRunData = new VectoRunData {
					JobName = InputDataProvider.JobInputData().JobName,
					EngineData = dao.CreateEngineData(InputDataProvider.EngineInputData),
					Cycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, CycleType.EngineOnly, cycle.Name),
					IsEngineOnly = IsEngineOnly
				};
				yield return simulationRunData;
			}
		}

		public bool IsEngineOnly
		{
			get { return true; }
		}
	}
}