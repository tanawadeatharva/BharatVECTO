using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Mockup.Simulation.RundataFactories;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
    public class MockupModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			LoadModule<CIFMockupModule>();
			LoadModule<MRFMockupModule>();
			LoadModule<SimulatorFactoryModule>();
			LoadModule<VIFMockupModule>();

			Rebind<IVectoRunDataFactoryFactory>().To<VectoMockUpRunDataFactoryFactory>();
			Rebind<IXMLDeclarationReportFactory>().To<MockupReportFactory>();
			Rebind<IXMLInputDataReader>().To<MockupXMLInputDataFactory>();
			Rebind<IResultsWriterFactory>().To<MockupReportResultsFactory>().InSingletonScope();

			Rebind<IModalDataPostProcessor>().To<MockupPostProcessing>()
				.Named(VectoSimulationJobType.ConventionalVehicle.ToString());
        }

		#endregion
	}

	public class MockupPostProcessing : IModalDataPostProcessor
	{
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			return new NoCorrection();
		}

		#endregion
	}

	public class NoCorrection : ICorrectedModalData
	{
		#region Implementation of ICorrectedModalData

		public WattSecond WorkESSMissing { get; }
		public WattSecond WorkWHREl { get; }
		public WattSecond WorkWHRElMech { get; }
		public WattSecond WorkWHRMech { get; }
		public WattSecond WorkWHR { get; }
		public WattSecond WorkBusAuxPSCorr { get; }
		public WattSecond WorkBusAux_elPS_SoC_ElRange { get; }
		public WattSecond WorkBusAux_elPS_SoC_Corr { get; }
		public WattSecond WorkBusAux_elPS_Corr_mech { get; }
		public WattSecond WorkBusAuxESMech { get; }
		public WattSecond WorkBusAuxHeatPumpHeatingElMech { get; }
		public WattSecond WorkBusAuxHeatPumpHeatingMech { get; }
		public WattSecond WorkBusAuxElectricHeater { get; }
		public WattSecond WorkBusAuxCorr { get; }
		public WattSecond EnergyDCDCMissing { get; }
		public Joule AuxHeaterDemand { get; }
		public NormLiter CorrectedAirDemand { get; }
		public NormLiter DeltaAir { get; }
		public IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			throw new NotImplementedException();
		}

		public KilogramPerMeter KilogramCO2PerMeter { get; }
		public Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; }
		public Kilogram CO2Total { get; }
		public Joule FuelEnergyConsumptionTotal { get; }
		public WattSecond ElectricEnergyConsumption_SoC { get; set; }
		public WattSecond ElectricEnergyConsumption_SoC_Corr { get; }
		public WattSecondPerMeter ElectricEnergyConsumption_SoC_PerMeter { get; }
		public WattSecond ElectricEnergyConsumption_Final { get; set; }
		public WattSecondPerMeter ElectricEnergyConsumption_Final_PerMeter { get; }

		#endregion
	}
}
