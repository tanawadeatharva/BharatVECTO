/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

using System.Dynamic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public class DriverResponse
	{
		public MeterPerSquareSecond Acceleration { get; set; }
		public OperatingPoint OperatingPoint { get; set; }
	}

	public abstract class AbstractComponentResponse
	{
		public Watt PowerRequest { get; set; }
	}

	public class EngineResponse : AbstractComponentResponse
	{
		public PerSecond EngineSpeed { get; set; }

		public NewtonMeter EngineTorqueDemand { get; set; }
		public NewtonMeter EngineTorqueDemandTotal { get; set; }
		public NewtonMeter EngineDynamicFullLoadTorque { get; set; }

		public NewtonMeter EngineStationaryFullLoadTorque { get; set; }

		public Watt DynamicFullLoadPower { get; set; }
		public Watt DragPower { get; set; }

		public Watt AuxiliariesPowerDemand { get; set; }
	}

	

	public class ClutchResponse : AbstractComponentResponse { }

	public class GearboxResponse : AbstractComponentResponse { }

	public class AxlegearResponse : AbstractComponentResponse
	{
		public NewtonMeter CardanTorque { get; set; }
	}

	public class AngledriveResponse : AbstractComponentResponse { }

	public class WheelsResponse : AbstractComponentResponse { }

	public class VehicleResponse
	{
		public MeterPerSecond VehicleSpeed { get; set; }
	}

	public class BrakesResponse
	{
		public Watt BrakePower { get; set; }
	}

	public class ElectricMotorResponse
	{
		public Watt ElectricMotorPowerMech { get; set; }
	}

	/// <summary>
	/// The Interface for a Response. Carries over result data to higher components.
	/// </summary>
	public interface IResponse
	{
		object Source { get; }

		Second AbsTime { get; set; }
		Meter SimulationDistance { get; set; }
		Second SimulationInterval { get; set; }

		DriverResponse Driver { get; }

		EngineResponse Engine { get; }

		ClutchResponse Clutch { get; }

		GearboxResponse Gearbox { get; }

		AxlegearResponse Axlegear { get; }

		AngledriveResponse Angledrive { get; }

		WheelsResponse Wheels { get; }

		VehicleResponse Vehicle { get; }

		BrakesResponse Brakes { get; }

		ElectricMotorResponse ElectricMotor { get; }

		IElectricSystemResponse ElectricSystem { get; set; }
	}

	public interface IBatteryResponse
	{
		Second AbsTime { get; set; }

		Second SimulationInterval { get; set; }

		Watt MaxBatteryLoadCharge { get; set; }

		Watt MaxBatteryLoadDischarge { get; set; }

		Watt BatteryPower { get; set; }

		Watt BatteryLoss { get; set; }

		object Source { get; }
	}

	public interface IElectricSystemResponse
	{
		IBatteryResponse BatteryResponse { get; set; }

		Watt AuxPower { get; set; }

		Watt ConsumerPower { get; set; }

		Watt ChargingPower { get; set; }

		Watt MaxPowerDrive { get; }

		Watt MaxPowerDrag { get; }

		object Source { get; }
	}
}