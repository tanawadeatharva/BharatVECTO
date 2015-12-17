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

using System;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	/// <summary>
	/// Base class for all vecto simulation components.
	/// </summary>
	public abstract class VectoSimulationComponent : LoggingObject
	{
		[NonSerialized] protected IDataBus DataBus;

		/// <summary>
		/// Constructor. Registers the component in the cockpit.
		/// </summary>
		/// <param name="dataBus">The vehicle container</param>
		protected VectoSimulationComponent(IVehicleContainer dataBus)
		{
			DataBus = dataBus;
			dataBus.AddComponent(this);
		}

		public void CommitSimulationStep(IModalDataContainer container)
		{
			if (container != null) {
				DoWriteModalResults(container);
			}
			DoCommitSimulationStep();
		}

		protected abstract void DoWriteModalResults(IModalDataContainer container);

		/// <summary>
		/// Commits the simulation step.
		/// Writes the moddata into the data writer.
		/// Commits the internal state of the object if needed.
		/// </summary>
		protected abstract void DoCommitSimulationStep();
	}
}