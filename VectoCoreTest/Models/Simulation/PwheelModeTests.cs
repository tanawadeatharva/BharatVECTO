/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.IO;
using System.Text;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Utils;
using System.Collections.Generic;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.InputData.Reader;
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
			var container = new VehicleContainer();
			var inputData = @"<t>,<Pwheel>,<gear>,<n>,<Padd>
                               1,89,2,1748,1.300
                               2,120,2,1400,0.4";

			var cycleFile = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel);

			var gearbox = new CycleGearbox(container,
				new GearboxData {
					Gears = new Dictionary<uint, GearData> { { 1, new GearData { Ratio = 2.0 } }, { 2, new GearData { Ratio = 3.5 } } }
				});

			var cycle = new PWheelCycle(container, drivingCycle, 2.3,
				gearbox.ModelData.Gears.ToDictionary(g => g.Key, g => g.Value.Ratio));

			Assert.AreEqual(container.CycleData.LeftSample.Time, 1.SI<Second>());
			Assert.AreEqual(container.CycleData.RightSample.Time, 2.SI<Second>());

			Assert.AreEqual(1748.RPMtoRad() / (2.3 * 3.5), container.CycleData.LeftSample.AngularVelocity);
			Assert.AreEqual(1400.RPMtoRad() / (2.3 * 3.5), container.CycleData.RightSample.AngularVelocity);

			Assert.AreEqual(89.SI().Kilo.Watt, container.CycleData.LeftSample.PWheel);
			Assert.AreEqual(120.SI().Kilo.Watt, container.CycleData.RightSample.PWheel);

			Assert.AreEqual(2u, container.CycleData.LeftSample.Gear);
			Assert.AreEqual(2u, container.CycleData.RightSample.Gear);

			Assert.AreEqual(1300.SI<Watt>(), container.CycleData.LeftSample.AdditionalAuxPowerDemand);
			Assert.AreEqual(400.SI<Watt>(), container.CycleData.RightSample.AdditionalAuxPowerDemand);

			Assert.AreEqual(89.SI().Kilo.Watt / (1748.RPMtoRad() / (2.3 * 3.5)), container.CycleData.LeftSample.Torque);
			Assert.AreEqual(120.SI().Kilo.Watt / (1400.RPMtoRad() / (2.3 * 3.5)), container.CycleData.RightSample.Torque);
		}

		/// <summary>
		/// Tests if the powertrain can be created in P_wheel_in mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_CreatePowertrain_Test()
		{
			// prepare input data
			var inputData = @"<t>,<Pwheel>,<gear>,<n>,  <Padd>
                               1,  89,      2,     1748, 1.3
                               2,  120,     2,     1400, 0.4";

			var cycleFile = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
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
				AxleGearData = new AxleGearData { AxleGear = new GearData { Ratio = 2.3 } },
				EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad(), FullLoadCurve = fullLoadCurve },
				GearboxData = new GearboxData { Gears = new Dictionary<uint, GearData> { { 2, new GearData { Ratio = 3.5 } } } },
				Retarder = new RetarderData()
			};

			// call builder (actual test)
			var builder = new PowertrainBuilder(null);
			var jobContainer = builder.Build(data);
		}

		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in P_wheel_in mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_Run_Test()
		{
			var jobFile = @"TestData\Pwheel\Pwheel.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			ResultFileHelper.TestSumFile(@"TestData\Pwheel\Results\Atego_ges.v2.vsum", @"TestData\Jobs\Pwheel.vsum");

			ResultFileHelper.TestModFile(@"TestData\Pwheel\Results\Atego_ges_Gear2_pt1_rep1_actual.vmod",
				@"TestData\Pwheel\Pwheel_Gear2_pt1_rep1_actual.vmod");
		}

		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in P_wheel_in mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestMethod]
		public void Pwheel_ultimate_Run_Test()
		{
			var jobFile = @"TestData\Pwheel\Pwheel_ultimate.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			ResultFileHelper.TestModFile(@"TestData\Pwheel\Results\P_wheel_in\Atego_HDVCO2_RD_#1_AuxStd.vmod",
				@"TestData\Pwheel\Pwheel_ultimate_RD_#1_Pwheel_AuxStd.vmod", testRowCount: false);
		}
	}
}