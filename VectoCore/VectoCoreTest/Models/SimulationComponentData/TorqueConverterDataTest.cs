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
		TestCase(190, 70, 1465.33, 1128.088),
		TestCase(530.517, 1.930, 100, 100),
		]
		public void TestTorqueConverterOperatingPoint(double nOut, double Pout, double nInExpected, double tqInExpected)
		{
			var tqLimit = 1600;
			Assert.IsTrue(nInExpected < tqLimit);
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

			// TODO!
			//Assert.IsTrue(inAngularSpeed.Value() < 1600.RPMtoRad().Value());
			Debug.WriteLine("n_out: {0}, P_out: {1}, n_in: {2}, Tq_in: {3}", nOut, Pout, result.InAngularVelocity,
				result.InTorque);
			//Assert.AreEqual(nInExpected.RPMtoRad().Value(), inAngularSpeed.Value(), 5);
			//Assert.AreEqual(tqInExpected, inTorque.Value(), 10);
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

		[Test]
		public void TestTorqueConverterComparisonV2_ZF_EcoLife()
		{
			var tqLimit = 1600;

			var tqInput = new[] {
				"0,2.29,646",
				"0.1,2.16,663",
				"0.2,1.99,643",
				"0.3,1.84,611",
				"0.4,1.67,586",
				"0.5,1.51,556",
				"0.6,1.35,521",
				"0.7,1.21,465",
				"0.8,1.07,404",
				"0.9,0.99,228",
				"1,0.99,0",
				"1.1,0.991,-103",
				"1.2,0.994,-172",
				"1.3,0.994,-229",
				"1.4,0.996,-286",
				"1.5,0.996,-346",
				"1.6,0.996,-407",
				"1.7,0.996,-460",
				"1.8,0.996,-546",
				"1.9,0.996,-616",
				"2,0.997,-685",
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


		[Test]
		public void TestTorqueConverterComparisonV2_ZF_W410_6_TP2_575_mue232()
		{
			var tqLimit = 1600;

			var tqInput = new[] {
				"0.000,2.320,518.3",
				"0.100,2.180,545.0",
				"0.200,2.010,575.4",
				"0.300,1.850,573.1",
				"0.400,1.690,572.5",
				"0.500,1.520,553.1",
				"0.600,1.360,516.6",
				"0.700,1.220,460.0",
				"0.800,1.080,397.1",
				"0.900,1.000,246.8",
				"0.950,0.990,113.6",
				"1.000,0.990,0.0",
				"1.100,0.991,-103.0",
				"1.200,0.994,-172.0",
				"1.300,0.994,-229.0",
				"1.400,0.996,-286.0",
				"1.500,0.996,-346.0",
				"1.600,0.996,-407.0",
				"1.700,0.996,-460.0",
				"1.800,0.996,-546.0",
				"1.900,0.996,-616.0",
				"2.000,0.997,-685.0",
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