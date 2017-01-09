/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

// ReSharper disable UnusedVariable

// ReSharper disable AccessToModifiedClosure

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
			var inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<vair_res>,<vair_beta>,<Aux_Alt>
				  			  0  ,0  ,0     ,3.2018,595.75,0     ,0         ,0          ,0.504";
			TestCycleRead(inputData, CycleType.MeasuredSpeedGear);

			// vair only
			inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>,<vair_res>,<vair_beta>
						  0  ,0  ,0     ,3.2018,595.75,0     ,0         ,0          ";
			TestCycleRead(inputData, CycleType.MeasuredSpeedGear, crossWindRequired: true);

			// vair required, but not there: error
			inputData = @"<t>,<v>,<grad>,<Padd>,<n>   ,<gear>
						  0  ,0  ,0     ,3.2018,595.75,0";
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead(inputData, CycleType.MeasuredSpeedGear, crossWindRequired: true),
				"ERROR while reading DrivingCycle Stream: Column vair_res was not found in DataRow.");

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
				"Line 1: The number of values is not correct. Expected 5 Columns, Got 2 Columns");
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
			TestCycleRead(inputData, CycleType.MeasuredSpeed, crossWindRequired: true);

			// vair required, but not there: error
			inputData = @"<t>,<v>,<grad>,<Padd>
						  0  ,0  ,0     ,3.2018";
			AssertHelper.Exception<VectoException>(
				() => TestCycleRead(inputData, CycleType.MeasuredSpeed, crossWindRequired: true),
				"ERROR while reading DrivingCycle Stream: Column vair_res was not found in DataRow.");

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
				"Line 1: The number of values is not correct. Expected 7 Columns, Got 2 Columns");
		}

		private static void TestCycleRead(string inputData, CycleType cycleType, bool autoCycle = true,
			bool crossWindRequired = false)
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);

			if (autoCycle) {
				var cycleTypeCalc = DrivingCycleDataReader.DetectCycleType(VectoCSVFile.ReadStream(inputData.ToStream()));
				Assert.AreEqual(cycleType, cycleTypeCalc);
			}
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.ToStream(), cycleType, "", crossWindRequired);
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

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.ToStream(), CycleType.MeasuredSpeedGear, "",
				false);

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
						CrossWindCorrectionCurve =
							new CrosswindCorrectionCdxALookup(
								CrossWindCorrectionCurveReader.GetNoCorrectionCurve(6.16498344.SI<SquareMeter>()),
								CrossWindCorrectionMode.NoCorrection),
						GrossVehicleWeight = 12000.SI<Kilogram>(),
						CurbWeight = 3400.SI<Kilogram>(),
						DynamicTyreRadius = 0.5.SI<Meter>(),
						AxleData =
							new List<Axle> {
								new Axle { AxleWeightShare = 1.0, TyreTestLoad = 52532.SI<Newton>(), Inertia = 10.SI<KilogramSquareMeter>() }
							}
					},
				AxleGearData = new AxleGearData { AxleGear = new GearData { Ratio = 2.3 } },
				EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad(), FullLoadCurve = fullLoadCurve },
				GearboxData = new GearboxData { Gears = new Dictionary<uint, GearData> { { 1, new GearData { Ratio = 6.2 } } } },
				Retarder = new RetarderData()
			};

			// call builder (actual test)
			var builder = new PowertrainBuilder(new MockModalDataContainer());
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

			var drivingCycle = DrivingCycleDataReader.ReadFromStream(inputData.ToStream(), CycleType.MeasuredSpeed, "", false);

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
						DynamicTyreRadius = 0.85.SI<Meter>(),
						CrossWindCorrectionCurve =
							new CrosswindCorrectionCdxALookup(
								CrossWindCorrectionCurveReader.GetNoCorrectionCurve(6.16498344.SI<SquareMeter>()),
								CrossWindCorrectionMode.NoCorrection)
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
			var builder = new PowertrainBuilder(new MockModalDataContainer());
			var jobContainer = builder.Build(data);
		}

		private static void RunJob(string jobFile, string expectedModFile, string actualModFile, string expectedSumFile,
			string actualSumFile, bool actualModData = false)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);
			runsFactory.ActualModalData = actualModData;

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			ResultFileHelper.TestModFile(expectedModFile, actualModFile);
			ResultFileHelper.TestSumFile(expectedSumFile, actualSumFile);
		}

		[TestMethod]
		public void MeasuredSpeed_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeed.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeed_MeasuredSpeed.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeed_MeasuredSpeed.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeed.vsum", @"TestData\MeasuredSpeed\MeasuredSpeed.vsum");
		}

		[TestMethod]
		public void MeasuredSpeedAux_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedAux.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedAux_MeasuredSpeedAux.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedAux_MeasuredSpeedAux.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedAux.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedAux.vsum");
		}

		[TestMethod]
		public void MeasuredSpeedVair_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedVair.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVair_MeasuredSpeedVair.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedVair_MeasuredSpeedVair.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVair.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedVair.vsum");
		}

		[TestMethod]
		public void MeasuredSpeedVair_WindFromFront_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedVairFront.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairFront_MeasuredSpeedVairFront.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedVairFront_MeasuredSpeedVairFront.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairFront.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedVairFront.vsum");
		}

		[TestMethod]
		public void MeasuredSpeedVair_WindFromBack_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedVairBack.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairBack_MeasuredSpeedVairBack.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedVairBack_MeasuredSpeedVairBack.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairBack.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedVairBack.vsum");
		}

		[TestMethod]
		public void MeasuredSpeedVair_NoWind_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedVairNoWind.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairNoWind_MeasuredSpeedVairNoWind.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedVairNoWind_MeasuredSpeedVairNoWind.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairNoWind.vsum",
				@"TestData\MeasuredSpeed\MeasuredSpeedVairNoWind.vsum");
		}

		[TestMethod]
		public void MeasuredSpeedVairAux_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedVairAux.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairAux_MeasuredSpeedVairAux.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedVairAux_MeasuredSpeedVairAux.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedVairAux.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedVairAux.vsum");
		}

		[TestMethod]
		public void MeasuredSpeed_Gear_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedGear.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGear_MeasuredSpeed_Gear_Rural.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedGear_MeasuredSpeed_Gear_Rural.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGear.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedGear.vsum", true);
		}

		[TestMethod]
		public void MeasuredSpeed_Gear_Aux_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedGearAux.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearAux_MeasuredSpeed_Gear_Rural_Aux.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearAux_MeasuredSpeed_Gear_Rural_Aux.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearAux.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedGearAux.vsum");
		}

		[TestMethod]
		public void MeasuredSpeed_Gear_Vair_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedGearVair.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearVair_MeasuredSpeed_Gear_Rural_Vair.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearVair_MeasuredSpeed_Gear_Rural_Vair.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearVair.vsum", @"TestData\MeasuredSpeed\MeasuredSpeedGearVair.vsum");
		}

		[TestMethod]
		public void MeasuredSpeed_Gear_VairAux_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedGearVairAux.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearVairAux_MeasuredSpeed_Gear_Rural_VairAux.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearVairAux_MeasuredSpeed_Gear_Rural_VairAux.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearVairAux.vsum",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearVairAux.vsum");
		}

		[TestMethod]
		public void MeasuredSpeed_Gear_AT_PS_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedGearAT-PS.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearAT-PS_MeasuredSpeedGear_AT-PS.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearAT-PS_MeasuredSpeedGear_AT-PS.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearAT-PS.vsum",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearAT-PS.vsum");
		}

		[TestMethod]
		public void MeasuredSpeed_Gear_AT_Ser_Run()
		{
			RunJob(@"TestData\MeasuredSpeed\MeasuredSpeedGearAT-Ser.vecto",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearAT-Ser_MeasuredSpeedGear_AT-Ser.vmod",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearAT-Ser_MeasuredSpeedGear_AT-Ser.vmod",
				@"TestData\MeasuredSpeed\Results\MeasuredSpeedGearAT-Ser.vsum",
				@"TestData\MeasuredSpeed\MeasuredSpeedGearAT-Ser.vsum");
		}

		[TestMethod]
		public void VcdbTest()
		{
			var tbl = VectoCSVFile.Read(@"TestData/MeasuredSpeed/VairBetaFull.vcdb");

			var dataBus = new MockVehicleContainer();

			var vairbeta = new CrosswindCorrectionVAirBeta(5.SI<SquareMeter>(),
				CrossWindCorrectionCurveReader.ReadCdxABetaTable(tbl));
			vairbeta.SetDataBus(dataBus);

			var cycleEntry = new DrivingCycleData.DrivingCycleEntry() {
				AirSpeedRelativeToVehicle = 20.KMPHtoMeterPerSecond(),
				WindYawAngle = 0
			};
			dataBus.CycleData = new CycleData() { LeftSample = cycleEntry };

			var pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond()).Value();
			Assert.AreEqual(509.259, pAvg, 1e-3);

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 21.KMPHtoMeterPerSecond()).Value();
			Assert.AreEqual(521.990, pAvg, 1e-3);

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 30.KMPHtoMeterPerSecond()).Value();
			Assert.AreEqual(636.574, pAvg, 1e-3);

			cycleEntry.WindYawAngle = 20;

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond()).Value();
			Assert.AreEqual(829.074, pAvg, 1e-3);

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 30.KMPHtoMeterPerSecond()).Value();
			Assert.AreEqual(1036.343, pAvg, 1e-3);

			cycleEntry.WindYawAngle = -120;

			pAvg =
				vairbeta.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(), 20.KMPHtoMeterPerSecond()).Value();
			Assert.AreEqual(-1019.5370, pAvg, 1e-3);
		}
	}
}