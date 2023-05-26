using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.OutputData.XML
{
	/// <summary>
	/// Create MRF and CIF of the complete(d) step
	/// </summary>
	public class XMLDeclarationReportCompletedVehicle : XMLDeclarationReport
	{
		#region Constructors
		//public XMLDeclarationReportCompletedVehicle(IReportWriter writer) : base(writer) { }
		public XMLDeclarationReportCompletedVehicle(IReportWriter writer, IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory,
			IVIFReportFactory vifFactory) : base(writer, mrfFactory, cifFactory)
		{
		}

		#endregion

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleReportInputData { get; set; }

		#region Overrides of XMLDeclarationReportCompletedVehicle

		protected override void InstantiateReports(VectoRunData modelData)
		{
			var inputData = modelData.InputData as IXMLMultistageInputDataProvider;
			var primaryVehicle = inputData.JobInputData.PrimaryVehicle.Vehicle;

			var ihpc = (primaryVehicle.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (primaryVehicle.Components?.IEPC != null);
			ManufacturerRpt = _mrfFactory.GetManufacturerReport(
				inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				inputData.JobInputData.JobType,
				primaryVehicle.ArchitectureID,
				primaryVehicle.ExemptedVehicle,
				iepc,
				ihpc);



			CustomerRpt = _cifFactory.GetCustomerReport(
				inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				inputData.JobInputData.JobType,
				primaryVehicle.ArchitectureID,
				primaryVehicle.ExemptedVehicle,
				iepc,
				ihpc);


		}
		public override void InitializeReport(VectoRunData modelData)
		{
			_weightingFactors = EqualWeighting;

			InstantiateReports(modelData);

			ManufacturerRpt.Initialize(modelData);
			CustomerRpt.Initialize(modelData);
		}
		#endregion



		private static IDictionary<Tuple<MissionType, LoadingType>, double> EqualWeighting =>
			new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(
				new Dictionary<Tuple<MissionType, LoadingType>, double>() {
					{ Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Construction, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.HeavyUrban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.HeavyUrban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Urban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Urban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Suburban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Suburban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Interurban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Interurban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Coach, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Coach, LoadingType.ReferenceLoad), 1 },
				});

		protected internal override void DoWriteReport()
		{
			foreach (var specificResult in Results.Where(x => x.VehicleClass.IsCompletedBus()).OrderBy(x => x.VehicleClass)
						.ThenBy(x => x.FuelMode).ThenBy(x => x.Mission))
			{

				var genericResult = Results.First(x => x.VehicleClass.IsPrimaryBus() 
														&& x.FuelMode == specificResult.FuelMode 
														&& x.Mission == specificResult.Mission 
														&& x.LoadingType == specificResult.LoadingType 
														&& x.OVCMode == specificResult.OVCMode);
				var primaryResult = genericResult.PrimaryResult ?? specificResult.PrimaryResult;
				if (primaryResult == null)
				{
					throw new VectoException(
						"no primary result entry set for simulation run vehicle class: {0}, mission: {1}, payload: {2}",
						genericResult.VehicleClass, genericResult.Mission, genericResult.Payload);
				}

				(ManufacturerRpt as IXMLManufacturerReportCompletedBus)?.WriteResult(genericResult, specificResult, primaryResult, GetCompletedResult);
				(CustomerRpt as IXMLCustomerReportCompletedBus)?.WriteResult(genericResult, specificResult, primaryResult, GetCompletedResult);
			}

			GenerateReports();

			if (Writer != null)
			{
				OutputReports();
			}
		}
		private double CalculateFactor<T>(
			(IResultEntry genericResult,
				IResultEntry specificResult) results,
			Func<IResultEntry, T> access)
		{
			dynamic spec = access(results.specificResult);
			dynamic gen = access(results.genericResult);
			dynamic factor = spec / gen;
			if (factor is Scalar sc)
			{
				return sc.Value();
			}
			return (double)factor;
		}



        private IResultEntry GetCompletedResult(IResultEntry generic, IResultEntry specific, IResult primary)
		{

			var result = new CompletedBusResult
            {
                Status = VectoRun.Status.Success,
            };
            result.Status = generic.Status != VectoRun.Status.Success ? generic.Status : result.Status;
            result.Status = specific.Status != VectoRun.Status.Success ? specific.Status : result.Status;
            result.OVCMode = specific.OVCMode;
            if (generic.OVCMode != specific.OVCMode)
            {
                throw new VectoException($"generic OVC Mode must be equal to specific OVC Mode! but was gen_ovc = {generic.OVCMode} != {specific.OVCMode} = spec_ovc");
            }
            result.BatteryData = specific.BatteryData;
            result.Mission = generic.Mission;
            result.Distance = generic.Distance;
            result.TotalVehicleMass = specific.TotalVehicleMass;
            result.Payload = specific.Payload;
            result.PassengerCount = specific.PassengerCount;
			result.VehicleClass = specific.VehicleClass;
			result.PrimaryVehicleClass = primary.VehicleGroup;
            
			result.WeightingFactor = specific.WeightingFactor;
			result.FuelMode = specific.FuelMode;
			result.LoadingType = specific.LoadingType;

            ///Factor for each fuel
            /// Factor for electric Energy Consumption

            //TODO: Add primary bus group to writer 


            //TODO: 
            var combinedResults = (generic, specific);
            //var speedFactor = CalculateFactor(combinedResults, r => r.AverageSpeed);
            //result.AverageSpeed = primaryResult.

            //Info not available in Primary Results -> no factor method
            result.AverageSpeed = specific.AverageSpeed;
            result.AverageDrivingSpeed = specific.AverageDrivingSpeed;
            result.MinSpeed = specific.MinSpeed;
            result.MaxSpeed = specific.MaxSpeed;


			result.MaxChargingPower = specific.MaxChargingPower;
            result.MaxDeceleration = specific.MaxDeceleration;
            result.MaxAcceleration = specific.MaxAcceleration;
            result.FullLoadPercentage = specific.FullLoadPercentage;
            result.GearshiftCount = specific.GearshiftCount;
            result.EngineSpeedDrivingAvg = specific.EngineSpeedDrivingAvg;
            result.EngineSpeedDrivingMin = specific.EngineSpeedDrivingMin;
            result.EngineSpeedDrivingMax = specific.EngineSpeedDrivingMax;
            result.AverageGearboxEfficiency = specific.AverageGearboxEfficiency;
            result.AverageAxlegearEfficiency = specific.AverageAxlegearEfficiency;



            //Fuels
            result.FuelData = specific.FuelData;
            var co2Sum = 0.SI<Kilogram>();

            foreach (var fuel in generic.FuelData.Select(f => f.FuelType))
            {

                var fuelFactor = CalculateFactor(combinedResults, r => r.FuelConsumptionFinal(fuel).TotalFuelConsumptionCorrected);
                var completedFuelConsumption =
                    fuelFactor * (primary.EnergyConsumption[fuel] * specific.Distance);
                var fuelConsumption = new CompletedBusFuelConsumption()
                {
                    Fuel = specific.FuelData.Single(f => f.FuelType == fuel),
                    EnergyDemand = completedFuelConsumption,
                };
                co2Sum += fuelConsumption.TotalFuelConsumptionCorrected * fuelConsumption.Fuel.CO2PerFuelWeight;
                result.CorrectedFinalFuelConsumption.Add(fuel, fuelConsumption);
            }
            result.CO2Total = co2Sum;


            result.ElectricEnergyConsumption = null;

            if (!(primary.ElectricEnergyConsumption?.IsEqual(0) ?? true))
            {
                var electricEnergyFactor = CalculateFactor(combinedResults,
                    r => r.ElectricEnergyConsumption);
                result.ElectricEnergyConsumption =
                    electricEnergyFactor * primary.ElectricEnergyConsumption * specific.Distance;

            }



            if (specific.ZEV_FuelConsumption_AuxHtr?.IsGreaterOrEqual(0) ?? false)
            {
                result.ZEV_FuelConsumption_AuxHtr = specific.ZEV_FuelConsumption_AuxHtr;
                var auxHeaterFuel = specific.AuxHeaterFuel;
                result.AuxHeaterFuel = auxHeaterFuel;
				result.ZEV_CO2 = result.ZEV_FuelConsumption_AuxHtr * auxHeaterFuel.CO2PerFuelWeight;
            }




            if (generic.VectoRunData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
                    VectoSimulationJobType.IEPC_E))
            {
                var elRanges = DeclarationData.CalculateElectricRangesPEVCompletedBus(batteryData: result.BatteryData,
                    result.ElectricEnergyConsumption, result.Distance);

                result.EquivalentAllElectricRange = elRanges.EquivalentAllElectricRange;
                result.ActualChargeDepletingRange = elRanges.ActualChargeDepletingRange;
                result.ZeroCO2EmissionsRange = elRanges.ZeroCO2EmissionsRange;
            }

			return result;
		}




        private class CompletedBusResult : IResultEntry
        {
            #region Implementation of IResultEntry

            public void Initialize(VectoRunData vectoRunData)
            {
                throw new NotImplementedException();
            }

			public VectoRunData VectoRunData => throw new NotImplementedException();
            public VectoRun.Status Status { get; set; }
            public OvcHevMode OVCMode { get; set; }
            public MissionType Mission { get; set; }
			public LoadingType LoadingType { get; set; }

			public int FuelMode { get; set; }

			public IList<IFuelProperties> FuelData { get; set; }
            public MeterPerSecond AverageSpeed { get; set; }
            public MeterPerSecond AverageDrivingSpeed { get; set; }
            public MeterPerSecond MaxSpeed { get; set; }
            public MeterPerSecond MinSpeed { get; set; }
            public MeterPerSquareSecond MaxDeceleration { get; set; }
            public MeterPerSquareSecond MaxAcceleration { get; set; }
            public PerSecond EngineSpeedDrivingMin { get; set; }
            public PerSecond EngineSpeedDrivingAvg { get; set; }
            public PerSecond EngineSpeedDrivingMax { get; set; }
            public double AverageGearboxEfficiency { get; set; }
            public double AverageAxlegearEfficiency { get; set; }
            public Scalar FullLoadPercentage { get; set; }
            public Scalar GearshiftCount { get; set; }
            public Meter Distance { get; set; }


            public Dictionary<FuelType, IFuelConsumptionCorrection> CorrectedFinalFuelConsumption =
                new Dictionary<FuelType, IFuelConsumptionCorrection>();
            public IFuelConsumptionCorrection FuelConsumptionFinal(FuelType fuelType)
            {
                return CorrectedFinalFuelConsumption.ContainsKey(fuelType) ? CorrectedFinalFuelConsumption[fuelType] : null;
            }

            public WattSecond ElectricEnergyConsumption { get; set; }
            public Kilogram CO2Total { get; set; }
            public Kilogram Payload { get; set; }
            public Kilogram TotalVehicleMass { get; set; }
            public CubicMeter CargoVolume { get; }
            public double? PassengerCount { get; set; }
            public VehicleClass VehicleClass { get; set; }

			public VehicleClass? PrimaryVehicleClass { get; set; }

			public Watt MaxChargingPower { get; set; }
			public double WeightingFactor { get; set; }
            public Meter ActualChargeDepletingRange { get; set; }
            public Meter EquivalentAllElectricRange { get; set; }
            public Meter ZeroCO2EmissionsRange { get; set; }
            public IFuelProperties AuxHeaterFuel { get; set; }
            public Kilogram ZEV_FuelConsumption_AuxHtr { get; set; }
            public Kilogram ZEV_CO2 { get; set; }

            public void SetResultData(VectoRunData runData, IModalDataContainer data, double weightingFactor)
            {
                throw new NotImplementedException();
            }

            public string Error => throw new NotImplementedException();
            public string StackTrace => throw new NotImplementedException();
            public BatterySystemData BatteryData
            {
                get;
                set;
            }

            #endregion
        }

        private class CompletedBusFuelConsumption : IFuelConsumptionCorrection
        {
            #region Implementation of IFuelConsumptionCorrection

            public IFuelProperties Fuel { get; set; }

            public KilogramPerWattSecond EngineLineCorrectionFactor => throw new NotImplementedException();

            public KilogramPerWattSecond VehicleLine => throw new NotImplementedException();

            public KilogramPerSecond FC_ESS_H => throw new NotImplementedException();

            public KilogramPerSecond FC_ESS_CORR_H => throw new NotImplementedException();

            public KilogramPerSecond FC_BusAux_PS_CORR_H => throw new NotImplementedException();

            public KilogramPerSecond FC_BusAux_ES_CORR_H => throw new NotImplementedException();

            public KilogramPerSecond FC_WHR_CORR_H => throw new NotImplementedException();

            public KilogramPerSecond FC_AUXHTR_H => throw new NotImplementedException();

            public KilogramPerSecond FC_AUXHTR_H_CORR => throw new NotImplementedException();

            public KilogramPerSecond FC_REESS_SOC_H => throw new NotImplementedException();

            public KilogramPerSecond FC_REESS_SOC_CORR_H => throw new NotImplementedException();

            public KilogramPerSecond FC_FINAL_H => throw new NotImplementedException();

            public KilogramPerMeter FC_WHR_CORR_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_BusAux_PS_CORR_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_BusAux_ES_CORR_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_AUXHTR_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_AUXHTR_KM_CORR => throw new NotImplementedException();

            public KilogramPerMeter FC_REESS_SOC_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_REESS_SOC_CORR_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_ESS_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_ESS_CORR_KM => throw new NotImplementedException();

            public KilogramPerMeter FC_FINAL_KM => throw new NotImplementedException();

            public VolumePerMeter FuelVolumePerMeter => throw new NotImplementedException();

            public Kilogram TotalFuelConsumptionCorrected => EnergyDemand / Fuel.LowerHeatingValueVecto;

            public Joule EnergyDemand { get; set; }

            #endregion
        }


    }
}