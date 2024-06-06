using System.Diagnostics;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class ValidationModeTests
{
	[TestCase]
	public void ValidationModeVehicleDataTest()
	{
		var vehicleData = new VehicleData {
			AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
			AirDensity = DeclarationData.AirDensity,
			CurbMass = 7500.SI<Kilogram>(),
			DynamicTyreRadius = 0.5.SI<Meter>(),
			//CurbWeigthExtra = 0.SI<Kilogram>(),
			Loading = 12000.SI<Kilogram>(),
			GrossVehicleMass = 16000.SI<Kilogram>(),
			TrailerGrossVehicleMass = 0.SI<Kilogram>(),
			AxleData = new List<Axle> {
				new Axle {
					AxleType = AxleType.VehicleNonDriven,
					AxleWeightShare = 0.4,
					Inertia = 0.5.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.00555,
					TyreTestLoad = 33000.SI<Newton>()
				},
				new Axle {
					AxleType = AxleType.VehicleNonDriven,
					AxleWeightShare = 0.6,
					Inertia = 0.5.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.00555,
					TyreTestLoad = 33000.SI<Newton>()
				},
			}
		};
		var result = vehicleData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.IsTrue(!result.Any(), "validation should have succeded but failed." + string.Concat(result));
		// Clear of History -> Normally happens inside the SimulatorFactory
		ValidationHelper.ClearValHistory();
		result = vehicleData.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.IsTrue(result.Any(), "validation should have failed, but succeeded." + string.Concat(result));
	}

    /// <summary>
    /// VECTO-107 Check valid range of input parameters
    /// </summary>
    [TestCase]
    public void ValidationModeVectoRunDataTest()
    {
        var container = new VehicleContainer(ExecutionMode.Engineering);
        var data = new DistanceRun(container);
        var engineData = new CombustionEngineData {
            FullLoadCurves =
                new Dictionary<uint, EngineFullLoadCurve>() {
                        { 0, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData)) },
                        { 1, FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData)) },
                },
            IdleSpeed = 560.RPMtoRad()
        };

        var gearboxData = new GearboxData();
        gearboxData.Gears[1] = new GearData {
            LossMap = TransmissionLossMapReader.Create(0.98, 1, "1"),
            Ratio = 1
        };

        var axleGearData = new AxleGearData {
            AxleGear = new GearData {
                Ratio = 1,
                LossMap = TransmissionLossMapReader.Create(0.98, 1, "1"),
            }
        };
        var vehicleData = new VehicleData {
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            AirDensity = DeclarationData.AirDensity,
            CurbMass = 7500.SI<Kilogram>(),
            DynamicTyreRadius = 0.5.SI<Meter>(),
            //CurbWeigthExtra = 0.SI<Kilogram>(),
            Loading = 12000.SI<Kilogram>(),
            GrossVehicleMass = 16000.SI<Kilogram>(),
            TrailerGrossVehicleMass = 0.SI<Kilogram>(),
            AxleData = new List<Axle> {
                    new Axle {
                        AxleType = AxleType.VehicleNonDriven,
                        AxleWeightShare = 0.4,
                        Inertia = 0.5.SI<KilogramSquareMeter>(),
                        RollResistanceCoefficient = 0.00555,
                        TyreTestLoad = 33000.SI<Newton>()
                    },
                    new Axle {
                        AxleType = AxleType.VehicleNonDriven,
                        AxleWeightShare = 0.6,
                        Inertia = 0.5.SI<KilogramSquareMeter>(),
                        RollResistanceCoefficient = 0.00555,
                        TyreTestLoad = 33000.SI<Newton>()
                    },
                }
        };

        container.RunData = new VectoRunData {
            JobRunId = 0,
            VehicleData = vehicleData,
            AirdragData = new AirdragData() {
                CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection,
                CrossWindCorrectionCurve =
                    new CrosswindCorrectionCdxALookup(5.SI<SquareMeter>(),
                        CrossWindCorrectionCurveReader.GetNoCorrectionCurve(5.SI<SquareMeter>()),
                        CrossWindCorrectionMode.NoCorrection)
            },
            GearboxData = gearboxData,
            EngineData = engineData,
            AxleGearData = axleGearData
        };

		var stopwatch = new Stopwatch();
		stopwatch.Start();
        var results = data.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		stopwatch.Stop();
		Console.WriteLine(stopwatch.Elapsed + " " + stopwatch.ElapsedMilliseconds);
        Assert.IsTrue(results.Any(), "Validation should have failed, but succeded.");

        results = vehicleData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
        Assert.IsTrue(!results.Any());
    }

    const string EngineFldHeader = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	private static readonly string[] EngineFldData = new string[] {
		"600,586,-44,0.60",
		"800,755,-54,0.60",
		"1000,883,-62,0.60",
		"1200,899,-74,0.60",
		"1300,899,-80,0.60",
		"1400,899,-87,0.60",
		"1500,899,-92,0.60",
		"1600,899,-97,0.60",
		"1700,890,-100,0.60",
		"1800,881,-103,0.60",
		"1900,867,-107,0.60",
		"2000,853,-111,0.43",
		"2150,811,-118,0.29",
		"2200,802,-125,0.25",
		"2300,755,-130,0.25",
		"2400,705,-135,0.25",
		"2500,644,-140,0.25",
		"2600,479,-145,0.25",
		"2700,0,-149,0.25",
	};

    //const string GearLossMapHeader = ""
}