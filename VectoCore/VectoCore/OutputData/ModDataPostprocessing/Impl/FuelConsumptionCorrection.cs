using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
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

        public Kilogram FcBusAuxPSAirDemand { get; set; }

        public Kilogram FcBusAuxElPS { get; set; }

        public Kilogram FcBusAuxPSDragICEOffStandstill { get; set; }
        public Kilogram FcBusAuxPSDragICEOffDriving { get; set; }
        public Kilogram FcBusAuxPs {
            get => FcBusAuxPSAirDemand + FcBusAuxPSDragICEOffDriving + FcBusAuxPSDragICEOffStandstill;
        }
        public Kilogram FcBusAuxEs { get; set; }
        public Kilogram FcWHR { get; set; }
        public Kilogram FcREESSSoc { get; set; }

        public Kilogram FcAuxHtr { get; set; }

        public Kilogram FcHeatPumpHeatingEl { get; set; }

        public Kilogram FcHeatPumpHeatingMech { get; set; }

        public Kilogram FcBusAuxEletcricHeater { get; set; }

        public Kilogram FcEssCorr => FcModSum + FcESS;
        public Kilogram FcBusAuxPsCorr => FcEssCorr + FcBusAuxPs + FcBusAuxElPS;
        public Kilogram FcBusAuxEsCorr => FcBusAuxPsCorr + FcBusAuxEs;
        public Kilogram FcBusAuxHeatingCorr => FcBusAuxEsCorr + FcHeatPumpHeatingEl + FcHeatPumpHeatingMech + FcBusAuxEletcricHeater;
        public Kilogram FcWHRCorr => FcBusAuxHeatingCorr + FcWHR;
        public Kilogram FcREESSSoCCorr => FcWHRCorr + FcREESSSoc;

        public Kilogram FcAuxHtrCorr => FcREESSSoCCorr + FcAuxHtr;

        public Kilogram FcFinal => FcAuxHtrCorr;

        #region Implementation of IFuelConsumptionCorrection

        public KilogramPerWattSecond EngineLineCorrectionFactor { get; set; }
        public KilogramPerWattSecond VehicleLine { get; set; }
        public KilogramPerSecond FC_ESS_H => Duration != null ? FcESS / Duration : null;
        public KilogramPerSecond FC_ESS_CORR_H => Duration != null ? FcEssCorr / Duration : null;
        public KilogramPerSecond FC_BusAux_PS_CORR_H => Duration != null ? FcBusAuxPsCorr / Duration : null;
        public KilogramPerSecond FC_BusAux_ES_CORR_H => Duration != null ? FcBusAuxEsCorr / Duration : null;
        public KilogramPerSecond FC_WHR_CORR_H => Duration != null ? FcWHRCorr / Duration : null;
        public KilogramPerSecond FC_AUXHTR_H => Duration != null ? FcAuxHtr / Duration : null;
        public KilogramPerSecond FC_AUXHTR_H_CORR => Duration != null ? FcAuxHtrCorr / Duration : null;
        public KilogramPerSecond FC_REESS_SOC_H => Duration != null ? FcREESSSoc / Duration : null;
        public KilogramPerSecond FC_REESS_SOC_CORR_H => Duration != null ? FcREESSSoCCorr / Duration : null;
        public KilogramPerSecond FC_FINAL_H => Duration != null ? FcFinal / Duration : null;

        public KilogramPerMeter FC_REESS_SOC_KM => Distance != null ? FcREESSSoc / Distance : null;
        public KilogramPerMeter FC_REESS_SOC_CORR_KM => Distance != null ? FcREESSSoCCorr / Distance : null;
        public KilogramPerMeter FC_ESS_KM => Distance != null ? FcESS / Distance : null;
        public KilogramPerMeter FC_ESS_CORR_KM => Distance != null ? FcEssCorr / Distance : null;
        public KilogramPerMeter FC_WHR_CORR_KM => Distance != null ? FcWHRCorr / Distance : null;
        public KilogramPerMeter FC_BusAux_PS_CORR_KM => Distance != null ? FcBusAuxPsCorr / Distance : null;
        public KilogramPerMeter FC_BusAux_ES_CORR_KM => Distance != null ? FcBusAuxEsCorr / Distance : null;
        public KilogramPerMeter FC_AUXHTR_KM => Distance != null ? FcAuxHtr / Distance : null;
        public KilogramPerMeter FC_AUXHTR_KM_CORR => Distance != null ? FcAuxHtrCorr / Distance : null;
        public KilogramPerMeter FC_FINAL_KM => Distance != null ? FcFinal / Distance : null;

        public VolumePerMeter FuelVolumePerMeter =>
            Fuel.FuelDensity != null && Distance != null
                ? (FcFinal / Distance / Fuel.FuelDensity).Cast<VolumePerMeter>()
                : null;

        public Kilogram TotalFuelConsumptionCorrected => FcFinal;

        public Joule EnergyDemand => FcFinal * Fuel.LowerHeatingValueVecto;

        #endregion
    }

	public class NoFuelConsumptionCorrection : IFuelConsumptionCorrection
	{
		#region Implementation of IFuelConsumptionCorrection

		public IFuelProperties Fuel { get; }
		public KilogramPerWattSecond EngineLineCorrectionFactor { get; }
		public KilogramPerWattSecond VehicleLine { get; }
		public KilogramPerSecond FC_ESS_H { get; }
		public KilogramPerSecond FC_ESS_CORR_H { get; }
		public KilogramPerSecond FC_BusAux_PS_CORR_H { get; }
		public KilogramPerSecond FC_BusAux_ES_CORR_H { get; }
		public KilogramPerSecond FC_WHR_CORR_H { get; }
		public KilogramPerSecond FC_AUXHTR_H { get; }
		public KilogramPerSecond FC_AUXHTR_H_CORR { get; }
		public KilogramPerSecond FC_REESS_SOC_H { get; }
		public KilogramPerSecond FC_REESS_SOC_CORR_H { get; }
		public KilogramPerSecond FC_FINAL_H { get; }
		public KilogramPerMeter FC_WHR_CORR_KM { get; }
		public KilogramPerMeter FC_BusAux_PS_CORR_KM { get; }
		public KilogramPerMeter FC_BusAux_ES_CORR_KM { get; }
		public KilogramPerMeter FC_AUXHTR_KM { get; }
		public KilogramPerMeter FC_AUXHTR_KM_CORR { get; }
		public KilogramPerMeter FC_REESS_SOC_KM { get; }
		public KilogramPerMeter FC_REESS_SOC_CORR_KM { get; }
		public KilogramPerMeter FC_ESS_KM { get; }
		public KilogramPerMeter FC_ESS_CORR_KM { get; }
		public KilogramPerMeter FC_FINAL_KM { get; }
		public VolumePerMeter FuelVolumePerMeter { get; }
		public Kilogram TotalFuelConsumptionCorrected { get; }
		public Joule EnergyDemand { get; }

		#endregion
	}


    /// <summary>
    /// Used for aux heaters
    /// </summary>
    public class PEVFuelConsumptionCorrection : IFuelConsumptionCorrection
	{
		private readonly Second _duration;
		private readonly Meter _distance;
		private readonly Kilogram _fcAuxHeater;
		private readonly IFuelProperties _fuel;

		public PEVFuelConsumptionCorrection(
			IFuelProperties fuel,
			Second duration,
			Meter distance,
			Kilogram fcAuxHeater
			)
		{
			_fuel = fuel;
			_duration = duration;
			_distance = distance;
			_fcAuxHeater = fcAuxHeater;
		}

		#region Implementation of IFuelConsumptionCorrection

		public IFuelProperties Fuel => _fuel;

		public KilogramPerWattSecond EngineLineCorrectionFactor => throw new System.NotImplementedException();

		public KilogramPerWattSecond VehicleLine => throw new System.NotImplementedException();

		public KilogramPerSecond FC_ESS_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_ESS_CORR_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_BusAux_PS_CORR_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_BusAux_ES_CORR_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_WHR_CORR_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_AUXHTR_H => _fcAuxHeater / _duration;

		public KilogramPerSecond FC_AUXHTR_H_CORR => FC_AUXHTR_H;

		public KilogramPerSecond FC_REESS_SOC_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_REESS_SOC_CORR_H => throw new System.NotImplementedException();

		public KilogramPerSecond FC_FINAL_H => throw new System.NotImplementedException();

		public KilogramPerMeter FC_WHR_CORR_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_BusAux_PS_CORR_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_BusAux_ES_CORR_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_AUXHTR_KM => _fcAuxHeater / _distance;

		public KilogramPerMeter FC_AUXHTR_KM_CORR => FC_AUXHTR_KM;

		public KilogramPerMeter FC_REESS_SOC_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_REESS_SOC_CORR_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_ESS_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_ESS_CORR_KM => throw new System.NotImplementedException();

		public KilogramPerMeter FC_FINAL_KM => throw new System.NotImplementedException();

		public VolumePerMeter FuelVolumePerMeter => throw new System.NotImplementedException();

		public Kilogram TotalFuelConsumptionCorrected => FC_AUXHTR_KM_CORR * _distance;

		public Joule EnergyDemand => throw new System.NotImplementedException();

		#endregion
	}
}