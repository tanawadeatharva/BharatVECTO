using System.IO;
using System.Text;
using TUGraz.VectoCore.Utils;
using System.Collections.Generic;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class PwheelModeTests
	{
		/// <summary>
		/// Test if the cycle file can be read.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_ReadCycle_Test()
		{
			IVehicleContainer container = new VehicleContainer();
			var inputData = @"<t>,<Pwheel>,<Gear>,<n>,<Padd>
1,89,2,1748,1.300
2,120,2,1400,0.4";

			Stream cycleFile = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel);
			var cycle = new PWheelCycle(container, drivingCycle, 2.3, new Dictionary<uint, double> { { 2, 3.5 } });

			Assert.AreEqual(cycle.CycleData().LeftSample.Time, 1.SI<Second>());
			Assert.AreEqual(cycle.CycleData().RightSample.Time, 2.SI<Second>());

			Assert.AreEqual(1748.RPMtoRad() / (2.3 * 3.5), cycle.CycleData().LeftSample.AngularVelocity);
			Assert.AreEqual(1400.RPMtoRad() / (2.3 * 3.5), cycle.CycleData().RightSample.AngularVelocity);

			Assert.AreEqual(89.SI().Kilo.Watt, cycle.CycleData().LeftSample.PWheel);
			Assert.AreEqual(120.SI().Kilo.Watt, cycle.CycleData().RightSample.PWheel);

			Assert.AreEqual(2u, cycle.CycleData().LeftSample.Gear);
			Assert.AreEqual(2u, cycle.CycleData().RightSample.Gear);

			Assert.AreEqual(1300.SI<Watt>(), cycle.CycleData().LeftSample.AdditionalAuxPowerDemand);
			Assert.AreEqual(400.SI<Watt>(), cycle.CycleData().RightSample.AdditionalAuxPowerDemand);

			Assert.AreEqual(89.SI().Kilo.Watt / (1748.RPMtoRad() / (2.3 * 3.5)), cycle.CycleData().LeftSample.Torque);
			Assert.AreEqual(120.SI().Kilo.Watt / (1400.RPMtoRad() / (2.3 * 3.5)), cycle.CycleData().RightSample.Torque);
		}

		/// <summary>
		/// Tests if the powertrain can be created in Pwheel mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_CreatePowertrain_Test()
		{
			var jobFile = @"TestData\Jobs\Pwheel.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
		}

		/// <summary>
		/// Tests if the simulation runs a Pwheel mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_Simulate_Test()
		{
			Assert.Fail("Test not implemented");
		}

		/// <summary>
		/// Tests if the modfile is correct in Pwheel mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_Output_Modfile_Test()
		{
			var jobFile = @"TestData\Jobs\Pwheel.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			ResultFileHelper.TestSumFile(@"TestData\Results\Pwheel\Atego_ges.v2.vsum", @"TestData\Jobs\Pwheel.vsum");

			ResultFileHelper.TestModFile(@"TestData\Results\Pwheel\Atego_ges_Gear2_pt1_rep1_actual.vmod",
				@"TestData\Jobs\Pwheel_Gear2_pt1_rep1_actual.vmod");
		}
	}
}