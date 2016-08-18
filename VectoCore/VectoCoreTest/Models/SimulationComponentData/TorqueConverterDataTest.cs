using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	public class TorqueConverterDataTest
	{
		[Test,
		TestCase(30, 10, 227.8707, 1780.6630),
		TestCase(40, 10, 197.9536, 1340.4737),
		TestCase(50, 10, 177.6759, 1076.9007),
		TestCase(60, 10, 162.8219, 901.5900),
		TestCase(70, 10, 151.3747, 776.6864),
		TestCase(70, 30, 259.9515, 2306.1578),
		TestCase(90, 10, 134.7344, 610.8267),
		TestCase(90, 30, 230.4896, 1805.2978),
		TestCase(170, 30, 172.2875, 987.4020),
		TestCase(170, 50, 220.2397, 1626.0613),
		TestCase(190, 30, 164.3260, 892.7507),
		TestCase(190, 50, 209.6016, 1465.6611),
		TestCase(190, 70, 246.4829, 2036.7399),
		TestCase(530.517, 1.930, 59.1449, 36.5684),
		]
		public void TestTorqueConverterOperatingPoint(double nOut, double Pout, double nInExpected, double tqInExpected)
		{
			var tqLimit = 1600;
			//Assert.IsTrue(nInExpected < tqLimit);
			var tqInput = new[] {
				"0.0,1.80,377.80",
				"0.1,1.71,365.21",
				"0.2,1.61,352.62",
				"0.3,1.52,340.02",
				"0.4,1.42,327.43",
				"0.5,1.33,314.84",
				"0.6,1.23,302.24",
				"0.7,1.14,264.46",
				"0.8,1.04,226.68",
				"0.9,0.95,188.90",
				"1.0,0.95,0.00",
			};

			var tqData =
				TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
					tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad());

			var outAngularSpeed = nOut.RPMtoRad();
			var outTorque = (Pout * 1000).SI<Watt>() / outAngularSpeed;
			var result = tqData.FindOperatingPoint(outTorque, outAngularSpeed);

			Assert.AreEqual(outAngularSpeed.Value(), result.OutAngularVelocity.Value(), 1e-3);
			Assert.AreEqual(outTorque.Value(), result.OutTorque.Value(), 1e-3);

			Debug.WriteLine("n_in: {0}, tq_in: {1}", result.InAngularVelocity.AsRPM, result.InTorque.Value());
			//Assert.IsTrue(result.InAngularVelocity.Value() < 1600.RPMtoRad().Value());
			Assert.AreEqual(nInExpected, result.InAngularVelocity.Value(), 1e-3);
			Assert.AreEqual(tqInExpected, result.InTorque.Value(), 1e-3);
		}

		[Test,
		TestCase(898, 463)]
		public void TestTorqueConverterOperatingPointForward(double nIn, double tqIn)
		{
			var tqLimit = 1600;

			var tqInput = new[] {
				"0.0,  4.5, 700",
				"0.1,  3.5, 640	",
				"0.2,  2.7, 560	",
				"0.3,  2.2, 460	",
				"0.4,  1.6, 350	",
				"0.5,  1.2, 250	",
				"0.6,  0.9, 160	",
				"0.74, 0.9,   1",
			};

			var tqData =
				TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
					tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad());


			var operatingPoint = tqData.GetOutTorqueAndSpeed(tqIn.SI<NewtonMeter>(), nIn.RPMtoRad(), null);

			Assert.AreEqual(operatingPoint.InTorque.Value(), tqIn, 1e-6);
			Assert.AreEqual(operatingPoint.InAngularVelocity.Value(), nIn.RPMtoRad().Value(), 1e-6);


			var reverseOP = tqData.FindOperatingPoint(operatingPoint.OutTorque, operatingPoint.OutAngularVelocity);

			Assert.AreEqual(operatingPoint.InTorque.Value(), reverseOP.InTorque.Value(), 1e-6);
			Assert.AreEqual(operatingPoint.OutTorque.Value(), reverseOP.OutTorque.Value(), 1e-6);
			Assert.AreEqual(operatingPoint.InAngularVelocity.Value(), reverseOP.InAngularVelocity.Value(), 1e-6);
			Assert.AreEqual(operatingPoint.OutAngularVelocity.Value(), reverseOP.OutAngularVelocity.Value(), 1e-6);
		}

		[Test]
		public void TestTorqueConverterComparisonV2()
		{
			var tqLimit = 1600;

			var tqInput = new[] {
				"0.0,1.80,377.80",
				"0.1,1.71,365.21",
				"0.2,1.61,352.62",
				"0.3,1.52,340.02",
				"0.4,1.42,327.43",
				"0.5,1.33,314.84",
				"0.6,1.23,302.24",
				"0.7,1.14,264.46",
				"0.8,1.04,226.68",
				"0.9,0.95,188.90",
				"1.0,0.95,0.00",
			};
			var testData = new List<Tuple<double, double>>() {
				Tuple.Create(139.4087374, 72.74847642),
				Tuple.Create(2275.286998, 453.9413043),
				Tuple.Create(1507.256216, 1012.44118),
				Tuple.Create(26.58522419, 29.25312925),
				Tuple.Create(1752.83589, 328.6386397),
			};
			var tqData =
				TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
					tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad());

			foreach (var entry in testData) {
				var torqueTCOut = entry.Item1.SI<NewtonMeter>();
				var angularSpeedOut = entry.Item2.RPMtoRad();
				var result = tqData.FindOperatingPoint(torqueTCOut, angularSpeedOut);
				Debug.WriteLine("n_out: {0}, tq_out: {1}, n_in: {2}, Tq_in: {3}", angularSpeedOut.Value() / Constants.RPMToRad,
					torqueTCOut.Value(), result.InAngularVelocity.Value() / Constants.RPMToRad, result.InTorque.Value());
			}
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
					tqInput), 1000.RPMtoRad(), tqLimit);

			var outAngularSpeed = nOut.RPMtoRad();
			var outTorque = (Pout * 1000).SI<Watt>() / outAngularSpeed;
			var result = tqData.FindOperatingPoint(outTorque, outAngularSpeed);

			Assert.IsTrue(result.InAngularVelocity.Value() > tqLimit.Value());
		}
	}
}