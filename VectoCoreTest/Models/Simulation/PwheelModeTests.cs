using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

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
			Assert.Fail("Test not implemented");
		}

		/// <summary>
		/// Tests if the powertrain can be created in Pwheel mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_CreatePowertrain_Test()
		{
			Assert.Fail("Test not implemented");
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

			//todo MK-2016-01-20: add sumdata file for pwheel mode tests.
			ResultFileHelper.TestSumFile(@"TestData\Results\Pwheel\Atego_ges.v2.vsum", @"Pwheel.vsum");

			//todo MK-2016-01-20: add moddata file for pwheel mode tests.
			ResultFileHelper.TestModFile(@"TestData\Results\Pwheel\Atego_ges_Gear2_pt1_rep1_actual.vmod",
				@"TestData\Jobs\Pwheel.vmod");

			Assert.Fail("Test not implemented");
		}
	}
}