using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public class BatteryElectricPostprocessingCorrection : ModalDataPostProcessingCorrectionBase
    {
        #region Implementation of IModalDataPostProcessor

        public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
        {
			return DoApplyCorrection(modData, runData);
		}

		protected virtual ICorrectedModalData DoApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
            var chgEfficiency = DeclarationData.CalculateChargingEfficiencyPEV(runData);

            var deltaEPSel = 0.SI<WattSecond>();
            var airDemandCorr = 0.SI<NormLiter>();
            var deltaAir = 0.SI<NormLiter>();
            if (runData.BusAuxiliaries != null && runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap == null) {
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

			var corrected = GetModalDataCorrection(modData);

			corrected.CorrectedAirDemand = airDemandCorr;
			corrected.DeltaAir = deltaAir;
			corrected.WorkBusAux_elPS_SoC_ElRange = deltaEPSel;
			corrected.ElectricEnergyConsumption_SoC = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
			corrected.ElectricEnergyConsumption_Final = (-modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) + deltaEPSel) / chgEfficiency;

				SetAuxHeaterDemand(modData, runData, corrected);
			if (corrected.AuxHeaterDemand?.IsGreater(0) ?? false) {
               
				var f = FuelData.Diesel;

				var fc = new AuxHeaterFuelConsumptionCorrection(
                    fuel: f,
					distance: modData.Distance,
					duration: modData.Duration,
					fcAuxHeater: corrected.AuxHeaterDemand / f.LowerHeatingValueVecto);


				corrected.FuelCorrection[f.FuelType] = fc;
			}

			return corrected;
		}

		protected virtual AbstractCorrectedModalData GetModalDataCorrection(IModalDataContainer modData)
		{
			return new PEVCorrectedModalData(modData);
		}

		#endregion
    }
}