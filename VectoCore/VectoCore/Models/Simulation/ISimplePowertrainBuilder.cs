using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.Simulation
{
    public interface ISimplePowertrainBuilder
    {
        /// <summary>
        /// Builds a simple conventional powertrain.
        /// <code>
        ///(MeasuredSpeedDrivingCycle)
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     ├(Angledrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └ATGearbox or Gearbox
        ///      ├(TransmissionInputRetarder)
        ///      ├(Clutch)
        ///      └CombustionEngine
        ///       └(Aux)
        /// </code>
        /// </summary>
        void BuildSimplePowertrain(VectoRunData data, IVehicleContainer container);

        void BuildSimpleHybridPowertrainGear(VectoRunData data, VehicleContainer container);

        /// <summary>
        /// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
        /// <code>
        /// Vehicle
        /// └Wheels
        ///  └SimpleHybridController
        ///   └Brakes
        ///    │ └Engine E4
        ///    └AxleGear
        ///     │ ├(AxlegearInputRetarder)
        ///     │ └Engine E3
        ///     ├(AngleDrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └Gearbox or APTNGearbox
        ///      ├(TransmissionInputRetarder)
        ///      └Engine E2
        /// </code>
        /// </summary>
        void BuildSimpleSerialHybridPowertrain(VectoRunData data, VehicleContainer container);

        /// <summary>
        /// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
        /// <code>
        /// Vehicle
        /// └Wheels
        ///  └SimpleHybridController
        ///   └Brakes
        ///    │ └Engine E4
        ///    └AxleGear
        ///     │ ├(AxlegearInputRetarder)
        ///     │ └Engine E3
        ///     ├(AngleDrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └Gearbox or APTNGearbox
        ///      ├(TransmissionInputRetarder)
        ///      └Engine E2
        /// </code>
        /// </summary>
        void BuildSimpleIEPCHybridPowertrain(VectoRunData data, VehicleContainer container);

        /// <summary>
        /// Builds a simple genset
        /// <code>
        /// Engine Gen
        ///  └CombustionEngine
        /// </code>
        /// </summary>
        void BuildSimpleGenSet(VectoRunData data, VehicleContainer container);

        /// <summary>
        /// Builds a simple hybrid powertrain.
        ///<code>
        /// (MeasuredSpeedDrivingCycle)
        ///  └Vehicle
        ///   └Wheels
        ///    └SimpleHybridController
        ///     └Brakes
        ///      ├(Engine P4)
        ///      └AxleGear
        ///       ├(Engine P3)
        ///       ├(Angledrive)
        ///       ├(TransmissionOutputRetarder)
        ///       └Gearbox, ATGearbox, or APTNGearbox
        ///        ├(TransmissionInputRetarder)
        ///        ├(Engine P2.5)
        ///        ├(Engine P2)
        ///        ├(SwitchableClutch)
        ///        ├(Engine P1)
        ///        └StopStartCombustionEngine
        ///         └(Aux)
        /// </code>
        /// </summary>
        void BuildSimpleHybridPowertrain(VectoRunData data, VehicleContainer container);

        /// <summary>
        /// Builds a simple battery electric powertrain for PEVs.
        /// <code>
        /// (Dummy MeasureSpeedDrivingCycle)
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     └ATGearbox or Gearbox
        ///      └Electric Motor
        /// </code>
        /// </summary>
        void BuildSimplePowertrainElectric(VectoRunData data, VehicleContainer container);
    }
}