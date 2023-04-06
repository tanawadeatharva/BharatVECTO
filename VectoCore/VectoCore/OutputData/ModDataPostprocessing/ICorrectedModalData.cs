using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing
{
    public interface ICorrectedModalData
    {
        WattSecond WorkESSMissing { get; }
        WattSecond WorkWHREl { get; }
        WattSecond WorkWHRElMech { get; }
        WattSecond WorkWHRMech { get; }
        WattSecond WorkWHR { get; }
        WattSecond WorkBusAuxPSCorr { get; }
		WattSecond WorkBusAux_elPS_SoC_ElRange { get; }
		WattSecond WorkBusAuxESMech { get; }
        WattSecond WorkBusAuxHeatPumpHeatingElMech { get; }
        WattSecond WorkBusAuxHeatPumpHeatingMech { get; }

        WattSecond WorkBusAuxElectricHeater { get; }

        WattSecond WorkBusAuxCorr { get; }
        WattSecond EnergyDCDCMissing { get; }
        Joule AuxHeaterDemand { get; }

        IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel);

        KilogramPerMeter KilogramCO2PerMeter { get; }
        Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; }
        Kilogram CO2Total { get; }
        Joule FuelEnergyConsumptionTotal { get; }
        WattSecond ElectricEnergyConsumption_SoC { get; set; }
        WattSecond ElectricEnergyConsumption_SoC_Corr { get; }
        WattSecondPerMeter ElectricEnergyConsumption_SoC_PerMeter { get; }

        WattSecond ElectricEnergyConsumption_Final { get; set; }
        WattSecondPerMeter ElectricEnergyConsumption_Final_PerMeter { get; }

    }
}