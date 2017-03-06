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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategy : BaseShiftStrategy
	{
		private ATGearbox _gearbox;
		private readonly NextGearState _nextGear = new NextGearState();

		public override IGearbox Gearbox
		{
			get { return _gearbox; }
			set
			{
				_gearbox = value as ATGearbox;
				if (_gearbox == null) {
					throw new VectoException("AT Shift strategy can only handle AT gearboxes, given: {0}", value.GetType());
				}
			}
		}

		public override GearInfo NextGear
		{
			get { return new GearInfo(_nextGear.Gear, _nextGear.TorqueConverterLocked); }
		}

		public ATShiftStrategy(GearboxData data, IDataBus dataBus) : base(data, dataBus) {}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				// AT always starts in first gear and TC active!
				_gearbox.TorqueConverterLocked = false;
				_gearbox.Disengaged = true;
				return 1;
			}
			var torqueConverterLocked = true;
			for (var gear = ModelData.Gears.Keys.Max(); gear > 1; gear--) {
				if (_gearbox.ModelData.Gears[gear].HasTorqueConverter) {
					torqueConverterLocked = false;
				}
				var response = _gearbox.Initialize(gear, torqueConverterLocked, torque, outAngularVelocity);

				if (response.EngineSpeed > DataBus.EngineRatedSpeed || response.EngineSpeed < DataBus.EngineIdleSpeed) {
					continue;
				}

				if (!IsBelowDownShiftCurve(gear, response.EnginePowerRequest / response.EngineSpeed, response.EngineSpeed)) {
					_gearbox.TorqueConverterLocked = torqueConverterLocked;
					_gearbox.Disengaged = false;
					return gear;
				}
			}
			// fallback: start with first gear;
			_gearbox.TorqueConverterLocked = false;
			_gearbox.Disengaged = false;
			return 1;
		}

		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (_nextGear.AbsTime != null && _nextGear.AbsTime.IsEqual(absTime)) {
				_gearbox.TorqueConverterLocked = _nextGear.TorqueConverterLocked;
				_gearbox.Disengaged = _nextGear.Disengaged;
				_nextGear.AbsTime = null;
				return _nextGear.Gear;
			}
			_nextGear.AbsTime = null;
			return _gearbox.Gear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new System.NotImplementedException("AT Shift Strategy does not support disengaging.");
		}

		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			// ENGAGE ---------------------------------------------------------
			// 0 -> 1C: drive off after disengaged - engage first gear
			if (_gearbox.Disengaged && outAngularVelocity.IsGreater(0.SI<PerSecond>())) {
				Log.Debug("shift required: drive off after vehicle stopped");
				_nextGear.SetState(absTime, disengaged: false, gear: 1, tcLocked: false);
				return true;
			}

			// DISENGAGE ------------------------------------------------------
			// 1) _ -> 0: disengage before halting
			var braking = DataBus.DriverBehavior == DrivingBehavior.Braking;
			var torqueNegative = outTorque.IsSmaller(0);
			var slowerThanDisengageSpeed =
				DataBus.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ATGearboxDisengageWhenHaltingSpeed);
			var disengageBeforeHalting = braking && torqueNegative && slowerThanDisengageSpeed;

			// 2) L -> 0: disengage if inAngularVelocity == 0
			var disengageAngularVelocityZero = _gearbox.TorqueConverterLocked && inAngularVelocity.IsEqual(0.SI<PerSecond>());

			// 3) 1C -> 0: disengange when negative T_out and positive T_in
			var gear1C = gear == 1 && !_gearbox.TorqueConverterLocked;
			var disengageTOutNegativeAndTInPositive = DataBus.DriverAcceleration <= 0 && gear1C && outTorque.IsSmaller(0) && inTorque.IsGreater(0);

			var disengageTCEngineSpeedLowerIdle = braking && torqueNegative && gear1C &&
												inAngularVelocity.IsSmallerOrEqual(DataBus.EngineIdleSpeed);

			if (disengageBeforeHalting || disengageTCEngineSpeedLowerIdle || disengageAngularVelocityZero ||
				disengageTOutNegativeAndTInPositive) {
				_nextGear.SetState(absTime, disengaged: true, gear: 1, tcLocked: false);
				return true;
			}

			// EMERGENCY SHIFTS ---------------------------------------
			// Emergency Downshift: if lower than engine idle speed
			if (inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				Log.Debug("engine speed would fall below idle speed - shift down");
				Downshift(absTime, gear);
				return true;
			}
			// Emergency Upshift: if higher than engine rated speed
			if (inAngularVelocity.IsGreaterOrEqual(DataBus.EngineRatedSpeed)) {
				// check if upshift is possible
				if (!ModelData.Gears.ContainsKey(gear + 1)) {
					return false;
				}
				Log.Debug("engine speed would be above rated speed - shift up");
				Upshift(absTime, gear);
				return true;
			}

			// UPSHIFT --------------------------------------------------------
			if (CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				lastShiftTime)) {
				return true;
			}

			// DOWNSHIFT ------------------------------------------------------
			if (CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				lastShiftTime)) {
				return true;
			}

			return false;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private bool CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(ModelData.ShiftTime);
			if (!shiftTimeReached)
				return false;

			var currentGear = ModelData.Gears[gear];

			if (_gearbox.TorqueConverterLocked || currentGear.HasLockedGear) {
				// UPSHIFT - General Rule
				// L -> L+1 
				// C -> L
				var nextGear = _gearbox.TorqueConverterLocked ? gear + 1 : gear;
				if (!ModelData.Gears.ContainsKey(nextGear)) {
					return false;
				}

				var nextEngineSpeed = outAngularVelocity * ModelData.Gears[nextGear].Ratio;
				if (nextEngineSpeed.IsEqual(0)) {
					return false;
				}

				var currentEnginePower = inTorque * inAngularVelocity;
				var nextEngineTorque = currentEnginePower / nextEngineSpeed;
				var isAboveUpShift = IsAboveUpShiftCurve(gear, nextEngineTorque, nextEngineSpeed, _gearbox.TorqueConverterLocked);

				var minAccelerationReachable = true;
				if (!DataBus.VehicleSpeed.IsEqual(0)) {
					var reachableAcceleration = EstimateAccelerationForGear(nextGear, outAngularVelocity);
					var minAcceleration = _gearbox.TorqueConverterLocked
						? ModelData.UpshiftMinAcceleration
						: ModelData.TorqueConverterData.CLUpshiftMinAcceleration;
					minAcceleration = VectoMath.Min(minAcceleration, DataBus.DriverAcceleration);
					minAccelerationReachable = reachableAcceleration.IsGreaterOrEqual(minAcceleration);
				}

				if (isAboveUpShift && minAccelerationReachable) {
					Upshift(absTime, gear);
					return true;
				}
			}

			// UPSHIFT - Special rule for 1C -> 2C
			if (!_gearbox.TorqueConverterLocked && ModelData.Gears.ContainsKey(gear + 1) &&
				ModelData.Gears[gear + 1].HasTorqueConverter) {
				// C -> C+1
				var nextGear = ModelData.Gears[gear + 1];
				var gearRatio = nextGear.TorqueConverterRatio / currentGear.TorqueConverterRatio;
				var minEngineSpeed = VectoMath.Min(700.RPMtoRad(), gearRatio * (DataBus.EngineN80hSpeed - 150.RPMtoRad()));

				var nextGearboxInSpeed = outAngularVelocity * nextGear.TorqueConverterRatio;
				var nextGearboxInTorque = outTorque / nextGear.TorqueConverterRatio;
				var tcOperatingPoint = _gearbox.TorqueConverter.FindOperatingPoint(nextGearboxInTorque, nextGearboxInSpeed);

				var engineSpeedOverMin = tcOperatingPoint.InAngularVelocity.IsGreater(minEngineSpeed);

				var reachableAcceleration = EstimateAccelerationForGear(gear + 1, outAngularVelocity);
				var minAcceleration = VectoMath.Min(ModelData.TorqueConverterData.CCUpshiftMinAcceleration,
					DataBus.DriverAcceleration);
				var minAccelerationReachable = reachableAcceleration.IsGreaterOrEqual(minAcceleration);

				if (engineSpeedOverMin && minAccelerationReachable) {
					Upshift(absTime, gear);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Tests if the operating point is above (right of) the up-shift curve.
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <param name="torqueConverterLocked">if true, the regular shift polygon is used, otherwise the shift polygon for the torque converter is used</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		private bool IsAboveUpShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed,
			bool torqueConverterLocked)
		{
			var shiftPolygon = torqueConverterLocked
				? ModelData.Gears[gear].ShiftPolygon
				: ModelData.Gears[gear].TorqueConverterShiftPolygon;

			return gear < ModelData.Gears.Keys.Max() && shiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		private void Upshift(Second absTime, uint gear)
		{
			// C -> L: switch from torque converter to locked gear
			if (!_gearbox.TorqueConverterLocked && ModelData.Gears[gear].HasLockedGear) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear, tcLocked: true);
				return;
			}

			// L -> L+1
			// C -> C+1
			if (ModelData.Gears.ContainsKey(gear + 1)) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear + 1, tcLocked: _gearbox.TorqueConverterLocked);
				return;
			}

			// C -> L+1 -- not allowed!!
			throw new VectoSimulationException(
				"ShiftStrategy wanted to shift up, but current gear has active torque converter (C) but no locked gear (no L) and shifting directly to (L) is not allowed.");
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private bool CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(ModelData.ShiftTime);

			if (shiftTimeReached && IsBelowDownShiftCurve(gear, inTorque, inAngularVelocity)) {
				Downshift(absTime, gear);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Tests if the operating point is below (left of) the down-shift curve.
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		private bool IsBelowDownShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			return gear > 1 && ModelData.Gears[gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		private void Downshift(Second absTime, uint gear)
		{
			// L -> C
			if (_gearbox.TorqueConverterLocked && ModelData.Gears[gear].HasTorqueConverter) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear, tcLocked: false);
				return;
			}

			// L -> L-1
			// C -> C-1
			if (ModelData.Gears.ContainsKey(gear - 1)) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear - 1, tcLocked: _gearbox.TorqueConverterLocked);
				return;
			}

			// L -> 0 -- not allowed!!
			throw new VectoSimulationException(
				"ShiftStrategy wanted to shift down but current gear is locked (L) and has no torque converter (C) and disenganging directly from (L) is not allowed.");
		}

		private class NextGearState
		{
			public Second AbsTime;
			public bool Disengaged;
			public uint Gear;
			public bool TorqueConverterLocked;

			public void SetState(Second absTime, bool disengaged, uint gear, bool tcLocked)
			{
				AbsTime = absTime;
				Disengaged = disengaged;
				Gear = gear;
				TorqueConverterLocked = tcLocked;
			}
		}
	}
}