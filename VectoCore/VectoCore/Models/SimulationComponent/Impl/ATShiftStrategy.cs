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
			if (DataBus.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ATGearboxDisengageWhenHaltingSpeed) &&
				outTorque.IsSmaller(0.SI<NewtonMeter>())) {
				// disengage before halting
				NextGear.SetState(absTime, true, 1, false);
			}

			if (inAngularVelocity != null) {
				// emergency shift to not stall the engine ------------------------
				if (inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					NextGear.SetState(absTime, false, gear - 1, !Data.Gears[gear - 1].HasTorqueConverter);
					Log.Debug("engine speed would fall below idle speed - shift down");
					return true;
				}
				if (inAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
					NextGear.SetState(absTime, false, gear + 1, Data.Gears[gear + 1].HasLockedGear);
					Log.Debug("engine speed would be above rated speed - shift up");
					return true;
				}
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
			return false;
		}

		private bool CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			if (_gearbox.TorqueConverterLocked) {
				// inTorque and inAngularVelocity are not set - 
				var engineSpeed = outAngularVelocity * Data.Gears[gear].Ratio;

				if (engineSpeed.IsSmaller(DataBus.EngineIdleSpeed)) {
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
			}
			if (!_gearbox.TorqueConverterLocked) {}
			// TODO: missing: C -> C downshift....


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

			var downSection = Data.Gears[gear].ShiftPolygon.Downshift.GetSection(entry => entry.AngularSpeed < inEngineSpeed);
			if (downSection.Item2.AngularSpeed < inEngineSpeed) {
				return false;
			}

			return ShiftPolygon.IsLeftOf(inEngineSpeed, inTorque, downSection);
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

			return ShiftPolygon.IsRightOf(inEngineSpeed, inTorque, upSection);
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