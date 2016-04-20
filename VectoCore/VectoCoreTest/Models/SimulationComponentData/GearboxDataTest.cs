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

using System;
using System.Data;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class GearboxDataTest
	{
		public TestContext TestContext { get; set; }

		protected const string GearboxFile = @"Testdata\Components\24t Coach.vgbx";

		protected const string EngineFile = @"TestData\Components\24t Coach.veng";

		[TestMethod]
		public void TestGearboxDataReadTest()
		{
			var gbxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxFile, EngineFile, false);

			Assert.AreEqual(GearboxType.AMT, gbxData.Type);
			Assert.AreEqual(1.0, gbxData.TractionInterruption.Value(), 0.0001);
			Assert.AreEqual(8, gbxData.Gears.Count);

			// Todo: Assert.AreEqual(3.240355, gbxData.AxleGearData.Ratio, 0.0001);
			Assert.AreEqual(1.0, gbxData.Gears[7].Ratio, 0.0001);

			Assert.AreEqual(-400, gbxData.Gears[1].ShiftPolygon.Downshift[0].Torque.Value(), 0.0001);
			Assert.AreEqual(560.RPMtoRad().Value(), gbxData.Gears[1].ShiftPolygon.Downshift[0].AngularSpeed.Value(), 0.0001);
			Assert.AreEqual(1289.RPMtoRad().Value(), gbxData.Gears[1].ShiftPolygon.Upshift[0].AngularSpeed.Value(), 0.0001);

			Assert.AreEqual(200.RPMtoRad().Value(), gbxData.Gears[1].LossMap[15].InputSpeed.Value(), 0.0001);
			Assert.AreEqual(-350, gbxData.Gears[1].LossMap[15].InputTorque.Value(), 0.0001);
			Assert.AreEqual(13.072, gbxData.Gears[1].LossMap[15].TorqueLoss.Value(), 0.0001);
		}

		[TestMethod]
		[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
			"|DataDirectory|\\TestData\\AxleGearLossInterpolation.csv",
			"AxleGearLossInterpolation#csv", DataAccessMethod.Sequential)]
		public void TestInterpolation()
		{
			var rdyn = double.Parse(TestContext.DataRow["rDyn"].ToString(), CultureInfo.InvariantCulture);
			var speed = double.Parse(TestContext.DataRow["v"].ToString(), CultureInfo.InvariantCulture);

			//var gbxData = MockSimulationDataFactory.CreateGearboxDataFromFile(TestContext.DataRow["GearboxDataFile"].ToString(),
			//	EngineFile);
			var axleData = MockSimulationDataFactory.CreateAxleGearDataFromFile(TestContext.DataRow["GearboxDataFile"].ToString());

			var PvD = double.Parse(TestContext.DataRow["PowerGbxOut"].ToString(), CultureInfo.InvariantCulture).SI<Watt>();

			var torqueToWheels = Formulas.PowerToTorque(PvD, SpeedToAngularSpeed(speed, rdyn));
			var torqueFromEngine = 0.SI<NewtonMeter>();

			var angSpeed = SpeedToAngularSpeed(speed, rdyn);
			if (TestContext.DataRow["Gear"].ToString() == "A") {
				torqueFromEngine = torqueToWheels / axleData.AxleGear.Ratio;
				torqueFromEngine += axleData.AxleGear.LossMap.GetTorqueLoss(angSpeed, torqueToWheels);
			}

			var powerEngine = Formulas.TorqueToPower(torqueFromEngine, angSpeed * axleData.AxleGear.Ratio);
			var loss = powerEngine - PvD;

			Assert.AreEqual(double.Parse(TestContext.DataRow["GbxPowerLoss"].ToString(), CultureInfo.InvariantCulture),
				loss.Value(), 0.1,
				TestContext.DataRow["TestName"].ToString());
		}

		[TestMethod]
		public void TestLossMap_IN_10_CONST_Interpolation_Extrapolation()
		{
			var data = new DataTable();
			data.Columns.Add("");
			data.Columns.Add("");
			data.Columns.Add("");
			data.Rows.Add("0", "0", "10"); //         (0,100):10 --  (100,150):10
			data.Rows.Add("0", "100", "10"); //        |      \          |
			data.Rows.Add("100", "0", "10"); //        |       \         |
			data.Rows.Add("100", "100", "10"); //    (0,0):10  ----- (100,10):10

			var map = TransmissionLossMap.Create(data, 1.0, "1");

			// test inside the triangles
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(25.RPMtoRad(), 25.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(75.RPMtoRad(), 50.SI<NewtonMeter>()));

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), -5.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 45.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 40.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 75.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), 25.SI<NewtonMeter>()));

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()));

			// test outside the corners
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), 120.SI<NewtonMeter>()));

			// test outside the edges
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 50.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(50.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), 50.SI<NewtonMeter>()));
		}

		[TestMethod]
		public void TestLossMap_OUT_10_CONST_Interpolation_Extrapolation()
		{
			var data = new DataTable();
			data.Columns.Add("");
			data.Columns.Add("");
			data.Columns.Add("");
			data.Rows.Add("0", "0", "10"); //         (0,100):10 --  (100,150):10
			data.Rows.Add("0", "100", "10"); //        |      \          |
			data.Rows.Add("100", "0", "10"); //        |       \         |
			data.Rows.Add("100", "100", "10"); //    (0,0):10  ----- (100,10):10

			var map = TransmissionLossMap.Create(data, 1.0, "1");

			// test inside the triangles
			AssertHelper.AreRelativeEqual(15, map.GetOutTorque(25.RPMtoRad(), 25.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(40, map.GetOutTorque(75.RPMtoRad(), 50.SI<NewtonMeter>(), true));

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(-15, map.GetOutTorque(50.RPMtoRad(), -5.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(35, map.GetOutTorque(0.RPMtoRad(), 45.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(30, map.GetOutTorque(50.RPMtoRad(), 40.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(65, map.GetOutTorque(50.RPMtoRad(), 75.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(15, map.GetOutTorque(100.RPMtoRad(), 25.SI<NewtonMeter>(), true));

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(-10, map.GetOutTorque(0.RPMtoRad(), 0.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(80, map.GetOutTorque(0.RPMtoRad(), 90.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(-20, map.GetOutTorque(100.RPMtoRad(), -10.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(50, map.GetOutTorque(100.RPMtoRad(), 60.SI<NewtonMeter>(), true));

			// test outside the corners
			AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(-20.RPMtoRad(), -20.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(110, map.GetOutTorque(-20.RPMtoRad(), 120.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(120.RPMtoRad(), -20.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(110, map.GetOutTorque(120.RPMtoRad(), 120.SI<NewtonMeter>(), true));

			// test outside the edges
			AssertHelper.AreRelativeEqual(40, map.GetOutTorque(-20.RPMtoRad(), 50.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(110, map.GetOutTorque(50.RPMtoRad(), 120.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(50.RPMtoRad(), -20.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(40, map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>(), true));
		}

		[TestMethod]
		public void TestLossMap_IN_Interpolation_Extrapolation()
		{
			var data = new DataTable();
			data.Columns.Add("");
			data.Columns.Add("");
			data.Columns.Add("");
			data.Rows.Add("0", "0", "0"); //         (0,100):10 --  (100,150):40
			data.Rows.Add("0", "110", "10"); //        |      \         |
			data.Rows.Add("100", "10", "10"); //        |       \       |
			data.Rows.Add("100", "140", "40"); //    (0,0):0 ----- (100,10):10

			var map = TransmissionLossMap.Create(data, 1.0, "1");

			// test inside the triangles
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(25.RPMtoRad(), 25.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(75.RPMtoRad(), 50.SI<NewtonMeter>()));

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(50.RPMtoRad(), -5.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(4.5, map.GetTorqueLoss(0.RPMtoRad(), 45.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(9, map.GetTorqueLoss(50.RPMtoRad(), 40.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(50.RPMtoRad(), 75.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(17.5, map.GetTorqueLoss(100.RPMtoRad(), 25.SI<NewtonMeter>()));

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(9, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(28, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()));

			// test outside the corners
			AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(-20.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(-20.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(120.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(40, map.GetTorqueLoss(120.RPMtoRad(), 120.SI<NewtonMeter>()));

			// test outside the edges
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(-20.RPMtoRad(), 50.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(25, map.GetTorqueLoss(50.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(5, map.GetTorqueLoss(50.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(25, map.GetTorqueLoss(120.RPMtoRad(), 50.SI<NewtonMeter>()));
		}

		[TestMethod]
		public void TestLossMap_OUT_Interpolation_Extrapolation()
		{
			var data = new DataTable();
			data.Columns.Add("");
			data.Columns.Add("");
			data.Columns.Add("");
			data.Rows.Add("0", "0", "0"); //         (0,100):10 -- (100,100):40
			data.Rows.Add("0", "100", "10"); //        |      \        |
			data.Rows.Add("100", "0", "10"); //        |       \       |
			data.Rows.Add("100", "100", "40"); //    (0,0):0 ----- (100,0):10

			var map = TransmissionLossMap.Create(data, 1.0, "1");

			// test inside the triangles
			AssertHelper.AreRelativeEqual(20, map.GetOutTorque(25.RPMtoRad(), 25.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(32.5, map.GetOutTorque(75.RPMtoRad(), 50.SI<NewtonMeter>()));

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(-5, map.GetOutTorque(50.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(40.5, map.GetOutTorque(0.RPMtoRad(), 45.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(31, map.GetOutTorque(50.RPMtoRad(), 40.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(57.5, map.GetOutTorque(50.RPMtoRad(), 75.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(7.5, map.GetOutTorque(100.RPMtoRad(), 25.SI<NewtonMeter>()));

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(0, map.GetTorqueLoss(0.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(0.RPMtoRad(), 90.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(10, map.GetTorqueLoss(100.RPMtoRad(), -10.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(40, map.GetTorqueLoss(100.RPMtoRad(), 60.SI<NewtonMeter>()));

			// test outside the corners
			AssertHelper.AreRelativeEqual(-20, map.GetOutTorque(-20.RPMtoRad(), -20.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(110, map.GetOutTorque(-20.RPMtoRad(), 120.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(-30, map.GetOutTorque(120.RPMtoRad(), -20.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(80, map.GetOutTorque(120.RPMtoRad(), 120.SI<NewtonMeter>(), true));

			// test outside the edges
			AssertHelper.AreRelativeEqual(45, map.GetOutTorque(-20.RPMtoRad(), 50.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(95, map.GetOutTorque(50.RPMtoRad(), 120.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(-25, map.GetOutTorque(50.RPMtoRad(), -20.SI<NewtonMeter>(), true));
			AssertHelper.AreRelativeEqual(25, map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>(), true));

			// test extrapolation not allowed
			AssertHelper.Exception<VectoException>(() => { map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>()); });
		}

		[TestMethod]
		public void TestFullLoadCurveIntersection()
		{
			var engineFLDString = new[] {
				"560, 1180, -149",
				"600, 1282, -148",
				"800, 1791, -149",
				"1000, 2300, -160",
				"1200, 2300, -179",
				"1400, 2300, -203",
				"1600, 2079, -235",
				"1800, 1857, -264",
				"2000, 1352, -301",
				"2100, 1100, -320",
			};
			var gbxFLDString = new[] {
				"560, 2500",
				"2100, 2500"
			};
			var dataEng =
				VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm]", engineFLDString));
			var engineFLD = EngineFullLoadCurve.Create(dataEng, true);

			var dataGbx = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm]", gbxFLDString));
			var gbxFLD = FullLoadCurve.Create(dataGbx, true);

			var fullLoadCurve = AbstractSimulationDataAdapter.IntersectFullLoadCurves(engineFLD, gbxFLD);

			Assert.AreEqual(10, fullLoadCurve.FullLoadEntries.Count);

			Assert.AreEqual(1180.0, fullLoadCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value());
			Assert.AreEqual(1100.0, fullLoadCurve.FullLoadStationaryTorque(2100.RPMtoRad()).Value());
		}

		/// <summary>
		///		VECTO-190
		/// </summary>
		[TestMethod]
		public void TestFullLoadSorting()
		{
			var gbxFLDString = new[] {
				"600, 1000",
				"2400, 2000",
				"1000, 500"
			};

			var dataGbx = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm]", gbxFLDString));
			var gbxFLD = FullLoadCurve.Create(dataGbx, true);

			var maxTorque = gbxFLD.FullLoadStationaryTorque(800.RPMtoRad());
			Assert.AreEqual(750, maxTorque.Value());
		}

		protected PerSecond SpeedToAngularSpeed(double v, double r)
		{
			return ((60 * v) / (2 * r * Math.PI / 1000)).RPMtoRad();
		}
	}
}