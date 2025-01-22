using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    public class CycleShiftStrategy : BaseShiftStrategy<CycleGearbox>
    {
        public CycleShiftStrategy(IVehicleContainer container) : base(container) { }

        //public override IGearbox Gearbox { get; set; }

        protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
            Second lastShiftTime, IResponse response)
        {
            return false;
        }

        public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public override GearshiftPosition NextGear => throw new NotImplementedException();


    }
}