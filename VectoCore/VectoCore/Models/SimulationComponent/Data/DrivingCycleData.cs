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
using System.Collections.Generic;
using System.Diagnostics;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public enum CycleType
	{
		EngineOnly,
		DistanceBased,
		PWheel,
		MeasuredSpeed,
		MeasuredSpeedGear,
		PTO
	}

	public static class CycleTypeHelper
	{
		public static bool IsDistanceBased(this CycleType type)
		{
			return type == CycleType.DistanceBased;
		}
	}

	public class DrivingCycleData : SimulationComponentData
	{
		internal DrivingCycleData() {}

		public List<DrivingCycleEntry> Entries { get; internal set; }

		public string Name { get; internal set; }

		public CycleType CycleType { get; internal set; }

		[DebuggerDisplay(
			"s:{Distance}, t:{Time}, v:{VehicleTargetSpeed}, grad:{RoadGradient}, n:{AngularVelocity}, gear:{Gear}")]
		public class DrivingCycleEntry
		{
			public DrivingCycleEntry() {}

			public DrivingCycleEntry(DrivingCycleEntry entry)
			{
				Distance = entry.Distance;
				Time = entry.Time;
				VehicleTargetSpeed = entry.VehicleTargetSpeed;
				RoadGradient = entry.RoadGradient;
				Altitude = entry.Altitude;
				StoppingTime = entry.StoppingTime;
				AngularVelocity = entry.AngularVelocity;
				Gear = entry.Gear;
				AdditionalAuxPowerDemand = entry.AdditionalAuxPowerDemand;
				AirSpeedRelativeToVehicle = entry.AirSpeedRelativeToVehicle;
				WindYawAngle = entry.WindYawAngle;
				Torque = entry.Torque;
				Drag = entry.Drag;
				PTOActive = entry.PTOActive;
				AuxiliarySupplyPower = new Dictionary<string, Watt>(entry.AuxiliarySupplyPower);
			}

			/// <summary>
			/// Travelled distance used for distance-based cycles. If "t" is also defined this column will be ignored.
			/// </summary>
			public Meter Distance;

			/// <summary>
			/// Used for time-based cycles. If neither this nor the distance. "s" is defined the data will be interpreted as 1Hz.
			/// </summary>
			public Second Time;

			/// <summary>
			/// Required except for Engine Only Mode calculations.
			/// </summary>
			public MeterPerSecond VehicleTargetSpeed;

			/// <summary>
			/// Optional.
			/// </summary>
			public Radian RoadGradient;

			/// <summary>
			/// [%] Optional.
			/// </summary>
			public Scalar RoadGradientPercent
			{
				get { return (Math.Tan(RoadGradient.Value()) * 100).SI<Scalar>(); }
			}

			/// <summary>
			/// relative altitude of the driving cycle over distance
			/// </summary>
			public Meter Altitude;

			/// <summary>
			/// Required for distance-based cycles. Not used in time based cycles. "stop" defines the time the vehicle spends in stop phases.
			/// </summary>
			public Second StoppingTime;

			/// <summary>
			/// Supply Power input for each auxiliary defined in the .vecto file where xxx matches the ID of the corresponding
			/// Auxiliary. ID's are not case sensitive and must not contain space or special characters.
			/// </summary>
			public Dictionary<string, Watt> AuxiliarySupplyPower;

			/// <summary>
			/// If "n_eng_avg" is defined VECTO uses that instead of the calculated engine speed value.
			/// </summary>
			public PerSecond AngularVelocity;

			/// <summary>
			/// [-] Gear input. Overwrites the gear shift model.
			/// </summary>
			public uint Gear;

			/// <summary>
			/// This power input will be directly added to the engine power in addition to possible other auxiliaries. Also used in Engine Only Mode.
			/// </summary>
			public Watt AdditionalAuxPowerDemand;

			/// <summary>
			/// Only required if Cross Wind Correction is set to Vair and Beta Input.
			/// </summary>
			public MeterPerSecond AirSpeedRelativeToVehicle;

			/// <summary>
			/// [°] Only required if Cross Wind Correction is set to Vair and Beta Input.
			/// </summary>
			public double WindYawAngle;

			/// <summary>
			/// Effective engine torque at clutch. Only required in Engine Only Mode. Alternatively power "Pe" can be defined. Use "DRAG" to define motoring operation.
			/// </summary>
			public NewtonMeter Torque;

			public bool Drag;

			/// <summary>
			/// Power on the Wheels (only used in PWheel Mode).
			/// </summary>
			public Watt PWheel;

			public bool? TorqueConverterActive { get; set; }

			/// <summary>
			/// The angular velocity at the wheel. only used in PWheelCycle.
			/// </summary>
			public PerSecond WheelAngularVelocity;

			/// <summary>
			/// Flag if PTO Cycle is active or not.
			/// </summary>
			public bool PTOActive;
		}
	}
}