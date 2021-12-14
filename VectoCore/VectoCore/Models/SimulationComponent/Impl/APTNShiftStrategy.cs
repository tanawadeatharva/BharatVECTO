using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class APTNShiftStrategy : PEVAMTShiftStrategy
	{
		public APTNShiftStrategy(IVehicleContainer dataBus) : base(dataBus) { }

		public new static string Name => "APT-N";

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
	}
}