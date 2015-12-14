/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation
{
	/// <summary>
	/// Defines Methods for adding components, commiting a simulation step and finishing the simulation.
	/// Also defines interfaces for all cockpit access to data.
	/// </summary>
	public interface IVehicleContainer : IDataBus
	{
		IModalDataContainer ModalData { get; }

		VectoRunData RunData { get; }

		ISimulationOutPort GetCycleOutPort();

		/// <summary>
		/// Adds a component to the vehicle container.
		/// </summary>
		/// <param name="component"></param>
		void AddComponent(VectoSimulationComponent component);

		/// <summary>
		/// Commits the current simulation step.
		/// </summary>
		void CommitSimulationStep(Second time, Second simulationInterval);

		/// <summary>
		/// Finishes the simulation.
		/// </summary>
		void FinishSimulation();

		VectoRun.Status RunStatus { get; set; }
	}
}