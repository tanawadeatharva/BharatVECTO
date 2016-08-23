using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected GearboxData Data;

		protected IDataBus DataBus;
		private ATGearbox _gearbox;

		protected NextGearState NextGear;

		public ATShiftStrategy(GearboxData data, IDataBus dataBus)
		{
			Data = data;
			DataBus = dataBus;
			NextGear = new NextGearState();
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
			if (_gearbox.Disengaged && outAngularVelocity.IsGreater(0.SI<PerSecond>())) {
				// drive off after stop - engage first gear
				Log.Debug("shift requried: drive off after vehicle stopped");
				NextGear.SetState(absTime, false, 1, false);
				return true;
			}
			if (DataBus.DriverBehavior == DrivingBehavior.Braking &&
				DataBus.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ATGearboxDisengageWhenHaltingSpeed) &&
				outTorque.IsSmaller(0.SI<NewtonMeter>())) {
				// disengage before halting
				NextGear.SetState(absTime, true, 1, false);
				return true;
			}

			if (inAngularVelocity != null) {
				// emergency shift to not stall the engine ------------------------
				if (_gearbox.TorqueConverterLocked && inAngularVelocity.IsEqual(0.SI<PerSecond>())) {
					NextGear.SetState(absTime, false, 1, false);
					return true;
				}
				if (inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					if (_gearbox.TorqueConverterLocked) {
						// downshift L -> L / C
						if (Data.Gears[gear].HasTorqueConverter) {
							NextGear.SetState(absTime, false, gear, false);
							return true;
						}
						if (Data.Gears.ContainsKey(gear - 1) && Data.Gears[gear - 1].HasLockedGear) {
							NextGear.SetState(absTime, false, gear - 1, true);
							return true;
						}
					} else {
						// downshift C -> C / 0
						if (Data.Gears.ContainsKey(gear - 1) && Data.Gears[gear - 1].HasTorqueConverter) {
							// C -> C
							NextGear.SetState(absTime, false, gear - 1, false);
							return true;
						}
						// C -> 0
						NextGear.SetState(absTime, true, 1, false);
						return true;
					}
					NextGear.SetState(absTime, false, gear - 1, !Data.Gears[gear - 1].HasTorqueConverter);
					Log.Debug("engine speed would fall below idle speed - shift down");
					return true;
				}
				if (inAngularVelocity.IsGreater(DataBus.EngineRatedSpeed) && Data.Gears.ContainsKey(gear + 1)) {
					NextGear.SetState(absTime, false, gear + 1, Data.Gears[gear + 1].HasLockedGear);
					Log.Debug("engine speed would be above rated speed - shift up");
					return true;
				}
			}

			if ((absTime - lastShiftTime).IsSmaller(Data.ShiftTime)) {
				return false;
			}

			if (CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime)) {
				return true;
			}
			if (CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime)) {
				return true;
			}

			return false;
		}

		private bool CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			if (gear == Data.Gears.Keys.Max()) {
				return false;
			}
			if ((!_gearbox.TorqueConverterLocked && Data.Gears[gear].HasLockedGear) || _gearbox.TorqueConverterLocked) {
				// C -> L , or L -> L upshift
				var nextGear = gear;
				if (_gearbox.TorqueConverterLocked) {
					nextGear = gear + 1;
				}
				var nextEngineSpeed = outAngularVelocity * Data.Gears[nextGear].Ratio;
				var enginePower = inAngularVelocity * inTorque;
				if (nextEngineSpeed.IsEqual(0)) {
					return false;
				}
				if (IsAboveUpShiftCurve(gear, enginePower / nextEngineSpeed, nextEngineSpeed) &&
					enginePower.IsSmallerOrEqual(DataBus.EngineStationaryFullPower(nextEngineSpeed))) {
					NextGear.SetState(absTime, false, nextGear, true);
					return true;
				}
			}
			if (!_gearbox.TorqueConverterLocked && Data.Gears.ContainsKey(gear + 1) && Data.Gears[gear + 1].HasTorqueConverter) {
				// C -> C upshift
				// TODO!
			}
			return false;
		}

		private bool CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			if (_gearbox.TorqueConverterLocked) {
				if (inAngularVelocity != null && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					// n_eng < n_eng_idle
					if (Data.Gears[gear].HasTorqueConverter) {
						// L -> C shift
						NextGear.SetState(absTime, false, gear, false);
					} else if (Data.Gears.ContainsKey(gear - 1) && Data.Gears[gear - 1].HasLockedGear) {
						// L -> L shift
						NextGear.SetState(absTime, false, gear - 1, true);
					} else {
						throw new VectoSimulationException("Downshift required, but failed to select gear!");
					}
				}
			} else {
				// already in converter mode
				if (!Data.Gears.ContainsKey(gear - 1)) {
					// downshift not possible
					return false;
				}
				if (inAngularVelocity != null && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					NextGear.SetState(absTime, false, gear - 1, false);
					return true;
				}
			}

			return false;
		}

		public uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (NextGear.AbsTime != null && NextGear.AbsTime.IsEqual(absTime)) {
				_gearbox.TorqueConverterLocked = NextGear.TorqueConverterLocked;
				_gearbox.Disengaged = NextGear.Disengaged;
				NextGear.AbsTime = null;
				return NextGear.Gear;
			}
			NextGear.AbsTime = null;
			return _gearbox.Gear;
		}

		public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new System.NotImplementedException();
		}

		public IGearbox Gearbox
		{
			get { return _gearbox; }
			set
			{
				var myGearbox = value as ATGearbox;
				if (myGearbox == null) {
					throw new VectoException("AT Shift strategy can only handle AT gearboxes, given: {0}", value.GetType());
				}
				_gearbox = myGearbox;
			}
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
			if (gear <= 1) {
				return false;
			}
			return Data.Gears[gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
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
			return Data.Gears[gear].ShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		protected class NextGearState
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