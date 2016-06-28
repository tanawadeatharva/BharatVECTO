using System;
using System.Globalization;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.ShiftStrategy
{
	[TestFixture]
	public class ShiftStrategyTest
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
		TestCase(75, 42.5, 4.5),
		TestCase(75, 42.5, 3.5),
		TestCase(75, 42.5, 2.1),
		TestCase(65, 42.5, 2.3),
		]
		public void Truck_Shifting_Test(double v1, double v2, double slope)
		{
			Assert.IsTrue(v1 > v2);

			var cycle = new[] {
				// <s>,<v>,<grad>,<stop>
				string.Format(CultureInfo.InvariantCulture, "  0,  {0}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1000, {1}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1100, {1},   0,  0", v1, v2, slope)
			};
			System.IO.Directory.CreateDirectory(string.Format(@"Shiftt_{0}_{1}", v1, v2, slope));
			var slopePrefix = "";
			if (!slope.IsEqual(0)) {
				slopePrefix = slope > 0 ? "uh_" : "dh_";
			}
			var modFile = string.Format(@"Truck_Shift_{0}_{1}_{3}{2:0.0}.vmod", v1, v2, Math.Abs(slope),
				slopePrefix);
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFile, gbxType: GearboxType.MT);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write(modFile);
		}
	}
}