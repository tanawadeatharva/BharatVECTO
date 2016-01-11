/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Data;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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

			var gbxData = MockSimulationDataFactory.CreateGearboxDataFromFile(TestContext.DataRow["GearboxDataFile"].ToString(),
				EngineFile);
			var axleData = MockSimulationDataFactory.CreateAxleGearDataFromFile(TestContext.DataRow["GearboxDataFile"].ToString());

			var PvD = double.Parse(TestContext.DataRow["PowerGbxOut"].ToString(), CultureInfo.InvariantCulture).SI<Watt>();

			var torqueToWheels = Formulas.PowerToTorque(PvD, SpeedToAngularSpeed(speed, rdyn));
			var torqueFromEngine = 0.SI<NewtonMeter>();

			var angSpeed = SpeedToAngularSpeed(speed, rdyn) * axleData.Ratio;
			if (TestContext.DataRow["Gear"].ToString() == "A") {
				torqueFromEngine = axleData.LossMap.GetInTorque(angSpeed, torqueToWheels);
			}

			var powerEngine = Formulas.TorqueToPower(torqueFromEngine, angSpeed);
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
			AssertHelper.AreRelativeEqual(35, map.GetInTorque(25.RPMtoRad(), 25.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(60, map.GetInTorque(75.RPMtoRad(), 50.SI<NewtonMeter>()));

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(5, map.GetInTorque(50.RPMtoRad(), -5.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(55, map.GetInTorque(0.RPMtoRad(), 45.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(50, map.GetInTorque(50.RPMtoRad(), 40.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(85, map.GetInTorque(50.RPMtoRad(), 75.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(35, map.GetInTorque(100.RPMtoRad(), 25.SI<NewtonMeter>()));

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(10, map.GetInTorque(0.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(100, map.GetInTorque(0.RPMtoRad(), 90.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(0, map.GetInTorque(100.RPMtoRad(), -10.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(70, map.GetInTorque(100.RPMtoRad(), 60.SI<NewtonMeter>()));

			// test outside the corners
			AssertHelper.AreRelativeEqual(-10, map.GetInTorque(-20.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(130, map.GetInTorque(-20.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(-10, map.GetInTorque(120.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(130, map.GetInTorque(120.RPMtoRad(), 120.SI<NewtonMeter>()));

			// test outside the edges
			AssertHelper.AreRelativeEqual(60, map.GetInTorque(-20.RPMtoRad(), 50.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(130, map.GetInTorque(50.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(-10, map.GetInTorque(50.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(60, map.GetInTorque(120.RPMtoRad(), 50.SI<NewtonMeter>()));
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
			AssertHelper.AreRelativeEqual(30, map.GetInTorque(25.RPMtoRad(), 25.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(67.5, map.GetInTorque(75.RPMtoRad(), 50.SI<NewtonMeter>()));

			// test interpolation on edges
			AssertHelper.AreRelativeEqual(0, map.GetInTorque(50.RPMtoRad(), -5.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(49.5, map.GetInTorque(0.RPMtoRad(), 45.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(49, map.GetInTorque(50.RPMtoRad(), 40.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(92.5, map.GetInTorque(50.RPMtoRad(), 75.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(42.5, map.GetInTorque(100.RPMtoRad(), 25.SI<NewtonMeter>()));

			// test interpolation on corner points
			AssertHelper.AreRelativeEqual(0, map.GetInTorque(0.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(99, map.GetInTorque(0.RPMtoRad(), 90.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(0, map.GetInTorque(100.RPMtoRad(), -10.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(88, map.GetInTorque(100.RPMtoRad(), 60.SI<NewtonMeter>()));

			// test outside the corners
			AssertHelper.AreRelativeEqual(-20, map.GetInTorque(-20.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(130, map.GetInTorque(-20.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(-10, map.GetInTorque(120.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(160, map.GetInTorque(120.RPMtoRad(), 120.SI<NewtonMeter>()));

			// test outside the edges
			AssertHelper.AreRelativeEqual(55, map.GetInTorque(-20.RPMtoRad(), 50.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(145, map.GetInTorque(50.RPMtoRad(), 120.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(-15, map.GetInTorque(50.RPMtoRad(), -20.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(75, map.GetInTorque(120.RPMtoRad(), 50.SI<NewtonMeter>()));
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
			AssertHelper.AreRelativeEqual(0, map.GetInTorque(0.RPMtoRad(), 0.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(100, map.GetInTorque(0.RPMtoRad(), 90.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(0, map.GetInTorque(100.RPMtoRad(), -10.SI<NewtonMeter>()));
			AssertHelper.AreRelativeEqual(100, map.GetInTorque(100.RPMtoRad(), 60.SI<NewtonMeter>()));

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
			AssertHelper.Exception<VectoException>(() => {
				map.GetOutTorque(120.RPMtoRad(), 50.SI<NewtonMeter>());
			});
		}

		[TestMethod]
		public void TestInputOutOfRange()
		{
			var gbxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxFile, EngineFile);

			Assert.Inconclusive("test another file which is not correct");
		}

		protected PerSecond SpeedToAngularSpeed(double v, double r)
		{
			return ((60 * v) / (2 * r * Math.PI / 1000)).RPMtoRad();
		}
	}
}