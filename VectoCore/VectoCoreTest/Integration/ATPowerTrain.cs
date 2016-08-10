using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration
{
	public class ATPowerTrain
	{
		public const string AccelerationFile = @"TestData\Components\Truck.vacc";
		public const string EngineFile = @"TestData\Components\AT_GBX\Engine.veng";
		//public const string AxleGearLossMap = @"TestData\Components\AT_GBX\Axle.vtlm";
		//public const string GearboxIndirectLoss = @"TestData\Components\AT_GBX\Indirect Gear.vtlm";
		//public const string GearboxDirectLoss = @"TestData\Components\AT_GBX\Direct Gear.vtlm";
		public const string TorqueConverterFile = @"TestData\Components\AT_GBX\TorqueConverter.vtcc";
		public const string GearboxShiftPolygonFile = @"TestData\Components\AT_GBX\AT-Shift.vgbs";


		public static VectoRun CreateEngineeringRun(DrivingCycleData cycleData, string modFileName,
			bool overspeed = false, KilogramSquareMeter gearBoxInertia = null)
		{
			var container = CreatePowerTrain(cycleData, Path.GetFileNameWithoutExtension(modFileName), overspeed, gearBoxInertia);
			return new DistanceRun(container);
		}

		public static VehicleContainer CreatePowerTrain(DrivingCycleData cycleData, string modFileName,
			bool overspeed = false, KilogramSquareMeter gearBoxInertia = null)
		{
			var fileWriter = new FileOutputWriter(modFileName);
			var modData = new ModalDataContainer(modFileName, fileWriter) {
				WriteModalResults = true,
				HasTorqueConverter = true
			};
			var container = new VehicleContainer(ExecutionMode.Engineering, modData) {
				RunData = new VectoRunData { JobName = modFileName, Cycle = cycleData }
			};

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData();
			var gearboxData = CreateGearboxData();
			if (gearBoxInertia != null) {
				gearboxData.Inertia = gearBoxInertia;
			}

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var driverData = CreateDriverData(AccelerationFile, overspeed);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var engine = new CombustionEngine(container, engineData);

			var tmp = cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy()))
				.AddComponent(new Vehicle(container, vehicleData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new DummyRetarder(container))
				.AddComponent(new ATGearbox(container, gearboxData, new ATShiftStrategy(gearboxData, container)))
				.AddComponent(engine);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("", 0.SI<Watt>());

			engine.Connect(aux.Port());

			return container;
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };

			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap = ratio.IsEqual(1)
								? TransmissionLossMapReader.Create(0.96, ratio, string.Format("Gear {0}", i))
								: TransmissionLossMapReader.Create(0.98, ratio, string.Format("Gear {0}", i)),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile),
							TorqueConverterRatio = i == 0 ? ratio : double.NaN,
							TorqueConverterGearLossMap =
								i == 0 ? TransmissionLossMapReader.Create(0.98, ratio, string.Format("Gear {0}", i)) : null,
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),
				ShiftTime = 1.SI<Second>(),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				StartTorqueReserve = 0.2,
				SkipGears = false,
				TorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				TorqueConverterData =
					TorqueConverterDataReader.ReadFromFile(TorqueConverterFile, 1000.RPMtoRad(),
						DeclarationData.Gearbox.TorqueConverterSpeedLimit)
			};
		}

		private static AxleGearData CreateAxleGearData()
		{
			const double ratio = 6.2;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.38,
					Inertia = 20.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = false,
					TyreTestLoad = 30436.0.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.62,
					Inertia = 18.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = true,
					TyreTestLoad = 30436.SI<Newton>()
				},
			};
			return new VehicleData {
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				//AerodynamicDragAera = 3.2634.SI<SquareMeter>(),
				//CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection,
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
				CurbWeight = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.465.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false
			};
		}

		private static DriverData CreateDriverData(string accelerationFile, bool overspeed = false)
		{
			return new DriverData {
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData {
					Enabled = true,
					//MinSpeed = 50.KMPHtoMeterPerSecond(),
					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
					LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				OverSpeedEcoRoll = overspeed
					? new DriverData.OverSpeedEcoRollData {
						Mode = DriverMode.Overspeed,
						MinSpeed = 50.KMPHtoMeterPerSecond(),
						OverSpeed = 5.KMPHtoMeterPerSecond()
					}
					: new DriverData.OverSpeedEcoRollData {
						Mode = DriverMode.Off
					},
				StartStop = new VectoRunData.StartStopData {
					Enabled = false
				}
			};
		}
	}
}