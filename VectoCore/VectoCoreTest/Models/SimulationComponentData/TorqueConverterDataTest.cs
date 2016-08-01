using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	public class TorqueConverterDataTest
	{
		[Test,
		TestCase(30, 10, 1230.7, 842.8317),
		TestCase(40, 10, 1081.028, 645.9384),
		TestCase(50, 10, 981.7928, 528.8284),
		TestCase(60, 10, 912.2327, 452.9006),
		TestCase(70, 10, 860.6045, 399.6834),
		TestCase(70, 30, 1433.893, 1129.279),
		TestCase(90, 10, 789.4108, 330.28),
		TestCase(90, 30, 1295.913, 912.1714),
		TestCase(170, 30, 1055.424, 574.8998),
		TestCase(170, 50, 1308.974, 900.0845),
		TestCase(190, 30, 1029.455, 539.6072),
		TestCase(190, 50, 1269.513, 837.2174),
		TestCase(190, 70, 1465.33, 1128.088),]
		public void TestTorqueConverterOperatingPoint(double nOut, double Pout, double nInExpected, double tqInExpected)
		{
			var tqLimit = 1600;
			Assert.IsTrue(nInExpected < tqLimit);
			var tqInput = new[] {
				"0,3.935741,563.6598  ",
				"0.1,3.296827,534.1364",
				"0.2,2.701476,504.6129",
				"0.3,2.265852,472.1372",
				"0.4,1.931875,421.9474",
				"0.5,1.554335,354.0435",
				"0.6,1.249399,268.4255",
				"0.7,1.075149,114.9037",
			};

			var tqData =
				TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
					tqInput), 1000.RPMtoRad());

			PerSecond inAngularSpeed;
			NewtonMeter inTorque;


			var outAngularSpeed = nOut.RPMtoRad();
			var outTorque = (Pout * 1000).SI<Watt>() / outAngularSpeed;
			tqData.GetInputTorqueAndAngularSpeed(outTorque, outAngularSpeed, out inTorque, out inAngularSpeed);

			Assert.IsTrue(inAngularSpeed.Value() < 1600.RPMtoRad().Value());
			Assert.AreEqual(nInExpected.RPMtoRad().Value(), inAngularSpeed.Value(), 5);
			Assert.AreEqual(tqInExpected, inTorque.Value(), 10);
		}

		[Test,
		TestCase(10, 110),
		TestCase(20, 130),
		TestCase(50, 90),
		TestCase(50, 190),
		TestCase(60, 150),
		TestCase(70, 70),
		TestCase(70, 90),
		TestCase(70, 190),
		TestCase(80, 50),
		TestCase(80, 130),
		TestCase(90, 70),
		TestCase(100, 150),
		TestCase(130, 70),
		TestCase(150, 80),
		TestCase(170, 80),
		]
		public void TestTorqueConverterInvalidOperatingPoint(double nOut, double Pout)
		{
			var tqLimit = 1600.RPMtoRad();

			var tqInput = new[] {
				"0,3.935741,563.6598  ",
				"0.1,3.296827,534.1364",
				"0.2,2.701476,504.6129",
				"0.3,2.265852,472.1372",
				"0.4,1.931875,421.9474",
				"0.5,1.554335,354.0435",
				"0.6,1.249399,268.4255",
				"0.7,1.075149,114.9037",
			};

			var tqData =
				TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
					tqInput), 1000.RPMtoRad());

			PerSecond inAngularSpeed;
			NewtonMeter inTorque;


			var outAngularSpeed = nOut.RPMtoRad();
			var outTorque = (Pout * 1000).SI<Watt>() / outAngularSpeed;
			tqData.GetInputTorqueAndAngularSpeed(outTorque, outAngularSpeed, out inTorque, out inAngularSpeed);

			Assert.IsTrue(inAngularSpeed.Value() > tqLimit.Value());
		}
	}
}