using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class SerialHybridModalDataPostprocessingCorrection : ModalDataPostprocessingCorrection
	{
		#region Overrides of ModalDataPostprocessingCorrection

		public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var r = base.ApplyCorrection(modData, runData);

			var chgEfficiency = 1.0;

			if (runData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting) {
				chgEfficiency = DeclarationData.CalculateChargingEfficiencyPEV(runData);
			}
			r.ElectricEnergyConsumption_SoC = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
			r.ElectricEnergyConsumption_Final = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) / chgEfficiency;
			
			return r;
		}

		#endregion

		protected override void SetReesCorrectionDemand(IModalDataContainer modData, VectoRunData runData,
			CorrectedModalData r)
		{
			var deltaEReess = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName());
			var startSoc = modData.REESSStartSoC();
			var endSoc = modData.REESSEndSoC();
			var emEff = 0.0;
			if (endSoc < startSoc) {
				var etaReessChg = modData.WorkREESSChargeInternal().Value() / modData.WorkREESSChargeTerminal().Value();
				emEff = 1.0 / (etaReessChg);
			}
			if (endSoc > startSoc) {
				var etaReessDischg = modData.WorkREESSDischargeTerminal().Value() / modData.WorkREESSDischargeInternal().Value();
				emEff = etaReessDischg;
			}

			r.DeltaEReessMech = double.IsNaN(emEff) ? 0.SI<WattSecond>() : -deltaEReess * emEff;
		}

		protected override FuelConsumptionCorrection SetFuelConsumptionCorrection(IModalDataContainer modData, VectoRunData runData,
			CorrectedModalData r, IFuelProperties fuel)
		{
			var duration = modData.Duration;
			var distance = modData.Distance;
			var essParams = runData.DriverData.EngineStopStart;
			var engFuel = runData.EngineData.Fuels.First(x => x.FuelData.Equals(fuel));

			var fcIceIdle = engFuel.ConsumptionMap.GetFuelConsumptionValue(
								0.SI<NewtonMeter>(),
								runData.EngineData.IdleSpeed) *
							engFuel.FuelConsumptionCorrectionFactor;

			var elPowerGenerated = modData.TimeIntegral<WattSecond>(string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(), PowertrainPosition.GEN));
			var fcGenCharging = modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel);
			var socCorr = elPowerGenerated.IsEqual(0)
				? (runData.GenSet.GenSetCharacteristics.OptimalPoint.FuelConsumption /
					runData.GenSet.GenSetCharacteristics.OptimalPoint.ElectricPower).Cast<KilogramPerWattSecond>()
				: (fcGenCharging / elPowerGenerated).Cast<KilogramPerWattSecond>();
			var engLine = modData.EngineLineCorrectionFactor(fuel);
			var comp =
				runData.BusAuxiliaries?.PneumaticUserInputsConfig.CompressorMap
					.Interpolate(runData.EngineData.IdleSpeed);

			var f = new FuelConsumptionCorrection {
				Fuel = fuel,
				Distance = distance != null && distance.IsGreater(0) ? distance : null,
				Duration = duration != null && duration.IsGreater(0) ? duration : null,
				EngineLineCorrectionFactor = engLine,
				VehicleLine = modData.VehicleLineSlope(fuel),
				FcModSum = modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel),
				FcESS_EngineStart = engLine * modData.WorkEngineStart(),

				FcESS_AuxStandstill_ICEOff = r.EnergyAuxICEOffStandstill_UF * engLine,
				FcESS_AuxStandstill_ICEOn = r.EnergyAuxICEOnStandstill_UF * engLine +
											fcIceIdle * r.ICEOffTimeStandstill * (1 - r.UtilityFactorStandstill),

				FcESS_AuxDriving_ICEOff = r.EnergyAuxICEOffDriving_UF * engLine,
				FcESS_AuxDriving_ICEOn = r.EnergyAuxICEOnDriving_UF * engLine +
										fcIceIdle * r.ICEOffTimeDriving * (1 - r.UtilityFactorDriving),

				FcESS_DCDCMissing = r.EnergyDCDCMissing * engLine,
				FcBusAuxPSAirDemand = engLine * r.WorkBusAuxPSCorr,

				FcBusAuxPSDragICEOffStandstill = comp == null
					? 0.SI<Kilogram>()
					: comp.PowerOff * r.ICEOffTimeStandstill * engLine * (1 - essParams.UtilityFactorStandstill),
				FcBusAuxPSDragICEOffDriving = comp == null
					? 0.SI<Kilogram>()
					: comp.PowerOff * r.ICEOffTimeDriving * engLine * (1 - essParams.UtilityFactorDriving),
				FcREESSSoc = r.DeltaEReessMech * socCorr,
				FcBusAuxEs = engLine * r.WorkBusAuxESMech,
				FcHeatPumpHeatingEl = engLine * r.WorkBusAuxHeatPumpHeatingElMech,
				FcHeatPumpHeatingMech = engLine * r.WorkBusAuxHeatPumpHeatingMech,
				FcBusAuxEletcricHeater = engLine * r.WorkBusAuxElectricHeater,
				FcWHR = engLine * r.WorkWHR,
				FcAuxHtr = 0.SI<Kilogram>()
			};

			return f;
		}
	}
}