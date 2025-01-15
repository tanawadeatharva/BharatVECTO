using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IShiftStrategyFactory
	{
		IShiftStrategy GetShiftStrategy(string name, IVehicleContainer container);

		string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType);

		IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName, ShiftStrategyParameters shiftParams);

		IShiftPolygonCalculator CreateShiftPolygonCalculator(GearboxType gearboxType, VectoSimulationJobType jobType, ShiftStrategyParameters shiftParams);
	}

	public interface IShiftPolygonCalculator
	{
		ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null);

		ShiftPolygon ComputeDeclarationExtendedShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null);

		ShiftPolygon ComputeElectricMotorDeclarationShiftPolygon(GearboxType gearboxType, int i,
			IList<ITransmissionInputData> gearboxGears, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData, ElectricMotorData emDataLimited);

	}

    public class ShiftStrategyFactory : IShiftStrategyFactory
	{
		public IShiftStrategy GetShiftStrategy(string name, IVehicleContainer container)
		{

			if (name == AMTShiftStrategyOptimized.Name) {
				return new AMTShiftStrategyOptimized(container);
			}

			if (name == PEVAMTShiftStrategy.Name) {
				return new PEVAMTShiftStrategy(container);
			}

			if (name == MTShiftStrategy.Name) {
				return new MTShiftStrategy(container);
			}

			if (name == ATShiftStrategyOptimized.Name) {
				return new ATShiftStrategyOptimized(container);
			}

			if (name == APTNShiftStrategy.Name) {
				return new APTNShiftStrategy(container);
			}

			throw new ArgumentOutOfRangeException(nameof(name), $@"Could not create shift strategy {name}");
		}

        public string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType)
        {
            switch (gearboxType) {
                case GearboxType.AMT:
                    switch (jobType) {
                        case VectoSimulationJobType.ConventionalVehicle:
                        case VectoSimulationJobType.ParallelHybridVehicle:
                            return AMTShiftStrategyOptimized.Name;
                        case VectoSimulationJobType.BatteryElectricVehicle:
                        case VectoSimulationJobType.SerialHybridVehicle:
                            return PEVAMTShiftStrategy.Name;
                        default:
                            throw new VectoException(
                                "no default gearshift strategy available for gearbox type {0} and job type {1}",
                                gearboxType, jobType);
                    }
                case GearboxType.MT:
                    return MTShiftStrategy.Name;

                case GearboxType.ATPowerSplit:
                case GearboxType.ATSerial:
                    switch (jobType) {
                        case VectoSimulationJobType.ParallelHybridVehicle:
                        case VectoSimulationJobType.ConventionalVehicle:
                            return ATShiftStrategyOptimized.Name;
                        case VectoSimulationJobType.SerialHybridVehicle:
                        case VectoSimulationJobType.BatteryElectricVehicle:
                            return APTNShiftStrategy.Name;
                        default:
                            throw new VectoException(
                                "no default gearshift strategy available for gearbox type {0} and job type {1}",
                                gearboxType, jobType);
                    }
                case GearboxType.APTN:
                    switch (jobType) {
                        case VectoSimulationJobType.ParallelHybridVehicle:
                        case VectoSimulationJobType.SerialHybridVehicle:
                        case VectoSimulationJobType.BatteryElectricVehicle:
                        case VectoSimulationJobType.IEPC_E:
                        case VectoSimulationJobType.IEPC_S:
                            return APTNShiftStrategy.Name;
                        //case VectoSimulationJobType.ConventionalVehicle when isTestPowerTrain:
                        //    return null;
                        default:
                            throw new ArgumentException(
                                "APT-N Gearbox is only applicable on hybrids and battery electric vehicles.");
                    }
                case GearboxType.IHPC:
                    switch (jobType) {
                        case VectoSimulationJobType.IHPC:
                            return AMTShiftStrategyOptimized.Name;
                        default:
                            throw new ArgumentException(
                                "IHPC Gearbox is only applicable on hybrid vehicle of type IHPC.");
                    }
                default:
                    throw new ArgumentOutOfRangeException("GearboxType", gearboxType,
                        "VECTO can not automatically derive shift strategy for GearboxType.");
            }
        }

		public IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName, ShiftStrategyParameters shiftParams)
		{
			switch (shiftStrategyName) {
                case AMTShiftStrategyOptimized.Name:
					return new AMTShiftStrategyOptimizedPolygonCalculator();
                case AMTShiftStrategy.Name:
					return new AMTShiftStrategyPolygonCalculator();
                case APTNShiftStrategy.Name:
				case PEVAMTShiftStrategy.Name:
					return new PEVAMTShiftStrategyPolygonCreator(shiftParams);
                case ATShiftStrategyOptimized.Name:
                    return new ATShiftStrategyOptimizedPolygonCalculator();
                default:
					throw new VectoException(
						$"undefined shift polygon calculator for shift strategy ${shiftStrategyName}");
			}
		}

		public IShiftPolygonCalculator CreateShiftPolygonCalculator(GearboxType gearboxType, VectoSimulationJobType jobType, ShiftStrategyParameters shiftParams)
		{
			return CreateShiftPolygonCalculator(GetShiftStrategyName(gearboxType, jobType), shiftParams);
		}
	}
}