using System;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Tests.InputData.DeclarationDataAdapterTest
{
	[TestFixture]
	public class DeclarationDataAdapterTest
	{
		private IDeclarationDataAdapterFactory _declarationDataAdapterFactory;

		public void OneTimeSetup()
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			_declarationDataAdapterFactory = kernel.Get<IDeclarationDataAdapterFactory>();
		}
		#region LorriesSerialHybridVehicle

		[TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.Conventional))]
		[TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_S2))]
		[TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S3, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_S3))]
		[TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S4, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_S4))]
		[TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S_IEPC, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_S_IEPC))]
		[TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P1, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_P1))]
		[TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_P2))]
		[TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2_5, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_P2_5))]
		[TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P3, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_P3))]
		[TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P4, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.HEV_P4))]
		[TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.PEV_E2))]
		[TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E3, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.PEV_E3))]
		[TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E4, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.PEV_E4))]
		[TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E_IEPC, VehicleCategoryHelper.Lorry, false, typeof(DeclarationDataAdapterHeavyLorry.PEV_E_IEPC))]

		public void DeclarationDataAdapterFactoryTest(VectoSimulationJobType jobType, ArchitectureID archId,
			string vehicleType, bool exempted, Type expectedType)
		{
			var iepc = false;
			var ihpc = false;
			var vehicleClassification =
				new VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification(jobType, archId, vehicleType,
					exempted, iepc, ihpc);
			var result =_declarationDataAdapterFactory.CreateDataAdapter(vehicleClassification);
			Assert.AreEqual(expectedType, result.GetType());
		}



		#endregion




	}
}