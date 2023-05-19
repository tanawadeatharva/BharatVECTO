using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{



	internal abstract class CompletedBusManufacturerReportBase : AbstractManufacturerReport, IXMLManufacturerReportCompletedBus
	{
		private class CompletedBusResult : IResultEntry
		{
			#region Implementation of IResultEntry

			public void Initialize(VectoRunData vectoRunData)
			{
				throw new NotImplementedException();
			}

			public VectoRunData VectoRunData { get; }
			public VectoRun.Status Status { get; set; }
			public OvcHevMode OVCMode { get; set; }
			public MissionType Mission { get; set; }
			public LoadingType LoadingType { get; }
			public int FuelMode { get; }
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
			public Watt MaxChargingPower { get; }
			public double WeightingFactor { get; }
			public Meter ActualChargeDepletingRange { get; set; }
			public Meter EquivalentAllElectricRange { get; set; }
			public Meter ZeroCO2EmissionsRange { get; set; }
			public IFuelProperties AuxHeaterFuel { get; }
			public Kilogram ZEV_FuelConsumption_AuxHtr { get; set; }
			public Kilogram ZEV_CO2 { get; }
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


		protected XNamespace _mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		private bool _allSuccess = true;
		public CompletedBusManufacturerReportBase(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }


		public override void Initialize(VectoRunData modelData)
		{
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;
			var inputData = modelData.InputData as IMultistepBusInputDataProvider;
			Input = inputData.JobInputData.PrimaryVehicle.Vehicle;
			if (inputData == null) {
				throw new VectoException("CompletedBus ManrufacturersRecordFile requires MultistepBusInputData");
			}
			Results = _resultFactory.GetMRFResultsWriter(modelData.VehicleData.VehicleCategory.GetVehicleType(),
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted);
			InputDataIntegrity = new XElement(Mrf_0_9 + XMLNames.Report_InputDataSignature,
				inputData.JobInputData.ConsolidateManufacturingStage.Signature == null
					? XMLHelper.CreateDummySig(_di)
					: inputData.JobInputData.ConsolidateManufacturingStage.Signature.ToXML(_di));
		}

		#region Implementation of IXMLManufacturerReportCompletedBus

		public double CalculateFactor<T>(
			(XMLDeclarationReport.ResultEntry genericResult,
				XMLDeclarationReport.ResultEntry specificResult) results,
			Func<XMLDeclarationReport.ResultEntry, T> access)
		{
			dynamic spec = access(results.specificResult);
			dynamic gen = access(results.genericResult);
			dynamic factor = spec / gen;
			if (factor is Scalar sc) {
				return sc.Value();
			}
			return (double)factor;
		}



		public virtual void WriteResult(XMLDeclarationReport.ResultEntry genericResult,
			XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			_allSuccess &= genericResult.Status == VectoRun.Status.Success;
			_allSuccess &= specificResult.Status == VectoRun.Status.Success;
			


			var result = new CompletedBusResult {
				Status = VectoRun.Status.Success,
			};
			result.Status = genericResult.Status != VectoRun.Status.Success ? genericResult.Status : result.Status;
			result.Status = specificResult.Status != VectoRun.Status.Success ? specificResult.Status : result.Status;
			result.OVCMode = specificResult.OVCMode;
			if (genericResult.OVCMode != specificResult.OVCMode) {
				throw new VectoException($"generic OVC Mode must be equal to specific OVC Mode! but was gen_ovc = {genericResult.OVCMode} != {specificResult.OVCMode} = spec_ovc");
			}
			result.BatteryData = specificResult.BatteryData;
			result.Mission = genericResult.Mission;
			result.Distance = genericResult.Distance;
			result.TotalVehicleMass = specificResult.TotalVehicleMass;
			result.Payload = specificResult.Payload;
			result.PassengerCount = specificResult.PassengerCount;
			result.VehicleClass = primaryResult.VehicleGroup;


			///Factor for each fuel
			/// Factor for electric Energy Consumption

			//TODO: Add primary bus group to writer 


			//TODO: 
			var combinedResults = (genericResult, specificResult);
			//var speedFactor = CalculateFactor(combinedResults, r => r.AverageSpeed);
			//result.AverageSpeed = primaryResult.
	
			//Info not available in Primary Results -> no factor method
			result.AverageSpeed = specificResult.AverageSpeed;
			result.AverageDrivingSpeed = specificResult.AverageDrivingSpeed;
			result.MinSpeed = specificResult.MinSpeed;
			result.MaxSpeed = specificResult.MaxSpeed;



			result.MaxDeceleration = specificResult.MaxDeceleration;
			result.MaxAcceleration = specificResult.MaxAcceleration;
			result.FullLoadPercentage = specificResult.FullLoadPercentage;
			result.GearshiftCount = specificResult.GearshiftCount;
			result.EngineSpeedDrivingAvg = specificResult.EngineSpeedDrivingAvg;
			result.EngineSpeedDrivingMin = specificResult.EngineSpeedDrivingMin;
			result.EngineSpeedDrivingMax = specificResult.EngineSpeedDrivingMax;
			result.AverageGearboxEfficiency = specificResult.AverageGearboxEfficiency;
			result.AverageAxlegearEfficiency = specificResult.AverageAxlegearEfficiency;



			//Fuels
			result.FuelData = specificResult.FuelData;
			var co2Sum = 0.SI<Kilogram>();
			foreach (var fuel in specificResult.CorrectedFinalFuelConsumption.Keys) {
				
				var fuelFactor = CalculateFactor(combinedResults, r => r.FuelConsumptionFinal(fuel).TotalFuelConsumptionCorrected);
				var completedFuelConsumption =
					fuelFactor * (primaryResult.EnergyConsumption[fuel] * specificResult.Distance );
                var fuelConsumption = new CompletedBusFuelConsumption() {
					Fuel = specificResult.FuelData.Single(f => f.FuelType == fuel),
					EnergyDemand = completedFuelConsumption,
				};
				co2Sum += fuelConsumption.TotalFuelConsumptionCorrected * fuelConsumption.Fuel.CO2PerFuelWeight;
				result.CorrectedFinalFuelConsumption.Add(fuel, fuelConsumption);
			}
			result.CO2Total = co2Sum;


			result.ElectricEnergyConsumption = null;
			
			if (primaryResult.ElectricEnergyConsumption != null){
				var electricEnergyFactor = CalculateFactor(combinedResults,
					r => r.ElectricEnergyConsumption);
				result.ElectricEnergyConsumption =
					electricEnergyFactor * primaryResult.ElectricEnergyConsumption * specificResult.Distance;
            }

			


			result.ZEV_FuelConsumption_AuxHtr = -777.SI<Kilogram>();

			if (genericResult.VectoRunData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
					VectoSimulationJobType.IEPC_E)) {
				var elRanges = DeclarationData.CalculateElectricRangesPEVCompletedBus(batteryData: result.BatteryData,
					result.ElectricEnergyConsumption, result.Distance);

				result.EquivalentAllElectricRange = elRanges.EquivalentAllElectricRange;
				result.ActualChargeDepletingRange = elRanges.ActualChargeDepletingRange;
				result.ZeroCO2EmissionsRange = elRanges.ZeroCO2EmissionsRange;
			} 


            _results.Add(result);
        }
		#endregion
	}

	internal class Conventional_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
    {
		public Conventional_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ConventionalCompletedBusManufacturerOutputDataType";



		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);

		}

		#endregion
	}

	internal class HEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public HEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEVCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport("HEVCompletedBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class PEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public PEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEVCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);
		}

		#endregion
	}

	internal class Exempted_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public Exempted_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ExemptedCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetExempted_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);
		}

		#endregion
	}
}
