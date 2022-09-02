using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.Impl;

namespace TUGraz.VectoCore.Tests.InputData.RunDataFactory
{
    [TestFixture]
    internal class RunDataFactoryFactoryTest
	{
		private IVectoRunDataFactoryFactory _runDataFactoryFactory;
		


		[OneTimeSetUp]
		public void OneTimeSetup()
		{
#if (MOCKUP)
			Assert.Ignore("Tests not meaningful in mockup mode");
#endif
			var kernel = new StandardKernel(new VectoNinjectModule()) {

			};
			_runDataFactoryFactory = kernel.Get<IVectoRunDataFactoryFactory>();
			Assert.IsTrue(_runDataFactoryFactory.GetType() == typeof(VectoRunDataFactoryFactory));

		}

		private void CreateRunDataFactory(Mock<IDeclarationInputDataProvider> inputMock, Type expectedType)
		{
			var result = _runDataFactoryFactory.CreateDeclarationRunDataFactory(inputMock.Object, null,
				null);
			Assert.IsTrue(result.GetType() == expectedType);
		}

#region HeavyLorry
		[TestCase()]
		public void ConventionalHeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Conventional()
				.Lorry();

			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.Conventional));
			
		}
		[TestCase()]
		public void HEV_S2_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S2));
		}

		[TestCase()]
		public void HEV_S3_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S3)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S3));
		}

		[TestCase()]
		public void HEV_S4_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S4)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S4));
		}

		[TestCase()]
		public void HEV_S_IEPC_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S_IEPC)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC));
		}

		[TestCase()]
		public void HEV_P1_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P1)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P1));
		}
		[TestCase()]
		public void HEV_P2_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P2));
		}

		[TestCase()]
		public void HEV_P2_5_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2_5)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5));
		}

		[TestCase()]
		public void HEV_P3_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P3)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P3));
		}
		[TestCase()]
		public void HEV_P4_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P4)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P4));
		}

		[TestCase()]
		public void PEV_E2_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E2)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E2));
		}
		[TestCase()]
		public void PEV_E3_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E3)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E3));
		}
		[TestCase()]
		public void PEV_E4_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E4)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E4));
		}
		[TestCase()]
		public void PEV_E_IEPC_HeavyLorryTest()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E_IEPC)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC));
		}


#endregion HeavyLorry
#region PrimaryBus
		[TestCase()]
		public void ConventionalPrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Conventional()
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.Conventional));
		}

		[TestCase()]
		public void HEV_S2_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S2));
		}

		[TestCase()]
		public void HEV_S3_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S3)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S3));
		}

		[TestCase()]
		public void HEV_S4_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S4)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S4));
		}

		[TestCase()]
		public void HEV_S_IEPC_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S_IEPC)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC));
		}

		[TestCase()]
		public void HEV_P1_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P1)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P1));
		}


		[TestCase()]
		public void HEV_P2_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S2));
		}


		[TestCase()]
		public void HEV_P3_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P3)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P3));
		}


		[TestCase()]
		public void HEV_P2_5_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2_5)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P2_5));
		}


		[TestCase()]
		public void HEV_P4_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P4)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P4));
		}


		[TestCase()]
		public void PEV_E2_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E2)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E2));
		}


		[TestCase()]
		public void PEV_E3_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E3)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E3));
		}
		[TestCase()]
		public void PEV_E4_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E4)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E4));
		}

		[TestCase()]
		public void PEV_E_IEPC_PrimaryBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E_IEPC)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC));
		}

#endregion PrimaryBus

		[TestCase()]
		public void ConventionalCompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Conventional()
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.Conventional));
		}

		[TestCase()]
		public void HEV_S2_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S2));
		}

		[TestCase()]
		public void HEV_S3_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S3)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S3));
		}

		[TestCase()]
		public void HEV_S4_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S4)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S4));
		}

		[TestCase()]
		public void HEV_S_IEPC_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S_IEPC)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC));
		}

		[TestCase()]
		public void HEV_P1_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P1)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P1));
		}


		[TestCase()]
		public void HEV_P2_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S2));
		}


		[TestCase()]
		public void HEV_P3_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P3)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P3));
		}


		[TestCase()]
		public void HEV_P2_5_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2_5)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P2_5));
		}


		[TestCase()]
		public void HEV_P4_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P4)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P4));
		}


		[TestCase()]
		public void PEV_E2_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E2)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E2));
		}


		[TestCase()]
		public void PEV_E3_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E3)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E3));
		}
		[TestCase()]
		public void PEV_E4_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E4)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E4));
		}

		[TestCase()]
		public void PEV_E_IEPC_CompletedBus()
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E_IEPC)
				.CompletedBus();
			CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC));
		}
	}

	internal static class InputDataProviderMockExtension
	{
		internal static Mock<IDeclarationInputDataProvider> Conventional(this Mock<IDeclarationInputDataProvider> mock)
		{
			mock.Setup(provider => provider.JobInputData.JobType)
				.Returns(VectoSimulationJobType.ConventionalVehicle);

			mock.Setup(provider => provider.JobInputData.Vehicle.VehicleType).
				Returns(VectoSimulationJobType.ConventionalVehicle);

			mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).
				Returns(ArchitectureID.UNKNOWN);

			return mock;
		}

		internal static Mock<IDeclarationInputDataProvider> HEV(this Mock<IDeclarationInputDataProvider> mock, ArchitectureID arch)
		{
			var type = arch.ToString().StartsWith("P")
				? VectoSimulationJobType.ParallelHybridVehicle
				: VectoSimulationJobType.SerialHybridVehicle;
			mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).
				Returns(arch);
			mock.Setup(p => p.JobInputData.JobType).
				Returns(type);
			mock.Setup(p => p.JobInputData.Vehicle.VehicleType).
				Returns(type);
			return mock;
		}

		internal static Mock<IDeclarationInputDataProvider> PEV(this Mock<IDeclarationInputDataProvider> mock, ArchitectureID arch)
		{
			mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).
				Returns(arch);
			mock.Setup(p => p.JobInputData.JobType).
				Returns(
				VectoSimulationJobType.BatteryElectricVehicle);
			mock.Setup(p => p.JobInputData.Vehicle.VehicleType).
				Returns(
				VectoSimulationJobType.BatteryElectricVehicle);
			return mock;
		}
		internal static Mock<IDeclarationInputDataProvider> Lorry(this Mock<IDeclarationInputDataProvider> mock)
		{
			mock.Setup(provider => provider.JobInputData.Vehicle.VehicleCategory)
				.Returns(VehicleCategory.RigidTruck);
			return mock;
		}

		internal static Mock<IDeclarationInputDataProvider> PrimaryBus(this Mock<IDeclarationInputDataProvider> mock)
		{
			mock.Setup(p => p.JobInputData.Vehicle.VehicleCategory).
				Returns(VehicleCategory.HeavyBusPrimaryVehicle);
			return mock;
		}

		internal static Mock<IDeclarationInputDataProvider> CompletedBus(this Mock<IDeclarationInputDataProvider> mock)
		{
			mock.Setup(p => p.JobInputData.Vehicle.VehicleCategory).
				Returns(VehicleCategory.HeavyBusCompletedVehicle);
			return mock;
		}

	}

}
