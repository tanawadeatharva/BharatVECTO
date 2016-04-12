/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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

using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
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