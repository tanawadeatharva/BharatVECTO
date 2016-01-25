using System.IO;
using System.Text;
using TUGraz.VectoCore.Utils;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
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
			var inputData = "<t>,<Pwheel>,<Gear>,<n>,<Padd>\n1,89,2,1748,1.300\n2,120,2,1400,0.4";

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
			// prepare input data
			var inputData = "<t>,<Pwheel>,<Gear>,<n>,<Padd>\n1,89,2,1748,1.300\n2,120,2,1400,0.4";

			Stream cycleFile = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel);

			var fuelConsumption = new DataTable();
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Rows.Add("1", "1", "1");
			fuelConsumption.Rows.Add("2", "2", "2");
			fuelConsumption.Rows.Add("3", "3", "3");

			var fullLoad = new DataTable();
			fullLoad.Columns.Add("Engine speed");
			fullLoad.Columns.Add("max torque");
			fullLoad.Columns.Add("drag torque");
			fullLoad.Columns.Add("PT1");
			fullLoad.Rows.Add("0", "5000", "-5000", "0");
			fullLoad.Rows.Add("3000", "5000", "-5000", "0");

			var fullLoadCurve = EngineFullLoadCurve.Create(fullLoad);
			var data = new VectoRunData {
				Cycle = drivingCycle,
				AxleGearData = new AxleGearData { Ratio = 2.3 },
				EngineData = new CombustionEngineData() { IdleSpeed = 560.RPMtoRad(), FullLoadCurve = fullLoadCurve },
				GearboxData = new GearboxData { Gears = new Dictionary<uint, GearData> { { 2, new GearData { Ratio = 3.5 } } } },
				Retarder = new RetarderData(),
			};

			// call builder (actual test)
			var builder = new PowertrainBuilder(null);
			var jobContainer = builder.Build(data);
		}

		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in Pwheel mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_Run_Test()
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