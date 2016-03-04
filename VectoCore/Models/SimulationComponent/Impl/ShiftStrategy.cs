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
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Class ShiftStrategy is a base class for shift strategies. Implements some helper methods for checking the shift curves.
	/// </summary>
	public abstract class ShiftStrategy : IShiftStrategy
	{
		protected IDataBus DataBus;

		protected GearboxData Data;

		protected ShiftStrategy(GearboxData data, IDataBus dataBus)
		{
			DataBus = dataBus;
			Data = data;
		}

		public abstract uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);

		public abstract void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);

		public abstract bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularSpeed, uint gear, Second lastShiftTime);

		public abstract uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);

		public Gearbox Gearbox { get; set; }

		/// <summary>
		/// Tests if the operating point is below the down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowDownShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear <= 1) {
				return false;
			}

			var downSection = Data.Gears[gear].ShiftPolygon.Downshift.GetSection(entry => entry.AngularSpeed < inEngineSpeed);
			if (downSection.Item2.AngularSpeed < inEngineSpeed) {
				return false;
			}

			return IsLeftOf(inEngineSpeed, inTorque, downSection.Item1, downSection.Item2);
		}

		/// <summary>
		/// Tests if the operating point is above the up-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAboveUpShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear >= Data.Gears.Count) {
				return false;
			}

			var upSection = Data.Gears[gear].ShiftPolygon.Upshift.GetSection(entry => entry.AngularSpeed < inEngineSpeed);

			if (upSection.Item2.AngularSpeed < inEngineSpeed) {
				return true;
			}

			return IsRightOf(inEngineSpeed, inTorque, upSection.Item1, upSection.Item2);
		}

		/// <summary>
		/// Tests if current power request is on the left side of the shiftpolygon segment
		/// </summary>
		/// <param name="angularSpeed">The angular speed.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <returns><c>true</c> if current power request is on the left side of the shiftpolygon segment; otherwise, <c>false</c>.</returns>
		/// <remarks>Computes a simplified cross product for the vectors: from--X, from--to and checks
		/// if the z-component is positive (which means that X was on the right side of from--to).</remarks>
		private static bool IsLeftOf(PerSecond angularSpeed, NewtonMeter torque, ShiftPolygon.ShiftPolygonEntry from,
			ShiftPolygon.ShiftPolygonEntry to)
		{
			var abX = to.AngularSpeed - from.AngularSpeed;
			var abY = to.Torque - from.Torque;
			var acX = angularSpeed - from.AngularSpeed;
			var acY = torque - from.Torque;
			var z = abX * acY - abY * acX;
			return z.IsGreater(0);
		}

		/// <summary>
		/// Tests if current power request is on the left side of the shiftpolygon segment
		/// </summary>
		/// <param name="angularSpeed">The angular speed.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <returns><c>true</c> if current power request is on the left side of the shiftpolygon segment; otherwise, <c>false</c>.</returns>
		/// <remarks>Computes a simplified cross product for the vectors: from--X, from--to and checks
		/// if the z-component is negative (which means that X was on the left side of from--to).</remarks>
		private static bool IsRightOf(PerSecond angularSpeed, NewtonMeter torque, ShiftPolygon.ShiftPolygonEntry from,
			ShiftPolygon.ShiftPolygonEntry to)
		{
			var abX = to.AngularSpeed - from.AngularSpeed;
			var abY = to.Torque - from.Torque;
			var acX = angularSpeed - from.AngularSpeed;
			var acY = torque - from.Torque;
			var z = abX * acY - abY * acX;
			return z.IsSmaller(0);
		}
	}

	/// <summary>
	/// AMTShiftStrategy implements the AMT Shifting Behaviour.
	/// </summary>
	public class AMTShiftStrategy : ShiftStrategy
	{
		/// <summary>
		/// The previous gear before the disengagement. Used for GetGear() when skipGears is false.
		/// </summary>
		protected uint PreviousGear;

		public AMTShiftStrategy(GearboxData data, IDataBus dataBus) : base(data, dataBus)
		{
			PreviousGear = 1;

			// todo: move to settings
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
			return (outAngularSpeed * Data.Gears[gear].Ratio).IsGreaterOrEqual(1.2 * DataBus.EngineRatedSpeed -
																				0.2 * DataBus.EngineIdleSpeed);
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
					var reserve = 1 - (response.EnginePowerRequest / fullLoadPower).Cast<Scalar>();
					var inTorque = response.ClutchPowerRequest / inAngularSpeed;

					// if in shift curve and above idle speed and torque reserve is provided.
					if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && inAngularSpeed > DataBus.EngineIdleSpeed &&
						reserve >= Data.StartTorqueReserve) {
						return gear;
					}
				}
				return 1;
			} else {
				for (var gear = (uint)Data.Gears.Count; gear > 1; gear--) {
					var response = Gearbox.Initialize(gear, outTorque, outEngineSpeed);

					var inAngularSpeed = outEngineSpeed * Data.Gears[gear].Ratio;
					var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - (response.EnginePowerRequest / fullLoadPower).Cast<Scalar>();
					var inTorque = response.ClutchPowerRequest / inAngularSpeed;

					// if in shift curve and torque reserve is provided: return the current gear
					if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
						reserve >= Data.StartTorqueReserve) {
						if ((inAngularSpeed - DataBus.EngineIdleSpeed) / (DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
							Constants.SimulationSettings.ClutchNormSpeed && gear > 1) {
							gear--;
						}

						return gear;
					}

					// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
					if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && gear < Data.Gears.Count) {
						return gear + 1;
					}
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

			// down shift
			while (IsBelowDownShiftCurve(NextGear, inTorque, inAngularVelocity)) {
				NextGear--;
				if (!Data.SkipGears) {
					break;
				}

				var tmpGear = Gearbox.Gear;
				Gearbox.Gear = NextGear;
				var response = (ResponseDryRun)Gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				Gearbox.Gear = tmpGear;

				inAngularVelocity = Data.Gears[NextGear].Ratio * outAngularVelocity;
				inTorque = response.ClutchPowerRequest / inAngularVelocity;
			}

			if (NextGear != gear) {
				return true;
			}

			// upshift
			while (IsAboveUpShiftCurve(NextGear, inTorque, inAngularVelocity)) {
				NextGear++;
				if (!Data.SkipGears) {
					break;
				}

				var tmpGear = Gearbox.Gear;
				Gearbox.Gear = NextGear;
				var response = (ResponseDryRun)Gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				Gearbox.Gear = tmpGear;

				inAngularVelocity = Data.Gears[NextGear].Ratio * outAngularVelocity;
				inTorque = response.ClutchPowerRequest / inAngularVelocity;
			}

			// early up shift to higher gear ---------------------------------------
			if (Data.EarlyShiftUp && NextGear < Data.Gears.Count) {
				// try if next gear would provide enough torque reserve
				var tryNextGear = NextGear + 1;
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
						var reserve = 1 - (response.EnginePowerRequest / fullLoadPower).Cast<Scalar>();

						if (reserve >= Data.TorqueReserve) {
							NextGear = tryNextGear;
						}
					}
				}
			}

			if ((Data.Gears[NextGear].Ratio * outAngularVelocity - DataBus.EngineIdleSpeed) /
				(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
				Constants.SimulationSettings.ClutchNormSpeed && NextGear > 1) {
				NextGear--;
			}

			return (NextGear != gear);
		}

		public uint NextGear { get; set; }
	}

	public class MTShiftStrategy : AMTShiftStrategy
	{
		public MTShiftStrategy(GearboxData data, IDataBus bus) : base(data, bus)
		{
			Data.EarlyShiftUp = false;
			Data.SkipGears = true;
		}
	}

	// TODO Implement ATShiftStrategy
	public class ATShiftStrategy : ShiftStrategy
	{
		public ATShiftStrategy(GearboxData data, IDataBus bus) : base(data, bus) {}

		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new NotImplementedException();
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new NotImplementedException();
		}

		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque,
			PerSecond inAngularSpeed, uint gear, Second lastShiftTime)
		{
			throw new NotImplementedException();
		}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new NotImplementedException();
		}
	}

	// TODO Implement CustomShiftStrategy
	public class CustomShiftStrategy : ShiftStrategy
	{
		public CustomShiftStrategy(GearboxData data, IDataBus dataBus) : base(data, dataBus) {}

		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque,
			PerSecond inAngularSpeed, uint gear, Second lastShiftTime)
		{
			throw new NotImplementedException();
		}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new NotImplementedException();
		}

		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new NotImplementedException();
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new NotImplementedException();
		}
	}
}