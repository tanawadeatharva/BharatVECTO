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

	}

	internal static class InputDataProviderMockExtension
	{
		internal static Mock<IDeclarationInputDataProvider> Conventional(this Mock<IDeclarationInputDataProvider> mock)
		{
			mock.Setup(provider => provider.JobInputData.JobType)
				.Returns(VectoSimulationJobType.ConventionalVehicle);
			mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).Returns(ArchitectureID.UNKNOWN);
			
			return mock;
		}

		internal static Mock<IDeclarationInputDataProvider> HEV(this Mock<IDeclarationInputDataProvider> mock, ArchitectureID arch)
		{


			mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).Returns(arch);
			mock.Setup(p => p.JobInputData.JobType).Returns(arch.ToString().StartsWith("P")
				? VectoSimulationJobType.ParallelHybridVehicle
				: VectoSimulationJobType.SerialHybridVehicle);
			return mock;
		}
		internal static Mock<IDeclarationInputDataProvider> Lorry(this Mock<IDeclarationInputDataProvider> mock)
		{
			mock.Setup(provider => provider.JobInputData.Vehicle.VehicleCategory)
				.Returns(VehicleCategory.RigidTruck);
			return mock;
		}

	}

}
