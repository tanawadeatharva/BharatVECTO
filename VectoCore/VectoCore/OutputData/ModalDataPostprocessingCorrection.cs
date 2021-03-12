using System.Collections.Generic;
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

			r.WorkESS = modData.WorkAuxiliariesDuringEngineStop() + modData.WorkEngineStart();
			
			r.WorkWHREl = modData.TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr);
			r.WorkWHRElMech = -r.WorkWHREl / DeclarationData.AlternaterEfficiency;
			r.WorkWHRMech = -modData.TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr);
			//r.WorkWHR = r.WorkWHRElMech + r.WorkWHRMech;

			if (runData.BusAuxiliaries != null) {
				var workBusAuxPSCompOff = modData.EnergyPneumaticCompressorPowerOff();
				var workBusAuxPSCompOn = modData.EnergyPneumaticCompressorAlwaysOn();
				var airBusAuxPSON = modData.AirGeneratedAlwaysOn();
				var deltaAir = modData.AirConsumed() - modData.AirGenerated();

				var kAir = (workBusAuxPSCompOn - workBusAuxPSCompOff) / (airBusAuxPSON - 0.SI<NormLiter>());
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

			r.AuxHeaterDemand = modData.AuxHeaterDemandCalc == null
				? 0.SI<Joule>()
				: modData.AuxHeaterDemandCalc(duration, engineWasteheatSum);

			var kilogramCO2PerMeter = 0.SI<KilogramPerMeter>();

			var firstFuel = true;
			foreach (var fuel in modData.FuelData) {
				var f = new FuelConsumptionCorrection();
				f.Fuel = fuel;
				f.Distance = distance != null && distance.IsGreater(0) ? distance : null;
				f.Duration = duration != null && duration.IsGreater(0) ? duration : null;

				f.EngineLineCorrectionFactor = modData.EngineLineCorrectionFactor(fuel);
				f.VehicleLine = modData.VehicleLineSlope(fuel);

				f.FcModSum = modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel);

				f.FcEssCorr = f.FcModSum + f.EngineLineCorrectionFactor * r.WorkESS;
				f.FcBusAuxPsCorr = f.FcEssCorr + f.EngineLineCorrectionFactor * r.WorkBusAuxPSCorr;
				f.FcBusAuxEsCorr = f.FcBusAuxPsCorr + f.EngineLineCorrectionFactor * r.WorkBusAuxESMech;
				f.FcWHRCorr = f.FcBusAuxEsCorr + f.EngineLineCorrectionFactor * r.WorkWHR;
				f.FcAuxHtr = 0.SI<Kilogram>();
				if (firstFuel) {
					firstFuel = false;
					f.FcAuxHtr = r.AuxHeaterDemand / fuel.LowerHeatingValueVecto;
				}
				f.FcAuxHtrCorr = f.FcWHRCorr + f.FcAuxHtr;
				f.FcFinal = f.FcAuxHtrCorr;

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

		public WattSecond WorkESS { get; set; }
		public WattSecond WorkWHREl { get; set; }
		public WattSecond WorkWHRElMech { get; set; }
		public WattSecond WorkWHRMech { get; set; }
		public WattSecond WorkWHR
		{
			get { return WorkWHRElMech + WorkWHRMech; }
		}
		public WattSecond WorkBusAuxPSCorr { get; set; }
		public WattSecond WorkBusAuxESMech { get; set; }
		public WattSecond WorkBusAuxCorr
		{
			get { return WorkBusAuxPSCorr + WorkBusAuxESMech; }
		}

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

		#endregion
	}

	public class FuelConsumptionCorrection : IFuelConsumptionCorrection
	{
		
		public IFuelProperties Fuel { get; set; }
		public Meter Distance { get; set; }
		public Second Duration { get; set; }

		public Kilogram FcModSum { get; set; }
		public Kilogram FcEssCorr { get; set; }
		public Kilogram FcBusAuxPsCorr { get; set; }
		public Kilogram FcBusAuxEsCorr { get; set; }
		public Kilogram FcWHRCorr { get; set; }
		public Kilogram FcAuxHtrCorr { get; set; }
		public Kilogram FcAuxHtr { get; set; }
		public Kilogram FcFinal { get; set; }

		#region Implementation of IFuelConsumptionCorrection

		public KilogramPerWattSecond EngineLineCorrectionFactor { get; set; }
		public KilogramPerWattSecond VehicleLine { get; set; }
		public KilogramPerSecond FC_ESS_CORR_H { get { return Duration != null ? (FcEssCorr / Duration) : null; }  }
		public KilogramPerSecond FC_BusAux_PS_CORR_H { get { return Duration != null ? (FcBusAuxPsCorr / Duration) : null; }  }
		public KilogramPerSecond FC_BusAux_ES_CORR_H { get { return Duration != null ? (FcBusAuxEsCorr / Duration) : null; }  }
		public KilogramPerSecond FC_WHR_CORR_H { get { return Duration != null ? (FcWHRCorr / Duration) : null; }  }
		public KilogramPerSecond FC_AUXHTR_H { get { return Duration != null ? (FcAuxHtr / Duration) : null; }  }
		public KilogramPerSecond FC_AUXHTR_H_CORR { get { return Duration != null ? (FcAuxHtrCorr / Duration) : null; }  }
		public KilogramPerSecond FC_FINAL_H { get { return Duration != null ? FcFinal / Duration : null; }  }

		public KilogramPerMeter FC_ESS_CORR_KM { get { return Distance != null ? (FcEssCorr / Distance) : null; } }
		public KilogramPerMeter FC_WHR_CORR_KM { get { return Distance != null ? (FcWHRCorr / Distance) : null; } }
		public KilogramPerMeter FC_BusAux_PS_CORR_KM { get { return Distance != null ? (FcBusAuxPsCorr / Distance) : null; } }
		public KilogramPerMeter FC_BusAux_ES_CORR_KM { get { return Distance != null ? (FcBusAuxEsCorr / Distance) : null; } }
		public KilogramPerMeter FC_AUXHTR_KM { get { return Distance != null ? (FcAuxHtr / Distance) : null; } }
		public KilogramPerMeter FC_AUXHTR_KM_CORR { get { return Distance != null ? (FcAuxHtrCorr / Distance) : null; } }
		public KilogramPerMeter FC_FINAL_KM { get { return Distance != null ? FcFinal / Distance : null; } }

		public VolumePerMeter FuelVolumePerMeter
		{
			get
			{
				return Fuel.FuelDensity != null && Distance != null
					? (FcFinal / Distance / Fuel.FuelDensity).Cast<VolumePerMeter>()
					: null;
			}
		}

		public Kilogram TotalFuelConsumptionCorrected
		{
			get { return FcFinal; }
		}

		public Joule EnergyDemand
		{
			get { return FcFinal * Fuel.LowerHeatingValueVecto; }
		}
		#endregion
	}

}