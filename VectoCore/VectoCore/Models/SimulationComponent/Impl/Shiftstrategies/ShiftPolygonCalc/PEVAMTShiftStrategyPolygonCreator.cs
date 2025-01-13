using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc
{
    public class PEVAMTShiftStrategyPolygonCreator : IShiftPolygonCalculator
    {
        private ShiftStrategyParameters _shiftStrategyParameters;

        public PEVAMTShiftStrategyPolygonCreator(ShiftStrategyParameters shiftStrategyparamets)
        {
            _shiftStrategyParameters = shiftStrategyparamets;
        }

        public ShiftPolygon ComputeDeclarationExtendedShiftPolygon(
            GearboxType gearboxType,
            int i,
            EngineFullLoadCurve engineDataFullLoadCurve,
            IList<ITransmissionInputData> gearboxGears,
            CombustionEngineData engineData,
            double axlegearRatio,
            Meter dynamicTyreRadius,
            ElectricMotorData electricMotorData = null)
        {
            throw new NotImplementedException("Not applicable to PEVAMT Gearbox.");
        }

        public ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
            IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius,
            ElectricMotorData electricMotorData = null)
        {
            if (electricMotorData == null) {
                throw new VectoException("ElectricMotorData is required to calculate Shift Polygon!");
            }
            var emFld = electricMotorData.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			return DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i,
				electricMotorData.EfficiencyData.VoltageLevels.First().FullLoadCurve,
				electricMotorData.RatioADC, gearboxGears, axlegearRatio, dynamicTyreRadius,
				_shiftStrategyParameters.PEV_DownshiftSpeedFactor.LimitTo(0, 1) * emFld.RatedSpeed,
				_shiftStrategyParameters.PEV_DownshiftMinSpeedFactor * emFld.RatedSpeed);
			//return ComputeElectricMotorDeclarationShiftPolygon(gearboxType, i, gearboxGears, axlegearRatio, dynamicTyreRadius, electricMotorData,
			//             _shiftStrategyParameters.PEV_DownshiftSpeedFactor.LimitTo(0, 1) * emFld.RatedSpeed, _shiftStrategyParameters.PEV_DownshiftMinSpeedFactor * emFld.RatedSpeed);
		}


        public ShiftPolygon ComputeElectricMotorDeclarationShiftPolygon(GearboxType gbxType, int i,
			IList<ITransmissionInputData> gearboxGears, double axlegearRatio,
			Meter dynamicTyreRadius,
			ElectricMotorData electricMotorData, ElectricMotorData emDataLimited)
        {
			var emFld = electricMotorData.EfficiencyData.VoltageLevels.First().FullLoadCurve;
            return DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i,
				emDataLimited.EfficiencyData.VoltageLevels.First().FullLoadCurve, electricMotorData.RatioADC,
                gearboxGears, axlegearRatio, dynamicTyreRadius,
				_shiftStrategyParameters.PEV_DeRatedDownshiftSpeedFactor * emFld.RatedSpeed,
				_shiftStrategyParameters.PEV_DownshiftMinSpeedFactor * emFld.RatedSpeed);
        }
    }
}