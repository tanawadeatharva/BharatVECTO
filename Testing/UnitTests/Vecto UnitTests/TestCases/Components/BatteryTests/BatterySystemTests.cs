using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.BatteryTests;

public class BatterySystemTests
{
	public const double REESS_Capacity = 4.5;
	public const double REESS_MinSoC = 0.2;
	public const double REESS_MaxSoC = 0.8;

    [TestCase(0.5, 0.5, 5000),
   TestCase(0.5, 0.5, -5000)]
    public void BatterySystemTimeDependentInternalResistanceTest_LoadChanges(double initialSoC, double dt, double powerDemand)
    {
        var r1 = 0.02;
        var r2 = 0.04;
        var r3 = 0.1;

        var batteryData = new BatterySystemData() {
            Batteries = new List<Tuple<int, BatteryData>>() {
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    })
                }
        };

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var es = new ElectricSystem(container, batteryData);
        es.Connect(bat);
        es.Connect(new MockElectricConsumer(0.SI<Watt>()));
        bat.Initialize(initialSoC);

        var modData = new MockModalDataContainer();

        var absTime = 0.SI<Second>();

        var i = 0;

        for (; i < 11; i++) { // constant for the first 2 sec
            var response = es.Request(absTime, dt.SI<Second>(), -Math.Sign(powerDemand) * Math.Abs(powerDemand).SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 5; i++) { // constant for the first 2 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 21; i++) { // linear increase for the next 8s to 0.04
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r2 - r1) / (10 - 2);
            var r = slope * (absTime.Value() - 5.5) + r1 - slope * 2;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 41; i++) { // linear increase for the next 10s to 0.1
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r3 - r2) / (20 - 10);
            var r = slope * (absTime.Value() - 5.5) + r2 - slope * 10;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 100; i++) { // constant after 20 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }
    }

    [TestCase(0.5, 0.5, 5000),
   TestCase(0.5, 0.5, -5000)]
    public void BatterySystemTimeDependentInternalResistanceTest_ConstantLoad(double initialSoC, double dt, double powerDemand)
    {
        var r1 = 0.02;
        var r2 = 0.04;
        var r3 = 0.1;

        var batteryData = new BatterySystemData() {
            Batteries = new List<Tuple<int, BatteryData>>() {
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    })
                }
        };

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var es = new ElectricSystem(container, batteryData);
        es.Connect(bat);
        es.Connect(new MockElectricConsumer(0.SI<Watt>()));
        bat.Initialize(initialSoC);

        var modData = new MockModalDataContainer();

        var absTime = 0.SI<Second>();

        var i = 0;
        for (; i < 5; i++) { // constant for the first 2 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 21; i++) { // linear increase for the next 8s to 0.04
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r2 - r1) / (10 - 2);
            var r = slope * absTime.Value() + r1 - slope * 2;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 41; i++) { // linear increase for the next 10s to 0.1
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r3 - r2) / (20 - 10);
            var r = slope * absTime.Value() + r2 - slope * 10;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 100; i++) { // constant after 20 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }
    }



}