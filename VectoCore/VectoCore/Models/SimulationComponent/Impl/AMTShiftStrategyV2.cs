using System;
using System.Data;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyV2 : ShiftStrategy
	{
		private uint _nextGear;

		public AMTShiftStrategyV2(VectoRunData data, IVehicleContainer dataBus) : base(data.GearboxData, dataBus)
		{
			var velocityDropData = new VelocityRollingLookup();
			dataBus.AddPreprocessor(new VelocitySpeedGearshiftPreprocessor(velocityDropData, data.GearboxData.TractionInterruption));
		}

		#region Overrides of BaseShiftStrategy

		public override bool ShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			throw new System.NotImplementedException();


		}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(torque, outAngularVelocity);
			}

			for (var gear = (uint)ModelData.Gears.Count; gear > 1; gear--) {
				var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
				if (DataBus.EngineSpeed < inAngularVelocity && inAngularVelocity < DataBus.EngineRatedSpeed) {
					_nextGear = gear;
					return gear;
				}
			}

			return 1;
		}

		private uint InitStartGear(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return 1;
		}

		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			throw new System.NotImplementedException();
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new System.NotImplementedException();
		}

		public override GearInfo NextGear
		{
			get { return new GearInfo(_nextGear, true); }
		}

		#endregion
	}
}
