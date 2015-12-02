using System;
using System.ComponentModel;

namespace TUGraz.VectoCore.Models.Simulation
{
	/// <summary>
	/// Defines the methods for a single vecto run.
	/// </summary>
	public interface IVectoRun
	{
		/// <summary>
		/// Run the simulation.
		/// </summary>
		void Run(BackgroundWorker worker = null, Action<double> ReportProgress = null);

		string Name { get; }

		/// <summary>
		/// Return the vehicle container.
		/// </summary>
		/// <returns></returns>
		IVehicleContainer GetContainer();

		bool FinishedWithoutErrors { get; }
	}
}