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