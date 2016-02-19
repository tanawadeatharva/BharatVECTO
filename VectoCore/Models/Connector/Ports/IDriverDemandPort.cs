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

using System;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports
{
	/// <summary>
	/// Defines a method to acquire an DriverDemand in port.
	/// </summary>
	public interface IDriverDemandInProvider
	{
		/// <summary>
		/// Returns the inport to connect it to another outport.
		/// </summary>
		/// <returns></returns>
		IDriverDemandInPort InPort();
	}

	/// <summary>
	/// Defines a method to acquire an DriverDemand out port.
	/// </summary>
	public interface IDriverDemandOutProvider
	{
		/// <summary>
		/// Returns the outport to send requests to.
		/// </summary>
		/// <returns></returns>
		IDriverDemandOutPort OutPort();
	}

	//===============================================


	/// <summary>
	/// Defines a connect method to connect the inport to an outport.
	/// </summary>
	public interface IDriverDemandInPort
	{
		/// <summary>
		/// Connects the inport to another outport.
		/// </summary>
		void Connect(IDriverDemandOutPort other);
	}

	/// <summary>
	/// Defines a request method for a DriverDemand-Out-Port.
	/// </summary>
	public interface IDriverDemandOutPort
	{
		IResponse Request(Second absTime, Second dt, MeterPerSquareSecond acceleration, Radian gradient, bool dryRun = false);

		/// <summary>
		/// Initialize the powertrain component to drive at the given steady state
		/// </summary>
		/// <param name="vehicleSpeed"></param>
		/// <param name="roadGradient"></param>
		/// <returns></returns>
		IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient);

		/// <summary>
		/// Pre-Initialize the powertrain when the vehicle is stopped with an assumed start acceleration to find correct drive-off gear
		/// </summary>
		/// <param name="vehicleSpeed"></param>
		/// <param name="startAcceleration"></param>
		/// <param name="roadGradient"></param>
		/// <returns></returns>
		IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration);
	}
}