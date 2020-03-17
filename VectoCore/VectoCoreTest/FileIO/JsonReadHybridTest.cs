using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	public class JsonReadHybridTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase()]
		public void TestReadBatteryPack()
		{
			var inputProvider = JSONInputDataFactory.ReadBatteryData(@"TestData\Hybrids\Battery\GenericBattery.vbat", false);

			Assert.AreEqual(14.SI(Unit.SI.Ampere.Hour), inputProvider.Capacity);
			
			var soc = inputProvider.Voltage;
			Assert.AreEqual("0", soc.Rows[0][BatterySOCReader.Fields.StateOfCharge]);
			Assert.AreEqual("590", soc.Rows[0][BatterySOCReader.Fields.BatteryVoltage]);

			Assert.AreEqual(20, inputProvider.MinSOC);
			Assert.AreEqual(80, inputProvider.MaxSOC);
			Assert.AreEqual(5, inputProvider.MaxCurrentFactor);

			var socMap = BatterySOCReader.Create(soc);

			Assert.AreEqual(590, socMap.Lookup(0).Value());
			Assert.AreEqual(658, socMap.Lookup(1).Value());
			Assert.AreEqual(640, socMap.Lookup(0.5).Value());

			Assert.AreEqual(639, socMap.Lookup(0.45).Value());
		}

		[TestCase()]
		public void TestReadElectricMotor()
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			Assert.AreEqual(0.15, inputProvider.Inertia.Value(), 1e-6);

			var fld = inputProvider.FullLoadCurve;
			Assert.AreEqual("0", fld.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("401.07", fld.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-401.07", fld.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMap = ElectricFullLoadCurveReader.Create(fld);
			Assert.AreEqual(401.07, fldMap.FullLoadDriveTorque(0.RPMtoRad()).Value());
			Assert.AreEqual(-407.07, fldMap.FullGenerationTorque(0.RPMtoRad()).Value());

			var pwr = inputProvider.EfficiencyMap;
			Assert.AreEqual("0", pwr.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwr.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("9.8449", pwr.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMap = ElectricMotorMapReader.Create(pwr);
			Assert.AreEqual(9844.9, pwrMap.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value());
		}

	}
}