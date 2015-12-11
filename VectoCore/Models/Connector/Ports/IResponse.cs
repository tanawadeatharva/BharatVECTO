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
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports
{
	/// <summary>
	/// Defines an interface for a Response.
	/// </summary>
	public interface IResponse
	{
		Second SimulationInterval { get; set; }

		MeterPerSquareSecond Acceleration { get; set; }

		Meter SimulationDistance { get; set; }

		Watt EnginePowerRequest { get; set; }

		Watt AuxiliariesPowerDemand { get; set; }

		Watt ClutchPowerRequest { get; set; }

		Watt GearboxPowerRequest { get; set; }

		Watt AxlegearPowerRequest { get; set; }

		Watt WheelsPowerRequest { get; set; }

		Watt VehiclePowerRequest { get; set; }

		Watt BrakePower { get; set; }
	}
}