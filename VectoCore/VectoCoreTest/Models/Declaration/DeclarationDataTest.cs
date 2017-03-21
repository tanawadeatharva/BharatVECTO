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

using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationDataTest
	{
		private const double Tolerance = 0.0001;

		private readonly MissionType[] _missions = {
			MissionType.LongHaul,
			MissionType.RegionalDelivery,
			MissionType.UrbanDelivery,
			MissionType.MunicipalUtility,
			MissionType.Construction,
		};

		[TestCase("285/60 R22.5", 10.6, 0.914, 3.03, 0.440766),
		TestCase("285/70 R19.5", 7.9, 0.895, 3.05, 0.434453),
		TestCase("395/85 R20", 27.9, 1.18, 3.05, 0.572798)]
		public void WheelDataTest(string wheels, double inertia, double wheelsDiameter, double circumferenceFactor,
			double expectedDynamicRadius)
		{
			var tmp = DeclarationData.Wheels.Lookup(wheels);

			AssertHelper.AreRelativeEqual(inertia, tmp.Inertia);
			AssertHelper.AreRelativeEqual(wheelsDiameter, tmp.WheelsDiameter);
			AssertHelper.AreRelativeEqual(circumferenceFactor, tmp.CircumferenceFactor);
			Assert.AreEqual(expectedDynamicRadius, tmp.DynamicTyreRadius.Value(), 1e-6);
		}

		[
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
		]
		public void PT1Test(double rpm, double expectedPt1)
		{
			var pt1 = DeclarationData.PT1.Lookup(rpm.RPMtoRad());
			Assert.AreEqual(expectedPt1, pt1.Value.Value(), Tolerance);
			Assert.IsFalse(pt1.Extrapolated);
		}

		[TestCase(200),
		TestCase(0),
		TestCase(3000),]
		public void PT1ExceptionsTest(double rpm)
		{
			// EXTRAPOLATE 
			var tmp = DeclarationData.PT1.Lookup(rpm.RPMtoRad());
			Assert.IsTrue(tmp.Extrapolated);
		}

		[TestCase]
		public void WHTCTest()
		{
			var whtc = DeclarationData.WHTCCorrection;

			var factors = new {
				urban = new[] { 0.11, 0.17, 0.69, 0.98, 0.62, 1.0, 1.0, 1.0, 0.45, 0.0 },
				rural = new[] { 0.0, 0.3, 0.27, 0.0, 0.32, 0.0, 0.0, 0.0, 0.36, 0.22 },
				motorway = new[] { 0.89, 0.53, 0.04, 0.02, 0.06, 0.0, 0.0, 0.0, 0.19, 0.78 }
			};

			var r = new Random();
			for (var i = 0; i < _missions.Length; i++) {
				var urban = r.NextDouble() * 2;
				var rural = r.NextDouble() * 2;
				var motorway = r.NextDouble() * 2;
				var whtcValue = whtc.Lookup(_missions[i], rural: rural, urban: urban, motorway: motorway);
				Assert.AreEqual(urban * factors.urban[i] + rural * factors.rural[i] + motorway * factors.motorway[i],
					whtcValue);
			}
		}

		[TestCase]
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

		[TestCase]
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

		[TestCase("RigidSolo", 0.013526, 0.017746, -0.000666),
		TestCase("RigidTrailer", 0.017125, 0.072275, -0.004148),
		TestCase("TractorSemitrailer", 0.030042, 0.040817, -0.00213),
		TestCase("CoachBus", -0.000794, 0.02109, -0.00109)]
		public void AirDrag_WithStringKey(string key, double a1, double a2, double a3)
		{
			var value = DeclarationData.AirDrag.Lookup(key);
			Assert.AreEqual(a1, value.A1);
			Assert.AreEqual(a2, value.A2);
			Assert.AreEqual(a3, value.A3);
		}

		[TestCase("RigidSolo", 0.013526, 0.017746, -0.000666),
		TestCase("TractorSemitrailer", 0.030042, 0.040817, -0.00213),
		TestCase("RigidTrailer", 0.017125, 0.072275, -0.004148),
		TestCase("CoachBus", -0.000794, 0.02109, -0.00109)]
		public void AirDrag_WithVehicleCategory(string parameterSet, double a1, double a2, double a3)
		{
			var value = DeclarationData.AirDrag.Lookup(parameterSet);
			Assert.AreEqual(a1, value.A1);
			Assert.AreEqual(a2, value.A2);
			Assert.AreEqual(a3, value.A3);
		}

		[TestCase("TractorSemitrailer", 6.46, 0, 8.05913),
		TestCase("TractorSemitrailer", 6.46, 60, 8.05913),
		TestCase("TractorSemitrailer", 6.46, 75, 7.639436),
		TestCase("TractorSemitrailer", 6.46, 100, 7.22305),
		TestCase("TractorSemitrailer", 6.46, 52.1234, 8.059126),
		TestCase("TractorSemitrailer", 6.46, 73.5432, 7.67487),
		TestCase("TractorSemitrailer", 6.46, 92.8765, 7.317215),
		TestCase("TractorSemitrailer", 6.46, 100.449, 7.217975),
		TestCase("TractorSemitrailer", 6.46, 103, 7.18915),
		TestCase("TractorSemitrailer", 6.46, 105, 7.166555),
		TestCase("TractorSemitrailer", 6.46, 115, 7.071136),
		TestCase("TractorSemitrailer", 6.46, 130, 6.961237),]
		public void CrossWindCorrectionTest(string parameterSet, double crossSectionArea, double kmph,
			double expected)
		{
			var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(),
				DeclarationDataAdapter.GetDeclarationAirResistanceCurve(parameterSet, crossSectionArea.SI<SquareMeter>()),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			var tmp = crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond());
			Assert.AreEqual(expected, tmp.Value(), Tolerance);
		}

		[TestCase("TractorSemitrailer", 6.46, -0.1),
		TestCase("TractorSemitrailer", 6.46, 130.1),]
		public void CrossWindCorrectionExceptionTest(string parameterSet, double crossSectionArea, double kmph)
		{
			var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(),
				DeclarationDataAdapter.GetDeclarationAirResistanceCurve(parameterSet, crossSectionArea.SI<SquareMeter>()),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			AssertHelper.Exception<VectoException>(() =>
				crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond()));
		}

		[TestCase(MissionType.LongHaul, "Standard technology", 1200, 0.7),
		TestCase(MissionType.RegionalDelivery, "Standard technology", 1000, 0.7),
		TestCase(MissionType.UrbanDelivery, "Standard technology", 1000, 0.7),
		TestCase(MissionType.MunicipalUtility, "Standard technology", 1000, 0.7),
		TestCase(MissionType.Construction, "Standard technology", 1000, 0.7),
		TestCase(MissionType.LongHaul, "Standard technology - LED headlights, all", 1150, 0.7),
		TestCase(MissionType.RegionalDelivery, "Standard technology - LED headlights, all", 950, 0.7),
		TestCase(MissionType.UrbanDelivery, "Standard technology - LED headlights, all", 950, 0.7),
		TestCase(MissionType.MunicipalUtility, "Standard technology - LED headlights, all", 950, 0.7),
		TestCase(MissionType.Construction, "Standard technology - LED headlights, all", 950, 0.7),]
		public void AuxElectricSystemTest(MissionType mission, string technology, double value, double efficiency)
		{
			AssertHelper.AreRelativeEqual(value / efficiency, DeclarationData.ElectricSystem.Lookup(mission, technology));
		}

		[TestCase(MissionType.Interurban, "Standard technology"),
		TestCase(MissionType.LongHaul, "Standard technology - Flux-Compensator")]
		public void AuxElectricSystem_NotExistingError(MissionType mission, string technology)
		{
			AssertHelper.Exception<VectoException>(() => { DeclarationData.ElectricSystem.Lookup(mission, technology); });
		}

		[TestCase("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", 50),
		TestCase("only the drive shaft of the PTO - multi-disc clutch", 1000),
		TestCase("only the drive shaft of the PTO - multi-disc clutch, oil pump", 2000),
		TestCase("drive shaft and/or up to 2 gear wheels - shift claw, synchronizer, sliding gearwheel", 300),
		TestCase("drive shaft and/or up to 2 gear wheels - multi-disc clutch", 1500),
		TestCase("drive shaft and/or up to 2 gear wheels - multi-disc clutch, oil pump", 3000),
		TestCase("drive shaft and/or more than 2 gear wheels - shift claw, synchronizer, sliding gearwheel", 600),
		TestCase("drive shaft and/or more than 2 gear wheels - multi-disc clutch", 2000),
		TestCase("drive shaft and/or more than 2 gear wheels - multi-disc clutch, oil pump", 4000),]
		public void AuxPTOTransmissionTest(string technology, double value)
		{
			AssertHelper.AreRelativeEqual(value, DeclarationData.PTOTransmission.Lookup(technology));
		}

		[TestCase("Superfluid")]
		public void AuxPTOTransmission_NotExistingError(string technology)
		{
			AssertHelper.Exception<VectoException>(() => { DeclarationData.PTOTransmission.Lookup(technology); });
		}

		[TestCase("", new[] { 618, 671, 516, 566, 1037 }),
		TestCase("Crankshaft mounted - Electronically controlled visco clutch", new[] { 618, 671, 516, 566, 1037 }),
		TestCase("Crankshaft mounted - Bimetallic controlled visco clutch", new[] { 818, 871, 676, 766, 1277 }),
		TestCase("Crankshaft mounted - Discrete step clutch", new[] { 668, 721, 616, 616, 1157 }),
		TestCase("Crankshaft mounted - On/off clutch", new[] { 718, 771, 666, 666, 1237 }),
		TestCase("Belt driven or driven via transm. - Electronically controlled visco clutch",
			new[] { 989, 1044, 833, 933, 1478 }),
		TestCase("Belt driven or driven via transm. - Bimetallic controlled visco clutch",
			new[] { 1189, 1244, 993, 1133, 1718 }),
		TestCase("Belt driven or driven via transm. - Discrete step clutch", new[] { 1039, 1094, 983, 983, 1598 }),
		TestCase("Belt driven or driven via transm. - On/off clutch", new[] { 1089, 1144, 1033, 1033, 1678 }),
		TestCase("Hydraulic driven - Variable displacement pump", new[] { 938, 1155, 832, 917, 1872 }),
		TestCase("Hydraulic driven - Constant displacement pump", new[] { 1200, 1400, 1000, 1100, 2300 }),
		TestCase("Hydraulic driven - Electronically controlled", new[] { 700, 800, 600, 600, 1400 }),]
		public void AuxFanTechTest(string technology, int[] expected)
		{
			for (var i = 0; i < _missions.Length; i++) {
				var value = DeclarationData.Fan.Lookup(_missions[i], technology);
				Assert.AreEqual(expected[i], value.Value(), Tolerance);
			}
		}

		[TestCase("Superfluid Hydraulic", MissionType.LongHaul, TestName = "AuxFanTechError( wrong tech )"),
		TestCase("Hydraulic driven - Electronically controlled", MissionType.Coach,
			TestName = "AuxFanTechError( wrong mission )")
		]
		public void AuxFanTechError(string technology, MissionType missionType)
		{
			AssertHelper.Exception<VectoException>(() => DeclarationData.Fan.Lookup(missionType, technology));
		}

		[TestCase(VehicleClass.Class1, new[] { 0, 150, 150, 0, 0 }),
		TestCase(VehicleClass.Class2, new[] { 200, 200, 150, 0, 0 }),
		TestCase(VehicleClass.Class3, new[] { 0, 200, 150, 0, 0 }),
		TestCase(VehicleClass.Class4, new[] { 350, 200, 0, 300, 0 }),
		TestCase(VehicleClass.Class5, new[] { 350, 200, 0, 0, 0 }),
		TestCase(VehicleClass.Class9, new[] { 350, 200, 0, 300, 0 }),
		TestCase(VehicleClass.Class10, new[] { 350, 200, 0, 0, 0 }),
		TestCase(VehicleClass.Class11, new[] { 350, 200, 0, 300, 200 }),
		TestCase(VehicleClass.Class12, new[] { 350, 200, 0, 0, 200 }),
		TestCase(VehicleClass.Class16, new[] { 0, 0, 0, 0, 200 })]
		public void AuxHeatingVentilationAirConditionTest(VehicleClass vehicleClass, int[] expected)
		{
			for (var i = 0; i < expected.Length; i++) {
				if (expected[i] > 0) {
					AssertHelper.AreRelativeEqual(expected[i],
						DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i], vehicleClass));
				} else {
					var i1 = i;
					AssertHelper.Exception<VectoException>(
						() => DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i1], vehicleClass));
				}
			}
		}

		[Test,
		TestCase("Small", new[] { 1400, 1300, 1200, 1200, 1300 }),
		TestCase("Small + ESS", new[] { 900, 800, 800, 800, 800 }),
		TestCase("Small + visco clutch", new[] { 800, 700, 700, 700, 700 }),
		TestCase("Small + mech. clutch", new[] { 600, 600, 650, 650, 600 }),
		TestCase("Small + ESS + AMS", new[] { 500, 400, 500, 500, 400 }),
		TestCase("Small + visco clutch + AMS", new[] { 400, 300, 400, 400, 300 }),
		TestCase("Small + mech. clutch + AMS", new[] { 200, 200, 350, 350, 200 }),
		TestCase("Medium Supply 1-stage", new[] { 1600, 1400, 1350, 1350, 1500 }),
		TestCase("Medium Supply 1-stage + ESS", new[] { 1000, 900, 900, 900, 900 }),
		TestCase("Medium Supply 1-stage + visco clutch", new[] { 850, 800, 800, 800, 750 }),
		TestCase("Medium Supply 1-stage + mech. clutch", new[] { 600, 550, 550, 550, 600 }),
		TestCase("Medium Supply 1-stage + ESS + AMS", new[] { 600, 700, 700, 700, 500 }),
		TestCase("Medium Supply 1-stage + visco clutch + AMS", new[] { 450, 600, 600, 600, 350 }),
		TestCase("Medium Supply 1-stage + mech. clutch + AMS", new[] { 200, 350, 350, 350, 200 }),
		TestCase("Medium Supply 2-stage", new[] { 2100, 1750, 1700, 1700, 2100 }),
		TestCase("Medium Supply 2-stage + ESS", new[] { 1100, 1050, 1000, 1000, 1000 }),
		TestCase("Medium Supply 2-stage + visco clutch", new[] { 1000, 850, 800, 800, 900 }),
		TestCase("Medium Supply 2-stage + mech. clutch", new[] { 700, 650, 600, 600, 800 }),
		TestCase("Medium Supply 2-stage + ESS + AMS", new[] { 700, 850, 800, 800, 500 }),
		TestCase("Medium Supply 2-stage + visco clutch + AMS", new[] { 600, 650, 600, 600, 400 }),
		TestCase("Medium Supply 2-stage + mech. clutch + AMS", new[] { 300, 450, 400, 400, 300 }),
		TestCase("Large Supply", new[] { 4300, 3600, 3500, 3500, 4100 }),
		TestCase("Large Supply + ESS", new[] { 1600, 1300, 1200, 1200, 1500 }),
		TestCase("Large Supply + visco clutch", new[] { 1300, 1100, 1000, 1000, 1200 }),
		TestCase("Large Supply + mech. clutch", new[] { 800, 800, 700, 700, 900 }),
		TestCase("Large Supply + ESS + AMS", new[] { 1100, 1000, 1000, 1000, 1000 }),
		TestCase("Large Supply + visco clutch + AMS", new[] { 800, 800, 800, 800, 700 }),
		TestCase("Large Supply + mech. clutch + AMS", new[] { 300, 500, 500, 500, 400 }),
		]
		public void AuxPneumaticSystemTest(string technology, int[] expected)
		{
			for (var i = 0; i < _missions.Length; i++) {
				var value = DeclarationData.PneumaticSystem.Lookup(_missions[i], technology);
				AssertHelper.AreRelativeEqual(expected[i], value);
			}
		}

		[
			TestCase(MissionType.LongHaul, VehicleClass.Class2, 370, "Fixed displacement", null, null, null),
			TestCase(MissionType.LongHaul, VehicleClass.Class4, 610, "Fixed displacement", null, null, null),
			TestCase(MissionType.LongHaul, VehicleClass.Class5, 720, "Fixed displacement", null, null, null),
			TestCase(MissionType.LongHaul, VehicleClass.Class9, 720, "Fixed displacement", null, null, null),
			TestCase(MissionType.LongHaul, VehicleClass.Class10, 570, "Fixed displacement", null, null, null),
			TestCase(MissionType.LongHaul, VehicleClass.Class11, 720, "Fixed displacement", null, null, null),
			TestCase(MissionType.LongHaul, VehicleClass.Class12, 570, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class1, 280, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 340, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class3, 370, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class4, 570, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class5, 670, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class9, 590, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class10, 570, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class11, 590, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class12, 570, "Fixed displacement", null, null, null),
			TestCase(MissionType.UrbanDelivery, VehicleClass.Class1, 270, "Fixed displacement", null, null, null),
			TestCase(MissionType.UrbanDelivery, VehicleClass.Class2, 310, "Fixed displacement", null, null, null),
			TestCase(MissionType.UrbanDelivery, VehicleClass.Class3, 350, "Fixed displacement", null, null, null),
			TestCase(MissionType.UrbanDelivery, VehicleClass.Class5, 620, "Fixed displacement", null, null, null),
			TestCase(MissionType.MunicipalUtility, VehicleClass.Class4, 510, "Fixed displacement", null, null, null),
			TestCase(MissionType.MunicipalUtility, VehicleClass.Class9, 510, "Fixed displacement", null, null, null),
			TestCase(MissionType.MunicipalUtility, VehicleClass.Class11, 510, "Fixed displacement", null, null, null),
			TestCase(MissionType.Construction, VehicleClass.Class11, 770, "Fixed displacement", null, null, null),
			TestCase(MissionType.Construction, VehicleClass.Class12, 770, "Fixed displacement", null, null, null),
			TestCase(MissionType.Construction, VehicleClass.Class16, 770, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 325.5, "Fixed displacement with elec. control", null,
				null,
				null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 289, "Dual displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 255, "Variable displacement mech. controlled", null, null,
				null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 204, "Variable displacement elec. controlled", null, null,
				null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 92.8571, "Electric", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 665, "Fixed displacement", "Fixed displacement", null,
				null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 1295, "Fixed displacement", "Fixed displacement",
				"Fixed displacement", "Fixed displacement"),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 1021.5, "Dual displacement",
				"Variable displacement mech. controlled", "Fixed displacement with elec. control",
				"Variable displacement elec. controlled"),
		]
		public void Aux_SteeringPumpLookupValues(MissionType mission, VehicleClass hdvClass, double expected, string axle1,
			string axle2, string axle3, string axle4)
		{
			// mk remark: made the test call with 4 axle params, so that the test name is clear in the test explorer.
			AssertHelper.AreRelativeEqual(expected,
				DeclarationData.SteeringPump.Lookup(mission, hdvClass,
					new[] { axle1, axle2, axle3, axle4 }.TakeWhile(a => a != null).ToArray()));
		}

		[TestCase(MissionType.LongHaul, VehicleClass.Class1, "Dual displacement",
			TestName = "Aux_SteeringPumpLookupFail( No Value )"),
		TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, "Super displacement",
			TestName = "Aux_SteeringPumpLookupFail( Wrong Tech )"),
		TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, "Dual displacement", "Dual displacement",
			"Dual displacement", "Dual displacement", "Dual displacement", TestName = "Aux_SteeringPumpLookupFail( >4 Techs )"),
		TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, TestName = "Aux_SteeringPumpLookupFail( Null Techs )"),
		TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, new string[0],
			TestName = "Aux_SteeringPumpLookupFail( 0 Techs )"),
		]
		public void Aux_SteeringPumpLookupFail(MissionType mission, VehicleClass hdvClass, params string[] tech)
		{
			AssertHelper.Exception<VectoException>(() => DeclarationData.SteeringPump.Lookup(mission, hdvClass, tech));
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
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 7500, 0, VehicleClass.Class11),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 40000, 0, VehicleClass.Class11),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 7500, 0, VehicleClass.Class12),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 99000, 0, VehicleClass.Class12),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 7500, 0, VehicleClass.Class16),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 99000, 0, VehicleClass.Class16)
		]
		public void SegmentLookupTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
			double curbWeight, VehicleClass expectedClass)
		{
			var segment = DeclarationData.Segments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
				curbWeight.SI<Kilogram>());
			Assert.AreEqual(expectedClass, segment.VehicleClass);
		}

		[Test,
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, VehicleClass.Class1, 1600, null,
			TestName = "SegmentLookupBodyWeight Class1 Rigid"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, VehicleClass.Class1, 1600, null,
			TestName = "SegmentLookupBodyWeight Class1 Tractor"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, VehicleClass.Class2, 1900, 3400,
			TestName = "SegmentLookupBodyWeight Class2 Rigid"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, VehicleClass.Class2, 1900, 3400,
			TestName = "SegmentLookupBodyWeight Class2 Tractor"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, VehicleClass.Class3, 2000, null,
			TestName = "SegmentLookupBodyWeight Class3 Rigid"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, VehicleClass.Class3, 2000, null,
			TestName = "SegmentLookupBodyWeight Class3 Tractor"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, VehicleClass.Class4, 2100, 5400,
			TestName = "SegmentLookupBodyWeight Class4"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, VehicleClass.Class5, null, 7500,
			TestName = "SegmentLookupBodyWeight Class5"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, VehicleClass.Class9, 2200, 5400,
			TestName = "SegmentLookupBodyWeight Class9"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, VehicleClass.Class10, null, 7500,
			TestName = "SegmentLookupBodyWeight Class10"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 0, VehicleClass.Class11, 2200, 5400,
			TestName = "SegmentLookupBodyWeight Class11"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 0, VehicleClass.Class12, null, 7500,
			TestName = "SegmentLookupBodyWeight Class12"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 0, VehicleClass.Class16, null, null,
			TestName = "SegmentLookupBodyWeight Class16")]
		public void SegmentLookupBodyTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
			double curbWeight, VehicleClass expectedClass, int? expectedBodyWeight, int? expectedTrailerWeight)
		{
			var segment = DeclarationData.Segments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
				curbWeight.SI<Kilogram>());
			Assert.AreEqual(expectedClass, segment.VehicleClass);

			if (expectedBodyWeight.HasValue) {
				Assert.AreEqual(expectedBodyWeight, segment.Missions[0].BodyCurbWeight.Value());
			}
			if (expectedTrailerWeight.HasValue) {
				var trailerMission = segment.Missions.Where(m => m.TrailerType != TrailerType.None).ToList();
				if (trailerMission.Count > 0) {
					Assert.AreEqual(expectedTrailerWeight, trailerMission.First().TrailerCurbWeight.Value());
				}
			}
		}

		[Test,
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, VehicleClass.Class1,
			new[] { 36.5, 36.5 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, VehicleClass.Class1,
			new[] { 36.5, 36.5 }),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, VehicleClass.Class2,
			new[] { 85.0, 45.2, 45.2 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, VehicleClass.Class2,
			new[] { 85.0, 45.2, 45.2 }),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, VehicleClass.Class3,
			new[] { 47.7, 47.7 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, VehicleClass.Class3,
			new[] { 47.7, 47.7 }),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, VehicleClass.Class4,
			new[] { 98.9, 49.4, 49.4 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, VehicleClass.Class5,
			new[] { 91.0, 91.0 }),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, VehicleClass.Class9,
			new[] { 101.4, 51.9, 51.9 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, VehicleClass.Class10,
			new[] { 91.0, 91.0 }),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 40000, 0, VehicleClass.Class11,
			new[] { 101.4, 51.9, 51.9, 51.9 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 99000, 0, VehicleClass.Class12,
			new[] { 91.0, 91.0, 91.0 }),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 99000, 0, VehicleClass.Class16,
			new[] { 0.0 })
		]
		public void SegmentLookupCargoVolumeTest(VehicleCategory category, AxleConfiguration axleConfiguration,
			double grossWeight,
			double curbWeight, VehicleClass expectedClass, double[] expectedCargoVolume)
		{
			var segment = DeclarationData.Segments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
				curbWeight.SI<Kilogram>());
			Assert.AreEqual(expectedClass, segment.VehicleClass);
			Assert.AreEqual(expectedCargoVolume.Length, segment.Missions.Length);
			for (var i = 0; i < expectedCargoVolume.Length; i++) {
				Assert.AreEqual(expectedCargoVolume[i], segment.Missions[i].CargoVolume.Value());
			}
		}

		/// <summary>
		/// trailer in longhaul, always pc formula
		/// </summary>
		[TestCase]
		public void Segment2Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.RigidTruck,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 11900.SI<Kilogram>(),
				CurbWeight = 5850.SI<Kilogram>()
			};

			var segment = DeclarationData.Segments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight);

			Assert.AreEqual(VehicleClass.Class2, segment.VehicleClass);

			var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
			TestAcceleration(data);

			Assert.AreEqual(3, segment.Missions.Length);

			AssertMission(segment.Missions[0], vehicleData: vehicleData, missionType: MissionType.LongHaul,
				cosswindCorrection: "RigidTrailer", axleWeightDistribution: new[] { 0.225, 0.325 },
				trailerAxleWeightDistribution: new[] { 0.45 }, bodyCurbWeight: 1900, trailerCurbWeight: 3400,
				trailerType: TrailerType.T1, minLoad: 0, refLoad: 9813.2353, trailerGrossVehicleWeight: 10500, deltaCdA: 0.6,
				maxLoad: 11250);

			AssertMission(segment.Missions[1], vehicleData: vehicleData, missionType: MissionType.RegionalDelivery,
				cosswindCorrection: "RigidSolo", axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { }, bodyCurbWeight: 1900, trailerCurbWeight: 0,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 2984.1176, trailerGrossVehicleWeight: 0, deltaCdA: 0,
				maxLoad: 4150);

			AssertMission(segment.Missions[2], vehicleData: vehicleData, missionType: MissionType.UrbanDelivery,
				cosswindCorrection: "RigidSolo", axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { }, bodyCurbWeight: 1900, trailerCurbWeight: 0,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 2984.1176, trailerGrossVehicleWeight: 0, deltaCdA: 0,
				maxLoad: 4150);
		}

		/// <summary>
		/// normal pc formula, no trailer
		/// </summary>
		[TestCase]
		public void Segment3Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.RigidTruck,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 14000.SI<Kilogram>(),
				CurbWeight = 5850.SI<Kilogram>()
			};

			var segment = DeclarationData.Segments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight);

			Assert.AreEqual(VehicleClass.Class3, segment.VehicleClass);

			Assert.AreEqual(2, segment.Missions.Length);

			AssertMission(segment.Missions[0], vehicleData: vehicleData, missionType: MissionType.RegionalDelivery,
				cosswindCorrection: "RigidSolo", axleWeightDistribution: new[] { 0.4, 0.6 },
				trailerAxleWeightDistribution: new double[] { }, bodyCurbWeight: 2000, trailerCurbWeight: 0,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 3811.7647, trailerGrossVehicleWeight: 0, deltaCdA: 0,
				maxLoad: 6150);

			AssertMission(segment.Missions[1], vehicleData: vehicleData, missionType: MissionType.UrbanDelivery,
				cosswindCorrection: "RigidSolo", axleWeightDistribution: new[] { 0.4, 0.6 },
				trailerAxleWeightDistribution: new double[] { }, bodyCurbWeight: 2000, trailerCurbWeight: 0,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 3811.7647, trailerGrossVehicleWeight: 0, deltaCdA: 0,
				maxLoad: 6150);
		}

		/// <summary>
		/// fixed reference weight, trailer only in longhaul
		/// </summary>
		[TestCase]
		public void Segment4Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.RigidTruck,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 18000.SI<Kilogram>(),
				CurbWeight = 7500.SI<Kilogram>()
			};

			var segment = DeclarationData.Segments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight);

			Assert.AreEqual(VehicleClass.Class4, segment.VehicleClass);

			var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
			TestAcceleration(data);

			Assert.AreEqual(3, segment.Missions.Length);

			AssertMission(segment.Missions[0], vehicleData: vehicleData, missionType: MissionType.LongHaul,
				cosswindCorrection: "RigidTrailer", axleWeightDistribution: new[] { 0.2, 0.3 },
				trailerAxleWeightDistribution: new[] { 0.25, 0.25 }, bodyCurbWeight: 2100, trailerCurbWeight: 5400,
				trailerType: TrailerType.T2, minLoad: 0, refLoad: 14000, trailerGrossVehicleWeight: 18000, deltaCdA: 0.6,
				maxLoad: 21000);

			AssertMission(segment.Missions[1], vehicleData: vehicleData, missionType: MissionType.RegionalDelivery,
				cosswindCorrection: "RigidSolo", axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { }, bodyCurbWeight: 2100, trailerCurbWeight: 0,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 4400, trailerGrossVehicleWeight: 0, deltaCdA: 0, maxLoad: 8400);

			AssertMission(segment.Missions[2], vehicleData: vehicleData, missionType: MissionType.MunicipalUtility,
				cosswindCorrection: "RigidSolo", axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { }, bodyCurbWeight: 2100, trailerCurbWeight: 0,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 4400, trailerGrossVehicleWeight: 0, deltaCdA: 0, maxLoad: 8400);
		}

		/// <summary>
		/// Segment 5: fixed reference weight, trailer always used
		/// </summary>
		[TestCase]
		public void Segment5Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.Tractor,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 18000.SI<Kilogram>(),
				CurbWeight = 7500.SI<Kilogram>()
			};

			var segment = DeclarationData.Segments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight);

			Assert.AreEqual(VehicleClass.Class5, segment.VehicleClass);

			var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
			TestAcceleration(data);

			Assert.AreEqual(2, segment.Missions.Length);

			AssertMission(segment.Missions[0], vehicleData: vehicleData, missionType: MissionType.LongHaul,
				cosswindCorrection: "TractorSemitrailer", axleWeightDistribution: new[] { 0.2, 0.25 },
				trailerAxleWeightDistribution: (0.55 / 3).Repeat(3).ToArray(), bodyCurbWeight: 0, trailerCurbWeight: 7500,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 19300, trailerGrossVehicleWeight: 24000, deltaCdA: 0,
				maxLoad: 25000);

			AssertMission(segment.Missions[1], vehicleData: vehicleData, missionType: MissionType.RegionalDelivery,
				cosswindCorrection: "TractorSemitrailer", axleWeightDistribution: new[] { 0.25, 0.25 },
				trailerAxleWeightDistribution: (0.5 / 3).Repeat(3).ToArray(), bodyCurbWeight: 0, trailerCurbWeight: 7500,
				trailerType: TrailerType.None, minLoad: 0, refLoad: 12900, trailerGrossVehicleWeight: 24000, deltaCdA: 0,
				maxLoad: 25000);
		}

		public static void AssertMission(Mission m, dynamic vehicleData, MissionType missionType, string cosswindCorrection,
			double[] axleWeightDistribution, double[] trailerAxleWeightDistribution, double bodyCurbWeight,
			double trailerCurbWeight, TrailerType trailerType, double minLoad, double refLoad, double maxLoad,
			double trailerGrossVehicleWeight, double deltaCdA)
		{
			Assert.AreEqual(missionType, m.MissionType);
			Assert.AreEqual(cosswindCorrection, m.CrossWindCorrectionParameters);
			CollectionAssert.AreEqual(axleWeightDistribution, m.AxleWeightDistribution,
				"Axle distribution not equal.\nexpected: {0}\nactual: {1}", string.Join(",", axleWeightDistribution),
				string.Join(",", m.AxleWeightDistribution));
			CollectionAssert.AreEqual(trailerAxleWeightDistribution, m.TrailerAxleWeightDistribution,
				"Trailer axle distribution not equal.\nexpected: {0}\nactual: {1}", string.Join(",", trailerAxleWeightDistribution),
				string.Join(",", m.TrailerAxleWeightDistribution));
			Assert.AreEqual(bodyCurbWeight.SI<Kilogram>(), m.BodyCurbWeight);
			Assert.AreEqual(trailerCurbWeight.SI<Kilogram>(), m.TrailerCurbWeight);
			Assert.AreEqual(trailerType, m.TrailerType);

			Assert.IsNotNull(m.CycleFile);
			Assert.IsTrue(!string.IsNullOrEmpty(new StreamReader(m.CycleFile).ReadLine()));
			Assert.AreEqual(minLoad.SI<Kilogram>(), m.MinLoad);
			AssertHelper.AreRelativeEqual(refLoad, m.RefLoad);
			Assert.AreEqual(maxLoad.SI<Kilogram>(), m.MaxLoad);
			Assert.AreEqual(trailerGrossVehicleWeight.SI<Kilogram>(), m.TrailerGrossVehicleWeight);
			Assert.AreEqual(VectoMath.Min(vehicleData.GrossVehicleMassRating + m.TrailerGrossVehicleWeight, 40000.SI<Kilogram>())
							- m.BodyCurbWeight - m.TrailerCurbWeight - vehicleData.CurbWeight,
				m.MaxLoad);
			Assert.AreEqual(deltaCdA.SI<SquareMeter>(), m.DeltaCdA);
		}

		private static void EqualAcceleration(AccelerationCurveData data, double velocity, double acceleration,
			double deceleration)
		{
			var entry = data.Lookup(velocity.KMPHtoMeterPerSecond());
			Assert.AreEqual(entry.Acceleration.Value(), acceleration, Tolerance);
			Assert.AreEqual(entry.Deceleration.Value(), deceleration, Tolerance);
		}

		private static void TestAcceleration(AccelerationCurveData data)
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

		[TestCase]
		public void Declaration_WheelsForT1_Class2()
		{
			var dataProvider =
				JSONInputDataFactory.ReadJsonJob(@"TestData\Jobs\12t Delivery Truck.vecto") as IDeclarationInputDataProvider;
			var dataReader = new DeclarationModeVectoRunDataFactory(dataProvider, null);

			var runs = dataReader.NextRun().ToList();
			Assert.AreEqual(9, runs.Count);
			var withT1 = new[] { 6.0, 6.0, 4.5 };
			CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(withT1, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));

			var bodyOnly = new[] { 6.0, 6.0 };
			CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[6].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[7].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[8].VehicleData.AxleData.Select(a => a.Inertia.Value()));
		}

		[TestCase]
		public void Declaration_WheelsForT2_Class4()
		{
			var dataProvider =
				JSONInputDataFactory.ReadJsonJob(@"TestData\Jobs\Class4_40t_Long_Haul_Truck.vecto") as IDeclarationInputDataProvider;
			var dataReader = new DeclarationModeVectoRunDataFactory(dataProvider, null);

			var runs = dataReader.NextRun().ToList();
			Assert.AreEqual(9, runs.Count);
			var withT1 = new[] { 14.9, 14.9, 19.2, 19.2 };
			CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(withT1, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));

			var bodyOnly = new[] { 14.9, 14.9 };
			CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[6].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[7].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[8].VehicleData.AxleData.Select(a => a.Inertia.Value()));
		}

		[TestCase]
		public void Declaration_WheelsForDefault_Class5()
		{
			var dataProvider =
				JSONInputDataFactory.ReadJsonJob(@"TestData\Jobs\40t_Long_Haul_Truck.vecto") as IDeclarationInputDataProvider;
			var dataReader = new DeclarationModeVectoRunDataFactory(dataProvider, null);

			var runs = dataReader.NextRun().ToList();
			Assert.AreEqual(6, runs.Count);
			var withT1 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2 };
			CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(withT1, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));

			var bodyOnly = new[] { 14.9, 14.9, 19.2, 19.2, 19.2 };
			CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
			CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));
		}
	}
}