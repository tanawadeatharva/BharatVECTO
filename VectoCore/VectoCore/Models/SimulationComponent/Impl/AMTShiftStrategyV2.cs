using System;
using System.Data;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
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

			var maxGradability = new MaxGradabilityLookup();
			dataBus.AddPreprocessor(new MaxGradabilityPreprocessor(maxGradability, data));
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
			var maxStartGear = (int)Math.Round(ModelData.Gears.Count / 2.0, MidpointRounding.AwayFromZero);

			var startGear = 1u;
			var minRating = double.MaxValue;
			for (uint i = (uint)maxStartGear; i > 0; i--) {
				if (StartGearAllowed(i)) {
					var rating = RatingStartGear(i);
					if (rating < minRating) {
						minRating = rating;
						startGear = i;
					}
				}
			}
			return startGear;
		}

		private double RatingStartGear(uint u)
		{
			throw new NotImplementedException();
		}

		private bool StartGearAllowed(uint gear)
		{
			throw new NotImplementedException();
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
