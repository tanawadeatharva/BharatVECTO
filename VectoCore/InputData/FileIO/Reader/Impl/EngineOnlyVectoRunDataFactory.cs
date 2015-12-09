using System.Collections.Generic;
using System.IO;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData.FileIO.EngineeringFile;
using TUGraz.VectoCore.InputData.FileIO.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.FileIO.Reader.Impl
{
	public class EngineOnlyVectoRunDataFactory : EngineeringModeVectoRunDataFactory
	{
		internal EngineOnlyVectoRunDataFactory(IInputDataProvider dataProvider) : base(dataProvider) {}

		public new IEnumerable<VectoRunData> NextRun()
		{
			if (InputDataProvider == null) {
				Log.Warn("No valid data provider given");
				yield break;
			}
			var dao = new EngineeringDataAdapter();
			foreach (var cycle in InputDataProvider.JobInputData().Cycles) {
				var simulationRunData = new VectoRunData {
					//BasePath = job.BasePath,
					JobName = InputDataProvider.JobInputData().JobName,
					EngineData = dao.CreateEngineData(InputDataProvider.EngineInputData),
					Cycle = DrivingCycleDataReader.Create(cycle.CycleData, cycle.Name, CycleType.EngineOnly),
					IsEngineOnly = IsEngineOnly
				};
				yield return simulationRunData;
			}
		}

		//protected override void ProcessJob(VectoJobFile vectoJob)
		//{
		//	var declaration = vectoJob as VectoJobFileV2Engineering;
		//	if (declaration == null) {
		//		throw new VectoException("Unhandled Job File Format");
		//	}
		//	var job = declaration;

		//	Engine = ReadEngine(Path.Combine(job.BasePath, job.Body.EngineFile));
		//}

		public bool IsEngineOnly
		{
			get { return true; }
		}
	}
}