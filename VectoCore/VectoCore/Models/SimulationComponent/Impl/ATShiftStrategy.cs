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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected readonly GearboxData Data;
		protected readonly IDataBus DataBus;
		private ATGearbox _gearbox;
		private readonly NextGearState _nextGear;

		public ATShiftStrategy(GearboxData data, IDataBus dataBus)
		{
			Data = data;
			DataBus = dataBus;
			_nextGear = new NextGearState();
		}

		//public ATGearbox Gearbox { get; protected internal set; }

		public uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				_gearbox.TorqueConverterLocked = false;
				_gearbox.Disengaged = true;
				return 1; // AT always starts in first gear!
			}
			var torqueConverterLocked = true;
			for (var gear = Data.Gears.Keys.Max(); gear > 1; gear--) {
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

		public bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			// 0 -> 1C: drive off after stop - engage first gear
			if (_gearbox.Disengaged && outAngularVelocity.IsGreater(0.SI<PerSecond>())) {
				Log.Debug("shift requried: drive off after vehicle stopped");
				_nextGear.SetState(absTime, false, 1, false);
				return true;
			}

			// _ -> 0: disengage before halting
			if (DataBus.DriverBehavior == DrivingBehavior.Braking && outTorque.IsSmaller(0) &&
				DataBus.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ATGearboxDisengageWhenHaltingSpeed)) {
				_nextGear.SetState(absTime, true, 1, false);
				return true;
			}

			// 1C -> 0: disengange when negative T_out and positive T_in
			if (gear == 1 && !_gearbox.TorqueConverterLocked && outTorque.IsSmaller(0) && inTorque.IsGreater(0)) {
				_nextGear.SetState(absTime, true, 1, false);
				return true;
			}

			if (inAngularVelocity == null) {
				return false;
			}

			// L -> 0: disengage if inAngularVelocity == 0
			if (_gearbox.TorqueConverterLocked && inAngularVelocity.IsEqual(0.SI<PerSecond>())) {
				_nextGear.SetState(absTime, true, 1, false);
				return true;
			}

			// Emergency Downshift: if lower than engine idle speed
			if (inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				Log.Debug("engine speed would fall below idle speed - shift down");
				Downshift(absTime, gear);
				return true;
			}

			// Emergency Upshift: if higher than engine rated speed
			if (inAngularVelocity.IsGreaterOrEqual(DataBus.EngineRatedSpeed) && Data.Gears.ContainsKey(gear + 1)) {
				Log.Debug("engine speed would be above rated speed - shift up");
				Upshift(absTime, gear);
				return true;
			}

			if ((absTime - lastShiftTime).IsSmaller(Data.ShiftTime)) {
				return false;
			}

			if (CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				lastShiftTime)) {
				return true;
			}

			if (CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				lastShiftTime)) {
				return true;
			}

			return false;
		}

		private void Upshift(Second absTime, uint gear)
		{
			// C -> L: switch from torque converter to locked gear
			if (!_gearbox.TorqueConverterLocked && Data.Gears[gear].HasLockedGear) {
				_nextGear.SetState(absTime, false, gear, true);
				return;
			}

			// L -> L+1
			// C -> C+1
			if (Data.Gears.ContainsKey(gear + 1)) {
				_nextGear.SetState(absTime, false, gear + 1, _gearbox.TorqueConverterLocked);
				return;
			}

			// C -> L+1 -- not allowed!!
			throw new VectoSimulationException(
				"ShiftStrategy wanted to shift up, but current gear has active torque converter (C) but no locked gear (no L) and shifting directly to (L) is not allowed.");
		}

		private void Downshift(Second absTime, uint gear)
		{
			// L -> C
			if (_gearbox.TorqueConverterLocked && Data.Gears[gear].HasTorqueConverter) {
				_nextGear.SetState(absTime, false, gear, false);
				return;
			}

			// L -> L-1
			// C -> C-1
			if (Data.Gears.ContainsKey(gear - 1)) {
				_nextGear.SetState(absTime, false, gear - 1, _gearbox.TorqueConverterLocked);
				return;
			}

			// L -> 0 -- not allowed!!
			throw new VectoSimulationException(
				"ShiftStrategy wanted to shift down but current gear is locked (L) and has no torque converter (C) and disenganging directly from (L) is not allowed.");
		}

		private bool CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			if (_gearbox.TorqueConverterLocked || Data.Gears[gear].HasLockedGear) {
				// L -> L+1 
				// C -> L
				var nextGear = _gearbox.TorqueConverterLocked ? gear + 1 : gear;
				if (!Data.Gears.ContainsKey(nextGear)) {
					return false;
				}

				var nextEngineSpeed = outAngularVelocity * Data.Gears[nextGear].Ratio;
				var enginePower = inAngularVelocity * inTorque;
				if (nextEngineSpeed.IsEqual(0)) {
					return false;
				}

				if (
					IsAboveUpShiftCurve(gear, enginePower / nextEngineSpeed, nextEngineSpeed,
						_gearbox.TorqueConverterLocked) &&
					enginePower.IsSmallerOrEqual(DataBus.EngineStationaryFullPower(nextEngineSpeed))) {
					Upshift(absTime, gear);
					return true;
				}
			}

			if (!_gearbox.TorqueConverterLocked && Data.Gears.ContainsKey(gear + 1) &&
				Data.Gears[gear + 1].HasTorqueConverter) {
				// C -> C+1
				var gearRatio = Data.Gears[gear + 1].TorqueConverterRatio / Data.Gears[gear].TorqueConverterRatio;
				var minEngineSpeed = VectoMath.Min(700.RPMtoRad(),
					gearRatio * (DataBus.EngineN80hSpeed - 150.RPMtoRad()));
				var nextGbxInSpeed = outAngularVelocity * Data.Gears[gear + 1].TorqueConverterRatio;
				var nextGbxInTorque = outTorque / Data.Gears[gear + 1].TorqueConverterRatio;
				var tcOperatingPoint = _gearbox.TorqueConverter.FindOperatingPoint(nextGbxInTorque, nextGbxInSpeed);
				if (tcOperatingPoint.InAngularVelocity.IsGreater(minEngineSpeed) &&
					DataBus.EngineStationaryFullPower(tcOperatingPoint.InAngularVelocity)
						.IsGreater(0.7 * DataBus.EngineStationaryFullPower(inAngularVelocity))) {
					Upshift(absTime, gear);
					return true;
				}
			}
			return false;
		}

		private bool CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			// downshift not possible
			if (!_gearbox.TorqueConverterLocked && gear == 1) {
				return false;
			}

			if (IsBelowDownShiftCurve(gear, inTorque, inAngularVelocity)) {
				Downshift(absTime, gear);
				return true;
			}

			return false;
		}

		public uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
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

		public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new System.NotImplementedException("AT Shift Strategy does not support disengaging.");
		}

		public IGearbox Gearbox
		{
			get { return _gearbox; }
			set
			{
				var myGearbox = value as ATGearbox;
				if (myGearbox == null) {
					throw new VectoException("AT Shift strategy can only handle AT gearboxes, given: {0}",
						value.GetType());
				}
				_gearbox = myGearbox;
			}
		}

		public GearInfo NextGear
		{
			get { return new GearInfo() { Gear = _nextGear.Gear, TorqueConverterLocked = _nextGear.TorqueConverterLocked }; }
		}

		/// <summary>
		/// Tests if the operating point is below the down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowDownShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			return gear > 1 && Data.Gears[gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		/// <summary>
		/// Tests if the operating point is above the up-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <param name="torqueConverterLocked">if true, the regular shift polygon is used, otherwise the shift polygon for the torque converter is used</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAboveUpShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed,
			bool torqueConverterLocked)
		{
			if (torqueConverterLocked) {
				return gear < Data.Gears.Keys.Max() &&
						Data.Gears[gear].ShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
			}
			return gear < Data.Gears.Keys.Max() &&
					Data.Gears[gear].TorqueConverterShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
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