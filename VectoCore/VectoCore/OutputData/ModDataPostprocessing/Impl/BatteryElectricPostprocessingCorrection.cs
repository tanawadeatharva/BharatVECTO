using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public class BatteryElectricPostprocessingCorrection : IModalDataPostProcessor
    {
        #region Implementation of IModalDataPostProcessor

        public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
        {
            var chgEfficiency = DeclarationData.CalculateChargingEfficiencyPEV(runData);

            var deltaEPSel = 0.SI<WattSecond>();
            var airDemandCorr = 0.SI<NormLiter>();
            var deltaAir = 0.SI<NormLiter>();
            if (runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap == null) {
                // no compressor map - electric compressor
                var actuations = new Actuations() {
                    Braking = runData.BusAuxiliaries.Actuations.Braking,
                    ParkBrakeAndDoors = runData.BusAuxiliaries.Actuations.ParkBrakeAndDoors,
                    Kneeling = runData.BusAuxiliaries.Actuations.Kneeling,
                    CycleTime = modData.Duration
                };
                airDemandCorr = M03Impl.TotalAirDemandCalculation(runData.BusAuxiliaries, actuations);
                deltaAir = airDemandCorr - modData.AirGenerated();
                deltaEPSel = deltaAir * DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated /
                            runData.DCDCData.DCDCEfficiency;

            }

            return new PEVCorrectedModalData(modData) {
                CorrectedAirDemand = airDemandCorr,
                DeltaAir = deltaAir,
                WorkBusAux_elPS_SoC_ElRange = deltaEPSel,
                ElectricEnergyConsumption_SoC = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int),
                ElectricEnergyConsumption_Final = (-modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) + deltaEPSel) / chgEfficiency,
            };
        }

        #endregion
    }
}