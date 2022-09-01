using System;
using System.Linq;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.Reader
{
	public interface IInternalRunDataFactoryFactory
	{
		IVectoRunDataFactory CreateDeclarationRunDataFactory(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);
	}


	public class VectoRunDataFactoryFactory : IVectoRunDataFactoryFactory
	{
		private readonly IInternalRunDataFactoryFactory _internalFactory;

		public VectoRunDataFactoryFactory(IInternalRunDataFactoryFactory internalFactory)
		{
			_internalFactory = internalFactory;
		}
		/// <summary>
		/// Creates a VectoRunDataFactory based on the type of inputDataProvider
		/// </summary>
		/// <param name="inputDataProvider"></param>
		/// <param name="report"></param>
		/// <returns></returns>
		public IVectoRunDataFactory CreateDeclarationRunDataFactory(IInputDataProvider inputDataProvider,
			IDeclarationReport report, IVTPReport vtpReport)
		{
			if (inputDataProvider == null)
				throw new ArgumentNullException(nameof(inputDataProvider));

			switch (inputDataProvider)
			{
				case IVTPDeclarationInputDataProvider vtpProvider:
					return CreateRunDataReader(vtpProvider, vtpReport);
				case ISingleBusInputDataProvider singleBusProvider:
					return CreateRunDataReader(singleBusProvider, report);
				case IDeclarationInputDataProvider declDataProvider:
					return CreateRunDataReader(declDataProvider, report);
				case IMultistageVIFInputData multistageVifInputData:
					return CreateRunDataReader(multistageVifInputData, report);
				default:
					break;
			}
			throw new VectoException("Unknown InputData for Declaration Mode!");
		}


		private IVectoRunDataFactory CreateRunDataReader(IMultistageVIFInputData multistageVifInputData, IDeclarationReport report)
		{
			if (multistageVifInputData.VehicleInputData == null) {
				return new DeclarationModeCompletedMultistageBusVectoRunDataFactory(
					multistageVifInputData.MultistageJobInputData,
					report);
			}
			else {
				return new DeclarationModeMultistageBusVectoRunDataFactory(multistageVifInputData, report);
			}
		}

		private IVectoRunDataFactory CreateRunDataReader(IDeclarationInputDataProvider declDataProvider, IDeclarationReport report)
		{
			//var vehicleCategory = declDataProvider.JobInputData.Vehicle.VehicleCategory;
			//if (vehicleCategory.IsLorry()) {
			//	return new DeclarationModeTruckVectoRunDataFactory(declDataProvider, report);
			//}

			//if (vehicleCategory.IsBus())
			//	switch (declDataProvider.JobInputData.Vehicle.VehicleCategory)
			//	{
			//		case VehicleCategory.HeavyBusCompletedVehicle:
			//			return new DeclarationModeCompletedBusVectoRunDataFactory(declDataProvider, report);
			//		case VehicleCategory.HeavyBusPrimaryVehicle:
			//			return new DeclarationModePrimaryBusVectoRunDataFactory(declDataProvider, report);
			//		default:
			//			break;
			//	}
			var vehicle = declDataProvider.JobInputData.Vehicle;
			try {
				
				var ihpc = (vehicle.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None")  > 0;
				var iepc = (vehicle.Components?.IEPC != null);
				return _internalFactory.CreateDeclarationRunDataFactory(declDataProvider.JobInputData.Vehicle.VehicleCategory, 
					declDataProvider.JobInputData.JobType,
					declDataProvider.JobInputData.Vehicle.ArchitectureID,
					declDataProvider.JobInputData.Vehicle.ExemptedVehicle, iepc, ihpc);
			} catch (Exception ex) {
				throw new Exception(
					$"Could not create RunDataFactory for Vehicle Category{declDataProvider.JobInputData.Vehicle.VehicleCategory} {declDataProvider.JobInputData.Vehicle.ArchitectureID}", ex);
			}
			
		}

		private IVectoRunDataFactory CreateRunDataReader(IVTPDeclarationInputDataProvider vtpProvider, IDeclarationReport report)
		{
			var vtpReport = CastReport<IVTPReport>(report);

			if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry())
			{
				return new DeclarationVTPModeVectoRunDataFactoryLorries(vtpProvider, vtpReport);
			}

			if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus())
			{
				return new DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider, vtpReport);
			}
			

			throw new Exception(
				$"Could not create RunDataFactory for Vehicle Category{vtpProvider.JobInputData.Vehicle.VehicleCategory}");
		}

		private IVectoRunDataFactory CreateRunDataReader(ISingleBusInputDataProvider singleBusProvider, IDeclarationReport report)
		{
			return new DeclarationModeSingleBusVectoRunDataFactory(singleBusProvider, report);
		}




		public IVectoRunDataFactory CreateEngineeringRunDataFactory(IEngineeringInputDataProvider inputDataProvider)
		{
			throw new NotImplementedException();
		}

		private T CastReport<T>(object report)
		{
			if (report is T castedReport) {
				return castedReport;
			} else {
				throw new VectoException("Error creating VectoRunDataFactory - wrong ReportType");
				//return null;
			}
		}
	}
}
