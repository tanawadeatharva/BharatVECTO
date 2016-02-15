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

using System;
using TUGraz.VectoCore.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class MeasuredSpeedModeTest
	{
		string inputData = @"  <t>         ,<v>     ,<grad>      ,<Padd>     ,<n>    ,<gear>,<vair_res>,<vair_beta>,<Aux_Alt>
								0           ,0       ,0           ,3.2018     ,595.75 ,0     ,0         ,0          ,0.504    
								1           ,0.3112  ,0           ,4.5321     ,983.75 ,1     ,0         ,0          ,0.476    
								2           ,5.2782  ,-0.0412     ,2.4533     ,723.75 ,1     ,8.532     ,0          ,0.42     
								3           ,10.576  ,-0.0497     ,3.5208     ,1223.25,1     ,12.024    ,34         ,0.476     ";


		/// <summary>
		/// Test if the cycle file can be read.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Shift_ReadCycle_Test()
		{
			var container = new VehicleContainer();
			var gearbox = new Gearbox(container,
				new GearboxData {
					Gears = new Dictionary<uint, GearData> {
						{ 1, new GearData { Ratio = 6.696 } },
						{ 2, new GearData { Ratio = 3.806 } },
						{ 3, new GearData { Ratio = 2.289 } }
					}
				}, new PWheelShiftStrategy(null, container));

			string inputData = @"<t>         ,<v>     ,<grad>      ,<Padd>     ,<n>    ,<gear>,<vair_res>,<vair_beta>,<Aux_Alt>
								 0           ,0       ,0           ,3.2018     ,595.75 ,0     ,0         ,0          ,0.504";
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), CycleType.MeasuredSpeedTrack);
			var cycle = new MeasuredSpeedCycle(container, drivingCycle, gearbox, 4.3, 0.848974.SI<Meter>(), 600.RPMtoRad(),
				1736.RPMtoRad());
		}

		#region rewrite!

		//todo mk-2016-02-15: rewrite!! test are old and use old cycle format

		/// <summary>
		/// Test if the cycle file can be read.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Dyno_ReadCycle_Test()
		{
			var container = new VehicleContainer();
			var inputData = @" < t >, <
			v >,   <
			grad >,<
			gear >,<
			Aux_Alt >, <
			Padd >
			1,
			1.383,
			0,
			1,
			0.5767916,
			1.969367304
			2,
			3.515,
			0,
			1,
			0.5426120,
			2.042128260
			";

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), CycleType.MeasuredSpeedDyno);

			var gearbox = new Gearbox(container,
				new GearboxData {
					Gears = new Dictionary<uint, GearData> { { 1, new GearData { Ratio = 2.0 } }, { 2, new GearData { Ratio = 3.5 } } }
				}, new PWheelShiftStrategy(null, container));

			var cycle = new MeasuredSpeedDynoCycle(container, drivingCycle, gearbox);

			Assert.AreEqual(container.CycleData.LeftSample.Time, 1.SI<Second>());
			Assert.AreEqual(container.CycleData.RightSample.Time, 2.SI<Second>());

			Assert.AreEqual(1.383.KMPHtoMeterPerSecond(), container.CycleData.LeftSample.VehicleTargetSpeed);
			Assert.AreEqual(3.515.KMPHtoMeterPerSecond(), container.CycleData.RightSample.VehicleTargetSpeed);

			Assert.AreEqual(0.5767916.SI().Kilo.Watt, container.CycleData.LeftSample.AuxiliarySupplyPower["Aux_Alt"]);
			Assert.AreEqual(0.5426120.SI().Kilo.Watt, container.CycleData.RightSample.AuxiliarySupplyPower["Aux_Alt"]);

			Assert.AreEqual(1u, container.CycleData.LeftSample.Gear);
			Assert.AreEqual(1u, container.CycleData.RightSample.Gear);

			Assert.AreEqual(1.969367304.SI().Kilo.Watt, container.CycleData.LeftSample.AdditionalAuxPowerDemand);
			Assert.AreEqual(2.042128260.SI().Kilo.Watt, container.CycleData.RightSample.AdditionalAuxPowerDemand);

			Assert.AreEqual(0.SI<Radian>(), container.CycleData.LeftSample.RoadGradient);
			Assert.AreEqual(0.SI<Radian>(), container.CycleData.RightSample.RoadGradient);
		}


		/// <summary>
		/// asserts that two cycle entries are the same.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		private void AssertCycleEntry(DrivingCycleData.DrivingCycleEntry expected, DrivingCycleData.DrivingCycleEntry actual)
		{
			Assert.AreEqual(expected.Time, actual.Time);
			Assert.AreEqual(expected.VehicleTargetSpeed, actual.VehicleTargetSpeed);
			Assert.AreEqual(expected.AngularVelocity, actual.AngularVelocity);
			Assert.AreEqual(expected.Gear, actual.Gear);
		}

		/// <summary>
		/// Tests if the powertrain can be created in MeasuredSpeed mode.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Dyno_CreatePowertrain_Test()
		{
			// prepare input data
			var inputData = @"<t>,<v>,   <grad>,<gear>,<Aux_Alt>, <Padd>
							   1,  1.383, 0,     1,     0.5767916, 1.969367304
							   2,  3.515, 0,     1,     0.5426120, 2.042128260";

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), CycleType.MeasuredSpeedDyno);

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
				VehicleData = new VehicleData { VehicleCategory = VehicleCategory.RigidTruck },
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
		/// Tests if the powertrain can be created in MeasuredSpeed mode.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Track_CreatePowertrain_Test()
		{
			// prepare input data
			var inputData = @"  <t> ,<v>     ,<grad>      ,<n>    ,<vair_res>,<vair_beta>,<Aux_Alt>,<Padd>     
								1   ,0       ,0           ,595.75 ,0         ,0          ,0.504    ,3.201815003
								2   ,0.3112  ,0           ,983.75 ,0         ,0          ,0.476    ,4.532197507
								3   ,5.2782  ,-0.041207832,723.75 ,8.532     ,0          ,0.42     ,2.453370264
								4   ,10.5768 ,-0.049730127,1223.25,12.024    ,34         ,0.476    ,3.520827362";

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), CycleType.MeasuredSpeedTrack);

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
				VehicleData =
					new VehicleData {
						VehicleCategory = VehicleCategory.RigidTruck,
						WheelsInertia = 2.SI<KilogramSquareMeter>(),
						DynamicTyreRadius = 0.85.SI<Meter>()
					},
				AxleGearData = new AxleGearData { AxleGear = new GearData { Ratio = 2.3 } },
				EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad(), FullLoadCurve = fullLoadCurve },
				GearboxData = new GearboxData {
					Gears = new Dictionary<uint, GearData> {
						{ 1, new GearData { Ratio = 6.696 } },
						{ 2, new GearData { Ratio = 3.806 } },
						{ 3, new GearData { Ratio = 2.289 } }
					}
				},
				Retarder = new RetarderData()
			};

			// call builder (actual test)
			var builder = new PowertrainBuilder(null);
			var jobContainer = builder.Build(data);
		}


		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in MeasuredSpeed mode.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Dyno_Run_Test()
		{
			var jobFile = @"TestData\MeasuredSpeed\Demo_ChassisDyno.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			Assert.IsTrue(File.Exists(@"TestData\Jobs\Pwheel.vsum"));
			Assert.IsTrue(File.Exists(@"TestData\Jobs\Pwheel_Gear2_pt1_rep1_actual.vmod"));
		}


		///// <summary>
		///// Tests if the simulation works and the modfile and sumfile are correct in MeasuredSpeed mode.
		///// </summary>
		///// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Track_Run_Test()
		{
			var jobFile = @"TestData\MeasuredSpeed\Demo_TestTrackCycle.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			//Assert.IsTrue(File.Exists(@"TestData\Jobs\Pwheel.vsum"));
			//Assert.IsTrue(File.Exists(@"TestData\Jobs\Pwheel_Gear2_pt1_rep1_actual.vmod"));

			Assert.Fail("Implement this test!");
		}

		#endregion
	}
}