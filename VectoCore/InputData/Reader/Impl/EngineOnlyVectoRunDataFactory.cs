/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

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