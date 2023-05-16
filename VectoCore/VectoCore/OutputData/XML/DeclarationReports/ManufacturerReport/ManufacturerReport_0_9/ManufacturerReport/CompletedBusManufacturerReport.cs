using System;
using System.Collections.Generic;
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

			public string Error { get; }
			public string StackTrace { get; }
			public BatterySystemData BatteryData { get; set; }

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

			result.BatteryData = genericResult.BatteryData;
			result.Mission = genericResult.Mission;
			result.Distance = genericResult.Distance;
			result.TotalVehicleMass = specificResult.TotalVehicleMass;
			result.Payload = specificResult.Payload;
			result.PassengerCount = specificResult.PassengerCount;
			result.VehicleClass = primaryResult.VehicleGroup;


			//TODO: Add primary bus group to writer 


			//TODO: 
			result.AverageSpeed = 42.KMPHtoMeterPerSecond();
			result.AverageDrivingSpeed = 42.KMPHtoMeterPerSecond();
			result.MinSpeed = 42.KMPHtoMeterPerSecond();
			result.MaxSpeed = 42.KMPHtoMeterPerSecond();
			result.MaxDeceleration = 9.SI<MeterPerSquareSecond>();
			result.MaxAcceleration = 3.SI<MeterPerSquareSecond>();
			result.FullLoadPercentage = 2.SI<Scalar>();
			result.GearshiftCount = 42.SI<Scalar>();
			result.EngineSpeedDrivingAvg = 900.RPMtoRad();
			result.EngineSpeedDrivingMin = 600.RPMtoRad();
			result.EngineSpeedDrivingMax = 1200.RPMtoRad();
			result.AverageGearboxEfficiency = 80;
			result.AverageAxlegearEfficiency = 70;



			//Fuels
			result.FuelData = specificResult.FuelData;
			foreach (var fuel in specificResult.CorrectedFinalFuelConsumption.Keys) {
				
				var genFuel = genericResult.FuelConsumptionFinal(fuel);
				var specFuel = genericResult.FuelConsumptionFinal(fuel);
				result.CorrectedFinalFuelConsumption.Add(fuel, genFuel);
			}

			result.CO2Total = 6.SI<Kilogram>();



			result.ElectricEnergyConsumption = 42.SI<WattSecond>();
			result.ZEV_FuelConsumption_AuxHtr = 42.SI<Kilogram>();

			result.ActualChargeDepletingRange = 3.SI<Meter>();
			result.EquivalentAllElectricRange = 300.SI<Meter>();
			result.ZeroCO2EmissionsRange = 400.SI<Meter>();

			_results.Add(result);
        }




		private double CalculateEnergyFactor(XMLDeclarationReport.ResultEntry specific,
			XMLDeclarationReport.ResultEntry generic)
		{
			return specific.EnergyConsumptionTotal.Value() / generic.EnergyConsumptionTotal.Value();
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
