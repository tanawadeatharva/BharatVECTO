/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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
		void Run();

		/// <summary>
		/// Cancel the running simulation.
		/// </summary>
		void Cancel();

		/// <summary>
		/// unique identifier of a single run
		/// </summary>
		int RunIdentifier { get; }

		string RunName { get; }
		string CycleName { get; }

		string RunSuffix { get; }

		double Progress { get; }

		/// <summary>
		/// Return the vehicle container.
		/// </summary>
		/// <returns></returns>
		IVehicleContainer GetContainer();

		bool FinishedWithoutErrors { get; }
	}
}