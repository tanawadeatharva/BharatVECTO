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

using System.Collections.Generic;
using System.Diagnostics;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public enum CycleType
	{
		EngineOnly,
		TimeBased,
		DistanceBased,
		PWheel,
		MeasuredSpeed,
		MeasuredSpeedGear
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
				AuxiliarySupplyPower = new Dictionary<string, Watt>(entry.AuxiliarySupplyPower);
			}

			/// <summary>
			/// Travelled distance used for distance-based cycles. If "t" is also defined this column will be ignored.
			/// </summary>
			public Meter Distance { get; set; }

			/// <summary>
			/// Used for time-based cycles. If neither this nor the distance. "s" is defined the data will be interpreted as 1Hz.
			/// </summary>
			public Second Time { get; set; }

			/// <summary>
			/// Required except for Engine Only Mode calculations.
			/// </summary>
			public MeterPerSecond VehicleTargetSpeed { get; set; }

			/// <summary>
			/// Optional.
			/// </summary>
			public Radian RoadGradient { get; set; }

			/// <summary>
			/// [%] Optional.
			/// </summary>
			public double RoadGradientPercent { get; set; }

			/// <summary>
			/// relative altitude of the driving cycle over distance
			/// </summary>
			public Meter Altitude { get; set; }

			/// <summary>
			/// Required for distance-based cycles. Not used in time based cycles. "stop" defines the time the vehicle spends in stop phases.
			/// </summary>
			public Second StoppingTime { get; set; }

			/// <summary>
			/// Supply Power input for each auxiliary defined in the .vecto file where xxx matches the ID of the corresponding
			/// Auxiliary. ID's are not case sensitive and must not contain space or special characters.
			/// </summary>
			public Dictionary<string, Watt> AuxiliarySupplyPower { get; set; }

			/// <summary>
			/// If "n" is defined VECTO uses that instead of the calculated engine speed value.
			/// </summary>
			public PerSecond AngularVelocity { get; set; }

			/// <summary>
			/// [-] Gear input. Overwrites the gear shift model.
			/// </summary>
			public uint Gear { get; set; }

			/// <summary>
			/// This power input will be directly added to the engine power in addition to possible other auxiliaries. Also used in Engine Only Mode.
			/// </summary>
			public Watt AdditionalAuxPowerDemand { get; set; }

			/// <summary>
			/// Only required if Cross Wind Correction is set to Vair and Beta Input.
			/// </summary>
			public MeterPerSecond AirSpeedRelativeToVehicle { get; set; }

			/// <summary>
			/// [°] Only required if Cross Wind Correction is set to Vair and Beta Input.
			/// </summary>
			public double WindYawAngle { get; set; }

			/// <summary>
			/// Effective engine torque at clutch. Only required in Engine Only Mode. Alternatively power "Pe" can be defined. Use "DRAG" to define motoring operation.
			/// </summary>
			public NewtonMeter Torque { get; set; }

			public bool Drag { get; set; }

			/// <summary>
			/// Power on the Wheels (only used in PWheel Mode).
			/// </summary>
			public Watt PWheel { get; set; }
		}
	}
}