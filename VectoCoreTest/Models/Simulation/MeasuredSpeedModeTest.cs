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
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Utils;
using System.Collections.Generic;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class MeasuredSpeedModeTest
	{
		/// <summary>
		/// Test if the cycle file can be read.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_ReadCycle_Gear()
		{
			// all data
			string inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<vair_res>,<vair_beta>,<Aux_Alt>
								 0  ,0  ,0     ,3.2018,595.75,0     ,0         ,0          ,0.504";
			TestCycleRead(inputData, CycleType.MeasuredSpeedGear);

			// vair only
			inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<vair_res>,<vair_beta>
						  0  ,0  ,0     ,3.2018,595.75,0     ,0         ,0          ";
			TestCycleRead(inputData, CycleType.MeasuredSpeedGear);

			// no aux, no vair
			inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>
						  0  ,0  ,0     ,3.2018,595.75,0     ";
			TestCycleRead(inputData, CycleType.MeasuredSpeedGear);

			// aux only
			inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<Aux_Alt>
						  0  ,0  ,0     ,3.2018,595.75,0     ,0.504";
			TestCycleRead(inputData, CycleType.MeasuredSpeedGear);

			// missing columns
			inputData = @"<t>,<grad>,<Padd>,<n>,<gear>
						  0  ,0     ,3.2018,595.75,0";
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead(inputData, CycleType.MeasuredSpeedGear, autoCycle: false),
				"ERROR while reading DrivingCycle Stream: Column(s) required: v");

			// auto find cycle type
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead(inputData, CycleType.MeasuredSpeedGear),
				"CycleFile format is unknown.");

			// not allowed columns
			inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<wrong>
						  0  ,0  ,0     ,3.2018,595.75,0     ,0.504";
			AssertHelper.Exception<VectoException>(() => TestCycleRead(inputData, CycleType.MeasuredSpeedGear, autoCycle: false),
				"ERROR while reading DrivingCycle Stream: Column(s) not allowed: wrong");

			// wrong data
			inputData = @"<t>,<grad>,<Padd>,<n>,<gear>
						  0  ,0";
			AssertHelper.Exception<VectoException>(() => TestCycleRead(inputData, CycleType.MeasuredSpeedGear),
				"Failed to read stream: Line 0: The number of values is not correct.");
		}

		/// <summary>
		/// Test if the cycle file can be read.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_ReadCycle()
		{
			// all data
			string inputData = @"<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>
								 0  ,0  ,0     ,3.2018,0         ,0          ,0.504";
			TestCycleRead(inputData, CycleType.MeasuredSpeed);

			// vair only
			inputData = @"<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>
						  0  ,0  ,0     ,3.2018,0         ,0          ";
			TestCycleRead(inputData, CycleType.MeasuredSpeed);

			// no aux, no vair
			inputData = @"<t>,<v>,<grad>,<Padd>
						  0  ,0  ,0     ,3.2018";
			TestCycleRead(inputData, CycleType.MeasuredSpeed);

			// aux only
			inputData = @"<t>,<v>,<grad>,<Padd>,<Aux_Alt>
						  0  ,0  ,0     ,3.2018,0.504";
			TestCycleRead(inputData, CycleType.MeasuredSpeed);

			// missing columns
			inputData = @"<t>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>
						  0  ,0     ,3.2018,0         ,0          ,0.504";
			AssertHelper.Exception<VectoException>(() => TestCycleRead(inputData, CycleType.MeasuredSpeed, autoCycle: false),
				"ERROR while reading DrivingCycle Stream: Column(s) required: v");

			// not allowed columns
			inputData = @"<t>,<v>,<wrong>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>
						  0  ,0  ,0     ,3.2018,0         ,0          ,0.504,0";
			AssertHelper.Exception<VectoException>(() => TestCycleRead(inputData, CycleType.MeasuredSpeed, autoCycle: false),
				"ERROR while reading DrivingCycle Stream: Column(s) not allowed: wrong");

			// auto find cycle
			AssertHelper.Exception<VectoException>(() => TestCycleRead(inputData, CycleType.MeasuredSpeed),
				"CycleFile format is unknown.");

			// wrong data
			inputData = @"<t>,<v>,<grad>,<Padd>,<vair_res>,<vair_beta>,<Aux_Alt>
						  0  ,0";
			AssertHelper.Exception<VectoException>(() => TestCycleRead(inputData, CycleType.MeasuredSpeed),
				"Failed to read stream: Line 0: The number of values is not correct.");
		}


		private static void TestCycleRead(string inputData, CycleType cycleType, bool autoCycle = true)
		{
			var container = new VehicleContainer();

			if (autoCycle) {
				var cycleTypeCalc = DrivingCycleDataReader.GetCycleType(VectoCSVFile.ReadStream(inputData.GetStream()));
				Assert.AreEqual(cycleType, cycleTypeCalc);
			}
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), cycleType);
			Assert.AreEqual(cycleType, drivingCycle.CycleType);

			var cycle = new MeasuredSpeedDrivingCycle(container, drivingCycle);
		}


		/// <summary>
		/// Tests if the powertrain can be created in MeasuredSpeed mode.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_CreatePowertrain_Gear()
		{
			// prepare input data
			var inputData = @"<t>,<v>    ,<grad>      ,<Padd>     ,<n>    ,<gear>
							  1  ,0      ,0           ,3.201815003,595.75 ,0
							  2  ,0.3112 ,0           ,4.532197507,983.75 ,1
							  3  ,5.2782 ,-0.041207832,2.453370264,723.75 ,1
							  4  ,10.5768,-0.049730127,3.520827362,1223.25,1";

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), CycleType.MeasuredSpeedGear);

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
				GearboxData = new GearboxData { Gears = new Dictionary<uint, GearData> { { 1, new GearData { Ratio = 6.2 } } } },
				Retarder = new RetarderData()
			};

			// call builder (actual test)
			var builder = new PowertrainBuilder(null);
			builder.Build(data);
		}


		/// <summary>
		/// Tests if the powertrain can be created in MeasuredSpeed mode.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_CreatePowertrain()
		{
			// prepare input data
			var inputData = @"<t>,<v>    ,<grad>      ,<Padd>     
							  1  ,0      ,0           ,3.201815003
							  2  ,0.3112 ,0           ,4.532197507
							  3  ,5.2782 ,-0.041207832,2.453370264
							  4  ,10.5768,-0.049730127,3.520827362";

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.GetStream(), CycleType.MeasuredSpeed);

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
		public void MeasuredSpeed_Run()
		{
			var jobFile = @"TestData\MeasuredSpeed\MeasuredSpeed.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeed_MeasuredSpeed.vmod"), "Mod file not found.");
			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeed.vsum"), "Sum file not found.");
		}


		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in MeasuredSpeed mode.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Run_Gear()
		{
			var jobFile = @"TestData\MeasuredSpeed\MeasuredSpeedGear.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeedGear.vsum"), "SUM file missing.");
			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeedGear_MeasuredSpeed_Gear_Rural.vmod"),
				"MOD File missing.");
		}

		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in MeasuredSpeed mode with Aux.
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Run_Gear_Aux()
		{
			var jobFile = @"TestData\MeasuredSpeed\MeasuredSpeedGearAux.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeedGearAux.vsum"), "SUM file missing.");
			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeedGearAux_MeasuredSpeed_Gear_Rural_Aux.vmod"),
				"MOD File missing.");
		}


		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in MeasuredSpeed mode with Vair & Beta
		/// </summary>
		/// <remarks>VECTO-181</remarks>
		[TestMethod]
		public void MeasuredSpeed_Run_Gear_Vair()
		{
			var jobFile = @"TestData\MeasuredSpeed\MeasuredSpeedGearVair.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeedGearVair.vsum"), "SUM file missing.");
			Assert.IsTrue(File.Exists(@"TestData\MeasuredSpeed\MeasuredSpeedGearVair_MeasuredSpeed_Gear_Rural_Vair.vmod"),
				"MOD File missing.");
		}

		[TestMethod]
		public void VcdbTest()
		{
			var tbl = VectoCSVFile.Read(@"TestData/MeasuredSpeed/VairBeta.vcdb");

			var dataBus = new MockVairVechicleContainer();
			var vairbeta = new VAirBetaCrosswindCorrection(5.SI<SquareMeter>(), tbl);
			vairbeta.SetDataBus(dataBus);

			var cycleEntry = new DrivingCycleData.DrivingCycleEntry() {
				AirSpeedRelativeToVehicle = 20.KMPHtoMeterPerSecond(),
				WindYawAngle = 0
			};
			dataBus.CycleData = new CycleData() { LeftSample = cycleEntry };

			var pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond(), 1.SI<Second>()).Value();
			Assert.AreEqual(509.259, pAvg, 1e-3);

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 21.KMPHtoMeterPerSecond(), 1.SI<Second>()).Value();
			Assert.AreEqual(521.990, pAvg, 1e-3);

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 30.KMPHtoMeterPerSecond(), 1.SI<Second>()).Value();
			Assert.AreEqual(636.574, pAvg, 1e-3);

			cycleEntry.WindYawAngle = 20;

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond(), 1.SI<Second>()).Value();
			Assert.AreEqual(638.611, pAvg, 1e-3);

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 30.KMPHtoMeterPerSecond(), 1.SI<Second>()).Value();
			Assert.AreEqual(798.263, pAvg, 1e-3);
		}
	}
}