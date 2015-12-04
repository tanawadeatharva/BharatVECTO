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