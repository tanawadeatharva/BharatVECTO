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


			Bind<IInternalRunDataFactoryFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				_vehicleStringHelper.CreateName,
				6, 6,
				typeof(IInternalRunDataFactoryFactory).GetMethod(nameof(IInternalRunDataFactoryFactory
					.CreateDeclarationRunDataFactory)))).InSingletonScope();


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

			#endregion


		}
		#endregion 



	}


	
}