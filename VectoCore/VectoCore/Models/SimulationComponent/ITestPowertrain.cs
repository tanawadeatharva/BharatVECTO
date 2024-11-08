using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{

    public interface ITestPowertrain<T> where T : class, IHybridControlledGearbox, IGearbox
    {
        void UpdateComponents();

        T Gearbox { get; }

        ICombustionEngine CombustionEngine { get; }

        ISimpleVehicleContainer Container { get; }

        ISimpleHybridController HybridController { get; }

        IAuxPort EngineAux { get; }

        IClutch Clutch { get; }

        IBrakes Brakes { get; }

        IElectricMotor ElectricMotor { get; }
        Dictionary<PowertrainPosition, ElectricMotor> ElectricMotorsUpstreamTransmission { get; }
        IDCDCConverter DCDCConverter { get; }
        ITorqueConverter TorqueConverter { get; }
        IElectricChargerPort Charger { get; }
		IElectricEnergyStorage BatterySystem { get; }
    }

    public interface ITestGenset
    {
        ICombustionEngine CombustionEngine { get; }

        IElectricMotor ElectricMotor { get; }

        IGensetMotorController ElectricMotorCtl { get; }

        IElectricEnergyStorage Battery { get; }
		IElectricEnergyStorage BatterySystem { get; }
        IElectricEnergyStorage SuperCap { get; }
        Joule EM_ThermalBuffer { set; }
        bool EM_DeRatingActive { set; }
        PerSecond EM_DrivetrainSpeed { set; }
        PerSecond EM_Speed { set; }

        IAuxPort EngineAux { get; }

        NewtonMeter ConvertEmTorqueToDrivetrain(PerSecond emSpeed, NewtonMeter tq, bool dryRun);
        PerSecond ConvertEmSpeedToDrivetrain(PerSecond emSpeed);
    }
}