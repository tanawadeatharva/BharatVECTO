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

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	/// <summary>
	/// The Interface for a Response. Carries over result data to higher components.
	/// </summary>
	public interface IResponse
	{
		Second AbsTime { get; set; }
		Meter SimulationDistance { get; set; }
		Second SimulationInterval { get; set; }
		MeterPerSquareSecond Acceleration { get; set; }
		PerSecond EngineSpeed { get; set; }
		OperatingPoint OperatingPoint { get; set; }
		object Source { get; set; }

		Watt EnginePowerRequest { get; set; }
		Watt ClutchPowerRequest { get; set; }
		Watt GearboxPowerRequest { get; set; }
		Watt AxlegearPowerRequest { get; set; }
		Watt WheelsPowerRequest { get; set; }
		
		//Watt VehiclePowerRequest { get; set; }
		Watt BrakePower { get; set; }
		Watt AngledrivePowerRequest { get; set; }

		Watt AuxiliariesPowerDemand { get; set; }

		NewtonMeter EngineTorqueDemand { get; set; }
		NewtonMeter EngineTorqueDemandTotal { get; set; }
		NewtonMeter EngineDynamicFullLoadTorque { get; set; }
		MeterPerSecond VehicleSpeed { get; set; }
	}
}