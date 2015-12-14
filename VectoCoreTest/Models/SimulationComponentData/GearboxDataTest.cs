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
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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
				torqueFromEngine = axleData.LossMap.GearboxInTorque(angSpeed, torqueToWheels);
			}

			var powerEngine = Formulas.TorqueToPower(torqueFromEngine, angSpeed);
			var loss = powerEngine - PvD;

			Assert.AreEqual(double.Parse(TestContext.DataRow["GbxPowerLoss"].ToString(), CultureInfo.InvariantCulture),
				loss.Value(), 0.1,
				TestContext.DataRow["TestName"].ToString());
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