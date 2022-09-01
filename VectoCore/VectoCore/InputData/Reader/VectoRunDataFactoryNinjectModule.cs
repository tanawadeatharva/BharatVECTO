using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.Reader
{
	public class VectoRunDataFactoryNinjectModule : AbstractNinjectModule
	{

		private IVehicleTypeAndArchitectureStringHelper _vehicleStringHelper = new VehicleTypeAndArchitectureStringHelperRundata();

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IVectoRunDataFactoryFactory>().To<VectoRunDataFactoryFactory>().InSingletonScope();


			Bind<IInternalRunDataFactoryFactory>().ToFactory(
					() => new CombineArgumentsToNameInstanceProvider(
										_vehicleStringHelper.CreateName,
										6, 6,
										typeof(IInternalRunDataFactoryFactory)
											.GetMethod(nameof(IInternalRunDataFactoryFactory
											.CreateDeclarationRunDataFactory)), 
										typeof(IInternalRunDataFactoryFactory)
											.GetMethod(nameof(IInternalRunDataFactoryFactory
											.CreateDeclarationCompletedBusRunDataFactory))
										)
				)
				.InSingletonScope();


			#region Lorries

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ConventionalVehicle));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC));
			

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));

			Bind<IVectoRunDataFactory>().To<DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.Lorry, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC));


			#endregion

			#region PrimaryBus
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ConventionalVehicle));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));
			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));

			Bind<IVectoRunDataFactory>().To<DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.PrimaryBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC));
			#endregion


			#region CompletedBus
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.Conventional>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ConventionalVehicle,
					ArchitectureID.UNKNOWN));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.SerialHybridVehicle,
					ArchitectureID.S_IEPC));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P1>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P1));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P2_5>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P2_5));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.HEV_P4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.ParallelHybridVehicle,
					ArchitectureID.P4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E2>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E2));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E3>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E3));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E4>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E4));
			Bind<IVectoRunDataFactory>().To<DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC>().Named(
				_vehicleStringHelper.GetName(VehicleCategoryHelper.CompletedBus, VectoSimulationJobType.BatteryElectricVehicle,
					ArchitectureID.E_IEPC));

			#endregion

		}
		#endregion 



	}


	
}