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

using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public abstract class AbstractResponse : IResponse
	{
		public object Source { get; set; }

		public Second AbsTime { get; set; }
		public Second SimulationInterval { get; set; }
		public Meter SimulationDistance { get; set; }
		public MeterPerSquareSecond Acceleration { get; set; }
		public OperatingPoint OperatingPoint { get; set; }
		public PerSecond EngineSpeed { get; set; }

		public Watt EnginePowerRequest { get; set; }
		public Watt AngularGearPowerRequest { get; set; }
		public Watt ClutchPowerRequest { get; set; }
		public Watt GearboxPowerRequest { get; set; }
		public Watt AxlegearPowerRequest { get; set; }
		public Watt WheelsPowerRequest { get; set; }
		public Watt VehiclePowerRequest { get; set; }
		public Watt BrakePower { get; set; }
		public Watt AuxiliariesPowerDemand { get; set; }

		public TorqueConverterOperatingPoint TorqueConverterOperatingPoint { get; set; }

		public override string ToString()
		{
			var t = GetType();
			return string.Format("{0}{{{1}}}", t.Name,
				", ".Join(t.GetProperties().Select(p => string.Format("{0}: {1}", p.Name, p.GetValue(this)))));
		}
	}

	/// <summary>
	/// Response when the Cycle is finished.
	/// </summary>
	public class ResponseCycleFinished : AbstractResponse {}

	/// <summary>
	/// Response when a request was successful.
	/// </summary>
	public class ResponseSuccess : AbstractResponse {}

	/// <summary>
	/// Response when the request resulted in an engine or gearbox overload. 
	/// </summary>
	public class ResponseOverload : AbstractResponse
	{
		public Watt Delta { get; set; }
		public double Gradient { get; set; }
	}

	/// <summary>
	/// Response when the request resulted in an engine under-load. 
	/// </summary>
	public class ResponseUnderload : AbstractResponse
	{
		public Watt Delta { get; set; }
		public double Gradient { get; set; }
	}

	/// <summary>
	/// Response when the Speed Limit was exceeded.
	/// </summary>
	public class ResponseSpeedLimitExceeded : AbstractResponse {}

	/// <summary>
	/// Response when the request should have another time interval.
	/// </summary>
	public class ResponseFailTimeInterval : AbstractResponse
	{
		public Second DeltaT { get; set; }
	}

	public class ResponseDrivingCycleDistanceExceeded : AbstractResponse
	{
		public Meter MaxDistance { get; set; }
	}

	internal class ResponseDryRun : AbstractResponse
	{
		public Watt DeltaFullLoad { get; set; }
		public Watt DeltaDragLoad { get; set; }
		public NewtonMeter EngineMaxTorqueOut { get; set; }
		public NewtonMeter EngineDragTorque { get; set; }
	}

	internal class ResponseGearShift : AbstractResponse {}

	internal class ResponseEngineSpeedTooLow : ResponseDryRun {}
}