using System;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	public class TorqueConverterDataTest
	{
		[Test]
		public void TestTorqueConverterOperatingPoint()
		{
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
			;
			var tqData =
				TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
					tqInput));

			PerSecond inAngularSpeed;
			NewtonMeter inTorque;

			tqData.GetInputTorqueAndAngularSpeed(400.SI<NewtonMeter>(), 10.RPMtoRad(), out inTorque, out inAngularSpeed);

			Assert.AreEqual(0, inTorque.Value());
			Assert.AreEqual(0, inAngularSpeed.Value());
		}
	}
}