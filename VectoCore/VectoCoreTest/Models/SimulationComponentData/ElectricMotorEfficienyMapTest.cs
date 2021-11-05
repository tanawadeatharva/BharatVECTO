using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData {
	[TestFixture]
	public class ElectricMotorEfficienyMapTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		
		[TestCase(-10000, 700, -120.6737), // EM drive has negative torque and thus negative electric power
		TestCase(-20000, 1200, -141.5367),
		TestCase(10000, 700, 153.5121),
		TestCase(20000, 1200, 177.4109)]
		public void TestLookupTorqueForBatPower(double batPwr, double emSpeed, double expectedTq)
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			var fldMap = ElectricFullLoadCurveReader.Create(fld, 1);

			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap; //ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.NotNull(tq);
			var lookup = pwrMap.LookupElectricPower(emSpeed.RPMtoRad(), tq, true);

			Assert.AreEqual(lookup.ElectricalPower.Value(), batPwr, 1e-6);
			Assert.AreEqual(expectedTq, tq.Value(), 1e-3);
		}

		[TestCase(-1000000, 700), // EM drive has negative torque and thus negative electric power
		TestCase(1000000, 700)]
		public void TestLookupTorqueForBatPower2(double batPwr, double emSpeed)
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			var fld = inputProvider.VoltageLevels.First().FullLoadCurve;
			var fldMap = ElectricFullLoadCurveReader.Create(fld, 1); 
			
			var pwr = inputProvider.VoltageLevels.First().PowerMap.First().PowerMap;//ToDo FK: maybe wrong selection
			// var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.IsNull(tq);

		}
	}
}