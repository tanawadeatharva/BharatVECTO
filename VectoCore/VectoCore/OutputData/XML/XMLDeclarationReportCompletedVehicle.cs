using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
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

				(ManufacturerRpt as IXMLManufacturerReportCompletedBus)?.WriteResult(genericResult, specificResult, primaryResult);
				(CustomerRpt as IXMLCustomerReportCompletedBus)?.WriteResult(genericResult, specificResult, primaryResult);
			}

			GenerateReports();

			if (Writer != null)
			{
				OutputReports();
			}
		}

	}
}