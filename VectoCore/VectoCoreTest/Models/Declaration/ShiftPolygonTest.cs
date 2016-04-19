using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Point = TUGraz.VectoCore.Utils.Point;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestClass]
	public class ShiftPolygonTest
	{
		[TestMethod]
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

		[TestMethod]
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


		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
		public void CompueShiftPolygonDeclarationTest()
		{
			var engineFile = @"TestData\Components\40t_Long_Haul_Truck.veng";
			var gearboxFile = @"TestData\Components\40t_Long_Haul_Truck.vgbx";

			var rdyn = 0.4882675.SI<Meter>();
			var axlegearRatio = 2.59;

			var expectedDownshift = new[] {
				// Gear 1
				new[] {
					new Point(64.50736915, 0),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
				// Gear 2
				new[] {
					new Point(64.50736915, 0),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
				// Gear 3
				new[] {
					new Point(64.50736915, 0),
					new Point(64.50736915, 970.37),
					new Point(121.059, 2530),
				},
			};
			var expectedUpshift = new[] {
				// Gear 1
				new[] {
					new Point(136.9338946, 0),
					new Point(136.9338946, 1419.679281),
					new Point(154.645394, 1766.853864),
					new Point(203.9530637, 2530),
				},
				// Gear 2
				new[] {
					new Point(136.9338946, 0),
					new Point(136.9338946, 1419.679281),
					new Point(162.34804, 1917.838528),
					new Point(201.3375424, 2530),
				},
				// Gear 3
				new[] {
					new Point(136.9338946, 0),
					new Point(136.9338946, 1419.679281),
					new Point(192.102071, 2501.066),
				},
			};

			var dao = new DeclarationDataAdapter();
			var engineData = dao.CreateEngineData(new JSONEngineDataV3(JSONInputDataFactory.ReadFile(engineFile), engineFile));

			var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);

			var shiftPolygons = new List<ShiftPolygon>();
			for (var i = 1; i <= gearboxData.Gears.Count; i++) {
				shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(i, engineData.FullLoadCurve, gearboxData.Gears,
					engineData, axlegearRatio, rdyn));
			}

			for (var i = 0; i < Math.Min(gearboxData.Gears.Count, expectedDownshift.Length); i++) {
				foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i, tuple);
				}

				foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
					Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i, tuple);
					Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i, tuple);
				}
			}

			Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);
		}


		//[TestMethod]
		//public void CompueShiftPolygonDeclarationTestConfidentialEngine()
		//{
		//	var engineFldFile = @"E:\QUAM\Downloads\EngineFLD\Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";
		//	var gearboxFile = @"TestData\Components\40t_Long_Haul_Truck.vgbx";

		//	var rdyn = 0.4882675.SI<Meter>();
		//	var axlegearRatio = 2.59;

		//	var engineData = new CombustionEngineData() {
		//		IdleSpeed = 509.RPMtoRad(),
		//		FullLoadCurve = EngineFullLoadCurve.ReadFromFile(engineFldFile, true)
		//	};

		//	var gearboxData = new JSONGearboxDataV5(JSONInputDataFactory.ReadFile(gearboxFile), gearboxFile);

		//	var shiftPolygons = new List<ShiftPolygon>();
		//	for (var i = 1; i <= gearboxData.Gears.Count; i++) {
		//		shiftPolygons.Add(DeclarationData.Gearbox.ComputeShiftPolygon(i, engineData.FullLoadCurve, gearboxData.Gears,
		//			engineData, axlegearRatio, rdyn));
		//	}
		//}
	}
}