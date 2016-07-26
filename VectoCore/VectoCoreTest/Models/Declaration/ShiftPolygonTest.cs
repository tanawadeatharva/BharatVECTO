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
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Point = TUGraz.VectoCommon.Utils.Point;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class ShiftPolygonTest
	{
		[TestCase]
		public void IntersectShiftLines1()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 8),
				new Point(18, 22)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var result = DeclarationData.Gearbox.IntersectShiftPolygon(upShift.ToList(), transformed.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}
		}

		[TestCase]
		public void IntersectShiftLines2()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 6),
				new Point(18, 14)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 7.6),
				new Point(20, 15.6),
			};

			var result = DeclarationData.Gearbox.IntersectShiftPolygon(upShift.ToList(), transformed.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}

			result = DeclarationData.Gearbox.IntersectShiftPolygon(transformed.ToList(), upShift.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}
		}

		[TestCase]
		public void IntersectShiftLines3()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 4),
				new Point(18, 22)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 7.6),
				new Point(13, 13),
				new Point(20, 20),
			};

			var result = DeclarationData.Gearbox.IntersectShiftPolygon(upShift.ToList(), transformed.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}

			result = DeclarationData.Gearbox.IntersectShiftPolygon(transformed.ToList(), upShift.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
			}
		}

		[TestCase]
		public void IntersectShiftLines4()
		{
			var upShift = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(20, 20),
			};

			var transformed = new[] {
				new Point(8, 0),
				new Point(8, 12),
				new Point(18, 16)
			};

			var expected = new[] {
				new Point(10, 0),
				new Point(10, 10),
				new Point(14.6666, 14.6666),
				new Point(20, 16.8),
			};

			var result = DeclarationData.Gearbox.IntersectShiftPolygon(upShift.ToList(), transformed.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}

			result = DeclarationData.Gearbox.IntersectShiftPolygon(transformed.ToList(), upShift.ToList());

			Assert.AreEqual(expected.Length, result.Count);

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase]
		public void ShiftPolygonFldMarginTest()
		{
			var engineFld = new[] {
				new Point(8, 10),
				new Point(9, 11),
				new Point(10, 11.5),
				new Point(11, 12),
				new Point(12, 12.5),
				new Point(13, 13),
				new Point(14, 13.5),
				new Point(15, 14),
				new Point(16, 14.5),
				new Point(17, 15),
				new Point(18, 16.5),
				new Point(19, 18),
				new Point(20, 19.5),
				new Point(21, 21),
				new Point(22, 22.5),
			};

			var expected = new[] {
				new Point(8, 9.8),
				new Point(9, 10.78),
				new Point(10, 11.27),
				new Point(11, 11.76),
				new Point(12, 12.25),
				new Point(13, 12.74),
				new Point(14, 13.23),
				new Point(15, 13.72),
				new Point(16, 14.21),
				new Point(17, 14.7),
				new Point(18, 16.17),
				new Point(19, 17.64),
				new Point(20, 19.11),
				new Point(21, 20.58),
				new Point(22, 22.05),
			};

			var result =
				DeclarationData.Gearbox.ShiftPolygonFldMargin(
					engineFld.Select(
						p =>
							new FullLoadCurve.FullLoadCurveEntry() {
								EngineSpeed = p.X.SI<PerSecond>(),
								TorqueFullLoad = p.Y.SI<NewtonMeter>()
							}).ToList(),
					23.SI<PerSecond>());

			foreach (var tuple in expected.Zip(result, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
			}
		}

		[TestCase]
		public void CorrectDownShiftByEngineFldTest()
		{
			var downshift = Edge.Create(new Point(10, 10), new Point(22, 20));
			var engineFldCorr = new[] {
				new Point(8, 9.8),
				new Point(9, 10.78),
				new Point(10, 11.27),
				new Point(11, 11.76),
				new Point(12, 12.25),
				new Point(13, 12.74),
				new Point(14, 13.23),
				new Point(15, 13.72),
				new Point(16, 14.21),
				new Point(17, 14.7),
				new Point(18, 16.17),
				new Point(19, 17.64),
				new Point(20, 19.11),
				new Point(21, 20.58),
				new Point(22, 22.05),
			};

			var corrected = DeclarationData.Gearbox.MoveDownshiftBelowFld(downshift, engineFldCorr, 20.SI<NewtonMeter>());

			Assert.AreEqual(10, corrected.P1.X, 1e-3);
			Assert.AreEqual(8.86666, corrected.P1.Y, 1e-3);
			Assert.AreEqual(23.36, corrected.P2.X, 1e-3);
			Assert.AreEqual(20, corrected.P2.Y, 1e-3);
		}

		[TestCase]
		public void CompueShiftPolygonDeclarationTest()
		{
			var engineFile = @"TestData\Components\40t_Long_Haul_Truck.veng";
			var gearboxFile = @"TestData\Components\40t_Long_Haul_Truck.vgbx";

			var rdyn = 0.4882675.SI<Meter>();
			var axlegearRatio = 2.59;

			var expectedDownshift = new[] {
				// Gear 1
				new[] {
					new Point(64.50736915, -352),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
				// Gear 2
				new[] {
					new Point(64.50736915, -352),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
				// Gear 3
				new[] {
					new Point(64.50736915, -352),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
			};
			var expectedUpshift = new[] {
				// Gear 1
				new[] {
					new Point(136.9338946, -352),
					new Point(136.9338946, 1281.30911),
					new Point(201.7326, 2427.6748),
				},
				// Gear 2
				new[] {
					new Point(136.9338946, -352),
					new Point(136.9338946, 1281.30911),
					new Point(203.9530, 2466.9558),
				},
				// Gear 3
				new[] {
					new Point(136.9338946, -352),
					new Point(136.9338946, 1281.30911),
					//new Point(153.606666, 1893.19273),
					new Point(201.3375, 2420.6842),
				},
			};

			var dao = new DeclarationDataAdapter();
			var engineData = dao.CreateEngineData(new JSONEngineDataV3(JSONInputDataFactory.ReadFile(engineFile), engineFile));

			var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);

			var shiftPolygons = new List<ShiftPolygon>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(i, engineData.FullLoadCurve, gearboxData.Gears,
					engineData, axlegearRatio, rdyn));
			}

			for (var i = 0; i < Math.Min(gearboxData.Gears.Count, expectedDownshift.Length); i++) {
				foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				}

				foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				}
			}

			Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);
		}

		[TestCase]
		public void CompueShiftPolygonDeclarationTestConfidentialEngine()
		{
			//var engineFldFile = @"E:\QUAM\Downloads\EngineFLD\Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";
			var engineFldFile = @"E:\QUAM\tmp\scania_fullload_shiftpolygon-test.csv";
			var gearboxFile = @"E:\QUAM\Downloads\TUG_dev_gbx\TUG_dev\GRS905R.vgbx";
			//@"TestData\Components\40t_Long_Haul_Truck.vgbx";

			if (!File.Exists(engineFldFile)) {
				Assert.Ignore("Confidential File not found. Test cannot run without file.");
			}

			var rdyn = 0.4882675.SI<Meter>();
			var axlegearRatio = 3.71; //2.31; // 3.71; //2.59;

			var engineData = new CombustionEngineData() {
				IdleSpeed = 509.RPMtoRad(),
				FullLoadCurve = EngineFullLoadCurve.ReadFromFile(engineFldFile, true)
			};
			engineData.FullLoadCurve.EngineData = engineData;

			var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);

			var shiftPolygons = new List<ShiftPolygon>();
			var downshiftTransformed = new List<List<Point>>();
			var downshiftOrig = new List<List<Point>>();
			var upshiftOrig = new List<List<Point>>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(i, engineData.FullLoadCurve, gearboxData.Gears,
					engineData, axlegearRatio, rdyn));
				List<Point> tmp1, tmp2, tmp3;
				ShiftPolygonComparison.ComputShiftPolygonPoints(i, engineData.FullLoadCurve, gearboxData.Gears,
					engineData, axlegearRatio, rdyn, out tmp1, out tmp2, out tmp3);
				upshiftOrig.Add(tmp1);
				downshiftTransformed.Add(tmp2);
				downshiftOrig.Add(tmp3);
			}

			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(gearboxFile), engineData.FullLoadCurve, shiftPolygons,
				"scania_fullload_shiftpolygon-test.png",
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / rdyn * axlegearRatio * gearboxData.Gears.Last().Ratio, upshiftOrig,
				downshiftTransformed, downshiftOrig);
		}
	}

	[TestFixture]
	public class ShiftPolygonComparison
	{
		const string BasePath = @"E:\QUAM\Workspace\Daten_INTERN\Testfahrzeuge\";

		[
			TestCase(@"class2_12t_baseline\175kW_Diesel_example.vfld", @"class2_12t_baseline\delivery_12t_example.vgbx", 0.421,
				4.18, 600),
			TestCase(@"class2_12t_iaxle_long\175kW_Diesel_example.vfld", @"class2_12t_iaxle_long\delivery_12t_example.vgbx",
				0.421, 2.85, 600),
			TestCase(@"class2_12t_iaxle_short\175kW_Diesel_example.vfld", @"class2_12t_iaxle_short\delivery_12t_example.vgbx",
				0.421, 5.33, 600),
			TestCase(@"class2_12t_Pmax_high\220kW_Diesel_example.vfld", @"class2_12t_Pmax_high\delivery_12t_example_220kW.vgbx",
				0.421, 4.18, 600),
			TestCase(@"class2_12t_Pmax_low\130kW_Diesel_example.vfld", @"class2_12t_Pmax_low\delivery_12t_example.vgbx", 0.421,
				4.18, 600),
			TestCase(@"class5_40t_baseline\12L-324kW.vfld", @"class5_40t_baseline\tractor_12gear_example.vgbx", 0.421, 2.64, 600),
			TestCase(@"class5_40t_iaxle_long\12L-324kW.vfld", @"class5_40t_iaxle_long\tractor_12gear_example.vgbx", 0.421, 2.31,
				600),
			TestCase(@"class5_40t_iaxle_short\12L-324kW.vfld", @"class5_40t_iaxle_short\tractor_12gear_example.vgbx", 0.421, 3.71,
				600),
			TestCase(@"class5_40t_Pmax_high\13-9-L-375kW.vfld", @"class5_40t_Pmax_high\tractor_12gear_example.vgbx", 0.421,
				2.64, 600),
			TestCase(@"class5_40t_Pmax_low\9-6-L_260kW.vfld", @"class5_40t_Pmax_low\tractor_12gear_example.vgbx", 0.421, 2.64,
				600),
		]
		public void ComputeShiftPolygon(string engineFldFile, string gearboxFile, double rdyn, double axlegearRatio,
			double idlingSpeed)
		{
			if (!Directory.Exists(BasePath)) {
				Assert.Ignore("Confidential File not found. Test cannot run without file.");
			}

			var engineData = new CombustionEngineData() {
				IdleSpeed = idlingSpeed.RPMtoRad(),
				FullLoadCurve = EngineFullLoadCurve.ReadFromFile(Path.Combine(BasePath, engineFldFile), true)
			};
			engineData.FullLoadCurve.EngineData = engineData;

			var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(Path.Combine(BasePath, gearboxFile)),
				Path.Combine(BasePath, gearboxFile));

			var shiftPolygons = new List<ShiftPolygon>();
			var downshiftTransformed = new List<List<Point>>();
			var upshiftOrig = new List<List<Point>>();
			for (var i = 0; i < gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(
					DeclarationData.Gearbox.ComputeShiftPolygon(i, engineData.FullLoadCurve, gearboxData.Gears,
						engineData, axlegearRatio, rdyn.SI<Meter>())
					);
				List<Point> tmp1, tmp2, tmp3;
				ComputShiftPolygonPoints(i, engineData.FullLoadCurve, gearboxData.Gears,
					engineData, axlegearRatio, rdyn.SI<Meter>(), out tmp1, out tmp2, out tmp3);
				upshiftOrig.Add(tmp1);
				downshiftTransformed.Add(tmp2);
			}

			var imageFile = Path.GetDirectoryName(gearboxFile) + "_" + Path.GetFileNameWithoutExtension(gearboxFile) + "_" +
							Path.GetFileNameWithoutExtension(engineFldFile) +
							".png";
			ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(gearboxFile), engineData.FullLoadCurve, shiftPolygons,
				imageFile,
				DeclarationData.Gearbox.TruckMaxAllowedSpeed / rdyn.SI<Meter>() * axlegearRatio * gearboxData.Gears.Last().Ratio,
				upshiftOrig, downshiftTransformed);
		}

		public static void ComputShiftPolygonPoints(int gear, FullLoadCurve fullLoadCurve,
			IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius,
			out List<Point> upshiftOrig, out List<Point> downshiftTransformed, out List<Point> downshiftOrig)
		{
			var engineSpeed85kmhLastGear = DeclarationData.Gearbox.TruckMaxAllowedSpeed / dynamicTyreRadius * axlegearRatio *
											gears[gears.Count - 1].Ratio;
			//var engineSpeed85kmhSecondToLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 2], axlegearRatio,
			//	dynamicTyreRadius, engine);

			var nVHigh = VectoMath.Min(engineSpeed85kmhLastGear, engine.FullLoadCurve.RatedSpeed);

			var diffRatio = gears[gears.Count - 2].Ratio / gears[gears.Count - 1].Ratio - 1;

			var maxDragTorque = engine.FullLoadCurve.MaxDragTorque * 1.1;

			var p1 = new Point(engine.IdleSpeed.Value() / 2, 0);
			var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
			var p3 = new Point(nVHigh.Value() * 0.9,
				engine.FullLoadCurve.FullLoadStationaryTorque(nVHigh * 0.9).Value());

			var p4 =
				new Point((nVHigh * (1 + diffRatio / 3)).Value(), 0);
			var p5 = new Point(engine.FullLoadCurve.N95hSpeed.Value(), engine.FullLoadCurve.MaxTorque.Value());

			var p6 = new Point(p2.X, VectoMath.Interpolate(p1, p3, p2.X));
			var p7 = new Point(p4.X, VectoMath.Interpolate(p2, p5, p4.X));

			downshiftOrig = new[] { p2, p6, p3 }.ToList();

			//var fldMargin = ShiftPolygonFldMargin(fullLoadCurve.FullLoadEntries, nVHigh * 0.95);
			//var downshiftCorr = MoveDownshiftBelowFld(Edge.Create(p6, p3), fldMargin, 1.1 * fullLoadCurve.MaxTorque);
			upshiftOrig = new[] { p4, p7, p5 }.ToList();
			downshiftTransformed = new List<Point>();

			if (gear >= gears.Count - 1) {
				return;
			}
			var gearRatio = gears[gear].Ratio / gears[gear + 1].Ratio;
			var rpmMarginFactor = 1 + DeclarationData.Gearbox.ShiftPolygonRPMMargin / 100.0;

			var p2p = new Point(p2.X * gearRatio * rpmMarginFactor, p2.Y / gearRatio);
			var p3p = new Point(p3.X * gearRatio * rpmMarginFactor, p3.Y / gearRatio);
			var p6p = new Point(p6.X * gearRatio * rpmMarginFactor, p6.Y / gearRatio);
			var edgeP6pP3p = new Edge(p6p, p3p);
			var p3pExt = new Point((1.1 * p5.Y - edgeP6pP3p.OffsetXY) / edgeP6pP3p.SlopeXY, 1.1 * p5.Y);

			downshiftTransformed = new[] { p2p, p6p, p3pExt }.ToList();
		}
	}
}