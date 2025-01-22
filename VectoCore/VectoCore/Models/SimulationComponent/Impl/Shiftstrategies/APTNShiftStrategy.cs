using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
	public class APTNShiftStrategy : PEVAMTShiftStrategy
	{
		public APTNShiftStrategy(IVehicleContainer container) : base(container, false)
		{
			if (container.RunData.VehicleData == null) {
				return;
			}

			if (!container.IsTestPowertrain) {
				SetupVelocityDropPreprocessor(container);
			}
		}

		public const string Name = "APT-N";

		protected override GearshiftPosition CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse resp)
		{
			return base.CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, resp);
		}

		protected override GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse resp)
		{
			return base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, resp);
		}

		protected override GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse r)
		{
			return base.DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, r);
		}
		
		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
            PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
        {
            var shiftAllowed = !dt.IsSmaller(Constants.SimulationSettings.TargetTimeInterval / 10);
			return shiftAllowed && base.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response) ;
        }
	}
}