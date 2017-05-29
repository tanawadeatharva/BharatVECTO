using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class FuelTypesTest
	{
		[TestCase(FuelType.DieselCI,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0, 0.0006944, 9383.8223,
			TestName = "Diesel LH Low"),
		TestCase(FuelType.EthanolCI,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0, 0.0004197, 6417.04,
			TestName = "Ethanol LH Low"),
		TestCase(FuelType.DieselCI,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1, 0.0008025, 10843.6305,
			TestName = "Diesel LH Ref"),
		TestCase(FuelType.EthanolCI,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1, 0.0004850, 7415.3164,
			TestName = "Ethanol LH Ref"),]
		public void TestFuelTypesCO2(FuelType fuelType, string jobName, int runIdx, double expectedCo2, double expectedMJ)
		{
			// the same engine fc-map is used for different fuel types, thus the difference in expected results
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter) { WriteModalResults = true };

			jobContainer.AddRuns(runsFactory);

			// store a reference to the data table here because it is going to be deleted
			var run = jobContainer.Runs[runIdx];
			var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
			var modData = modContainer.Data;
			modContainer.FuelData = FuelData.Instance().Lookup(fuelType);

			run.Run.Run();

			// restore data table before assertions
			modContainer.Data = modData;

			Assert.AreEqual(expectedCo2, modContainer.CO2PerMeter().Value(), 1e-6);
			Assert.AreEqual(expectedMJ, modContainer.EnergyPerMeter().Value(), 1e-3);
		}
	}
}