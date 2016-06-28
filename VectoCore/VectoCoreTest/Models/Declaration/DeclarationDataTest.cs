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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using CrossWindCorrectionMode = TUGraz.VectoCommon.Models.CrossWindCorrectionMode;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationDataTest
	{
		public const double Tolerance = 0.0001;
		public readonly MissionType[] Missions = Enum.GetValues(typeof(MissionType)).Cast<MissionType>().ToArray();

		[Test,
		TestCase("285/70 R19.5", 7.9, 0.8943, "b"),
		]
		public void WheelDataTest(string wheels, double intertia, double dynamicRadius, string sizeClass)
		{
			var tmp = DeclarationData.Wheels.Lookup(wheels);

			Assert.AreEqual(intertia, tmp.Inertia.Value(), Tolerance);
			Assert.AreEqual(dynamicRadius, tmp.DynamicTyreRadius.Value(), Tolerance);
			Assert.AreEqual(sizeClass, tmp.SizeClass);
		}

		[Test,
		TestCase("15° DC Rims", 3.03, 3.05),
		]
		public void RimsDataTest(string rim, double fa, double fb)
		{
			var tmp = DeclarationData.Rims.Lookup(rim);

			Assert.AreEqual(fa, tmp.F_a, Tolerance);
			Assert.AreEqual(fb, tmp.F_b, Tolerance);
		}

		[Test,
		// fixed points
		TestCase(400, 0),
		TestCase(800, 0.47),
		TestCase(1000, 0.58),
		TestCase(1200, 0.53),
		TestCase(1400, 0.46),
		TestCase(1500, 0.43),
		TestCase(1750, 0.22),
		TestCase(1800, 0.2),
		TestCase(2000, 0.11),
		TestCase(2500, 0.11),
		// interpolate
		TestCase(600, 0.235),
		TestCase(900, 0.525),
		TestCase(1100, 0.555),
		TestCase(1300, 0.495),
		TestCase(1450, 0.445),
		TestCase(1625, 0.325),
		TestCase(1775, 0.21),
		TestCase(1900, 0.155),
		TestCase(2250, 0.11),
		// extrapolate
		TestCase(3000, 0.11),
		]
		public void PT1Test(double rpm, double expectedPt1)
		{
			Assert.AreEqual(expectedPt1, DeclarationData.PT1.Lookup(rpm.RPMtoRad()).Value(), Tolerance);
		}

		public void PT1ExceptionsTest()
		{
			// EXTRAPOLATE 
			AssertHelper.Exception<VectoException>(() => DeclarationData.PT1.Lookup(200.RPMtoRad()));
			AssertHelper.Exception<VectoException>(() => DeclarationData.PT1.Lookup(0.RPMtoRad()));
		}

		[Test]
		public void WHTCTest()
		{
			var whtc = DeclarationData.WHTCCorrection;

			var factors = new {
				urban = new[] { 0.11, 0.17, 0.69, 0.98, 0.62, 1.0, 1.0, 1.0, 0.45, 0.0 },
				rural = new[] { 0.0, 0.3, 0.27, 0.0, 0.32, 0.0, 0.0, 0.0, 0.36, 0.22 },
				motorway = new[] { 0.89, 0.53, 0.04, 0.02, 0.06, 0.0, 0.0, 0.0, 0.19, 0.78 }
			};

			var r = new Random();
			for (var i = 0; i < Missions.Length; i++) {
				var urban = r.NextDouble() * 2;
				var rural = r.NextDouble() * 2;
				var motorway = r.NextDouble() * 2;
				var whtcValue = whtc.Lookup(Missions[i], rural: rural, urban: urban, motorway: motorway);
				Assert.AreEqual(urban * factors.urban[i] + rural * factors.rural[i] + motorway * factors.motorway[i],
					whtcValue);
			}
		}

		[TestMethod]
		public void WHTCLookupTestLongHaul()
		{
			var expected = 1.015501;

			var rural = 1.0265;
			var urban = 1.0948;
			var motorway = 1.0057;

			var lookup = DeclarationData.WHTCCorrection.Lookup(MissionType.LongHaul, rural: rural, urban: urban,
				motorway: motorway);
			Assert.AreEqual(expected, lookup, 1e-8);
		}

		[TestMethod]
		public void WHTCLookupTestRegionalDelivery()
		{
			var expected = 1.02708700;

			var rural = 1.0265;
			var urban = 1.0948;
			var motorway = 1.0057;

			var lookup = DeclarationData.WHTCCorrection.Lookup(MissionType.RegionalDelivery, rural: rural, urban: urban,
				motorway: motorway);
			Assert.AreEqual(expected, lookup, 1e-8);
		}

		[Test]
		public void AirDragTest()
		{
			var airDrag = DeclarationData.AirDrag;

			var expected = new Dictionary<string, AirDrag.AirDragEntry> {
				{ "RigidSolo", new AirDrag.AirDragEntry { A1 = 0.013526, A2 = 0.017746, A3 = -0.000666 } },
				{ "RigidTrailer", new AirDrag.AirDragEntry { A1 = 0.017125, A2 = 0.072275, A3 = -0.004148 } },
				{ "TractorSemitrailer", new AirDrag.AirDragEntry { A1 = 0.034767, A2 = 0.039367, A3 = -0.001897 } },
				{ "CoachBus", new AirDrag.AirDragEntry { A1 = -0.000794, A2 = 0.02109, A3 = -0.00109 } }
			};

			foreach (var kv in expected) {
				Assert.AreEqual(kv.Value, airDrag.Lookup(kv.Key));
			}

			var expectedCat = new Dictionary<VehicleCategory, AirDrag.AirDragEntry> {
				{
					VehicleCategory.RigidTruck,
					new AirDrag.AirDragEntry { A1 = 0.013526, A2 = 0.017746, A3 = -0.000666 }
				}, {
					VehicleCategory.Tractor,
					new AirDrag.AirDragEntry { A1 = 0.034767, A2 = 0.039367, A3 = -0.001897 }
				}, {
					VehicleCategory.CityBus,
					new AirDrag.AirDragEntry { A1 = -0.000794, A2 = 0.02109, A3 = -0.00109 }
				}, {
					VehicleCategory.Coach, new AirDrag.AirDragEntry { A1 = -0.000794, A2 = 0.02109, A3 = -0.00109 }
				}, {
					VehicleCategory.InterurbanBus,
					new AirDrag.AirDragEntry { A1 = -0.000794, A2 = 0.02109, A3 = -0.00109 }
				}
			};

			foreach (var kv in expectedCat) {
				Assert.AreEqual(kv.Value, airDrag.Lookup(kv.Key));
			}
		}

		[
			TestCase(VehicleCategory.Tractor, 6.46, 0, 8.12204),
			TestCase(VehicleCategory.Tractor, 6.46, 60, 8.12204),
			TestCase(VehicleCategory.Tractor, 6.46, 75, 7.67058),
			TestCase(VehicleCategory.Tractor, 6.46, 100, 7.23735),
			TestCase(VehicleCategory.Tractor, 6.46, 52.1234, 8.12196),
			TestCase(VehicleCategory.Tractor, 6.46, 73.5432, 7.70815),
			TestCase(VehicleCategory.Tractor, 6.46, 92.8765, 7.33443),
			TestCase(VehicleCategory.Tractor, 6.46, 100.449, 7.2321466),
			TestCase(VehicleCategory.Tractor, 6.46, 103, 7.2025564),
			TestCase(VehicleCategory.Tractor, 6.46, 105, 7.17936),
			TestCase(VehicleCategory.Tractor, 6.46, 115, 7.08174),
			TestCase(VehicleCategory.Tractor, 6.46, 130, 6.96979),
		]
		public void CrossWindCorrectionTest(VehicleCategory vehicleCategory, double crossSectionArea, double kmph,
			double expected)
		{
			var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				DeclarationDataAdapter.GetDeclarationAirResistanceCurve(vehicleCategory, crossSectionArea.SI<SquareMeter>()),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			var tmp = crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond());
			Assert.AreEqual(expected, tmp.Value(), Tolerance);
		}

		[
			TestCase(VehicleCategory.Tractor, 6.46, -0.1),
			TestCase(VehicleCategory.Tractor, 6.46, 130.1),
		]
		public void CrossWindCorrectionExceptionTest(VehicleCategory vehicleCategory, double crossSectionArea, double kmph)
		{
			var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				DeclarationDataAdapter.GetDeclarationAirResistanceCurve(vehicleCategory, crossSectionArea.SI<SquareMeter>()),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			AssertHelper.Exception<VectoException>(() =>
				crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond()));
		}

		[Test,
		// fixed points
		TestCase(150, 1.000, 1.000, 0.00),
		TestCase(150, 1.100, 1.000, -40.34),
		TestCase(150, 1.222, 1.000, -80.34),
		TestCase(150, 1.375, 1.000, -136.11),
		TestCase(150, 1.571, 1.000, -216.52),
		TestCase(150, 1.833, 1.000, -335.19),
		TestCase(150, 2.200, 1.000, -528.77),
		TestCase(150, 2.750, 1.000, -883.40),
		TestCase(150, 4.400, 1.000, -2462.17),
		TestCase(150, 11.000, 1.000, -16540.98),
		// interpolated
		TestCase(150, 1.0025, 1.0, 0.0),
		TestCase(150, 1.0525, 1.0, -20.17),
		TestCase(150, 1.161, 1.0, -60.34),
		TestCase(150, 1.2985, 1.0, -108.225),
		TestCase(150, 1.473, 1.0, -176.315),
		TestCase(150, 1.702, 1.0, -275.855),
		TestCase(150, 2.0165, 1.0, -431.98),
		TestCase(150, 2.475, 1.0, -706.085),
		TestCase(150, 3.575, 1.0, -1672.785),
		TestCase(150, 7.7, 1.0, -9501.575),
		// extrapolated
		TestCase(150, 0.5, 1.0, 0.0),
		TestCase(150, 12.0, 1.0, -18674.133), // = (12-4.4)*(-16540.98- -2462.17)/(11-4.4)+ -2462.17
		]
		public void DefaultTCTest(double referenceRpm, double nu, double mu, double torque)
		{
			var referenceSpeed = referenceRpm.SI<PerSecond>();

			var r = new Random();

			var muLookup = DeclarationData.TorqueConverter.LookupMu(nu);
			Assert.AreEqual(muLookup, mu);

			var angularSpeed = r.Next(1000).SI<PerSecond>();
			var torqueLookup = DeclarationData.TorqueConverter.LookupTorque(nu, angularSpeed, referenceSpeed);
			AssertHelper.AreRelativeEqual(
				torque.SI<NewtonMeter>() * Math.Pow((angularSpeed / referenceSpeed).Cast<Scalar>(), 2), torqueLookup);
		}

		[Test]
		public void AuxElectricSystemTest()
		{
			var es = DeclarationData.ElectricSystem;

			var expected = new[] {
				new { Mission = MissionType.LongHaul, Base = 1240.SI<Watt>(), LED = 1190.SI<Watt>(), Efficiency = 0.7 },
				new {
					Mission = MissionType.RegionalDelivery,
					Base = 1055.SI<Watt>(),
					LED = 1005.SI<Watt>(),
					Efficiency = 0.7
				},
				new {
					Mission = MissionType.UrbanDelivery,
					Base = 974.SI<Watt>(),
					LED = 924.SI<Watt>(),
					Efficiency = 0.7
				},
				new {
					Mission = MissionType.MunicipalUtility,
					Base = 974.SI<Watt>(),
					LED = 924.SI<Watt>(),
					Efficiency = 0.7
				},
				new {
					Mission = MissionType.Construction,
					Base = 975.SI<Watt>(),
					LED = 925.SI<Watt>(),
					Efficiency = 0.7
				},
				new { Mission = MissionType.HeavyUrban, Base = 0.SI<Watt>(), LED = 0.SI<Watt>(), Efficiency = 1.0 },
				new { Mission = MissionType.Urban, Base = 0.SI<Watt>(), LED = 0.SI<Watt>(), Efficiency = 1.0 },
				new { Mission = MissionType.Suburban, Base = 0.SI<Watt>(), LED = 0.SI<Watt>(), Efficiency = 1.0 },
				new { Mission = MissionType.Interurban, Base = 0.SI<Watt>(), LED = 0.SI<Watt>(), Efficiency = 1.0 },
				new { Mission = MissionType.Coach, Base = 0.SI<Watt>(), LED = 0.SI<Watt>(), Efficiency = 1.0 }
			};
			Assert.AreEqual(expected.Length, Enum.GetValues(typeof(MissionType)).Length);

			foreach (var expectation in expected) {
				var baseConsumption = es.Lookup(expectation.Mission, null);
				var leds = es.Lookup(expectation.Mission, new[] { "LED lights" });

				AssertHelper.AreRelativeEqual(expectation.Base / expectation.Efficiency, baseConsumption);
				AssertHelper.AreRelativeEqual(expectation.LED / expectation.Efficiency, leds);
			}
		}

		[Test,
		TestCase("Crankshaft mounted - Electronically controlled visco clutch (Default)",
			new[] { 618, 671, 516, 566, 1037, 0, 0, 0, 0, 0 }),
		TestCase("Crankshaft mounted - Bimetallic controlled visco clutch", new[] { 818, 871, 676, 766, 1277, 0, 0, 0, 0, 0 }),
		TestCase("Crankshaft mounted - Discrete step clutch", new[] { 668, 721, 616, 616, 1157, 0, 0, 0, 0, 0 }),
		TestCase("Crankshaft mounted - On/Off clutch", new[] { 718, 771, 666, 666, 1237, 0, 0, 0, 0, 0 }),
		TestCase("Belt driven or driven via transm. - Electronically controlled visco clutch",
			new[] { 889, 944, 733, 833, 1378, 0, 0, 0, 0, 0 }),
		TestCase("Belt driven or driven via transm. - Bimetallic controlled visco clutch",
			new[] { 1089, 1144, 893, 1033, 1618, 0, 0, 0, 0, 0 }),
		TestCase("Belt driven or driven via transm. - Discrete step clutch", new[] { 939, 994, 883, 883, 1498, 0, 0, 0, 0, 0 }
			),
		TestCase("Belt driven or driven via transm. - On/Off clutch", new[] { 989, 1044, 933, 933, 1578, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic driven - Variable displacement pump", new[] { 738, 955, 632, 717, 1672, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic driven - Constant displacement pump", new[] { 1000, 1200, 800, 900, 2100, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic driven - Electronically controlled", new[] { 700, 800, 600, 600, 1400, 0, 0, 0, 0, 0 }),
		]
		public void AuxFanTechTest(string technology, int[] expected)
		{
			var fan = DeclarationData.Fan;

			var defaultExpected = new[] { 618, 671, 516, 566, 1037, 0, 0, 0, 0, 0 };

			for (var i = 0; i < Missions.Length; i++) {
				// default tech
				var defaultValue = fan.Lookup(Missions[i], "");
				Assert.AreEqual(defaultExpected[i], defaultValue.Value(), Tolerance);

				// all fan techs
				foreach (var expect in expected) {
					var value = fan.Lookup(Missions[i], technology);
					Assert.AreEqual(expected[i], value.Value(), Tolerance);
				}
			}
		}

		[Test]
		public void AuxHeatingVentilationAirConditionTest()
		{
			var hvac = DeclarationData.HeatingVentilationAirConditioning;

			var expected = new Dictionary<VehicleClass, int[]> {
				{ VehicleClass.Class1, new[] { 0, 150, 150, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class2, new[] { 200, 200, 150, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class3, new[] { 0, 200, 150, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class4, new[] { 350, 200, 0, 300, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class5, new[] { 350, 200, 0, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class6, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class7, new[] { 0, 0, 0, 0, 200, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class8, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class9, new[] { 350, 200, 0, 300, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class10, new[] { 350, 200, 0, 0, 0, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class11, new[] { 0, 0, 0, 0, 200, 0, 0, 0, 0, 0 } },
				{ VehicleClass.Class12, new[] { 0, 0, 0, 0, 200, 0, 0, 0, 0, 0 } }
			};

			for (var i = 0; i < Missions.Length; i++) {
				foreach (var expect in expected) {
					var value = hvac.Lookup(Missions[i], expect.Key);
					Assert.AreEqual(expect.Value[i], value.Value(), Tolerance);
				}
			}
		}

		[Test,
		TestCase(VehicleClass.Class1, new[] { 0, 1300, 1240, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class2, new[] { 1180, 1280, 1320, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class3, new[] { 0, 1360, 1380, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class4, new[] { 1300, 1340, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class5, new[] { 1340, 1820, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class6, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class7, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class8, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class9, new[] { 1340, 1540, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class10, new[] { 1340, 1820, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class11, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class12, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		]
		public void AuxPneumaticSystemTest(VehicleClass vehicleClass, int[] expected)
		{
			var ps = DeclarationData.PneumaticSystem;

			for (var i = 0; i < Missions.Length; i++) {
				var value = ps.Lookup(Missions[i], vehicleClass);
				Assert.AreEqual(expected[i], value.Value(), Tolerance);
			}
		}

		[Test,
		TestCase("Fixed displacement", VehicleClass.Class1, new[] { 0, 260, 270, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class2, new[] { 370, 320, 310, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class3, new[] { 0, 340, 350, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class4, new[] { 610, 530, 0, 530, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class5, new[] { 720, 630, 620, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class6, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class7, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class8, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class9, new[] { 720, 550, 0, 550, 0, 0, 0, 0, 0, 0 }),
		TestCase("Fixed displacement", VehicleClass.Class10, new[] { 570, 530, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class1, new[] { 0, 156, 162, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class2, new[] { 222, 192, 186, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class3, new[] { 0, 204, 210, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class4, new[] { 366, 318, 0, 318, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class5, new[] { 432, 378, 372, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class6, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class7, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class8, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class9, new[] { 432, 330, 0, 330, 0, 0, 0, 0, 0, 0 }),
		TestCase("Variable displacement", VehicleClass.Class10, new[] { 342, 318, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class1, new[] { 0, 225, 235, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class2, new[] { 322, 278, 269, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class3, new[] { 0, 295, 304, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class4, new[] { 531, 460, 0, 460, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class5, new[] { 627, 546, 540, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class6, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class7, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class8, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class9, new[] { 627, 478, 0, 478, 0, 0, 0, 0, 0, 0 }),
		TestCase("Hydraulic supported by electric", VehicleClass.Class10, new[] { 498, 461, 0, 0, 0, 0, 0, 0, 0, 0 }),
		]
		public void AuxSteeringPumpTest(string technology, VehicleClass hdvClass, int[] expected)
		{
			var sp = DeclarationData.SteeringPump;

			for (var i = 0; i < Missions.Length; i++) {
				var value = sp.Lookup(Missions[i], hdvClass, technology);
				Assert.AreEqual(expected[i], value.Value(), Tolerance);
			}
		}

		[
			TestCase(0),
			TestCase(1000),
			TestCase(3500),
			TestCase(7499)
		]
		public void SegmentWeightOutOfRange4X2(double weight)
		{
			AssertHelper.Exception<VectoException>(() =>
				DeclarationData.Segments.Lookup(
					VehicleCategory.RigidTruck,
					AxleConfiguration.AxleConfig_4x2,
					weight.SI<Kilogram>(),
					0.SI<Kilogram>()),
				"Gross vehicle mass must be greater than 7.5 tons");
		}

		[
			TestCase(0),
			TestCase(1000),
			TestCase(3500),
			TestCase(7499)
		]
		public void SegmentWeightOutOfRange4X4(double weight)
		{
			AssertHelper.Exception<VectoException>(() =>
				DeclarationData.Segments.Lookup(
					VehicleCategory.RigidTruck,
					AxleConfiguration.AxleConfig_4x4,
					weight.SI<Kilogram>(),
					0.SI<Kilogram>()),
				"Gross vehicle mass must be greater than 7.5 tons");
		}

		[Test,
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, VehicleClass.Class1),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, VehicleClass.Class1),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, VehicleClass.Class1),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, VehicleClass.Class1),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10001, 0, VehicleClass.Class2),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10001, 0, VehicleClass.Class2),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, VehicleClass.Class2),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, VehicleClass.Class2),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12001, 0, VehicleClass.Class3),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12001, 0, VehicleClass.Class3),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, VehicleClass.Class3),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, VehicleClass.Class3),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, VehicleClass.Class4),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 99000, 0, VehicleClass.Class4),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, VehicleClass.Class5),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 99000, 0, VehicleClass.Class5),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7500, 0, VehicleClass.Class9),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, VehicleClass.Class9),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, VehicleClass.Class9),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 99000, 0, VehicleClass.Class9),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, VehicleClass.Class10),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, VehicleClass.Class10),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, VehicleClass.Class10),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 99000, 0, VehicleClass.Class10),
		]
		public void SegmentLookupTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
			double curbWeight, VehicleClass expectedClass)
		{
			var segment = DeclarationData.Segments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
				curbWeight.SI<Kilogram>());
			Assert.AreEqual(expectedClass, segment.VehicleClass);
		}

		[Test]
		public void SegmentTest()
		{
			//mock vehicleData
			var vehicleData = new {
				VehicleCategory = VehicleCategory.RigidTruck,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 11900.SI<Kilogram>(),
				CurbWeight = 5850.SI<Kilogram>()
			};

			var segment = DeclarationData.Segments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight);

			Assert.AreEqual(VehicleClass.Class2, segment.VehicleClass);

			var data = AccelerationCurveData.ReadFromStream(segment.AccelerationFile);
			TestAcceleration(data);

			Assert.AreEqual(3, segment.Missions.Length);

			var longHaulMission = segment.Missions[0];
			Assert.AreEqual(MissionType.LongHaul, longHaulMission.MissionType);

			Assert.AreEqual("RigidSolo", longHaulMission.CrossWindCorrection);

			Assert.IsTrue(new[] { 0.4, 0.6 }.SequenceEqual(longHaulMission.AxleWeightDistribution));
			Assert.IsTrue(new double[] { }.SequenceEqual(longHaulMission.TrailerAxleWeightDistribution));
			Assert.AreEqual(1900.SI<Kilogram>(), longHaulMission.BodyCurbWeight);

			Assert.IsNotNull(longHaulMission.CycleFile);
			Assert.IsTrue(!string.IsNullOrEmpty(new StreamReader(longHaulMission.CycleFile).ReadLine()));

			Assert.AreEqual(0.SI<Kilogram>(), longHaulMission.MinLoad);
			Assert.AreEqual(4150, longHaulMission.RefLoad.Value());
			Assert.AreEqual(vehicleData.GrossVehicleMassRating - longHaulMission.BodyCurbWeight - vehicleData.CurbWeight,
				longHaulMission.MaxLoad);

			var regionalDeliveryMission = segment.Missions[1];
			Assert.AreEqual(MissionType.RegionalDelivery, regionalDeliveryMission.MissionType);

			Assert.AreEqual("RigidSolo", regionalDeliveryMission.CrossWindCorrection);

			Assert.IsTrue(new[] { 0.45, 0.55 }.SequenceEqual(regionalDeliveryMission.AxleWeightDistribution));
			Assert.IsTrue(new double[] { }.SequenceEqual(regionalDeliveryMission.TrailerAxleWeightDistribution));
			Assert.AreEqual(1900.SI<Kilogram>(), regionalDeliveryMission.BodyCurbWeight);

			Assert.IsNotNull(regionalDeliveryMission.CycleFile);
			Assert.IsTrue(!string.IsNullOrEmpty(new StreamReader(regionalDeliveryMission.CycleFile).ReadLine()));

			Assert.AreEqual(0.SI<Kilogram>(), regionalDeliveryMission.MinLoad);
			Assert.AreEqual(0.3941 * vehicleData.GrossVehicleMassRating - 1705.9.SI<Kilogram>(),
				regionalDeliveryMission.RefLoad);
			Assert.AreEqual(
				vehicleData.GrossVehicleMassRating - regionalDeliveryMission.BodyCurbWeight - vehicleData.CurbWeight,
				regionalDeliveryMission.MaxLoad);

			var urbanDeliveryMission = segment.Missions[2];
			Assert.AreEqual(MissionType.UrbanDelivery, urbanDeliveryMission.MissionType);

			Assert.AreEqual("RigidSolo", urbanDeliveryMission.CrossWindCorrection);

			Assert.IsTrue(new[] { 0.45, 0.55 }.SequenceEqual(urbanDeliveryMission.AxleWeightDistribution));
			Assert.IsTrue(new double[] { }.SequenceEqual(urbanDeliveryMission.TrailerAxleWeightDistribution));
			Assert.AreEqual(1900.SI<Kilogram>(), urbanDeliveryMission.BodyCurbWeight);

			Assert.IsNotNull(urbanDeliveryMission.CycleFile);
			Assert.IsTrue(!string.IsNullOrEmpty(new StreamReader(urbanDeliveryMission.CycleFile).ReadLine()));

			Assert.AreEqual(0.SI<Kilogram>(), urbanDeliveryMission.MinLoad);
			Assert.AreEqual(0.3941 * vehicleData.GrossVehicleMassRating - 1705.9.SI<Kilogram>(),
				urbanDeliveryMission.RefLoad);
			Assert.AreEqual(
				vehicleData.GrossVehicleMassRating - urbanDeliveryMission.BodyCurbWeight - vehicleData.CurbWeight,
				urbanDeliveryMission.MaxLoad);
		}

		public void EqualAcceleration(AccelerationCurveData data, double velocity, double acceleration,
			double deceleration)
		{
			var entry = data.Lookup(velocity.KMPHtoMeterPerSecond());
			Assert.AreEqual(entry.Acceleration.Value(), acceleration, Tolerance);
			Assert.AreEqual(entry.Deceleration.Value(), deceleration, Tolerance);
		}

		public void TestAcceleration(AccelerationCurveData data)
		{
			// FIXED POINTS
			EqualAcceleration(data, 0, 1, -1);
			EqualAcceleration(data, 25, 1, -1);
			EqualAcceleration(data, 50, 0.642857143, -1);
			EqualAcceleration(data, 60, 0.5, -0.5);
			EqualAcceleration(data, 120, 0.5, -0.5);

			// INTERPOLATED POINTS
			EqualAcceleration(data, 20, 1, -1);
			EqualAcceleration(data, 40, 0.785714286, -1);
			EqualAcceleration(data, 55, 0.571428572, -0.75);
			EqualAcceleration(data, 80, 0.5, -0.5);
			EqualAcceleration(data, 100, 0.5, -0.5);

			// EXTRAPOLATE 
			EqualAcceleration(data, -20, 1, -1);
			EqualAcceleration(data, 140, 0.5, -0.5);
		}

		[Test]
		public void Declaration_Class2_WeightTest()
		{
			Assert.Fail();
		}
	}
}