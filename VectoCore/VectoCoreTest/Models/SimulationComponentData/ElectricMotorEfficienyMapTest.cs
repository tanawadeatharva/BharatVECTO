using System;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData {
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
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

			var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
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

			var pwr = inputProvider.VoltageLevels.First().EfficiencyMap;
			var pwrMap = ElectricMotorMapReader.Create(pwr, 1);

			var maxEmPwr = batPwr < 0
				? fldMap.FullLoadDriveTorque(emSpeed.RPMtoRad())
				: fldMap.FullGenerationTorque(emSpeed.RPMtoRad());
			var tq = pwrMap.LookupTorque(batPwr.SI<Watt>(), emSpeed.RPMtoRad(), maxEmPwr);

			Assert.IsNull(tq);

		}

		[TestCase(@"E:\QUAM\Downloads\VECTO-1437_E2_EM-inv_Interpolation\VECTO_model\vem_P_inverter_DC_90.vemo")]
		public void TestInterpolationMethod_Proposal(string filename)
		{
			var speedOffset = 0.01.RPMtoRad();

			var tbl = VectoCSVFile.Read(filename);
			var delaunayMap = new DelaunayMap("ElectricMotorEfficiencyMap Test");
			foreach (DataRow row in tbl.Rows) {
				var entry = new EfficiencyMap.Entry(
					speed: row.ParseDouble("n").RPMtoRad(),
					torque: row.ParseDouble("T").SI<NewtonMeter>(),
					powerElectrical: row.ParseDouble("P_el").SI(Unit.SI.Kilo.Watt).Cast<Watt>());
				
					delaunayMap.AddPoint(-entry.Torque.Value(), 
						(entry.MotorSpeed).Value(), 
						-((entry.PowerElectrical - entry.MotorSpeed * entry.Torque) / (entry.MotorSpeed + speedOffset)).Value());
			}

			delaunayMap.Triangulate();
			var emMap = new EfficiencyMap(delaunayMap);

			for (var n = 10.RPMtoRad(); n < 4000.RPMtoRad(); n += 10.RPMtoRad()) {
				for (var tq = -2800.SI<NewtonMeter>(); tq <= 2800.SI<NewtonMeter>(); tq += 100.SI<NewtonMeter>()) {
					if (tq.IsEqual(0)) {
						continue;
					}
					try {
						var pwr = emMap.LookupElectricPower(n, tq);
						if (pwr.ElectricalPower == null) {
							continue;
						}
						Console.WriteLine($"{pwr.Speed.AsRPM}, {pwr.Torque.Value()}, {((pwr.ElectricalPower.Value() * (pwr.Speed + speedOffset).Value()).SI<Watt>() + pwr.Speed * pwr.Torque).Value()}, {pwr.Extrapolated}, {(n * tq).Value()}");
					} catch (Exception e) {
						Console.WriteLine(e.Message);
					}
				}
			}
		}

		[TestCase(@"E:\QUAM\Downloads\VECTO-1437_E2_EM-inv_Interpolation\VECTO_model\vem_P_inverter_DC_90.vemo")]
		public void TestInterpolationMethod_Proposal2(string filename)
		{
			EfficiencyMap emMap;
			using (var fs = File.OpenRead(filename)) {
				emMap = ElectricMotorMapReaderNew.Create(fs, 1);
			}

			for (var n = 10.RPMtoRad(); n < 4000.RPMtoRad(); n += 10.RPMtoRad()) {
				for (var tq = -2800.SI<NewtonMeter>(); tq <= 2800.SI<NewtonMeter>(); tq += 100.SI<NewtonMeter>()) {
					if (tq.IsEqual(0)) {
						continue;
					}
					try {
						var pwr = emMap.LookupElectricPower(n, tq);
						if (pwr.ElectricalPower == null) {
							continue;
						}
						Console.WriteLine($"{pwr.Speed.AsRPM}, {pwr.Torque.Value()}, {pwr.ElectricalPower.Value()}, {pwr.Extrapolated}, {(n * tq).Value()}");
					} catch (Exception e) {
						Console.WriteLine(e.Message);
					}
				}
			}
		}

		[TestCase(@"E:\QUAM\Downloads\VECTO-1437_E2_EM-inv_Interpolation\VECTO_model\vem_P_inverter_DC_90.vemo")]
		public void TestInterpolationMethod_Current(string filename)
		{
			EfficiencyMap emMap;
			using (var fs = File.OpenRead(filename)) {
				emMap = ElectricMotorMapReader.Create(fs, 1);
			}

			for (var n = 10.RPMtoRad(); n < 4000.RPMtoRad(); n += 10.RPMtoRad()) {
				for (var tq = -2800.SI<NewtonMeter>(); tq <= 2800.SI<NewtonMeter>(); tq += 100.SI<NewtonMeter>()) {
					if (tq.IsEqual(0)) {
						continue;
					}
					try {
						var pwr = emMap.LookupElectricPower(n, tq);
						if (pwr.ElectricalPower == null) {
							continue;
						}
						Console.WriteLine($"{pwr.Speed.AsRPM}, {pwr.Torque.Value()}, {pwr.ElectricalPower.Value()}, {pwr.Extrapolated}, {(n * tq).Value()}");
					} catch (Exception e) {
						Console.WriteLine(e.Message);
					}
				}
			}
		}
	}
}