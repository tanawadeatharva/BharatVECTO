using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class ModalDataPostprocessingCorrection : IModalDataPostProcessor
	{
		
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var r = new CorrectedModalData();
			var duration = modData.Duration;
			var distance = modData.Distance;
			var essParams = runData.DriverData.EngineStopStart;

			var entriesAuxICEStandstill = modData.GetValues(x => new
			{
				dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()), 
				P_off = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_off.GetName()) ?? 0.SI<Watt>(),
				P_on = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_on.GetName()) ?? 0.SI<Watt>(),
				v = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
				IceOn = x.Field<bool>(ModalResultField.ICEOn.GetName())
			}).Where(x => x.v != null && x.v.IsEqual(0) && !x.IceOn).ToList();
			var entriesAuxICEDriving = modData.GetValues(x => new {
				dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
				P_off = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_off.GetName()) ?? 0.SI<Watt>(),
				P_on = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_on.GetName()) ?? 0.SI<Watt>(),
				v = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
				IceOn = x.Field<bool>(ModalResultField.ICEOn.GetName())
			}).Where(x => x.v != null && !x.v.IsEqual(0) && !x.IceOn).ToList();

			r.ICEOffTimeStandstill = entriesAuxICEStandstill.Sum(x => x.dt) ?? 0.SI<Second>();
			r.EnergyAuxICEOffStandstill = entriesAuxICEStandstill.Sum(x => x.P_off * x.dt) ?? 0.SI<WattSecond>(); 
			r.EnergyAuxICEOnStandstill = entriesAuxICEStandstill.Sum(x => x.P_on * x.dt) ?? 0.SI<WattSecond>();
				
			r.ICEOffTimeDriving = entriesAuxICEDriving.Sum(x => x.dt) ?? 0.SI<Second>();
			r.EnergyAuxICEOffDriving = entriesAuxICEDriving.Sum(x => x.P_off * x.dt) ?? 0.SI<WattSecond>();
			r.EnergyPowerICEOnDriving = entriesAuxICEDriving.Sum(x => x.P_on * x.dt) ?? 0.SI<WattSecond>();

			
			r.WorkWHREl = modData.TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr);
			r.WorkWHRElMech = -r.WorkWHREl / DeclarationData.AlternaterEfficiency;
			r.WorkWHRMech = -modData.TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr);
			
			if (runData.BusAuxiliaries != null) {
				var workBusAuxPSCompOff = modData.EnergyPneumaticCompressorPowerOff();
				var workBusAuxPSCompOn = modData.EnergyPneumaticCompressorAlwaysOn();
				var airBusAuxPSON = modData.AirGeneratedAlwaysOn();
				var deltaAir = modData.AirConsumed() - modData.AirGenerated();

				var kAir = airBusAuxPSON.IsEqual(0) ? 0.SI(Unit.SI.Watt.Second.Per.Cubic.Meter) :(workBusAuxPSCompOn - workBusAuxPSCompOff) / (airBusAuxPSON - 0.SI<NormLiter>());
				r.WorkBusAuxPSCorr = (kAir * deltaAir).Cast<WattSecond>();

				var workBusAuxES = modData.EnergyBusAuxESConsumed() - modData.EnergyBusAuxESGenerated();
				r.WorkBusAuxESMech = workBusAuxES /
									runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>()) /
									runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorGearEfficiency;

			} else {
				r.WorkBusAuxPSCorr = 0.SI<WattSecond>();
				r.WorkBusAuxESMech = 0.SI<WattSecond>();
			}

			var engineWasteheatSum = modData.FuelData.Aggregate(
				0.SI<Joule>(),
				(current, fuel) => current + modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel) *
					fuel.LowerHeatingValueVecto);

			r.EnergyDCDCMissing = 0.SI<WattSecond>();
			if (runData.BusAuxiliaries != null && runData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
				// either case C1, C2a, or C3a
				var missingDCDCEnergy = modData.TimeIntegral<WattSecond>(ModalResultField.P_DCDC_missing);
				if (runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart) {
					// case C3a
					if (runData.ElectricMachinesData.Count != 1) {
						throw new VectoException("exactly 1 electric machine is required. got {0} ({1})",
							runData.ElectricMachinesData.Count,
							string.Join(",", runData.ElectricMachinesData.Select(x => x.Item1.ToString())));
					}
					var emPos = runData.ElectricMachinesData.First().Item1;
					var averageEmEfficiencyCharging = modData.ElectricMotorEfficiencyGenerate(emPos);
					r.EnergyDCDCMissing = missingDCDCEnergy /
										runData.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency /
										averageEmEfficiencyCharging;
				} else {
					// case C1, C2a
					r.EnergyDCDCMissing = missingDCDCEnergy / DeclarationData.AlternaterEfficiency;
				}
			}

			r.AuxHeaterDemand = modData.AuxHeaterDemandCalc == null
				? 0.SI<Joule>()
				: modData.AuxHeaterDemandCalc(duration, engineWasteheatSum);

			var kilogramCO2PerMeter = 0.SI<KilogramPerMeter>();

			var firstFuel = true;
			foreach (var fuel in modData.FuelData) {
				var engFuel = runData.EngineData.Fuels.First(x => x.FuelData.Equals(fuel));
				var f = new FuelConsumptionCorrection();
				f.Fuel = fuel;
				f.Distance = distance != null && distance.IsGreater(0) ? distance : null;
				f.Duration = duration != null && duration.IsGreater(0) ? duration : null;

				f.EngineLineCorrectionFactor = modData.EngineLineCorrectionFactor(fuel);
				f.VehicleLine = modData.VehicleLineSlope(fuel);

				f.FcModSum = modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel);

				var fcIceOnAuxStandstill = engFuel.ConsumptionMap.GetFuelConsumptionValue(
					r.AvgAuxPowerICEOnStandstill / runData.EngineData.IdleSpeed, runData.EngineData.IdleSpeed);
				var fcIceOnAuxDriving = engFuel.ConsumptionMap.GetFuelConsumptionValue(
					r.AvgAuxPowerICEOnDriving / runData.EngineData.IdleSpeed, runData.EngineData.IdleSpeed);

				f.FcESS_EngineStart = f.EngineLineCorrectionFactor * modData.WorkEngineStart();
				f.FcESS_AuxStandstill_ICEOff = r.EnergyAuxICEOffStandstill * f.EngineLineCorrectionFactor *
												essParams.UtilityFactorStandstill;
				f.FcESS_AuxStandstill_ICEOn = fcIceOnAuxStandstill * r.ICEOffTimeStandstill *
											(1 - essParams.UtilityFactorStandstill);
				f.FcESS_AuxDriving_ICEOn = r.EnergyAuxICEOffDriving * f.EngineLineCorrectionFactor *
											essParams.UtilityFactorDriving;
				f.FcESS_AuxDriving_ICEOff = fcIceOnAuxDriving * r.ICEOffTimeDriving *
											(1 - essParams.UtilityFactorDriving);
				f.FcESS_DCDCMissing =
					r.EnergyDCDCMissing * f.EngineLineCorrectionFactor * essParams.UtilityFactorStandstill;

				f.FcBusAuxPs = f.EngineLineCorrectionFactor * r.WorkBusAuxPSCorr;
				f.FcBusAuxEs =  f.EngineLineCorrectionFactor * r.WorkBusAuxESMech;
				f.FcWHR =  f.EngineLineCorrectionFactor * r.WorkWHR;
				f.FcAuxHtr = 0.SI<Kilogram>();
				if (firstFuel) {
					firstFuel = false;
					f.FcAuxHtr = r.AuxHeaterDemand / fuel.LowerHeatingValueVecto;
				}
				
				kilogramCO2PerMeter += distance == null || distance.IsEqual(0)
					? 0.SI<KilogramPerMeter>()
					: f.FcFinal * fuel.CO2PerFuelWeight / distance;
				
				//--
				r.FuelCorrection[fuel.FuelType] = f;
			}

			r.KilogramCO2PerMeter = kilogramCO2PerMeter;
			return r;
		}

		#endregion
	}


	public class CorrectedModalData : ICorrectedModalData
	{
		public Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; }
		#region Implementation of ICorrectedModalData

		public CorrectedModalData()
		{
			FuelCorrection = new Dictionary<FuelType, IFuelConsumptionCorrection>();
		}

		//public WattSecond WorkESS { get; set; }
		public WattSecond WorkWHREl { get; set; }
		public WattSecond WorkWHRElMech { get; set; }
		public WattSecond WorkWHRMech { get; set; }
		public WattSecond WorkWHR => WorkWHRElMech + WorkWHRMech;
		public WattSecond WorkBusAuxPSCorr { get; set; }
		public WattSecond WorkBusAuxESMech { get; set; }
		public WattSecond WorkBusAuxCorr => WorkBusAuxPSCorr + WorkBusAuxESMech;

		public Joule AuxHeaterDemand { get; set; }
		public KilogramPerMeter KilogramCO2PerMeter { get; set; }

		public IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			if (!FuelCorrection.ContainsKey(fuel.FuelType)) {
				throw new VectoException("Invalid fuel {0}", fuel);
			}

			return FuelCorrection[fuel.FuelType];
		}

		public Kilogram CO2Total
		{
			get
			{
				return FuelCorrection.Sum(x => x.Value.TotalFuelConsumptionCorrected * x.Value.Fuel.CO2PerFuelWeight);
			}
		}

		public Joule EnergyConsumptionTotal
		{
			get
			{
				return FuelCorrection.Sum(x =>
					x.Value.TotalFuelConsumptionCorrected * x.Value.Fuel.LowerHeatingValueVecto);
			}
		}

		public Second ICEOffTimeStandstill { get; set; }
		public WattSecond EnergyAuxICEOffStandstill { get; set; }
		public WattSecond EnergyAuxICEOnStandstill { get; set; }
		public Watt AvgAuxPowerICEOnStandstill => ICEOffTimeStandstill.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnStandstill / ICEOffTimeStandstill;


		public Second ICEOffTimeDriving { get; set; }
		public WattSecond EnergyAuxICEOffDriving { get; set; }
		public WattSecond EnergyPowerICEOnDriving { get; set; }
		public Watt AvgAuxPowerICEOnDriving => ICEOffTimeDriving.IsEqual(0) ? 0.SI<Watt>() : EnergyPowerICEOnDriving / ICEOffTimeDriving;

		public WattSecond EnergyDCDCMissing { get; set; }

		#endregion
	}

	public class FuelConsumptionCorrection : IFuelConsumptionCorrection
	{

		public IFuelProperties Fuel { get; set; }
		public Meter Distance { get; set; }
		public Second Duration { get; set; }

		public Kilogram FcModSum { get; set; }

		public Kilogram FcESS_EngineStart { get; set; }
		public Kilogram FcESS_AuxStandstill_ICEOff { get; set; }
		public Kilogram FcESS_AuxStandstill_ICEOn { get; set; }
		public Kilogram FcESS_AuxDriving_ICEOn { get; set; }
		public Kilogram FcESS_AuxDriving_ICEOff { get; set; }

		public Kilogram FcESS_DCDCMissing { get; set; }

		public Kilogram FcESS =>
			FcESS_EngineStart + FcESS_AuxStandstill_ICEOff + FcESS_AuxStandstill_ICEOn
			+ FcESS_AuxDriving_ICEOn + FcESS_AuxDriving_ICEOff + FcESS_DCDCMissing;

		public Kilogram FcBusAuxPs { get; set; }
		public Kilogram FcBusAuxEs { get; set; }
		public Kilogram FcWHR { get; set; }
		public Kilogram FcAuxHtr { get; set; }


		public Kilogram FcEssCorr => FcModSum + FcESS;
		public Kilogram FcBusAuxPsCorr => FcEssCorr + FcBusAuxPs;
		public Kilogram FcBusAuxEsCorr => FcBusAuxPsCorr + FcBusAuxEs;
		public Kilogram FcWHRCorr => FcBusAuxEsCorr + FcWHR;
		public Kilogram FcAuxHtrCorr => FcWHRCorr + FcAuxHtr;

		public Kilogram FcFinal => FcAuxHtrCorr;

		#region Implementation of IFuelConsumptionCorrection

		public KilogramPerWattSecond EngineLineCorrectionFactor { get; set; }
		public KilogramPerWattSecond VehicleLine { get; set; }
		public KilogramPerSecond FC_ESS_CORR_H => Duration != null ? (FcEssCorr / Duration) : null;
		public KilogramPerSecond FC_BusAux_PS_CORR_H => Duration != null ? (FcBusAuxPsCorr / Duration) : null;
		public KilogramPerSecond FC_BusAux_ES_CORR_H => Duration != null ? (FcBusAuxEsCorr / Duration) : null;
		public KilogramPerSecond FC_WHR_CORR_H => Duration != null ? (FcWHRCorr / Duration) : null;
		public KilogramPerSecond FC_AUXHTR_H => Duration != null ? (FcAuxHtr / Duration) : null;
		public KilogramPerSecond FC_AUXHTR_H_CORR => Duration != null ? (FcAuxHtrCorr / Duration) : null;
		public KilogramPerSecond FC_FINAL_H => Duration != null ? FcFinal / Duration : null;

		public KilogramPerMeter FC_ESS_CORR_KM => Distance != null ? (FcEssCorr / Distance) : null;
		public KilogramPerMeter FC_WHR_CORR_KM => Distance != null ? (FcWHRCorr / Distance) : null;
		public KilogramPerMeter FC_BusAux_PS_CORR_KM => Distance != null ? (FcBusAuxPsCorr / Distance) : null;
		public KilogramPerMeter FC_BusAux_ES_CORR_KM => Distance != null ? (FcBusAuxEsCorr / Distance) : null;
		public KilogramPerMeter FC_AUXHTR_KM => Distance != null ? (FcAuxHtr / Distance) : null;
		public KilogramPerMeter FC_AUXHTR_KM_CORR => Distance != null ? (FcAuxHtrCorr / Distance) : null;
		public KilogramPerMeter FC_FINAL_KM => Distance != null ? FcFinal / Distance : null;

		public VolumePerMeter FuelVolumePerMeter =>
			Fuel.FuelDensity != null && Distance != null
				? (FcFinal / Distance / Fuel.FuelDensity).Cast<VolumePerMeter>()
				: null;

		public Kilogram TotalFuelConsumptionCorrected => FcFinal;

		public Joule EnergyDemand => FcFinal * Fuel.LowerHeatingValueVecto;

		#endregion
	}

}