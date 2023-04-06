using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public abstract class AbstractCorrectedModalData : ICorrectedModalData
    {
        protected readonly IModalDataContainer _modData;

        public AbstractCorrectedModalData(IModalDataContainer modData)
        {
            _modData = modData;
        }


        #region Implementation of ICorrectedModalData

        public virtual WattSecond WorkESSMissing { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHREl { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHRElMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHRMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkWHR => WorkWHRElMech + WorkWHRMech;
        public virtual WattSecond WorkBusAuxPSCorr { get; set; } = 0.SI<WattSecond>();
		public virtual WattSecond WorkBusAuxESMech { get; set; } = 0.SI<WattSecond>();
		public virtual WattSecond WorkBusAuxHeatPumpHeatingElMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkBusAuxHeatPumpHeatingMech { get; set; } = 0.SI<WattSecond>();
        public virtual WattSecond WorkBusAuxElectricHeater { get; set; } = 0.SI<WattSecond>();
		
		// electric compressor work applied to REESS when calculating electric range (HEV CD, PEV) 
		public virtual WattSecond WorkBusAux_elPS_SoC_ElRange { get; set; } = 0.SI<WattSecond>();

        // electric compressor work applied to REESS correcting deltaSoC (and consequently deltaFC)
		public virtual WattSecond WorkBusAux_elPS_SoC_Corr { get; set; } = 0.SI<WattSecond>();

        // electric compressor work applied as fuel consumption
        public virtual WattSecond WorkBusAux_elPS_el_Corr { get; set; } = 0.SI<WattSecond>();


        public virtual WattSecond WorkBusAuxCorr => WorkBusAuxPSCorr + WorkBusAuxESMech;
        public virtual WattSecond EnergyDCDCMissing { get; set; } = 0.SI<WattSecond>();
        public virtual Joule AuxHeaterDemand { get; set; } = 0.SI<WattSecond>();
		public virtual KilogramPerMeter KilogramCO2PerMeter { get; set; } = 0.SI<KilogramPerMeter>();
        public virtual Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection => new Dictionary<FuelType, IFuelConsumptionCorrection>();
        public virtual Kilogram CO2Total { get; set; } = 0.SI<Kilogram>();
        public virtual Joule FuelEnergyConsumptionTotal => 0.SI<Joule>();
        public virtual WattSecond ElectricEnergyConsumption_SoC { get; set; } = 0.SI<WattSecond>();
		public virtual WattSecond ElectricEnergyConsumption_SoC_Corr => ElectricEnergyConsumption_SoC - WorkBusAux_elPS_SoC_ElRange;
        public virtual WattSecond ElectricEnergyConsumption_Final { get; set; } = 0.SI<WattSecond>();


		public abstract IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel);

        #endregion

        public virtual NormLiter CorrectedAirDemand { get; set; }
        public virtual NormLiter DeltaAir { get; set; }

        public WattSecondPerMeter ElectricEnergyConsumption_SoC_PerMeter => ElectricEnergyConsumption_SoC == null || _modData.Distance.IsEqual(0)
            ? null
            : ElectricEnergyConsumption_SoC / _modData.Distance;

        public WattSecondPerMeter ElectricEnergyConsumption_Final_PerMeter =>
            ElectricEnergyConsumption_Final == null || _modData.Distance.IsEqual(0)
                ? null
                : ElectricEnergyConsumption_Final / _modData.Distance;
    }


    public class CorrectedModalData : AbstractCorrectedModalData
    {
        public SI kAir { get; set; }
        public Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; }
        #region Implementation of ICorrectedModalData

        public CorrectedModalData(IModalDataContainer modData) : base(modData)
        {

            FuelCorrection = new Dictionary<FuelType, IFuelConsumptionCorrection>();
        }

        public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
        {
            if (!FuelCorrection.ContainsKey(fuel.FuelType)) {
                throw new VectoException("Invalid fuel {0}", fuel);
            }

            return FuelCorrection[fuel.FuelType];
        }

        public  Second ICEOffTimeStandstill { get; set; }
        public WattSecond EnergyAuxICEOffStandstill { get; set; }
        public WattSecond EnergyAuxICEOffStandstill_UF {
            get {
                return EnergyAuxICEOffStandstill * UtilityFactorStandstill;
            }
        }

        public double UtilityFactorStandstill { get; set; }

        public WattSecond EnergyAuxICEOnStandstill { get; set; }
        public WattSecond EnergyAuxICEOnStandstill_UF {
            get { return EnergyAuxICEOnStandstill * (1 - UtilityFactorStandstill); }
        }
        public Watt AvgAuxPowerICEOnStandstill => ICEOffTimeStandstill.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnStandstill / ICEOffTimeStandstill;


        public Second ICEOffTimeDriving { get; set; }
        public WattSecond EnergyAuxICEOffDriving { get; set; }
        public WattSecond EnergyAuxICEOffDriving_UF { get { return EnergyAuxICEOffDriving * UtilityFactorDriving; } }
        public double UtilityFactorDriving { get; set; }

        public WattSecond EnergyAuxICEOnDriving { get; set; }
        public WattSecond EnergyAuxICEOnDriving_UF {
            get {
                return EnergyAuxICEOnDriving * (1 - UtilityFactorDriving);
            }
        }
        public Watt AvgAuxPowerICEOnDriving => ICEOffTimeDriving.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnDriving / ICEOffTimeDriving;

        public WattSecond EnergyDCDCMissing { get; set; }
        public NormLiter CorrectedAirDemand { get; set; }
        public NormLiter DeltaAir { get; set; }
        public WattSecond DeltaEReessMech { get; set; }

        #endregion
    }

	public class PEVCorrectedModalData : AbstractCorrectedModalData
	{

		public PEVCorrectedModalData(IModalDataContainer modData) : base(modData)
		{

		}

		public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			return new NoFuelConsumptionCorrection();
		}
	}

	public class EngineOnlyCorrectedModalData : AbstractCorrectedModalData
	{
		public EngineOnlyCorrectedModalData(IModalDataContainer modData) : base(modData)
		{

		}

		public override IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			return new NoFuelConsumptionCorrection();
		}
    }
}