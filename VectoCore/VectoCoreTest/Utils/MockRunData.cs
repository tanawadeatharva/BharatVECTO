using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockRunData : VectoRunData
	{
		public MockRunData()
		{
			JobName = "MockJob";
			Retarder = new RetarderData() { Type = RetarderType.None };
			VehicleData = new VehicleData() {
				CurbWeight = 0.SI<Kilogram>(),
				BodyAndTrailerWeight = 0.SI<Kilogram>(),
				CargoVolume = 0.SI<CubicMeter>(),
				Loading = 0.SI<Kilogram>(),
				TotalRollResistanceCoefficient = 0,
				DynamicTyreRadius = 1.SI<Meter>(),
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(1.SI<SquareMeter>(),
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(1.SI<SquareMeter>()), CrossWindCorrectionMode.NoCorrection)
			};
			Cycle = new DrivingCycleData() {
				Name = "MockCycle",
			};
			EngineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad(),
				Displacement = 0.SI<CubicMeter>(),
				FullLoadCurve = new EngineFullLoadCurve() {
					FullLoadEntries = new List<FullLoadCurve.FullLoadCurveEntry>() {
						new FullLoadCurve.FullLoadCurveEntry() {
							EngineSpeed = 600.RPMtoRad(),
							TorqueDrag = -100.SI<NewtonMeter>(),
							TorqueFullLoad = 500.SI<NewtonMeter>()
						},
						new FullLoadCurve.FullLoadCurveEntry() {
							EngineSpeed = 1800.RPMtoRad(),
							TorqueDrag = -120.SI<NewtonMeter>(),
							TorqueFullLoad = 1200.SI<NewtonMeter>()
						},
						new FullLoadCurve.FullLoadCurveEntry() {
							EngineSpeed = 2500.RPMtoRad(),
							TorqueDrag = -150.SI<NewtonMeter>(),
							TorqueFullLoad = 400.SI<NewtonMeter>()
						},
					}
				}
			};
			GearboxData = new GearboxData() {
				Type = GearboxType.MT,
				Gears = new Dictionary<uint, GearData>() {
					//{ 1, new GearData() { Ratio = 1 } },
					//{ 2, new GearData() { Ratio = 5 } }
				}
			};
			AxleGearData = new AxleGearData() {
				AxleGear = new GearData() { Ratio = 1 }
			};
		}
	}
}