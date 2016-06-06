using System;
using System.Globalization;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.ModelbasedTests.DriverStrategy
{
	[TestFixture]
	public class CoastingTests
	{
		[TestFixtureSetUp]
		public void DisableLogging()
		{
			//LogManager.DisableLogging();
#if TRACE
			GraphWriter.Enable();
#else
			GraphWriter.Disable();
#endif

			GraphWriter.Xfields = new[] { ModalResultField.dist };

			GraphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_eng_avg, ModalResultField.Gear,
				ModalResultField.P_eng_out, /*ModalResultField.T_eng_fcmap, */ ModalResultField.FCMap,
			};
			GraphWriter.PlotDrivingMode = true;
			GraphWriter.Series1Label = "Vecto 3";
		}

		[Test,
		TestCase(60, 20, 0),
		TestCase(60, 20, 0.6),
		TestCase(60, 20, 1.4),
		TestCase(60, 20, 2.7),
		TestCase(60, 20, 3.3),
		TestCase(60, 20, 3.7),
		TestCase(60, 35, 5.3),
		TestCase(50, 47.5, -2.1),
		TestCase(65, 62.5, -0.8),
		TestCase(60, 50, 5.6),
		TestCase(80, 40, 4.0),
		TestCase(65, 60, 4.7),
		TestCase(70, 62.5, 4.6),
		TestCase(75, 65, 4.5),
		]
		public void Truck_Coasting_Test(double v1, double v2, double slope)
		{
			Assert.IsTrue(v1 > v2);

			var cycle = new[] {
				// <s>,<v>,<grad>,<stop>
				string.Format(CultureInfo.InvariantCulture, "  0,  {0}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1000, {1}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1100, {1},   0,  0", v1, v2, slope)
			};
			System.IO.Directory.CreateDirectory(string.Format(@"Coast_{0}_{1}", v1, v2, slope));
			var slopePrefix = "";
			if (!slope.IsEqual(0)) {
				slopePrefix = slope > 0 ? "uh_" : "dh_";
			}
			var modFile = string.Format(@"Coast_{0}_{1}\Truck_Coast_{0}_{1}_{3}{2:0.0}.vmod", v1, v2, Math.Abs(slope),
				slopePrefix);
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFile);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write(modFile);
		}

		[Ignore, Test,
		TestCase(40),
		TestCase(45),
		TestCase(50),
		TestCase(55),
		TestCase(60),
		TestCase(65),
		TestCase(70),
		TestCase(75),
		TestCase(80),
		]
		public void Truck_Coasting_Variability_Test(double v1)
		{
			const double vStep = 2.5;
			const double vMin = 40;
			Assert.IsTrue(vMin - vStep > 0);
			for (var v2 = vMin; v2 <= v1 - vStep; v2 += vStep) {
				for (var slope = -6.0; slope <= 6; slope += 0.1) {
					Truck_Coasting_Test(v1, v2, slope);
				}
			}
		}
	}
}