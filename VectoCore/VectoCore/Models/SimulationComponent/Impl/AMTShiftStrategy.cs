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

using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// AMTShiftStrategy implements the AMT Shifting Behaviour.
	/// </summary>
	public class AMTShiftStrategy : ShiftStrategy
	{
		/// <summary>
		/// The previous gear before the disengagement. Used for GetGear() when skipGears is false.
		/// </summary>
		protected uint PreviousGear;

		public uint NextGear { get; set; }

		public AMTShiftStrategy(GearboxData data, IDataBus dataBus) : base(data, dataBus)
		{
			PreviousGear = 1;
			Data.EarlyShiftUp = true;
			Data.SkipGears = true;
		}


		private bool SpeedTooLowForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * Data.Gears[gear].Ratio).IsSmaller(DataBus.EngineIdleSpeed);
		}

		// original vecto2.2: (inAngularSpeed - IdleSpeed) / (RatedSpeed - IdleSpeed) >= 1.2
		//                  =  inAngularSpeed - IdleSpeed >= 1.2*(RatedSpeed - IdleSpeed)
		//                  =  inAngularSpeed >= 1.2*RatedSpeed - 0.2*IdleSpeed
		private bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * Data.Gears[gear].Ratio).IsGreaterOrEqual(DataBus.EngineN95hSpeed);
		}


		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			while (NextGear > 1 && SpeedTooLowForEngine(NextGear, outAngularVelocity)) {
				NextGear--;
			}
			while (NextGear < Data.Gears.Count && SpeedTooHighForEngine(NextGear, outAngularVelocity)) {
				NextGear++;
			}

			return NextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			PreviousGear = Gearbox.Gear;
		}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				for (var gear = (uint)Data.Gears.Count; gear > 1; gear--) {
					var inAngularSpeed = outEngineSpeed * Data.Gears[gear].Ratio;

					var ratedSpeed = Data.Gears[gear].FullLoadCurve != null
						? Data.Gears[gear].FullLoadCurve.RatedSpeed
						: DataBus.EngineRatedSpeed;
					if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
						continue;
					}

					var response = Gearbox.Initialize(gear, outTorque, outEngineSpeed);

					var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.EnginePowerRequest / fullLoadPower;
					var inTorque = response.ClutchPowerRequest / inAngularSpeed;

					// if in shift curve and above idle speed and torque reserve is provided.
					if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && inAngularSpeed > DataBus.EngineIdleSpeed &&
						reserve >= Data.StartTorqueReserve) {
						return gear;
					}
				}
				return 1;
			}
			for (var gear = (uint)Data.Gears.Count; gear > 1; gear--) {
				var response = Gearbox.Initialize(gear, outTorque, outEngineSpeed);

				var inAngularSpeed = outEngineSpeed * Data.Gears[gear].Ratio;
				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;
				var inTorque = response.ClutchPowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
					reserve >= Data.StartTorqueReserve) {
					if ((inAngularSpeed - DataBus.EngineIdleSpeed) / (DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
						Constants.SimulationSettings.ClutchClosingSpeedNorm && gear > 1) {
						gear--;
					}

					return gear;
				}

				// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
				if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && gear < Data.Gears.Count) {
					return gear + 1;
				}
			}

			// fallback: return first gear
			return 1;
		}

		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			// no shift when vehicle stands
			if (DataBus.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			NextGear = gear;
			while (NextGear > 1 && SpeedTooLowForEngine(NextGear, outAngularVelocity)) {
				NextGear--;
			}
			while (NextGear < Data.Gears.Count && SpeedTooHighForEngine(NextGear, outAngularVelocity)) {
				NextGear++;
			}
			if (NextGear != gear) {
				return true;
			}


			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed = (lastShiftTime + Data.ShiftTime).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			NextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear);
			if (NextGear != gear) {
				return true;
			}

			NextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear);

			if ((Data.Gears[NextGear].Ratio * outAngularVelocity - DataBus.EngineIdleSpeed) /
				(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
				Constants.SimulationSettings.ClutchClosingSpeedNorm && NextGear > 1) {
				NextGear--;
			}

			return (NextGear != gear);
		}

		protected virtual uint CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - Gearbox.LastDownshift).IsSmaller(Gearbox.ModelData.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);
			if (nextGear == currentGear) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(Gearbox.ModelData.UpshiftMinAcceleration)) {
				// if less than 0.1 for next gear, don't shift
				if (nextGear - currentGear == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear > currentGear &&
					EstimateAccelerationForGear(currentGear + 1, outAngularVelocity).IsSmaller(Gearbox.ModelData.UpshiftMinAcceleration))
				{
					return currentGear;
				}
				nextGear = currentGear + 1;
			}

			return nextGear;
		}

		protected virtual uint CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			if ((absTime - Gearbox.LastUpshift).IsSmaller(Gearbox.ModelData.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);
		}

		protected virtual uint DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// upshift
			while (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear++;
				if (!Data.SkipGears) {
					break;
				}

				var tmpGear = Gearbox.Gear;
				Gearbox.Gear = currentGear;
				var response = (ResponseDryRun)Gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				Gearbox.Gear = tmpGear;

				inAngularVelocity = Data.Gears[currentGear].Ratio * outAngularVelocity;
				inTorque = response.ClutchPowerRequest / inAngularVelocity;
			}

			// early up shift to higher gear ---------------------------------------
			if (Data.EarlyShiftUp && currentGear < Data.Gears.Count) {
				// try if next gear would provide enough torque reserve
				var tryNextGear = currentGear + 1;
				var tmpGear = Gearbox.Gear;
				Gearbox.Gear = tryNextGear;
				var response = (ResponseDryRun)Gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				Gearbox.Gear = tmpGear;
				if (!(response is ResponseEngineSpeedTooLow)) {
					inAngularVelocity = Data.Gears[tryNextGear].Ratio * outAngularVelocity;
					inTorque = response.ClutchPowerRequest / inAngularVelocity;

					// if next gear supplied enough power reserve: take it
					// otherwise take
					if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
						var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
						var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

						if (reserve >= Data.TorqueReserve) {
							currentGear = tryNextGear;
						}
					}
				}
			}
			return currentGear;
		}


		protected virtual uint DoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// down shift
			while (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear--;
				if (!Data.SkipGears) {
					break;
				}
				var tmpGear = Gearbox.Gear;
				Gearbox.Gear = currentGear;
				var response = (ResponseDryRun)Gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				Gearbox.Gear = tmpGear;

				inAngularVelocity = Data.Gears[currentGear].Ratio * outAngularVelocity;
				inTorque = response.ClutchPowerRequest / inAngularVelocity;
			}
			return currentGear;
		}
	}
}