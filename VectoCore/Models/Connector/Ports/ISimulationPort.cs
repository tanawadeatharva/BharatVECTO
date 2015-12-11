/*
* Copyright 2015 Graz University of Technology
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
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports
{
	/// <summary>
	/// Defines a method to acquire an DriverCycle Demand out port.
	/// </summary>
	public interface ISimulationOutProvider
	{
		/// <summary>
		/// Returns the outport to send requests to.
		/// </summary>
		/// <returns></returns>
		ISimulationOutPort OutPort();
	}

	//========================================================================

	/// <summary>
	/// Defines a method to request the outport.
	/// </summary>
	public interface ISimulationOutPort
	{
		/// <summary>
		/// Requests a demand for a specific absolute time and a time interval dt.
		/// </summary>
		/// <param name="absTime">The absolute time of the simulation.</param>
		/// <param name="ds"></param>
		/// <returns></returns>
		IResponse Request(Second absTime, Meter ds);

		IResponse Request(Second absTime, Second dt);

		IResponse Initialize();

		double Progress { get; }
	}
}